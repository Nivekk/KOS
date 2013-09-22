using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace kOS {
    public class TelnetServer {
        const byte ESCAPE = 27;
        const byte LEFT_SQUARE_BRACKET = 91;
        const byte ESC_LEFT = (byte)'D';
        const byte ESC_RIGHT = (byte)'C';
        const byte ESC_UP = (byte)'A';
        const byte ESC_DOWN = (byte)'B';
        const char KEYBOARD_LEFT = (char)37;
        const char KEYBOARD_RIGHT = (char)39;
        const char KEYBOARD_UP = (char)38;
        const char KEYBOARD_DOWN = (char)40;


        private TcpListener _listener;
        private Thread _listenThread;
        private List<TcpClient> _clients;
        private kOS.ImmediateMode _ec;
        private List<Thread> _clientThreads;
        private Int32 _port;
        public Int32 Port { get { return _port; } }


        private volatile char[,] _buffer;

        private volatile bool _shouldStop;

        public TelnetServer(kOS.ImmediateMode ec, int Port) {
            _port = Port;
            _shouldStop = false;
            _clientThreads = new List<Thread>();
            _ec = ec;
            _listener = new TcpListener(IPAddress.Any, Port);
            _listenThread = new Thread(new ThreadStart(ListenForClients));
            _listenThread.IsBackground = true;
            _listenThread.Start();
            _clients = new List<TcpClient>();
        }

        private void ListenForClients() {
            _listener.Start();

            while (!_shouldStop) {
                if (_listener.Pending()) {
                    TcpClient client = _listener.AcceptTcpClient();

                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.Start(client);
                    _clientThreads.Add(clientThread);
                    _clients.Add(client);
                } else {
                    Thread.Sleep(50);
                }
            }
        }

        public void Stop() {
            _shouldStop = true;
        }

        public void BufferAt(Char c, int x, int y) {
            switch ((Int32)c) {
                case 0:
                    this._buffer[x, y] = ' ';
                    break;
                default:
                    this._buffer[x, y] = c;
                    break;
            }
        }

        public void ShowBuffer() {
            var output = "";
            for (var y = 0; y < _buffer.GetLength(1); y++) {
                output += "\x1b[" + (y + 1).ToString() + ";1f";
                for (var x = 0; x < _buffer.GetLength(0); x++) {
                    output += _buffer[x, y];
                }
            }
            Write(output);
        }

        public void WriteAt(Char c, int x, int y) {
            if (_buffer == null) {
                _buffer = new char[50, 36];
            }
            if (this._buffer[x, y] != c) {
                this._buffer[x, y] = c;
                switch ((Int32)c) {
                    case 0:
                        Write("\x1b[" + (y + 1).ToString() + ";" + (x + 1).ToString() + "f ");
                        break;
                    default:
                        Write("\x1b[" + (y + 1).ToString() + ";" + (x + 1).ToString() + "f" + c.ToString());
                        break;
                }
            }
        }

        public void WriteBuffer() {
            foreach (var client in _clients.ToList()) {
                if (!client.Connected) {
                    _clients.Remove(client);
                } else {
                    SendInitialScreen(client.GetStream());
                }
            }
        }

        public void LocateCursor(int x, int y) {
            Write("\x1b[" + (y + 1).ToString() + ";" + (x + 1).ToString() + "f");
        }

        public void Write(String message) {
            ASCIIEncoding encoder = new ASCIIEncoding();
            foreach (var client in _clients.ToList()) {
                if (!client.Connected) {
                    _clients.Remove(client);
                } else {
                    NetworkStream clientStream = client.GetStream();
                    var bytes = encoder.GetBytes(message);
                    clientStream.Write(bytes, 0, bytes.Length);
                    clientStream.Flush();
                }
            }
        }

        private void SendCommands(NetworkStream clientStream) {
            var m = new List<Byte>();
            m.Add((Byte)Commands.IAC);
            m.Add((Byte)Commands.Dont);
            m.Add((Byte)Options.Echo);

            m.Add((Byte)Commands.IAC);
            m.Add((Byte)Commands.Will);
            m.Add((Byte)Options.Echo);

            m.Add((Byte)Commands.IAC);
            m.Add((Byte)Commands.Do);
            m.Add((Byte)Options.SuppressGoAhead);

            m.Add((Byte)Commands.IAC);
            m.Add((Byte)Commands.Will);
            m.Add((Byte)Options.SuppressGoAhead);




            clientStream.Write(m.Select(x => x).ToArray(), 0, m.Count);
            clientStream.Flush();
        }

        private void SendInitialScreen(NetworkStream clientStream) {
            Write("\x1b[32m\x1b[1;1f");

            var m = new List<byte>();
            var buffer = _ec.GetBuffer();

            for (var y = 0; y < buffer.GetLength(1); y++) {
                for (var x = 0; x < buffer.GetLength(0); x++) {
                    var c = buffer[x, y]; //
                    if ((Int32)c == 0)
                        c = ' ';
                    m.Add((Byte)c);
                }
                if (y < buffer.GetLength(1) - 1) {
                    m.Add(13);
                    m.Add(10);
                }
            }

            clientStream.Write(m.ToArray(), 0, m.Count);
            clientStream.Flush();


            Write("\x1b[" + (_ec.GetCursorX() + 1) + ";" + (_ec.GetCursorY() + 1) + "f");
        }

        private void HandleClientComm(object client) {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            SendCommands(clientStream);
            Thread.Sleep(100);
            SendInitialScreen(clientStream);

            byte[] message = new byte[4096];
            int bytesRead;

            while (!_shouldStop) {
                if (tcpClient.Available > 0) {
                    bytesRead = 0;

                    try {
                        bytesRead = clientStream.Read(message, 0, 4096);
                    } catch {
                        break; //Socket error
                    }

                    if (bytesRead == 0) {
                        break;  //Client has disconnected from the server
                    }


                    for (var i = 0; i < bytesRead; i++) {
                        var b = message[i];
                        if (b == 255) {
                            //Hide commands
                            if (bytesRead > i + 2) {
                                var c = message[++i];
                                var o = message[++i];
                            }
                        } else if (b == 27) {
                            //Escape Sequence
                            if (bytesRead > i + 2) {
                                var bracket = message[++i];
                                var k = message[++i];
                                switch (k) {
                                    case ESC_LEFT: _ec.SpecialKey(kOSKeys.LEFT); break;
                                    case ESC_RIGHT: _ec.SpecialKey(kOSKeys.RIGHT); break;
                                    case ESC_UP: _ec.SpecialKey(kOSKeys.UP); break;
                                    case ESC_DOWN: _ec.SpecialKey(kOSKeys.DOWN); break;
                                    case (Byte)'1':
                                        var k2 = message[++i];
                                        switch ((char)k2) {
                                            case '1': _ec.SpecialKey(kOSKeys.F1); break;
                                            case '2': _ec.SpecialKey(kOSKeys.F2); break;
                                            case '3': _ec.SpecialKey(kOSKeys.F3); break;
                                            case '4': _ec.SpecialKey(kOSKeys.F4); break;
                                            case '5': _ec.SpecialKey(kOSKeys.F5); break;
                                            case '7': _ec.SpecialKey(kOSKeys.F6); break;
                                            case '8': _ec.SpecialKey(kOSKeys.F7); break;
                                            case '9': _ec.SpecialKey(kOSKeys.F8); break;
                                            case '~': _ec.SpecialKey(kOSKeys.HOME); break;

                                        }
                                        if(k2 != '~')
                                            i++;
                                        break;
                                    case (Byte)'2':
                                        var k3 = message[++i];
                                        switch ((char)k3) {
                                            case '0': _ec.SpecialKey(kOSKeys.F9); break;
                                            case '1': _ec.SpecialKey(kOSKeys.F10); break;
                                            case '3': _ec.SpecialKey(kOSKeys.F11); break;
                                            case '4': _ec.SpecialKey(kOSKeys.F12); break;
                                        }
                                        if (k3 != '~')
                                            i++;
                                        break;
                                    case (Byte)'3':
                                        var k4 = message[++i];
                                        switch ((char)k4) {
                                            case '~': _ec.SpecialKey(kOSKeys.DEL); break;
                                        }
                                        if (k4 != '~')
                                            i++;
                                        break;
                                    case (Byte)'4':
                                        var k5 = message[++i];
                                        switch ((char)k5) {
                                            case '~': _ec.SpecialKey(kOSKeys.END); break;
                                        }
                                        if (k5 != '~')
                                            i++;
                                        break;
                                }
                            }
                        } else {
                            switch (b) {
                                case 0:
                                case 10:
                                    break;
                                case 127:
                                    _ec.Type((Char)8);
                                    break;
                                default:
                                    _ec.Type((Char)b);
                                    break;
                            }
                        }
                    }
                } else {
                    Thread.Sleep(50);
                }
            }
        }

        enum Commands {
            EndOfSubNegotiation = 240,
            NoOperation = 241,
            DataMark = 242,
            Break = 243,
            GoAhead = 249,
            SubNegotian = 250,
            Will = 251,
            Wont = 252,
            Do = 253,
            Dont = 254,
            IAC = 255
        }

        enum Options {
            NAWS = 31,
            TerminalSpeed = 32,
            TerminalType = 24,
            NewEnviroment = 39,
            Echo = 1,
            SuppressGoAhead = 3
        }
    }
}
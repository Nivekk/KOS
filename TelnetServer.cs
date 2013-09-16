using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace kOS {
    public class TelnetServer {
        private TcpListener _listener;
        private Thread _listenThread;
        private List<TcpClient> _clients;
        private kOS.ImmediateMode _ec;
        private List<Thread> _clientThreads;
        private Int32 _port;
        public Int32 Port { get { return _port; } }


        private volatile bool _shouldStop;

        int maxx = 0;
        int maxy = 0;

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

        public void WriteAt(Char c, int x, int y) {
            if (c == 8) {
                Write("\x1b[" + (y + 1).ToString() + ";" + (x + 1).ToString() + "f " + c.ToString());
            } else {
                Write("\x1b[" + (y + 1).ToString() + ";" + (x + 1).ToString() + "f" + c.ToString());
            }
        }

        public void Write(String message) {
            ASCIIEncoding encoder = new ASCIIEncoding();
            foreach (var client in _clients) {
                NetworkStream clientStream = client.GetStream();
                var bytes = encoder.GetBytes(message);
                clientStream.Write(bytes, 0, bytes.Length);
                clientStream.Flush();
            }
        }

        private void SendCommands(NetworkStream clientStream) {
            var m = new List<byte>();
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


            clientStream.Write(m.ToArray(), 0, m.Count);
            clientStream.Flush();
        }

        private void SendInitialScreen(NetworkStream clientStream) {
            Write("\x1b[1;1f");

            var m = new List<byte>();
            var buffer = _ec.GetBuffer();

            for (var y = 0; y < buffer.GetLength(1); y++) {
                for (var x = 0; x < buffer.GetLength(0); x++) {
                    m.Add((Byte)buffer[x, y]);
                }
                m.Add(13);
                m.Add(10);
            }
            clientStream.Write(m.ToArray(), 0, m.Count);
            clientStream.Flush();
        }

        private void HandleClientComm(object client) {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            SendCommands(clientStream);
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
                                i += 2;
                            }
                        } else {
                            if (b != 0) {
                                _ec.Type((Char)b);
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
            NegotiateWindowSize = 31,
            TerminalSpeed = 32,
            TerminalType = 24,
            NewEnviroment = 39,
            Echo = 1,
            SuppressGoAhead = 3
        }
    }
}
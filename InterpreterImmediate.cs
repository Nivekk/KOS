using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    public class ImmediateMode : ExecutionContext
    {
        int BufferWidth { get { return buffer.GetLength(1); } }
        int BufferHeight { get { return buffer.GetLength(0); } }
        private int cursor = 0;
        private int baseLineY = 0;
        private static int CMD_BACKLOG = 20;
        private List<String> previousCommands = new List<String>();
        private int prevCmdIndex = -1;
        private String inputBuffer = "";
        private String commandBuffer = "";
        private int CursorX = 0;
        private int CursorY = 0;
        private new Queue<Command> Queue = new Queue<Command>();

        private new char[,] buffer = new char[ROWS, COLUMNS];

        public ImmediateMode(ExecutionContext parent) : base(parent) 
        {
            StdOut("kOS Operating System Build " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision);
            StdOut("KerboScript v0.8");
            StdOut("");
            StdOut("Proceed.");
        }

        public void Add(string cmdString)
        {
            commandBuffer += cmdString;
            string nextCmd;

            while (parseNext(ref commandBuffer, out nextCmd))
            {
                try
                {
                    Command cmd = Command.Get(nextCmd, this);
                    Queue.Enqueue(cmd);
                }
                catch (kOSException e)
                {
                    StdOut(e.Message);
                    Queue.Clear(); // HALT!!
                }
            }
        }

        public override int GetCursorX()
        {
            return ChildContext != null ? ChildContext.GetCursorX() : CursorX;
        }

        public override int GetCursorY()
        {
            return ChildContext != null ? ChildContext.GetCursorY() : CursorY;
        }

        public override bool Type(char ch)
        {
            if (base.Type(ch)) return true;

            switch (ch)
            {
                case (char)8:
                    if (cursor > 0)
                    {
                        inputBuffer = inputBuffer.Remove(cursor - 1, 1);
                        cursor--;
                    }
                    break;

                case (char)13:
                    Enter();
                    break;

                default:
                    inputBuffer = inputBuffer.Insert(cursor, new String(ch, 1));
                    cursor++;
                    break;
            }

            UpdateCursorXY();
            return true;
        }

        public override char[,] GetBuffer()
        {
            var childBuffer = (ChildContext != null) ? ChildContext.GetBuffer() : null;

            return childBuffer != null ? childBuffer : buffer;
        }

        public void UpdateCursorXY()
        {
            CursorX = cursor % BufferWidth;
            CursorY = (cursor / BufferWidth) + baseLineY;
        }

        public void ShiftUp()
        {
            Array.Copy(buffer, BufferWidth, buffer, 0, buffer.Length - BufferWidth);
            Array.Clear(buffer, buffer.Length - BufferWidth, BufferWidth);

            if (baseLineY > 0) baseLineY--;

            UpdateCursorXY();
        }

        public override void Put(string text, int x, int y)
        {
            int count = Math.Min(text.Length, BufferWidth - x);
            int offset = y * BufferWidth + x;
            Buffer.BlockCopy(text.ToCharArray(), 0, buffer, offset * sizeof(char), count * sizeof(char));
        }

        public override void StdOut(string line)
        {
            int linesWritten = WriteLine(line);
            baseLineY += linesWritten;
            UpdateCursorXY();
        }

        public void ClearScreen()
        {
            baseLineY = 0;
            cursor = 0;

            Array.Clear(buffer, 0, buffer.Length);

            UpdateCursorXY();
        }

        public int WriteLine(string line)
        {
            int position = 0;
            int lineOffset = 0; 
            int lineCount = (line.Length / BufferWidth) + 1;
            char[] lineArray = line.ToCharArray();

            while (baseLineY + lineCount > BufferHeight)
            {
                ShiftUp();
            }

            // Clear the lines we are going to write the string to
            Array.Clear(buffer, baseLineY * BufferWidth, BufferWidth * lineCount);

            do
            {
                int count = Math.Min(lineArray.Length - position, BufferWidth);
                Buffer.BlockCopy(lineArray, position * sizeof(char), buffer, (baseLineY + lineOffset) * BufferWidth * sizeof(char), count * sizeof(char));

                position += count;
                lineOffset++;
            } while (position < line.Length);

            return lineCount;
        }

        public override void Update(float time)
        {
            if (ChildContext == null)
            {
                if (Queue.Count > 0)
                {
                    var currentCmd = Queue.Dequeue();

                    try
                    {
                        Push(currentCmd);
                        currentCmd.Evaluate();
                    }
                    catch (kOSException e)
                    {
                        StdOut(e.Message);
                        Queue.Clear();          // Halt all pending instructions
                        ChildContext = null;    //
                    }
                    catch (Exception e)
                    {
                        // Non-kos exception! This is a bug, but no reason to kill the OS
                        StdOut("Flagrant error occured, logging");
                        UnityEngine.Debug.Log("Immediate mode error");
                        UnityEngine.Debug.Log(e);
                        Queue.Clear();
                        ChildContext = null;
                    }
                }
                else
                {
                    WriteLine(inputBuffer);
                }
            }

            try
            {
                base.Update(time);
            }
            catch (kOSException e)
            {
                StdOut(e.Message);
                ChildContext = null;
                Queue.Clear();          // Halt all pending instructions
            }
            catch (Exception e)
            {
                // Non-kos exception! This is a bug, but no reason to kill the OS
                StdOut("Flagrant error occured, logging");
                UnityEngine.Debug.Log("Immediate mode error");
                UnityEngine.Debug.Log(e);
                Queue.Clear();
                ChildContext = null;
            }
        }

        private void Enter()
        {
            baseLineY += WriteLine(inputBuffer);

            while (baseLineY > BufferHeight - 1) ShiftUp();

            
            previousCommands.Add(inputBuffer);
            if (previousCommands.Count > CMD_BACKLOG)
            {
                int overflow = previousCommands.Count - CMD_BACKLOG;
                previousCommands.RemoveRange(0, overflow);
            }

            prevCmdIndex = -1;

            Add(inputBuffer += "\n");

            inputBuffer = "";
            cursor = 0;

            UpdateCursorXY();
        }

        public override void SendMessage(SystemMessage message)
        {
            switch (message)
            {
                case SystemMessage.CLEARSCREEN:
                    ClearScreen();
                    break;

                default:
                    base.SendMessage(message);
                    break;
            }
        }

        public void PreviousCommand(int direction)
        {
            if (previousCommands.Count == 0) return;

            inputBuffer = "";
            cursor = 0;

            prevCmdIndex += direction;
            if (prevCmdIndex <= -1)
            {
                inputBuffer = "";
                prevCmdIndex = -1;
                cursor = 0;
                UpdateCursorXY();
                return;
            }
            if (prevCmdIndex > previousCommands.Count-1)
            {
                prevCmdIndex = previousCommands.Count - 1;
            }
            
            inputBuffer = previousCommands[(previousCommands.Count-1) - prevCmdIndex];
            cursor = inputBuffer.Length;
            UpdateCursorXY();
        }

        public override bool SpecialKey(kOSKeys key)
        {
            if (base.SpecialKey(key)) return true;

            switch (key)
            {
                case kOSKeys.UP:
                    PreviousCommand(1);
                    return true;

                case kOSKeys.DOWN:
                    PreviousCommand(-1);
                    return true;
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    public class InterpreterBootup : ExecutionContext
    {
        int BufferWidth { get { return buffer.GetLength(1); } }
        int BufferHeight { get { return buffer.GetLength(0); } }
        private float bootTime = 0;
        private float animationTime = 0;
        private new char[,] buffer = new char[ROWS, COLUMNS];

        public InterpreterBootup(ExecutionContext parent)
            : base(parent) 
        {
            PrintAt("BOOTING UP...", 22, 20);

            State = ExecutionState.WAIT;
        }

        public override char[,] GetBuffer()
        {
            return buffer;
        }

        public void PrintAt(String text, int x, int y)
        {
            int count = Math.Min(text.Length, BufferWidth - x);
            int offset = y * BufferWidth + x;
            Buffer.BlockCopy(text.ToCharArray(), 0, buffer, offset * sizeof(char), count * sizeof(char));
        }

        public override void Update(float time)
        {
            bootTime += time;
            animationTime += time;

            if (animationTime > 0.5f) animationTime -= 0.5f;
            ShowAnimationFrame(animationTime > 0.25f ? 1 : 0);

            if (bootTime > 2.25f)
            {
                State = ExecutionState.DONE;
            }
        }

        
        public void ShowAnimationFrame(int frame)
        {
            var tX = 25;
            var tY = 14;

            for (var y = 0; y < 6; y++)
            {
                for (var x = 0; x < 4; x++)
                {
                    var sY = y + 11;
                    var sX = x + (frame * 4);

                    char c = (char)(sY * 16 + sX);

                    buffer[tY + y, tX + x] = c;
                }
            }
        }
    }
}

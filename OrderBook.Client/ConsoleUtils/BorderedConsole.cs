using System;
using System.Collections.Generic;
using System.Drawing;

namespace OrderBook.Client.ConsoleUtils
{
    public class BorderedConsole
    {
        public Point Position { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public IList<string> Lines { get; set; }

        public BorderedConsole(int width, int height)
        {
            Width = width;
            Height = height;
            Lines = new List<string>();
        }

        internal void Draw(char[,] outputBuffer)
        {
            DrawBorder(outputBuffer);
            DrawContent(outputBuffer);
        }

        private void DrawBorder(char[,] outputBuffer)
        {
            // Draw corners
            outputBuffer[Position.X, Position.Y] = '╔';
            outputBuffer[Position.X, Position.Y + Height - 1] = '╚';
            outputBuffer[Position.X + Width - 1, Position.Y] = '╗';
            outputBuffer[Position.X + Width - 1, Position.Y + Height - 1] = '╝';

            // Vertical borders
            for (int i = Position.Y + 1; i < Position.Y + Height - 1; i++)
            {
                outputBuffer[Position.X, i] = '║';
                outputBuffer[Position.X + Width - 1, i] = '║';
            }

            // Horizontal borders
            for (int i = Position.X + 1; i < Position.X + Width - 1; i++)
            {
                outputBuffer[i, Position.Y] = '═';
                outputBuffer[i, Position.Y + Height - 1] = '═';
            }
        }

        private void DrawContent(char[,] outputBuffer)
        {
            if (Lines.Count < 1)
            {
                return;
            }

            for (int i = 0; i < Lines.Count; i++)
            {
                DrawString(outputBuffer, Lines[i], i);
            }
        }

        private void DrawString(char[,] outputBuffer, string text, int indLine)
        {
            var chars = text.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                outputBuffer[Position.X + 2 + i, Position.Y + indLine + 1] = chars[i];
            }
        }
    }
}

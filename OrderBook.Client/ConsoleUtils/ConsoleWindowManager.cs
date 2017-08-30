using System;
using System.Collections.Generic;
using System.Drawing;

namespace OrderBook.Client.ConsoleUtils
{
    public static class ConsoleWindowManager
    {
        private const int WIDTH = 120;
        private const int HEIGHT = 30;
        private static char[,] OutputBuffer;
        private static IList<BorderedConsole> ConsoleRenderStack;

        public static void Initialize()
        {
            OutputBuffer = new char[WIDTH, HEIGHT];
            ConsoleRenderStack = new List<BorderedConsole>();

            var helpConsole = new BorderedConsole(84, 26);
            var ordersConsole = new BorderedConsole(36, 26);
            var commandsConsole = new BorderedConsole(120, 4);

            helpConsole.Position = new Point(0, 0);
            ordersConsole.Position = new Point(84, 0);
            commandsConsole.Position = new Point(0, 26);

            helpConsole.Lines.Add("1: SELL 100 TEST @ 10");
            helpConsole.Lines.Add("2: BUY  100 TEST @ 10");
            helpConsole.Lines.Add("3: SELL  250 TEST @ 10");
            helpConsole.Lines.Add("4: BUY  250 TEST @ 10");

            ordersConsole.Lines.Add("Accepted orders:");
            ordersConsole.Lines.Add("");
            ordersConsole.Lines.Add("Executed orders:");

            commandsConsole.Lines.Add("Enter your choice:");

            ConsoleRenderStack.Add(helpConsole);
            ConsoleRenderStack.Add(ordersConsole);
            ConsoleRenderStack.Add(commandsConsole);
        }

        private static void RenderToOutputBuffer()
        {
            foreach (var console in ConsoleRenderStack)
            {
                console.Draw(OutputBuffer);
            }
        }

        public static void Draw()
        {
            RenderToOutputBuffer();
            Console.Clear();

            for (int j = 0; j < HEIGHT; j++)
            {
                for (int i = 0; i < WIDTH; i++)
                {
                    Console.Write(OutputBuffer[i, j]);
                }
            }

            Console.SetWindowPosition(0, 0);
            Console.SetCursorPosition(2, 28);
        }
    }
}

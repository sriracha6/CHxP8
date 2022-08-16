using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = Colorful.Console;
using System.Drawing;

namespace CHxP8
{
    namespace Debugger
    {
        public static class Renderer
        {
            public static void RenderBottomBar(int opcode, string status, int PC)
            {
                string leftside = $"{PC} -> {opcode.ToString("X4")} : {DebugInstructions.GetInstruction(opcode)}";
                string div = new string(' ', 120 - (leftside.Length + status.Length));

                Console.BackgroundColor = Color.MidnightBlue;
                Console.SetCursorPosition(0, 32); // bruh
                Console.Write(leftside + div);
                Console.Write(status, Color.Gold);
                Console.BackgroundColor = Color.Black;
            }

            public static void RefreshGUI(int opcode, int programCounter, string status)
            {
                if (Console.WindowWidth != 120 || Console.WindowHeight != 32)    // todo: make height update with game
                    Console.SetWindowSize(120, 32);
                RenderBottomBar(opcode, status, programCounter);
            }
        }
    }
}
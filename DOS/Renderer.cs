using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Console = Colorful.Console;

namespace CHxP8.Emulator
{
    public static class Renderer
    {
        public static bool[,] screen = new bool[CHIP8.GFX_cols, CHIP8.GFX_rows];
        public static bool drawFlag = false;
        public static List<int> updatedRows = new List<int>();

        public static void RefreshSize()
        {
            if (Console.WindowWidth != CHIP8.GFX_cols || Console.WindowHeight != CHIP8.GFX_rows)
            {
                Console.SetWindowSize(CHIP8.GFX_cols, CHIP8.GFX_rows);
                Console.SetBufferSize(CHIP8.GFX_cols, CHIP8.GFX_rows);
            }
        }

        public static void TryDoStuff()
        {
            RefreshSize();

            if (drawFlag)                                    // 100% optimization for console only ;). also lets us
            {                                               // flip and rotate the screen for 100% free
                foreach (int row in updatedRows)
                {
                    Console.SetCursorPosition(0, row);
                    Console.Write(GetRow(row));
                }
            }

            drawFlag = false;
        }

        public static string GetRow(int rowNumber)
        {
            bool[] row = Enumerable.Range(0, screen.GetLength(0))
                            .Select(x => screen[x, rowNumber])
                            .ToArray();
            string t = "";
            foreach (bool r in row)
            {
                if (r)
                    t += "█";
                else
                    t += " ";
            }

            return t;
        }

        public static void shiftScreenDown(int amount)
        {
            for (int x = CHIP8.GFX_rows - 1; x >= 0; x--)
            {
                for (int y = CHIP8.GFX_cols - 1; y >= 0; y--)
                {
                    if (y < amount)
                        screen[x,y] = false;
                    else
                        screen[x,y] = screen[x, y - amount];
                }
            }

        }

        public static void shiftRight()
        {
            //Loop for each X and Y
            for (int x = CHIP8.GFX_rows - 1; x >= 0; x--)
            {
                for (int y = CHIP8.GFX_cols - 1; y >= 0; y--)
                {
                    if (x - 4 < 0) 
                        screen[x,y] = false;
                    else
                        screen[x,y] = screen[x - 4,y];

                }
            }

        }

        public static void shiftLeft()
        {
            //Loop for each X and Y
            for (int x = CHIP8.GFX_rows - 1; x >= 0; x++)
            {
                for (int y = CHIP8.GFX_cols - 1; y >= 0; y++)
                {
                    if (x + 4 < 0)
                        screen[x, y] = false;
                    else
                        screen[x, y] = screen[x + 4, y];

                }
            }

        }

        public static void ClearScreen()
        {
            Console.Clear();
            screen = new bool[CHIP8.GFX_cols, CHIP8.GFX_rows];
        }

        public static void DisplayHelp()
        {
            Console.Write("============ ");
            Console.Write("CHxP8 Help", Color.LightYellow);
            Console.Write(" ============\n");
            
            displayArg("-s48", "Disable super chip 48 mode. On by default.");
            displayArg("-m8", "Enable megachip8 support. Off by default.");
            displayArg("-rr", "Change refresh rate of the machine. 0 by default.");
            displayArg("-hz", "Change hertz of the sound.");
            displayArg("-nosound", "Disable sound.");
            displayArg("-seed", "Set the RNG seed to a custom value.");

            Console.WriteLine("\n[path]   : Rom file to load. Use modern terminal tab key autocompletion ;)");
        }

        private static void displayArg(string arg, string help)
        {
            Console.Write($"    {arg}   ", Color.SpringGreen);
            Console.Write(" : ");
            Console.Write(help+"\n");
        }    
    }
}
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace CHxP8
{
    namespace Debugger
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                Window w = new Window();
                w.Width = 9;
                //w.Show();


                Console.CursorVisible = false;
                Console.TreatControlCAsInput = true;
                //Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
                // ^^ shit doesnt work!
                //while (true)
                //{
                //    Renderer.RefreshGUI();
                //}
            }
        }
    }
}
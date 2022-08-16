using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
//using CHxP8.Debugger;

namespace CHxP8.Emulator
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Renderer.DisplayHelp();
                return;
            }

            string path = args[args.Length-1];
            Console.Title = "CHxP8 : " + path;

            Console.CursorVisible = false;
            Console.TreatControlCAsInput = true;
            Renderer.RefreshSize();
            Console.SetBufferSize(CHIP8.GFX_cols, CHIP8.GFX_rows);

            Settings.LoadSettings(File.ReadAllText(path));

            CHIP8.Init();
            OPCodeParser.LoadRom(File.ReadAllBytes(path));

            Thread inputThread = new(Keyboard.GetKey); // wtf why can you do this
            inputThread.Start();

            ProcessStartInfo d_info = new ProcessStartInfo();
            d_info.UseShellExecute = true;
            d_info.CreateNoWindow = false;
            d_info.WindowStyle = ProcessWindowStyle.Normal;
            d_info.FileName = Directory.GetCurrentDirectory()+"/Debugger.exe";

            //Process.Start(d_info);
            Debugger.Program.Main(null);

            for(; ; )
            {
                OPCodeParser.doCycle();
                Renderer.TryDoStuff();
                //Thread.Sleep((int)CHIP8.RefreshRate);

                //Debugger.Renderer.RefreshGUI(OPCodeParser.currentExecOpcode, Registers.programCounter, CHIP8.status);
            }
        }
    }
}

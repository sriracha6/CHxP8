using System;
using System.Linq;

namespace CHxP8
{
    public struct C8Key
    {
        public byte VirtualKey;
        public ConsoleKeyInfo KeyDriver;

        public C8Key(byte virtualKey, ConsoleKeyInfo keyDriver)
        {
            VirtualKey = virtualKey;
            KeyDriver = keyDriver;
        }
    }
}

namespace CHxP8.Emulator
{
    public static class Keyboard
    {
        public static C8Key[] keys = new C8Key[16];
        public static bool keyPause = false;

        public static void GetKey()
        {
            for (; ; )
            {
                if (Keyboard.keyPause)
                    CHIP8.currentKey = Console.ReadKey();
                else
                    CHIP8.currentKey = Console.ReadKey(true);
            }
        }

        public static byte keyCheck(ConsoleKeyInfo key)
        {
            return keys.ToList().Find(x => x.KeyDriver.Key == key.Key).VirtualKey;
        }
    }
}
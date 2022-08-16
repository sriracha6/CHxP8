using System;
using System.Threading;

namespace CHxP8.Emulator
{
    public static class Timer
    {
        public static int soundTimer { get; set; }
        public static int delayTimer { get; set; }

        public static void playSound(int length)
        {
            soundTimer = length;
        }

        public static void setDelay(int length)
        {
            delayTimer = length;
        }
    }
}
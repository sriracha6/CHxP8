using System;

namespace CHxP8.Emulator
{
    public static class Registers
    {
        public static UInt16 programCounter; // currently executing address
        public static byte[] Values = new byte[16];
        public static int VF = 0; // not for use in programs apparently
        public static int I; // usually used for address, last 12 bits only usually
    }
}
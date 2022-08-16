using System;

/// <summary>
/// Wanted an indexer, C# said no.
/// </summary>
namespace CHxP8.Emulator
{
    public static class Stack
    {
        public static Int16[] stack = new Int16[16];
        public static byte stackPointer = 0;
    }
}
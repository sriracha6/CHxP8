using System;
using System.Drawing;

namespace CHxP8.Emulator
{
	public static class CHIP8
	{
		public const int memSize = 0xFFF; // 4K, 0x0 - 0xFFF

		public static int GFX_rows = 32;
		public static int GFX_cols = 64;
		public static Color GFX_color = Color.White;

		public const int stackSize = 16;
		public static float RefreshRate = 16.6666666667f; // 60Hz
		public static int soundBeepHZ = 528;

		public static ConsoleKeyInfo currentKey;
		public static byte[] memory = new byte[4096];

		public static Random rnd;
		public static string status = "Running ";

		public static void updateKeyboard()
		{
			for (int i = 0; i < 16; i++)
			{
				var letter = i.ToString("X1").ToCharArray()[0];
				ConsoleKey ck;
				Enum.TryParse<ConsoleKey>(letter.ToString(), out ck);
				Keyboard.keys[i] = new C8Key((byte)i, new ConsoleKeyInfo(letter, ck, false, false, false)); // 0-9 A-F
			}
		}

		public static void loadFont()
		{
			byte[] chip8_fontset =
			{
			0xF0, 0x90, 0x90, 0x90, 0xF0, //0
			0x20, 0x60, 0x20, 0x20, 0x70, //1
			0xF0, 0x10, 0xF0, 0x80, 0xF0, //2
			0xF0, 0x10, 0xF0, 0x10, 0xF0, //3
			0x90, 0x90, 0xF0, 0x10, 0x10, //4
			0xF0, 0x80, 0xF0, 0x10, 0xF0, //5
			0xF0, 0x80, 0xF0, 0x90, 0xF0, //6
			0xF0, 0x10, 0x20, 0x40, 0x40, //7
			0xF0, 0x90, 0xF0, 0x90, 0xF0, //8
			0xF0, 0x90, 0xF0, 0x10, 0xF0, //9
			0xF0, 0x90, 0xF0, 0x90, 0x90, //A
			0xE0, 0x90, 0xE0, 0x90, 0xE0, //B
			0xF0, 0x80, 0x80, 0x80, 0xF0, //C
			0xE0, 0x90, 0x90, 0x90, 0xE0, //D
			0xF0, 0x80, 0xF0, 0x80, 0xF0, //E
			0xF0, 0x80, 0xF0, 0x80, 0x80  //F
		};
			for (int i = 0; i < chip8_fontset.Length; i++)
			{
				memory[i] = chip8_fontset[i];
			}
		}

		public static void Init()
		{
			Renderer.RefreshSize();
			updateKeyboard();
			rnd = new Random(); // todo seeds

			loadFont();

			Timer.setDelay(0x200);    // Set program counter to 0x200
			Registers.I = 0;          // Reset I
			Stack.stackPointer = 0;        // Reset stack pointer
			Registers.programCounter = 0x200;
		}
	}
}

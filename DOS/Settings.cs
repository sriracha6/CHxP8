using System;
using System.Drawing;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CHxP8.Emulator
{
	public class Settings
	{
		public static Settings Instance;

		public bool enableDebugger { get; private set; }

		public int width { get; private set; } //             based off of file format but eh
		public int height { get; private set; }

		public C8Key[] internalKeybinds { get; private set; }
		public ConsoleKey[] keybinds { get; private set; }

		public Color color { get; private set; }
		public bool soundEnabled { get; private set; }

		public bool enableSC48 { get; private set; } = true; //        super chip-48 instructions
		public bool enable0NNN { get; private set; } //        enable system 0nnn instruction

		public float refreshRate { get; private set; }
		public int soundHZ { get; private set; }

		public bool overridedSeed { get; private set; }
		public long overridedSeedValue { set; private get; }


		public static void LoadSettings(string yml)
		{
			var d = new DeserializerBuilder()
				.WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
				.Build();

			//Instance = d.Deserialize<Settings>(yml);
		}
	}
}
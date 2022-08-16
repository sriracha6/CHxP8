using CHxP8;
using CHxP8.Emulator;
using System;
using System.Collections;
using Stack = CHxP8.Emulator.Stack;

public static class OPCodeParser
{
    public struct OP
    {
        /// <summary>
        /// Lower 12 bits of instruction.
        /// </summary>
        public int opCode;    // full code, like 0xEE00
        public int addr;      // lowest 12 bits of the instruction
        public Int4 nibble;     // lowest 4 bits of the instruction
        public Int4 x;          // lower 4 bits of the high byte of the instruction
        public Int4 y;          // upper 4 bits of the low byte of the instruction
        public byte kk;         // lowest 8 bits of the instruction

        static int bitExtracted(int number, int k, int p)
        {
            return (((1 << k) - 1) & (number >> (p - 1)));
        }

        public OP(int byteCode)
        {
            this.opCode = byteCode;
            this.addr = (byteCode & 0x0FFF)/2;//bitExtracted(byteCode, 12, 2)/2; // why didnt i fucking use &
            this.nibble = (Int4)bitExtracted(byteCode, 4, 1);
            this.x = (Int4)bitExtracted(byteCode, 4, 9);
            this.y = (Int4)bitExtracted(byteCode, 4, 5);
            this.kk = (byte)bitExtracted(byteCode, 8, 1);
            /*this.addr = (Int12)(byteCode & 0x0FFF); // why didnt i fucking use &
            this.nibble = (Int4)(byteCode & 0x000F);
            this.x = (Int4)(byteCode & 0x0F00);
            this.y = (Int4)(byteCode & 0x00F0);
            this.kk = (byte)(byteCode & 0x00FF);*/
        }
    }

    //public static OP[] opcodes;
    public static int currentExecOpcode { get; private set; }
    private static int currentRomLength;

    public static void LoadRom(byte[] prog)
    {
        for (int i = 0; i < prog.Length; i++)
        {
            CHIP8.memory[0x200+i] = prog[i];
        }
        currentRomLength = prog.Length;
    }

    public static void doCycle()
    {
        byte lowByte = CHIP8.memory[Registers.programCounter];
        byte highByte = CHIP8.memory[Registers.programCounter + 1];

        OP opcode = new OP((lowByte << 8) | highByte);
        currentExecOpcode = opcode.opCode;

        switch (opcode.opCode & 0xF000)
        {
            case 0x0000:
                switch (opcode.opCode & 0x00FF)
                {
                    case 0x00E0:                                        // CLS
                        Renderer.ClearScreen();
                        Registers.programCounter+=2;
                        break;
                    case 0x00EE:                                        // RET
                        --Stack.stackPointer;
                        Registers.programCounter = (ushort)Stack.stack[Stack.stackPointer];
                        Registers.programCounter+=2;
                        break;
                    case 0x00FF:                                        // sc48 - highres
                        if (Settings.Instance.enableSC48)
                        {
                            CHIP8.GFX_cols = 64;
                            CHIP8.GFX_rows = 128;
                            Renderer.RefreshSize();
                        }
                        else
                            goto default;
                        break;
                    case 0x00FE:                                       // sc48 - lowres
                        if (Settings.Instance.enableSC48)
                        {
                            CHIP8.GFX_cols = 64;
                            CHIP8.GFX_rows = 32;
                        }
                        else
                            goto default;
                        break;
                    case 0x00FB:
                        if (Settings.Instance.enableSC48)
                        {
                            Renderer.shiftRight();
                        }
                        else
                            goto default;
                        break;
                    case 0x00FC:
                        if (Settings.Instance.enableSC48)
                        {
                            Renderer.shiftLeft();
                        }
                        else
                            goto default;
                        break;

                    default:
                        throw new Exception($"Invalid OPCode: 0x{opcode.opCode.ToString("X")}");
                }
                switch(opcode.opCode & 0x00F0) // no default: sc48 only
                {
                    case 0x00C0:                // scroll screen down by X pixels (nibble)
                        if (Settings.Instance.enableSC48)
                        {
                            Renderer.shiftScreenDown(opcode.nibble);
                        }
                        break;
                }
                break;

            case 0x1000:                                        // JMP
                Registers.programCounter = (ushort)(opcode.opCode & 0x0FFF);
                break;

            case 0x2000:                                        // CALL
                Stack.stack[Stack.stackPointer] = (short)Registers.programCounter;
                Stack.stackPointer++;
                Registers.programCounter = (ushort)(opcode.opCode & 0x0FFF);
                break;

            case 0x3000:                                        // SE
                if (Registers.Values[opcode.x] == opcode.kk)
                    Registers.programCounter += 4;
                else
                    Registers.programCounter+=2;
                break;

            case 0x4000:                                        // SNE
                if (Registers.Values[opcode.x] != opcode.kk)
                    Registers.programCounter += 4;
                else
                    Registers.programCounter+=2;
                break;

            case 0x5000:                                        // SE x,y
                if (Registers.Values[opcode.x] == opcode.y)
                    Registers.programCounter += 4;
                else
                    Registers.programCounter+=2;
                break;                                          
            case 0x6000:                                        // LD
                Registers.Values[opcode.x] = opcode.kk;
                Registers.programCounter+=2; 
                break;
            case 0x7000:                                        // ADD
                Registers.Values[opcode.x] += opcode.kk;
                Registers.programCounter+=2;
                break;
            case 0x8000:
                switch(opcode.opCode & 0x000F)
                {
                    case 0x0000:                                // LD X, Y
                        Registers.Values[opcode.x] = Registers.Values[opcode.y];
                        Registers.programCounter+=2;
                        break;
                    case 0x0001:                                // OR X,Y 
                        Registers.Values[opcode.x] |= Registers.Values[opcode.y];
                        Registers.programCounter+=2;
                        break;
                    case 0x0002:                                // AND X,Y
                        Registers.Values[opcode.x] &= Registers.Values[opcode.y];
                        Registers.programCounter+=2;
                        break;
                    case 0x0003:                                // XOR X,Y
                        Registers.Values[opcode.x] ^= Registers.Values[opcode.y];
                        Registers.programCounter+=2;
                        break;
                    case 0x0004:                                // ADD X,Y
                        Registers.Values[opcode.x] += Registers.Values[opcode.y];
                        if (Registers.Values[opcode.x] > 0xFF) // todo does this fucking work??
                            Registers.Values[0xF] = 1;
                        else
                            Registers.Values[0xF] = 0;
                        Registers.programCounter+=2;
                        break;
                    case 0x0005:                                // SUB X,Y      : todo, is this good?
                        Registers.Values[opcode.x] -= Registers.Values[opcode.y];
                        if (Registers.Values[opcode.x] > Registers.Values[opcode.y])
                            Registers.VF = 1;
                        else
                            Registers.VF = 0;
                        Registers.programCounter+=2;
                        break;
                    case 0x0006:                               // SHR X,Y
                        Registers.VF = Registers.Values[opcode.x] & 0x1;
                        Registers.Values[opcode.x] >>= 1;
                        Registers.programCounter+=2;
                        break;
                    case 0x0007:                              // SUBN x,y
                        Registers.Values[opcode.x] = (byte)(Registers.Values[opcode.y] - Registers.Values[opcode.x]);
                        if (Registers.Values[opcode.y] > Registers.Values[opcode.x])
                            Registers.VF = 1;
                        else
                            Registers.VF = 0;
                        Registers.programCounter+=2;
                        break;
                    case 0x000E:                               // SHL X,Y
                        Registers.VF = Registers.Values[opcode.x] & 0x1;
                        Registers.Values[opcode.x] <<= 1;
                        Registers.programCounter+=2;
                        break;
                    default:
                        throw new Exception($"Invalid OPCode: {opcode.opCode.ToString("X")}");
                }
                break;

            case 0x9000:
                if (Registers.Values[opcode.x] != Registers.Values[opcode.y])
                    Registers.programCounter += 4;
                else
                    Registers.programCounter+=2;
                break;

            case 0xA000:                                       // LD x,y
                Registers.I = (short)(opcode.opCode & 0x0FFF);
                Registers.programCounter+=2;
                break;

            case 0xB000:                                      // JP
                Registers.programCounter = (ushort)((opcode.opCode & 0x0FFF) + Registers.Values[0]);
                break;
            case 0xC000:                                      // RND, todo for seeds and custom seeding
                Registers.Values[opcode.x] = (byte)(CHIP8.rnd.Next(0,256) & opcode.kk);
                Registers.programCounter+=2;
                break;
            case 0xD000:                                     // draw sprite
                ushort x = Registers.Values[(opcode.opCode & 0x0F00) >> 8];
                ushort y = Registers.Values[(opcode.opCode & 0x00F0) >> 4];
                ushort height = (ushort)opcode.nibble;
                ushort pixel;

                Registers.VF = 0;
                for (int yline = 0; yline < height; yline++)
                {
                    pixel = CHIP8.memory[Registers.I + yline];
                    for (int xline = 0; xline < 8; xline++)
                    {
                        if ((pixel & (0x80 >> xline)) != 0)
                        {
                            if (Renderer.screen[(x + xline), ((y + yline)/* * 64*/)] == true)
                            {
                                Registers.VF = 1;
                            }
                            Renderer.screen[x + xline, ((y + yline)/* * 64*/)] ^= true;
                        }
                    }
                }
                Renderer.drawFlag = true;
                Registers.programCounter += 2;

                for(int i = 0; i < height; i++)
                {
                    Renderer.updatedRows.Add(i+y);
                }
                break;

            case 0xE000:
                switch(opcode.opCode & 0x00FF)
                {
                    case 0x009E:                           // skip next instruction if key pressed
                        if (Keyboard.keyCheck(CHIP8.currentKey) == Registers.Values[opcode.x])
                            Registers.programCounter += 4;
                        else
                            Registers.programCounter+=2;
                        break;

                    case 0x00A1:                           // skip next instruction if key NOT pressed
                        if (Keyboard.keyCheck(CHIP8.currentKey) != Registers.Values[opcode.x])
                            Registers.programCounter += 4;
                        else
                            Registers.programCounter+=2;
                        break;


                    default:
                        throw new Exception($"Invalid OPCode: 0x{opcode}");
                }
                break;

            case 0xF000:
                switch (opcode.opCode & 0x00FF)
                {
                    case 0x0007:                    // set x to delay timer
                        Registers.Values[opcode.x] = (byte)Timer.delayTimer;
                        Registers.programCounter+=2;
                        break;
                    case 0x000A:                    // wait for next keypress
                        Keyboard.keyPause = true;
                        Registers.programCounter+=2;
                        break;
                    case 0x0015:                     // set delay timer
                        Timer.delayTimer = Registers.Values[opcode.x];
                        Registers.programCounter+=2;
                        break;
                    case 0x0018:                    // set sound timer
                        Timer.soundTimer = Registers.Values[opcode.x];
                        Registers.programCounter+=2;
                        break;
                    case 0x001E:                    // add i and X and store in I
                        Registers.I = (Int16)(Registers.I + Registers.Values[opcode.x]);
                        Registers.programCounter+=2;
                        break;
                    case 0x0029:                    // I = location of sprite for digit Vx. todo
                        Registers.I = (short)(opcode.x * 5);
                        Registers.programCounter+=2;
                        break;
                    case 0x0033:                   // store bcd in i, i+1, i+2
                        CHIP8.memory[Registers.I] = (byte)(Registers.Values[opcode.x] / 100);
                        CHIP8.memory[Registers.I + 1] = (byte)((Registers.Values[opcode.x] / 10) % 10);
                        CHIP8.memory[Registers.I + 2] = (byte)(Registers.Values[opcode.x] % 10);
                        Registers.programCounter+=2;
                        break;
                    case 0x0055:                   // stores registers v0-vx in memory starting at I
                        for(int i = 0; i <= opcode.x; i++)
                        {
                            CHIP8.memory[Registers.I+i] = Registers.Values[i];
                        }
                        Registers.I += opcode.x + 1;
                        Registers.programCounter+=2;
                        break;

                    case 0x0065:                  // load from memory I to X into v0-vx
                        for(int i = 0; i < opcode.x; i ++)
                        {
                            Registers.Values[i] = (byte)CHIP8.memory[opcode.x + i];
                        }
                        Registers.programCounter+=2;
                        break;

                    default:
                        throw new Exception($"Invalid OPCode: 0x{opcode.opCode.ToString("X")}");
                }
                break;

            default: // CHECK FOR NOTS!!!!
                throw new Exception($"Invalid OPCode: {opcode}");
        }

        if (Keyboard.keyPause)
            Keyboard.keyPause = false;

        if(Timer.delayTimer>0)
            Timer.delayTimer--;

        if (Timer.soundTimer > 0)
        {
            Console.Beep(CHIP8.soundBeepHZ, (int)CHIP8.RefreshRate);
            Timer.soundTimer--;
        }

        if (Registers.programCounter == 0x200 + currentRomLength)
            CHIP8.status = "Halted ";
    }
}

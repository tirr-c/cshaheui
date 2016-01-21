using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CShAheui
{
    public class Aheui
    {
        private static readonly int[] Jongseong = new int[]
        {
            0, 2, 4, 4, 2, 5, 5, 3, 5, 7, 9, 9, 7, 9, 9, 8, 4, 4, 6, 2, 4, -1, 3, 4, 3, 4, 4, -2,
        };

        private string[] code;
        private Cursor cursor;
        private Storage storage;

        public System.IO.TextReader Input { get; set; }
        public System.IO.TextWriter Output { get; set; }
        public System.IO.TextWriter Error { get; set; }
        int? inputCache = null;

        public Aheui()
        {
            storage = new Storage();
            cursor = new Cursor();
            Input = new System.IO.StreamReader(Console.OpenStandardInput(), new UTF8Encoding(false));
            Output = new System.IO.StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false));
            Error = new System.IO.StreamWriter(Console.OpenStandardError(), new UTF8Encoding(false));
        }

        public void Reset()
        {
            storage = new Storage();
            cursor = new Cursor();
        }

        public int Execute(string aheui)
        {
            BigInteger ret = 0;
            code = aheui.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            cursor = new Cursor();

            while (true)
            {
                Hangul insturction = Step();
                if (insturction.Command == 'ㅎ')
                {
                    try
                    {
                        ret = storage.Pop();
                    }
                    catch (AheuiUnderflowException)
                    {
                        ret = 0;
                    }
                    break;
                }
            }
            Output.Flush();
            return (int)ret;
        }

        private Hangul Step()
        {
            // 1. Poll
            Hangul instruction = new Hangul();
            if (code[cursor.Y].Length > cursor.X)
            {
                instruction = new Hangul(code[cursor.Y][cursor.X]);
            }
            // 2. Execute
            bool reversed = false;
            if (!instruction.IsNop)
            {
                try
                {
                    BigInteger stackFirst, stackSecond;
                    switch (instruction.Command)
                    {
                        case 'ㅎ':
                            return instruction;

                        case 'ㄷ':
                            storage.AssertPop(2);
                            storage.Push(storage.Pop() + storage.Pop());
                            break;
                        case 'ㄸ':
                            storage.AssertPop(2);
                            storage.Push(storage.Pop() * storage.Pop());
                            break;
                        case 'ㅌ':
                            storage.AssertPop(2);
                            stackFirst = storage.Pop();
                            stackSecond = storage.Pop();
                            storage.Push(stackSecond - stackFirst);
                            break;
                        case 'ㄴ':
                            storage.AssertPop(2);
                            stackFirst = storage.Pop();
                            stackSecond = storage.Pop();
                            storage.Push(stackSecond / stackFirst);
                            break;
                        case 'ㄹ':
                            storage.AssertPop(2);
                            stackFirst = storage.Pop();
                            stackSecond = storage.Pop();
                            storage.Push(stackSecond % stackFirst);
                            break;

                        case 'ㅁ':
                            stackFirst = storage.Pop();
                            if (Jongseong[instruction.Argument] == -1)
                            {
                                Output.Write(stackFirst);
                            }
                            else if (Jongseong[instruction.Argument] == -2)
                            {
                                if (stackFirst < 0x10000) Output.Write((char)stackFirst);
                                else
                                {
                                    int z, x, y;
                                    z = (int)((stackFirst & 0x1f0000) >> 16) - 1;
                                    x = (int)((stackFirst & 0xfc00) >> 10);
                                    y = (int)(stackFirst & 0x3ff);
                                    Output.Write((char)(0xd800 | (z << 6) | x));
                                    Output.Write((char)(0xdc00 | y));
                                }
                            }
                            break;
                        case 'ㅂ':
                            if (Jongseong[instruction.Argument] == -1)
                            {
                                int number = 0;
                                int sign = 1;
                                bool numberStarted = false;
                                while (true)
                                {
                                    int t;
                                    if (inputCache == null) t = Input.Read();
                                    else
                                    {
                                        t = inputCache.Value;
                                        inputCache = null;
                                    }
                                    if (t <= -1) break;
                                    char c = (char)t;
                                    if (c < '0' || c > '9')
                                    {
                                        if (!numberStarted)
                                        {
                                            if (c == '-') sign = -1;
                                            else sign = 1;
                                            continue;
                                        }
                                        if (!" \t\r\n".Contains(c)) inputCache = t;
                                        break;
                                    }
                                    numberStarted = true;
                                    number *= 10;
                                    number += (c - '0');
                                }
                                storage.Push(number * sign);
                            }
                            else if (Jongseong[instruction.Argument] == -2)
                            {
                                int t;
                                if (inputCache != null)
                                {
                                    t = inputCache.Value;
                                    inputCache = null;
                                }
                                else
                                {
                                    t = Input.Read();
                                }
                                if (t >= 0xd800 && t < 0xdc00)
                                {
                                    int k = Input.Read();
                                    if (k >= 0xdc00 && t < 0xe000)
                                    {
                                        // convert surrogate pair
                                        int z, x, y;
                                        z = ((t & 0x3c0) >> 6) + 1;
                                        x = (t & 0x3f);
                                        y = (k & 0x3ff);
                                        t = (z << 16) + (x << 10) + y;
                                    }
                                    else
                                    {
                                        // not a surrogate pair
                                        inputCache = k;
                                    }
                                }
                                storage.Push(t);
                            }
                            else
                            {
                                storage.Push(Jongseong[instruction.Argument]);
                            }
                            break;
                        case 'ㅃ':
                            storage.Duplicate();
                            break;
                        case 'ㅍ':
                            storage.Swap();
                            break;

                        case 'ㅅ':
                            storage.SelectedStorage = instruction.Argument;
                            break;
                        case 'ㅆ':
                            storage.Move(instruction.Argument);
                            break;
                        case 'ㅈ':
                            storage.AssertPop(2);
                            stackFirst = storage.Pop();
                            stackSecond = storage.Pop();
                            if (stackSecond >= stackFirst) storage.Push(1);
                            else storage.Push(0);
                            break;
                        case 'ㅊ':
                            reversed = storage.Pop() == 0;
                            break;
                    }
                }
                catch (NotImplementedException)
                {
                    Error.WriteLine("[!!] 구현되지 않은 행동: {0}행 {1}열 '{2}'", cursor.Y + 1, cursor.X + 1, code[cursor.Y][cursor.X]);
                }
                catch (DivideByZeroException)
                {
                    Error.WriteLine("[!!] 0으로 나눔: {0}행 {1}열 '{2}'", cursor.Y + 1, cursor.X + 1, code[cursor.Y][cursor.X]);
                }
                catch (AheuiUnderflowException)
                {
                    reversed = true;
                }
            }
            // 3. Move
            cursor.Move(code, instruction.Direction, reversed);

            return instruction;
        }
    }
}

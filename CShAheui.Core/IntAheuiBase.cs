using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CShAheui.Core
{
    public abstract class IntAheuiBase : AheuiBase<IntStorage, int>
    {
        public IntAheuiBase()
        {
        }

        protected override void WriteChar(int val)
        {
            if (val < 0x10000) Output.Write((char)val);
            else
            {
                int z, x, y;
                z = (int)((val & 0x1f0000) >> 16) - 1;
                x = (int)((val & 0xfc00) >> 10);
                y = (int)(val & 0x3ff);
                Output.Write((char)(0xd800 | (z << 6) | x));
                Output.Write((char)(0xdc00 | y));
            }
        }

        protected override int Calculate(int a, int b, BinaryOperation operation)
        {
            switch (operation)
            {
                case BinaryOperation.Add:
                    return a + b;
                case BinaryOperation.Multiply:
                    return a * b;
                case BinaryOperation.Subtract:
                    return a - b;
                case BinaryOperation.Divide:
                    return a / b;
                case BinaryOperation.Modulo:
                    return a % b;
                default:
                    throw new ArgumentException("operation");
            }
        }

        protected override int ReadFromInput(int argument)
        {
            if (argument == -1)
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
                return number * sign;
            }
            else if (argument == -2)
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
                return t;
            }
            else return argument;
        }
    }
}

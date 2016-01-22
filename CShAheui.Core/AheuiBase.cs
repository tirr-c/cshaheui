using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShAheui.Core
{
    public abstract class AheuiBase<Storage, T>
        where Storage : IStorage<T>, new()
        where T : IComparable
    {
        protected enum BinaryOperation
        {
            Add, Multiply, Subtract, Divide, Modulo
        }

        private static readonly int[] Jongseong = new int[]
        {
            0, 2, 4, 4, 2, 5, 5, 3, 5, 7, 9, 9, 7, 9, 9, 8, 4, 4, 6, 2, 4, -1, 3, 4, 3, 4, 4, -2,
        };
        
        protected Storage storage;

        public System.IO.TextReader Input { get; set; }
        public System.IO.TextWriter Output { get; set; }
        public System.IO.TextWriter Error { get; set; }

        private bool AlwaysFlush
        {
            get
            {
                return !Console.IsInputRedirected;
            }
        }

        protected int? inputCache = null;

        public AheuiBase()
        {
            storage = new Storage();
            Input = new System.IO.StreamReader(Console.OpenStandardInput(), new UTF8Encoding(false));
            Output = new System.IO.StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false));
            Error = new System.IO.StreamWriter(Console.OpenStandardError(), new UTF8Encoding(false));
        }

        public virtual void Reset()
        {
            storage = new Storage();
        }

        public int Execute()
        {
            int ret = ExecuteInternal();
            Output.Flush();
            Error.Flush();
            return ret;
        }

        protected abstract int ExecuteInternal();

        protected abstract void WriteChar(T val);
        protected abstract T ReadFromInput(int argument);
        protected abstract T Calculate(T a, T b, BinaryOperation operation);

        protected bool Step(char command, int argument)
        {
            bool reversed = false;
            if (command != '\0' && command != 'ㅇ')
            {
                try
                {
                    T stackFirst, stackSecond;
                    switch (command)
                    {
                        case 'ㅎ':
                            return false;

                        case 'ㄷ':
                            storage.AssertPop(2);
                            storage.Push(Calculate(storage.Pop(), storage.Pop(), BinaryOperation.Add));
                            break;
                        case 'ㄸ':
                            storage.AssertPop(2);
                            storage.Push(Calculate(storage.Pop(), storage.Pop(), BinaryOperation.Multiply));
                            break;
                        case 'ㅌ':
                            storage.AssertPop(2);
                            stackFirst = storage.Pop();
                            stackSecond = storage.Pop();
                            storage.Push(Calculate(stackSecond, stackFirst, BinaryOperation.Subtract));
                            break;
                        case 'ㄴ':
                            storage.AssertPop(2);
                            stackFirst = storage.Pop();
                            stackSecond = storage.Pop();
                            storage.Push(Calculate(stackSecond, stackFirst, BinaryOperation.Divide));
                            break;
                        case 'ㄹ':
                            storage.AssertPop(2);
                            stackFirst = storage.Pop();
                            stackSecond = storage.Pop();
                            storage.Push(Calculate(stackSecond, stackFirst, BinaryOperation.Modulo));
                            break;

                        case 'ㅁ':
                            stackFirst = storage.Pop();
                            if (Jongseong[argument] == -1)
                            {
                                Output.Write(stackFirst);
                            }
                            else if (Jongseong[argument] == -2)
                            {
                                WriteChar(stackFirst);
                            }
                            break;
                        case 'ㅂ':
                            storage.Push(ReadFromInput(Jongseong[argument]));
                            break;
                        case 'ㅃ':
                            storage.Duplicate();
                            break;
                        case 'ㅍ':
                            storage.Swap();
                            break;

                        case 'ㅅ':
                            storage.SelectedStorage = argument;
                            break;
                        case 'ㅆ':
                            storage.Move(argument);
                            break;
                        case 'ㅈ':
                            storage.AssertPop(2);
                            stackFirst = storage.Pop();
                            stackSecond = storage.Pop();
                            if (stackSecond.CompareTo(stackFirst) >= 0) storage.Push(ReadFromInput(1));
                            else storage.Push(ReadFromInput(0));
                            break;
                        case 'ㅊ':
                            reversed = storage.Pop().Equals(ReadFromInput(0));
                            break;
                    }
                }
                catch (AheuiUnderflowException)
                {
                    reversed = true;
                }
            }
            if (AlwaysFlush)
            {
                Output.Flush();
                Error.Flush();
            }
            return reversed;
        }
    }
}

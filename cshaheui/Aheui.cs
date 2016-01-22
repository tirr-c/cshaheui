using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CShAheui.Core;

namespace CShAheui.App
{
#if BIGINT
    public class Aheui : BigIntAheuiBase
#else
    public class Aheui : IntAheuiBase
#endif
    {
        private string[] code;
        private Cursor cursor;
        
        public Aheui() : base()
        {
            cursor = new Cursor();
        }

        public override void Reset()
        {
            base.Reset();
            cursor = new Cursor();
        }

        public int Execute(string aheui)
        {
            code = aheui.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            cursor = new Cursor();

            return Execute();
        }

        protected override int ExecuteInternal()
        {
            while (true)
            {
                Hangul insturction = Step();
                if (insturction.Command == 'ㅎ')
                {
                    return storage.MakeReturnValue();
                }
            }
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
                    reversed = base.Step(instruction.Command, instruction.Argument);
                }
                catch (NotImplementedException)
                {
                    Error.WriteLine("[!!] 구현되지 않은 행동: {0}행 {1}열 '{2}'", cursor.Y + 1, cursor.X + 1, code[cursor.Y][cursor.X]);
                }
                catch (DivideByZeroException)
                {
                    Error.WriteLine("[!!] 0으로 나눔: {0}행 {1}열 '{2}'", cursor.Y + 1, cursor.X + 1, code[cursor.Y][cursor.X]);
                }
            }
            // 3. Move
            cursor.Move(code, instruction.Direction, reversed);

            return instruction;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShAheui.App
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("No input file");
                return 1;
            }
            string code = System.IO.File.ReadAllText(args[0], Encoding.UTF8);
#if BIGINT
            bool _bigint = true;
#else
            bool _bigint = false;
#endif
            bool bigint = _bigint, compile = false, binary = false;
            if (args.Length >= 2)
            {
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i].Equals("compile", StringComparison.OrdinalIgnoreCase))
                    {
                        compile = true;
                    }
                    if (args[i].Equals("bigint", StringComparison.OrdinalIgnoreCase))
                    {
                        bigint = true;
                    }
                    if (args[i].Equals("binary", StringComparison.OrdinalIgnoreCase))
                    {
                        compile = binary = true;
                    }
                }
            }
            if (bigint != _bigint && !compile)
            {
                Console.Error.WriteLine("You cannot change the integer size when cshaheui runs in the interpreter mode.");
            }

            int ret;
            if (compile)
            {
                AheuiCompiler compiler = new AheuiCompiler();
                compiler.Load(code);
                var execute = compiler.Compile(bigint, binary);

                if (execute != null) ret = execute();
                else ret = 0;
            }
            else
            {
                Aheui aheui = new Aheui();
                ret = aheui.Execute(code);
            }
            return ret;
        }
    }
}

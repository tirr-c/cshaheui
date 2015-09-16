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
            Aheui aheui = new Aheui();
            string code = System.IO.File.ReadAllText(args[0], Encoding.UTF8);
            return aheui.Execute(code);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CShAheui.Core;

namespace CShAheui.App
{
    public class CodePlane
    {
        private Hangul[][] plane;

        public CodePlane(string aheui)
        {
            string[] code = aheui.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            plane = new Hangul[code.Length][];
            var tempList = new List<Hangul>();
            int cnt = 0;
            foreach (string line in code)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    if (c >= 0xd800 && c < 0xdc00)
                    {
                        if (i + 1 < line.Length && line[i + 1] >= 0xdc00 && line[i + 1] < 0xe000)
                        {
                            i++;
                            tempList.Add(new Hangul());
                            continue;
                        }
                    }
                    tempList.Add(new Hangul(c));
                }
                plane[cnt++] = tempList.ToArray();
                tempList.Clear();
            }
        }

        public Hangul At(int x, int y)
        {
            if (y >= Height) return null;
            if (x >= plane[y].Length) return null;
            return plane[y][x];
        }

        public int WidthAt(int y)
        {
            return plane[y].Length;
        }

        public int Height
        {
            get
            {
                return plane.Length;
            }
        }
    }
}

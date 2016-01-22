namespace CShAheui.Core
{
    public class Hangul
    {
        private static readonly string[] HangulTable = new string[]
        {
            "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ", // choseong
            "ㅏ\0ㅑ\0ㅓ\0ㅕ\0ㅗ\0\0\0ㅛㅜ\0\0\0ㅠㅡㅢㅣ", // jungseong
        };

        public bool IsNop { get; private set; }
        public char Command { get; private set; }
        public char Direction { get; private set; }
        public int Argument { get; private set; }

        public Hangul()
        {
            IsNop = true;
            Command = Direction = '\0';
            Argument = 0;
        }

        public Hangul(char hangul)
        {
            // Hangul syllable?
            if (hangul < '가' || hangul > '힣')
            {
                IsNop = true;
                Command = Direction = '\0';
                Argument = 0;
                return;
            }

            int idx = hangul - '가';
            char[] result = new char[2];

            Argument = idx % 28; // jongseong
            idx /= 28;
            for (int i = 1; i >= 0; i--)
            {
                result[i] = HangulTable[i][idx % HangulTable[i].Length];
                idx /= HangulTable[i].Length;
            }
            Command = result[0];
            Direction = result[1];
            IsNop = (Command == '\0');
        }
    }
}

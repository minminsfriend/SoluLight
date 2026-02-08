using System;


namespace shine.libs.hangul
{
        /*
        // 초성 리스트. 00 ~ 18
        static readonly char[] CHOSUNG_LIST = new char[]
        {
        'ㄱ','ㄲ','ㄴ','ㄷ','ㄸ','ㄹ','ㅁ','ㅂ','ㅃ', 
        'ㅅ','ㅆ','ㅇ','ㅈ','ㅉ','ㅊ','ㅋ','ㅌ','ㅍ','ㅎ'
        };
        // 중성 리스트. 00 ~ 20
        static readonly char[] JUNGSUNG_LIST =new char[]
        {
        'ㅏ','ㅐ','ㅑ','ㅒ','ㅓ','ㅔ','ㅕ','ㅖ',
        'ㅗ','ㅘ','ㅙ','ㅚ',
        'ㅛ','ㅜ','ㅝ','ㅞ','ㅟ','ㅠ','ㅡ','ㅢ','ㅣ'
        };
        // 종성 리스트. 00 ~ 27 + 1(1개 없음)
        static readonly char[] ZONGSUNG_LIST =new char[]
        {
        'ㄱ','ㄲ','ㄳ','ㄴ','ㄵ','ㄶ','ㄷ',
        'ㄹ','ㄺ','ㄻ','ㄼ','ㄽ','ㄾ','ㄿ','ㅀ',
        'ㅁ','ㅂ','ㅄ','ㅅ','ㅆ','ㅇ','ㅈ','ㅊ','ㅋ','ㅌ','ㅍ','ㅎ'
        };
        */

    public static class Han
    {
        public static readonly int[] CHOSUNG_VALS = new int[]{12593,12594,12596,12599,12600,12601,
                                    12609,12610,12611,12613,12614,12615,
                                    12616,12617,12618,12619,12620,12621,12622};

        public const int CHOSUNG_Count = 19;
        public const int JUNGSUNG_Count = 21;
        public const int ZONGSUNG_Count = 28;

        //public const int CHO_START = 20000;
        public const int FIRST_HANGUL = 44032;
        public const int AllLettersBy1Chosung = 588;
        //public const int JUNGSUNG_GAP = 28; // = 종성리스트 갯수
        public static class Jindo
        {
            public const int zero = 0;
            public const int Cho = 1;
            public const int ChoX = 2;
            public const int Jung = 3;
            public const int JungX = 4;
            public const int Zong = 5;
            public const int ZongX = 6;
        }
        public static int getChoIndex(int unicode)
        {
            int i;
            for (i = 0; i < 19; i++)
            {
                if (Han.CHOSUNG_VALS[i] == unicode)
                    return i;

            }
            return -1;
        }
        public static int getUnicode(int nCho, int nJung, int nZong)
        {
            if (nJung == -1)//only chosung
            {
                if (nCho > -1 && nCho < 19)
                    return CHOSUNG_VALS[nCho];

                else
                    return 12593;//ㄱ
            }
            else
            {
                if (nZong == -1)
                    nZong = 0;

                return Han.FIRST_HANGUL + 588 * nCho + 28 * nJung + nZong;
            }
        }
        public static bool inChoRange(int choCode, int hanCode)
        {
            int[] range = isOnlyCho(choCode);

            if (range[0] == -1)
                return false;
            else if (range[0] <= hanCode && hanCode <= range[1])
                return true;

            return false;
        }
        public static bool inJungRange(int jungCode, int hanCode)
        {
            int[] range = inNoZongRange(jungCode);

            if (range[0] == -1)
                return false;
            else if (range[0] <= hanCode && hanCode <= range[1])
                return true;

            //free(range);
            return false;
        }
        public static int[] isOnlyCho(int code)
        {
            int[] range = new int[2];

            int index = getChoIndex(code);
            if (index == -1)
            {
                range[0] = -1;
                range[1] = -1;
            }
            else
            {
                int cho0 = Han.FIRST_HANGUL + Han.AllLettersBy1Chosung * index;
                int cho9 = cho0 + Han.AllLettersBy1Chosung - 1;

                range[0] = cho0;
                range[1] = cho9;
            }

            return range;
        }
        public static int[] inNoZongRange(int code)
        {
            int[] range = new int[] { -1, -1 };

            if (code < Han.FIRST_HANGUL)
                return range;

            int hcode = code - Han.FIRST_HANGUL;
            int cho = hcode / Han.AllLettersBy1Chosung;

            int chorem = hcode % Han.AllLettersBy1Chosung;
            int jung = chorem / Han.ZONGSUNG_Count;
            int zong = chorem % Han.ZONGSUNG_Count;

            if (zong > 0)
                return range;

            int jung0 = Han.FIRST_HANGUL + Han.AllLettersBy1Chosung * cho + Han.ZONGSUNG_Count * jung;
            int jung9 = jung0 + Han.ZONGSUNG_Count - 1;

            range[0] = jung0;
            range[1] = jung9;

            return range;
        }
    }
}

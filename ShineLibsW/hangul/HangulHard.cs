using System;
using System.Text;
using shine.libs.utils;
using System.Collections.Generic;
using System.Windows.Forms;

namespace shine.libs.hangul
{
    public class HangulHard
    {
        const string CLAS = "HangulHard";
        public int currKeycode, preKeycode390;
        int currKeycode390;
        readonly string textUpperNums = ")!@#$%^&*(";

        public UniHangul uniHan;
        public bool SHIFT;
        public bool ALT;
        //public bool CTRL;

        public HangulHard()
        {
            fillDicHangulIndexs();
            fillDics390Keycodes();
            
            reset();
        }
        public void reset()
        {
            currKeycode390 = currKeycode = preKeycode390 = -1;
            //SHIFT = CTRL = ALT = false;
            SHIFT = ALT = false;

            uniHan = new UniHangul();
        }
        public void setCurrentCode(Keys keycode)
        {
            this.preKeycode390 = this.currKeycode390;
            this.currKeycode390 = SHIFT ? -(int)keycode : (int)keycode;

            this.currKeycode = (int)keycode;
        }
        public bool check_key_back(ref bool deleteSurround)
        {
            if (currKeycode390 == (int)Keys.Back)// == win backspace
            {
                uniHan.Delete(ref deleteSurround);

                return true;
            }

            return false;
        }
        public bool check_key_space(ref string text_add)
        {
            if (currKeycode390 == (int)Keys.Space)
            {
                text_add = " ";
                return true;
            }
            else if (currKeycode390 == (int)Keys.Enter)
            {
                text_add = "\n";
                return true;
            }

            return false;
        }
        public bool check_key_symbols_hangul(ref string text_add)
        {
            text_add = null;

            switch (currKeycode390)
            {
                case -(int)Keys.T:
                    text_add = ";";
                    break;
                case -(int)Keys.Y:
                    text_add = "<";
                    break;
                case -(int)Keys.U:
                    text_add = "7";
                    break;
                case -(int)Keys.I:
                    text_add = "8";
                    break;
                case -(int)Keys.O:
                    text_add = "9";
                    break;
                case -(int)Keys.P:
                    text_add = ">";
                    break;
                case -(int)Keys.G:
                    text_add = "/";
                    break;
                case -(int)Keys.H:
                    text_add = "\'";
                    break;
                case -(int)Keys.J:
                    text_add = "4";
                    break;
                case -(int)Keys.K:
                    text_add = "5";
                    break;
                case -(int)Keys.L:
                    text_add = "6";
                    break;
                case -(int)Keys.B:
                    text_add = "!";
                    break;
                case -(int)Keys.N:
                    text_add = "0";
                    break;
                case -(int)Keys.M:
                    text_add = "1";
                    break;
                case -(int)Keys.Oemcomma:
                    text_add = "2";
                    break;
                case -(int)Keys.OemPeriod:
                    text_add = "3";
                    break;
                case -(int)Keys.OemQuestion:
                    text_add = "?";
                    break;

                case (int)Keys.Oemcomma:
                    text_add = ",";
                    break;
                case (int)Keys.OemPeriod:
                    text_add = ".";
                    break;
                case (int)Keys.OemQuestion:
                    text_add = "/";
                    break;
                case -(int)Keys.D9:
                    text_add = "(";
                    break;
                case -(int)Keys.D0:
                    text_add = ")";
                    break;


                /* 이하 영어와 겹친다. */

                //case -(int)Keys.Oemcomma:
                //    text_add = "<";
                //    break;
                //case -(int)Keys.OemPeriod:
                //    text_add = ">";
                //    break;
                //case -(int)Keys.OemQuestion:///?
                //    text_add = "?";
                //    break;

                case -(int)Keys.OemSemicolon://::
                    text_add = ":";
                    break;
                case -(int)Keys.OemQuotes://
                    text_add = "\"";
                    break;
                case -(int)Keys.OemBackslash://|
                case -(int)Keys.Oem5://|
                    text_add = "|";
                    break;
                case -(int)Keys.OemMinus:
                    text_add = "_";
                    break;
                case -(int)Keys.Oemplus://
                    text_add = "+";
                    break;
                case -(int)Keys.Oemtilde://~
                    text_add = "~";
                    break;
                case -(int)Keys.OemOpenBrackets://{
                    text_add = "{";
                    break;
                case -(int)Keys.OemCloseBrackets://}
                    text_add = "}";
                    break;

                //case (int)Keys.OemSemicolon:// ㅂ
                //    text_add = ";";
                //  break;
                //case (int)Keys.OemQuotes:// ㅌ
                //    text_add = "\'";
                //    break;
                case (int)Keys.OemBackslash://\\
                case (int)Keys.Oem5://\\
                    text_add = "\\";
                    break;
                case (int)Keys.OemMinus:
                    text_add = "-";
                    break;
                case (int)Keys.Oemplus://
                    text_add = "=";
                    break;
                case (int)Keys.Oemtilde://~
                    text_add = "`";
                    break;
                case (int)Keys.OemOpenBrackets://{
                    text_add = "[";
                    break;
                case (int)Keys.OemCloseBrackets://}
                    text_add = "]";
                    break;
            }

            return text_add != null;
        }
        public bool check_key_symbols_english(ref string text_add)
        {
            text_add = null;

            switch (currKeycode390)// !SHIFT
            {
                case (int)Keys.OemSemicolon://:
                    text_add = ";";
                    break;
                case (int)Keys.Oemcomma:
                    text_add = ",";
                    break;
                case (int)Keys.OemPeriod:
                    text_add = ".";
                    break;
                case (int)Keys.OemQuestion:///?
                    text_add = "/";
                    break;
                case (int)Keys.OemQuotes://
                    text_add = "\'";
                    break;
                case (int)Keys.OemBackslash://\\
                case (int)Keys.Oem5://\\
                    text_add = "\\";
                    break;
                case (int)Keys.OemMinus:
                    text_add = "-";
                    break;
                case (int)Keys.Oemplus://
                    text_add = "=";
                    break;
                case (int)Keys.Oemtilde://~
                    text_add = "`";
                    break;
                case (int)Keys.OemOpenBrackets://{
                    text_add = "[";
                    break;
                case (int)Keys.OemCloseBrackets://}
                    text_add = "]";
                    break;
            }

            // SHIFT + 
            switch (currKeycode390)
            {
                case -(int)Keys.OemSemicolon://::
                    text_add = ":";
                    break;
                case -(int)Keys.Oemcomma:
                    text_add = "<";
                    break;
                case -(int)Keys.OemPeriod:
                    text_add = ">";
                    break;
                case -(int)Keys.OemQuestion:///?
                    text_add = "?";
                    break;
                case -(int)Keys.OemQuotes://
                    text_add = "\"";
                    break;
                case -(int)Keys.OemBackslash://|
                case -(int)Keys.Oem5://\\
                    text_add = "|";
                    break;
                case -(int)Keys.OemMinus:
                    text_add = "_";
                    break;
                case -(int)Keys.Oemplus://
                    text_add = "+";
                    break;
                case -(int)Keys.Oemtilde://~
                    text_add = "~";
                    break;
                case -(int)Keys.OemOpenBrackets://{
                    text_add = "{";
                    break;
                case -(int)Keys.OemCloseBrackets://}
                    text_add = "}";
                    break;
            }

            return text_add != null;
        }
        public bool check_key_english(ref string text_add)
        {
            text_add = null;

            if ((int)Keys.A <= currKeycode && currKeycode <= (int)Keys.Z)
            {
                text_add = $"{(Keys)currKeycode}";

                if (SHIFT)
                    text_add = text_add.ToUpper();
                else
                    text_add = text_add.ToLower();
            }
            else if ((int)Keys.D0 <= currKeycode && currKeycode <= (int)Keys.D9)
            {
                int n = currKeycode - (int)Keys.D0;

                if (SHIFT)
                    text_add = textUpperNums.Substring(n, 1);
                else
                    text_add = $"{n}";
            }
            else
            {
                //text_add = "_";
            }

            return text_add != null;
        }
        public bool check_key_letters_hangul()
        {
            int hanIndex = -1;

            switch (uniHan.CompoJindo)
            {
                case Han.Jindo.zero:
                    if (FindChosung(ref hanIndex))
                        uniHan.SetCho(hanIndex);

                    break;
                case Han.Jindo.Cho:
                    if (FindChosung(ref hanIndex))
                        uniHan.SetCho(hanIndex);
                    else if (FindJungsung(ref hanIndex))
                        uniHan.SetJung(hanIndex);

                    break;
                case Han.Jindo.ChoX:
                    if (FindJungsung(ref hanIndex))
                        uniHan.SetJung(hanIndex);
                    break;
                case Han.Jindo.Jung:
                    if (FindJungsung(ref hanIndex))
                        uniHan.SetJung(hanIndex);
                    else if (FindZongsung(ref hanIndex))
                        uniHan.SetZong(hanIndex);
                    else if (FindChosung(ref hanIndex))
                        uniHan.SetCho(hanIndex);
                    break;
                case Han.Jindo.JungX:
                case Han.Jindo.Zong:
                    if (FindZongsung(ref hanIndex))
                        uniHan.SetZong(hanIndex);
                    else if (FindChosung(ref hanIndex))
                        uniHan.SetCho(hanIndex);
                    break;
                case Han.Jindo.ZongX:
                    if (FindChosung(ref hanIndex))
                        uniHan.SetCho(hanIndex);

                    break;
            }

            return hanIndex > -1;
        }
        public bool check_key_hangul_missing()
        {
            if ((int)Keys.A <= currKeycode && currKeycode <= (int)Keys.Z)
                return true;
            if ((int)Keys.D0 <= currKeycode390 && currKeycode390 <= (int)Keys.D9)//none shift
                return true;

            return false;
        }
        public bool check_key_finish_compo()
        {
            switch ((Keys)currKeycode)
            {
                case Keys.Oemcomma:
                case Keys.OemPeriod:
                case Keys.Sleep:
                case Keys.OemSemicolon:
                case Keys.OemQuotes:

                    return true;
            }

            return false;
        }
        bool FindChosung(ref int hanIndex)
        {
            hanIndex = -1;

            if (uniHan.CompoJindo == Han.Jindo.Cho)
            {
                if (preKeycode390 == currKeycode390)
                {
                    if (currKeycode390 == Keycode390Cho["ㄱ"])
                        hanIndex = ChoIndex["ㄲ"];
                    else if (currKeycode390 == Keycode390Cho["ㄷ"])
                        hanIndex = ChoIndex["ㄸ"];
                    else if (currKeycode390 == Keycode390Cho["ㅂ"])
                        hanIndex = ChoIndex["ㅃ"];
                    else if (currKeycode390 == Keycode390Cho["ㅅ"])
                        hanIndex = ChoIndex["ㅆ"];
                    else if (currKeycode390 == Keycode390Cho["ㅈ"])
                        hanIndex = ChoIndex["ㅉ"];
                }
            }
            else
            {
                if (currKeycode390 == Keycode390Cho["ㄱ"])
                    hanIndex = ChoIndex["ㄱ"];
                else if (currKeycode390 == Keycode390Cho["ㄷ"])
                    hanIndex = ChoIndex["ㄷ"];
                else if (currKeycode390 == Keycode390Cho["ㅂ"])
                    hanIndex = ChoIndex["ㅂ"];
                else if (currKeycode390 == Keycode390Cho["ㅅ"])
                    hanIndex = ChoIndex["ㅅ"];
                else if (currKeycode390 == Keycode390Cho["ㅈ"])
                    hanIndex = ChoIndex["ㅈ"];

                else if (currKeycode390 == Keycode390Cho["ㄴ"])
                    hanIndex = ChoIndex["ㄴ"];
                else if (currKeycode390 == Keycode390Cho["ㄹ"])
                    hanIndex = ChoIndex["ㄹ"];
                else if (currKeycode390 == Keycode390Cho["ㅁ"])
                    hanIndex = ChoIndex["ㅁ"];
                else if (currKeycode390 == Keycode390Cho["ㅇ"])
                    hanIndex = ChoIndex["ㅇ"];
                else if (currKeycode390 == Keycode390Cho["ㅊ"])
                    hanIndex = ChoIndex["ㅊ"];
                else if (currKeycode390 == Keycode390Cho["ㅋ"])
                    hanIndex = ChoIndex["ㅋ"];
                else if (currKeycode390 == Keycode390Cho["ㅌ"])
                    hanIndex = ChoIndex["ㅌ"];
                else if (currKeycode390 == Keycode390Cho["ㅍ"])
                    hanIndex = ChoIndex["ㅍ"];
                else if (currKeycode390 == Keycode390Cho["ㅎ"])
                    hanIndex = ChoIndex["ㅎ"];
            }

            return hanIndex > -1;
        }
        bool FindJungsung(ref int hanIndex)
        {
            hanIndex = -1;

            if (uniHan.CompoJindo == Han.Jindo.Jung)
            {
                if (preKeycode390 == Keycode390Jung["ㅗ"])
                {
                    if (currKeycode390 == Keycode390Jung["ㅏ"])
                        hanIndex = JungIndex["ㅘ"];
                    else if (currKeycode390 == Keycode390Jung["ㅐ"])
                        hanIndex = JungIndex["ㅙ"];
                    else if (currKeycode390 == Keycode390Jung["ㅣ"])
                        hanIndex = JungIndex["ㅚ"];
                }
                else if (preKeycode390 == Keycode390Jung["ㅜ"])
                {
                    if (currKeycode390 == Keycode390Jung["ㅓ"])
                        hanIndex = JungIndex["ㅝ"];
                    else if (currKeycode390 == Keycode390Jung["ㅔ"])
                        hanIndex = JungIndex["ㅞ"];
                    else if (currKeycode390 == Keycode390Jung["ㅣ"])
                        hanIndex = JungIndex["ㅟ"];
                }
            }
            else
            {
                if (currKeycode390 == Keycode390Jung["ㅗ"])
                    hanIndex = JungIndex["ㅗ"];
                else if (currKeycode390 == Keycode390Jung["ㅜ"])
                    hanIndex = JungIndex["ㅜ"];
                else if (currKeycode390 == Keycode390Jung["ㅏ"])
                    hanIndex = JungIndex["ㅏ"];
                else if (currKeycode390 == Keycode390Jung["ㅓ"])
                    hanIndex = JungIndex["ㅓ"];
                else if (currKeycode390 == Keycode390Jung["ㅑ"])
                    hanIndex = JungIndex["ㅑ"];
                else if (currKeycode390 == Keycode390Jung["ㅕ"])
                    hanIndex = JungIndex["ㅕ"];
                else if (currKeycode390 == Keycode390Jung["ㅛ"])
                    hanIndex = JungIndex["ㅛ"];
                else if (currKeycode390 == Keycode390Jung["ㅠ"])
                    hanIndex = JungIndex["ㅠ"];

                else if (currKeycode390 == Keycode390Jung["ㅡ"])
                    hanIndex = JungIndex["ㅡ"];
                else if (currKeycode390 == Keycode390Jung["ㅣ"])
                    hanIndex = JungIndex["ㅣ"];
                else if (currKeycode390 == Keycode390Jung["ㅐ"])
                    hanIndex = JungIndex["ㅐ"];
                else if (currKeycode390 == Keycode390Jung["ㅒ"])
                    hanIndex = JungIndex["ㅒ"];
                else if (currKeycode390 == Keycode390Jung["ㅔ"])
                    hanIndex = JungIndex["ㅔ"];
                else if (currKeycode390 == Keycode390Jung["ㅖ"])
                    hanIndex = JungIndex["ㅖ"];
                else if (currKeycode390 == Keycode390Jung["ㅢ"])
                    hanIndex = JungIndex["ㅢ"];
            }

            return hanIndex > -1;
        }
        bool FindZongsung(ref int hanIndex)
        {
            hanIndex = -1;

            if (uniHan.CompoJindo == Han.Jindo.Zong)
            {
                if (preKeycode390 == Keycode390Zong["ㄱ"])
                {
                    if (currKeycode390 == Keycode390Zong["ㅅ"])
                        hanIndex = ZongIndex["ㄳ"];
                }
                else if (preKeycode390 == Keycode390Zong["ㄴ"])
                {
                    if (currKeycode390 == Keycode390Zong["ㅈ"])
                        hanIndex = ZongIndex["ㄵ"];
                }
                else if (preKeycode390 == Keycode390Zong["ㄹ"])
                {
                    if (currKeycode390 == Keycode390Zong["ㅂ"])
                        hanIndex = ZongIndex["ㄼ"];
                    else if (currKeycode390 == Keycode390Zong["ㅅ"])
                        hanIndex = ZongIndex["ㄽ"];
                    else if (currKeycode390 == Keycode390Zong["ㅌ"])
                        hanIndex = ZongIndex["ㄾ"];
                    else if (currKeycode390 == Keycode390Zong["ㅍ"])
                        hanIndex = ZongIndex["ㄿ"];
                }
            }
            else //if ( Jindo == Jung , JungX )
            {
                if (currKeycode390 == Keycode390Zong["ㄱ"])
                    hanIndex = ZongIndex["ㄱ"];
                else if (currKeycode390 == Keycode390Zong["ㄲ"])
                    hanIndex = ZongIndex["ㄲ"];
                else if (currKeycode390 == Keycode390Zong["ㄴ"])
                    hanIndex = ZongIndex["ㄴ"];
                else if (currKeycode390 == Keycode390Zong["ㄶ"])
                    hanIndex = ZongIndex["ㄶ"];
                else if (currKeycode390 == Keycode390Zong["ㄷ"])
                    hanIndex = ZongIndex["ㄷ"];
                else if (currKeycode390 == Keycode390Zong["ㄹ"])
                    hanIndex = ZongIndex["ㄹ"];
                else if (currKeycode390 == Keycode390Zong["ㄺ"])
                    hanIndex = ZongIndex["ㄺ"];

                else if (currKeycode390 == Keycode390Zong["ㄻ"])
                    hanIndex = ZongIndex["ㄻ"];
                else if (currKeycode390 == Keycode390Zong["ㅀ"])
                    hanIndex = ZongIndex["ㅀ"];
                else if (currKeycode390 == Keycode390Zong["ㅁ"])
                    hanIndex = ZongIndex["ㅁ"];
                else if (currKeycode390 == Keycode390Zong["ㅂ"])
                    hanIndex = ZongIndex["ㅂ"];

                else if (currKeycode390 == Keycode390Zong["ㅄ"])
                    hanIndex = ZongIndex["ㅄ"];
                else if (currKeycode390 == Keycode390Zong["ㅅ"])
                    hanIndex = ZongIndex["ㅅ"];
                else if (currKeycode390 == Keycode390Zong["ㅆ"])
                    hanIndex = ZongIndex["ㅆ"];
                else if (currKeycode390 == Keycode390Zong["ㅇ"])
                    hanIndex = ZongIndex["ㅇ"];

                else if (currKeycode390 == Keycode390Zong["ㅈ"])
                    hanIndex = ZongIndex["ㅈ"];
                else if (currKeycode390 == Keycode390Zong["ㅊ"])
                    hanIndex = ZongIndex["ㅊ"];
                else if (currKeycode390 == Keycode390Zong["ㅋ"])
                    hanIndex = ZongIndex["ㅋ"];
                else if (currKeycode390 == Keycode390Zong["ㅌ"])
                    hanIndex = ZongIndex["ㅌ"];

                else if (currKeycode390 == Keycode390Zong["ㅍ"])
                    hanIndex = ZongIndex["ㅍ"];
                else if (currKeycode390 == Keycode390Zong["ㅎ"])
                    hanIndex = ZongIndex["ㅎ"];
            }

            return hanIndex > -1;
        }

        Dictionary<string, int> Keycode390Cho = new Dictionary<string, int>();
        Dictionary<string, int> Keycode390Jung = new Dictionary<string, int>();
        Dictionary<string, int> Keycode390Zong = new Dictionary<string, int>();

        Dictionary<string, int> ChoIndex = new Dictionary<string, int>();
        Dictionary<string, int> JungIndex = new Dictionary<string, int>();
        Dictionary<string, int> ZongIndex = new Dictionary<string, int>();
        void fillDics390Keycodes()
        {
            Keycode390Cho.Clear();
            Keycode390Cho.Add("ㄱ", (int)Keys.K);
            Keycode390Cho.Add("ㄴ", (int)Keys.H);
            Keycode390Cho.Add("ㄷ", (int)Keys.U);
            Keycode390Cho.Add("ㄹ", (int)Keys.Y);
            Keycode390Cho.Add("ㅁ", (int)Keys.I);
            Keycode390Cho.Add("ㅂ", (int)Keys.OemSemicolon);
            Keycode390Cho.Add("ㅅ", (int)Keys.N);
            Keycode390Cho.Add("ㅇ", (int)Keys.J);//ㅇㅇ
            Keycode390Cho.Add("ㅈ", (int)Keys.L);//ㅉ
            Keycode390Cho.Add("ㅊ", (int)Keys.O);
            Keycode390Cho.Add("ㅋ", (int)Keys.D0);
            Keycode390Cho.Add("ㅌ", (int)Keys.OemQuotes);
            Keycode390Cho.Add("ㅍ", (int)Keys.P);
            Keycode390Cho.Add("ㅎ", (int)Keys.M);

            Keycode390Jung.Clear();
            Keycode390Jung.Add("ㅏ", (int)Keys.F);
            Keycode390Jung.Add("ㅐ", (int)Keys.R);
            Keycode390Jung.Add("ㅑ", (int)Keys.D6);
            Keycode390Jung.Add("ㅒ", -(int)Keys.R);
            Keycode390Jung.Add("ㅓ", (int)Keys.T);
            Keycode390Jung.Add("ㅔ", (int)Keys.C);
            Keycode390Jung.Add("ㅕ", (int)Keys.E);
            Keycode390Jung.Add("ㅖ", (int)Keys.D7);
            Keycode390Jung.Add("ㅗ", (int)Keys.V);
            Keycode390Jung.Add("ㅛ", (int)Keys.D4);
            Keycode390Jung.Add("ㅜ", (int)Keys.D9);
            Keycode390Jung.Add("ㅠ", (int)Keys.D5);
            Keycode390Jung.Add("ㅡ", (int)Keys.G);
            Keycode390Jung.Add("ㅢ", (int)Keys.D8);
            Keycode390Jung.Add("ㅣ", (int)Keys.D);

            Keycode390Zong.Clear();
            Keycode390Zong.Add("ㄱ", (int)Keys.X);
            Keycode390Zong.Add("ㄲ", -(int)Keys.F);
            Keycode390Zong.Add("ㄴ", (int)Keys.S);
            Keycode390Zong.Add("ㄶ", -(int)Keys.S);
            Keycode390Zong.Add("ㄷ", -(int)Keys.A);
            Keycode390Zong.Add("ㄹ", (int)Keys.W);
            Keycode390Zong.Add("ㄺ", -(int)Keys.D);
            Keycode390Zong.Add("ㄻ", -(int)Keys.C);
            Keycode390Zong.Add("ㅀ", -(int)Keys.V);
            Keycode390Zong.Add("ㅁ", (int)Keys.Z);
            Keycode390Zong.Add("ㅂ", (int)Keys.D3);
            Keycode390Zong.Add("ㅄ", -(int)Keys.X);
            Keycode390Zong.Add("ㅅ", (int)Keys.Q);
            Keycode390Zong.Add("ㅆ", (int)Keys.D2);
            Keycode390Zong.Add("ㅇ", (int)Keys.A);
            Keycode390Zong.Add("ㅈ", -(int)Keys.D1);
            Keycode390Zong.Add("ㅊ", -(int)Keys.Z);
            Keycode390Zong.Add("ㅋ", -(int)Keys.E);
            Keycode390Zong.Add("ㅌ", -(int)Keys.W);
            Keycode390Zong.Add("ㅍ", -(int)Keys.Q);
            Keycode390Zong.Add("ㅎ", (int)Keys.D1);
        }
        void fillDicHangulIndexs()
        {
            ChoIndex.Clear();
            ChoIndex.Add("ㄱ", 0);
            ChoIndex.Add("ㄲ", 1);
            ChoIndex.Add("ㄴ", 2);
            ChoIndex.Add("ㄷ", 3);
            ChoIndex.Add("ㄸ", 4);
            ChoIndex.Add("ㄹ", 5);
            ChoIndex.Add("ㅁ", 6);
            ChoIndex.Add("ㅂ", 7);
            ChoIndex.Add("ㅃ", 8);
            ChoIndex.Add("ㅅ", 9);
            ChoIndex.Add("ㅆ", 10);
            ChoIndex.Add("ㅇ", 11);
            ChoIndex.Add("ㅈ", 12);
            ChoIndex.Add("ㅉ", 13);
            ChoIndex.Add("ㅊ", 14);
            ChoIndex.Add("ㅋ", 15);
            ChoIndex.Add("ㅌ", 16);
            ChoIndex.Add("ㅍ", 17);
            ChoIndex.Add("ㅎ", 18);

            JungIndex.Clear();
            JungIndex.Add("ㅏ", 0);
            JungIndex.Add("ㅐ", 1);
            JungIndex.Add("ㅑ", 2);
            JungIndex.Add("ㅒ", 3);
            JungIndex.Add("ㅓ", 4);
            JungIndex.Add("ㅔ", 5);
            JungIndex.Add("ㅕ", 6);
            JungIndex.Add("ㅖ", 7);
            JungIndex.Add("ㅗ", 8);
            JungIndex.Add("ㅘ", 9);
            JungIndex.Add("ㅙ", 10);
            JungIndex.Add("ㅚ", 11);
            JungIndex.Add("ㅛ", 12);
            JungIndex.Add("ㅜ", 13);
            JungIndex.Add("ㅝ", 14);
            JungIndex.Add("ㅞ", 15);
            JungIndex.Add("ㅟ", 16);
            JungIndex.Add("ㅠ", 17);
            JungIndex.Add("ㅡ", 18);
            JungIndex.Add("ㅢ", 19);
            JungIndex.Add("ㅣ", 20);

            ZongIndex.Clear();
            ZongIndex.Add("X", 0);
            ZongIndex.Add("ㄱ", 1);
            ZongIndex.Add("ㄲ", 2);
            ZongIndex.Add("ㄳ", 3);
            ZongIndex.Add("ㄴ", 4);
            ZongIndex.Add("ㄵ", 5);
            ZongIndex.Add("ㄶ", 6);
            ZongIndex.Add("ㄷ", 7);
            ZongIndex.Add("ㄹ", 8);
            ZongIndex.Add("ㄺ", 9);
            ZongIndex.Add("ㄻ", 10);
            ZongIndex.Add("ㄼ", 11);
            ZongIndex.Add("ㄽ", 12);
            ZongIndex.Add("ㄾ", 13);
            ZongIndex.Add("ㄿ", 14);
            ZongIndex.Add("ㅀ", 15);
            ZongIndex.Add("ㅁ", 16);
            ZongIndex.Add("ㅂ", 17);
            ZongIndex.Add("ㅄ", 18);
            ZongIndex.Add("ㅅ", 19);
            ZongIndex.Add("ㅆ", 20);
            ZongIndex.Add("ㅇ", 21);
            ZongIndex.Add("ㅈ", 22);
            ZongIndex.Add("ㅊ", 23);
            ZongIndex.Add("ㅋ", 24);
            ZongIndex.Add("ㅌ", 25);
            ZongIndex.Add("ㅍ", 26);
            ZongIndex.Add("ㅎ", 27);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace shine.libs.serial
{
    public class MacroKeys
    {
        public static Keys parseKey(string keyx)
        {
            if (keyx == null)
                return Keys.None;

            switch (keyx.ToLower())
            {
                case "null": return Keys.None;
                case "none": return Keys.None;
                case "control": return Keys.LControlKey;
                case "ctrl": return Keys.LControlKey;
                case "shift": return Keys.LShiftKey;
                case "alt": return Keys.LMenu;
                case "win": return Keys.LWin;
                case "tab": return Keys.Tab;
                case "space": return Keys.Space;
                case "escape": return Keys.Escape;
                case "home": return Keys.Home;
                case "end": return Keys.End;
                case "delete": return Keys.Delete;
                case "insert": return Keys.Insert;
                case "pgdn": return Keys.Next;
                case "pgup": return Keys.Prior;
                case "return": case "enter": return Keys.Return;

                case "up": case "▲": return Keys.Up;
                case "down": case "▼": return Keys.Down;
                case "left": case "◀": return Keys.Left;
                case "right": case "▶": return Keys.Right;

                case "~": return Keys.Oemtilde;

                case "a": return Keys.A;
                case "b": return Keys.B;
                case "c": return Keys.C;
                case "d": return Keys.D;
                case "e": return Keys.E;
                case "f": return Keys.F;
                case "g": return Keys.G;

                case "h": return Keys.H;
                case "i": return Keys.I;
                case "j": return Keys.J;
                case "k": return Keys.K;
                case "l": return Keys.L;
                case "m": return Keys.M;
                case "n": return Keys.N;

                case "o": return Keys.O;
                case "p": return Keys.P;
                case "q": return Keys.Q;
                case "r": return Keys.R;
                case "s": return Keys.S;
                case "t": return Keys.T;
                case "u": return Keys.U;

                case "v": return Keys.V;
                case "w": return Keys.W;
                case "x": return Keys.X;
                case "y": return Keys.Y;
                case "z": return Keys.Z;

                case "0": return Keys.D0;
                case "1": return Keys.D1;
                case "2": return Keys.D2;
                case "3": return Keys.D3;
                case "4": return Keys.D4;
                case "5": return Keys.D5;
                case "6": return Keys.D6;
                case "7": return Keys.D7;
                case "8": return Keys.D8;
                case "9": return Keys.D9;

                case "f1": return Keys.F1;
                case "f2": return Keys.F2;
                case "f3": return Keys.F3;
                case "f4": return Keys.F4;
                case "f5": return Keys.F5;
                case "f6": return Keys.F6;
                case "f7": return Keys.F7;
                case "f8": return Keys.F8;
                case "f9": return Keys.F9;
                case "f10": return Keys.F10;
                case "f11": return Keys.F11;
                case "f12": return Keys.F12;
            }

            return Keys.None;
        }
        public static List<Keys> parseKeys(string keysx)
        {
            if ("%" == keysx.Substring(0, 1))
            {
                List<Keys> keys = new List<Keys>();
                var _eysx = keysx.Substring(1);
                
                foreach (var key in _eysx)
                    keys.Add(parseKey("" + key));

                return keys;
            }
            else
                return new List<Keys> { parseKey(keysx) };
        }
    }
}

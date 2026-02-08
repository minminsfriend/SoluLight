using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using shine.libs.hangul;

namespace shine.libs.serial
{
    public class ToArduKeys
    {
        public static ArduKeys Ardu(Keys key)
        {
            if (key == Keys.None)
                return ArduKeys.NONE;

            switch(key)
            {
                case Keys.A: return ArduKeys.A;
                case Keys.B: return ArduKeys.B;
                case Keys.C: return ArduKeys.C;
                case Keys.D: return ArduKeys.D;
                case Keys.E: return ArduKeys.E;
                case Keys.F: return ArduKeys.F;
                case Keys.G: return ArduKeys.G;
                case Keys.H: return ArduKeys.H;
                case Keys.I: return ArduKeys.I;
                case Keys.J: return ArduKeys.J;
                case Keys.K: return ArduKeys.K;
                case Keys.L: return ArduKeys.L;
                case Keys.M: return ArduKeys.M;
                case Keys.N: return ArduKeys.N;
                case Keys.O: return ArduKeys.O;
                case Keys.P: return ArduKeys.P;
                case Keys.Q: return ArduKeys.Q;
                case Keys.R: return ArduKeys.R;
                case Keys.S: return ArduKeys.S;
                case Keys.T: return ArduKeys.T;
                case Keys.U: return ArduKeys.U;
                case Keys.V: return ArduKeys.V;
                case Keys.W: return ArduKeys.W;
                case Keys.X: return ArduKeys.X;
                case Keys.Y: return ArduKeys.Y;
                case Keys.Z: return ArduKeys.Z;

                case Keys.D0: return ArduKeys.D0;
                case Keys.D1: return ArduKeys.D1;
                case Keys.D2: return ArduKeys.D2;
                case Keys.D3: return ArduKeys.D3;
                case Keys.D4: return ArduKeys.D4;
                case Keys.D5: return ArduKeys.D5;
                case Keys.D6: return ArduKeys.D6;
                case Keys.D7: return ArduKeys.D7;
                case Keys.D8: return ArduKeys.D8;
                case Keys.D9: return ArduKeys.D9;

                case Keys.F1: return ArduKeys.F1;
                case Keys.F2: return ArduKeys.F2;
                case Keys.F3: return ArduKeys.F3;
                case Keys.F4: return ArduKeys.F4;
                case Keys.F5: return ArduKeys.F5;
                case Keys.F6: return ArduKeys.F6;
                case Keys.F7: return ArduKeys.F7;
                case Keys.F8: return ArduKeys.F8;
                case Keys.F9: return ArduKeys.F9;
                case Keys.F10: return ArduKeys.F10;
                case Keys.F11: return ArduKeys.F11;
                case Keys.F12: return ArduKeys.F12;

                case Keys.Right: return ArduKeys.RIGHT_ARROW;
                case Keys.Left: return ArduKeys.LEFT_ARROW;
                case Keys.Down: return ArduKeys.DOWN_ARROW;
                case Keys.Up: return ArduKeys.UP_ARROW;
                
                case Keys.Insert: return ArduKeys.INSERT;
                case Keys.Delete: return ArduKeys.DELETE;
                //case Keys.PageUp: return ArduKeys.PAGE_UP;
                //case Keys.PageDown: return ArduKeys.PAGE_DOWN;
                case Keys.Prior: return ArduKeys.PAGE_UP;
                case Keys.Next: return ArduKeys.PAGE_DOWN;
                case Keys.Home: return ArduKeys.HOME;
                case Keys.End: return ArduKeys.END;
                case Keys.CapsLock: return ArduKeys.CAPS_LOCK;
                case Keys.PrintScreen: return ArduKeys.PRINT_SCREEN;
                case Keys.Scroll: return ArduKeys.SCROLL_LOCK;
                case Keys.Pause: return ArduKeys.PAUSE;
                case Keys.Back: return ArduKeys.BACKSPACE;
                case Keys.Tab: return ArduKeys.TAB;
                case Keys.Space: return ArduKeys.SPACE;
                case Keys.Return: return ArduKeys.RETURN;

                //case Keys.Menu: return AKeys.Menu;
                case Keys.Escape: return ArduKeys.ESC;
                case Keys.OemMinus: return ArduKeys.MINUS;
                case Keys.Oemplus: return ArduKeys.PLUS;
                case Keys.OemOpenBrackets: return ArduKeys.LEFT_BRACKET;
                case Keys.OemCloseBrackets: return ArduKeys.RIGHT_BRACKET;
                case Keys.OemBackslash: return ArduKeys.BACKSLASH;
                case Keys.Oemtilde: return ArduKeys.GRAVE;
                case Keys.OemQuotes: return ArduKeys.QUOTO;
                case Keys.Oemcomma: return ArduKeys.COMMA;
                case Keys.OemPeriod: return ArduKeys.PERIOD;
                case Keys.OemQuestion: return ArduKeys.SLASH;
                case Keys.OemSemicolon: return ArduKeys.SEMICOLON;

                case Keys.LControlKey: return ArduKeys.LEFT_CTRL;
                case Keys.LShiftKey: return ArduKeys.LEFT_SHIFT;
                case Keys.LMenu: return ArduKeys.LEFT_ALT;

                case Keys.RControlKey: return ArduKeys.LEFT_CTRL;
                case Keys.RShiftKey: return ArduKeys.LEFT_SHIFT;
                case Keys.RMenu: return ArduKeys.LEFT_ALT;

                case Keys.ControlKey: return ArduKeys.LEFT_CTRL;
                case Keys.ShiftKey: return ArduKeys.LEFT_SHIFT;
                case Keys.Menu: return ArduKeys.LEFT_ALT;

                case Keys.LWin: return ArduKeys.LEFT_GUI;
                case Keys.RWin: return ArduKeys.LEFT_GUI;

            }

            return ArduKeys.NONE;
        }

        public static ArduKeys Ardu(int key)
        {
            if (key == WKeyCode.NONAME)
                return ArduKeys.NONE;

            switch (key)
            {
                case WKeyCode.A: return ArduKeys.A;
                case WKeyCode.B: return ArduKeys.B;
                case WKeyCode.C: return ArduKeys.C;
                case WKeyCode.D: return ArduKeys.D;
                case WKeyCode.E: return ArduKeys.E;
                case WKeyCode.F: return ArduKeys.F;
                case WKeyCode.G: return ArduKeys.G;
                case WKeyCode.H: return ArduKeys.H;
                case WKeyCode.I: return ArduKeys.I;
                case WKeyCode.J: return ArduKeys.J;
                case WKeyCode.K: return ArduKeys.K;
                case WKeyCode.L: return ArduKeys.L;
                case WKeyCode.M: return ArduKeys.M;
                case WKeyCode.N: return ArduKeys.N;
                case WKeyCode.O: return ArduKeys.O;
                case WKeyCode.P: return ArduKeys.P;
                case WKeyCode.Q: return ArduKeys.Q;
                case WKeyCode.R: return ArduKeys.R;
                case WKeyCode.S: return ArduKeys.S;
                case WKeyCode.T: return ArduKeys.T;
                case WKeyCode.U: return ArduKeys.U;
                case WKeyCode.V: return ArduKeys.V;
                case WKeyCode.W: return ArduKeys.W;
                case WKeyCode.X: return ArduKeys.X;
                case WKeyCode.Y: return ArduKeys.Y;
                case WKeyCode.Z: return ArduKeys.Z;

                case WKeyCode.N0: return ArduKeys.D0;
                case WKeyCode.N1: return ArduKeys.D1;
                case WKeyCode.N2: return ArduKeys.D2;
                case WKeyCode.N3: return ArduKeys.D3;
                case WKeyCode.N4: return ArduKeys.D4;
                case WKeyCode.N5: return ArduKeys.D5;
                case WKeyCode.N6: return ArduKeys.D6;
                case WKeyCode.N7: return ArduKeys.D7;
                case WKeyCode.N8: return ArduKeys.D8;
                case WKeyCode.N9: return ArduKeys.D9;

                case WKeyCode.F1: return ArduKeys.F1;
                case WKeyCode.F2: return ArduKeys.F2;
                case WKeyCode.F3: return ArduKeys.F3;
                case WKeyCode.F4: return ArduKeys.F4;
                case WKeyCode.F5: return ArduKeys.F5;
                case WKeyCode.F6: return ArduKeys.F6;
                case WKeyCode.F7: return ArduKeys.F7;
                case WKeyCode.F8: return ArduKeys.F8;
                case WKeyCode.F9: return ArduKeys.F9;
                case WKeyCode.F10: return ArduKeys.F10;
                case WKeyCode.F11: return ArduKeys.F11;
                case WKeyCode.F12: return ArduKeys.F12;

                case WKeyCode.RIGHT: return ArduKeys.RIGHT_ARROW;
                case WKeyCode.LEFT: return ArduKeys.LEFT_ARROW;
                case WKeyCode.DOWN: return ArduKeys.DOWN_ARROW;
                case WKeyCode.UP: return ArduKeys.UP_ARROW;

                case WKeyCode.INSERT: return ArduKeys.INSERT;
                case WKeyCode.DELETE: return ArduKeys.DELETE;
                //case WKeyCode.PageUp: return ArduKeys.PAGE_UP;
                //case WKeyCode.PageDown: return ArduKeys.PAGE_DOWN;
                case WKeyCode.PRIOR: return ArduKeys.PAGE_UP;
                case WKeyCode.NEXT: return ArduKeys.PAGE_DOWN;
                case WKeyCode.HOME: return ArduKeys.HOME;
                case WKeyCode.END: return ArduKeys.END;
                case WKeyCode.CAPITAL: return ArduKeys.CAPS_LOCK;
                case WKeyCode.PRINT: return ArduKeys.PRINT_SCREEN;
                case WKeyCode.SCROLL: return ArduKeys.SCROLL_LOCK;
                case WKeyCode.PAUSE: return ArduKeys.PAUSE;
                case WKeyCode.BACK: return ArduKeys.BACKSPACE;
                case WKeyCode.TAB: return ArduKeys.TAB;
                case WKeyCode.SPACE: return ArduKeys.SPACE;
                case WKeyCode.RETURN: return ArduKeys.RETURN;

                //case WKeyCode.Menu: return AKeys.Menu;
                case WKeyCode.ESCAPE: return ArduKeys.ESC;
                case WKeyCode.MINUS: return ArduKeys.MINUS;
                case WKeyCode.PLUS: return ArduKeys.PLUS;
                case WKeyCode.LEFTBRACKET: return ArduKeys.LEFT_BRACKET;
                case WKeyCode.RIGHTBRACKET: return ArduKeys.RIGHT_BRACKET;
                case WKeyCode.BACKSLASH: return ArduKeys.BACKSLASH;
                case WKeyCode.GRAVE: return ArduKeys.GRAVE;
                case WKeyCode.APOSTROPHE: return ArduKeys.QUOTO;
                case WKeyCode.COMMA: return ArduKeys.COMMA;
                case WKeyCode.PERIOD: return ArduKeys.PERIOD;
                case WKeyCode.SLASH: return ArduKeys.SLASH;
                case WKeyCode.SEMICOLON: return ArduKeys.SEMICOLON;

                case WKeyCode.LCONTROL: return ArduKeys.LEFT_CTRL;
                case WKeyCode.LSHIFT: return ArduKeys.LEFT_SHIFT;
                case WKeyCode.LMENU: return ArduKeys.LEFT_ALT;
                case WKeyCode.LWIN: return ArduKeys.LEFT_GUI;

                //case WKeyCode.RCONTROL: return ArduKeys.LEFT_CTRL;
                //case WKeyCode.RSHIFT: return ArduKeys.LEFT_SHIFT;
                case WKeyCode.RMENU: return ArduKeys.LEFT_ALT;
                case WKeyCode.RWIN: return ArduKeys.LEFT_GUI;

                case WKeyCode.NUMPAD0: return ArduKeys.KP_0;
                case WKeyCode.NUMPAD1: return ArduKeys.KP_1;
                case WKeyCode.NUMPAD2: return ArduKeys.KP_2;
                case WKeyCode.NUMPAD3: return ArduKeys.KP_3;
                case WKeyCode.NUMPAD4: return ArduKeys.KP_4;
                case WKeyCode.NUMPAD5: return ArduKeys.KP_5;
                case WKeyCode.NUMPAD6: return ArduKeys.KP_6;
                case WKeyCode.NUMPAD7: return ArduKeys.KP_7;
                case WKeyCode.NUMPAD8: return ArduKeys.KP_8;
                case WKeyCode.NUMPAD9: return ArduKeys.KP_9;
                case WKeyCode.DECIMAL: return ArduKeys.KP_DOT;
                case WKeyCode.DIVIDE: return ArduKeys.KP_SLASH;
                case WKeyCode.MULTIPLY: return ArduKeys.KP_ASTERISK;
                case WKeyCode.ADD: return ArduKeys.KP_PLUS;
                case WKeyCode.SUBTRACT: return ArduKeys.KP_MINUS;
            }

            return ArduKeys.NONE;
        }

        static ArduKeys FromHostKey(int key)
        {
            if (4 <= key && key <= 29)//a~z
                return (ArduKeys)(97 + (key - 4));
            else if (30 <= key && key <= 39)//1~0
                return (ArduKeys)(166 + (key - 30));
            else if (58 <= key && key <= 69)//f1~f12
                return (ArduKeys)((int)ArduKeys.F1 + (key - 58));

            else if (key == 82) return ArduKeys.UP_ARROW;
            else if (key == 81) return ArduKeys.DOWN_ARROW;
            else if (key == 80) return ArduKeys.LEFT_ARROW;
            else if (key == 79) return ArduKeys.RIGHT_ARROW;
            else if (key == 42) return ArduKeys.BACKSPACE;
            else if (key == 43) return ArduKeys.TAB;
            else if (key == 40) return ArduKeys.RETURN;
            else if (key == 41) return ArduKeys.ESC;
            else if (key == 73) return ArduKeys.INSERT;
            else if (key == 76) return ArduKeys.DELETE;
            else if (key == 75) return ArduKeys.PAGE_UP;
            else if (key == 78) return ArduKeys.PAGE_DOWN;
            else if (key == 74) return ArduKeys.HOME;
            else if (key == 77) return ArduKeys.END;
            else if (key == 57) return ArduKeys.CAPS_LOCK;
            else if (key == 70) return ArduKeys.PRINT_SCREEN;
            else if (key == 71) return ArduKeys.SCROLL_LOCK;
            else if (key == 83) return ArduKeys.NUM_LOCK;

            else if (key == 44) return ArduKeys.SPACE;//space
            else if (key == 45) return ArduKeys.MINUS;//minus
            else if (key == 46) return ArduKeys.PLUS;//plus
            else if (key == 47) return ArduKeys.LEFT_BRACKET;//left bracket
            else if (key == 48) return ArduKeys.RIGHT_BRACKET;//right bracket
            else if (key == 49) return ArduKeys.BACKSLASH;//back slash
            else if (key == 51) return ArduKeys.SEMICOLON;//semicolon
            else if (key == 52) return ArduKeys.QUOTO;//quoto
            else if (key == 53) return ArduKeys.GRAVE;//grave
            else if (key == 54) return ArduKeys.COMMA;//comma
            else if (key == 55) return ArduKeys.PERIOD;//period
            else if (key == 56) return ArduKeys.SLASH;//slash

            else if (key == 101) return ArduKeys.MENU;
            else if (key == 144) return ArduKeys.NONE;// han eng;
            else if (key == 145) return ArduKeys.NONE;// hanja;

            return ArduKeys.NONE;
        }

    }
}

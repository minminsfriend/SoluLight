using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using shine.libs.hangul;

namespace shine.libs.simul
{
    //const int KEYEVENTF_KEYDN = 0x0000;
    //const int KEYEVENTF_KEYUP = 0x0002;
    //const int KEYEVENTF_EXTENDEDKEY = 0x0001;

    public enum ACTIONS
    {
        Down = 0x0000,
        Up = 0x0002,
    }

    public enum KeyEvent
    {
        down = 0x0000,
        up = 0x0002,
        exDown = 0x0001 | 0x0000,
        exUp = 0x0001 | 0x0002,
    }

    public static class simulKeys
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static void KeyDownUp(int vkey)
        {
            stroke(ACTIONS.Down, vkey);
            stroke(ACTIONS.Up, vkey);
        }
        public static void KeyDownUp(int vkey, bool ctrl, bool shift, bool alt)
        {
            if (shift)
                stroke(ACTIONS.Down, WKeyCode.LSHIFT);
            if (ctrl)
                stroke(ACTIONS.Down, WKeyCode.LCONTROL);
            if (alt)
                stroke(ACTIONS.Down, WKeyCode.LMENU);

            stroke(ACTIONS.Down, vkey);
            stroke(ACTIONS.Up, vkey);

            if (alt)
                stroke(ACTIONS.Up, WKeyCode.LMENU);
            if (ctrl)
                stroke(ACTIONS.Up, WKeyCode.LCONTROL);
            if (shift)
                stroke(ACTIONS.Up, WKeyCode.LSHIFT);
        }
        public static void stroke(ACTIONS action, int vkey)
        {
            KeyEvent ke;

            switch (vkey)
            {
                case WKeyCode.LSHIFT:
                    ke = action == ACTIONS.Down ? KeyEvent.exDown : KeyEvent.exUp;
                    break;
                default:
                    ke = action == ACTIONS.Down ? KeyEvent.down : KeyEvent.up;
                    break;
            }

            switch (vkey)
            {
                case WKeyCode.LCONTROL:
                    keybd_event((byte)vkey, 0x9d, (int)ke, 0);

                    break;
                case WKeyCode.LMENU:
                    keybd_event((byte)vkey, 0x9d, (int)ke, 0);

                    break;
                case WKeyCode.LSHIFT:
                    keybd_event((byte)vkey, 42, (int)ke, 0);

                    break;
                case WKeyCode.BACK://up만 체크된다. 폰 엑티버티 문제
                case WKeyCode.ESCAPE:
                    if (action == ACTIONS.Up)
                    {
                        keybd_event((byte)vkey, 0x9e, (int)KeyEvent.down, 0);
                        keybd_event((byte)vkey, 0x9e, (int)KeyEvent.up, 0);
                    }

                    break;
                case -1:

                    break;
                default:
                    keybd_event((byte)vkey, 0x9e, (int)ke, 0);

                    break;
            }
        }
        public static void stroke2(ACTIONS action, int vkey)
        {
            KeyEvent ke;

            switch (vkey)
            {
                //case WKeyCode.LSHIFT:
                //    ke = action == ACTIONS.Down ? KeyEvent.exDown : KeyEvent.exUp;
                //    break;
                default:
                    ke = action == ACTIONS.Down ? KeyEvent.down : KeyEvent.up;
                    break;
            }

            switch (vkey)
            {
                //case WKeyCode.LCONTROL:
                //    keybd_event((byte)vkey, 0x9d, (int)ke, 0);

                //    break;
                //case WKeyCode.LMENU:
                //    keybd_event((byte)vkey, 0x9d, (int)ke, 0);

                //    break;
                //case WKeyCode.LSHIFT:
                //    keybd_event((byte)vkey, 42, (int)ke, 0);

                //    break;
                //case WKeyCode.BACK://up만 체크된다. 폰 엑티버티 문제
                //case WKeyCode.ESCAPE:
                //    if (action == ACTIONS.Up)
                //    {
                //        keybd_event((byte)vkey, 0x9e, (int)KeyEvent.down, 0);
                //        keybd_event((byte)vkey, 0x9e, (int)KeyEvent.up, 0);
                //    }

                //    break;
                default:
                    keybd_event((byte)vkey, 0x9e, (int)ke, 0);

                    break;
            }
        }

      
        public static void cleanSHIFT(int app)
        {
            byte scan = 42;

            if (app == 0)// 한번 더 눌러서 Shift 상태를 깨끗하게
            {
                scan = 0x9d;

                keybd_event((byte)WKeyCode.LSHIFT, scan, (int)KeyEvent.down, 0);
                keybd_event((byte)WKeyCode.LSHIFT, scan, (int)KeyEvent.up, 0);
            }
        }

        public static void Parse(string keytext)
        {
            switch (keytext)
            {
                case "xxx":

                    break;
                case "delete":
                    KeyDownUp(WKeyCode.DELETE);

                    break;
            }
        }
    }
}

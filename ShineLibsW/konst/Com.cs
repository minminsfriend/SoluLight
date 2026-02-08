using System;

namespace shine.libs.konst
{
    public static class Com
    {
        public enum MSGBOX
        {
            None = 0,
            Yes = 1,
            No = 2,
            Ok = 11,
            Cancel = 12,
        }
        public enum MOUSEMODE
        {
            None = 0,
            Left = 1,
            Middle = 2,
            Right = 3,
            Wheel = 4,
        }
        public enum BTNMODE
        {
            MAIN = 0,
            MOUSE = 1,
            WINNAVI = 10,
            WINAPP = 11,
            KEYBTN = 12,
            HOTKEY = 13,
            PLAY = 14,
            STUDIO = 20,
            SHARP = 21,
        }
        public enum BTNHARD
        {
            Home = 300,
            End = 301,
            Insert = 302,
            Delete = 303,
            LWin = 304,
            Han = 305,
            Esc = 310,
            F1 = 381,
            F2 = 382,
            F3 = 383,
            F4 = 384,
            F5 = 385,
            F6 = 386,
            F7 = 387,
            F8 = 388,
            F9 = 389,
            F10 = 390,
            F11 = 391,
            F12 = 392,
        }
        public enum BTNPLAY
        {
            Empty = 500,
            Exit = 501,
            Run = 502,
            Stop = 503,
            Request = 510,
            Resize = 511,
            ViewCreated = 512,
            DrawFrame = 520,
            RequestNext = 521,
        }
        public enum BTNNAVI
        {
            Request = 1001,
            ZoomIn = 1003,
            ZoomOut = 1004,
            moveLeft = 1010,
            moveRight = 1011,
            moveUp = 1012,
            moveDown = 1013,
            moveR00 = 1020,
            moveR01 = 1021,
            moveR02 = 1022,
            moveR03 = 1023,
            moveR10 = 1024,
            moveR11 = 1025,
            moveR12 = 1026,
            moveR13 = 1027,
            movePre = 1030,
            saveArea = 1050,
            touchIsMove = 1060,
        }
        public enum BTNHOT
        {
            CtrlAltLeft = 2004,
            CtrlAltRight = 2005,
            Ctrl4f = 2006,
            Alt4f = 2007,
            RClick = 2008,
            Save = 2008,
            Shift11f = 2010,
            Shift5f = 2011,
            ServerRdp = 2100,
            RequestFull = 2200,

        }
        public enum BTNAPP
        {
            None = 3000,
            Andro = 3001,
            Sharp = 3002,
            Total = 3003,
            Cmd = 3004,
            Tcc = 3005,
            Plus = 3006,
            Note = 3007,
            Geny = 3008,
            Chrome = 3009,
            Server = 3010,
            Gimp = 3011,
            Ink = 3012,
            Blender = 3013,
        }
        public enum COMMAND
        {
            None = 5000,
            Invalidate = 5011,
            SendRect = 5021,
        }

        public enum BTNAPK
        {
            none = -1,
            Index0 = 0,
            Index1 = 1,
            Index2 = 2,
            Index3 = 3,
            Index4 = 4,
            Index5 = 5,
            FilesList = 10,
        }
        public enum ANILEVEL
        {
            Pressed = 0,
            Released = 1,
            Moved = 2,
            Normal = 3,
            Canceled = 4,
            Repaint = 5,
            LongPressed = 10,
        }
        public enum REPAINT
        {
            No = 0,
            Yes = 1,
            YesKbd = Yes * 100,
            YesDoc = Yes * 10,
            YesMsg = Yes * 1,
        }
    }
}

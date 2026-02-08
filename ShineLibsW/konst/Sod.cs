using System;

namespace shine.libs.konst
{
	public static class Sod
	{
        public enum HCODE
        {
            none = 776206270,
            error = 760286763,
            close = 741874100,
            connect = 741874101,
            image = 749653134,
            file = 276206276,
            key = 852952895,
            text = 842852795,
        }
        public enum KM
        {
            kbd = 0,
            mouse = 1,
            hot = 2,
            camera = 3,
            vmouse = 11,
            area = 12,
            socket = 15,
        }
        public enum SOKAT
        {
            close = 0,
            connect = 1,
        }
        public enum CAPAREA
        {
            none = 0,
            tablet = 1,
            differ = 2,
            desktop = 3,
            app = 4,
        }
        public enum CAPSTYLE
        {
            none = 0,
            startpan = 1,
            //offpan = 2,
            stoppan = 3,
            setonly = 4,
            fullcap = 5,
        }
        public enum TYPE_TEXT
        {
            SYMBOL = 10,
            HANGUL = 11,
            ENGLISH = 12,
        }
        public static class MYIP
        {
            public const string home03 = "192.168.0.3";
            public const string mulgong = "mulgong.iptime.org";
        }
    }
}

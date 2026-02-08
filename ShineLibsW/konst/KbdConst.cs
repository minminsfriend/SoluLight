using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shine.libs.konst
{
    public enum MKBD
    {
        none = 0,
        hangul = 1,
        english = 2,
        symbol = 3,
        custom = 4,
    }
    public enum PADID
    {
        none = 0,
        softkbd = 10,
        writer = 11,
        msgbox = 12,
        memolist = 13,
        menupan = 14,
        navipan = 15,
        mswitch = 20,
        grimpan = 21,
        warf = 22,
        blehot = 23,
        tree = 25,
        camera = 26,
        top = 27,
        phone = 28,
        ghost=29,

        memokbd = 41,
        zog = 51,
        quicklog = 52,
        label = 53,
        gmouse = 54,
        bird = 55,

        selcate = 111,
        selkeyval = 112,
        catcode = 113,
        textman = 114,
        selicon = 115,
        colors = 116,

        layers = 151,
        seltool = 152,
    }
    public enum xPadID
    {
        none = 0,
        command = 1,
        playApp = 2,

        layer = 11,
        color = 12,
        editor = 21,

        hard = 31,
        zoom = 41,
    }
}
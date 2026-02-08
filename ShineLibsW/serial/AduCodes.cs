using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace shine.libs.serial
{
    public enum ArduKeys
    {
        NONE = 0x00,
        // Modifiers,
        LEFT_CTRL = 0x80,
        LEFT_SHIFT = 0x81,
        LEFT_ALT = 0x82,
        LEFT_GUI = 0x83,
        RIGHT_CTRL = 0x84,
        RIGHT_SHIFT = 0x85,
        RIGHT_ALT = 0x86,
        RIGHT_GUI = 0x87,

        // Misc keys,
        UP_ARROW = 0xDA,
        DOWN_ARROW = 0xD9,
        LEFT_ARROW = 0xD8,
        RIGHT_ARROW = 0xD7,

        INSERT = 0xD1,
        DELETE = 0xD4,
        PAGE_UP = 0xD3,
        PAGE_DOWN = 0xD6,
        HOME = 0xD2,
        END = 0xD5,
        CAPS_LOCK = 0xC1,
        PRINT_SCREEN = 0xCE, // Print Screen / SysRq,
        SCROLL_LOCK = 0xCF,
        PAUSE = 0xD0, // Pause / Break,

        // Numeric keypad,
        NUM_LOCK = 0xDB,
        KP_SLASH = 0xDC,
        KP_ASTERISK = 0xDD,
        KP_MINUS = 0xDE,
        KP_PLUS = 0xDF,
        KP_ENTER = 0xE0,
        KP_1 = 0xE1,
        KP_2 = 0xE2,
        KP_3 = 0xE3,
        KP_4 = 0xE4,
        KP_5 = 0xE5,
        KP_6 = 0xE6,
        KP_7 = 0xE7,
        KP_8 = 0xE8,
        KP_9 = 0xE9,
        KP_0 = 0xEA,
        KP_DOT = 0xEB,

        // Function keys,
        F1 = 0xC2,
        F2 = 0xC3,
        F3 = 0xC4,
        F4 = 0xC5,
        F5 = 0xC6,
        F6 = 0xC7,
        F7 = 0xC8,
        F8 = 0xC9,
        F9 = 0xCA,
        F10 = 0xCB,
        F11 = 0xCC,
        F12 = 0xCD,
        F13 = 0xF0,
        F14 = 0xF1,
        F15 = 0xF2,
        F16 = 0xF3,
        F17 = 0xF4,
        F18 = 0xF5,
        F19 = 0xF6,
        F20 = 0xF7,
        F21 = 0xF8,
        F22 = 0xF9,
        F23 = 0xFA,
        F24 = 0xFB,

        A = 97,
        B = 98,
        C = 99,
        D = 100,
        E = 101,
        F = 102,
        G = 103,
        H = 104,
        I = 105,
        J = 106,
        K = 107,
        L = 108,
        M = 109,
        N = 110,
        O = 111,
        P = 112,
        Q = 113,
        R = 114,
        S = 115,
        T = 116,
        U = 117,
        V = 118,
        W = 119,
        X = 120,
        Y = 121,
        Z = 122,

        // 이하 어렵게 찾은 값

        BACKSPACE = 0xB2,
        TAB = 0xB3,
        SPACE = 0xB4,
        RETURN = 0xB0,
        MENU = 0xED, // "Keyboard Application" in USB standard,
        ESC = 0xB1,

        MINUS = 0xB5,
        PLUS = 0xB6,
        LEFT_BRACKET = 0xB7,
        RIGHT_BRACKET = 0xB8,
        BACKSLASH = 0xB9,

        //xxxx=0xBA,// ?
        SEMICOLON = 0xBB,//~
        QUOTO = 0xBC,
        GRAVE = 0xBD,
        COMMA = 0xBE,
        PERIOD = 0xBF,
        SLASH = 0xC0,

        D1 = 166,
        D2 = 167,
        D3 = 168,
        D4 = 169,
        D5 = 170,
        D6 = 171,
        D7 = 172,
        D8 = 173,
        D9 = 174,
        D0 = 175,
    }
    public enum AActions
    {
        None = 0,
        MouseMove = 1,
        Wheel = 2,

        LMouseClick = 10,
        LMousePress = 11,
        LMouseRelease = 12,

        RMouseClick = 20,
        RMousePress = 21,
        RMouseRelease = 22,

        KeyClick = 50,
        KeyPress = 51,
        KeyRelease = 52,
    }
    public enum HKEY
    {
        N1 = 89,
        N2 = 90,
        N3 = 91,
        N4 = 92,
        N5 = 93,
        N6 = 94,
        N7 = 95,
        N8 = 96,
        N9 = 97,
        N0 = 98,
        DEL = 99,
        LOC = 83,
        DIV = 84,
        MUL = 85,
        SUB = 86,
        ADD = 87,
        ENT = 88,
        BAK = 42,
    }
}
/*
#define A_MOUSE_MOVE 1
#define A_MOUSE_WHEEL 2
#define A_MOUSE_LEFT_CLICK 10
#define A_MOUSE_LEFT_PRESS 11
#define A_MOUSE_LEFT_RELEASE 12
#define A_MOUSE_RIGHT_CLICK 20
#define A_MOUSE_RIGHT_PRESS 21
#define A_MOUSE_RIGHT_RELEASE 22
#define A_KEYBOARD_CLICK 50
#define A_KEYBOARD_PRESS 51
#define A_KEYBOARD_RELEASE 52
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.Views;

namespace shine.libs.konst
{
    public enum ZogKey
    {
        none = 0,
        ShowTree = 11,
        ShowTool = 12,
        ShowWarf = 13,
        ShowPhone = 14,
        ShowLRM = 21,
        ShowCSA = 22,
        ShowGSR = 23,
        ShowOKC = 24,
        OneTwoWarf = 31,
        NaviToggle = 33,
    }
    public enum MenuKey
    {
        none = 0,
        AppExit = 5,
        SaveMode = 32,
        DifferBox = 34,
        Cloaking = 51,
        CateRoot = 100,
        CateCurrent = 101,
        CatePhone = 110,
        CateBlenderObject = 112,
        CateBlenderEdit = 113,
        CateBlenderPose = 114,
        CateBlenderTexture = 115,
        CateBlenderPaint = 116,

        FavorArea00 = 250,
        FavorArea01 = 251,
        FavorArea02 = 252,
        FavorArea03 = 253,
        FavorArea04 = 254,
        FavorArea05 = 255,
        FavorArea06 = 256,
        FavorArea07 = 257,
        FavorArea08 = 258,
        FavorArea09 = 259,
        FavorCurr = 260,
        FavorSave = 270,

        SocketX192168 = 301,
        SocketMulgong = 302,
        xSocketX192168 = 311,
        xSocketMulgong = 312,
        SocketClose = 303,
    }
    public enum CamKey
    {
        none = -1,
        n0xCamera = 0,
        n1xFront = 1,
        n2xRollDown = 2,
        n3xRight = 3,
        n4xRollLeft = 4,
        n5xPerspect = 5,
        n6xRollRight = 6,
        n7xTop = 7,
        n8xRollUp = 8,
        n9xaaaa = 9,

        enter = 10,
        plusZoomOut = 11,
        minusZoomIn = 12,
        periodCenter = 20,

        Cloaking = 100,
    }
    public enum SenderKey
    {
        __ = -1,
        none = 1000,
        AppExit = 1200,

        A = Keycode.A,
        B = Keycode.B,
        C = Keycode.C,
        D = Keycode.D,
        E = Keycode.E,
        F = Keycode.F,
        G = Keycode.G,
        H = Keycode.H,
        I = Keycode.I,
        J = Keycode.J,
        K = Keycode.K,
        L = Keycode.L,
        M = Keycode.M,
        N = Keycode.N,
        O = Keycode.O,
        P = Keycode.P,
        Q = Keycode.Q,
        R = Keycode.R,
        S = Keycode.S,
        T = Keycode.T,
        U = Keycode.U,
        V = Keycode.V,
        W = Keycode.W,
        X = Keycode.X,
        Y = Keycode.Y,
        Z = Keycode.Z,

        Num0 = Keycode.Num0,
        Num1 = Keycode.Num1,
        Num2 = Keycode.Num2,
        Num3 = Keycode.Num3,
        Num4 = Keycode.Num4,
        Num5 = Keycode.Num5,
        Num6 = Keycode.Num6,
        Num7 = Keycode.Num7,
        Num8 = Keycode.Num8,
        Num9 = Keycode.Num9,

        Ctrl = Keycode.CtrlLeft,
        Shift = Keycode.ShiftLeft,
        Alt = Keycode.AltLeft,

        ConnectHome = 1301,
        ConnectOuter = 1302,
        xConnectHome = 1311,
        xConnectOuter = 1312,
        SocketClose = 1303,
        TogglePort = 1321,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.Views;

namespace shine.libs.konstw
{
    public enum BleCate
    {
        BlenderObject = 120112,
        BlenderEdit = 120113,
        BlenderPose = 120114,
        BlenderTexture = 120115,
        BlenderSkin = 120116,
        BlenderPaint = 120117,

        bled_Vertex = 120211,
        bled_Edge = 120212,
        bled_Face = 120213,
        bled_Select = 120220,

        blob_Camera = 120311,
    }
    public enum BleKeyval
    {
        blcam_Front = 2111,
        blcam_Top = 2112,
        blcam_Left = 2113,
        blcam_Right = 2114,
        blcam_Cam = 2115,
        blcam_Pers = 2121,
        blcam_Align = 2122,
        blcam_ZoomIn = 2123,
        blcam_ZoomOut = 2124,
        blcam_rollLeft = 2131,
        blcam_rollRight = 2132,
        blcam_rollUp = 2133,
        blcam_rollDown = 2134,
    }
    public enum WinCate
    {
        winSystem = 130000,
        WinTasks = 130100,
        WinVS = 130201,
        WinChrome = 130202,
    }
    public enum WinHotKey
    {
        rect_VS = 30101,
        rect_TC = 30102,
        rect_Chrome = 30103,
        rect_Ble = 30104,
        serv_Kbd = 30121,
        serv_Rdp = 30122,
        serv_Apk = 30123,
    }
}

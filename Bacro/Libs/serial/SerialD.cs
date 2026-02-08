using shine.libs.window;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Bacro
{
    public class SerialD
    {
        Makro macro;
        private IntPtr hwndLeoPad=IntPtr.Zero;

        public SerialD(Makro macro)
        {
            this.macro = macro;
        }

        internal void KeyAct(string action, string key)
        {
            SendMsgToLeoPad($"KeyAct {action} {key}");
        }

        internal void KeyClick(string mkeyx, string keyx)
        {
            if (mkeyx == null) { mkeyx = "null"; }
            SendMsgToLeoPad($"KeyClick {mkeyx} {keyx}");
        }

        internal void KeyClickX5(string keyx, int sleepIn, int count)
        {
            SendMsgToLeoPad($"KeyClickX5 {keyx} {sleepIn} {count}");
        }

        internal void KeyLongClick(string mkeyx, string keyx, int sleepPressed)
        {
            if (mkeyx == null) { mkeyx = "null"; }
            SendMsgToLeoPad($"KeyLongClick {mkeyx} {keyx} {sleepPressed}");
        }

        internal void KeysClick(string mkeyx, string keyx, int sleepIn)
        {
            if (mkeyx == null) { mkeyx = "null"; }
            SendMsgToLeoPad($"KeysClick {mkeyx} {keyx} {sleepIn}");
        }

        internal void MouseAct(string button, string action)
        {
            SendMsgToLeoPad($"MouseAct {button} {action}");
        }

        internal void MouseClick(string button, int sleepPressed)
        {
            SendMsgToLeoPad($"MouseClick {button} {sleepPressed}");
        }

        internal void MouseClickX(string button, string option)
        {
            if (option == null) { option = "null"; }
            SendMsgToLeoPad($"MouseClickX {button} {option}");
        }

        internal void MouseMove(int x, int y)
        {
            SendMsgToLeoPad($"MouseMove {x} {y}");
        }
        internal void MouseOffset(int dx, int dy)
        {
            SendMsgToLeoPad($"MouseOffset {dx} {dy}");
        }
        internal void SendMsgToLeoPad(string msg)
        {
            //Console.WriteLine(msg);

            if (!findWindowMemoMaster(ref hwndLeoPad))
                return;

            IntPtr pData = IntPtr.Zero;

            try
            {
                var textx = $"{msg}::_________________________________";
                DataForSend data = new DataForSend();
                data.dwData = new IntPtr(2);
                data.dataSize = msg.Length + 100;
                data.lpData = Marshal.StringToHGlobalAnsi(textx);

                pData = Marshal.AllocCoTaskMem(Marshal.SizeOf(data));
                Marshal.StructureToPtr(data, pData, false);

                wop.SendMessage(hwndLeoPad, wop.WM_COPYDATA, IntPtr.Zero, pData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            finally
            {
                if (pData != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pData);
            }
        }
        bool findWindowMemoMaster(ref IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero && wop.IsWindow(hwnd))
                return true;

            else
            {
                Process[] processRunning = Process.GetProcesses();

                foreach (Process p in processRunning)
                {
                    if (p.ProcessName == "LeoPad" && p.MainWindowTitle != "")
                    {
                        hwnd = p.MainWindowHandle;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
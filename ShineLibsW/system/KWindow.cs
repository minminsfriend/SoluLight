/*
 * Created by SharpDevelop.
 * User: shine
 * Date: 2016-05-16
 * Time: 오후 8:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using shine.libs.math;

namespace shine.libs.window
{
	public struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;
	}
    [StructLayout(LayoutKind.Sequential)]
    public struct DataForSend
    {
        /// <summary>
        /// The data to be passed to the receiving application. This member can be IntPtr.Zero.
        /// </summary>
        public IntPtr dwData;

        /// <summary>
        /// The size, in bytes, of the data pointed to by the lpData member.
        /// </summary>
        public int dataSize;

        /// <summary>
        /// The data to be passed to the receiving application. This member can be IntPtr.Zero.
        /// </summary>
        public IntPtr lpData;
    }

    public static class wop
	{
		public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_RESTORE = 9;

        public const int SWP_NOACTIVATE = 0x10;
        public const uint WM_COPYDATA = 0x004A;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetForegroundWindow();
		[DllImport("user32")]
		public static extern int GetWindowRect(int hwnd, ref RECT lpRect);
        [DllImport("user32.dll")]
        public  static extern int FindWindowEx(int  hWnd1,int  hWnd2,string  lpsz1,string  lpsz2);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32")]
		public static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int w, int h, int wFlags);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindow(IntPtr hWnd);        
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);
        //대상이되는 핸들값 //대상 핸들의자식

        //부모핸들..이런 식으로0~5까지 값으로 구별하여 접근할수가 있는것이다.
        //const int GW_HWNDFIRST = 0;
        //const int GW_HWNDLAST = 1;
        //const int GW_HWNDNEXT = 2;
        //const int GW_HWNDPREV = 3;
        //const int GW_OWNER = 4;
        //const int GW_CHILD = 5;
        [DllImport("user32.dll")]//핸들값을 넘겨주면 캡션값을 반환해줌
        public static extern int GetWindowText(int hWnd, StringBuilder title, int size);
        //1. 알아내고자하는 핸듶대상  2. 그 윈도우의 캡션값을 두번째 인자값으로 반환해줌 3.받을 텍스트값의 최대크기
        [DllImport("user32.dll")] //윈도우 창의상태를 변경 
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
       //1.변화시키고가자하는 핸들값 2.인자값을 어떻게 넘겨주느냐에 따라서 상태가 변하고
        //상태에 대한 값은 아래 값들을 참고하길..
        //private const int SW_HIDE = 0;
        //private const int SW_SHOWNORMAL = 1;
        //private const int SW_SHOWMINIMIZED = 2;
        //private const int SW_SHOWMAXIMIZED = 3;
        //private const int SW_SHOWNOACTIVATE = 4;
        //private const int SW_RESTORE = 9;
        //private const int SW_SHOWDEFAULT = 10;
		public const UInt32 WM_CLOSE = 0x0010;        
        public static void CloseWindow(IntPtr hwnd)
        {
	        SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
        public static void getWindowRect(IntPtr hwnd, ref krect rect)
        {
            RECT rec = new RECT();
            wop.GetWindowRect((int)hwnd, ref rec);

            rect.x = rec.left;
            rect.y = rec.top;
            rect.w = rec.right - rec.left;
            rect.h = rec.bottom - rec.top;
        }
        public static List<string> GetListOfWindowsTitle()
		{
			List<string> result = new List<string>();
			Process[] processRunning = Process.GetProcesses();
			
			foreach (Process pr in processRunning)
			{
				if (pr.MainWindowTitle != "")
				{
					result.Add(pr.MainWindowTitle);
				}
			}
			return result;
		}        
        public static IntPtr FindWindowByPatt(string pattern)
        {
			Process[] processRunning = Process.GetProcesses();
			
			foreach (Process pr in processRunning)
			{
				if (pr.MainWindowTitle != "")
				{
					if(Regex.IsMatch(pr.MainWindowTitle, pattern))
						return pr.MainWindowHandle;
				}
			}
        	
			return IntPtr.Zero;
        }
        public static IntPtr FindWindowByTitle(string title)
        {
            Process[] processRunning = Process.GetProcesses();

            foreach (Process pr in processRunning)
            {
                if (pr.MainWindowTitle == title)
                    return pr.MainWindowHandle;
            }

            return IntPtr.Zero;
        }
    }
}

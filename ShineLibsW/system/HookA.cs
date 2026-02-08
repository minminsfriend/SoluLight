using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace shine.libs.hook
{
	public class HookA
    {
		public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);

		public struct keyboardHookStruct
        {
			public int vkCode;
			public int scanCode;
			public int flags;
			public int time;
			public int dwExtraInfo;
		}

		const int WH_KEYBOARD_LL = 13;
		const int WM_KEYDOWN = 0x100;
		const int WM_KEYUP = 0x101;
		const int WM_SYSKEYDOWN = 0x104;
		const int WM_SYSKEYUP = 0x105;

        public List<Keys> HookedKeys = new List<Keys>();
		public IntPtr hookWin = IntPtr.Zero;

		public event KeyEventHandler KeyDown;
		public event KeyEventHandler KeyUp;


		public HookA()
        {
			//hook();
		}

		~HookA()
        {
			unhook();
		}

		public void hook()
        {
			IntPtr hInstance = LoadLibrary("User32");
            hookWin = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hInstance, 0);
		}
        public void SetHook(keyboardHookProc hookProc)
        {
            IntPtr hInstance = LoadLibrary("User32");
            hookWin = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hInstance, 0);
        }

        public void unhook()
        {
			UnhookWindowsHookEx(hookWin);
		}

		public int hookProc(int code, int wParam, ref keyboardHookStruct lParam)
        {
			if (code >= 0)
            {
				Keys key = (Keys)lParam.vkCode;

				if (HookedKeys.Contains(key))
                {
                    KeyEventArgs kea = new KeyEventArgs(key);

                    if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                    {
					    KeyDown(this, kea) ;
				    }
                    else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                    {
					    KeyUp(this, kea);
				    }

				    if (kea.Handled)
					    return 1;
				}

			}
			return CallNextHookEx(hookWin, code, wParam, ref lParam);
		}

		[DllImport("user32.dll")]
		static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

		[DllImport("user32.dll")]
		static extern bool UnhookWindowsHookEx(IntPtr hInstance);

		[DllImport("user32.dll")]
		public static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);

		[DllImport("kernel32.dll")]
		static extern IntPtr LoadLibrary(string lpFileName);

        public static bool IsKeyDown(int wParam)
        {
			return wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN;
        }
        public static bool IsKeyUp(int wParam)
        {
            return wParam == WM_KEYUP || wParam == WM_SYSKEYUP;
        }
    }
}

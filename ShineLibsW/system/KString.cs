/*
 * Created by SharpDevelop.
 * User: shine
 * Date: 2016-05-16
 * Time: 오후 4:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace shine.libs.window
{
	public static class kclip
	{
		[DllImport("User32.dll", SetLastError = true)]
		public static extern IntPtr GetClipboardData(uint uFormat);
		
		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool OpenClipboard(IntPtr hWndNewOwner);
		
		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseClipboard();
		
		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern IntPtr GlobalLock(IntPtr hMem);
		
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GlobalUnlock(IntPtr hMem);
		
		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern int GlobalSize(IntPtr hMem);
		
		public const uint CF_UNICODETEXT = 13U;
		
	  	public static string GetText()
	    {
	        //if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
	            //return null;
	
	        try
	        {
	            if (!OpenClipboard(IntPtr.Zero))
	                return null;
	
	            IntPtr handle = GetClipboardData(CF_UNICODETEXT);
	            if (handle == IntPtr.Zero)
	                return null;
	
	            IntPtr pointer = IntPtr.Zero;
	
	            try
	            {
	                pointer = GlobalLock(handle);
	                if (pointer == IntPtr.Zero)
	                    return null;
	
	                int size = GlobalSize(handle);
	                byte[] buff = new byte[size];
	
	                Marshal.Copy(pointer, buff, 0, size);
	
	                return Encoding.Unicode.GetString(buff).TrimEnd('\0');
	            }
	            finally
	            {
	                if (pointer != IntPtr.Zero)
	                    GlobalUnlock(handle);
	            }
	        }
	        finally
	        {
	            CloseClipboard();
	        }
	        
            //return null;
	    }
	}
	
	public static class kparse
	{
		public static int s2n(string str)
		{
			int n;
			
			if (Int32.TryParse(str, out n))
				return n;
				
			else 
				return -1000;
			
		}
	}
	public static class ktex
	{
		public static string getCurrTime()
		{
			DateTime t = DateTime.Now;
			
			string text=string.Format("{0:D2}{1:D2}-{2:D2}{3:D2}{4:D2}",
			                          t.Month, t.Day, t.Hour, t.Minute, t.Second);
			                          
			return text;
		}
        public static void removeLastSlash(ref string dir)
        {
            if (dir == null || dir.Length == 0)
                return;

            var Len = dir.Length;
            var La = dir.Substring(Len - 1);

            if (La == "\\")
                dir = dir.Substring(0, Len - 1);
        }
    }
}

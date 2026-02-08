/*
 * Created by SharpDevelop.
 * User: shine
 * Date: 2016-05-15
 * Time: 오후 4:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using shine.libs.math;

namespace shine.libs.simul
{
	public static class MouseSimul
	{
        [Flags]
	    public enum MouseEventFlags
	    {
	        LeftDown = 0x00000002,
	        LeftUp = 0x00000004,
	        MiddleDown = 0x00000020,
	        MiddleUp = 0x00000040,
	        Move = 0x00000001,
	        Absolute = 0x00008000,
	        RightDown = 0x00000008,
	        RightUp = 0x00000010,
            Wheel = 0x00000800
        }
        [StructLayout(LayoutKind.Sequential)]
	    public struct MousePoint
	    {
	        public int X;
	        public int Y;
	
	        public MousePoint(int x, int y)
	        {
	            X = x;
	            Y = y;
	        }
	    }
	    [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
	    [return: MarshalAs(UnmanagedType.Bool)]
	    public static extern bool SetCursorPos(int X, int Y);      
	    [DllImport("user32.dll")]
	    [return: MarshalAs(UnmanagedType.Bool)]
	    public static extern bool GetCursorPos(out MousePoint lpMousePoint);
	    [DllImport("user32.dll")]
	    public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
	    public static void SetCursorPosition(int X, int Y) 
	    {
	        SetCursorPos(X, Y);
	    }
	    public static void SetCursorPosition(MousePoint point)
	    {
	        SetCursorPos(point.X, point.Y);
	    }
	    public static MousePoint GetCursorPosition()
	    {
	        MousePoint currentMousePoint;
	        var gotPoint = GetCursorPos(out currentMousePoint);
	        if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
	        return currentMousePoint;
	    }
	    public static void MouseEvent(MouseEventFlags value)
	    {
	        MousePoint position = GetCursorPosition();
	        mouse_event((int)value, position.X, position.Y,  0,  0);
	    }
        public static void MouseEvent(MouseEventFlags value, kvec position)
        {
            mouse_event((int)value, position.X, position.Y, 0, 0);
        }
        public static void click(kvec pos, int nsleep)
		{
			int mDn=(int)MouseEventFlags.LeftDown;
			int mUp=(int)MouseEventFlags.LeftUp;
			
		    SetCursorPos(pos.X, pos.Y);
		    Thread.Sleep(5);
		    
			mouse_event(mDn, pos.X, pos.Y, 0, 0);
            mouse_event(mUp, pos.X, pos.Y, 0, 0);
            
            if(nsleep>0)
			    Thread.Sleep(nsleep);
		}
		public static void wheel(int val)
		{
			mouse_event((int)MouseEventFlags.Wheel, 0, 0, val, 0);
        }
    }
}

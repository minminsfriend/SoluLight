using System;
using shine.libs.math;

namespace Android.Views
{
    public enum MotionEventActions
    {
        Down = 0,
        Up = 1,
        Move = 2,
        Cancel = 3,
        Outside = 4,
        Pointer1Down = 5,
        PointerDown = 5,
        Pointer1Up = 6,
        PointerUp = 6,
        HoverMove = 7,
        PointerIdShift = 8,
        PointerIndexShift = 8,
        Scroll = 8,
        HoverEnter = 9,
        HoverExit = 10,
        ButtonPress = 11,
        ButtonRelease = 12,
        Mask = 255,
        Pointer2Down = 261,
        Pointer2Up = 262,
        Pointer3Down = 517,
        Pointer3Up = 518,
        PointerIdMask = 65280,
        PointerIndexMask = 65280
    }
    public class MotionEvent
    {
        MotionEventActions action;
        kvec pos;

        public MotionEvent(MotionEventActions action, float x, float y)
        {
            this.action = action;
            pos = new kvec(x, y);
        }
        public MotionEventActions Action
        {
            get
            {
                return action;
            }
        }

        public float GetX()
        {
            return pos.x;
        }

        public float GetY()
        {
            return pos.y;
        }
        public float GetX(int id)
        {
            return pos.x;
        }

        public float GetY(int id)
        {
            return pos.y;
        }

        public int PointerCount
        {
            get
            {
                return 0;
            }
        }
        public int GetPointerId(int index)
        {

            return 0;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.Content;
using Android.Graphics;

namespace Android.Views
{
    public static class MeasureSpec
    {
        public static int GetSize(int width)
        {
            return width;
        }
    }

    public abstract class SurfaceView
    {
        public bool FocusableInTouchMode = true;

        public SurfaceView(Context context)
        {

        }

        protected abstract void OnMeasure(int widthMeasureSpec, int heightMeasureSpec);

        protected void SetMeasuredDimension(int canvasWidth, int canvasHeight)
        {
            //fake nothing
        }

        public virtual bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            return true;
        }
        public virtual bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            return false;
        }

        public virtual bool OnGenericMotionEvent(MotionEvent e)
        {
            return false;
        }
        public virtual bool OnTouchEvent(MotionEvent e)
        {
            return false;
        }

        public abstract void WinPaint(System.Drawing.Graphics g);//추가 페이크
        public abstract void OnMeasureFake(int width, int height); // 추가 페이크
    }
}

using System;
using System.Drawing;

using Android.Views;
using Color = System.Drawing.Color;

namespace Android.Graphics
{
    public class PaintFlags
    {
        public const int AntiAlias = 0;
    }
    public class Paint
    {
        public enum Style
        {
            Stroke = 1,
            Fill = 2,
        }

        Color color = Color.Black;
        private object antiAlias;
        private float strokeWidth;

        public Style style;

        public Font font;
        public Pen pen;
        public SolidBrush brush;
        public bool AntiAlias;

        public Paint()
        {
            init();
        }
        public Paint(object antiAlias)
        {
            init();

            this.antiAlias = antiAlias;
        }
        void init()
        {
            pen = new Pen(color);
            brush = new SolidBrush(color);

            font = new Font(new FontFamily("맑은 고딕"), 20);
        }
        public Color Color
        {
            set
            {
                color = value;
                pen.Color = color;
                brush.Color = color;
            }
        }
        public float TextSize
        {
            set
            {
                float fontSize = 0.67f * value;
                font = new Font(new FontFamily("맑은 고딕"), fontSize);
            }
        }
        public float StrokeWidth
        {
            set
            {
                strokeWidth = value;
                pen.Width = strokeWidth;

            }
        }
        public void SetStyle(Style style)
        {
            this.style = style;
        }

        public void SetTypeface(Typeface fontNamsan)
        {
            //font = fontNamsan
        }

        public Style GetStyle()
        {
            return style;
        }
        public void GetTextBounds(string text, int start, int length, Rect rectText)
        {
            Paint paint = new Paint();
            paint.GetTextBounds(text, start, length, rectText);
        }
    }
}
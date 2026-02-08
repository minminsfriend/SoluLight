using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using shine.libs.math;

namespace shine.libs.pad
{
    public enum GBoxStyle
    {
        none = 0,
        text = 1,
        pilgy = 2,
        grim = 3,

        vcolor = 11,
        vint = 12,
        vfloat = 13,
    }

    public class GBox
    {
        public krect rect;
        public string title;
        public string memo;
        public string path, linked;
        public float val;
        public GBoxStyle style = GBoxStyle.text;

        public Color colorBack, colorText;
        public bool pressed, released;
        public bool hovered, moveG;
        public bool visible;

        public GBox()
        {
            colorBack = Color.LightGray;
            colorText = Color.Black;

            pressed = released = false;
            hovered = moveG = false;

            title = memo = path = linked = "";
            rect = new krect(100, 100);
            visible = true;
            val = 0;
        }
        public bool Contains(kvec pos)
        {
            if (rect.contains(pos))
                return true;

            else return false;
        }

        public GBox copy()
        {
            GBox gNew = new GBox();

            gNew.rect = rect.copy();
            gNew.colorBack = colorBack;
            gNew.colorText = colorText;

            return gNew;
        }
    }
    public class LBox
    {
        public krect rect;
        public string title;
        public string memo;
        public string file;

        public Color colorBack, colorText;
        public bool pressed, released;
        public bool hovered;
        public bool visible;

        public LBox()
        {
            colorBack = Color.LightGray;
            colorText = Color.Black;

            pressed = released = false;
            hovered = false;

            title = memo = file = "";
            rect = new krect(100, 100);
            visible = true;
        }
        public bool Contains(kvec pos)
        {
            if (rect.contains(pos))
                return true;

            else return false;
        }

        internal LBox copy()
        {
            LBox gNew = new LBox();

            gNew.rect = rect.copy();
            gNew.colorBack = colorBack;
            gNew.colorText = colorText;

            return gNew;
        }

        internal bool OnMouseDown(kvec pos)
        {
            return rect.contains(pos);
        }

        internal bool OnMouseUp(kvec pos)
        {
            return rect.contains(pos);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using shine.libs.math;

namespace shine.libs.pad
{
    public class BBox
    {
        public krect rect;
        public string title;
        public string memo;
        public string file;
        public int index;

        public Color colorBack, colorText;
        public bool pressed, released;
        public bool hovered;
        public bool visible;

        public BBox()
        {
            colorBack = Color.LightGray;
            colorText = Color.Black;

            pressed = released = false;
            hovered = false;

            title = memo = file = "";
            rect = new krect(100, 100);
            visible = true;
            index = -1;
        }
        public bool Contains(kvec pos)
        {
            if (rect.contains(pos))
                return true;

            else return false;
        }

        public BBox copy()
        {
            BBox boxNew = new BBox();

            boxNew.rect = rect.copy();
            boxNew.colorBack = colorBack;
            boxNew.colorText = colorText;

            return boxNew;
        }
    }

}

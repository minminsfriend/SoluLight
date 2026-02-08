using shine.libs.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shine.libs.pad
{
    internal class SamplePad : BasePad
    {
        public SamplePad(string name, int rows, int cols, float fontsize) : base(name, rows, cols, fontsize)
        {

        }

        protected override void createGBoxs()
        {
            base.createGBoxs();
        }
        protected override void setRects()
        {
            base.setRects();
        }

        public override void Paint(Graphics g)
        {
            base.Paint(g);
        }
        public override void SetRects(krect rectCanvas)
        {
            base.SetRects(rectCanvas);
        }
    }
}

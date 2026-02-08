using shine.libs.math;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bacro
{
    public partial class CapView : Form
    {
        Bitmap ImageNow;
        object lockImage = new object();

        List<krect> rectShips = new List<krect>();
        List<kvec> pGreens = new List<kvec>();
        string PaintMode = "Compass";
        Pen penLine;

        public CapView()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(10, 800);
            this.ShowInTaskbar = false;
            //this.TopMost = true;

            this.Paint += CapShow_Paint;
        }

        private void CapShow_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(Color.Green);

            if (ImageNow == null)
                return;

            kvec offset=new kvec(10,10);

            try
            {
                Monitor.Enter(lockImage);
                krect rectSrc = new krect(ImageNow.Width, ImageNow.Height);
                krect rectDst = rectSrc.copy();
                rectDst.offset(offset);

                g.DrawImage(ImageNow, rectDst.R, rectSrc.R, GraphicsUnit.Pixel);

                if (PaintMode == "compass")
                {

                }
                else if (PaintMode == "ships")
                {
                    foreach (var rect in rectShips)
                    {
                        krect rectx = rect.copy();
                        rectx.offset(offset);

                        g.DrawRectangle(Pens.Violet, rectx.R);
                    }

                    if (penLine == null)
                        penLine = new Pen(Color.Pink, 2f);

                    foreach (var px in pGreens)
                    {
                        kvec p = px.copy();
                        p.offset(offset);

                        var LT = p.copy(); LT.offset(-5, -5);
                        var RT = p.copy(); RT.offset(+5, -5);
                        var LB = p.copy(); LB.offset(-5, +5);
                        var RB = p.copy(); RB.offset(+5, +5);

                        g.DrawLine(penLine, LT.P, RB.P);
                        g.DrawLine(penLine, RT.P, LB.P);
                    }
                }
            }
            finally
            {
                Monitor.Exit(lockImage);
            }
        }
        void InvokePaint()
        {
            this.Invoke(new Action(() => {
                this.Activate();
                this.Invalidate();
            }));
        }
        public void PrintImage(Bitmap bmpCap)
        {
            PaintMode = "compass";
            try
            {
                Monitor.Enter(lockImage);

                //set image
                if (ImageNow != null)
                {
                    ImageNow.Dispose();
                    ImageNow = null;
                }

                ImageNow = new Bitmap(bmpCap);

                // set canvas size
                setCanvasSize();
            }
            finally
            {
                Monitor.Exit(lockImage);
            }

            InvokePaint();
        }
        internal void PrintShipBoxs(Bitmap bmpScan, List<krect> rectShips, List<kvec> pgreens)
        {
            PaintMode = "ships";

            if (!this.Visible)
            {
                this.Invoke(new Action(() =>
                {
                    this.Show();
                    this.Activate();
                }));
            }

            try
            {
                Monitor.Enter(lockImage);

                //set image
                if (ImageNow != null)
                {
                    ImageNow.Dispose();
                    ImageNow = null;
                }

                ImageNow = new Bitmap(bmpScan);

                // set rects
                this.rectShips = rectShips;
                this.pGreens = pgreens;

                // set canvas size
                setCanvasSize();
            }
            finally
            {
                Monitor.Exit(lockImage);
            }

            InvokePaint();
        }
        void setCanvasSize()
        {
            krect rectCanvas = new krect(ImageNow.Width, ImageNow.Height);
            rectCanvas.inflate(10, 10);

            this.Invoke(new Action(() => {
                this.ClientSize = new Size(rectCanvas.W, rectCanvas.H);
            }));
        }
    }
}

using shine.libs.math;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bacro
{
    public partial class CityMap : Form
    {
        string filemap, fileinput;
        Bitmap ImageBackMap;
        kvec posMove = new kvec();
        kvec posPivot = new kvec();
        bool mouseDowned = false;
        object lockImagePoints = new object();

        krect rectDraw;

        kvec tail = new kvec(0, 1);
        kvec head = new kvec();
        public List<kvec> tomovepoints = new List<kvec>();

        //bool GirlIsMoving = false;

        bool CTRL, SHIFT, ALT;
        bool onKeepPaintSleep = false;
        
        //Bacro main;
        int IndexSel = -1;

        public CityMap(string filemap, string fileinput)
        {
            //this.filemap = @"d:\Netmarble\_Images\dho cap\selImages\porutu citymap.png";
            //this.fileinput = main.fileinput;
            this.filemap = @"d:\Netmarble\_Images\dho cap\selImages\porutu citymap.png";
            this.filemap = filemap;
            this.fileinput = fileinput;

            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            this.ShowInTaskbar = false;

            this.Paint += Map_Paint;
            this.KeyDown += Map_KeyDown;
            this.KeyUp += Map_KeyUp;
            this.MouseDown += Map_MouseDown;
            this.MouseUp += Map_MouseUp;
            this.MouseMove += Map_MouseMove;
            this.MouseWheel += Map_MouseWheel;

            this.Load += Map_Load;
            this.Resize += Map_Resize;
            this.FormClosing += Map_Closing;

            CTRL = SHIFT = ALT = false;
        }
        void Map_Load(object sender, EventArgs e)
        {
            ImageBackMap = (Bitmap)Bitmap.FromFile(filemap);
            float aspect = 1f * ImageBackMap.Width / ImageBackMap.Height;

            Size size = this.ClientSize;

            rectDraw = new krect(size.Width, size.Height);
            rectDraw.inflate(-20, -20);
            rectDraw.h = rectDraw.w / aspect;
        }
        void Map_Closing(object sender, FormClosingEventArgs e)
        {
           
        }
        void Map_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }
        void Map_MouseWheel(object sender, MouseEventArgs e)
        {
            var wheel = e.Delta;
      
        }
        void Map_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDowned = true;

            posPivot = new kvec(e.X, e.Y);
        }
        void Map_MouseMove(object sender, MouseEventArgs e)
        {
            posMove = new kvec(e.X, e.Y);

            if (mouseDowned)
            {
                var dis = posMove - posPivot;

                if (!onKeepPaintSleep)
                    new Thread(PaintIntermittent).Start();
            }
            else
            {
        
            }
        }
        void Map_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDowned = false;
        }
        void Map_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.ControlKey:
                    CTRL = true;
                    return;
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.ShiftKey:
                    SHIFT = true;
                    return;
                case Keys.LMenu:
                case Keys.RMenu:
                case Keys.Menu:
                    ALT = true;
                    return;
            }
        }
        void Map_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.ControlKey:
                    CTRL = false;
                    return;
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.ShiftKey:
                    SHIFT = false;
                    return;
                case Keys.LMenu:
                case Keys.RMenu:
                case Keys.Menu:
                    ALT = false;
                    return;
            }

            if(CTRL)
            {
                switch (e.KeyCode)
                {
                    case Keys.P:
                        saveCoords();

                        return;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.X:
                        if (!mouseDowned)
                        {
                            AddPoint(posMove);
                            this.Invalidate();
                        }
                        return;
                    case Keys.C:
                        if (!mouseDowned)
                        {
                            tomovepoints.Clear();
                            this.Invalidate();
                        }
                        return;
                    case Keys.S:
                        if (!mouseDowned)
                        {
                            if(SelPoint(posMove))
                            {
                            }

                            this.Invalidate();
                        }
                        return;
                    case Keys.E:
                        if (!mouseDowned)
                        {
                            if (!InsertPoint(posMove))
                                IndexSel = -1;

                            this.Invalidate();
                        }
                        return;
                }
            }

            if (IndexSel > -1)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        if (CTRL)
                            MoveCoord(IndexSel, -10, 0);
                        else
                            MoveCoord(IndexSel, -1, 0);
                        break;
                    case Keys.Right:
                        if (CTRL)
                            MoveCoord(IndexSel, +10, 0);
                        else
                            MoveCoord(IndexSel, +1, 0);
                        break;
                    case Keys.Up:
                        if (CTRL)
                            MoveCoord(IndexSel, 0, -10);
                        else
                            MoveCoord(IndexSel, 0, -1);
                        break;
                    case Keys.Down:
                        if (CTRL)
                            MoveCoord(IndexSel, 0, +10);
                        else
                            MoveCoord(IndexSel, 0, +1);
                        break;
                }

                switch (e.KeyCode)
                {
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                        this.Invalidate();
                        return;
                }
            }
        }
        void MoveCoord(int indexSel, int dx, int dy)
        {
            //kvec coord = coords[indexSel];
            //coord.offset(dx, dy);

            //coord.x = Math.Max(0, coord.x);
            //coord.x = Math.Min(coord.x, ImageBackMap.Width);
            //coord.y = Math.Max(0, coord.y);
            //coord.y = Math.Min(coord.y, ImageBackMap.Height);

            //coords[indexSel] = coord.copy();
        }
        void Map_Paint(object sender, PaintEventArgs e)
        {
            if (rectDraw == null)
                return;

            try
            {
                Monitor.Enter(lockImagePoints);

                if (ImageBackMap != null)
                {
                    krect rectSrc = new krect(ImageBackMap.Width, ImageBackMap.Height);

                    Graphics g = e.Graphics;

                    g.DrawImage(ImageBackMap, rectDraw.R, rectSrc.R, GraphicsUnit.Pixel);

                    drawPoints(g);
                }
            }
            finally
            {
                Monitor.Exit(lockImagePoints);
            }
        }
        void PaintIntermittent()
        {
            onKeepPaintSleep = true;

            InvokePaint();

            Thread.Sleep(50);

            onKeepPaintSleep = false;
        }
        public void InvokePaint()
        {
            this.Invoke(new Action(() => this.Invalidate()));

        }
        void drawPoints(Graphics g)
        {
            if (tomovepoints.Count == 0)
                return;

            Font font = new Font(new FontFamily("맑은 고딕"), 12f);

            int index = -1;
            var M = 6;

            foreach (var coord in tomovepoints)
            {
                var pos = calcPointToDraw(coord);

                index++;
                var colorPen = index == IndexSel ? Color.Blue : Color.Pink;

                drawPointX(g, pos, colorPen, M);

                pos.offset(0, -30);
                g.DrawString($"{index}", font, Brushes.White, pos.P);
            }

            /*긴 화살표를 그려보자.*/

            kvec posHead = calcPointToDraw(head);
            kvec posTail = calcPointToDraw(tail);
            kvec vec = posTail - posHead;

            if (vec.length() > 5f)
            {
                vec.normalize();
                posTail = posHead + 50 * vec;
                g.DrawLine(Pens.Black, posHead.P, posTail.P);
            }
        }
        void drawPointX(Graphics g, kvec pos, Color color, int M)
        {
            kvec lt = pos.copy(); lt.offset(-M, -M);
            kvec rt = pos.copy(); rt.offset(+M, -M);
            kvec lb = pos.copy(); lb.offset(-M, +M);
            kvec rb = pos.copy(); rb.offset(+M, +M);

            Pen pen = new Pen(color);

            g.DrawLine(pen, lt.P, rb.P);
            g.DrawLine(pen, rt.P, lb.P);
        }
        void AddPoint(kvec pos)
        {
            kvec coord = calcPointToCoord(pos);

            int n = -1;
            int indexExists = -1;

            foreach (var p in tomovepoints)
            {
                n++;
                var dis = p - coord;

                if (dis.length() < 3f)
                {
                    indexExists = n;
                    break;
                }
            }

            if (indexExists > -1)
                tomovepoints.RemoveAt(indexExists);
            else
                tomovepoints.Add(coord.copy());
        }
        bool SelPoint(kvec pos)
        {
            kvec coord = calcPointToCoord(pos);

            int n = -1;
            int indexExists = -1;

            foreach (var p in tomovepoints)
            {
                n++;
                var dis = p - coord;

                if (dis.length() < 3f)
                {
                    indexExists = n;
                    break;
                }
            }

            if (indexExists > -1)
            {
                IndexSel = indexExists;
                return true;
            }
            else
            {
                IndexSel = -1;
                return false;
            }

            //return false;
        }
        bool InsertPoint(kvec pos)
        {
            if (IndexSel > -1 && IndexSel < tomovepoints.Count)
            { }
            else
                return false;

            kvec coord = calcPointToCoord(pos);

            int n = -1;
            int indexExists = -1;

            foreach (var p in tomovepoints)
            {
                n++;
                var dis = p - coord;

                if (dis.length() < 3f)
                {
                    indexExists = n;
                    break;
                }
            }

            if (indexExists > -1)
            {
                return false;
            }
            else//if (IndexSel < 0 || IndexSel > coords.Count - 1)
            {
                tomovepoints.Insert(IndexSel + 1, coord.copy());
                return true;
            }

            //return false;
        }
        kvec calcPointToCoord(kvec posmove)
        {
            float bae = ImageBackMap.Width / rectDraw.w;

            var pos = posmove - rectDraw.pos();
            pos.scale(bae);

            return pos;
        }
        kvec calcPointToDraw(kvec coord)
        {
            float bae = ImageBackMap.Width / rectDraw.w;

            var pos = coord.copy();

            pos.scale(1f / bae);
            pos.offset(rectDraw.pos());

            return pos;
        }
        void saveCoords()
        {
            
        }
        internal void FeedPoints(Dictionary<string, kvec> roadpoints)
        {
            tomovepoints = new List<kvec>();

            foreach(var r in roadpoints.Keys)
            {
                tomovepoints.Add(roadpoints[r].copy());
            }

            this.Invalidate();
        }
        internal void SetImage(Bitmap imageCap)
        {
            try
            {
                Monitor.Enter(lockImagePoints);
                ImageBackMap = imageCap;

                InvokePaint();
            }
            finally
            {
                Monitor.Exit(lockImagePoints);
            }
        }
        internal void setHeadAndTail(kvec head, kvec tail)
        {
            this.head = head.copy();
            this.tail = tail.copy();
        }
    }
}

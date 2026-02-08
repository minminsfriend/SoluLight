//using Android.Graphics;
using shine.libs.capture;
using shine.libs.graphics;
using shine.libs.math;
using shine.libs.serial;
using shine.libs.window;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static shine.libs.hangul.Han;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Bacro
{
    public partial class MunZZa : Form
    {
        Bitmap Image;
        Bitmap ImageSel, ImagePixels;
        CapDho capDho;

        List<krect> rectsShip = new List<krect>();
        List<kvec> pGreens = new List<kvec>();
        kvec pos = new kvec();
        List<kvec> pixels = new List<kvec>();

        krect rectSelTemp, rectSel;
        krect rectCapDest, rectDraw;
        int Zoom = 8;
        bool SHIFT, CTRL, ALT;
        bool mouseDowned = false;
        internal object ImageLock = new object();
        bool VisibleLettRects = true;
        string LocationPixelImage = "우측";

        string dirCap;
        bool VisibleHotKeysHelp = false;

        public Bacro main;
        PickPIcs pickPics;
        public MunZZa(Bacro main)
        {
            this.main= main;
            dirCap = $@"{main.dirImage}\dho cap";

            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            this.ShowInTaskbar = false;

            this.Paint += MunZZa_Paint;
            this.KeyDown += MunZZa_KeyDown;
            this.KeyUp += MunZZa_KeyUp;
            this.MouseDown += MunZZa_MouseDown;
            this.MouseUp += MunZZa_MouseUp;
            this.MouseMove += MunZZa_MouseMove;
            this.Load += MunZZa_Load;
            this.Resize += MunZZa_Resize;
            this.FormClosing += MunZZa_FormClosing;
            this.main = main;
        }
        void MunZZa_Resize(object sender, EventArgs e)
        {
            Size size = this.ClientSize;

            this.Invalidate();
        }
        void MunZZa_Load(object sender, EventArgs e)
        {
            SHIFT = CTRL = ALT = false;

            Size size = this.ClientSize;
            rectCapDest = new krect(20, 30, size.Width / 2, size.Height / 2);

            capDho = main.capDho;
            capDho.FocusWorkDho();

            pickPics = new PickPIcs(this);
            pickPics.PeedTems(this.main.pics);

            if (Image == null)
            {
                DirectoryInfo dinfo = new DirectoryInfo(dirCap);
                FileInfo[] finfos = dinfo.GetFiles().OrderBy(p => p.CreationTime).Reverse().ToArray();

                List<string> files = new List<string>();

                string filepng = null;

                foreach (FileInfo finfo in finfos)
                {
                    var file = finfo.FullName;
                    var ext = Path.GetExtension(file);

                    if (ext.ToLower() == ".png")
                    {
                        filepng = file;
                        break;
                    }
                }

                if (filepng != null && File.Exists(filepng))
                    Image = new Bitmap(filepng);
                else
                    return;
            }
        }
        void MunZZa_FormClosing(object sender, FormClosingEventArgs e)
        {
            messagesAdd("문짜 종료.");
            Thread.Sleep(1000);
        }
        void MunZZa_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (rectCapDest.contains(pos))
                {
                    rectSelTemp = new krect(0, 0);
                    rectSelTemp.setxy(e.X, e.Y);

                    //rectSel = null;
                    mouseDowned = true;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                //rectSel = null;
                this.Invalidate();
            }
        }
        void MunZZa_MouseMove(object sender, MouseEventArgs e)
        {
            pos = new kvec(e.X, e.Y);

            if (e.Button == MouseButtons.Left)
            {
                if (mouseDowned)
                {
                    if (rectCapDest.contains(pos))
                    {
                        kvec dis = pos - rectSelTemp.pos();

                        if (dis.x > 0 && dis.y > 0)
                        {
                            rectSelTemp.w = dis.x;
                            rectSelTemp.h = dis.y;
                        }
                    }

                    this.Invalidate();
                }
            }
            else if (e.Button == MouseButtons.None)
                this.Invalidate();

        }
        void MunZZa_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pos = new kvec(e.X, e.Y);

                if (rectCapDest == null || rectSelTemp == null)
                    return;

                if (rectCapDest.contains(pos))
                {
                    if (rectSelTemp.w > 10 && rectSelTemp.h > 10)
                    {
                        rectSel = rectSelTemp.copy();

                        ImageSel = buildSelImage();
                        ImagePixels = buildPixelsImage();
                    }

                    rectSelTemp = null;
                    mouseDowned = false;
                }
                //else if (rectDraw.contains(pos))
                //{


                //}

                this.Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {

            }
        }
        void MunZZa_KeyDown(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;

            switch (key)
            {
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    SHIFT = true;
                    return;
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    CTRL = true;
                    return;
                case Keys.LMenu:
                case Keys.RMenu:
                case Keys.Menu:
                    ALT = true;
                    return;
            }

            switch (key)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    return;
            }
        }
        void MunZZa_KeyUp(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;

            switch (key)
            {
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    SHIFT = false;
                    return;
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    CTRL = false;
                    return;
                case Keys.LMenu:
                case Keys.RMenu:
                case Keys.Menu:
                    ALT = false;
                    return;
            }

            /* left right up down rectsel 조작 */
            switch (key)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    MunZZa_KeyUp_rectMove(key);
                    return;
            }

            var CtrlKeys = new Keys[] { Keys.O, Keys.S, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0 };
            if (CTRL && -1 < Array.IndexOf(CtrlKeys, key))
            {
                switch (key)
                {
                    case Keys.S:
                        break;
                    case Keys.O:
                        loadImage($@"{dirCap}");
                        break;
                    case Keys.D4:
                        Console.WriteLine(" UpdatePicpics();");
                        UpdatePicpics();
                        break;
                    case Keys.D5:
                        showPicpics();
                        break;
                    case Keys.D6:
                        loadPic();
                        ImageSel = buildSelImage();
                        ImagePixels = buildPixelsImage();
                        break;
                    case Keys.D7:
                        saveSelImage($@"{dirCap}\selImages");
                        break;
                    case Keys.D8:
                        loadPixelImage($@"{dirCap}\selImages");
                        break;
                    case Keys.D9:
                        saveRectSel($@"{dirCap}\rects");
                        break;
                    case Keys.D0:
                        loadRectSel($@"{dirCap}\rects");
                        break;
                }

                if (key == Keys.O)
                    this.Text = Name;

                this.Invalidate();
                CTRL = false;
                return;
            }

            var CtrlKeys2 = new Keys[] { Keys.R, Keys.Q, Keys.C, Keys.T,Keys.G, Keys.X, Keys.P,Keys.L,
                Keys.OemMinus, Keys.OemSemicolon, Keys.OemCloseBrackets };

            if (CTRL && -1 < Array.IndexOf(CtrlKeys2, key))
            {
                string text, name;
                kvec pixel = getPixel(pos);

                switch (key)
                {
                    case Keys.R:
                        break;
                    case Keys.X:
                        LocationPixelImage = LocationPixelImage == "아래" ? "우측" : "아래";
                        this.Invalidate();
                        break;
                    case Keys.P:
                        using (var mInput = new MInput())
                        {
                            if (mInput.ShowDialog() == DialogResult.OK)
                            {
                                name = mInput.InputValue;
                                text = getPicData4File(null, name);
                                Clipboard.SetText(text);
                            }
                        }
                        break;
                    case Keys.L:
                        using (var mInput = new MInput())
                        {
                            if (mInput.ShowDialog() == DialogResult.OK)
                            {
                                name = mInput.InputValue;
                                text = getPicData4Code(name);
                                Clipboard.SetText(text);
                            }
                        }
                        break;
                    case Keys.OemSemicolon:
                        using (var mInput = new MInput())
                        {
                            if (mInput.ShowDialog() == DialogResult.OK)
                            {
                                name = mInput.InputValue;
                                text = getPicData4File("해적", name);
                                Clipboard.SetText(text);
                            }
                        }
                        break;
                    case Keys.Q:
                        text = getRectData();
                        if (text != null)
                            Clipboard.SetText(text);

                        //MessageBox.Show("pixels 데이타", "클립보드로 복사", MessageBoxButtons.OK);
                        break;

                    case Keys.C:
                        text = getOnePixel();
                        Clipboard.SetText(text);

                        MessageBox.Show(" pos : color \n\n" + text, "클립보드로 복사", MessageBoxButtons.OK);
                        break;
                    case Keys.T:
                        if (Image != null)
                        {
                            Bitmap bmpCut = buildCutImage(main.sail.RectScan);
                            bool enemyFound = false;
                            List<ShipNameRect> ships = main.sail.ScanningShipsOfFront(bmpCut, ref pGreens, ref enemyFound);

                            rectsShip.Clear();

                            foreach(var ship in ships)
                            {
                                rectsShip.Add(ship.rect.copy());

                                Console.WriteLine($"발견 해적 : {ship.name} <{ship.rect.toString()}>");
                            }
                        }
                        break;
                    case Keys.G:
                        rectsShip.Clear();
                        pGreens.Clear();

                        break;
                    case Keys.OemCloseBrackets:
                        //capDho.SearchDhoWindows();
                        //ShotDho();

                        break;
                    case Keys.OemMinus:

                        break;
                }

                CTRL = false;
                this.Invalidate();
                return;
            }

            // Not CTRL
            if (CTRL)
                return;

            switch (key)
            {
                case Keys.X:
                    addPixel(pos);
                    break;
                case Keys.C:
                    pixels.Clear();
                    break;
                case Keys.M:
                    break;
                case Keys.K:
                    break;
                case Keys.N:
                    break;
                case Keys.OemOpenBrackets:
                    break;
                case Keys.OemCloseBrackets:
                    //ShotDho();
                    break;
                case Keys.OemQuotes:

                    break;
                case Keys.Oem5:

                    break;
                case Keys.OemQuestion:
                    VisibleLettRects = !VisibleLettRects;
                    break;
                case Keys.F12:
                    VisibleHotKeysHelp = !VisibleHotKeysHelp;
                    break;
            }

            // Not CTRL
            this.Invalidate();
        }
        void showPicpics()
        {
            if (pickPics != null && pickPics.IsDisposed)
                pickPics = null;

            if(pickPics == null)
            {
                pickPics = new PickPIcs(this);
                pickPics.PeedTems(this.main.pics);
            }

            pickPics.ShowFront();
        }
        void UpdatePicpics()
        {
            if (pickPics != null && pickPics.IsDisposed)
                pickPics = null;

            if (pickPics == null)
                pickPics = new PickPIcs(this);
            
            main.LoadPics();
            pickPics.PeedTems(main.pics);//invoke 내장
            pickPics.ShowFront();
        }
        void MunZZa_KeyUp_rectMove(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Left:
                    if (CTRL && SHIFT)
                        SelRectMove(-10, 0);
                    else if (CTRL)
                        SelRectMove(-1, 0);
                    else if (SHIFT && ALT)
                        SelRectSize(-10, 0);
                    else if (SHIFT)
                        SelRectSize(-1, 0);
                    else
                    { }
                    break;
                case Keys.Right:
                    if (CTRL && SHIFT)
                        SelRectMove(+10, 0);
                    else if (CTRL)
                        SelRectMove(+1, 0);
                    else if (SHIFT && ALT)
                        SelRectSize(+10, 0);
                    else if (SHIFT)
                        SelRectSize(+1, 0);
                    else
                    { }
                    break;
                case Keys.Up:
                    if (CTRL && SHIFT)
                        SelRectMove(0, -10);
                    else if (CTRL)
                        SelRectMove(0, -1);
                    else if (SHIFT)
                        SelRectSize(0, -1);
                    else
                    { }
                    break;
                case Keys.Down:
                    if (CTRL && SHIFT)
                        SelRectMove(0, +10);
                    else if (CTRL)
                        SelRectMove(0, +1);
                    else if (SHIFT)
                        SelRectSize(0, +1);
                    else
                    { }
                    break;
            }

            this.Invalidate();
        }
        void MunZZa_Paint(object sender, PaintEventArgs e)
        {
            if (Image == null)
                return;

            Graphics g = e.Graphics;

            krect rectBmp = new krect(Image.Width, Image.Height);
            rectCapDest = rectBmp.copy();
            rectCapDest.offset(20, 20);

            g.Clear(Color.LightGray);
            g.DrawImage(Image, rectCapDest.R, rectBmp.R, GraphicsUnit.Pixel);

            //
            //drawPixelsImage(g);

            if (mouseDowned && rectSelTemp != null)
                g.DrawRectangle(Pens.Red, rectSelTemp.R);

            else if (!mouseDowned && rectSel != null)
                g.DrawRectangle(Pens.Coral, rectSel.R);

            if (ImagePixels != null)
            {
                drawImagePixels(g);

                if (rectDraw.contains(pos))
                {
                    drawPixelData(g);
                }
            }

            if (VisibleHotKeysHelp)
            {
                drawHotKeysHelp(g);
            }

            if (rectsShip.Count > 0 || pGreens.Count>0)
            {
                drawShipRects(g);
            }
        }
        void drawShipRects(Graphics g)
        {
            krect rectScan = main.sail.RectScan;

            var offsetImage = rectCapDest.pos();
            var offsetScan = rectScan.pos();

            foreach (var rectx in rectsShip)
            {
                var rect = rectx.copy();

                rect.offset(offsetImage);
                rect.offset(offsetScan);

                g.DrawRectangle(Pens.Red, rect.R);
            }

            //Console.WriteLine($"pGreens.Count {pGreens.Count}");

            Pen penLine = new Pen(Color.Pink, 2f);

            foreach (var px in pGreens)
            {
                var p = px.copy();
                //Console.WriteLine($"pGreen == {p.X} {p.Y}");

                p.offset(offsetImage);
                p.offset(offsetScan);

                var LT = p.copy(); LT.offset(-5, -5);
                var RT = p.copy(); RT.offset(+5, -5);
                var LB = p.copy(); LB.offset(-5, +5);
                var RB = p.copy(); RB.offset(+5, +5);

                g.DrawLine(penLine, LT.P, RB.P);
                g.DrawLine(penLine, RT.P, LB.P);
            }
        }
        void drawHotKeysHelp(Graphics g)
        {
            Size size = this.ClientSize;

            krect rectHelp = new krect();
            rectHelp.x = rectCapDest.Right + 20;
            rectHelp.y = rectCapDest.y;

            rectHelp.w = rectCapDest.w / 2;
            rectHelp.h = rectCapDest.h;

            g.FillRectangle(Brushes.LightGray, rectHelp.R);
            g.DrawRectangle(Pens.Black, rectHelp.R);

            Font font = new Font(new FontFamily("맑은 고딕"), 12f);

            List<string> helptexs = new List<string>();
            helptexs.Add("F12 : 단축키 도움말 토글");
            helptexs.Add("ctrl + O : dho cap 열기");
            helptexs.Add("ctrl + N8 : 픽셀8 이미지 열기");
            helptexs.Add("ctrl + N9 : RectSel 가져오기");
            helptexs.Add("ctrl + N0 : ImageSel 저장하기");
            helptexs.Add("ctrl + Q : 클립보드에 RectSel 복사");
            helptexs.Add("ctrl + P : 클립보드에 픽셀 좌표 복사");
            helptexs.Add("ctrl + C : 클립보드에 픽셀 칼라 복사");
            helptexs.Add("ctrl + [:] : 클립보드에 해적 픽셀 복사");
            helptexs.Add("ctrl + N0 : 숫자 인식");
            helptexs.Add("ctrl + X : 픽셀이미지 위치이동");

            var H = 12f * 3;

            var px = rectHelp.x + 10;
            var py = rectHelp.y + 10;

            foreach (var tex in helptexs)
            {
                g.DrawString(tex, font, Brushes.Black, px, py);

                py += H;
            }
        }
        void drawImagePixels(Graphics g)
        {
            krect rectImage = new krect(ImagePixels.Width, ImagePixels.Height);
            rectDraw = rectImage.copy();

            if (LocationPixelImage == "우측")
            {
                rectDraw.x = rectCapDest.Right + 20;
                rectDraw.y = rectCapDest.y;
            }
            else
            {
                rectDraw.x = rectCapDest.x;
                rectDraw.y = rectCapDest.Bottom + 10;
            }

            g.DrawImage(ImagePixels, rectDraw.R, rectImage.R, GraphicsUnit.Pixel);
        }
        void drawPixelData(Graphics g)
        {
            kvec pixel = getPixel(pos);

            krect rectText = rectDraw.copy();
            rectText.offset(70, rectDraw.h + 20);

            Font font = new Font(new FontFamily("맑은 고딕"), 12f);
            string text;

            Color colorPixel = getPixelColor(pixel);

            // draw 픽셀 data

            kvec posText = new kvec();
            posText.x = rectDraw.x + 80;
            posText.y = rectDraw.y + rectDraw.h + 20;

            text = $"X Y == {pixel.toString2()}";
            g.DrawString(text, font, Brushes.Black, posText.x, posText.y);

            posText.offset(0, 25);
            text = $"R G B == {colorPixel.R} {colorPixel.G} {colorPixel.B}";
            g.DrawString(text, font, Brushes.Black, posText.x, posText.y);

            if (rectSel != null)
            {
                posText.offset(0, 25);

                krect rectSrc = rectSel.copy();
                rectSrc.offset(-1f * rectCapDest.pos());

                text = $"[{rectSrc.toString()}]";
                g.DrawString(text, font, Brushes.Black, posText.x, posText.y);
            }

            // draw pixel color
            krect rectPxColor = new krect(50, 50);
            rectPxColor.x = rectDraw.x + 5;
            rectPxColor.y = rectDraw.y + rectDraw.h + 20;

            g.FillRectangle(new SolidBrush(colorPixel), rectPxColor.R);

            //
            drawPickedPixels(g);

            //
            krect recPixel = getPixelRect(pixel);
            g.DrawRectangle(Pens.Coral, recPixel.R);
        }
        void drawPickedPixels(Graphics g)
        {
            if (pixels.Count == 0)
                return;

            Pen pen = new Pen(Color.Violet);
            pen.Width = 2f;

            foreach (var p_ in pixels)
            {
                kvec p = rectDraw.pos() + Zoom * p_;

                kvec posL = p.copy();
                kvec posR = p.copy();
                posR.offset(Zoom, Zoom);

                kvec posT = p.copy();
                posT.offset(Zoom, 0);
                kvec posB = p.copy();
                posB.offset(0, Zoom);

                g.DrawLine(pen, posL.P, posR.P);
                g.DrawLine(pen, posT.P, posB.P);
            }
        }
        void SelRectSize(int dx, int dy)
        {
            if (rectSel != null)
            {
                if (rectSel.W + dx > 4)
                    rectSel.w += dx;
                if (rectSel.H + dy > 4)
                    rectSel.h += dy;

                ImageSel = buildSelImage();
                ImagePixels = buildPixelsImage();
            }
        }
        void SelRectMove(int dx, int dy)
        {
            if (rectSel != null)
            {
                if (rectSel.x + dx > rectCapDest.x)
                    rectSel.x += dx;
                if (rectSel.y + dy > rectCapDest.y)
                    rectSel.y += dy;

                ImageSel = buildSelImage();
                ImagePixels = buildPixelsImage();
            }
        }
        Color getPixelColor(kvec pixel)
        {
            krect rectPixel = new krect(Zoom, Zoom);
            rectPixel.offset(Zoom * pixel);

            int cx = rectPixel.X + Zoom / 2;
            int cy = rectPixel.Y + Zoom / 2;

            Color color;
            if (gx.GetPixel(ImagePixels, new kvec(cx, cy), out color))
                return color;
            else
                return Color.White;
        }
        krect getPixelRect(kvec pixel)
        {
            kvec pPixel = rectDraw.pos() + Zoom * pixel;
            return new krect(pPixel.x, pPixel.y, Zoom, Zoom);
        }
        kvec getPixel(kvec pos)
        {
            if (pos == null)
                return new kvec(0, 0);
            if (rectDraw == null)
                return new kvec(0, 0);

            kvec p = pos.copy();
            p.offset(-10, -10);
            p.offset(-1f * rectDraw.pos());

            int c = p.X / Zoom;
            int r = p.Y / Zoom;
            if (p.X % Zoom > 0)
                c += 1;
            if (p.Y % Zoom > 0)
                r += 1;

            return new kvec(c, r);
        }
        string getOnePixel()
        {
            kvec pix = getPixel(pos);

            Color color = getPixelColor(pix);

            return $"{pix.toString2()} : {gx.ColorToString(color)}";
        }
        string getPicData4File(string cate, string name)
        {
            List<string> tposs = new List<string>();
            List<string> tcolors = new List<string>();

            if (cate == null)
            {
                foreach (var p in pixels)
                {
                    Color c = getPixelColor(p);

                    tposs.Add(p.toString2());
                    tcolors.Add($"{c.R} {c.G} {c.B}");
                }

                var texrect = "_ _ _ _";

                if (rectSel != null)
                {
                    krect rectSrc = rectSel.copy();
                    rectSrc.offset(-1f * rectCapDest.pos());
                    texrect = rectSrc.toString();
                }

                var texposs = string.Join(",", tposs);
                var texcolors = string.Join(",", tcolors);

                return $"{name} /==/ {texrect} / {texposs} / {texcolors}";
            }
            else if (cate == "해적")
            {
                int n = -1;
                kvec offset = new kvec();

                foreach (var p in pixels)
                {
                    Color c = getPixelColor(p);
                    n++;

                    if (n == 0)
                    {
                        offset.x = -p.x;
                        offset.y = -p.y;
                    }
                    else
                    {
                        tposs.Add(p.toString2());
                        tcolors.Add($"{c.R} {c.G} {c.B}");
                    }
                }

                var texsize = "_ _ _ _";

                if (rectSel != null)
                {
                    krect rectSrc = rectSel.copy();
                    rectSrc.offset(-1f * rectCapDest.pos());
                    texsize = $"{rectSrc.W} {rectSrc.H}";
                }
                var offsetx = $"{offset.X} {offset.Y}";
                var texposs = string.Join(",", tposs);
                var texcolors = string.Join(",", tcolors);

                return $"{name} /==/ {offsetx} / {texsize} / {texposs} / {texcolors}";
            }
            else
            {
                return "";
            }
        }
        string getPicData4Code(string name)
        {
            List<string> tposs = new List<string>();
            List<string> tcolors = new List<string>();

            foreach (var p in pixels)
            {
                Color c = getPixelColor(p);

                tposs.Add(p.toString2());
                tcolors.Add($"{c.R} {c.G} {c.B}");
            }

            var texrect = "_ _ _ _";

            if (rectSel != null)
            {
                krect rectSrc = rectSel.copy();
                rectSrc.offset(-1f * rectCapDest.pos());
                texrect = rectSrc.toString();
            }

            var texposs = string.Join(",", tposs);
            var texcolors = string.Join(",", tcolors);
            var data = $"{texrect}::{texposs}::{texcolors}";

            return $"pics[\"{name}\"] = \"{data}\";";
        }
        string getRectData()
        {
            if (rectSel != null)
            {
                krect rectSrc = rectSel.copy();
                rectSrc.offset(-1f * rectCapDest.pos());
                return rectSrc.toString();
            }
            else
                return null;
        }
        string getNumPointsPattern()
        {
            List<string> tposs = new List<string>();

            kvec p0 = pixels[0];

            foreach (var p in pixels)
            {
                kvec offx = p - p0;
                tposs.Add(offx.toString2());
            }

            var texposs = string.Join(",", tposs);

            return $"원점과 거리 : \n숫자 x :: {texposs}";
        }
        bool loadImage(string dirInit)
        {
            var fileimage = showDialogImgFileOpen(dirInit);
            if (fileimage == null)
                return false;

            Name = Path.GetFileNameWithoutExtension(fileimage);

            try
            {
                Monitor.Enter(ImageLock);

                if (Image != null)
                {
                    Image.Dispose();
                    Image = null;
                }

                Image = (Bitmap)Bitmap.FromFile(fileimage);
            }
            finally
            {
                Monitor.Exit(ImageLock);
            }

            return true;
        }
        bool loadPixelImage(string dirInit)
        {
            var fileimage = showDialogImgFileOpen(dirInit);
            if (fileimage == null)
                return false;

            //
            var name = Path.GetFileNameWithoutExtension(fileimage);
            var nam = name.Substring(2);

            var dirMaps = @"d:\Netmarble\_Images\dho cap\maps";
            var filemap = $"{dirMaps}\\{nam}.png";

            if (File.Exists(filemap))
            {
                ImageSel = (Bitmap)Bitmap.FromFile(filemap);
                //MessageBox.Show(filemap, "ImageSel 자동 등록", MessageBoxButtons.OK);

                if (ImagePixels != null)
                {
                    ImagePixels.Dispose();
                    ImagePixels = null;
                }
                ImagePixels = (Bitmap)Bitmap.FromFile(fileimage);

                return true;
            }

            return false;
        }
        bool saveRectSel(string dirInit)
        {
            var filerect = showDialogTxtFileSave(dirInit);
            if (filerect == null)
                return false;

            FileStream fw = new FileStream(filerect, FileMode.Create, FileAccess.Write);

            byte[] data = Encoding.UTF8.GetBytes("empty");
            fw.Write(data, 0, data.Length);
            fw.Close();

            return true;
        }
        bool loadRectSel(string dirInit)
        {
            var filerect = showDialogTxtFileOpen(dirInit);
            if (filerect == null)
                return false;

            var name = Path.GetFileNameWithoutExtension(filerect);
            var ss = Regex.Split(name, "-");

            if (ss.Length == 2)
                rectSel = krect.Parse(ss[1].Trim());
            else
                rectSel = krect.Parse(name.Trim());

            rectSel.offset(rectCapDest.pos());

            ImageSel = buildSelImage();
            ImagePixels = buildPixelsImage();

            return true;
        }
        string showDialogImgFileOpen(string dirInit)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "이미지 열기";
            ofd.FileName = "*.png";
            ofd.InitialDirectory = dirInit;
            ofd.Filter = "그림 파일 (*.png,*.bmp,*.jpg) | *.png; *.bmp; *.jpg; | 모든 파일 (*.*) | *.*";

            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
                return ofd.FileName;
            else
                return null;
        }
        string showDialogImgSelFileSave(string dirInit)
        {
            SaveFileDialog ofd = new SaveFileDialog();

            ofd.Title = "이미지 저장";
            ofd.FileName = "*.png";
            ofd.InitialDirectory = dirInit;
            ofd.Filter = "그림 파일 (*.png) | *.png | 모든 파일 (*.*) | *.*";

            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
                return ofd.FileName;
            else
                return null;
        }
        string showDialogTxtFileOpen(string dirInit)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "텍스트화일 열기";
            ofd.FileName = "*.txt";
            ofd.InitialDirectory = dirInit;
            ofd.Filter = "문서 파일 (*.txt) | *.txt | 모든 파일 (*.*) | *.*";

            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
                return ofd.FileName;
            else
                return null;
        }
        string showDialogTxtFileSave(string dirInit)
        {
            SaveFileDialog ofd = new SaveFileDialog();

            ofd.Title = "텍스트화일 저장";
            ofd.FileName = "*.txt";
            ofd.InitialDirectory = dirInit;
            ofd.Filter = "문서 파일 (*.txt) | *.txt | 모든 파일 (*.*) | *.*";

            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
                return ofd.FileName;
            else
                return null;
        }
        void addPixel(kvec pos)
        {
            kvec pixel = getPixel(pos);

            int index = -1;
            int indexExists = -1;

            foreach (var p in pixels)
            {
                index++;

                if (p.X == pixel.X && p.Y == pixel.Y)
                {
                    indexExists = index;
                    break;
                }
            }

            if (indexExists > -1)
                pixels.RemoveAt(indexExists);
            else
                pixels.Add(pixel.copy());
        }
        Bitmap buildSelImage()
        {
            Bitmap bmpSel = new Bitmap(rectSel.W, rectSel.H, Image.PixelFormat);
            Graphics g = Graphics.FromImage(bmpSel);

            krect rectSrc = rectSel.copy();
            rectSrc.offset(-1f * rectCapDest.pos());

            krect rectSelImage = new krect(bmpSel.Width, bmpSel.Height);

            g.DrawImage(Image, rectSelImage.R, rectSrc.R, GraphicsUnit.Pixel);

            return bmpSel;
        }
        Bitmap buildCutImage(krect rectCut)
        {
            Bitmap bmpCut = new Bitmap(rectCut.W, rectCut.H, Image.PixelFormat);
            Graphics g = Graphics.FromImage(bmpCut);

            krect rectDest = new krect(bmpCut.Width, bmpCut.Height);

            g.DrawImage(Image, rectDest.R, rectCut.R, GraphicsUnit.Pixel);

            return bmpCut;
        }
        Bitmap buildPixelsImage()
        {
            if (ImageSel == null)
                return null;

            int width = ImageSel.Width * Zoom;
            int height = ImageSel.Height * Zoom;

            Bitmap bmpZ = new Bitmap(width, height, ImageSel.PixelFormat);
            Graphics g = Graphics.FromImage(bmpZ);

            krect rec0 = new krect(Zoom, Zoom);
            SolidBrush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(Color.White);
            pen.Color = Color.FromArgb(160, Color.White);
            pen.Width = 0.5f;

            for (int y = 0; y < ImageSel.Height; y++)
            {
                for (int x = 0; x < ImageSel.Width; x++)
                {
                    Color color = ImageSel.GetPixel(x, y);

                    krect rec = rec0.copy();
                    rec.offset(Zoom * x, Zoom * y);

                    brush.Color = color;

                    g.FillRectangle(brush, rec.R);
                    g.DrawRectangle(pen, rec.R);

                }
            }

            return bmpZ;
        }
        void saveCapImage()
        {
            //var textime = DateTime.Now.ToString("yyyyMMddHHmmss"); // 20180403160147

            var textime = DateTime.Now.ToString("ddHHmmss"); // 03160147
            var filecap = $@"{dirCap}\dho {textime}.png";

            Image.Save(filecap, ImageFormat.Png);

            messagesAdd($"cap 저장 : {filecap}");

            //오래된 저장화일 삭제 
            removeOldCapFiles(dirCap);
        }
        void removeOldCapFiles(string dircap)
        {
            DirectoryInfo dinfo = new DirectoryInfo(dircap);
            FileInfo[] finfos = dinfo.GetFiles().OrderBy(p => p.CreationTime).ToArray();

            List<string> files = new List<string>();

            foreach (FileInfo finfo in finfos)
            {
                //Console.WriteLine(file.FullName);
                files.Add(finfo.FullName);
            }

            while (files.Count > 100)
            {
                var file0 = files[0];

                File.Delete(file0);
                files.RemoveAt(0);

                //Console.WriteLine(file0);
            }
        }
        void messagesAdd(string msg)
        {

            Console.WriteLine(msg);

        }
        public void PrintImageDhoCap(Bitmap image)
        {
            Image = image;

            if (Image != null)
            {
                if (rectSel != null)
                {
                    ImageSel = buildSelImage();
                    ImagePixels = buildPixelsImage();
                }

                saveCapImage();
            }

            this.invokePaint();
        }
        internal Bitmap requestImage(krect rectDhoSel, string name)
        {
            krect rectCap = rectDhoSel.copy();
            rectCap.offset(capDho.RectDho.pos());

            Bitmap bmp = ScreenCopy.GetImage(rectCap);

            //Console.WriteLine($"{bmp.Width} {bmp.Height}");

            return bmp;
        }
        void temp()
        {
            if (ImageSel == null)
                return;

            var dirMaps = $@"{dirCap}\maps";

            krect rectMap = krect.Parse("690 496 110 110");

            List<string> tposs = new List<string>();

            foreach (var p in pixels)
            {
                var pos = p.copy();
                pos.offset(-1f * rectMap.pos());
                pos.offset(-1f * rectCapDest.pos());
                tposs.Add(pos.toString2());

            }

            var texposs = string.Join(",", tposs);

            Console.WriteLine(texposs);

        }
        void loadPic()
        {
            using (var mInput = new MInput())
            {
                if (mInput.ShowDialog() == DialogResult.OK)
                {
                    var name = mInput.InputValue;
                    var picx = main.pics[name];

                    Console.WriteLine($"name = {name}");
                    Console.WriteLine($"pic = {picx}");

                    pixels.Clear();

                    var ss = Regex.Split(picx, "::");
                    var nn = Regex.Split(ss[1], ",");

                    foreach (var px in nn)
                    {
                        pixels.Add(kvec.Parse2d(px));
                    }

                    var rectPic = krect.Parse(ss[0]);
                    rectSel = rectPic.copy();
                    rectSel.offset(rectCapDest.pos());
                }
            }
        }
        void savePixelsImage()
        {
            krect rectMap = krect.Parse("690 496 110 110");

            DirectoryInfo dinfo = new DirectoryInfo(dirCap);
            FileInfo[] finfos = dinfo.GetFiles().OrderBy(p => p.CreationTime).Reverse().ToArray();

            List<string> files = new List<string>();

            int count = 0;

            Bitmap bmpMap0 = new Bitmap(rectMap.W, rectMap.H, PixelFormat.Format32bppArgb);
            krect rectBmpOut = new krect(rectMap.W, rectMap.H);

            var dirOut = $@"{dirCap}\maps";

            foreach (FileInfo finfo in finfos)
            {
                if (++count > 10)
                    break;

                var file = finfo.FullName;

                var ext = Path.GetExtension(file).ToLower();

                if (ext == ".png")
                {
                    using (Bitmap bmp = (Bitmap)Bitmap.FromFile(file))
                    using (Bitmap bmpMap = new Bitmap(bmpMap0))
                    {
                        Graphics g = Graphics.FromImage(bmpMap);
                        g.DrawImage(bmp, rectBmpOut.R, rectMap.R, GraphicsUnit.Pixel);

                        using (Bitmap bmpPx8 = PixelsImage(bmpMap))
                        {
                            var fileout = $"{dirOut}\\px{count:d02}.png";
                            bmpPx8.Save(fileout, ImageFormat.Png);
                        }
                    }
                }
            }
        }
        bool saveSelImage(string dirout)
        {
            var fileout = showDialogImgSelFileSave(dirout);
            if (fileout == null)
                return false;

            if (ImageSel == null)
                return false;

            ImageSel.Save(fileout, ImageFormat.Png);

            return true;
        }
        Bitmap PixelsImage(Bitmap bmp)
        {
            int width = bmp.Width * Zoom;
            int height = bmp.Height * Zoom;

            Bitmap bmpZ = new Bitmap(width, height, bmp.PixelFormat);
            Graphics g = Graphics.FromImage(bmpZ);

            krect rec0 = new krect(Zoom, Zoom);
            SolidBrush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(Color.White);
            pen.Color = Color.FromArgb(160, Color.White);
            pen.Width = 0.5f;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color color = bmp.GetPixel(x, y);

                    krect rec = rec0.copy();
                    rec.offset(Zoom * x, Zoom * y);

                    brush.Color = color;

                    g.FillRectangle(brush, rec.R);
                    g.DrawRectangle(pen, rec.R);

                }
            }

            return bmpZ;
        }
        void invokePaint()
        {
            this.Invoke(new Action(() =>
            {
                this.Invalidate();
            }));
        }
        internal void PrintPic(krect rect)
        {
            if (!this.Visible)
            {
                this.Invoke(new Action(() =>
                {
                    this.Show();
                    this.Activate();
                }));
            }

            var offx = rectCapDest.pos();
            rectSel = rect.copy();
            rectSel.offset(offx);

            ImageSel = buildSelImage();
            ImagePixels = buildPixelsImage();

            //rectMacro = rect.copy();
            //rectMacro.offset(offx);

            invokePaint();
        }
        internal void picpic_LoadPic(string name)
        {
            var picx = main.pics[name];

            Console.WriteLine($"name = {name}");
            Console.WriteLine($"pic = {picx}");

            pixels.Clear();

            var ss = Regex.Split(picx, "::");
            var nn = Regex.Split(ss[1], ",");

            foreach (var px in nn)
            {
                pixels.Add(kvec.Parse2d(px));
            }

            var rectPic = krect.Parse(ss[0]);

            /* 폼에 그린다 */
            rectSel = rectPic.copy();
            rectSel.offset(rectCapDest.pos());

            ImageSel = buildSelImage();
            ImagePixels = buildPixelsImage();

            invokePaint();
        }
    }
}

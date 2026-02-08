using shine.libs.drawing;
using shine.libs.graphics;
using shine.libs.math;
using shine.libs.pad;
using shine.libs.window;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using dPath = System.IO.Path;
using gPath = System.Drawing.Drawing2D.GraphicsPath;

namespace Geminapi
{
    public partial class ImgViewer : Form
    {
        bool CTRL = false;
        bool SHIFT = false;
        string fileCurr;
        Bitmap imageBase;
        public Bitmap imageBlack;
        krect rectSel, rectDst;
        kvec posDown = kvec.Zero;
        kvec posMove = kvec.Zero;
        bool MouseDowned= false;

        string DrawMode = "NORMAL";
        Bitmap imageCurr
        {
            get
            {
                if(DrawMode == "BLACK")
                    return imageBlack;

                return imageBase;
            }
        }

        Geminapi main;
        private string fileBwRec;
        private string fileBmpRedRec;
        private bool visibleRectSel = true;
        private string fileImage;

        public ImgViewer(Geminapi main)
        {
            this.main = main;
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            this.ShowInTaskbar = false;

            this.Load += ImgViewer_Load;
            this.Paint += ImgViewer_Paint;

            this.KeyDown += ImgViewer_KeyDown;
            this.KeyUp += ImgViewer_KeyUp;

            this.MouseDown += ImgViewer_MouseDown;
            this.MouseUp += ImgViewer_MouseUp;
            this.MouseMove += ImgViewer_MouseMove;
            this.MouseWheel += ImgViewer_MouseWheel;

            this.FormClosing += ImgViewer_FormClosing;
            this.Activated += ImgViewer_Activated;
            this.Resize += ImgViewer_Resize;

        }
        void ImgViewer_Load(object sender, EventArgs e)
        {
            this.Location = new Point(500, 100);
            Size size = ClientSize;
            //this.Opacity = 0.6D;
        }
        void ImgViewer_Paint(object sender, PaintEventArgs e)
        {
            if (imageCurr == null)
                return;

            krect rectImg = new krect(imageCurr.Width, imageCurr.Height);

            //Size size = this.ClientSize;
            //krect rectCanvas = new krect(0, 0, size.Width, size.Height);

            //krect rectDst = rectCanvas.copy();

            //if (rectCanvas.Aspect < rectImg.Aspect)
            //{
            //    rectDst.h = rectDst.w / rectImg.Aspect;
            //    rectDst.y = 0.5f * (rectCanvas.h - rectDst.h);
            //}
            //else //if (rectCanvas.Aspect >= rectImg.Aspect)
            //{
            //    rectDst.w = rectDst.h * rectImg.Aspect;
            //    rectDst.x = 0.5f * (rectCanvas.w - rectDst.w);
            //}

            Graphics g = e.Graphics;
            g.DrawImage(imageCurr, rectDst.R, rectImg.R, GraphicsUnit.Pixel);

            if (rectSel != null)
                g.DrawRectangle(Pens.Red, rectSel.R);
        }
        void setRectDst()
        {
            if (imageCurr == null)
                return;

            Size size = this.ClientSize;
            krect rectCanvas = new krect(0, 0, size.Width, size.Height);
            krect rectImg = new krect(imageCurr.Width, imageCurr.Height);

            rectDst = rectCanvas.copy();

            if (rectCanvas.Aspect < rectImg.Aspect)
            {
                rectDst.h = rectDst.w / rectImg.Aspect;
                rectDst.y = 0.5f * (rectCanvas.h - rectDst.h);
            }
            else //if (rectCanvas.Aspect >= rectImg.Aspect)
            {
                rectDst.w = rectDst.h * rectImg.Aspect;
                rectDst.x = 0.5f * (rectCanvas.w - rectDst.w);
            }
        }
        bool openImageFile(string file, ref Bitmap bmp)
        {
            if (File.Exists(file))
            {
                bmp = (Bitmap)Image.FromFile(fileCurr);
                return true;
            }

            return false;
        }
        public void openFile(string fileimage)
        {
            this.Text = dPath.GetFileNameWithoutExtension(fileimage);
            fileCurr = fileimage;

            if (openImageFile(fileCurr, ref imageBase))
            {
                setRectDst();
                this.Invalidate();
            }
        }
        public string ImageToAscii()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("    ");

            for (int x = 0; x < imageBlack.Width; x++)
            {
                if (x == imageBlack.Width - 1)
                    break;
                if (x < 100)
                {
                    if (x % 10 == 0)
                        sb.Append($"{x:00}");
                    else if (x % 10 < 2)
                        sb.Append("");
                    else if (x % 10 >= 2)
                        sb.Append(" ");
                }
                else if (x >= 100)
                {
                    if (x % 10 == 0)
                        sb.Append($"{x:000}");
                    else if (x % 10 < 3)
                        sb.Append("");
                    else if (x % 10 >= 3)
                        sb.Append(" ");
                }
            }
            sb.AppendLine();

            for (int y = 0; y < imageBlack.Height; y++)
            {
                sb.Append($"{y:000} "); // 한 라인 시작

                for (int x = 0; x < imageBlack.Width; x++)
                {
                    Color color0 = imageBlack.GetPixel(x, y);
                    //int gray = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    int brightness = (int)(color0.R * 0.299 + color0.G * 0.587 + color0.B * 0.114);

                    // 128보다 어두우면 검은색(■), 밝으면 흰색(□)
                    //sb.Append(color0.Equals(Color.Black) ? "■" : "□");
                    sb.Append(brightness < 128 ? "■" : "□");
                }

                sb.AppendLine(); // 한 라인 종료 후 줄바꿈
            }

            return sb.ToString();
        }
        Bitmap getImageBlack(Bitmap bmp)
        {
            Bitmap bmpBla = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color color0 = bmp.GetPixel(x, y);
                    //int gray = (color0.R + color0.G + color0.B) / 3;

                    int brightness = (int)(color0.R * 0.299 + color0.G * 0.587 + color0.B * 0.114);
                    Color colorN = brightness < 128 ? Color.Black : Color.White;
                    
                    bmpBla.SetPixel(x, y, colorN);
                }
            }

            return bmpBla;
        }
        void ImgViewer_Resize(object sender, EventArgs e)
        {
            setRectDst();
            this.Invalidate();
        }
        void ImgViewer_Activated(object sender, EventArgs e)
        {

        }
        void ImgViewer_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        void ImgViewer_MouseWheel(object sender, MouseEventArgs e)
        {

        }
        void ImgViewer_MouseDown(object sender, MouseEventArgs e)
        {
            posDown = new kvec(e.X, e.Y);
            posMove = posDown.copy();

            MouseDowned = true;
        }
        void ImgViewer_MouseUp(object sender, MouseEventArgs e)
        {
            var pos = new kvec(e.X, e.Y);

            rectSel = new krect(
                Math.Min(posDown.x, pos.x),
                Math.Min(posDown.y, pos.y),
                Math.Abs(posDown.x - pos.x),
                Math.Abs(posDown.y - pos.y)
                );

            rectSel.x = Math.Max(rectSel.x, rectDst.x);
            rectSel.y = Math.Max(rectSel.y, rectDst.y);

            if (rectSel.Right > rectDst.Right)
                rectSel.x -= rectSel.Right - rectDst.Right;
            if (rectSel.Bottom > rectDst.Bottom)
                rectSel.y -= rectSel.Bottom - rectDst.Bottom;

            if (rectSel.w > 30 && rectSel.h > 30)
                BuildFileBW();

            MouseDowned = false;
            this.Invalidate();
        }
        void ImgViewer_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = new kvec(e.X, e.Y);

            if(MouseDowned)
            {
                rectSel = new krect(
                         Math.Min(posDown.x, pos.x),
                         Math.Min(posDown.y, pos.y),
                         Math.Abs(posDown.x - pos.x),
                         Math.Abs(posDown.y - pos.y)
                         );

                posMove = pos;
                this.Invalidate();
            }
        }
        void ImgViewer_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.ShiftKey:
                    SHIFT = true;
                    return;
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.ControlKey:
                    CTRL = true;
                    return;
            }

            if (CTRL)
            {
                switch (e.KeyCode)
                {
                    case Keys.S:
                        //showMessage($"화일저장 => {fileCurr}");

                        //makeThumb();
                        //saveFile();
                        return;
                    case Keys.O:

                        //if (openImageFile(fileCurr, ref imageCurr))
                        //    this.Invalidate();
                        return;
                }
            }

            if (e.KeyCode == Keys.Space)
            {
                if(DrawMode == "NORMAL")
                    DrawMode = "BLACK";
                else
                    DrawMode = "NORMAL";

                if (DrawMode == "BLACK" && imageBase != null && imageBlack == null)
                {
                    imageBlack = getImageBlack(imageBase);
                }

                this.Invalidate();
                return;
            }

            else if (e.KeyCode == Keys.R)
            {
                visibleRectSel = !visibleRectSel;
                this.Invalidate();
                return;
            }
        }
        void ImgViewer_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.ShiftKey:
                    SHIFT = false;
                    return;
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.ControlKey:
                    CTRL = false;
                    return;
            }

            if (CTRL)
            {
                switch (e.KeyCode)
                {
                    case Keys.S:
                        return;
                    case Keys.O:
                        return;
                }

                switch (e.KeyCode)
                {
                    case Keys.D2:
                    case Keys.D3:
                    case Keys.D4:
                    case Keys.D5:
                        this.Invalidate();
                        return;
                }
            }
            if (e.KeyCode == Keys.Escape)
            {
                main.Focus();
                main.Activate();
                return;
            }
        }
        internal void SetOpenFile(string file)
        {
            fileCurr = file;

            if (openImageFile(file, ref imageBase))
                this.Invalidate();
        }
        internal void ShowImage(string filepath)
        {
            this.Show();
            fileImage = filepath;
            openFile(filepath);
        }
        internal bool GetFileBW(ref string filetext, ref string fileimage)
        {
            if (rectSel == null || imageBlack == null)
                return false;

            if (!File.Exists(fileBwRec) || !File.Exists(fileBmpRedRec))
                return false;

            filetext = fileBwRec;
            fileimage = fileBmpRedRec;

            return true;
        }
        void BuildFileBW()
        {
            if (imageBlack == null)
                return;

            var dirFileImage = Path.GetDirectoryName(fileImage);
            var nameNude = Path.GetFileNameWithoutExtension(fileImage);

            var dirBW = $@"{dirFileImage}\{nameNude}_BW";
            if(!Directory.Exists(dirBW))
                Directory.CreateDirectory(dirBW);

            /* 화일명 null 값 탈출 */
            fileBmpRedRec = $@"{dirFileImage}\{nameNude}_BW\imageRedOnBlack.jpg";
            fileBwRec = $@"{dirFileImage}\{nameNude}_BW\bwInRedRec.txt";

            var rectSelRed = rectSel.copy();
            rectSelRed.x = rectSel.x - rectDst.x;
            rectSelRed.y = rectSel.y - rectDst.y;

            var scale = rectDst.w / imageBlack.Width;
            rectSelRed.scale(1f / scale);

            using (var bmpBlackRed = new Bitmap(imageBlack.Width, imageBlack.Height, PixelFormat.Format24bppRgb))
            {
                using (Graphics g = Graphics.FromImage(bmpBlackRed))
                {
                    var rectSrc = new krect(bmpBlackRed.Width, bmpBlackRed.Height);
                    g.DrawImage(imageBlack, rectSrc.R, rectSrc.R, GraphicsUnit.Pixel);
                    g.DrawRectangle(Pens.Red, rectSelRed.R);

                    bmpBlackRed.Save(fileBmpRedRec, ImageFormat.Jpeg);
                }
            }

            var text = $"rectangle : {rectSelRed.toString()}\n";
            for (int y = rectSelRed.Y; y <= rectSelRed.Bottom; y++)
            {
                if (y < 0) continue;
                if (y > imageBlack.Height - 1)
                    break;

                for (int x = rectSelRed.X; x <= rectSelRed.Right; x++)
                {
                    if (x < 0) continue;
                    if (x > imageBlack.Width - 1)
                        break;

                    Color color = imageBlack.GetPixel(x, y);
                    int brightness = (int)(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);

                    // 128보다 어두우면 검은색(■), 밝으면 흰색(□)
                    //sb.Append(color0.Equals(Color.Black) ? "■" : "□");
                    text += brightness < 128 ? "■" : "□";
                }

                if (y < rectSelRed.Bottom)
                    text += "\n";
            }

            var fw = new FileStream(fileBwRec, FileMode.Create, FileAccess.Write);
            var data = Encoding.UTF8.GetBytes(text);
            fw.Write(data, 0, data.Length);
            fw.Close();
        }
    }
}

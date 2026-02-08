#define LOAD_LIMA

using shine.libs.capture;
using shine.libs.math;
using shine.libs.pad;
using shine.libs.window;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bacro
{
    public partial class Navi : Form
    {
        public string dirCoords;

        string fileship;
        string filemap, fileinput;
        string filecityposs;
        Bitmap ImageBigMap;
        Bitmap bmpShip;

        kvec posMove = new kvec();
        kvec posPivot = new kvec();
        bool mouseDowned = false;
        object lockImagePoints = new object();
        bool blockResizeEvent = false;

        krect rectCut,rectCutDown;
        krect rectCutA,rectCutB;

        krect rectDraw;
        krect rectDrawA, rectDrawB;

        public List<kvec> coordsWorld = new List<kvec>();
        string fileCoord;

        bool CTRL, SHIFT, ALT;
        bool onKeepPaintSleep = false;
        
        int IndexSel = -1;
        public kvec ShipGps = null;
        public kvec ShipGpsStart = null;
        public string StartCity;
        public string GoalCity;

        public Dictionary<string, kvec> CityPoss = new Dictionary<string, kvec>();

        Bacro main;
        Sail sail;

        public float BAE
        {
            get
            {
                if (sail != null)
                    return sail.RectWorld.w / ImageBigMap.Width;
                else
                    return 1f;
            }
        }
        public Navi(Bacro main)
        {
            this.main = main;
            this.sail= main.sail;

            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            this.ShowInTaskbar = false;

            this.Paint += Navi_Paint;
            this.KeyDown += Navi_KeyDown;
            this.KeyUp += Navi_KeyUp;
            this.MouseDown += Navi_MouseDown;
            this.MouseUp += Navi_MouseUp;
            this.MouseMove += Navi_MouseMove;
            this.MouseWheel += Navi_MouseWheel;

            this.Load += Navi_Load;
            this.Resize += Navi_Resize;
            this.FormClosing += Navi_Closing;

            CTRL = SHIFT = ALT = false;
        }
        void Navi_Closing(object sender, FormClosingEventArgs e)
        {
            saveRectInfo(fileinput);
        }
        void Navi_Load(object sender, EventArgs e)
        {
            dirCoords = $@"{main.dirData}\navigate";
            fileship = $@"{main.dirData}\karak.png";
            filemap = $@"{main.dirData}\worldmap_c.png";
            fileinput = $@"{main.dirData}\input.txt";
            filecityposs = $@"{main.dirData}\cities.txt";

            ImageBigMap = (Bitmap)Bitmap.FromFile(filemap);

            krect rectcut, rectform;
            readRectInfo(fileinput, out rectform, out rectcut);

            if (rectform != null)
            {
                this.Location = rectform.pos().P;
                this.ClientSize = rectform.Size;
            }

            Size size = this.ClientSize;

            if (rectcut != null)
            {
                rectCut = rectcut;

                var aspect = rectCut.w / rectCut.h;
                var height = (int)(size.Width / aspect);

                this.ClientSize = new Size(size.Width, height);
            }
            else
            {
                rectCut = new krect(size.Width / 2, size.Height / 2);
            }

            /*rectCut fix 꼭 실행*/
            FixRectCut();

            rectCutDown = rectCut.copy();

            bmpShip = (Bitmap)Bitmap.FromFile(fileship);

            readCityPoss();

#if LOAD_LIMA
            /* 리마 리마 좌표 열기*/
            fileCoord = $@"{main.dirData}\navigate\리마 리마.txt";
            if (fileCoord != null && File.Exists(fileCoord))
                openCoords_core(fileCoord);
#endif
        }
        void Navi_Resize(object sender, EventArgs e)
        {
            if (blockResizeEvent)//한번만 막을 수 있다.
            {
                blockResizeEvent = false;
                return;
            }

            Size size = this.ClientSize;

            var width = Math.Max(200, size.Width);
            var height = Math.Max(200, size.Height);

            if (width != size.Width || height != size.Height)
            {
                blockResizeEvent = true;
                this.ClientSize = new Size(width, height);
            }

            if (rectCut != null)
            {
                size = this.ClientSize;//다시 잰다. 위에서 바뀔 경우
                var aspectx = 1f * size.Width / size.Height;

                rectCut.h = rectCut.w / aspectx;
                FixRectCut();
            }

            blockResizeEvent = false;
            this.Invalidate();
        }
        void Navi_MouseWheel(object sender, MouseEventArgs e)
        {
            var wheel = e.Delta;

            //this.Text = $"wheel == {wheel}";

            if (wheel > 0)
            {
                if (rectCut.h > 200f)
                {
                    rectCut.scaleCen(0.7f);
                    FixRectCut();

                    this.Invalidate();
                }
            }
            else if (wheel < 0)
            {
                if (rectCut.h< 900f)
                {
                    rectCut.scaleCen(1.5f);
                    FixRectCut();

                    this.Invalidate();
                }
            }
        }
        void Navi_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDowned = true;

            posPivot = new kvec(e.X, e.Y);
            rectCutDown = rectCut.copy();
        }
        void Navi_MouseMove(object sender, MouseEventArgs e)
        {
            posMove = new kvec(e.X, e.Y);

            if (mouseDowned)
            {
                var dis = posMove - posPivot;
                rectCut = rectCutDown.copy();
                rectCut.offset(-1f * dis);

                FixRectCut();

                if (!onKeepPaintSleep)
                    new Thread(PaintIntermittent).Start();
            }
            else
            {
                if (sail != null)
                {
                    float bAE = sail.RectWorld.w / ImageBigMap.Width;
                    var coordWorld = bAE * calcPosOnImage(posMove);

                    if (coordWorld.X == 0)
                        coordWorld.x = sail.RectWorld.w + 1;

                    this.Text = coordWorld.toString2();
                }
            }
        }
        void Navi_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDowned = false;
        }
        void Navi_KeyDown(object sender, KeyEventArgs e)
        {
            if (CTRL && e.KeyCode == Keys.M)
            {
                //main.navimap_StartSailing();
                return;
            }
            else if (CTRL && e.KeyCode == Keys.N)
            {
                //main.navimap_StopSailing();
                return;
            }
            
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
        void Navi_KeyUp(object sender, KeyEventArgs e)
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
                    case Keys.O:
                        CTRL = false;
                        openCoords();
                        return;
                    case Keys.S:
                        CTRL = false;
                        saveCoords();
                        return;
                    case Keys.U:
                        CTRL = false;
                        main.LoadPics();
                        readCityPoss();
                        Console.WriteLine("bacro.LoadPics();");
                        Console.WriteLine("navi.readCityPoss();");
                        return;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.S:
                        if (!mouseDowned)
                        {
                           
                        }
                        return;
                    case Keys.E:
                        if (!mouseDowned)
                        {
                          
                        }
                        return;
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
                            coordsWorld.Clear();
                            ShipGps = ShipGpsStart = null;
                            GoalCity = null;

                            this.Invalidate();
                        }
                        return;
                    case Keys.T:
                        if (!mouseDowned)
                        {
                            if (SelPoint(posMove))
                            {
                            }
                            this.Invalidate();
                        }
                        return;
                    case Keys.I:
                        if (!mouseDowned)
                        {
                            if (!InsertPoint(posMove))
                                IndexSel = -1;

                            this.Invalidate();
                        }
                        return;
                }

                switch (e.KeyCode)
                {
                    case Keys.OemOpenBrackets:
                        if (!mouseDowned)
                        {

                        }
                        return;
                    case Keys.OemCloseBrackets:
                        if (!mouseDowned)
                        {
                        }
                        return;
                    case Keys.OemBackslash:
                        if (!mouseDowned)
                        {

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
            kvec posOnImage = (1f / BAE) * coordsWorld[indexSel];
            posOnImage.offset(dx, dy);

            posOnImage.x = Math.Max(0, posOnImage.x);
            posOnImage.x = Math.Min(posOnImage.x, ImageBigMap.Width);
            posOnImage.y = Math.Max(0, posOnImage.y);
            posOnImage.y = Math.Min(posOnImage.y, ImageBigMap.Height);

            coordsWorld[indexSel] = BAE * posOnImage;
        }
        void Navi_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Monitor.Enter(lockImagePoints);

                if (ImageBigMap != null)
                {
                    navi_paint_core(e.Graphics);

                }
            }
            finally
            {
                Monitor.Exit(lockImagePoints);
            }
        }
        void navi_paint_core(Graphics g)
        {
            int xMax = ImageBigMap.Width;

            Size size = this.ClientSize;

            rectDraw = new krect(size.Width, size.Height);

            if (rectCut.Right <= xMax)
            {
                g.DrawImage(ImageBigMap, rectDraw.R, rectCut.R, GraphicsUnit.Pixel);
            }
            else
            {
                g.DrawImage(ImageBigMap, rectDrawA.R, rectCutA.R, GraphicsUnit.Pixel);
                g.DrawImage(ImageBigMap, rectDrawB.R, rectCutB.R, GraphicsUnit.Pixel);
            }

            if (coordsWorld.Count > 1)
                drawLinesBetwenPoints(g);

            drawPoints(g);
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
            //if (coords.Count > 1)
            //    drawLinesBetwenPoints(g);

            int index = -1;
            var M = 4;

            foreach (var coord in coordsWorld)
            {
                index++;

                kvec pos = calcDrawPosW(coord);
                Color colorPen = index == IndexSel ? Color.Blue : Color.Pink;

                drawPointX(g, pos, colorPen, M);
            }

            if (IndexSel > -1 && IndexSel < coordsWorld.Count)
            {
                this.Text = coordsWorld[IndexSel].toString2();
            }

            if (ShipGps != null)
            {
                krect rectsrc = new krect(bmpShip.Width, bmpShip.Height);
                krect rectdst = new krect(M * 4, M * 4);

                kvec posShipOnImage = (1f / BAE) * ShipGps;
                kvec posShip = calcDrawPos(posShipOnImage);

                rectdst.setxy(posShip.x, posShip.y);
                rectdst.offset(-M * 2, -M * 2);

                g.DrawImage(bmpShip, rectdst.R, rectsrc.R, GraphicsUnit.Pixel);
            }
        }
        kvec calcDrawPosW(kvec coord)
        {
            kvec posOnImage = (1f / BAE) * coord;

            return calcDrawPos(posOnImage);
        }
        kvec calcDrawPos(kvec posOnImage)
        {
            var pos = posOnImage.copy();

            int xMax = ImageBigMap.Width;
            float bAEyUL = rectDraw.h / rectCut.h;

            if (rectCut.Right <= xMax)
            {
                pos.offset(-1f * rectCut.pos());
                pos.scale(bAEyUL);
                pos.offset(rectDraw.pos());
            }
            else
            {
                if (rectCutA.contains(posOnImage))
                {
                    pos.offset(-1f * rectCutA.pos());
                    pos.scale(bAEyUL);
                    pos.offset(rectDrawA.pos());
                }
                else if (rectCutB.contains(posOnImage))
                {
                    pos.offset(-1f * rectCutB.pos());
                    pos.scale(bAEyUL);
                    pos.offset(rectDrawB.pos());
                }
            }

            return pos;
        }
        void drawLinesBetwenPoints(Graphics g)
        {
            List<kvec> points = new List<kvec>();
            foreach(var coord in coordsWorld)
                points.Add((1f / BAE) * coord);

            /* 라인그리기(출발지점 --- 첫좌표) */
            if (ShipGpsStart != null)
                points.Insert(0, (1f / BAE) * ShipGpsStart);

            kvec p0, p1;
            Pen pen = Pens.Gray;

            var size = this.ClientSize;

            for (int k = 1; k < points.Count; k++)
            {
                p0 = calcDrawPos(points[k - 1]);
                p1 = calcDrawPos(points[k]);

                if ((p1 - p0).length() > 0.8f * size.Width)
                    continue;

                g.DrawLine(pen, p0.P, p1.P);
            }
        }
        void drawPointX(Graphics g, kvec coord, Color colorPen, int M)
        {
            var lt = coord.copy(); lt.offset(-M, -M);
            var rt = coord.copy(); rt.offset(+M, -M);
            var lb = coord.copy(); lb.offset(-M, +M);
            var rb = coord.copy(); rb.offset(+M, +M);

            Pen pen = new Pen(colorPen);

            g.DrawLine(pen, lt.P, rb.P);
            g.DrawLine(pen, rt.P, lb.P);
        }
        void readRectInfo(string fileinput, out krect rectform, out krect rectcut )
        {
            rectform = null;
            rectcut = null;

            var lines = File.ReadAllLines(fileinput, Encoding.UTF8);

            foreach (var linex in lines)
            {
                var line = linex.Trim();
                if (line.Length < 3) continue;
                if (line.Substring(0, 2) == "//") continue;

                var ss = Regex.Split(line, "::");
                if (ss.Length == 2)
                {
                    var title = ss[0].Trim();
                    var text = ss[1].Trim();

                    if (title == "rect map" && text != "null")
                        rectcut = krect.Parse(text);
                    else if (title == "rect form" && text != "null")
                        rectform = krect.Parse(text);
                }
            }
        }
        void saveRectInfo(string fileinput)
        {
            var lines = File.ReadAllLines(fileinput, Encoding.UTF8);

            var textfull = "";
            int n = -1;

            foreach (var line in lines)
            {
                n++;
                var linex = line;
                var ss = Regex.Split(line, "::");

                if (ss.Length == 2)
                {
                    var title = ss[0].Trim();
                    var text = ss[1].Trim();

                    if (title == "rect map")
                        linex = $"rect map :: {rectCut.toString()}";
                    else if (title == "rect form")
                    {
                        var pos = this.Location;
                        var size = this.ClientSize;
                        linex = $"rect form :: {pos.X} {pos.Y} {size.Width} {size.Height}";
                    }
                }

                if (n == 0)
                    textfull += $"{linex}";
                else
                    textfull += $"\n{linex}";
            }

            FileStream fw = new FileStream(fileinput, FileMode.Create, FileAccess.Write);
            byte[] data = Encoding.UTF8.GetBytes(textfull);
            fw.Write(data, 0, data.Length);
            fw.Close();
        }
        void FixRectCut()
        {
            if (rectDraw == null)
                rectDraw = new krect(this.ClientSize);

            int xMax = ImageBigMap.Width;
            int yMax = ImageBigMap.Height;

            /* y fix */
            rectCut.y = Math.Max(rectCut.y, 0);
            rectCut.y = Math.Min(rectCut.y, yMax - rectCut.h);

            /* x fix */
            if (rectCut.x < 0f)
                rectCut.offset(xMax, 0);
            else if (rectCut.x > xMax)
                rectCut.offset(-xMax, 0);

            /* 여기서 손보는 게 개이득 */
            if (rectCut.Right > xMax)
            {
                rectCutB = rectCut.copy();
                rectCutB.w = rectCut.Right - xMax;
                rectCutB.x = 0;

                rectCutA = rectCut.copy();
                rectCutA.w = rectCut.w - rectCutB.w;

                rectDrawA = rectDraw.copy();
                rectDrawA.w = rectDraw.w * (rectCutA.w / rectCut.w);

                rectDrawB = rectDraw.copy();
                rectDrawB.w = rectDraw.w * (rectCutB.w / rectCut.w);
                rectDrawB.x = rectDrawA.Right;
            }
        }
        public kvec calcWorldCoord(kvec coord)
        {
            /*
            1해리 1.852km
            1마일 1.608km
            1dho 2.823km
             */

            float bAE = sail.RectWorld.w / ImageBigMap.Width;
            var coordWorld = bAE * coord;

            if (coordWorld.X == 0)
                coordWorld.x = sail.RectWorld.w + 1;

            return coordWorld;
        }
        kvec calcPosOnImage(kvec pos)
        {
            Size size = this.ClientSize;
            float BAE = rectCut.w / size.Width;

            var posMap = BAE * pos;
            posMap.offset(rectCut.pos());

            if (posMap.x > ImageBigMap.Width)
                posMap.x -= ImageBigMap.Width;
            if (posMap.x < 0)
                posMap.x += ImageBigMap.Width;

            return posMap;
        }
        void AddPoint(kvec pos)
        {
            kvec posOnImage = calcPosOnImage(pos);

            int n = -1;
            int indexExists = -1;

            foreach (var coord in coordsWorld)
            {
                n++;

                var p = (1f / BAE) * coord;
                var dis = p - posOnImage;

                if (dis.length() < 3f)
                {
                    indexExists = n;
                    break;
                }
            }

            if (indexExists > -1)
                coordsWorld.RemoveAt(indexExists);
            else
                coordsWorld.Add(BAE * posOnImage);
        }
        bool SelPoint(kvec pos)
        {
            kvec posOnImage = calcPosOnImage(pos);

            int n = -1;
            int indexExists = -1;

            foreach (var coord in coordsWorld)
            {
                n++;

                var p = (1f / BAE) * coord;
                var dis = p - posOnImage;

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
        }
        bool InsertPoint(kvec pos)
        {
            if (IndexSel > -1 && IndexSel < coordsWorld.Count)
            { }
            else
                return false;

            kvec posOnImage = calcPosOnImage(pos);

            int n = -1;
            int indexExists = -1;

            foreach (var coord in coordsWorld)
            {
                n++;

                var p = (1f / BAE) * coord;
                var dis = p - posOnImage;

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
                coordsWorld.Insert(IndexSel + 1, BAE * posOnImage);
                return true;
            }
        }
        void openCoords()
        {
            IndexSel = -1;
            var file = openFileDialog(dirCoords);

            if (file != null)
            {
                fileCoord = file;
                openCoords_core(file);

                this.Invalidate();

                Console.WriteLine($"좌표읽음 :\n {file}");
            }
        }
        public void openCoords_core(string file)
        {
            if (!File.Exists(file))
                return;

            var lines = File.ReadAllLines(file, Encoding.UTF8);

            coordsWorld.Clear();
          
            foreach (var linex in lines)
            {
                var line = linex.Trim();
                if (line.Length == 0)
                    continue;
                if (line.Substring(0, 1) == "#")
                    continue;

                var ss = Regex.Split(line, ":");
                if (ss.Length != 2) continue;

                coordsWorld.Add(kvec.Parse2d(ss[1].Trim()));
            }

            GoalCity = null;
            ShipGps = ShipGpsStart = null;

            /* 목표 항구 이름 설정 */
            GoalCity = getGoalCity(file);
            Console.WriteLine($"★★ 목표 항구★★ == {GoalCity}");
        }
        string getGoalCity(string file)
        {
            string goalCity = null;

            if (file != null)
            {
                var namenude = Path.GetFileNameWithoutExtension(file);
                var cc = Regex.Split(namenude, " ");

                if (cc.Length >= 2)
                {
                    var gCity = cc[1].Trim();
                    goalCity = gCity;

                    if (CityPoss.Keys.Contains(gCity)) { }
                    else if (main.pics.Keys.Contains(gCity)) { }
                    else goalCity = "x" + gCity;
                }
            }
            return goalCity;
        }
        void saveCoords()
        {
            var file = saveFileDialog(dirCoords);

            if (file != null && coordsWorld.Count > 1)
            {
                fileCoord = file;
                GoalCity = getGoalCity(file);
                ShipGps = ShipGpsStart = null;

                if (coordsWorld.Count > 0)
                    if (GoalCity != null && CityPoss.Keys.Contains(GoalCity))
                    {
                        coordsWorld[coordsWorld.Count - 1] = CityPoss[GoalCity];
                        this.Invalidate();
                    }

                string text = "";
                int n = -1;

                foreach (var coord in coordsWorld)
                {
                    if (++n > 0) text += "\n";
                    text += $"{n} : {coord.toString2()}";
                }

                FileStream fw = new FileStream(file, FileMode.Create, FileAccess.Write);

                byte[] data = Encoding.UTF8.GetBytes(text);

                fw.Write(data, 0, data.Length);
                fw.Close();

                Console.WriteLine($"좌표저장 :\n {file}");
            }
        }
        string saveFileDialog(string dirInit)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.Title = "좌표 저장";
            saveDialog.FileName = "*.txt";
            if (fileCoord != null && File.Exists(fileCoord))
                saveDialog.FileName = Path.GetFileNameWithoutExtension(fileCoord);

            saveDialog.InitialDirectory = dirInit;
            saveDialog.Filter = "text (*.txt) | *.txt | 모든 파일 (*.*) | *.*";

            DialogResult dr = saveDialog.ShowDialog();

            if (dr == DialogResult.OK)
                return saveDialog.FileName;
            else
                return null;
        }
        string openFileDialog(string dirInit)
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            openDialog.Title = "좌표 열기";
            openDialog.FileName = "*.txt";
            if (fileCoord != null && File.Exists(fileCoord))
                openDialog.FileName = Path.GetFileNameWithoutExtension(fileCoord);
            openDialog.InitialDirectory = dirInit;
            openDialog.Filter = "text (*.txt) | *.txt | 모든 파일 (*.*) | *.*";

            DialogResult dr = openDialog.ShowDialog();

            if (dr == DialogResult.OK)
                return openDialog.FileName;
            else
                return null;
        }
        public void SetShipGps(kvec shipGps)
        {
            ShipGps = shipGps.copy();

            if (ShipGpsStart == null)
                ShipGpsStart = ShipGps.copy();

            if (InvokeRequired)
            {
                this.Invoke(new Action(() => { this.Invalidate(); }));
            }
            else
            {
                this.Invalidate();
            }
        }
        void readCityPoss()
        {
            var lines = File.ReadAllLines(filecityposs, Encoding.UTF8);

            CityPoss.Clear();

            //포르투 /==/ 15764 3116 / 90
            foreach (var linex in lines)
            {
                var line = linex.Trim();
                if (line.Length == 0)
                    continue;

                var ss = Regex.Split(line, " /==/ ");

                var name = ss[0].Trim();
                var nn = Regex.Split(ss[1], " / ");

                CityPoss[name] = kvec.Parse2d(nn[0].Trim());
            }
        }
    }
}

using Android.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpToken;
using shine.libs.konst;
using shine.libs.math;
using shine.libs.capture;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
//using static System.Net.WebRequestMethods;
//using static System.Net.Mime.MediaTypeNames;

namespace Geminapi
{
    public partial class Geminapi : Form
    {
        const string 다이아 = "◇";
        const string 사용 = "사용";
        const string _서버 = "_서버";
        const string _캐시 = "_캐시";
        const string _BW = "_BW";
        const string _텍스트 = "_텍스트";
        const string _이미지 = "_이미지";
        const string _제미나이 = "_제미나이";
        const string 플래시 = "플래시";
        const string 프로 = "프로";
        const string 바나나 = "바나나";
        const string 저장text = "저장text";
        const string 저장image = "저장image";
        const string 저장shot = "저장shot";
        const string 저장log = "저장log";
        const string 저장bwbw = "저장bwbw";
        const string 저장black = "저장black";
 
        const string 텍스트0 = "텍스트0";
        const string 이미지0 = "이미지0";
       
        const string 폴더2서버 = "폴더2서버";
        const string Using캐시 = "Using캐시";
        const string UsingBW = "UsingBW";
        const string Build캐시 = "Build캐시";

        Dictionary<string, string> dics = new Dictionary<string, string>();
        Dictionary<string, string> fileIds = new Dictionary<string, string>();
        Dictionary<string, string> cashIds = new Dictionary<string, string>();
        Dictionary<string, krect> rects = new Dictionary<string, krect>();

        FileMan fm;
        KShow kshow;
        ImgViewer imgv;
        MementoStory mstory;
        KConsole kcon;
        TextBoard tboard;
        KPostAsync kps;
        Shotter shotter;

        GptEncoding gptEnco;
        long ToKen = 0;

        string dirApi = @"d:\Works\ApiGemini";
        string 현재네임키 = "";

        bool CTRL, SHIFT;
        bool OnShowMessage = false;
        object lockDics = new object();

        public Geminapi()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            this.Load += 폼로드시;
            this.Paint += 폼페인트;

            this.KeyDown += 키다운;
            this.KeyUp += 키업;

            this.MouseDown += 마우스다운;
            this.MouseUp += 마우스업;
            this.MouseMove += 마우스무브;
            this.FormClosing += 폼닫힐때;

            CTRL = SHIFT = false;
            /* TLS 1.2 강제 */
            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;
            kps = new KPostAsync();
        }
        void 폼닫힐때(object sender, FormClosingEventArgs e)
        {
            환경저장();


        }
        void 폼로드시(object sender, EventArgs e)
        {
            kcon = new KConsole(this);
            kcon.Show();

            Console.SetOut(new WinFormConsoleWriter(kcon.TextWords));

            // 이제 테스트 해보세요
            Console.WriteLine("Console.SetOut(new WinFormConsoleWriter(kcon.TextWords));");

            shotter = new Shotter();

            환경불러오기();
            //LoadCash();
            환경변수들위치정하기();
        }
        void 폼페인트(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Monitor.Enter(lockDics);
            try
            {
                if (dics.Count == 0)
                    return;

                Size size = this.ClientSize;

                float H = 24;
                //float W = size.Width / 6;

                Font font = new Font("맑은 고딕", 10f);
                Font fontB = new Font("맑은 고딕", 10f, FontStyle.Bold);

                krect rect0 = new krect(0, 0, size.Width / 3, H);

                var n = -1;
                var x = 0;

                foreach (var k in dics.Keys)
                {
                    n++;
                    krect rect = rects[k];
                    krect rectT = rect.copy();
                    krect rectC = rect.copy();
                    rectT.offset(5, 2);
                    rectC.offset(5 + rect.w / 4f, 2);

                    var text = dics[k];
                    var mkey = k;

                    if (k.Contains("＠"))
                    {
                        g.DrawString($"♧ {text} ♧", fontB, Brushes.Black, rectC.R);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.LightGray, rect.R);

                        /* 경로 출력 줄이기 */
                        if (text.StartsWith("texts"))
                            text = text.Substring(6);
                        else if (text.StartsWith("images"))
                            text = text.Substring(7);

                        if (mkey.StartsWith("텍스트"))
                        {
                            /* 네임키 생략 */
                            rectT.offset(20, 0);
                            g.DrawString($"{text}", font, Brushes.Black, rectT.R);
                        }
                        else if (mkey.StartsWith("이미지"))
                        {
                            /* 네임키 생략 */
                            rectT.offset(20, 0);
                            g.DrawString($"{text}", font, Brushes.Black, rectT.R);
                        }
                        else
                        {
                            g.DrawString($"{mkey}", fontB, Brushes.Black, rectT.R);
                            g.DrawString($"{text}", font, Brushes.Black, rectC.R);
                        }

                        /* 선택된 */
                        if (k == 현재네임키)
                        {
                            g.DrawRectangle(Pens.Red, rect.R);
                        }
                    }
                }
            }
            finally
            {
                Monitor.Exit(lockDics);
            }
        }
        void 마우스무브(object sender, MouseEventArgs e)
        {
            kvec pos = new kvec(e.X, e.Y);

            string 현재네임키New = null;

            foreach (var k in rects.Keys)
            {
                if (rects[k].contains(pos))
                {
                    현재네임키New = k;
                    break;
                }
            }

            if (현재네임키New == null)
                현재네임키 = "";
            else if (현재네임키New != 현재네임키)
            {
                현재네임키 = 현재네임키New;
                this.Invalidate();
            }
        }
        void 마우스업(object sender, MouseEventArgs e)
        {
        }
        void 마우스다운(object sender, MouseEventArgs e)
        {
        }
        void 키다운(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;

            switch (keyCode)
            {
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    CTRL = true;
                    return;
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    SHIFT = true;
                    return;
            }

            //e.SuppressKeyPress = true;

            switch (keyCode)
            {
                case Keys.Escape:
                    if (SHIFT)
                    {
                        Application.Exit();
                        SHIFT = false;
                    }
                    else
                        this.Hide();

                    e.SuppressKeyPress = true;
                    return;
            }
        }
        void 키업(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;

            switch (keyCode)
            {
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    CTRL = false;
                    return;
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    SHIFT = false;
                    return;
            }

            var lenKey = 현재네임키.Length;

            if (현재네임키.StartsWith("텍스트") || 현재네임키.StartsWith("이미지"))
            {
                키업_텍미지_준비(keyCode);
                return;
            }
            else if (현재네임키.StartsWith("_"))
            {
                키업_제이슨_포함(keyCode);
                return;
            }
            else if (현재네임키.StartsWith("저장"))
            {
                키업_저장(keyCode);
                return;
            }
            else
            {
                키업_기타(keyCode, 현재네임키);
                //return;
            }

            if(CTRL)
            {
                switch (keyCode)
                {
                    case Keys.K:


                        break;
                }

            }
        }
        async void 키업_텍미지_준비(Keys keyCode)
        {
            string result = "";
            switch (keyCode)
            {
                case Keys.F:
                    파일관리자열기(현재네임키);
                    return;
                case Keys.Q:
                    파일내용보기(현재네임키);
                    this.Invalidate();
                    return;
                case Keys.T:
                    ShowTBoard(현재네임키);
                    this.Invalidate();
                    return;
                case Keys.P:
                    서버에화일올리기(현재네임키);
                    this.Invalidate();
                    return;
                case Keys.L:
                    result = await kps.GetFilesListInServer();
                    Console.WriteLine(result);
                    //parseFileIds(result);
                    this.Invalidate();
                    break;
                case Keys.OemSemicolon:
                    result = await kps.GetCashesList();
                    Console.WriteLine(result);
                    break;
                case Keys.X:
                    dics[현재네임키] = 다이아;
                    this.Invalidate();
                    return;
                case Keys.Space:
                    텍스트또는이미지화일셋팅(현재네임키);

                    this.Invalidate();
                    return;
            }
        }
        void 키업_제이슨_포함(Keys keyCode)
        {
            var msg = $"{keyCode} : {현재네임키}";
            메세지창띄우기(msg, 1000);
        
            if (현재네임키 == _텍스트)
            {
                switch (keyCode)
                {
                    case Keys.X:
                        dics[_텍스트] = "◇";
                        if (mstory != null)
                            mstory.Text = MakeTitleMStory();
                        break;
                }
                this.Invalidate();
            }
            else if (현재네임키 == _이미지)
            {
                switch (keyCode)
                {
                    case Keys.X:
                        dics[_이미지] = "◇";
                        if (mstory != null)
                            mstory.Text = MakeTitleMStory();
                        break;

                }
                this.Invalidate();
            }
            else if (현재네임키 == _제미나이)
            {
                if (keyCode == Keys.Space)
                {
                    switch (dics[_제미나이])
                    {
                        case 플래시: dics[_제미나이] = 바나나; break;
                        case 바나나: dics[_제미나이] = 프로; break;
                        case 프로: dics[_제미나이] = 플래시; break;
                        default: dics[_제미나이] = 플래시; break;
                    }

                    this.Invalidate();
                }
            }
            else if (현재네임키 == _서버)
            {
                switch (keyCode)
                {
                    case Keys.Space:
                        dics[_서버] = dics[_서버] == 사용 ? 다이아 : 사용;
                        break;

                }
                this.Invalidate();
            }
            else if (현재네임키 == _캐시)
            {
                switch (keyCode)
                {
                    case Keys.Space:
                        dics[_캐시] = dics[_캐시] == 사용 ? 다이아 : 사용;
                        break;

                }
                this.Invalidate();
            }
            else if (현재네임키 == _BW)
            {
                switch (keyCode)
                {
                    case Keys.Space:
                        dics[_BW] = dics[_BW] == 사용 ? 다이아 : 사용;
                        break;

                }
                this.Invalidate();
            }
        }
        async void 키업_기타(Keys keyCode, string 현재네임키)
        {
            var msg = $"{keyCode} : {현재네임키}";
            메세지창띄우기(msg, 1000);

            if (현재네임키 == Build캐시)
            {
                switch (keyCode)
                {
                    case Keys.F:
                        var dirStart = $@"{dirApi}\build";
                        파일관리자열기(현재네임키, dirStart);
                        break;
                    case Keys.P:
                        캐시만들기();
                        break;
                }
            }
            else if (현재네임키 == Using캐시)
            {
                switch (keyCode)
                {
                    case Keys.F:
                        var dirStart=$@"{dirApi}\cashIds";
                        파일관리자열기(현재네임키, dirStart);
                        break;
                    case Keys.L:
                        캐시리스트확인();
                        break;
                    case Keys.X:
                        dics[Using캐시] = "◇";
                        this.Invalidate();
                        break;
                    case Keys.Delete:
                        캐시삭제();
                        break;
                }
            }
            else if (현재네임키 == UsingBW)
            {
                switch (keyCode)
                {
                    case Keys.F:
                        var filename = dics[UsingBW];
                        var dirStart = $@"{dirApi}\{filename}";
                        파일관리자열기(현재네임키, dirStart);
                        break;
                    case Keys.L:
                        //GetListCashes();
                        break;
                    case Keys.X:
                        dics[UsingBW] = "◇";
                        this.Invalidate();
                        break;
                    case Keys.Delete:
                        //DeleteCash();
                        break;
                }
            }
            else if (현재네임키 == 폴더2서버)
            {
                var result = "";
                switch (keyCode)
                {
                    case Keys.P:
                        서버에화일보내기(현재네임키);
                        break;
                    case Keys.F:
                        파일관리자열기(현재네임키);
                        break;
                    case Keys.Delete:
                        서버화일들삭제();
                        break;
                    case Keys.X:
                        dics[현재네임키] = 다이아;
                        this.Invalidate();
                        break;
                    case Keys.L:
                        result = await kps.GetFilesListInServer();
                        Console.WriteLine(result);

                        fileIds.Clear();
                        fileIds = kps.parseFileIds(result);

                        if (fm != null && fm.IsDisposed)
                            fm = null;
                        if (fm == null)
                            fm = new FileMan(this);

                        fm.FeedFileIds(fileIds);
                        break;
                }
            }
        }
        private void 키업_저장(Keys keyCode)
        {
            var msg = $"{keyCode} : {현재네임키}";
            메세지창띄우기(msg, 1000);

            if (현재네임키 == 저장text)
            {
                switch (keyCode)
                {
                    case Keys.Q:
                        저장텍스트(현재네임키);
                        break;
                    case Keys.F:
                        파일관리자열기(현재네임키);
                        break;
                }
            }
            else if (현재네임키 == 저장image)
            {
                switch (keyCode)
                {
                    case Keys.Q:
                        저장이미지(현재네임키);
                        break;
                    case Keys.F:
                        파일관리자열기(현재네임키);
                        break;
                }
            }
            else if (현재네임키 == 저장log)
            {
                switch (keyCode)
                {
                    case Keys.Q:
                        인공지능답변보기();
                        break;
                    case Keys.F:
                        파일관리자열기(현재네임키);
                        break;
                }
            }
            else if (현재네임키 == 저장shot)
            {
                switch (keyCode)
                {
                    case Keys.Q:
                        저장이미지(현재네임키);
                        break;
                    case Keys.F:
                        파일관리자열기(현재네임키);
                        break;
                    case Keys.V:
                        if (shotter != null && shotter.IsDisposed) shotter = null;
                        if (shotter == null) shotter = new Shotter();
                        if (shotter.Visible)
                            shotter.Hide();
                        else
                            shotter.Show();
                        break;
                    case Keys.X:
                        if (shotter != null && shotter.IsDisposed) shotter = null;
                        if (shotter != null) shotter.Close();
                        break;
                }
            }
            else if (현재네임키 == 저장black)
            {
                switch (keyCode)
                {
                    case Keys.Q:
                        저장이미지(현재네임키);
                        break;
                    case Keys.F:
                        파일관리자열기(현재네임키);
                        break;
                }
            }
            else if (현재네임키 == 저장bwbw)
            {
                switch (keyCode)
                {
                    case Keys.Q:
                        저장이미지(현재네임키);
                        break;
                    case Keys.F:
                        파일관리자열기(현재네임키);
                        break;
                }
            }
        }
        void 저장텍스트(string 현재네임키)
        {
            var part = dics[현재네임키];
            var filetext = $@"{dirApi}\{part}";

            var fw = new FileStream(filetext, FileMode.Create, FileAccess.Write);
            var data = Encoding.UTF8.GetBytes(mstory.GetText());
            fw.Write(data, 0, data.Length);
            fw.Close();
        }
        void 저장이미지(string 현재네임키)
        {
            if (현재네임키 == 저장image)
            {

                return;
            }
            else if (현재네임키 == 저장shot)
            {
                var part = dics[현재네임키];
                var fileshot = $@"{dirApi}\{part}";

                사각샷(fileshot);

                return;
            }
            else if (현재네임키== 저장black)
            {
                var part = dics[현재네임키];
                var fileimage = $@"{dirApi}\{part}";

                if(imgv != null && imgv.imageBlack != null)
                {
                    imgv.imageBlack.Save(fileimage, ImageFormat.Jpeg);
                }
                return;
            }
            else if (현재네임키 == 저장bwbw)
            {
                var part = dics[현재네임키];
                var fileimage = $@"{dirApi}\{part}";
                var imageOutPath = fileimage;
                if (imgv != null && imgv.imageBlack != null)
                {
                    var bwtext = imgv.ImageToAscii();
                    var fw = new FileStream(imageOutPath, FileMode.Create, FileAccess.Write);
                    var data = Encoding.UTF8.GetBytes(bwtext);
                    fw.Write(data, 0, data.Length);
                    fw.Close();
                }
                return;
            }
        }
        void 사각샷(string fileshot)
        {
            var loc = shotter.Location;
            var size = shotter.ClientSize;

            krect rect = new krect(loc.X, loc.Y, size.Width, size.Height);
            rect.offset(5, 29);

            using (var bmp = ScreenCopy.GetImage(rect))
            {
                bmp.Save(fileshot, ImageFormat.Jpeg);
            }

            Console.WriteLine($"스샷 : {fileshot}");
        }
        void 텍스트또는이미지화일셋팅(string 현재네임키)
        {
            var part = dics[현재네임키];
            var filepath = $@"{dirApi}\{part}";

            if (File.Exists(filepath))
            {
                if (filepath.EndsWith(".txt"))
                    dics[_텍스트] = part;
                else if (filepath.EndsWith(".jpg"))
                    dics[_이미지] = part;
            }
        }
        async void 서버에화일올리기(string 현재네임키)
        {
            var partpath = dics[현재네임키];
            var filepath = $@"{dirApi}\{partpath}";

            if (File.Exists(filepath))
            {
                var ext = Path.GetExtension(filepath);
                string _MIME_ = ext == ".txt" ? "text/plain" : "image/jpeg";
                string displayName = DateTime.Now.ToString("MMddHHmmss") + ext;

                var result = await kps.UploadFile2Server(filepath, displayName, _MIME_);
                Console.WriteLine(result);

                kps.parseFileIdsAdd(result);
            }
        }
        void ShowTBoard(string 현재네임키)
        {
            var partpath = dics[현재네임키];
            var filepath = $@"{dirApi}\{partpath}";

            if (File.Exists(filepath) && filepath.EndsWith(".txt"))
            {
                if (tboard != null && tboard.IsDisposed)
                    tboard = null;
                if (tboard == null)
                {
                    tboard = new TextBoard(this);
                    Thread.Sleep(500);
                }

                //ToKen = CalcToken(textfull);

                tboard.ShowFile(filepath);
            }
        }
        void 파일내용보기(string 현재네임키)
        {
            var partpath = dics[현재네임키];
            var filepath = $@"{dirApi}\{partpath}";

            if (!File.Exists(filepath))
                return;

            if (filepath.EndsWith(".txt"))
            {
                if (mstory != null && mstory.IsDisposed)
                    mstory = null;
                if (mstory == null)
                {
                    mstory = new MementoStory(this);
                    Thread.Sleep(500);
                }

                dics[저장text] = filepath.Substring(1 + dirApi.Length);
                var textfull = File.ReadAllText(filepath, Encoding.UTF8);
                ToKen = CalcToken(textfull);

                Thread.Sleep(500);
                var title = MakeTitleMStory();
                mstory.ShowText(title, textfull);

            }
            else if (filepath.EndsWith(".jpg") || filepath.EndsWith(".png"))
            {
                if (imgv != null && imgv.IsDisposed)
                    imgv = null;
                if (imgv == null)
                {
                    imgv = new ImgViewer(this);
                    Thread.Sleep(500);
                }

                imgv.ShowImage(filepath);
            }
        }
        void 파일관리자열기(string 현재네임키)
        {
            if (fm != null && fm.IsDisposed)
                fm = null;
            if (fm == null)
                fm = new FileMan(this);

            var loca = fm.Location;
            loca.X = this.Location.X + this.ClientSize.Width + 20;
            fm.Location = loca;

            var pathPartial = dics[현재네임키];

            var filepath = $@"{dirApi}\{pathPartial}";
            var dirStart = Path.GetDirectoryName(filepath);

            if (!Directory.Exists(dirStart))
            {
                var ext = Path.GetExtension(filepath);
                dirStart = ext == ".txt" ? $@"{dirApi}\texts" : $@"{dirApi}\images";
            }

            fm.ShowX(현재네임키, dirStart);
        }
        void 파일관리자열기(string 현재네임키, string dirStart)
        {
            if (fm != null && fm.IsDisposed)
                fm = null;
            if (fm == null)
                fm = new FileMan(this);

            var loca = fm.Location;
            loca.X = this.Location.X + this.ClientSize.Width + 20;
            fm.Location = loca;

            fm.ShowX(현재네임키, dirStart);
        }
        public void 화일관리자로부터(string nameKeySel, string pathNew, string command)
        {
            if(nameKeySel == null || pathNew == null)
                return;

            if (nameKeySel.Contains("텍스트") || nameKeySel.Contains("이미지") || nameKeySel.Contains("저장"))
            {
                var len = dirApi.Length + 1;
                var partialPath = pathNew;

                if (partialPath.Length > len)
                    partialPath = partialPath.Substring(len);

                dics[nameKeySel] = $"{partialPath}";

                this.Invalidate();
                this.Show();
                this.Focus();
            }
            else if (nameKeySel == 폴더2서버)
            {
                var len = dirApi.Length + 1;
                var partialPath = pathNew;

                if (partialPath.Length > len)
                    partialPath = partialPath.Substring(len);

                dics[nameKeySel] = $"{partialPath}";

                this.Invalidate();
                this.Show();
                this.Focus();
            }
            else if (nameKeySel == Using캐시)
            {
                var filename = Path.GetFileName(pathNew);
                dics[nameKeySel] = filename;

                this.Invalidate();
                this.Show();
                this.Focus();
            }
            else if (nameKeySel == UsingBW)
            {
                var filename = Path.GetFileName(pathNew);
                dics[nameKeySel] = filename;

                this.Invalidate();
                this.Show();
                this.Focus();
            }
            else if (nameKeySel == Build캐시)
            {
                var filename = Path.GetFileName(pathNew);
                dics[nameKeySel] = filename;

                this.Invalidate();
                this.Show();
                this.Focus();
            }
        }
        public string MakeTitleMStory()
        {
            var modelName = dics[_제미나이];
            var NameNude = Path.GetFileNameWithoutExtension(dics[저장text]);

            var reffile = "";

            if (!dics[_텍스트].StartsWith("◇"))
                reffile = "텍스트";

            if (!dics[_이미지].StartsWith("◇"))
            {
                if (reffile == "텍스트")
                    reffile = "텍스트/이미지";
                else
                    reffile = "이미지";
            }

            return $"[{reffile}] ★ {modelName} ★ {NameNude} - {ToKen}";
        }
        void 환경저장()
        {
            /* 사전 설정값 저장 */
            var text = "";

            foreach (var tem in dics)
            {
                var val = tem.Value.Trim();
                if (val.Length == 0)
                    val = "◇";

                text += $"{tem.Key} : {val}\n";
            }

            var filedics = $@"{dirApi}\사전멤버들.txt";
            var fw = new FileStream(filedics, FileMode.Create, FileAccess.Write);
            var data = Encoding.UTF8.GetBytes(text);
            fw.Write(data, 0, data.Length);
            fw.Close();
        }
        void 환경불러오기()
        {
            /* 멤버 읽기 */
            var filedics = $@"{dirApi}\사전멤버들.txt";
            var lines = File.ReadAllLines(filedics, Encoding.UTF8);
            dics.Clear();

            foreach (var linex in lines)
            {
                var line = linex.Trim();

                if (line.Length == 0)
                    continue;

                var ss = Regex.Split(line, " : ");
                var key = ss[0].Trim();
                var val = ss[1].Trim();

                if (key.StartsWith("In서버"))
                    /* 시작후 채운다. */
                    dics[key] = "◇";
                else
                    dics[key] = val;
            }
        }
        void 환경변수들위치정하기()
        {
            rects.Clear();

            if (dics.Count == 0) return;
            Size size = this.ClientSize;

            float H = 28;
            krect rect0 = new krect(0, 0, size.Width / 3, H);

            var n = -1;
            var x = 0;

            foreach (var k in dics.Keys)
            {
                n++;
                var line = $"{k} : {dics[k]}";

                krect rect = rect0.copy();

                if (n == 21)
                {
                    x += rect0.W;
                    n = 0;
                }

                rect.offset(x, n * H);
                rect.inflate(-5, -2);

                rects[k] = rect;
            }
        }
        void 메세지창띄우기(string msg, int timeSleep)
        {
            if (kshow != null && kshow.IsDisposed)
                kshow = null;
            if (kshow == null)
                kshow = new KShow();

            if (OnShowMessage)
                return;

            kshow.Show();

            new Thread(() =>
            {
                OnShowMessage = true;

                kshow.ShowMessage(msg);
                Thread.Sleep(timeSleep);

                kshow.HideX();
                OnShowMessage = false;

                this.Invoke(new Action(() =>
                {
                    this.Focus();
                    this.Activate();
                }));

            }).Start();
        }
        public async Task<string> 인공지능과의대화(string text4Send)
        {
            var _model = dics[_제미나이];
            var _parttext = dics[_텍스트];
            var _partimage = dics[_이미지];

            List<string> files2send = new List<string>();

            Console.WriteLine("================================================ 참조화일 준비");
            보낼화일추가(ref files2send, $@"{dirApi}\{_parttext}");
            보낼화일추가(ref files2send, $@"{dirApi}\{_partimage}");

            string result = "";

            if (dics[_BW] == 사용)
            {
                var filetextbw = "";
                var fileimagebw = "";
                if (!흑백화일들얻기(ref filetextbw, ref fileimagebw))
                {
                    메세지창띄우기("BW 화일 준비 실패.", 2000);
                    return "BW 화일 준비 실패.";
                }
                else
                {
                    보낼화일추가(ref files2send, filetextbw);
                    보낼화일추가(ref files2send, fileimagebw);
                    result = await kps.SendDialog(text4Send, files2send, null, null, _model);
                }
            }
            else if (dics[_캐시] == 사용)
            {
                string cashId = 캐시아이디();
                if(cashId == null)
                {
                    메세지창띄우기("캐시아이디 가 없습니다.", 2000);
                    return "캐시아이디 가 없습니다.";
                }

                if (files2send.Count == 0) files2send = null;
                result = await kps.SendDialog(text4Send, files2send, null, cashId, _model);
            }
            else if (dics[_서버] == 사용)
            {
                //List<string> idsInServer = getIdsInServer();
                //result = await kps.SendDialog(text4Send, filetext, fileimage, idsInServer, null, _model);
            }
            else
            {
                if (files2send.Count == 0) files2send = null;
                result = await kps.SendDialog(text4Send, files2send, null, null, _model);
            }
            
            //await Task.Delay(1000);
            //return "테스트";
            return result;
        }
        void 보낼화일추가(ref List<string> files2send, string file2add)
        {
            if (file2add.EndsWith(다이아))
                return;

            if (File.Exists(file2add))
            {
                files2send.Add(file2add);
                Console.WriteLine($"{files2send.Count} : {file2add}");
            }
            else
            {
                Console.WriteLine($"존재하지않는화일 : {file2add}");
            }
        }
        bool 흑백화일들얻기(ref string filetext, ref string fileimage)
        {
            if (imgv == null)
                return false;

            return imgv.GetFileBW(ref filetext, ref fileimage);
        }
        async void 캐시리스트확인()
        {
            var result = await kps.GetCashesList();
            Console.WriteLine(result);

            cashIds.Clear();
            cashIds = kps.parseCashIds(result);

            foreach (var cid in cashIds)
            {
                Console.WriteLine($"{cid.Key} :\n   {cid.Value}");
            }

            if (fm != null && fm.IsDisposed)
                fm = null;
            if (fm == null)
                fm = new FileMan(this);

            fm.FeedCashIds(cashIds);
        }
        async void 서버화일들삭제()
        {
            var count = 0;
            foreach (var fid in fileIds)
            {
                Console.WriteLine($"* [ {++count} / {fileIds.Count} ] : {fid.Key} : {fid.Value} : 삭제.");
                var res = await kps.DeleteServerFile(fid.Value);
                Console.WriteLine(res);
            }

            Console.WriteLine($"---------------  삭제완료 : {fileIds.Count} 화일 ---------------------");
        }
        async void 서버에화일보내기(string 현재네임키)
        {
            var part = dics[현재네임키];
            var dir4send = $@"{dirApi}\{part}";

            var files = KFile.GetFiles(dir4send);
            var count = 0;
            foreach (var file in files)
            {
                var filepath = $@"{dir4send}\{file}";
                var mime = file.EndsWith(".txt") ? "text/plain" : "image/jpeg";

                Console.WriteLine($"* [ {++count} / {files.Count} ] : {file} : 전송2서버");
                var res = await kps.UploadFile2Server(filepath, file, mime);
                Console.WriteLine(res);
            }

            Console.WriteLine($"---------------  전송완료 : {files.Count} 화일 ---------------------");
        }
        async void 캐시만들기()
        {
            var filename= dics[Build캐시];
            var cashname= Path.GetFileNameWithoutExtension(filename);
            var filepath = $@"{dirApi}\build\{filename}";

            List<string> idsInServer = await 서버아이디확인(filepath);
            
            if( idsInServer == null)
            {
                //Console.WriteLine("화일 아이디가 없습니다.");
                return;
            }

            var _model = dics[_제미나이];
            var displayName = $"{cashname}_" + DateTime.Now.ToString("MMddHHmm");
            var result = await kps.CreateContextCache(idsInServer, _model, displayName);
            //var result = await kps.CreateContextCache(idsInServer[0], _model);
            Console.WriteLine(result);
        }
        async void 캐시삭제()
        {
            var a = "cachedContents/yz99a2yetula3i20td00p49mll0c6eetxhsfr2gw";
            //var b = "cachedContents/30tfxihf276kd79ogdcnj90srofyk9z1hm1w3egl";

            List<string> cashes = new List<string>() { a};

            foreach (var c in cashes)
            {
                Console.WriteLine($"캐시 삭제 : {c}");
                var result = await kps.DeleteContextCache(c);
                Console.WriteLine(result);
            }
        }
        async Task<List<string>> 서버아이디확인(string filepath)
        {
            var result = await kps.GetFilesListInServer();
            fileIds.Clear();
            fileIds = kps.parseFileIds(result);
            if(fileIds.Count == 0)
            {
                Console.WriteLine("서버 화일 목록이 없습니다.");
                return null;
            }

            List<string> idsInServer = new List<string>();
            var lines = File.ReadAllLines(filepath, Encoding.UTF8);
            foreach (var linex in lines)
            {
                var filename = linex.Trim();
                if (filename.Length == 0)
                    continue;

                if (fileIds.ContainsKey(filename))
                {
                    idsInServer.Add(fileIds[filename]);
                    Console.WriteLine($"{filename} : {fileIds[filename]}");
                }
                else
                {
                    Console.WriteLine($"{filename} : 서버에 없음");
                }
            }

            return idsInServer.Count == 0? null :idsInServer;
        }
        string 캐시아이디()
        {
            var filename = dics[Using캐시];
            var model = dics[_제미나이];

            if (!filename.Contains(model))
                return null;

            var filepath = $@"{dirApi}\cashIds\{filename}";
            if (File.Exists(filepath))
            {
                var lines = File.ReadAllLines(filepath, Encoding.UTF8);
                Console.WriteLine($"캐시 아이디 추출 : {lines[0]}");

                return lines[0].Trim();
            }

            return null;
        }
        void checkSameNameImage(string imageOutPath)
        {
            if (File.Exists(imageOutPath))
            {
                var name = Path.GetFileNameWithoutExtension(imageOutPath);
                var ext = Path.GetExtension(imageOutPath);
                var dir = Path.GetDirectoryName(imageOutPath);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                var imageNew = $@"{dir}\{name}_{timestamp}{ext}";
                File.Move(imageOutPath, imageNew);
            }
        }
        byte[] ResizeImageToBytes(string imagePath, int targetWidth)
        {
            using (System.Drawing.Image source = System.Drawing.Image.FromFile(imagePath))
            {
                // 1. 비율에 맞춘 높이 계산
                int targetHeight = (source.Height * targetWidth) / source.Width;

                // 2. 24비트 포맷의 새 비트맵 생성
                using (Bitmap newBmp = new Bitmap(targetWidth, targetHeight, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(newBmp))
                    {
                        // 이미지를 깔끔하게 그리기 위한 기본 설정
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(source, 0, 0, targetWidth, targetHeight);
                    }

                    // 3. 메모리 스트림에 JPEG 포맷으로 저장 후 바이트 리턴
                    using (MemoryStream ms = new MemoryStream())
                    {
                        newBmp.Save(ms, ImageFormat.Jpeg);
                        return ms.ToArray();
                    }
                }
            }
        }
        void ResizeImage(string imageSrc, string imageDst, int targetWidth)
        {
            using (System.Drawing.Image source = System.Drawing.Image.FromFile(imageSrc))
            {
                // 1. 비율에 맞춘 높이 계산
                int targetHeight = (source.Height * targetWidth) / source.Width;

                // 2. 24비트 포맷의 새 비트맵 생성
                using (Bitmap newBmp = new Bitmap(targetWidth, targetHeight, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(newBmp))
                    {
                        // 이미지를 깔끔하게 그리기 위한 기본 설정
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(source, 0, 0, targetWidth, targetHeight);
                    }

                    newBmp.Save(imageDst, ImageFormat.Jpeg);
                }
            }
        }
        int CalcToken(string text)
        {
            // 1. GPT-4 / GPT-4o가 사용하는 인코딩 방식 선택

            // 2. 분석할 한글 텍스트

            // 3. 토큰화 (텍스트 -> ID 리스트)

            if (gptEnco == null)
                gptEnco = GptEncoding.GetEncoding("cl100k_base");

            var tokens = gptEnco.Encode(text);

            // 4. 결과 출력
            //Console.WriteLine($"입력 텍스트: {text}");
            Console.WriteLine($"문자, 토큰 == {text.Length}, {tokens.Count}");

            return tokens.Count;
        }
        internal void 인공지능답변저장(string result)
        {
            var part = dics[저장log];
            var filepath = $@"{dirApi}\{part}";

            var fw = new FileStream(filepath, FileMode.Create, FileAccess.Write);
            var data = Encoding.UTF8.GetBytes(result);
            fw.Write(data, 0, data.Length);
            fw.Close();
        }
        void 인공지능답변보기()
        {
            var part = dics[저장log];
            var filepath = $@"{dirApi}\{part}";
            var text = File.ReadAllText(filepath, Encoding.UTF8);

            Console.WriteLine();
            Console.WriteLine(text);
            Console.WriteLine();
        }
    }
}

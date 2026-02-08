using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using shine.libs.math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MidiFile = Melanchall.DryWetMidi.Core.MidiFile;

namespace KPlayer
{
    public partial class KPlayer : Form
    {
        const string 저장 = "저장";
        const string 열기 = "열기";
        const string 업로드 = "업로드";
        const string 화일아이디 = "화일아이디";
        const string 캐싱 = "캐싱";
        const string 삭제 = "삭제";
        const string 대화기록 = "대화기록";
        const string 대화모드 = "대화모드";
        const string 참조 = "참조";
        const string 안참조 = "안참조";
        const string 캐시참조 = "캐시참조";

        const string 미디변경 = "미디변경";
        const string 화일Add = "화일Add";
        const string 저장노트 = "저장노트";
        const string 저장멜로디 = "저장멜로디";
        const string 구간연주 = "구간연주";
        const string 갱신네임즈 = "갱신네임즈";
        const string 로드미디 = "로드미디";
        const string 멜로디수정 = "멜로디수정";
        const string 멜로디모드 = "멜로디모드";
        const string 선택토글 = "선택토글";
        const string 선택클리어 = "선택클리어";
        const string 선택시작 = "선택시작";
        const string 선택끝 = "선택끝";
        const string 테스트 = "테스트";

        const string 폴더미디 = "폴더미디";
        const string 폴더미디k = "폴더미디k";
        const string 화일미디 = "화일미디";
        const string 화일출력 = "화일출력";
        const string 화일4Add = "화일4Add";
        const string 화일Add출력 = "화일Add출력";
        const string 화일플레이 = "화일플레이";
        const string 화일4저장 = "화일4저장";
        const string 화일4저장m = "화일4저장m";
        const string 길이scale = "길이scale";


        string dirCurr = @"d:\Works\Muse";
        string nameCurr = 대화기록;
        //string fileId= "3hwc1udgv1sx";
        string fileId = "2ymfzqeqot9x";

        bool OnSending = false;
        bool OnShowMessage = false;
        bool OnShowingWaitingForm = false;

        KShow kshow;
        HotPlace kplace;

        MData md;

        Dictionary<string, MData> mds = new Dictionary<string, MData>();

        MPlayer mplay;
        kvec pos = kvec.Zero;
        bool CTRL, SHIFT, ALT;

        FileMan fm;

        Dictionary<string, string> Names = new Dictionary<string, string>();
        string DialMode;
        public KPlayer()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 배경 지우기 무시 (WM_ERASEBKGND 차단)
            this.SetStyle(ControlStyles.UserPaint, true);           // OS가 아닌 사용자가 직접 그림
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true); // 메모리 버퍼링 사용
            this.SetStyle(ControlStyles.ResizeRedraw, true);        // 창 크기 조절 시 다시 그림
            this.UpdateStyles();

            this.DoubleBuffered = true;
            //this.FormBorderStyle = FormBorderStyle.None;

            this.Paint += KPlayer_Paint;
            this.KeyDown += KPlayer_KeyDown;
            this.KeyUp += KPlayer_KeyUp;
            this.Load += KPlayer_Load;

            this.MouseDown += KPlayer_MouseDown;
            this.MouseUp += KPlayer_MouseUp;
            this.MouseMove += KPlayer_MouseMove;
            this.MouseHover += KPlayer_MouseHover;

            CTRL = SHIFT = ALT = false;
        }
        void KPlayer_MouseHover(object sender, EventArgs e)
        {
        }
        void KPlayer_MouseMove(object sender, MouseEventArgs e)
        {
            pos = new kvec(e.X, e.Y);
        }
        void KPlayer_MouseUp(object sender, MouseEventArgs e)
        {
            if (mplay.OnPlay) return;

        }
        void KPlayer_MouseDown(object sender, MouseEventArgs e)
        {
            if (mplay.OnPlay) return;

        }
        void KPlayer_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;

            this.ClientSize = new Size(1800, 800);
            this.Location = new Point(50, 100);

            mplay = new MPlayer(this);

            reloadNames();
            loadMidi();

            kplace = new HotPlace(this);
            kplace.Owner = this;
            kplace.Show();
            kplace.Command(저장, nameCurr);

            fm = new FileMan(this);
        }
        void loadMidi()
        {
            var scale = float.Parse(Names[길이scale]);
            var filemidi = getFilePath(폴더미디k, 화일플레이);
            md = new MData(filemidi, scale);

            mds[Names[화일플레이]] = md;

            this.Text = (md.Melody ? "[멜] " : "") + Names[화일플레이];
        }
        void reloadNames()
        {
            Names.Clear();

            Names[폴더미디] = @"D:\Works\Muse\Data\midis";
            Names[폴더미디k] = @"D:\Works\Muse\Data\kidis";
            Names[화일미디] = "미래가있다Mel.mid";
            Names[화일출력] = "미래가있다Mel.txt";
            Names[화일4Add] = "내멜로디 4 add 2.txt";
            Names[화일Add출력] = "내멜로디 addout 1.txt";

            Names[화일플레이] = "Kalinka.txt";
            Names[화일플레이] = "세명의전차병6.txt";
            Names[화일플레이] = "Korobeiniki멜.txt";
            Names[화일플레이] = "슬라브여인M.txt";
            Names[화일플레이] = "내멜로디addout1.txt";
            Names[화일플레이] = "미래가있다Mel.txt";
            Names[화일플레이] = "슬라브여인2.txt";
            Names[화일플레이] = "Korobeiniki.txt";

            Names[화일4저장] = "저장.txt";
            Names[화일4저장m] = "멜로디.txt";

            Names[길이scale] = "1.0";
        }
        void KPlayer_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(Color.Black);

            mplay.Paint(g, md);
        }
        internal void player_drawNotes()
        {
            this.BeginInvoke(new Action(() =>
            {
                this.Invalidate();
            }));
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
        void KPlayer_KeyDown(object sender, KeyEventArgs e)
        {
            var keyCode = e.KeyCode;
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
        }
        void KPlayer_KeyUp(object sender, KeyEventArgs e)
        {
            var keyCode = e.KeyCode;
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

            if (mplay.OnPlay)
            {
                switch (keyCode)
                {
                    case Keys.Escape:
                    case Keys.Space:
                        mplay.OnPlay = false;
                        mplay.OnRepeat = false;
                        return;
                }
            }
            else
            {
                switch (keyCode)
                {
                    case Keys.Escape:
                        if (SHIFT)
                        {
                            Application.Exit();
                            SHIFT = false;
                        }
                        return;
                    case Keys.Space:
                        if (!mplay.OnRepeat)
                        {
                            mplay.OnPlay = true;
                            mplay.Play(md, false);
                        }
                        return;
                }
            }

            if (mplay.OnPlay || mplay.OnRepeat)
                return;

            switch (keyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    mplay.MoveTime(CTRL, keyCode, md);
                    this.Invalidate();
                    return;
            }

            string command = null;

            if (CTRL)
            {
                switch (keyCode)
                {
                    case Keys.Q: command = 미디변경; break;
                    case Keys.S: command = 저장노트; break;
                    case Keys.U: command = 화일Add; break;
                    case Keys.M: command = 저장멜로디; break;
                    case Keys.K: command = 갱신네임즈; break;
                    case Keys.L: command = 로드미디; break;
                    case Keys.P: command = 구간연주; break;
                    case Keys.Z: command = 테스트; break;
                }

                CTRL = false;
            }
            else
            {
                switch (keyCode)
                {
                    case Keys.M: command = 멜로디모드; break;
                    case Keys.P: command = 멜로디수정; break;
                    case Keys.X: command = 선택토글; break;
                    case Keys.C: command = 선택클리어; break;
                    case Keys.OemOpenBrackets: command = 선택시작; break;
                    case Keys.OemCloseBrackets: command = 선택끝; break;
                }
            }

            if (command != null)
                showMessage(command, 1000);

            switch (command)
            {
                case 미디변경:
                case 화일Add:
                case 저장노트:
                case 저장멜로디:
                    SendCommand(command);
                    break;
            }

            switch (command)
            {

                case 구간연주:
                    mplay.OnPlay = true;
                    mplay.OnRepeat = true;
                    mplay.Play(md, true);
                    break;
                case 갱신네임즈:
                    reloadNames();
                    this.Invalidate();
                    break;
                case 로드미디:
                    loadMidi();
                    this.Invalidate();
                    break;
                case 멜로디수정:
                    mplay.ModifyMelos(md);
                    this.Invalidate();
                    break;
                case 멜로디모드:
                    md.Melody = !md.Melody;
                    if (md.Melody)
                        md.BulidMelEnvents();

                    this.Text = (md.Melody ? "[멜] " : "") + Names[화일플레이];
                    this.Invalidate();
                    break;
                case 선택토글:
                    mplay.SelectNote(pos, md);
                    this.Invalidate();
                    break;
                case 선택클리어:
                    mplay.ClearSels(md);
                    this.Invalidate();
                    break;
                case 선택시작:
                    mplay.SetNoteStart(pos, md);
                    this.Invalidate();
                    break;
                case 선택끝:
                    mplay.SetNoteEnd(pos, md);
                    this.Invalidate();
                    break;
                case 테스트:
                    //mplay.SetNoteEnd(pos, md);
                    this.Invalidate();
                    break;
            }
        }
        string getFilePath(string folder, string file)
        {
            return Names[folder] + "\\" + Names[file];
        }
        internal void player_drawSelNotes()
        {
            this.Invalidate();
        }
        void ConvertM2T(string filemidi, string fileout)
        {
            var midiFile = MidiFile.Read(filemidi);

            // 파일 내의 모든 노트 추출
            var notes = midiFile.GetNotes();
            var text = "";

            foreach (var note in notes)
            {
                var pitch = KNote.GetPitch(note.Octave, note.NoteName);
                Console.WriteLine($"{note.Time} : 音 {pitch}, 길이 {note.Length}, Vel {note.Velocity}");

                var line = $"{note.Time} : 音 {pitch} : 길이 {note.Length} : Vel {note.Velocity} : 0";
                text += line + "\n";
            }

            FileStream fw = new FileStream(fileout, FileMode.Create, FileAccess.Write);
            var data = Encoding.UTF8.GetBytes(text);

            fw.Write(data, 0, data.Length);
            fw.Close();
        }
        private void AddKMidi(string fileadd, string fileout)
        {
            var lines = File.ReadAllLines(fileadd, Encoding.UTF8);
            List<string> alines = new List<string>();

            float scale = 1.0f;
            int pitchAdd = 100;
            int vel = 100;
            bool firstNote = false;
            long TimeAddStart = 0;
            long TimeOffMax = 0;
            long TimeLastEnd = 0;

            foreach (string linex in lines)
            {
                var line = linex.Trim();
                if (line.Length == 0) continue;

                string[] ss, nn;

                if (line.StartsWith("#"))
                {
                    ss = Regex.Split(line, " : ");

                    if (ss[0] == "#add")
                    {
                        scale = float.Parse(ss[1]);
                        pitchAdd = int.Parse(ss[2]);
                        vel = int.Parse(ss[3]);

                        firstNote = true;
                    }
                }
                else
                {
                    ss = Regex.Split(line, " : ");
                    if (ss.Length >= 4)
                    {
                        var time = long.Parse(ss[0]); time = (long)(time * scale);
                        nn = Regex.Split(ss[1], " ");
                        var pitch = int.Parse(nn[1]); pitch = pitch + pitchAdd;
                        nn = Regex.Split(ss[2], " ");
                        var dura = long.Parse(nn[1]); dura = (long)(dura * scale);
                        nn = Regex.Split(ss[3], " ");
                        var velx = int.Parse(nn[1]);

                        if (firstNote)
                        {
                            firstNote = false;
                            TimeAddStart = time;
                            TimeLastEnd = TimeOffMax;
                        }

                        time = TimeLastEnd + (time - TimeAddStart);

                        var aline = $"{time} : 音 {pitch} : 길이 {dura} : Vel {vel} : 1";
                        alines.Add(aline);

                        if (TimeOffMax < time + dura)
                            TimeOffMax = time + dura;
                    }
                }
            }

            /* alines -> textfull */
            var text = "";
            foreach (var aline in alines)
                text += aline + "\n";

            FileStream fw = new FileStream(fileout, FileMode.Create, FileAccess.Write);
            var data = Encoding.UTF8.GetBytes(text);

            fw.Write(data, 0, data.Length);
            fw.Close();
        }
        void SaveNotes(string fileout, MData md)
        {
            var text = "";

            foreach (var nx in md.kNotesX)
            {
                Console.WriteLine($"time:{nx.Key},  음갯수:{nx.Value.Count}");

                var Time = nx.Key;

                foreach (var knote in nx.Value)
                {
                    Console.WriteLine($"{Time} : 音 {knote.Pitch}, 길이 {knote.Duration}, Vel {knote.Vel}, {knote.isMel}");

                    var isMel = knote.isMel ? 1 : 0;
                    var line = $"{Time} : 音 {knote.Pitch} : 길이 {knote.Duration} : Vel {knote.Vel} : {isMel}";

                    text += line + "\n";
                }
            }

            FileStream fw = new FileStream(fileout, FileMode.Create, FileAccess.Write);

            var data = Encoding.UTF8.GetBytes(text);

            fw.Write(data, 0, data.Length);
            fw.Close();
        }
        void SaveMelody(string fileout, MData md)
        {
            var text = "";

            foreach (var nx in md.kNotesX)
            {
                var Time = nx.Key;

                foreach (var knote in nx.Value)
                {
                    if (knote.isMel)
                    {
                        var line = $"{Time} : 音 {knote.Pitch} : 길이 {knote.Duration} : Vel {knote.Vel} : 1";
                        text += line + "\n";
                    }
                }
            }

            FileStream fw = new FileStream(fileout, FileMode.Create, FileAccess.Write);

            var data = Encoding.UTF8.GetBytes(text);

            fw.Write(data, 0, data.Length);
            fw.Close();
        }
        void showMessage(string msg, int time) 
        {
            switch (msg)
            {
                case "미디변경":
                case "저장노트":
                case "저장멜로디":
                case "구간연주":
                case "갱신네임즈":
                case "로드미디":
                case "멜로디수정":
                case "멜로디모드":
                case "화일Add":
                    //아래 진행
                    break;
                case "선택클리어":
                case "선택시작":
                case "선택끝":
                case "선택토글":
                default:
                    //진행 안함
                    return;
            }

            new Thread(() =>
            {
              

            }).Start();
        }
        void SendCommand(string command)
        {
            if (kplace != null && kplace.IsDisposed)
                kplace = null;

            if (kplace == null)
                kplace = new HotPlace(this);

            var text = "";

            switch (command)
            {
                case 열기:
                case 저장:
                    text = nameCurr;
                    break;
                default:
                    text = "";
                    break;
            }

            kplace.Command(command, text);

            kplace.Show();
            kplace.Focus();
        }
        public void kplace_doSomething(string command, string text)
        {
            Console.WriteLine($"{command} {text}");
            return;

            var fileout = "";
            switch (command)
            {
                case 미디변경:
                    var filemidi = getFilePath(폴더미디, 화일미디);
                    fileout = getFilePath(폴더미디k, 화일출력);
                    ConvertM2T(filemidi, fileout);
                    SendCommand(command);
                    break;
                case 화일Add:
                    var fileadd = getFilePath(폴더미디k, 화일4Add);
                    fileout = getFilePath(폴더미디k, 화일Add출력);
                    AddKMidi(fileadd, fileout);
                    break;
                case 저장노트:
                    fileout = getFilePath(폴더미디k, 화일4저장);
                    SaveNotes(fileout, md);
                    break;
                case 저장멜로디:
                    var filemel = getFilePath(폴더미디k, 화일4저장m);
                    SaveMelody(filemel, md);
                    break;
  
            }
        }
        public void hotPlace_doSomething(string command, string text)
        {
            if (text != null)
                text = text.Trim();

            if (text != null && text != "")
            {

            }
            else return;

            if (command == 열기)
            {
                showMessage($"{열기} : {text}", 1000);

                var fileopen = $@"{dirCurr}\{text}.txt";

                if (!File.Exists(fileopen))
                {
                    showMessage($"파일없다 : {fileopen}", 1000);
                    return;
                }

                var textfull = File.ReadAllText(fileopen, Encoding.UTF8);

           
                nameCurr = text;
                if (DialMode == 참조)
                    this.Text = $"[참조] {nameCurr}";
                else
                    this.Text = $"{nameCurr}";
            }
            else if (command == 저장)
            {
                showMessage($"{저장} : {text}", 1000);

                var filesave = $@"{dirCurr}\{text}.txt";
                var fw = new FileStream(filesave, FileMode.Create, FileAccess.Write);
                var data = Encoding.UTF8.GetBytes("xxxxx");

                fw.Write(data, 0, data.Length);
                fw.Close();

                nameCurr = text;
                if (DialMode == 참조)
                    this.Text = $"[참조] {nameCurr}";
                else
                    this.Text = $"{nameCurr}";
            }
           
            else if (command == 삭제)
            {
                showMessage($"{삭제} : {text}", 1000);

                var fileId = text;
               

            }
            else if (command == 대화모드)
            {
                showMessage($"{대화모드} : {text}", 1000);

                var modeNew = text == 참조 ? 참조 : 안참조;
                DialMode = DialMode != modeNew ? modeNew : DialMode;

                if (DialMode == 참조)
                    this.Text = $"[참조] {nameCurr}";
                else
                    this.Text = $"{nameCurr}";
            }
        }
    }
}


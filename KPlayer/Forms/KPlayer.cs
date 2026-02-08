using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.MusicTheory;
using shine.libs.math;
using System;
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
        const string 명령 = "명령";
        const string 구간시작 = "구간시작";
        const string 구간끝 = "구간끝";
        const string 저장 = "저장";
        const string 열기 = "열기";
        const string 삭제 = "삭제";

        const string 미디변환 = "미디변환";
        const string 화일Add = "화일Add";
        const string 저장노트 = "저장노트";
        const string 저장멜로디 = "저장멜로디";

        const string 폴더미디m = "폴더미디m";
        const string 폴더미디k = "폴더미디k";
        const string 변환대상 = "변환대상";
        const string 변환출력 = "변환출력";
        const string 화일4Add = "화일4Add";
        const string 화일Add출력 = "화일Add출력";
        const string 화일4저장 = "화일4저장";
        const string 화일4저장m = "화일4저장m";

        const string _미디now = "_미디now";
        const string 미디file0 = "미디file0";
        const string 미디file1 = "미디file1";
        const string 미디file2 = "미디file2";
        const string 미디file3 = "미디file3";
        const string 미디file4 = "미디file4";
        const string 미디file5 = "미디file5";
        const string 미디file6 = "미디file6";
        const string 미디file7 = "미디file7";
        const string 미디file8 = "미디file8";
        const string 미디file9 = "미디file9";
        const string 미디play0 = "미디play0";
        const string 미디play1 = "미디play1";
        const string 미디play2 = "미디play2";
        const string 미디play3 = "미디play3";
        const string 미디play4 = "미디play4";
        const string 미디play5 = "미디play5";
        const string 미디play6 = "미디play6";
        const string 미디play7 = "미디play7";
        const string 미디play8 = "미디play8";
        const string 미디play9 = "미디play9";

        Dictionary<string, string> dics = new Dictionary<string, string>();
        Dictionary<string, krect> rects = new Dictionary<string, krect>();
        Dictionary<string, MData> mds = new Dictionary<string, MData>();
        
        public MData md
        {
            get
            {
                var name = dics[_미디now];

                if (mds.ContainsKey(name))
                    return mds[name];
                else
                    return null;
            }
        }
        Player player;
        FileMan fm;
        KShow kshow;
        string dirMuse = @"d:\Works\Muse";
        string nameKeySelected = "";

        bool CTRL, SHIFT;
        bool OnShowMessage = false;

        public KPlayer()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            this.Load += KPlayer_Load;
            this.Paint += KPlayer_Paint;

            this.KeyDown += KPlayer_KeyDown;
            this.KeyUp += KPlayer_KeyUp;

            this.MouseDown += KPlayer_MouseDown;
            this.MouseUp += KPlayer_MouseUp;
            this.MouseMove += KPlayer_MouseMove;

            CTRL = SHIFT = false;
        }
        void KPlayer_Load(object sender, EventArgs e)
        {
            Size size = this.ClientSize;

            ReloadDics();
            SetRectsOfDics();
            
            var m_s_f = dics[_미디now];
            var ss = Regex.Split(m_s_f, " ");

            var scale = 0.1f * float.Parse(ss[1]);
            var filemidi = $@"{dics[폴더미디k]}\{ss[2]}";

            mds[m_s_f] = new MData(filemidi, scale, ss[0] == "m");
            updateDicsOfPlay(미디play0);
            this.Text = m_s_f;
        }
        void KPlayer_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (dics.Count == 0) return;
            Size size = this.ClientSize;

            float H = 28;
            float W = size.Width / 6;

            Font font = new Font("맑은 고딕", 11f);
            Font fontB = new Font("맑은 고딕", 11f,FontStyle.Bold);

            krect rect0 = new krect(0, 0, size.Width/2, H);

            var n = -1;
            var x = 0;

            foreach(var k in dics.Keys)
            {
                n++;
                krect rect = rects[k];

                g.FillRectangle(Brushes.LightGray, rect.R);

                krect rectT = rect.copy();
                rectT.offset(5, 2);

                g.DrawString($"{k}", fontB, Brushes.Black, rectT.R);
                
                rectT.offset(W, 0);

                var text = dics[k];
                switch (k)
                {
                    case 폴더미디m:
                    case 폴더미디k:
                        text = text.Replace(dirMuse,"..");
                        break;
                }

                g.DrawString($"{text}", font, Brushes.Black, rectT.R);

                if (k == nameKeySelected)
                {
                    g.DrawRectangle(Pens.Red, rect.R);
                }
            }
        }
        void KPlayer_MouseMove(object sender, MouseEventArgs e)
        {
            kvec pos = new kvec(e.X, e.Y);

            string nameKeyCurrNew = null;

            foreach (var k in rects.Keys)
            {
                if (rects[k].contains(pos))
                {
                    nameKeyCurrNew = k;
                    break;
                }
            }

            if (nameKeyCurrNew == null)
                nameKeySelected = "";
            else if(nameKeyCurrNew != nameKeySelected)
            {
                nameKeySelected = nameKeyCurrNew;
                this.Invalidate();
            }
        }
        void KPlayer_MouseUp(object sender, MouseEventArgs e)
        {
        }
        void KPlayer_MouseDown(object sender, MouseEventArgs e)
        {
        }
        void KPlayer_KeyDown(object sender, KeyEventArgs e)
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
                        SaveDics();

                        Application.Exit();
                        SHIFT = false;
                    }
                    else 
                        this.Hide();

                    e.SuppressKeyPress = true;
                    return;
            }
        }
        void KPlayer_KeyUp(object sender, KeyEventArgs e)
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

            var lenKey = nameKeySelected.Length;

            if (nameKeySelected.Contains("미디file"))
            {
                KPlayer_KeyUp_MidiFile(keyCode);
                return;
            }
            else if (nameKeySelected.Contains("미디play"))
            {
                KPlayer_KeyUp_MidiPlay(keyCode);
                return;
            }
            else
            {
                KPlayer_KeyUp_Commands(keyCode, nameKeySelected);
                return;
            }
        }
        void KPlayer_KeyUp_Commands(Keys keyCode, string nameKeySelected)
        {
            string fileout, filemidi, fileadd, filemel;

            if (keyCode == Keys.Q)
            {
                switch (nameKeySelected)
                {
                    case 변환대상:
                    case 변환출력:
                        filemidi = $@"{dics[폴더미디m]}\{dics[변환대상]}";
                        fileout = $@"{dics[폴더미디k]}\{dics[변환출력]}";
                        showMessage($"변환 : {dics[변환대상]} ->  {dics[변환출력]}", 1000);
                        ConvertM2T(filemidi, fileout);
                        break;
                    case 화일4Add:
                    case 화일Add출력:
                        fileadd = $@"{dics[폴더미디k]}\{dics[화일4Add]}";
                        fileout = $@"{dics[폴더미디k]}\{dics[화일Add출력]}";
                        showMessage($"화일4Add", 1000);
                        AddKMidi(fileadd, fileout);
                        break;
                    case 화일4저장:
                        fileout = $@"{dics[폴더미디k]}\{dics[화일4저장]}";
                        showMessage($"화일저장", 1000);
                        SaveNotes(fileout, md);
                        break;
                    case 화일4저장m:
                        filemel = $@"{dics[폴더미디k]}\{dics[화일4저장m]}";
                        showMessage($"멜로디저장", 1000);
                        SaveMelody(filemel, md);
                        break;
                }
            }
            else if (keyCode == Keys.F)
            {
                switch (nameKeySelected)
                {
                    case 변환대상:
                    case 변환출력:
                    case 화일4Add:
                    case 화일Add출력:
                    case 화일4저장:
                    case 화일4저장m:
                    ShowFileMan(nameKeySelected);

                        break;
                }
            }
        }
        void KPlayer_KeyUp_MidiPlay(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Q:
                    ShowAndPlay(nameKeySelected);
                    this.Invalidate();
                    return;
                case Keys.X:
                    //Console.WriteLine($"{nameKeySelected} nameKeySelected");
                    var mdKey = dics[nameKeySelected];
                    mds.Remove(mdKey);
                    updateDicsOfPlay(미디play0);
                    this.Invalidate();
                    return;
            }
        }
        void KPlayer_KeyUp_MidiFile(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.F:
                    ShowFileMan(nameKeySelected);
                    return;
                case Keys.Q:
                    ShowAndPlay(nameKeySelected);
                    this.Invalidate();
                    return;
                case Keys.A:
                    ModifyPlayMode(nameKeySelected);
                    this.Invalidate();
                    return;
                case Keys.W:
                    ModifyScale(nameKeySelected, +1);
                    this.Invalidate();
                    return;
                case Keys.S:
                    ModifyScale(nameKeySelected, -1);
                    this.Invalidate();
                    return;
            }
        }
        void ModifyPlayMode(string nameKeySelected)
        {
            var m_s_f = dics[nameKeySelected];
            var ss = Regex.Split(m_s_f, " ");

            var am = ss[0] == "a" ? "m" : "a";
            dics[nameKeySelected]= $"{am} {ss[1]} {ss[2]}";
        }
        void ModifyScale(string nameKeySelected, int ds)
        {
            var m_s_f = dics[nameKeySelected];
            var ss = Regex.Split(m_s_f, " ");

            int scalex = int.Parse(ss[1]);

            scalex += ds;
            scalex = Math.Max(scalex, 1);

            dics[nameKeySelected] = $"{ss[0]} {scalex:00} {ss[2]}";
        }
        void ShowAndPlay(string nameKeySelected)
        {
            if (player != null && player.IsDisposed)
                player = null;
            if (player == null)
                player = new Player(this);

            var m_s_f = dics[nameKeySelected];
            dics[_미디now] = m_s_f;

            if (!mds.ContainsKey(m_s_f))
            {
                var ss = Regex.Split(m_s_f, " ");

                var filemidi = $@"{dics[폴더미디k]}\{ss[2]}";
                if (File.Exists(filemidi))
                {
                    var scale = 0.1f * float.Parse(ss[1]);
                    mds[m_s_f] = new MData(filemidi, scale, ss[0] == "m");
                    updateDicsOfPlay(미디play0);
                }
            }

            player.Text = m_s_f;
            player.ShowX();
        }
        void updateDicsOfPlay(string namekey)
        {
            if (namekey == 미디play0)
            {
                var mdkeys = new List<string>();
                foreach(var md in mds)
                    mdkeys.Add(md.Key);

                for (int i = 0; i <= 9; i++)
                {
                    var dickey = $"미디play{i}";

                    if (i < mdkeys.Count)
                        dics[dickey] = mdkeys[i];
                    else
                        dics[dickey] = "◇";
                }
            }
        }
        void ShowFileMan(string nameKeyCurr)
        {
            if (fm != null && fm.IsDisposed)
                fm = null;
            if (fm == null)
                fm = new FileMan(this);

            var loca = fm.Location;
            loca.X = this.Location.X + this.ClientSize.Width + 20;
            fm.Location = loca;

            var dirStart = nameKeyCurr == 변환대상 ? dics[폴더미디m] : dics[폴더미디k];

            fm.ShowX(nameKeyCurr, dirStart);
        }
        public void fm_doSomething(string nameKeySel, string targetPath)
        {
            if (nameKeySel.Contains("미디file"))
            {
                var filename = Path.GetFileName(targetPath);
                dics[nameKeySel] = $"a 10 {filename}";
            }
            else if (nameKeySel.Contains("화일"))
            {
                var filename = Path.GetFileName(targetPath);
                dics[nameKeySel] = $"{filename}";
            }
            else if (nameKeySel.Contains("변환"))
            {
                var filename = Path.GetFileName(targetPath);
                dics[nameKeySel] = $"{filename}";
            }

            this.Invalidate();
            this.Show();
            this.Focus();
        }
        public void SaveDics()
        {
            var filesList = $@"{dirMuse}\Data\미디files.txt";
            var text = "";

            foreach (var tem in dics)
            {
                text += $"{tem.Key} : {tem.Value}\n";
            }

            var fw = new FileStream(filesList, FileMode.Create, FileAccess.Write);
            var data = Encoding.UTF8.GetBytes(text);
            fw.Write(data, 0, data.Length);
        }
        void ReloadDics()
        {
            dics.Clear();

            var filesList = $@"{dirMuse}\Data\미디files.txt";
            var lines = File.ReadAllLines(filesList, Encoding.UTF8);

            foreach (var linex in lines)
            {
                var line = linex.Trim();

                if (line.Length == 0)
                    continue;

                var ss = Regex.Split(line, " : ");
                var key = ss[0].Trim();
                var val = ss[1].Trim();

                if (key.Contains("미디play"))
                    dics[key] = "◇";
                else
                    dics[key] = val;
            }
        }
        void SetRectsOfDics()
        {
            rects.Clear();

            if (dics.Count == 0) return;
            Size size = this.ClientSize;

            float H = 28;
            krect rect0 = new krect(0, 0, size.Width / 2, H);

            var n = -1;
            var x = 0;

            foreach (var k in dics.Keys)
            {
                n++;
                var line = $"{k} : {dics[k]}";

                krect rect = rect0.copy();

                if (x == 0 && n * H > size.Height - H)
                {
                    x = size.Width / 2;
                    n = 0;
                }

                rect.offset(x, n * H);
                rect.inflate(-5, -2);

                rects[k] = rect;
            }
        }
        void ConvertM2T(string filemidi, string fileout)
        {
            var readingSettings = new ReadingSettings
            {
                InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore,
                NotEnoughBytesPolicy = NotEnoughBytesPolicy.Ignore,
                UnexpectedTrackChunksCountPolicy = UnexpectedTrackChunksCountPolicy.Ignore,
                InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.SnapToLimits,
                MissedEndOfTrackPolicy = MissedEndOfTrackPolicy.Ignore,
                UnknownFileFormatPolicy = UnknownFileFormatPolicy.Ignore
            };

            try
            {
                var midiFile = MidiFile.Read(filemidi, readingSettings);
                var channelInstruments = midiFile.GetTimedEvents()
                    .Select(e => e.Event)
                    .OfType<ProgramChangeEvent>()
                    .GroupBy(e => (int)e.Channel)
                    .ToDictionary(g => g.Key, g => (int)g.Last().ProgramNumber);

                var text = "";
                text+="# 채널 : 악기\n\n";
                foreach (var ch in channelInstruments.Keys.OrderBy(c => c))
                {
                    var name = MData.ToolNames[channelInstruments[ch]];
                    text+=$"{ch} : {name}\n";
                }

                text += "\n# 노트들 \n\n";

                // 2. [본문] 노트 추출 및 채널 정보 첨가
                var notes = midiFile.GetNotes();
                foreach (var note in notes)
                {
                    var pitch = KNote.GetPitch(note.Octave, note.NoteName);
                    var ch = note.Channel; // 실제 채널 번호

                    // 마지막 부분에 ": 채널 {ch}" 형식으로 첨가
                    var line = $"{note.Time} : 音 {pitch} : 길이 {note.Length} : Vel {note.Velocity} : 채널 {ch} : 0";
                    text += line + "\n";
                }

                var fw = new FileStream(fileout, FileMode.Create, FileAccess.Write);
                var data = Encoding.UTF8.GetBytes(text);
                fw.Write(data, 0, data.Length);
                fw.Close();

                Console.WriteLine($"변환 완료: {fileout}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        void AddKMidi(string fileadd, string fileout)
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
                //Console.WriteLine($"time:{nx.Key},  음갯수:{nx.Value.Count}");

                var Time = nx.Key;

                foreach (var knote in nx.Value)
                {
                    //Console.WriteLine($"{Time} : 音 {knote.Pitch}, 길이 {knote.Duration}, Vel {knote.Vel}, {knote.isMel}");

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
        void showMessage(string msg, int timeSleep)
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

                this.Invoke(new Action(() => {
                    this.Focus();
                    this.Activate();
                }));

            }).Start();
        }
    }
}

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
    public partial class Player : Form
    {
        const string 메인폼 = "메인폼";
        const string 멜로디모드 = "멜로디모드";

        const string 구간연주 = "구간연주";
        const string 멜로디수정 = "멜로디수정";
        const string 선택토글 = "선택토글";
        const string 선택클리어 = "선택클리어";
        const string 구간시작 = "구간시작";
        const string 구간끝 = "구간끝";
        
        public KShow kshow;
        public KPlayer main;

        MPlayer mplay;
        kvec pos = kvec.Zero;
        bool CTRL, SHIFT, ALT;

        public Player(KPlayer main)
        {
            this.main = main;

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
        }
        void KPlayer_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(Color.Black);

            mplay.Paint(g, main.md);
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
                            main.SaveDics();
                            Application.Exit();
                            SHIFT = false;
                        }
                        return;
                    case Keys.Space:
                        if (!mplay.OnRepeat)
                        {
                            mplay.OnPlay = true;
                            mplay.Play(main.md, false);
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
                    mplay.MoveTime(CTRL, keyCode, main.md);
                    this.Invalidate();
                    return;
            }

            string command = null;

            if (CTRL)
            {
                switch (keyCode)
                {
                    
                }

                CTRL = false;
            }
            else
            {
                switch (keyCode)
                {
                    case Keys.Q: command = 메인폼; break;
                    case Keys.P: command = 구간연주; break;
                    case Keys.L: command = 멜로디수정; break;
                    case Keys.X: command = 선택토글; break;
                    case Keys.C: command = 선택클리어; break;
                    case Keys.OemOpenBrackets: command = 구간시작; break;
                    case Keys.OemCloseBrackets: command = 구간끝; break;
                }
            }

            if (command != null)
                showMessage(command, 1000);

            switch (command)
            {
                case 메인폼:
                    main.Show();
                    main.Focus();
                    break;
                case 구간연주:
                    mplay.OnPlay = true;
                    mplay.OnRepeat = true;
                    mplay.Play(main.md, true);
                    break;
                case 멜로디수정:
                    mplay.ModifyMelos(main.md);
                    this.Invalidate();
                    break;
                case 선택토글:
                    mplay.SelectNote(pos, main.md);
                    this.Invalidate();
                    break;
                case 선택클리어:
                    mplay.ClearSels(main.md);
                    this.Invalidate();
                    break;
                case 구간시작:
                    mplay.SetNoteStart(pos, main.md);
                    this.Invalidate();
                    break;
                case 구간끝:
                    mplay.SetNoteEnd(pos, main.md);
                    this.Invalidate();
                    break;
            }
        }
        private void showMessage(string command, int time)
        {
            
        }
        internal void ShowX()
        {
            this.Invalidate();
            this.Show();
            this.Focus();
        }
    }
}


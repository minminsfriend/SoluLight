using Android.Graphics;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using shine.libs.math;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KPlayer
{
    public class MPlayer 
    {
        Dictionary<string, int[]> chords = new Dictionary<string, int[]>();
        Dictionary<long, List<krect>> pRects = new Dictionary<long, List<krect>>();
        Dictionary<long, List<krect>> mRects = new Dictionary<long, List<krect>>();
        Dictionary<long, List<krect>> sRects = new Dictionary<long, List<krect>>();

        OutputDevice outputDevice;// = OutputDevice.GetByIndex(0);

        Player main;
        internal bool OnPlay = false;
        internal bool OnRepeat = false;

        public MPlayer(Player main)
        {
            this.main = main;
            outputDevice = OutputDevice.GetByIndex(0);
        }
        internal void Paint(Graphics g, MData md)
        {
            long timeA = md.TimePlay - 5300;
            long timeB = md.TimePlay + 5000;

            Size size = main.ClientSize;

            drawGrids(g, size, md);
            drawNotes(g, size, md, timeA, timeB);

            drawPlayTime(g, size, md);
        }
        void drawPlayTime(Graphics g, Size size, MData md)
        {
            krect rect = new krect(size.Width - 100, size.Height - 20, 100, 20);

            var text = $"{md.TimePlay / 10} / {md.TimeMax / 10}";
            Font font = new Font("Arial", 10);

            g.FillRectangle(Brushes.Gray, rect.R);

            var rectT = rect.copy();
            rectT.offset(3, 2);
            g.DrawString(text, font,Brushes.Black, rectT.R);
        }
        void drawNotes(Graphics g, Size size, MData md, long timeA, long timeB)
        {
            pRects.Clear();
            mRects.Clear();
            sRects.Clear();

            for (long t = timeA; t <= timeB; t++)
            {
                if (md.kNotesX.ContainsKey(t))
                {
                    pRects[t] = new List<krect>();

                    foreach (var knote in md.kNotesX[t])
                    {
                        krect rect = getRect(t, knote, size, md);
                        pRects[t].Add(rect);

                        if (knote.selected)
                        {
                            if (!sRects.ContainsKey(t))
                                sRects[t] = new List<krect>();

                            krect srect = rect.copy();
                            srect.inflate(1, 1);
                            sRects[t].Add(srect);
                        }
                        if (knote.isMel)
                        {
                            if (!mRects.ContainsKey(t))
                                mRects[t] = new List<krect>();

                            krect mrect = rect.copy();
                            mrect.inflate(1, 1);
                            mRects[t].Add(mrect);
                        }
                    }
                }
            }

            Font font = new Font("Arial", 8);

            foreach (var time in pRects.Keys)
            {
                var rects = pRects[time];
                int n = -1;

                foreach (var rect in rects)
                {
                    n++;
                    var N = rects.Count - 1 - n;
                    var knote = md.kNotesX[time][n];

                    //var colorNote = knote.color;
                    if(!md.channs.ContainsKey(knote.Channel))
                    {
                        Console.WriteLine($"누락채널 : {knote.Time} {knote.Pitch} {knote.Channel}");
                        continue;
                    }

                    var toolname = md.channs[knote.Channel];
                    var colorNote = MData.ToolColors[toolname];

                    g.DrawRectangle(md.GetCachedPen(colorNote), rect.R);
                    if (toolname == "트럼펫")
                    {
                        var rectk = rect.copy();
                        rectk.inflate(-1, -2);
                        g.FillRectangle(md.GetCachedBrush(colorNote), rectk.R);
                    }

                    var pit = $"{knote.Pitch % 100}";
                    var rectT = rect.copy();
                    rectT.offset(-2, 0);

                    //var brushText = getTextBrush(colorNote);
                    var brushText = md.GetCachedBrush(Color.FloralWhite);
                    g.DrawString(pit, font, brushText, rectT.R);
                }
            }
            foreach (var time in mRects.Keys)
            {
                foreach (var rect in mRects[time])
                {
                    g.DrawRectangle(Pens.White, rect.R);
                }
            }
            foreach (var time in sRects.Keys)
            {
                foreach (var rect in sRects[time])
                {
                    g.DrawRectangle(Pens.Violet, rect.R);
                }
            }
        }
        Brush getTextBrush(Color bcolor)
        {
            // W3C 가이드라인에 따른 밝기 계산 공식 (상대적 휘도)
            // 0.299*R + 0.587*G + 0.114*B
            double luminance = (0.299 * bcolor.R + 0.587 * bcolor.G + 0.114 * bcolor.B);

            // 밝기가 128보다 크면(밝으면) 검은색, 아니면 흰색 반환
            return luminance > 128 ? Brushes.Black : Brushes.White;
        }
        void drawGrids(Graphics g, Size canv, MData md)
        {
            var H = (canv.Height - 20) / 50;// (PitchMax - PitchMin);
            var y = 10 + H * (md.PitchMax - 60);

            kvec cen = new kvec(canv.Width / 2, y);
            cen.offset(0, H / 2f);

            kvec pL = new kvec(0, cen.y);
            kvec pR = new kvec(canv.Width, cen.y);

            kvec pT = new kvec(cen.x, 0);
            kvec pB = new kvec(cen.x, canv.Height);

            g.DrawLine(Pens.Gray, pL.P, pR.P);
            g.DrawLine(Pens.Gray, pT.P, pB.P);
        }
        krect getRect(long t, KNote knote, Size canv, MData md)
        {
            var pitch = md.KPitchs[knote.Pitch];
            var scale = 0.2f;

            var H = (canv.Height - 20) / 50;// (PitchMax - PitchMin);
            var x = (int)Math.Round(canv.Width / 2 + scale * (t - md.TimePlay));

            var y = 10 + H * (md.PitchMax - pitch);
            var w = (int)Math.Round(scale * knote.Duration);
            if (w > 20)
                w -= 5;
            else
                w -= 2;

            krect rect = new krect(x, y, w, H);
            rect.inflate(0, -1);

            return rect;
        }
        public void Play(MData md, bool repeat)
        {
            if (outputDevice == null)
                outputDevice = OutputDevice.GetByIndex(0);

            /* 리젯 악기 */
            resetDevice(false);

            /* 악기 세팅 */
            foreach (var cha in md.channs)
            {
                var nch = cha.Key;
                var ntool = MData.ToolNums[cha.Value];

                var pcEvent = new ProgramChangeEvent((SevenBitNumber)ntool)
                {
                    Channel = (FourBitNumber)nch
                };

                outputDevice.SendEvent(pcEvent);
            }

            /* 플레이 */
            new Thread(() =>
            {
                if (!repeat)
                {
                    play(md, md.TimeStart, md.TimeMax + 10);
                }
                else
                {
                    while (OnRepeat && md.EnableLoop())
                    {
                        md.TimePlay = md.TimeLoopStart - 1000;
                        play(md, md.TimeLoopStart - 50, md.TimeLoopEnd);

                        Thread.Sleep(100);
                        OnPlay = true;
                    }
                }

                /* 리젯 악기 */
                resetDevice(true);
            
            }).Start();
        }
        void resetDevice(bool resetTools)
        {
            for (byte i = 0; i < 16; i++)
            {
                var nch = (FourBitNumber)i;
                // 1. All Sound Off (CC 120): 즉시 무음
                outputDevice.SendEvent(new ControlChangeEvent((SevenBitNumber)120, (SevenBitNumber)0) { Channel = nch });
                // 2. All Notes Off (CC 123): 모든 노트 해제
                outputDevice.SendEvent(new ControlChangeEvent((SevenBitNumber)123, (SevenBitNumber)0) { Channel = nch });
                // 3. Reset All Controllers (CC 121): 페달, 피치뱅크 등 초기화
                outputDevice.SendEvent(new ControlChangeEvent((SevenBitNumber)121, (SevenBitNumber)0) { Channel = nch });
                // 2. 서스테인 페달 해제 (Sustain Off)
                outputDevice.SendEvent(new ControlChangeEvent((SevenBitNumber)64, (SevenBitNumber)0) { Channel = nch });

                if (resetTools)
                    outputDevice.SendEvent(new ProgramChangeEvent((SevenBitNumber)0) { Channel = nch });
            }
        }
        void play(MData md, long timeStart, long timeEnd)
        {
            Dictionary<long, List<NoteOnEvent>> notesOn;
            Dictionary<long, List<NoteOffEvent>> notesOff;

            if (md.Melody)
            {
                notesOn = md.melsOn;
                notesOff = md.melsOff;
            }
            else
            {
                notesOn = md.notesOn;
                notesOff = md.notesOff;
            }

            var sw = Stopwatch.StartNew();
            md.TimeStart = timeStart;
            md.TimePlay = md.TimeStart;
            var timePaint = md.TimePlay;

            List<int> __pitchs__ = new List<int>();

            while (OnPlay && md.TimePlay < timeEnd)
            {
                bool isEventTime = false;

                if (notesOn.ContainsKey(md.TimePlay))
                {
                    foreach (var noteOn in notesOn[md.TimePlay])
                    {
                        var pitch = noteOn.NoteNumber;
                        __pitchs__.Contains(pitch);
                        {
                            var noteOff = new NoteOffEvent((SevenBitNumber)pitch, (SevenBitNumber)0);
                            outputDevice.SendEvent(noteOff);
                            __pitchs__.Remove(pitch);
                        }
                        __pitchs__.Add(pitch);
                        outputDevice.SendEvent(noteOn);
                    }

                    isEventTime = true;
                }
                if (notesOff.ContainsKey(md.TimePlay))
                {
                    foreach (var noteOff in notesOff[md.TimePlay])
                    {
                        outputDevice.SendEvent(noteOff);
                        __pitchs__.Remove(noteOff.NoteNumber);
                    }

                    isEventTime = true;
                }

                md.TimePlay = md.TimeStart + sw.ElapsedMilliseconds;
                //Thread.Sleep(1);

                if (!isEventTime)
                {
                    if (md.TimePlay - timePaint >= 20)
                    {
                        timePaint = md.TimePlay;
                        main.player_drawNotes();
                    }
                }
            }

            /* offEvent 미실행 음 처리 */
            foreach (var pitch in __pitchs__)
            {
                var noteOff = new NoteOffEvent((SevenBitNumber)pitch, (SevenBitNumber)0);
                outputDevice.SendEvent(noteOff);
            }

            if (md.TimePlay >= md.TimeMax)
                md.TimeStart = 0;
            else
                md.TimeStart = md.TimePlay;

            OnPlay = false;
        }
        void playNoteK(KNote knote, MData md)
        {
            var pitch = md.KPitchs[knote.Pitch];
            var vel = 100;// knote.Vel;
            var noteOn = new NoteOnEvent((SevenBitNumber)pitch, (SevenBitNumber)vel);
            var noteOff = new NoteOffEvent((SevenBitNumber)pitch, (SevenBitNumber)vel);

            if (outputDevice != null)
            {
                outputDevice.SendEvent(noteOn);

                Thread.Sleep(1000);
                outputDevice.SendEvent(noteOff);
            }

            OnPlay = false;
        }
        internal void MoveTime(bool cTRL, Keys keyCode, MData md)
        {
            switch (keyCode)
            {
                case Keys.Left:
                    if(cTRL)
                        md.TimeStart -= 10000;
                    else
                        md.TimeStart -= 1000;
                    if (md.TimeStart < 0)
                        md.TimeStart = 0;

                    break;
                case Keys.Right:
                    if (cTRL)
                        md.TimeStart += 10000;
                    else
                        md.TimeStart += 1000;
                    if (md.TimeStart > md.TimeMax)
                        md.TimeStart = 0;

                    break;
            }

            md.TimePlay = md.TimeStart;
        }
        internal KNote SelectNote(kvec pos, MData md)
        {
            KNote knote = null;

            foreach (var time in pRects.Keys)
            {
                var rects = pRects[time];
                int n = -1;

                foreach (var rect in rects)
                {
                    n++;
                    if (rect.contains(pos))
                    {
                        knote = md.kNotesX[time][n];
                        break;
                    }
                }

                if (knote != null)
                    break;
            }

            if (knote != null)
            {
                knote.selected = !knote.selected;

                Console.WriteLine($"{knote.Time} : {knote.Pitch} : {knote.Duration} : {knote.Vel}");

                if (!OnPlay)
                {
                    OnPlay = true;
                    new Thread(() => playNoteK(knote, md)).Start();
                }
            }

            return knote;
        }
        internal void ClearSels(MData md)
        {
            foreach (var nx in md.kNotesX)
            {
                foreach (var knote in nx.Value)
                {
                    knote.selected = false;
                }
            }
        }
        internal void ModifyMelos(MData md)
        {
            foreach (var nx in md.kNotesX)
            {
                var time = nx.Key;

                foreach (var knote in md.kNotesX[time])
                {
                    if (knote.selected)
                    {
                        knote.isMel = !knote.isMel;
                        knote.selected = false;
                    }
                }
            }
        }
        internal void SetNoteStart(kvec pos, MData md)
        {
            var knote = SelectNote(pos, md);
            if (knote != null)
            {
                if (knote.selected)
                {
                    md.TimeLoopStart = knote.Time;
                    Console.WriteLine($"LoopStart {md.TimeLoopStart}");
                }
                else
                    md.TimeLoopStart = -1;
            }
        }
        internal void SetNoteEnd(kvec pos, MData md)
        {
            var knote = SelectNote(pos, md);
            if (knote != null)
            {
                if (knote.selected)
                {
                    md.TimeLoopEnd = knote.Time + knote.Duration;
                    Console.WriteLine($"LoopEnd {md.TimeLoopEnd}");
                }
                else
                    md.TimeLoopEnd = -1;
            }
        }
    }
}

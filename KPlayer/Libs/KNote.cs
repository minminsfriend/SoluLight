using Android.Graphics;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.MusicTheory;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using MidiFile = Melanchall.DryWetMidi.Core.MidiFile;
using Note = Melanchall.DryWetMidi.Interaction.Note;

namespace KPlayer
{
    public class KNote
    {
        public long Time;     
        public long Duration;
        public int Pitch;
        public int Vel;

        public int Octave;
        public NoteName noteName;
        public Color color;
        public bool selected;
        public bool isMel;
        public int Channel;

        public KNote(long time, int pitch, long duration, int vel, int channel, bool mel)
        {
            Time = time;
            Pitch = pitch;
            Duration = duration;
            Vel = vel;

            noteName = GetNoteName(out Octave);
            color = GetNoteColor(noteName);
            selected = false;

            Channel = channel;
            isMel = mel;
        }
        public override bool Equals(object obj)
        {
            if (obj is KNote other)
            {
                return this.Time == other.Time && this.Pitch == other.Pitch;
            }

            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Time.GetHashCode();
                hash = hash * 23 + Pitch.GetHashCode();
                return hash;
            }
        }
        NoteName GetNoteName(out int octave)
        {
            octave = Pitch / 100;
            var noteNam = Pitch % 100;

            switch (noteNam)
            {
                default: return NoteName.C;
                case 10: return NoteName.C;
                case 15: return NoteName.CSharp;
                case 20: return NoteName.D;
                case 25: return NoteName.DSharp;
                case 30: return NoteName.E;
                case 35: return NoteName.F;
                case 40: return NoteName.FSharp;
                case 45: return NoteName.G;
                case 50: return NoteName.GSharp;
                case 55: return NoteName.A;
                case 60: return NoteName.ASharp;
                case 65: return NoteName.B;
            }
        }
        public static int GetPitch(int octave, NoteName noteName)
        {
            var pitch = 10;

            switch (noteName)
            {
                default: pitch = 10; break;
                case NoteName.C: pitch = 10; break;
                case NoteName.CSharp: pitch = 15; break;
                case NoteName.D: pitch = 20; break;
                case NoteName.DSharp: pitch = 25; break;
                case NoteName.E: pitch = 30; break;
                case NoteName.F: pitch = 35; break;
                case NoteName.FSharp: pitch = 40; break;
                case NoteName.G: pitch = 45; break;
                case NoteName.GSharp: pitch = 50; break;
                case NoteName.A: pitch = 55; break;
                case NoteName.ASharp: pitch = 60; break;
                case NoteName.B: pitch = 65; break;
            }

            return octave * 100 + pitch;
        }
        public static Color GetNoteColor(NoteName note)
        {
            switch (note)
            {
                case NoteName.C: return Color.Red;
                case NoteName.CSharp: return Color.Coral;
                case NoteName.D: return Color.Orange;
                case NoteName.DSharp: return Color.Gold;
                case NoteName.E: return Color.Teal;
                case NoteName.F: return Color.LightGreen;
                case NoteName.FSharp: return Color.Magenta;
                case NoteName.G: return Color.Azure;
                case NoteName.GSharp: return Color.SkyBlue;
                case NoteName.A: return Color.Khaki;
                case NoteName.ASharp: return Color.LightPink;
                case NoteName.B: return Color.Violet;
                default: return Color.Gray;
            }
        }
        internal KNote copy()
        {
            KNote knote = new KNote(Time, Pitch, Duration, Vel, Channel, isMel);
            return knote;
        }
    }
}

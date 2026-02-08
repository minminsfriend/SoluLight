using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.MusicTheory;
using shine.libs.math;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using MidiFile = Melanchall.DryWetMidi.Core.MidiFile;

namespace KPlayer
{
    public class MData
    {
        public Dictionary<int, int> KPitchs;
        public Dictionary<int, string> PNames;
        public Dictionary<long, List<KNote>> kNotesX = new Dictionary<long, List<KNote>>();
        public Dictionary<long, List<KNote>> kNotesP = new Dictionary<long, List<KNote>>();
        public Dictionary<long, List<NoteOnEvent>> notesOn = new Dictionary<long, List<NoteOnEvent>>();
        public Dictionary<long, List<NoteOffEvent>> notesOff = new Dictionary<long, List<NoteOffEvent>>();
        public Dictionary<long, List<NoteOnEvent>> melsOn = new Dictionary<long, List<NoteOnEvent>>();
        public Dictionary<long, List<NoteOffEvent>> melsOff = new Dictionary<long, List<NoteOffEvent>>();
        
        public static readonly Dictionary<string, int> ToolNums = new Dictionary<string, int>()
        {
            // 0-7: 피아노
            { "어쿠스틱그랜드피아노", 0 }, { "밝은피아노", 1 }, { "전기그랜드피아노", 2 }, { "혼키통크피아노", 3 },
            { "전기피아노1", 4 }, { "전기피아노2", 5 }, { "하프시코드", 6 }, { "클라비넷", 7 },
        
            // 8-15: 반음계 타악기
            { "글로켄슈필", 8 }, { "뮤직박스", 9 }, { "비브라폰", 10 }, { "마림바", 11 },
            { "실로폰", 12 }, { "튜블러벨", 13 }, { "둘시머", 14 }, { "드로바오르간", 15 },
        
            // 16-23: 오르간
            { "퍼커시브오르간", 16 }, { "락오르간", 17 }, { "교회오르간", 18 }, { "리드오르간", 19 },
            { "아코디언", 20 }, { "하모니카", 21 }, { "반도네온", 22 }, { "나일론기타", 23 },
        
            // 24-31: 기타
            { "어쿠스틱기타", 24 }, { "스틸기타", 25 }, { "재즈기타", 26 }, { "클린기타", 27 },
            { "뮤트기타", 28 }, { "오버드라이브기타", 29 }, { "디스토션기타", 30 }, { "기타하모닉스", 31 },
        
            // 32-39: 베이스
            { "어쿠스틱베이스", 32 }, { "핑거베이스", 33 }, { "픽베이스", 34 }, { "프렛리스베이스", 35 },
            { "슬랩베이스1", 36 }, { "슬랩베이스2", 37 }, { "신스베이스1", 38 }, { "신스베이스2", 39 },
        
            // 40-47: 현악기
            { "바이올린", 40 }, { "비올라", 41 }, { "첼로", 42 }, { "콘트라베이스", 43 },
            { "트레몰로스트링", 44 }, { "피치카토스트링", 45 }, { "오케스트라하프", 46 }, { "팀파니", 47 },
        
            // 48-55: 앙상블
            { "스트링앙상블1", 48 }, { "스트링앙상블2", 49 }, { "신스스트링1", 50 }, { "신스스트링2", 51 },
            { "콰이어아", 52 }, { "콰이어오", 53 }, { "신스보이스", 54 }, { "오케스트라히트", 55 },
        
            // 56-63: 브라스
            { "트럼펫", 56 }, { "트롬본", 57 }, { "튜바", 58 }, { "뮤트트럼펫", 59 },
            { "프렌치호른", 60 }, { "브라스섹션", 61 }, { "신스브라스1", 62 }, { "신스브라스2", 63 },
        
            // 64-71: 리드
            { "소프라노색소폰", 64 }, { "알토색소폰", 65 }, { "테너색소폰", 66 }, { "바리톤색소폰", 67 },
            { "오보에", 68 }, { "잉글리쉬호른", 69 }, { "바순", 70 }, { "클라리넷", 71 },
        
            // 72-79: 파이프
            { "피콜로", 72 }, { "플루트", 73 }, { "리코더", 74 }, { "팬플루트", 75 },
            { "병불기", 76 }, { "샤쿠하치", 77 }, { "휘슬", 78 }, { "오카리나", 79 },
        
            // 80-87: 신스리드
            { "스퀘어리드", 80 }, { "톱니파리드", 81 }, { "캘리오프리드", 82 }, { "치프리드", 83 },
            { "차랑리드", 84 }, { "보이스리드", 85 }, { "다섯째리드", 86 }, { "베이스리드", 87 },
        
            // 88-95: 신스패드
            { "뉴에이지패드", 88 }, { "따뜻한패드", 89 }, { "폴리신스패드", 90 }, { "합창패드", 91 },
            { "보우패드", 92 }, { "금속패드", 93 }, { "헤일로패드", 94 }, { "스윕패드", 95 },
        
            // 96-103: 신스효과음
            { "비패드", 96 }, { "사운드트랙패드", 97 }, { "크리스탈패드", 98 }, { "대기패드", 99 },
            { "밝음패드", 100 }, { "고블린패드", 101 }, { "에코패드", 102 }, { "공상과학패드", 103 },
        
            // 104-111: 민속악기
            { "시타르", 104 }, { "밴조", 105 }, { "샤미센", 106 }, { "고토", 107 },
            { "칼림바", 108 }, { "백파이프", 109 }, { "피들", 110 }, { "샤나이", 111 },
        
            // 112-119: 퍼커션
            { "팅클벨", 112 }, { "아고고", 113 }, { "스틸드럼", 114 }, { "우드블록", 115 },
            { "태고", 116 }, { "멜로딕톰", 117 }, { "신스드럼", 118 }, { "리버스심벌", 119 },
        
            // 120-127: 효과음
            { "기타프렛소음", 120 }, { "숨소리", 121 }, { "파도소리", 122 }, { "새소리", 123 },
            { "전화벨", 124 }, { "헬리콥터", 125 }, { "박수", 126 }, { "총소리", 127 }
        };
        public static readonly Dictionary<int, string> ToolNames = ToolNums.ToDictionary(x => x.Value, x => x.Key);
        public static readonly Dictionary<string, Color> ToolColors = new Dictionary<string, Color>()
        {
            // 0-7: 피아노
            { "어쿠스틱그랜드피아노", Color.Coral }, { "밝은피아노", Color.Pink },
            { "전기그랜드피아노", Color.SlateGray }, { "혼키통크피아노", Color.DarkSlateGray },
            { "전기피아노1", Color.SteelBlue }, { "전기피아노2", Color.CadetBlue },
            { "하프시코드", Color.RosyBrown }, { "클라비넷", Color.Peru },

            // 8-15: 반음계 타악기
            { "글로켄슈필", Color.Silver }, { "뮤직박스", Color.WhiteSmoke },
            { "비브라폰", Color.Violet }, { "마림바", Color.Tan },
            { "실로폰", Color.BurlyWood }, { "튜블러벨", Color.Gainsboro },
            { "둘시머", Color.SandyBrown }, { "드로바오르간", Color.DarkKhaki },

            // 16-23: 오르간
            { "퍼커시브오르간", Color.Sienna }, { "락오르간", Color.Chocolate },
            { "교회오르간", Color.DarkMagenta }, { "리드오르간", Color.Purple },
            { "아코디언", Color.MediumOrchid }, { "하모니카", Color.SkyBlue },
            { "반도네온", Color.Plum }, { "나일론기타", Color.SaddleBrown },

            // 24-31: 기타
            { "어쿠스틱기타", Color.DarkOrange }, { "스틸기타", Color.Orange },
            { "재즈기타", Color.Goldenrod }, { "클린기타", Color.LimeGreen },
            { "뮤트기타", Color.ForestGreen }, { "오버드라이브기타", Color.Red },
            { "디스토션기타", Color.Firebrick }, { "기타하모닉스", Color.IndianRed },

            // 32-39: 베이스
            { "어쿠스틱베이스", Color.MidnightBlue }, { "핑거베이스", Color.Navy },
            { "픽베이스", Color.DarkBlue }, { "프렛리스베이스", Color.RoyalBlue },
            { "슬랩베이스1", Color.Blue }, { "슬랩베이스2", Color.MediumBlue },
            { "신스베이스1", Color.DarkSlateBlue }, { "신스베이스2", Color.Indigo },

            // 40-47: 현악기
            { "바이올린", Color.Gold }, { "비올라", Color.Yellow },
            { "첼로", Color.OrangeRed }, { "콘트라베이스", Color.DarkRed },
            { "트레몰로스트링", Color.LightYellow }, { "피치카토스트링", Color.LemonChiffon },
            { "오케스트라하프", Color.Khaki }, { "팀파니", Color.DarkSalmon },

            // 48-55: 앙상블
            { "스트링앙상블1", Color.LightPink }, { "스트링앙상블2", Color.Pink },
            { "신스스트링1", Color.HotPink }, { "신스스트링2", Color.DeepPink },
            { "콰이어아", Color.Lavender }, { "콰이어오", Color.Thistle },
            { "신스보이스", Color.MediumPurple }, { "오케스트라히트", Color.Crimson },

            // 56-63: 브라스
            { "트럼펫", Color.YellowGreen }, { "트롬본", Color.OliveDrab },
            { "튜바", Color.Olive }, { "뮤트트럼펫", Color.DarkOliveGreen },
            { "프렌치호른", Color.DarkGoldenrod }, { "브라스섹션", Color.Gold },
            { "신스브라스1", Color.DarkCyan }, { "신스브라스2", Color.Teal },

            // 64-71: 리드
            { "소프라노색소폰", Color.Green }, { "알토색소폰", Color.DarkGreen },
            { "테너색소폰", Color.SeaGreen }, { "바리톤색소폰", Color.MediumSeaGreen },
            { "오보에", Color.DarkCyan }, { "잉글리쉬호른", Color.LightSeaGreen },
            { "바순", Color.MediumAquamarine }, { "클라리넷", Color.Aquamarine },

            // 72-79: 파이프
            { "피콜로", Color.Cyan }, { "플루트", Color.Violet },
            { "리코더", Color.LightCyan }, { "팬플루트", Color.PowderBlue },
            { "병불기", Color.LightBlue }, { "샤쿠하치", Color.DeepSkyBlue },
            { "휘슬", Color.CornflowerBlue }, { "오카리나", Color.MediumTurquoise },

            // 80-87: 신스리드
            { "스퀘어리드", Color.SpringGreen }, { "톱니파리드", Color.GreenYellow },
            { "캘리오프리드", Color.LawnGreen }, { "치프리드", Color.MediumSpringGreen },
            { "차랑리드", Color.LightGreen }, { "보이스리드", Color.PaleGreen },
            { "다섯째리드", Color.Lime }, { "베이스리드", Color.Chartreuse },

            // 88-95: 신스패드
            { "뉴에이지패드", Color.MintCream }, { "따뜻한패드", Color.MistyRose },
            { "폴리신스패드", Color.OldLace }, { "합창패드", Color.LavenderBlush },
            { "보우패드", Color.AliceBlue }, { "금속패드", Color.LightSteelBlue },
            { "헤일로패드", Color.Azure }, { "스윕패드", Color.GhostWhite },

            // 96-103: 신스효과음
            { "비패드", Color.LightSkyBlue }, { "사운드트랙패드", Color.PaleTurquoise },
            { "크리스탈패드", Color.White }, { "대기패드", Color.LightCyan },
            { "밝음패드", Color.LightYellow }, { "고블린패드", Color.DarkSeaGreen },
            { "에코패드", Color.DarkKhaki }, { "공상과학패드", Color.MediumSlateBlue },

            // 104-111: 민속악기
            { "시타르", Color.DarkKhaki }, { "밴조", Color.BurlyWood },
            { "샤미센", Color.Maroon }, { "고토", Color.Brown },
            { "칼림바", Color.Peru }, { "백파이프", Color.DarkSeaGreen },
            { "피들", Color.Sienna }, { "샤나이", Color.Orange },

            // 112-119: 퍼커션
            { "팅클벨", Color.LightGoldenrodYellow }, { "아고고", Color.Lime },
            { "스틸드럼", Color.Silver }, { "우드블록", Color.Tan },
            { "태고", Color.Firebrick }, { "멜로딕톰", Color.DeepPink },
            { "신스드럼", Color.BlueViolet }, { "리버스심벌", Color.DarkGray },

            // 120-127: 효과음
            { "기타프렛소음", Color.DimGray }, { "숨소리", Color.Honeydew },
            { "파도소리", Color.DeepSkyBlue }, { "새소리", Color.Yellow },
            { "전화벨", Color.Red }, { "헬리콥터", Color.DarkSlateGray },
            { "박수", Color.Salmon }, { "총소리", Color.Black }
        };

        public Dictionary<int, string> channs = new Dictionary<int, string>();

        public long TimeMax = 0;
        public long TimeStart = 0;
        public long TimePlay = 0;
        public int PitchMin = 1000;
        public int PitchMax = 0;

        public bool Melody = false;
        public string file;
        public float scale;
        internal long TimeLoopEnd;
        internal long TimeLoopStart;

        public MData(string file, float scale, bool isMel)
        {
            this.file = file;
            this.scale = scale;

            fillChords();
            fillKPitchs();

            ReadTxtMidi();
            buildEvents();

            if (isMel)
                BuildMelEnvents();

            TimeLoopStart = -1;
            TimeLoopEnd = -1;
        }
        public string Name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(file);
            }
        }
        internal Color GetChannColor(int channel)
        {
            var name = ToolNames[channel];
            return ToolColors[name];
        }
        private Dictionary<Color, SolidBrush> _brushCache = new Dictionary<Color, SolidBrush>();
        private Dictionary<Color, Pen> _penCache = new Dictionary<Color, Pen>();

        public SolidBrush GetCachedBrush(Color color)
        {
            if (!_brushCache.ContainsKey(color))
            {
                _brushCache[color] = new SolidBrush(color);
            }
            return _brushCache[color];
        }
        internal Pen GetCachedPen(Color color)
        {
            if (!_penCache.ContainsKey(color))
            {
                _penCache[color] = new Pen(color);
            }
            return _penCache[color];
        }
        public bool EnableLoop()
        {
            return TimeLoopStart >= 0 && TimeLoopEnd<= TimeMax && TimeLoopEnd > TimeLoopStart; 
        }
        public void Scaling()
        {
            kNotesX.Clear();

            foreach (var kns in kNotesP)
            {
                var time = (long)(scale * kns.Key);

                if (!kNotesX.ContainsKey(time))
                {
                    kNotesX[time] = new List<KNote>();
                }

                foreach (var knote0 in kns.Value)
                {
                    var knote = knote0.copy();

                    knote.Time = time;
                    knote.Duration = (int)(scale * knote0.Duration);

                    kNotesX[time].Add(knote);
                }
            }
        }
        public void Scaling(float scalex)
        {
            scale = scalex;
            Scaling();
        }
        void ReadTxtMidi()
        {
            var lines = File.ReadAllLines(file, Encoding.UTF8);
            var knotes = new List<KNote>();

            var readMode = "none";

            foreach (var linex in lines)
            {
                var line = linex.Trim();

                if (line.Length == 0) continue;

                if (readMode != "노트들")
                {
                    if (line.StartsWith("# 채널"))
                    {
                        readMode = "채널악기";
                        continue;
                    }
                    else if (line.StartsWith("# 노트들"))
                    {
                        readMode = "노트들";
                        continue;
                    }
                }

                if (readMode == "채널악기")
                {
                    var ss = Regex.Split(line, " : ");
                    if (ss.Length < 2) continue;

                    int ch = int.Parse(ss[0].Trim());
                    channs[ch] = ss[1].Trim();
                }
                else if (readMode == "노트들")
                {
                    var ss = Regex.Split(line, " : ");

                    if (ss.Length < 5) continue;

                    var time = long.Parse(ss[0]);

                    var pp = Regex.Split(ss[1], " ");
                    int pitch = int.Parse(pp[1]);
                    var dd = Regex.Split(ss[2], " ");
                    int dura = int.Parse(dd[1]);
                    var vv = Regex.Split(ss[3], " ");
                    int vel = int.Parse(vv[1]);
                    var cc = Regex.Split(ss[4], " ");
                    int ch = int.Parse(cc[1]);

                    bool mel = 1 == int.Parse(ss[5]);

                    knotes.Add(new KNote(time, pitch, dura, vel, ch, mel));
                }
            }

            kNotesP.Clear();

            foreach (var nox in knotes)
            {
                //Console.WriteLine($"not.Pitch == {not.Pitch}");

                if (!KPitchs.Keys.Contains(nox.Pitch)) continue;

                var pitch = KPitchs[nox.Pitch];

                if (!kNotesP.ContainsKey(nox.Time))
                {
                    kNotesP[nox.Time] = new List<KNote>();
                }

                kNotesP[nox.Time].Add(nox);
            }

            Scaling(scale);
        }
        void buildEvents()
        {
            notesOn.Clear();
            notesOff.Clear();

            PitchMin = 1000;
            PitchMax = 0;
            TimeMax = 0;

            foreach (var nx in kNotesX)
            {
                //Console.WriteLine($"time:{nx.Key},  음갯수:{nx.Value.Count}");

                foreach (var knote in nx.Value)
                {
                    var pitch = KPitchs[knote.Pitch];
                    var vel = knote.Vel;
                    var noteOn = new NoteOnEvent((SevenBitNumber)pitch, (SevenBitNumber)vel);
                    var noteOff = new NoteOffEvent((SevenBitNumber)pitch, (SevenBitNumber)0);
                    noteOn.Channel = (FourBitNumber)knote.Channel;
                    noteOff.Channel = noteOn.Channel;

                    var timeOn = knote.Time;
                    var timeOff = knote.Time + knote.Duration;

                    if (!notesOn.ContainsKey(timeOn))
                        notesOn[timeOn] = new List<NoteOnEvent>();

                    if (!notesOff.ContainsKey(timeOff))
                        notesOff[timeOff] = new List<NoteOffEvent>();

                    notesOn[timeOn].Add(noteOn);
                    notesOff[timeOff].Add(noteOff);

                    if (timeOff > TimeMax)
                        TimeMax = timeOff;

                    if (pitch < PitchMin)
                        PitchMin = pitch;
                    if (pitch > PitchMax)
                        PitchMax = pitch;
                }
            }
        }
        public void BuildMelEnvents()
        {
            melsOn.Clear();
            melsOff.Clear();

            foreach (var nx in kNotesX)
            {
                var time = nx.Key;

                foreach (var knote in kNotesX[time])
                {
                    if (!knote.isMel)
                        continue;

                    var pitch = KPitchs[knote.Pitch];
                    var vel = knote.Vel;
                    var melOn = new NoteOnEvent((SevenBitNumber)pitch, (SevenBitNumber)vel);
                    var melOff = new NoteOffEvent((SevenBitNumber)pitch, (SevenBitNumber)0);

                    var timeOn = knote.Time;
                    var timeOff = knote.Time + knote.Duration;

                    if (!melsOn.ContainsKey(timeOn))
                        melsOn[timeOn] = new List<NoteOnEvent>();

                    if (!melsOff.ContainsKey(timeOff))
                        melsOff[timeOff] = new List<NoteOffEvent>();

                    melsOn[timeOn].Add(melOn);
                    melsOff[timeOff].Add(melOff);
                }
            }
        }
        void fillKPitchs()
        {
            PNames = new Dictionary<int, string>();

            PNames[10] = "도";//C
            PNames[15] = "도#";
            PNames[20] = "레";//D
            PNames[25] = "레#";
            PNames[30] = "미";//E
            PNames[35] = "파";//F
            PNames[40] = "파#";
            PNames[45] = "솔";//G
            PNames[50] = "솔#";
            PNames[55] = "라";//A
            PNames[60] = "라#";
            PNames[65] = "시";//B
            PNames[70] = "도";//C

            KPitchs = new Dictionary<int, int>();

            KPitchs[110] = 24;
            KPitchs[115] = 25;
            KPitchs[120] = 26;
            KPitchs[125] = 27;
            KPitchs[130] = 28;
            KPitchs[135] = 29;
            KPitchs[140] = 30;
            KPitchs[145] = 31;
            KPitchs[150] = 32;
            KPitchs[155] = 33;
            KPitchs[160] = 34;
            KPitchs[165] = 35;
            KPitchs[170] = 36;

            KPitchs[210] = 36;
            KPitchs[215] = 37;
            KPitchs[220] = 38;
            KPitchs[225] = 39;
            KPitchs[230] = 40;
            KPitchs[235] = 41;
            KPitchs[240] = 42;
            KPitchs[245] = 43;
            KPitchs[250] = 44;
            KPitchs[255] = 45;
            KPitchs[260] = 46;
            KPitchs[265] = 47;
            KPitchs[270] = 48;

            KPitchs[310] = 48;
            KPitchs[315] = 49;
            KPitchs[320] = 50;
            KPitchs[325] = 51;
            KPitchs[330] = 52;
            KPitchs[335] = 53;
            KPitchs[340] = 54;
            KPitchs[345] = 55;
            KPitchs[350] = 56;
            KPitchs[355] = 57;
            KPitchs[360] = 58;
            KPitchs[365] = 59;
            KPitchs[370] = 60;

            KPitchs[410] = 60;
            KPitchs[415] = 61;
            KPitchs[420] = 62;
            KPitchs[425] = 63;
            KPitchs[430] = 64;
            KPitchs[435] = 65;
            KPitchs[440] = 66;
            KPitchs[445] = 67;
            KPitchs[450] = 68;
            KPitchs[455] = 69;
            KPitchs[460] = 70;
            KPitchs[465] = 71;
            KPitchs[470] = 72;

            KPitchs[510] = 72;
            KPitchs[515] = 73;
            KPitchs[520] = 74;
            KPitchs[525] = 75;
            KPitchs[530] = 76;
            KPitchs[535] = 77;
            KPitchs[540] = 78;
            KPitchs[545] = 79;
            KPitchs[550] = 80;
            KPitchs[555] = 81;
            KPitchs[560] = 82;
            KPitchs[565] = 83;
            KPitchs[570] = 84;

            KPitchs[610] = 84;
            KPitchs[615] = 85;
            KPitchs[620] = 86;
            KPitchs[625] = 87;
            KPitchs[630] = 88;
            KPitchs[635] = 89;
            KPitchs[640] = 90;
            KPitchs[645] = 91;
            KPitchs[650] = 92;
            KPitchs[655] = 93;
            KPitchs[660] = 94;
            KPitchs[665] = 95;
            KPitchs[670] = 96;

            KPitchs[710] = 96;
            KPitchs[715] = 97;
            KPitchs[720] = 98;
            KPitchs[725] = 99;
            KPitchs[730] = 100;
            KPitchs[735] = 101;
            KPitchs[740] = 102;
            KPitchs[745] = 103;
            KPitchs[750] = 104;
            KPitchs[755] = 105;
            KPitchs[760] = 106;
            KPitchs[765] = 107;
            KPitchs[770] = 108;

            KPitchs[810] = 108;
            KPitchs[815] = 109;
            KPitchs[820] = 110;
            KPitchs[825] = 111;
            KPitchs[830] = 112;
            KPitchs[835] = 113;
            KPitchs[840] = 114;
            KPitchs[845] = 115;
            KPitchs[850] = 116;
            KPitchs[855] = 117;
            KPitchs[860] = 118;
            KPitchs[865] = 119;
            KPitchs[870] = 120;

            KPitchs[910] = 120;
            KPitchs[915] = 121;
            KPitchs[920] = 122;
            KPitchs[925] = 123;
            KPitchs[930] = 124;
            KPitchs[935] = 125;
            KPitchs[940] = 126;
            KPitchs[945] = 127;
        }
        void fillChords()
        {
            var chords = new Dictionary<string, int[]>();

            //chords["C major"] = new[] { 10, 30, 45 };
            //chords["C minor"] = new[] { 10, 25, 45 };
            //chords["D minor"] = new[] { 20, 35, 55 };
            //chords["E major"] = new[] { 30, 50, 65 };
            //chords["E minor"] = new[] { 30, 45, 65 };
            //chords["F major"] = new[] { 35, 55, 70 };
            //chords["G major"] = new[] { 45, 65, 80 };
            //chords["A minor"] = new[] { 55, 70, 90 };`

            chords["C major"] = new[] { 10, 20, 35 };
            chords["C minor"] = new[] { 10, 15, 35 };
            chords["D minor"] = new[] { 20, 15, 35 };
            chords["E major"] = new[] { 30, 20, 35 };
            chords["E minor"] = new[] { 30, 15, 35 };
            chords["F major"] = new[] { 35, 20, 35 };
            chords["G major"] = new[] { 45, 20, 35 };
            chords["A minor"] = new[] { 55, 15, 35 };
        }

    
    }
}

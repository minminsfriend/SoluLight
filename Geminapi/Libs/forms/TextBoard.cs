using Android.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpToken;
using shine.libs.math;
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
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
//using static System.Net.WebRequestMethods;
//using static System.Net.Mime.MediaTypeNames;

namespace Geminapi
{
    using shine.libs.graphics;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    public partial class TextBoard : Form
    {
        int indexPageCurr = 0;

        int fontSize = 10;
        int LineHei = 20;
        Font fonta = new Font("맑은 고딕", 10);
        string filepath;
        List<Dictionary<string, string>> pages = new List<Dictionary<string, string>>();

        Bitmap bmpTemp = new Bitmap(760, 150, PixelFormat.Format24bppRgb);
        Geminapi main;

        public TextBoard(Geminapi main)
        {
            this.main = main;

            InitializeComponent();

            // 폼 설정
            this.DoubleBuffered = true; // 깜빡임 방지
            this.KeyPreview = true;      // 폼에서 키 입력을 먼저 받음

            // 초기 페이지 분할 실행
            this.Load += TextBoard_Load;
            this.KeyDown += TextBoard_KeyDown;

            this.Paint += TextBoard_Paint;
        }
        void TextBoard_Load(object sender, EventArgs e)
        {
            this.ClientSize = new Size(bmpTemp.Width, bmpTemp.Width);
        }
        void TextBoard_Paint(object sender, PaintEventArgs e)
        {
            if (pages.Count == 0) return;

            Graphics g = e.Graphics;
            var filename = Path.GetFileNameWithoutExtension(filepath);

            using (var bmpPage = buildBmp(this.ClientSize, indexPageCurr, filename))
            {
                krect rectDst = new krect(this.ClientSize);
                krect rectSrc = new krect(bmpPage.Width, bmpPage.Height);

                g.DrawImage(bmpPage, rectDst.R, rectSrc.R, GraphicsUnit.Pixel);
            }
        }
        void CalculatePages(string[] rawLines)
        {
            pages.Clear();
            var currentPageDict = new Dictionary<string, string>();

            Size size = this.ClientSize;
            Console.WriteLine($"이미지 사이즈 : {size.Width} x {size.Height}");

            int currentLineSum = 0;
            int maxLinesPerPage = (int)((size.Height - 40) / LineHei);
            float maxWidth = size.Width - 20 * 2 + 10;

            for (int i = 0; i < rawLines.Length; i++)
            {
                // 1. 픽셀 스캔으로 줄 수 확인 (앞서 만든 함수 호출)
                int vLines = GetActualLineCount(rawLines[i], fonta, maxWidth);

                // 2. 페이지 한도 초과 시 딕셔너리를 리스트에 넣고 새로 시작
                if (currentLineSum + vLines > maxLinesPerPage)
                {
                    pages.Add(new Dictionary<string, string>(currentPageDict));
                    currentPageDict.Clear();
                    currentLineSum = 0;
                }

                // 3. 키값에 "인덱스/줄수" 형태로 저장
                string key = $"{i}/{vLines}";
                currentPageDict[key] = rawLines[i];

                currentLineSum += vLines;
            }
            if (currentPageDict.Count > 0) pages.Add(currentPageDict);
        }
        int GetActualLineCount(string line, Font font, float maxWidth)
        {
            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                g.Clear(Color.Black); // 검은 배경
                                      // 1. 렉탱글에 그리기
                var rect = new krect(0, 0, maxWidth, bmpTemp.Height);
                g.DrawString(line, font, Brushes.White, rect.R, StringFormat.GenericTypographic);
            }

            // 2. 아래서부터 위로 올라오며 첫 번째 흰색 픽셀 찾기 (가장 빠른 스캔)
            int lastY = 0;
            for (int y = bmpTemp.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < 50; x++) // 왼쪽 일부만 검사해도 충분
                {
                    if (bmpTemp.GetPixel(x, y).R > 128)
                    {
                        lastY = y;
                        goto Found; // 이중 루프 탈출
                    }
                }
            }

        Found:
            if (lastY == 0) return 1; // 빈 줄이어도 최소 1줄 점유

            // 3. 실제 줄 수 계산 (행간 포함 높이 고려)
            // 단순히 폰트 높이로 나누고 올림 처리
            return (int)Math.Ceiling((float)lastY / LineHei);
        }
        void TextBoard_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.PageDown:
                case Keys.Space:
                    if (indexPageCurr < pages.Count - 1)
                    {
                        indexPageCurr++;
                        this.Invalidate();
                    }
                    break;
                case Keys.PageUp:
                case Keys.Back:
                    if (indexPageCurr > 0)
                    {
                        indexPageCurr--;
                        this.Invalidate();
                    }
                    break;
                case Keys.Home:
                    indexPageCurr = 0;
                    this.Invalidate();
                    break;
                case Keys.F6:
                    if (filepath != null)
                    {
                        var lines = File.ReadAllLines(filepath, Encoding.UTF8);

                        CalculatePages(lines);
                        this.Invalidate();
                    }
                    break;
                case Keys.Escape:
                    this.Hide();
                    main.Show();
                    main.Focus();
                    break;
                case Keys.F7:
                    BuildImagePages(filepath);
                    break;
                case Keys.F1:
                    showHelpBox();
                    break;
                default:
                    return;
            }
        }
        void showHelpBox()
        {
            MessageBox.Show(
                "TextBoard 단축키:\n\n" +
                "Space: 다음 페이지\n" +
                "Backspace: 이전 페이지\n" +
                "Home: 첫 페이지로 이동\n" +
                "F6: 페이지 나누기\n" +
                "F7: 모든 페이지 이미지로\n" +
                "Esc: TextBoard 닫기\n" +
                "F1: 도움말 표시",
                "TextBoard 도움말",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        public Bitmap buildBmp(Size size, int indexPage, string filename)
        {
            Bitmap bmpPage = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);

            Graphics g = Graphics.FromImage(bmpPage);

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Color.Black);

            krect rect0 = new krect(20, 15, size.Width - 20 * 2, LineHei * 5);
            var rectT = rect0.copy();
            rectT.offset(0, -5);

            var pagetext = $"  [ {indexPage + 1} / {pages.Count} ]      {filename}";
            g.DrawString(pagetext, fonta, Brushes.Yellow, rectT.R, StringFormat.GenericTypographic);

            var linesCount = 0;
            rect0.offset(0, LineHei);

            var pageDict = pages[indexPage];

            foreach (var entry in pageDict)
            {
                string[] info = entry.Key.Split('/');
                int addLinesCount = int.Parse(info[1]);
                string text = entry.Value;

                var rect = rect0.copy();
                rect.offset(0, linesCount * LineHei);
                linesCount += addLinesCount;

                g.DrawString(text, fonta, Brushes.White, rect.R, StringFormat.GenericTypographic);
            }

            return bmpPage;
        }
        public void BuildImagePages(string filetext)
        {
            var dirImagesx = Path.GetDirectoryName(filetext);
            var filename = Path.GetFileNameWithoutExtension(filetext);

            var dirImages = $@"{dirImagesx}\pages_{filename}";

            if (!Directory.Exists(dirImages))
                Directory.CreateDirectory(dirImages);

            //Size size = this.ClientSize;
            Size size = new Size(bmpTemp.Width, bmpTemp.Width);

            var ext = ".jpg";
            var n = -1;

            while (++n < pages.Count)
            {
                var filebmp = $@"{dirImages}\{filename}{n:00}{ext}";

                using (var bmpPage = buildBmp(size, n, filename))
                {
                    bmpPage.Save(filebmp, ImageFormat.Jpeg);
                }
            }
        }
        internal void ShowFile(string filepath)
        {
            this.filepath = filepath;

            var lines = File.ReadAllLines(filepath, Encoding.UTF8);

            CalculatePages(lines);
            indexPageCurr = 0;
            this.Invalidate();

            this.Show();
            this.Focus();
        }
    }
}
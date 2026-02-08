using shine.libs.math;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;

using shine.libs.window;
using shine.libs.capture;
using System.Diagnostics;
using shine.libs.simul;

namespace Bacro
{
    public class CapDho
    {
        public bool DhoExists
        {
            get { return hwndsDho.Count > 0; }
        }

        List<IntPtr> hwndsDho = new List<IntPtr>();
        List<krect> rectsDho = new List<krect>();

        public krect RectDho
        {
            get
            {
                var LR = main.buttons.bboxs[4].title;

                if (rectsDho.Count == 1)
                    return rectsDho[0];
                else if (rectsDho.Count == 2)
                    return LR == "LEFT" ? rectsDho[0] : rectsDho[1];
                else
                    return null;
            }
        }
        public IntPtr HwndDho
        {
            get
            {
                var LR = main.buttons.bboxs[4].title;

                if (hwndsDho.Count == 1)
                    return hwndsDho[0];
                else if (hwndsDho.Count == 2)
                    return LR == "LEFT" ? hwndsDho[0] : hwndsDho[1];
                else
                    return IntPtr.Zero;
            }
        }
        Bacro main;
        public CapDho(Bacro main)
        {
            this.main = main;

            SearchDhoWindows();
            FocusWorkDho();
        }
        public void SearchDhoWindows()
        {
            /*초기화*/
            hwndsDho.Clear();
            rectsDho.Clear();

            /*임시*/
            List<IntPtr> hwnds = new List<IntPtr>();
            List<krect> rects = new List<krect>();

            Process[] processRunning = Process.GetProcesses();

            foreach (Process pr in processRunning)
            {
                if (pr.MainWindowTitle == "대항해시대 온라인")
                {
                    hwnds.Add(pr.MainWindowHandle);
                }
            }

            Console.WriteLine($"♥ DHO 창 갯수 ♥ [{hwnds.Count}]");

            if (hwnds.Count == 0)
                return;

            foreach (var hwnd in hwnds)
            {
                krect rect = new krect();
                wop.getWindowRect(hwnd, ref rect);

                rects.Add(rect);
            }

            if (hwnds.Count == 1)
            {
                hwndsDho.Add(hwnds[0]);
                rectsDho.Add(rects[0]);

                Console.WriteLine($"창 하나 ▶ {rectsDho[0].toString()}");
            }
            else if (hwnds.Count == 2)
            {
                if (rects[0].x < rects[1].x)
                {
                    hwndsDho.Add(hwnds[0]);
                    rectsDho.Add(rects[0]);
                    hwndsDho.Add(hwnds[1]);
                    rectsDho.Add(rects[1]);
                }
                else
                {
                    hwndsDho.Add(hwnds[1]);
                    rectsDho.Add(rects[1]);
                    hwndsDho.Add(hwnds[0]);
                    rectsDho.Add(rects[0]);
                }

                Console.WriteLine($"좌 창 ▶ {rectsDho[0].toString()}");
                Console.WriteLine($"우 창 ▶ {rectsDho[1].toString()}");
            }
        }
        public byte[] DhoCapAndSend()
        {
            var hwnd = HwndDho;

            if (wop.IsWindow(hwnd))
            {
                wop.SetForegroundWindow(hwnd);
                Thread.Sleep(100);
            }

            byte[] data = null;
            ScreenCopy.GetBytes(RectDho, ImageFormat.Png, ref data);

            return data;
        }
        public byte[] DhoCapSelAndSend(krect rectSel)
        {
            var hwndDho = HwndDho;

            if (wop.IsWindow(hwndDho))
            {
                wop.SetForegroundWindow(hwndDho);
                Thread.Sleep(100);
            }

            rectSel.offset(1f * RectDho.pos());

            byte[] data = null;
            ScreenCopy.GetBytes(rectSel, ImageFormat.Png, ref data);

            return data;
        }
        internal void FocusWorkDho()
        {
            if (HwndDho == IntPtr.Zero)
                SearchDhoWindows();

            var hwndDho = HwndDho;

            if (hwndDho != IntPtr.Zero && wop.IsWindow(hwndDho))
            {
                wop.SetForegroundWindow(hwndDho);
            }
        }
        internal void printCursor()
        {
            MouseSimul.MousePoint mp = MouseSimul.GetCursorPosition();
            kvec cursor = new kvec(mp.X, mp.Y);

            if (hwndsDho.Count == 0)
                SearchDhoWindows();

            if (hwndsDho.Count > 0)
                cursor.offset(-1f * RectDho.pos());

            var line = $"\nposs[\"xxx\"] = new kvec({cursor.X}, {cursor.Y});";
            Console.WriteLine(line);
        }
        internal Bitmap CapImage()
        {
            var hwndDho = HwndDho;
            var rectDho = RectDho;

            if (hwndDho == IntPtr.Zero || rectDho == null)
                return null;

            if (wop.IsWindow(hwndDho))
            {
                wop.SetForegroundWindow(hwndDho);
                Thread.Sleep(100);
            }

            return ScreenCopy.GetImage(rectDho);
        }
    }
}

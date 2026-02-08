using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;
using System.Threading;

using shine.libs.math;
using shine.libs.window;

namespace shine.libs.capture
{
    public class ScreenCopy
    {
        public static byte[] Copy(krect rectCut)
        {
            // 주화면의 크기 정보 읽기
            Rectangle rect = Screen.PrimaryScreen.Bounds;
            // 2nd screen = Screen.AllScreens[1]

            // 픽셀 포맷 정보 얻기 (Optional)
            PixelFormat pixelFormat = GetPixelFormat();

            // 화면 크기만큼의 Bitmap 생성
            Bitmap bmp = new Bitmap(rectCut.W, rectCut.H, pixelFormat);

            // Bitmap 이미지 변경을 위해 Graphics 객체 생성
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                // 화면을 그대로 카피해서 Bitmap 메모리에 저장
                gr.CopyFromScreen(rectCut.X, rectCut.Y, 0, 0, rectCut.Size);
            }

            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Jpeg);
            byte[] data = stream.ToArray();

            bmp.Dispose();
            return data;
        }
        public static void GetBytes(krect rectCut, ImageFormat imageFormat, ref byte[] data)
        {
            PixelFormat pixelFormat = GetPixelFormat();

            using (Bitmap bmp = new Bitmap(rectCut.W, rectCut.H, pixelFormat))
            {
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.CopyFromScreen(rectCut.X, rectCut.Y, 0, 0, rectCut.Size);
                }
                using (MemoryStream stream = new MemoryStream())
                {
                    bmp.Save(stream, imageFormat);
                    data = stream.ToArray();
                }
            }
        }
        public static Bitmap GetImage(krect rectCut)
        {
            PixelFormat pixelFormat = GetPixelFormat();

            Bitmap bmp = new Bitmap(rectCut.W, rectCut.H, pixelFormat);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.CopyFromScreen(rectCut.X, rectCut.Y, 0, 0, rectCut.Size);
            }

            return bmp;
        }
        static PixelFormat GetPixelFormat()
        {
            int bitsPerPixel = Screen.PrimaryScreen.BitsPerPixel;
            PixelFormat pixelFormat;

            if (bitsPerPixel <= 16)
                pixelFormat = PixelFormat.Format16bppRgb565;
            else if (bitsPerPixel == 24)
                pixelFormat = PixelFormat.Format24bppRgb;
            else
                pixelFormat = PixelFormat.Format32bppArgb;

           
            return pixelFormat;
        }
        public static Bitmap CaptureApplication(string procName)
        {
            Process proc;

            // Cater for cases when the process can't be located.
            try
            {
                proc = Process.GetProcessesByName(procName)[0];
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            // You need to focus on the application
            wop.SetForegroundWindow(proc.MainWindowHandle);
            wop.ShowWindowAsync(proc.MainWindowHandle, wop.SW_RESTORE);

            // You need some amount of delay, but 1 second may be overkill
            Thread.Sleep(1000);

            RECT rect = new RECT();
            var error = wop.GetWindowRect((int)proc.MainWindowHandle, ref rect);

            // sometimes it gives error.
            while (error == 0)
            {
                error = wop.GetWindowRect((int)proc.MainWindowHandle, ref rect);
            }

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;
            int x = rect.left;
            int y = rect.top;
            Size size = new Size(width, height);

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);

            g.CopyFromScreen(x, y, 0, 0, size, CopyPixelOperation.SourceCopy);

            return bmp;
        }
    }
}

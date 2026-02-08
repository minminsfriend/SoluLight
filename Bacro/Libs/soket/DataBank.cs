using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Windows.Input;
using System.IO;
using System.Drawing;
using System.Windows.Markup;

using shine.libs.utils;

using shine.libs.konst;
using System.Globalization;
using Encoding = System.Text.Encoding;

namespace Bacro
{
    class DataBank
    {
        byte[] data;

        int filledSize = 0;
        int targetSize = 0;
        public int recvCount = 0;
        public int targetCount = 0;
        public DataBank(int size)
        {
            data = new byte[size];
        }
        public void Write(byte[] src, int srcPos, int lenWrite)
        {
            //if (fillSize >= targetSize)
            //    return;

            Array.Copy(src, srcPos, data, filledSize, lenWrite);
            filledSize += lenWrite;
        }
        public void reset(int targetSize)
        {
            this.targetSize = targetSize;

            filledSize = 0;
            recvCount = 0;
            targetCount = targetSize / 1024;
            if (targetSize % 1024 > 0)
                targetCount += 1;
        }
        internal Bitmap getImage()
        {
            if (filledSize == targetSize)
            {
                MemoryStream ms = new MemoryStream(data, 0, filledSize);
                Bitmap bitmap = new Bitmap(ms);

                return bitmap;
            }
            else
                return null;
        }
        internal string getText()
        {
            return Encoding.UTF8.GetString(data, 0, filledSize);
        }
    }
}

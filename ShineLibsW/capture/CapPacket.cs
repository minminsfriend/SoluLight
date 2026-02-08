using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.IO;

using shine.libs.math;
using shine.libs.konst;

namespace shine.libs.capture
{
    public class KPacket
    {
        public const int CHUNK = 1024;//2048;
        public const int MEGA = 1000000;
        public const int VALMAX = 294;//== (1204-2*4)/4 
        public const int KEYMAX = 49;//== 294/6 
        //public const int _B = 6; //aKey int size 

        static byte[] IB(int nn)
        {
            byte[] bb = new byte[4];
            int rem = nn;

            bb[0] = (byte)(rem / 0xffffff);
            rem = rem % 0xffffff;
            bb[1] = (byte)(rem / 0xffff);
            rem = rem % 0xffff;
            bb[2] = (byte)(rem / 0xff);
            rem = rem % 0xff;
            bb[3] = (byte)rem;

            return bb;
        }
        static int BI(byte[] bb, int index)
        {
            return bb[index + 0] * 0xffffff + bb[index + 1] * 0xffff
                + bb[index + 2] * 0xff + bb[index + 3];
        }

        public class Header
        {
            public class cHeader
            {
                public Sod.HCODE hcode;
                public Sod.CAPAREA area;
                public kvec cursor;
                public krect rect;
                public int fileLength;

                public cHeader()
                {
                    cursor = new kvec();
                    rect = new krect();
                    fileLength = 0;

                    hcode = Sod.HCODE.none;
                    area = Sod.CAPAREA.none;
                }
            }

            public static void Serialize(cHeader dp, ref byte[] data)
            {
                int n = -1;

                Array.Copy(IB((int)dp.hcode), 0, data, ++n * 4, 4);
                Array.Copy(IB((int)dp.area), 0, data, ++n * 4, 4);
                Array.Copy(IB(dp.cursor.X), 0, data, ++n * 4, 4);
                Array.Copy(IB(dp.cursor.Y), 0, data, ++n * 4, 4);
                Array.Copy(IB(dp.rect.X), 0, data, ++n * 4, 4);
                Array.Copy(IB(dp.rect.Y), 0, data, ++n * 4, 4);
                Array.Copy(IB(dp.rect.W), 0, data, ++n * 4, 4);
                Array.Copy(IB(dp.rect.H), 0, data, ++n * 4, 4);
                Array.Copy(IB(dp.fileLength), 0, data, ++n * 4, 4);
            }
            public static cHeader Deserialize(byte[] data)
            {
                cHeader dp = new cHeader();
                int n = -1;

                dp.hcode = (Sod.HCODE)BI(data, ++n * 4);
                dp.area = (Sod.CAPAREA)BI(data, ++n * 4);
                dp.cursor.x = BI(data, ++n * 4);
                dp.cursor.y = BI(data, ++n * 4);
                dp.rect.x = BI(data, ++n * 4);
                dp.rect.y = BI(data, ++n * 4);
                dp.rect.w = BI(data, ++n * 4);
                dp.rect.h = BI(data, ++n * 4);
                dp.fileLength = BI(data, ++n * 4);

                return dp;
            }
            public static Sod.HCODE GetMessage(byte[] data, ref string text)
            {
                int n = -1;
                Sod.HCODE hcode = (Sod.HCODE)BI(data, ++n * 4);
                int textlen = BI(data, ++n * 4);

                text = Encoding.UTF8.GetString(data, ++n * 4, textlen);

                return hcode;
            }

            public static void SetMessage(Sod.HCODE hcode, string text, ref byte[] data)
            {
                byte[] textdata = Encoding.UTF8.GetBytes(text);
                int textlen = Math.Min(textdata.Length, data.Length - 4 * 2);

                int n = -1;
                Array.Copy(IB((int)hcode), 0, data, ++n * 4, 4);
                Array.Copy(IB(textlen), 0, data, ++n * 4, 4);
                Array.Copy(textdata, 0, data, ++n * 4, textlen);
            }
        }
        public class Keyvals
        {
            public class aKey
            {
                public int km;
                public int action;

                public int v0, v1, v2, v3;

                public aKey(int km, int action, 
                    int v0, int v1, int v2, int v3)
                {
                    this.km = km;
                    this.action = action;
                    this.v0 = v0;
                    this.v1 = v1;
                    this.v2 = v2;
                    this.v3 = v3;
                }
            }
            public class KeysArray// max 1204 byte
            {
                public Sod.HCODE hcode;
                int _count;
                public int[] val; // 

                public KeysArray()
                {
                    hcode = Sod.HCODE.none;
                    _count = 0;
                    val = new int[VALMAX];
                }

                public int count
                {
                    get { return _count; }
                    set { _count = Math.Min(value, KEYMAX); }
                }
            }

            public static void Serialize(KeysArray kv, ref byte[] data)
            {
                int n = -1;
                int M;

                Array.Copy(IB((int)kv.hcode), 0, data, ++n * 4, 4);
                Array.Copy(IB(kv.count), 0, data, ++n * 4, 4);

                for (int i = 0; i < kv.count; i++)
                {
                    M = 6 * i;
                    Array.Copy(IB(kv.val[M + 0]), 0, data, ++n * 4, 4);
                    Array.Copy(IB(kv.val[M + 1]), 0, data, ++n * 4, 4);
                    Array.Copy(IB(kv.val[M + 2]), 0, data, ++n * 4, 4);
                    Array.Copy(IB(kv.val[M + 3]), 0, data, ++n * 4, 4);
                    Array.Copy(IB(kv.val[M + 4]), 0, data, ++n * 4, 4);
                    Array.Copy(IB(kv.val[M + 5]), 0, data, ++n * 4, 4);
                }
            }

            public static KeysArray Deserialize(byte[] data)
            {
                KeysArray kv = new KeysArray();
                int n = -1;
                int M;

                kv.hcode = (Sod.HCODE)BI(data, ++n * 4);
                kv.count = BI(data, ++n * 4);

                for (int i = 0; i < kv.count; i++)
                {
                    M = 6 * i;
                    kv.val[M + 0] = BI(data, ++n * 4);
                    kv.val[M + 1] = BI(data, ++n * 4);
                    kv.val[M + 2] = BI(data, ++n * 4);
                    kv.val[M + 3] = BI(data, ++n * 4);
                    kv.val[M + 4] = BI(data, ++n * 4);
                    kv.val[M + 5] = BI(data, ++n * 4);
                }

                return kv;
            }
        }
    }
}

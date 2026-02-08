using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Android.Graphics
{
    public enum Format
    {
        Translucent = -3,
        Transparent = -2,
        Opaque = -1,
        Unknown = 0,
        Rgba8888 = 1,
        Rgbx8888 = 2,
        Rgb888 = 3,
        Rgb565 = 4,
        Rgba5551 = 6,
        Rgba4444 = 7,
        A8 = 8,
        L8 = 9,
        La88 = 10,
        Rgb332 = 11,
        Ycbcr422Sp = 16,
        Ycbcr420Sp = 17,
        Ycbcr422I = 20,
        Jpeg = 256
    }
}

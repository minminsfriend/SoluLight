using System.Drawing;

namespace Android.Graphics
{
    public class Typeface
    {
        public FontFamily fontFamily;

        public Typeface(string fontname)
        {
            fontFamily = new FontFamily(fontname);

        }
        public static Typeface CreateFromFile(string file)
        {
            return null;
        }
    }
}
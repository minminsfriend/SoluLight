/*
 * Created by SharpDevelop.
 * User: shine
 * Date: 2016-08-09
 * Time: 오후 10:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using shine.libs.math;

namespace shine.libs.window
{
    public class KWinfo
    {
        public string title;
        public string pattern;
        public IntPtr hwin;
        public krect rect;
        public string exxe;

        public KWinfo(string title, string pattern)
        {
            this.title = title;
            this.pattern = pattern;

            hwin = IntPtr.Zero;
            rect = new krect();
            exxe = null;
        }
    }
}

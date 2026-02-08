#undef ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using shine.libs.system;

#if ANDROID
using Android.Util;
#else
using System.Diagnostics;
#endif

namespace shine.libs.utils
{
    public static class xLog
    {
        static bool enableLog = false;
        static string APPNAME = "APPNAME";

        public static void Set(string AppName, bool enable)
        {
            APPNAME = AppName;
            enableLog = enable;
        }
        public static void d(string classx, string text)
        {
            if (!enableLog)
                return;

#if ANDROID
            string line = string.Format("{0} : [{1}] {2}", APPNAME, classx, text);
            Log.Debug(classx, line);
#else
            string line = string.Format("{0} : [{1}] {2}", APPNAME, classx, text);
            Debug.WriteLine(line);
#endif

        }
        public static void LogVersion(string clas, string dirApp)
        {
            string path = dirApp + "/version.txt";
            int nver;

            if (File.Exists(path))
            {
                using (FileStream fr = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[1024];

                    int readlen = fr.Read(buffer, 0, 1024);
                    string line = Encoding.UTF8.GetString(buffer, 0, readlen);

                    nver = int.Parse(line.Trim());
                }
            }
            else
                nver = 0;

            using (FileStream fw = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                nver += 10;
                string line = string.Format("{0}", nver);
                byte[] data = Encoding.UTF8.GetBytes(line);

                fw.Write(data, 0, data.Length);
            }

            d(clas, string.Format("Version == {0}", nver));
        }
    }
}
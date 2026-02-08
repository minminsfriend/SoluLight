using System;
using System.Runtime.InteropServices;

namespace shine.libs.system
{
    public static class KSys
	{
	    static readonly DateTime Jan1St1970 = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	    /// <summary>Get extra long current timestamp</summary>
	    public static long CurrentMillis()
	    { 
	    	return (long)((DateTime.UtcNow - Jan1St1970).TotalMilliseconds); 
	    } 
		public static string ConsumedTime(long timeStart)
		{
            var timeSpent = KSys.CurrentMillis() - timeStart;

            var secSpent = timeSpent / 1000;
            var min = secSpent / 60;
            var sec = secSpent % 60;

            return $"{min}분 {sec}초";
        }

        [DllImport("KERNEL32.DLL")]
        extern public static void Beep(int freq, int dur);

        public static void beepSample()
        {
            // 도 = 256Hz
            // 레 = 도 * 9/8 = 288Hz
            // 미 = 레 * 10/9 = 320Hz
            // 파 = 미 * 16/15 = 341.3Hz
            // 솔 = 파 * 9/8 = 384Hz
            // 라 = 솔 * 10/9 = 426.6Hz
            // 시 = 라 * 9/8 = 480Hz
            // 도 = 시 * 16/15 = 512Hz (= 처음 도의 2배)
            // 2배 = 높은음, 1/2배 = 낮은음

            Beep(512, 300); // 도 0.3초
            Beep(640, 300); // 미 0.3초
            Beep(768, 300); // 솔 0.3초
        }
    }
}

using System;

using shine.libs.math;

namespace shine.libs.utils
{
    public class ThreadData
    {
        public int progress;
        public string message;
        public bool onProgress;
        public bool onMessage;
        public bool movingMode;
        public int timeDelayed;

        public krect rectDiffer, rectTablet, rectDesk;
        public int CapMode;

        public ThreadData()
        {
            progress = -1;
            message = null;
            timeDelayed = 0;

            onProgress = onMessage = false;
            movingMode = false;

            rectDiffer = new krect();
            rectTablet = new krect();
            rectDesk = new krect();
        }
    }
}
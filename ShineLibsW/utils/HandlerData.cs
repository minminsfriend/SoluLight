using System;

using shine.libs.math;

namespace shine.libs.utils
{
    public class HandlerData
    {
        public int progress;
        public string message;
        public bool onProgress;
        public bool onMessage;
        public bool movingMode;

        public krect rectDiffer, rectTablet, rectDesk;
        public int CapMode;

        public HandlerData()
        {
            progress = -1;
            message = null;

            onProgress = onMessage = false;
            movingMode = false;

            rectDiffer = new krect();
            rectTablet = new krect();
            rectDesk = new krect();
        }
    }
}
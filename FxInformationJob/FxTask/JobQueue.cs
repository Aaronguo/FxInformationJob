using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FxTask
{
    public class JobQueue
    {
        public static FxQueue<int> CarBuyJobLoadQueue = new FxQueue<int>();

        public static FxQueue<int> CarBuyJobPictureProcessQueue = new FxQueue<int>();
    }
}

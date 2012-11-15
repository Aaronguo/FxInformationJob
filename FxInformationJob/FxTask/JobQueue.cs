using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FxTask
{
    public class JobQueue
    {
        #region FxCar
        public static FxQueue<int> CarBuyJobLoadQueue = new FxQueue<int>();

        public static FxQueue<int> CarBuyJobPictureProcessQueue = new FxQueue<int>();

        public static FxQueue<int> CarTransferJobLoadQueue = new FxQueue<int>();

        public static FxQueue<int> CarTransferJobPictureProcessQueue = new FxQueue<int>();
        #endregion
        





    }
}

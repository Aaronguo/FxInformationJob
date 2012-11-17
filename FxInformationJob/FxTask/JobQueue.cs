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

        //public static FxQueue<int> CarBuyJobPictureProcessQueue = new FxQueue<int>();

        public static FxQueue<int> CarTransferJobLoadQueue = new FxQueue<int>();

        public static FxQueue<int> CarTransferJobPictureProcessQueue = new FxQueue<int>();
        #endregion



        #region FxGoods
        public static FxQueue<int> GoodsBuyJobLoadQueue = new FxQueue<int>();

        public static FxQueue<int> GoodsBuyJobPictureProcessQueue = new FxQueue<int>();

        public static FxQueue<int> GoodsTransferJobLoadQueue = new FxQueue<int>();

        public static FxQueue<int> GoodsTransferJobPictureProcessQueue = new FxQueue<int>();
        #endregion



        #region FxHouse
        public static FxQueue<int> HouseBuyJobLoadQueue = new FxQueue<int>();

        //public static FxQueue<int> HouseBuyJobPictureProcessQueue = new FxQueue<int>();

        public static FxQueue<int> HouseTransferJobLoadQueue = new FxQueue<int>();

        public static FxQueue<int> HouseTransferJobPictureProcessQueue = new FxQueue<int>();
        #endregion
    }
}

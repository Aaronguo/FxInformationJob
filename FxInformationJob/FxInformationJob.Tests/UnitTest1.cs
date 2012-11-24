using System;
using FxInformationJob.Tests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FxInformationJob.Tests
{
    [TestClass]
    public class UnitTest1 : BaseUnit
    {
        [TestMethod]
        public void RunAll()
        {
            RunGoodsTransfer();
            RunGoodsBuy();
            RunCarBuy();
            RunCarTransfer();
            RunHouseBuy();
            RunHouseTransfer();
        }


        [TestMethod]
        public void RunGoodsTransfer()
        {
            new FxTask.FxGoods.Transfer.GoodsTransferJobLoad().Execute();
        }

        [TestMethod]
        public void RunGoodsBuy()
        {
            new FxTask.FxGoods.Buy.GoodsBuyJobLoad().Execute();
        }

        [TestMethod]
        public void RunCarBuy()
        {
            new FxTask.FxCar.Buy.CarBuyJobLoad().Execute();
        }

        [TestMethod]
        public void RunCarTransfer()
        {
            new FxTask.FxCar.Transfer.CarTransferJobLoad().Execute();
        }

        [TestMethod]
        public void RunHouseBuy()
        {
            new FxTask.FxHouse.Buy.HouseBuyJobLoad().Execute();
        }

        [TestMethod]
        public void RunHouseTransfer()
        {
            new FxTask.FxHouse.Transfer.HouseTransferJobLoad().Execute();
        }
    }
}

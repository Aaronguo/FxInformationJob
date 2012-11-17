using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace FxTask
{
    public class QuartzLoadProvider
    {
        IScheduler scheduler;
        public void Load()
        {
            var schedFact = new StdSchedulerFactory();
            scheduler = schedFact.GetScheduler();
            InitScheduler();
            scheduler.Start();
        }

        protected void InitScheduler()
        {
            //AddJob<FxTask.KeepOwn.KeepOwn>(JobKey.KeepOwn, JobKey.KeepOwn + "Group", 10, 40);

            AddJob<FxTask.KeepYingTao.KeepYingTao>(JobKey.KeepYingTao, JobKey.KeepYingTao + "Group", 8, 40);


            AddJob<FxGoods.Buy.GoodsBuyJobLoad>(JobKey.GoodsBuyLoad, JobKey.GoodsBuyLoad + "Group", 1);//2分钟考虑100张图片压缩
            AddJob<FxGoods.Transfer.GoodsTransferJobLoad>(JobKey.GoodsTransferLoad, JobKey.GoodsTransferLoad + "Group", 1, 20);

            AddJob<FxCar.Buy.CarBuyJobLoad>(JobKey.CarBuyLoad, JobKey.CarBuyLoad + "Group", 1, 12);//2分钟考虑100张图片压缩
            AddJob<FxCar.Transfer.CarTransferJobLoad>(JobKey.CarTransferJobLoad, JobKey.CarTransferJobLoad + "Group", 1, 32);

            AddJob<FxHouse.Buy.HouseBuyJobLoad>(JobKey.HouseBuyLoad, JobKey.HouseBuyLoad + "Group", 1, 24);//2分钟考虑100张图片压缩
            AddJob<FxHouse.Transfer.HouseTransferJobLoad>(JobKey.HouseTransferLoad, JobKey.HouseTransferLoad + "Group", 1, 54);


            //AddJob<FxGoods.Buy.GoodsBuyJobLoad>(JobKey.GoodsBuyLoad, JobKey.GoodsBuyLoad + "Group", 1);
            //AddJob<FxGoods.Transfer.GoodsTransferJobLoad>(JobKey.GoodsTransferLoad, JobKey.GoodsTransferLoad + "Group", 1, 20);


            //AddJob<FxCar.Buy.CarBuyJobLoad>(JobKey.CarBuyLoad, JobKey.CarBuyLoad + "Group", 1);//2分钟考虑100张图片压缩
            //AddJob<FxCar.Transfer.CarTransferJobLoad>(JobKey.CarTransferJobLoad, JobKey.CarTransferJobLoad + "Group", 1);


            //AddJob<FxCar.Buy.CarBuyJobAuthorize>(JobKey.CarBuyJobAuthorize, JobKey.CarBuyJobAuthorize + "Group", 2, 20);
        }

        private void AddJob<T>(string name, string group, int minutes, int delaySeconds = 0) where T : IJob
        {
            DateTimeOffset offset = new DateTimeOffset(DateTime.UtcNow.AddSeconds(delaySeconds));
            IJobDetail job = JobBuilder.Create<T>()
               .WithIdentity(name, group)
               .Build();
            ITrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity(name, group)
                .StartAt(offset)
                .WithSimpleSchedule(r => r.WithIntervalInMinutes(minutes).RepeatForever())
                .Build();
            scheduler.ScheduleJob(job, trigger);
        }
    }
}

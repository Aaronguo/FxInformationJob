using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace FxTask
{
    public class QuartzLoadProvider //: StdSchedulerProvider
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
            AddJob<FxTask.KeepOwn>(JobKey.KeepOwn, JobKey.KeepOwn + "Group", 10, 40);

            AddJob<FxCar.Buy.CarBuyJobLoad>(JobKey.CarBuyLoad, JobKey.CarBuyLoad + "Group", 2);//2分钟考虑100张图片压缩
            AddJob<FxCar.Transfer.CarTransferJobLoad>(JobKey.CarTransferJobLoad, JobKey.CarTransferJobLoad + "Group", 2);


            AddJob<FxCar.Buy.CarBuyJobAuthorize>(JobKey.CarBuyJobAuthorize, JobKey.CarBuyJobAuthorize + "Group", 2, 20);
        }


        protected void CreateJob(IScheduler scheduler)
        {
            IJobDetail keepOwnJob = JobBuilder.Create<KeepOwn>()
             .WithIdentity("KeepOwnJob", "KeepOwnJobGroup")
             .Build();
            ITrigger keepOwnTrigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("KeepOwnTrigger", "KeepOwnTriggerGroup")
                .StartNow()
                .WithSimpleSchedule(r => r.WithIntervalInMinutes(1).RepeatForever())
                .Build();
            scheduler.ScheduleJob(keepOwnJob, keepOwnTrigger);
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
                .WithIdentity(name)
                .Build();
            scheduler.ScheduleJob(job, trigger);
        }

        private void AddJob<T>(int seconds, string name, string group, int delaySeconds = 0) where T : IJob
        {
            DateTimeOffset offset = new DateTimeOffset(DateTime.UtcNow.AddSeconds(delaySeconds));
            IJobDetail job = JobBuilder.Create<T>()
               .WithIdentity(name, group)
               .Build();
            ITrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity(name, group)
                  .StartAt(offset)
                .WithSimpleSchedule(r => r.WithIntervalInSeconds(seconds).RepeatForever())
                .Build();
            scheduler.ScheduleJob(job, trigger);
        }
    }
}

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
            AddJob<FxTask.KeepOwn>(JobKey.KeepOwn, JobKey.CarBuyLoad + "Group", 1);
            AddJob<FxCar.Buy.CarBuyLoad>(JobKey.CarBuyLoad, JobKey.CarBuyLoad + "Group", 1);
            AddJob<FxCar.Buy.CarBuyJobAuthorize>(30, JobKey.CarBuyJobAuthorize, JobKey.CarBuyJobAuthorize + "Group");
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


        private void AddJob<T>(string name, string group, int minutes) where T : IJob
        {
            IJobDetail job = JobBuilder.Create<T>()
               .WithIdentity(name, group)
               .Build();
            ITrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity(name, group)
                .StartNow()
                .WithSimpleSchedule(r => r.WithIntervalInMinutes(minutes).RepeatForever())
                .Build();
            scheduler.ScheduleJob(job, trigger);
        }

        private void AddJob<T>(int seconds, string name, string group) where T : IJob
        {
            IJobDetail job = JobBuilder.Create<T>()
               .WithIdentity(name, group)
               .Build();
            ITrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity(name, group)
                .StartNow()
                .WithSimpleSchedule(r => r.WithIntervalInSeconds(seconds).RepeatForever())
                .Build();
            scheduler.ScheduleJob(job, trigger);
        }
    }
}

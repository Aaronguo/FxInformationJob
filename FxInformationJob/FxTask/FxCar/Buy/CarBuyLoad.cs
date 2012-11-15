using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;
using Fx.Domain.FxCar;
using Fx.Entity;
using Fx.Infrastructure.Caching;
using FxTask.IService;
using Quartz;

namespace FxTask.FxCar.Buy
{
    public class CarBuyLoad : IInfoJobLoad, IJob
    {
        ICacheManager cacheService;
        AppSettings appSettings;
        public CarBuyLoad()
        {
            this.cacheService = DependencyResolver.Current.GetService<ICacheManager>(); ;
            this.appSettings = DependencyResolver.Current.GetService<AppSettings>();
        }
        public void LoadJob()
        {
            try
            {
                List<int> list = new List<int>(); ;
                using (var context = new FxCarContext())
                {
                    list = context.CarBuyInfos
                         .Where(r => r.InfoProcessState == (int)ProcessState.Commit).
                             Select(r => r.CarBuyInfoId).Take(20).ToList();
                }
                JobQueue.CarBuyJobLoadQueue.AddRange(list);
            }
            catch (Exception ex)
            {
                ex.LogEx("FxTask.FxCar.Buy.CarBuyLoad");
            }
        }

        public void Execute(IJobExecutionContext context)
        {
            if (!appSettings.TaskShutDown())
            {
                LoadJob();
            }
        }
    }
}

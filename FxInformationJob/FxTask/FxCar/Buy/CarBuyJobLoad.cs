using System;
using System.Collections.Generic;
using System.Linq;
using Fx.Domain.FxCar;
using Fx.Entity;
using FxTask.IService;

namespace FxTask.FxCar.Buy
{
    public class CarBuyJobLoad : JobBase, IInfoJobLoad
    {
        public CarBuyJobLoad()
        {
            this.JobKey = "FxTask.FxCar.Buy.CarBuyJobLoad";
        }
        public void LoadJob()
        {
            try
            {
                List<int> list = new List<int>();
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
                ex.LogEx(string.Format("{0} {1}", JobKey, "CarBuyJobLoad"));
            }
        }

        protected override void RunJobBusiness()
        {
            LoadJob();
        }

        protected override void JobCompleted()
        {
            base.JobCompleted();
            if (JobQueue.CarBuyJobLoadQueue.HasItem())
            {
                new CarBuyJobAuthorize().Execute();
            }
        }
    }
}

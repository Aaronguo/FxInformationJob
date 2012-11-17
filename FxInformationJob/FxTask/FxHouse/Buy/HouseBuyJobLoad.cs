using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fx.Domain.FxHouse;
using Fx.Entity;
using FxTask.IService;

namespace FxTask.FxHouse.Buy
{
    class HouseBuyJobLoad : JobBase, IInfoJobLoad
    {
        public HouseBuyJobLoad()
        {
            this.JobKey = "FxTask.FxHouse.Buy.HouseBuyJobLoad";
        }

        public void LoadJob()
        {
            try
            {
                List<int> list = new List<int>();
                using (var context = new FxHouseContext())
                {
                    list = context.HouseBuyInfos
                         .Where(r => r.InfoProcessState == (int)ProcessState.Commit).
                             Select(r => r.HouseBuyInfoId).Take(20).ToList();
                }
                JobQueue.HouseBuyJobLoadQueue.AddRange(list);
            }
            catch (Exception ex)
            {
                ex.LogEx(string.Format("{0} {1}", JobKey, "HouseBuyJobLoad"));
            }
        }

        protected override void RunJobBusiness()
        {
            LoadJob();
        }

        protected override void Completed()
        {
            if (JobQueue.HouseBuyJobLoadQueue.HasItem())
            {
                new HouseBuyJobAuthorize().Execute();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fx.Domain.FxHouse;
using Fx.Entity;
using FxTask.IService;

namespace FxTask.FxHouse.Transfer
{
    public class HouseTransferJobLoad : JobBase, IInfoJobLoad
    {
       public HouseTransferJobLoad()
        {
            this.JobKey = "FxTask.FxHouse.Transfer.HouseTransferJobLoad";
        }

        protected override void RunJobBusiness()
        {
            LoadJob();
        }

        protected override void Completed()
        {
            if (JobQueue.HouseTransferJobLoadQueue.HasItem())
            {
                new HouseTransferJobAuthorize().Execute();
            }
        }

        public void LoadJob()
        {
            try
            {
                List<int> list = new List<int>();
                using (var context = new FxHouseContext())
                {
                    list = context.HouseTransferInfos
                         .Where(r => r.InfoProcessState == (int)ProcessState.Commit).
                             Select(r => r.HouseTransferInfoId).Take(20).ToList();
                }
                JobQueue.HouseTransferJobLoadQueue.AddRange(list);
            }
            catch (Exception ex)
            {
                ex.LogEx(string.Format("{0} {1}", JobKey, "HouseBuyJobLoad"));
            }
        }
    }
}

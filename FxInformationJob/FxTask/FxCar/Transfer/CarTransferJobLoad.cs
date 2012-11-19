using System;
using System.Collections.Generic;
using System.Linq;
using Fx.Domain.FxCar;
using Fx.Entity;
using FxTask.IService;

namespace FxTask.FxCar.Transfer
{
    public class CarTransferJobLoad : JobBase, IInfoJobLoad
    {
        public CarTransferJobLoad()
        {
            this.JobKey = "FxTask.FxCar.Transfer.CarTransferJobLoad";
        }
        public void LoadJob()
        {
            try
            {
                List<int> list = new List<int>(); ;
                using (var context = new FxCarContext())
                {
                    list = context.CarTransferInfos
                         .Where(r => r.InfoProcessState == (int)ProcessState.Commit).
                             Select(r => r.CarTransferInfoId).Take(20).ToList();
                }
                JobQueue.CarTransferJobLoadQueue.AddRange(list);
            }
            catch (Exception ex)
            {
                ex.LogEx(string.Format("{0} {1}", JobKey, "LoadJob"));
            }
        }

        protected override void RunJobBusiness()
        {
            LoadJob();
        }

        protected override void JobCompleted()
        {
            base.JobCompleted();
            if (JobQueue.CarTransferJobLoadQueue.HasItem())
            {
                new CarTransferJobAuthorize().Execute();
            }
        }
    }
}

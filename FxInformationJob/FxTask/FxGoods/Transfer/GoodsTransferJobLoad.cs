using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fx.Domain.FxGoods;
using Fx.Entity;
using FxTask.IService;

namespace FxTask.FxGoods.Transfer
{
    public class GoodsTransferJobLoad : JobBase, IInfoJobLoad
    {
        public GoodsTransferJobLoad()
        {
            this.JobKey = "FxTask.FxGoods.Transfer.GoodsTransferJobLoad";
        }

        protected override void RunJobBusiness()
        {
            LoadJob();
        }

        protected override void JobCompleted()
        {
            base.JobCompleted();
            if (JobQueue.GoodsTransferJobLoadQueue.HasItem())
            {
                new GoodsTransferJobAuthorize().Execute();
            }
        }

        public void LoadJob()
        {
            try
            {
                List<int> list = new List<int>();
                using (var context = new FxGoodsContext())
                {
                    list = context.GoodsTransferInfos
                         .Where(r => r.InfoProcessState == (int)ProcessState.Commit).
                             Select(r => r.GoodsTransferInfoId).Take(20).ToList();
                }
                JobQueue.GoodsTransferJobLoadQueue.AddRange(list);
            }
            catch (Exception ex)
            {
                ex.LogEx(string.Format("{0} {1}", JobKey, "GoodsTransferJobLoad"));
            }
        }
    }
}

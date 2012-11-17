using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fx.Domain.FxGoods;
using Fx.Entity;
using FxTask.IService;

namespace FxTask.FxGoods.Buy
{
    class GoodsBuyJobLoad : JobBase, IInfoJobLoad
    {
        public GoodsBuyJobLoad()
        {
            this.JobKey = "FxTask.FxGoods.Buy.GoodsBuyJobLoad";
        }

        public void LoadJob()
        {
            try
            {
                List<int> list = new List<int>();
                using (var context = new FxGoodsContext())
                {
                    list = context.GoodsBuyInfos
                         .Where(r => r.InfoProcessState == (int)ProcessState.Commit).
                             Select(r => r.GoodsBuyInfoId).Take(20).ToList();
                }
                JobQueue.GoodsBuyJobLoadQueue.AddRange(list);
            }
            catch (Exception ex)
            {
                ex.LogEx(string.Format("{0} {1}", JobKey, "GoodsBuyJobLoad"));
            }
        }

        protected override void RunJobBusiness()
        {
            LoadJob();
        }

        protected override void Completed()
        {
            if (JobQueue.GoodsBuyJobLoadQueue.HasItem())
            {
                new GoodsBuyJobAuthorize().Execute();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fx.Domain.FxGoods;
using Fx.Domain.FxGoods.IService;
using FxTask.IService;

namespace FxTask.FxGoods.Buy
{
    public class GoodsBuyJobAuthorize : JobBase, IInfoJobAuthorize
    {
        Filter filter;
        IGoodsBuyJob goodsBuyJobService;
        public GoodsBuyJobAuthorize()
        {
            this.filter = DependencyResolver.Current.GetService<Filter>();
            this.goodsBuyJobService = DependencyResolver.Current.GetService<IGoodsBuyJob>();
            this.JobKey = "FxTask.FxGoods.Buy.GoodsBuyJobAuthorize";
        }

        public void Authorize()
        {
            int goodsId = JobQueue.GoodsBuyJobLoadQueue.GetItem(goodsBuyJobService.Authorizing);
            if (goodsId == 0)
            {
                return;
            }
            using (var context = new FxGoodsContext())
            {
                while (goodsId != 0)
                {
                    var goods = context.GoodsBuyInfos.Where(r => r.GoodsBuyInfoId == goodsId).
                            Select(r => new { r.Mark, r.PublishTitle,r.GoodsConditionMsg }).FirstOrDefault();
                    if (goods != null)
                    {
                        if (filter.FilterContent(goods.Mark).Success && 
                            filter.FilterContent(goods.PublishTitle).Success&&
                            filter.FilterContent(goods.GoodsConditionMsg).Success)
                        {
                            try
                            {
                                goodsBuyJobService.AuthorizeSuccess(goodsId);
                                JobQueue.GoodsBuyJobPictureProcessQueue.Add(goodsId);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "goodsBuyJobService.AuthorizeSuccess", goodsId));
                            }
                        }
                        else
                        {
                            try
                            {
                                string msg = "";
                                var filter1 = filter.FilterContent(goods.Mark);
                                var filter2 = filter.FilterContent(goods.PublishTitle);
                                var filter3 = filter.FilterContent(goods.GoodsConditionMsg);
                                if (!filter1.Success)
                                {
                                    msg += string.Format("你的帖子中包含了[[{0}]] 这个关键字", filter1.Key);
                                }
                                if (!filter2.Success)
                                {
                                    msg += string.Format("你的帖子中包含了[[{0}]] 这个关键字", filter2.Key);
                                }
                                if (!filter3.Success)
                                {
                                    msg += string.Format("你的帖子中包含了[[{0}]] 这个关键字", filter3.Key);
                                }
                                goodsBuyJobService.AuthorizeFaild(goodsId,msg);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "goodsBuyJobService.AuthorizeFaild", goodsId));
                            }
                        }
                        goodsId = JobQueue.GoodsBuyJobLoadQueue.GetItem(goodsBuyJobService.Authorizing);
                    }
                }
            }
        }

        protected override void RunJobBusiness()
        {
            Authorize();
        }

        protected override void JobCompleted()
        {
            base.JobCompleted();
            if (JobQueue.GoodsBuyJobPictureProcessQueue.HasItem())
            {
                new GoodsBuyJobPictureProcess().Execute();
            }
        }
    }
}

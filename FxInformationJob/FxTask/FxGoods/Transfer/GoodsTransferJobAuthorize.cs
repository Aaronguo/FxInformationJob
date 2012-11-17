using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fx.Domain.FxGoods;
using Fx.Domain.FxGoods.IService;
using FxTask.IService;

namespace FxTask.FxGoods.Transfer
{
    public class GoodsTransferJobAuthorize : JobBase, IInfoJobAuthorize
    {
        Filter filter;
        IGoodsTransferJob goodsTransferJobService;
        public GoodsTransferJobAuthorize()
        {
            this.filter = DependencyResolver.Current.GetService<Filter>();
            this.goodsTransferJobService = DependencyResolver.Current.GetService<IGoodsTransferJob>();
            this.JobKey = "FxTask.FxGoods.Transfer.GoodsTransferJobAuthorize";
        }

        public void Authorize()
        {
            int id = JobQueue.GoodsTransferJobLoadQueue.GetItem(goodsTransferJobService.Authorizing);
            if (id == 0)
            {
                return;
            }
            using (var context = new FxGoodsContext())
            {
                while (id != 0)
                {
                    var goods = context.GoodsTransferInfos.Where(r => r.GoodsTransferInfoId == id).
                            Select(r => new { r.Mark, r.PublishTitle,r.GoodsConditionMsg }).FirstOrDefault();
                    if (goods != null)
                    {
                        if (filter.FilterContent(goods.Mark).Success &&
                            filter.FilterContent(goods.PublishTitle).Success &&
                            filter.FilterContent(goods.GoodsConditionMsg).Success)
                        {
                            try
                            {
                                goodsTransferJobService.AuthorizeSuccess(id);
                                JobQueue.GoodsTransferJobPictureProcessQueue.Add(id);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "goodsTransferJobService.AuthorizeSuccess", id));
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
                                goodsTransferJobService.AuthorizeFaild(id, msg);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "goodsTransferJobService.AuthorizeFaild", id));
                            }
                        }
                        id = JobQueue.GoodsTransferJobLoadQueue.GetItem(goodsTransferJobService.Authorizing);
                    }
                }
            }
        }

        protected override void RunJobBusiness()
        {
            Authorize();
        }

        protected override void Completed()
        {
            new GoodsTransferJobPictureProcess().Execute();
        }
    }
}

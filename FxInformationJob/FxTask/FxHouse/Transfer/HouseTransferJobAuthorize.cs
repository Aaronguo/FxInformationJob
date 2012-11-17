using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fx.Domain.FxHouse;
using Fx.Domain.FxHouse.IService;
using FxTask.IService;

namespace FxTask.FxHouse.Transfer
{
    public class HouseTransferJobAuthorize : JobBase, IInfoJobAuthorize
    {
        Filter filter;
        IHouseTransferJob houseTransferJobService;
        public HouseTransferJobAuthorize()
        {
            this.filter = DependencyResolver.Current.GetService<Filter>();
            this.houseTransferJobService = DependencyResolver.Current.GetService<IHouseTransferJob>();
            this.JobKey = "FxTask.FxHouse.Transfer.HouseTransferJobAuthorize";
        }

        public void Authorize()
        {
            int houseId = JobQueue.HouseTransferJobLoadQueue.GetItem(houseTransferJobService.Authorizing);
            if (houseId == 0)
            {
                return;
            }
            using (var context = new FxHouseContext())
            {
                while (houseId != 0)
                {
                    var house = context.HouseTransferInfos.Where(r => r.HouseTransferInfoId == houseId).
                            Select(r => new { r.Mark, r.PublishTitle, r.PostCode, r.RoadName }).FirstOrDefault();
                    if (house != null)
                    {
                        if (filter.FilterContent(house.Mark).Success &&
                            filter.FilterContent(house.PublishTitle).Success &&
                            filter.FilterContent(house.PostCode).Success &&
                            filter.FilterContent(house.RoadName).Success)
                        {
                            try
                            {
                                houseTransferJobService.AuthorizeSuccess(houseId);
                                JobQueue.HouseTransferJobPictureProcessQueue.Add(houseId);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "houseTransferJobService.AuthorizeSuccess", houseId));
                            }
                        }
                        else
                        {
                            try
                            {
                                string msg = "";
                                var filter1 = filter.FilterContent(house.Mark);
                                var filter2 = filter.FilterContent(house.PublishTitle);
                                var filter3 = filter.FilterContent(house.PostCode);
                                var filter4 = filter.FilterContent(house.RoadName);
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
                                if (!filter4.Success)
                                {
                                    msg += string.Format("你的帖子中包含了[[{0}]] 这个关键字", filter4.Key);
                                }
                                houseTransferJobService.AuthorizeFaild(houseId, msg);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "houseTransferJobService.AuthorizeFaild", houseId));
                            }
                        }
                        houseId = JobQueue.HouseTransferJobLoadQueue.GetItem(houseTransferJobService.Authorizing);
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
            new HouseTransferJobPictureProcess().Execute();
        }
    }
}

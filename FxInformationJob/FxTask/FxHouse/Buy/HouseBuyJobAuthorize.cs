using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fx.Domain.FxHouse;
using Fx.Domain.FxHouse.IService;
using FxTask.IService;

namespace FxTask.FxHouse.Buy
{
    public class HouseBuyJobAuthorize : JobBase, IInfoJobAuthorize
    {
        Filter filter;
        IHouseBuyJob houseBuyJobService;
        public HouseBuyJobAuthorize()
        {
            this.filter = DependencyResolver.Current.GetService<Filter>();
            this.houseBuyJobService = DependencyResolver.Current.GetService<IHouseBuyJob>();
            this.JobKey = "FxTask.FxHouse.Buy.HouseBuyJobAuthorize";
        }

        public void Authorize()
        {
            int houseId = JobQueue.HouseBuyJobLoadQueue.GetItem(houseBuyJobService.Authorizing);
            if (houseId == 0)
            {
                return;
            }
            using (var context = new FxHouseContext())
            {
                while (houseId != 0)
                {
                    var house = context.HouseBuyInfos.Where(r => r.HouseBuyInfoId == houseId).
                            Select(r => new { r.Mark, r.PublishTitle }).FirstOrDefault();
                    if (house != null)
                    {
                        if (filter.FilterContent(house.Mark).Success && filter.FilterContent(house.PublishTitle).Success)
                        {
                            try
                            {
                                houseBuyJobService.AuthorizeSuccess(houseId);
                                houseBuyJobService.Publish(houseId);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "houseBuyJobService.AuthorizeSuccess", houseId));
                            }
                        }
                        else
                        {
                            try
                            {
                                string msg = "";
                                var filter1 = filter.FilterContent(house.Mark);
                                var filter2 = filter.FilterContent(house.PublishTitle);
                                if (!filter1.Success)
                                {
                                    msg += string.Format("你的帖子中包含了[[{0}]] 这个关键字", filter1.Key);
                                }
                                if (!filter2.Success)
                                {
                                    msg += string.Format("你的帖子中包含了[[{0}]] 这个关键字", filter2.Key);
                                }
                                houseBuyJobService.AuthorizeFaild(houseId,msg);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "houseBuyJobService.AuthorizeFaild", houseId));
                            }
                        }
                        houseId = JobQueue.HouseBuyJobLoadQueue.GetItem(houseBuyJobService.Authorizing);
                    }
                }
            }
        }

        protected override void RunJobBusiness()
        {
            Authorize();
        }
    }
}

using System;
using System.Linq;
using System.Web.Mvc;
using Fx.Domain.FxCar;
using Fx.Domain.FxCar.IService;
using FxTask.IService;

namespace FxTask.FxCar.Buy
{
    public class CarBuyJobAuthorize : JobBase, IInfoJobAuthorize
    {
        Filter filter;
        ICarBuyJob carBuyJobService;
        public CarBuyJobAuthorize()
        {
            this.filter = DependencyResolver.Current.GetService<Filter>();
            this.carBuyJobService = DependencyResolver.Current.GetService<ICarBuyJob>();
            this.JobKey = "FxTask.FxCar.Buy.CarBuyJobAuthorize";
        }
        public void Authorize()
        {
            int carId = JobQueue.CarBuyJobLoadQueue.GetItem(carBuyJobService.Authorizing);
            if (carId == 0)
            {
                return;
            }
            using (var context = new FxCarContext())
            {
                while (carId != 0)
                {                    
                    var car = context.CarBuyInfos.Where(r => r.CarBuyInfoId == carId).
                            Select(r => new { r.Mark, r.PublishTitle }).FirstOrDefault();
                    if (car != null)
                    {
                        if (filter.FilterContent(car.Mark).Success && filter.FilterContent(car.PublishTitle).Success)
                        {
                            try
                            {
                                carBuyJobService.AuthorizeSuccess(carId);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "carbuyjobService.AuthorizeSuccess ", carId));
                            }
                            try
                            {
                                carBuyJobService.Publish(carId);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "carbuyjobService.Publish ", carId));
                            }
                        }
                        else
                        {
                            try
                            {
                                string msg = "";
                                var filter1 = filter.FilterContent(car.Mark);
                                var filter2 = filter.FilterContent(car.PublishTitle);
                                if (!filter1.Success)
                                {
                                    msg += string.Format("你的帖子中包含了[[{0}]] 这个关键字", filter1.Key);
                                }
                                if (!filter2.Success)
                                {
                                    msg += string.Format("你的帖子中包含了[[{0}]] 这个关键字", filter2.Key);
                                }
                                carBuyJobService.AuthorizeFaild(carId,msg);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "carbuyjobService.AuthorizeFaild ", carId));
                            }
                        }
                        carId = JobQueue.CarBuyJobLoadQueue.GetItem(carBuyJobService.Authorizing);
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

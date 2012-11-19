using System;
using System.Linq;
using System.Web.Mvc;
using Fx.Domain.FxCar;
using Fx.Domain.FxCar.IService;
using FxTask.IService;

namespace FxTask.FxCar.Transfer
{
    public class CarTransferJobAuthorize : JobBase, IInfoJobAuthorize
    {
        Filter filter;
        ICarTransferJob carTransferJobService;
        public CarTransferJobAuthorize()
        {
            this.filter = DependencyResolver.Current.GetService<Filter>();
            this.carTransferJobService = DependencyResolver.Current.GetService<ICarTransferJob>();
            JobKey = "FxTask.FxCar.Transfer.CarTransferJobAuthorize ";
        }
        public void Authorize()
        {
            int carId = JobQueue.CarTransferJobLoadQueue.GetItem(carTransferJobService.Authorizing);
            if (carId == 0)
            {
                return;
            }
            using (var context = new FxCarContext())
            {
                while (carId != 0)
                {
                    var car = context.CarTransferInfos.Where(r => r.CarTransferInfoId == carId).
                            Select(r => new { r.Mark, r.PublishTitle }).FirstOrDefault();
                    if (car != null)
                    {
                        if (filter.FilterContent(car.Mark).Success && filter.FilterContent(car.PublishTitle).Success)
                        {
                            try
                            {
                                carTransferJobService.AuthorizeSuccess(carId);
                                JobQueue.CarTransferJobPictureProcessQueue.Add(carId);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "carTransferJobService.AuthorizeSuccess", carId));
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
                                carTransferJobService.AuthorizeFaild(carId, msg);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "carTransferJobService.AuthorizeFaild", carId));
                            }
                        }
                        carId = JobQueue.CarTransferJobLoadQueue.GetItem(carTransferJobService.Authorizing);
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
            if (JobQueue.CarTransferJobPictureProcessQueue.HasItem())
            {
                new CarTransferJobPictureProcess().Execute();
            }
        }
    }
}

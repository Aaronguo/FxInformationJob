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
            int id = JobQueue.CarTransferJobLoadQueue.GetItem();
            if (id == 0)
            {
                return;
            }
            using (var context = new FxCarContext())
            {
                while (id != 0)
                {
                    var car = context.CarTransferInfos.Where(r => r.CarTransferInfoId == id).
                            Select(r => new { r.Mark, r.PublishTitle }).FirstOrDefault();
                    if (car != null)
                    {
                        if (filter.FilterContent(car.Mark) && filter.FilterContent(car.PublishTitle))
                        {
                            try
                            {
                                carTransferJobService.AuthorizeSuccess(id);
                                JobQueue.CarTransferJobPictureProcessQueue.Add(id);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "carTransferJobService.AuthorizeSuccess", id));
                            }
                        }
                        else
                        {
                            try
                            {
                                carTransferJobService.AuthorizeFaild(id);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "carTransferJobService.AuthorizeFaild", id));
                            }
                        }
                        id = JobQueue.CarBuyJobLoadQueue.GetItem();
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
            if (JobQueue.CarTransferJobPictureProcessQueue.HasItem())
            {
                new CarTransferJobPictureProcess().Execute();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fx.Domain.FxCar;
using Fx.Domain.FxCar.IService;
using FxTask.IService;
using Quartz;

namespace FxTask.FxCar.Buy
{
    public class CarBuyJobAuthorize : IInfoJobAuthorize, IJob
    {
        Filter filter;
        ICarBuyJob carbuyjobService;
        AppSettings appSettings;
        public CarBuyJobAuthorize()
        {
            this.filter = DependencyResolver.Current.GetService<Filter>();
            this.carbuyjobService = DependencyResolver.Current.GetService<ICarBuyJob>();
            this.appSettings = DependencyResolver.Current.GetService<AppSettings>();
        }
        public void Authorize()
        {
            int id = JobQueue.CarBuyJobLoadQueue.GetItem();
            if (id == 0)
            {
                return;
            }
            using (var context = new FxCarContext())
            {
                while (id != 0)
                {
                    var car = context.CarBuyInfos.Where(r => r.CarBuyInfoId == id).
                            Select(r => new { r.Mark, r.PublishTitle }).FirstOrDefault();
                    if (car != null)
                    {
                        if (filter.FilterContent(car.Mark) && filter.FilterContent(car.PublishTitle))
                        {
                            try
                            {
                                carbuyjobService.AuthorizeSuccess(id);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx("FxTask.FxCar.Buy.CarBuyJobAuthorize carbuyjobService.AuthorizeSuccess(id)");
                            }
                            try
                            {
                                carbuyjobService.Publish(id);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx("FxTask.FxCar.Buy.CarBuyJobAuthorize carbuyjobService.Publish(id)");
                            }
                        }
                        else
                        {
                            try
                            {
                                carbuyjobService.AuthorizeFaild(id);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx("FxTask.FxCar.Buy.CarBuyJobAuthorize carbuyjobService.AuthorizeFaild(id)");
                            }
                        }
                        id = JobQueue.CarBuyJobLoadQueue.GetItem();
                    }
                }
            }
        }

        public void Execute(IJobExecutionContext context)
        {
            if (!appSettings.TaskShutDown())
            {
                Authorize();
            }
        }
    }
}

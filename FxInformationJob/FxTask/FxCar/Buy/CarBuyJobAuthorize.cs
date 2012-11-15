﻿using System;
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
                                carBuyJobService.AuthorizeSuccess(id);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "carbuyjobService.AuthorizeSuccess ", id));
                            }
                            try
                            {
                                carBuyJobService.Publish(id);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "carbuyjobService.Publish ", id));
                            }
                        }
                        else
                        {
                            try
                            {
                                carBuyJobService.AuthorizeFaild(id);
                            }
                            catch (Exception ex)
                            {
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "carbuyjobService.AuthorizeFaild ", id));
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
    }
}

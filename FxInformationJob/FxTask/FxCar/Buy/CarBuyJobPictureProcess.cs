using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FxTask.IService;
using Fx.Domain.FxCar;
using Quartz;
using System.Web.Mvc;

namespace FxTask.FxCar.Buy
{
    /// <summary>
    /// 后续的因为没有图片 不启用
    /// </summary>
    public class CarBuyJobPictureProcess : IInfoJobPictureProcess,IJob
    {
        AppSettings appSettings;
        public CarBuyJobPictureProcess()
        {
            this.appSettings = DependencyResolver.Current.GetService<AppSettings>();
        }
        public void PictureProcess()
        {

            //int id = JobQueue.CarBuyJobPictureProcessQueue.GetItem();
            //if (id != 0)
            //{
            //    using (var context = new FxCarContext())
            //    {
            //        var car = context.CarBuyInfos.Where(r => r.CarBuyInfoId == id).FirstOrDefault();
            //        if (car != null)
            //        {
                        
            //        }
            //    }
            //}   
        }

        public void Execute(IJobExecutionContext context)
        {
            if (!appSettings.TaskShutDown())
            {
                PictureProcess();
            }
        }
    }
}


 //ImageJob job = new ImageJob(Request.Files[upload].InputStream,
//"~/images/"+new Random().Next(1000000)+".jpg", 
//new ResizeSettings("maxwidth=500&crop=auto"));
 //                   ImageBuilder.Current.Build(job);
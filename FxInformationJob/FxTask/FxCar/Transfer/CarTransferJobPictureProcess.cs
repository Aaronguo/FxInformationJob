using System;
using System.Linq;
using System.Web.Mvc;
using Fx.Domain.FxCar;
using Fx.Domain.FxCar.IService;
using FxTask.IService;
using ImageResizer;

namespace FxTask.FxCar.Transfer
{
    public class CarTransferJobPictureProcess : JobBase, IInfoJobPictureProcess
    {
        ICarTransferJob carTransferJobService;
        public CarTransferJobPictureProcess()
        {
            this.JobKey = "FxTask.FxCar.Transfer.CarTransferJobPictureProcess";
            this.carTransferJobService = DependencyResolver.Current.GetService<ICarTransferJob>();
        }

        protected override void RunJobBusiness()
        {
            PictureProcess();
        }

        public void PictureProcess()
        {
            int carId = JobQueue.CarTransferJobPictureProcessQueue.GetItem(carTransferJobService.PictrueCdning);
            if (carId == 0)
            {
                return;
            }
            using (var context = new FxCarContext())
            {
                while (carId != 0)
                {
                    var car = context.CarTransferInfos.Where(r => r.CarTransferInfoId == carId).FirstOrDefault();
                    if (car != null)
                    {
                        string source;
                        string destnation;
                        string destnationmin;
                        int error = 0;
                        string errmsg = "";
                        foreach (var picture in car.Pictures)
                        {
                            try
                            {
                                source = string.Format(picture.PhysicalPath);
                                destnation = string.Format("{0}{1}", appSettings.CdnPath(), picture.ImageUrl.Replace(@"/", @"\"));
                                destnationmin = string.Format("{0}{1}", appSettings.CdnPath(), picture.MinImageUrl.Replace(@"/", @"\"));
                                var job = new ImageJob(source, destnation, new ResizeSettings()
                                {
                                    MaxHeight = 500,
                                    MaxWidth = 500,
                                }) { CreateParentDirectory = true };
                                var jobmin = new ImageJob(source, destnationmin, new ResizeSettings()
                                {
                                    MaxHeight = 64,
                                    MaxWidth = 64,
                                }) { CreateParentDirectory = true };
                                ImageBuilder.Current.Build(job);
                                ImageBuilder.Current.Build(jobmin);
                            }
                            catch (Exception ex)
                            {
                                if (string.IsNullOrEmpty(errmsg))
                                {
                                    if (ex.InnerException != null)
                                    {
                                        errmsg = ex.InnerException.Message;
                                    }
                                    else
                                    {
                                        errmsg = ex.Message;
                                    }
                                }
                                error++;
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "PictureProcess", picture.TransferPictureId));
                            }
                        }
                        if (error == 0)
                        {
                            carTransferJobService.PictrueCdnSuccessd(carId);
                            carTransferJobService.Publish(carId);
                        }
                        else
                        {
                            carTransferJobService.PictrueCdnFailed(carId, errmsg);
                        }
                        carId = JobQueue.CarTransferJobPictureProcessQueue.GetItem(carTransferJobService.PictrueCdning);
                    }
                }
            }
        }
    }
}


//            ImageJob job = new ImageJob(Request.Files[upload].InputStream, "~/images/"+new Random().Next(1000000)+".jpg", new ResizeSettings("maxwidth=500&crop=auto"));
//            ImageBuilder.Current.Build(job);
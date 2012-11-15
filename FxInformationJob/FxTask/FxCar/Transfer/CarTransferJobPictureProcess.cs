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
            int id = JobQueue.CarTransferJobPictureProcessQueue.GetItem();
            if (id == 0)
            {
                return;
            }
            using (var context = new FxCarContext())
            {
                while (id != 0)
                {
                    var car = context.CarTransferInfos.Where(r => r.CarTransferInfoId == id).FirstOrDefault();
                    if (car != null)
                    {
                        string source;
                        string destnation;
                        int error = 0;
                        string errmsg = "";
                        foreach (var picture in car.Pictures)
                        {
                            try
                            {
                                source = string.Format(picture.PhysicalPath);
                                destnation = string.Format("{0}{1}", appSettings.CdnPath(), picture.ImageUrl.Replace(@"/", @"\"));
                                var job = new ImageJob(source, destnation, new ResizeSettings()
                                {
                                    MaxHeight = 500,
                                    MaxWidth = 500,
                                }) { CreateParentDirectory = true };
                                ImageBuilder.Current.Build(job);                                
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
                            carTransferJobService.PictrueCdnSuccessd(id);
                            carTransferJobService.Publish(id);
                        }
                        else
                        {
                            carTransferJobService.PictrueCdnFailed(id, errmsg);
                        }
                        id = JobQueue.CarTransferJobPictureProcessQueue.GetItem();
                    }
                }
            }
        }

        protected override void Completed()
        {
            base.Completed();
        }
    }
}


//            ImageJob job = new ImageJob(Request.Files[upload].InputStream, "~/images/"+new Random().Next(1000000)+".jpg", new ResizeSettings("maxwidth=500&crop=auto"));
//            ImageBuilder.Current.Build(job);

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fx.Domain.FxHouse;
using Fx.Domain.FxHouse.IService;
using FxTask.IService;
using ImageResizer;

namespace FxTask.FxHouse.Transfer
{
    public class HouseTransferJobPictureProcess : JobBase, IInfoJobPictureProcess
    {
        IHouseTransferJob houseTransferJobService;
        public HouseTransferJobPictureProcess()
        {
            this.JobKey = "FxTask.FxHouse.Transfer.HouseTransferJobPictureProcess";
            this.houseTransferJobService = DependencyResolver.Current.GetService<IHouseTransferJob>();
        }

        protected override void RunJobBusiness()
        {
            PictureProcess();
        }

        public void PictureProcess()
        {
            int houseId = JobQueue.HouseTransferJobPictureProcessQueue.GetItem(houseTransferJobService.PictrueCdning);
            if (houseId == 0)
            {
                return;
            }
            using (var context = new FxHouseContext())
            {
                while (houseId != 0)
                {
                    var car = context.HouseTransferInfos.Where(r => r.HouseTransferInfoId == houseId).FirstOrDefault();
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
                            houseTransferJobService.PictrueCdnSuccessd(houseId);
                            houseTransferJobService.Publish(houseId);
                        }
                        else
                        {
                            houseTransferJobService.PictrueCdnFailed(houseId, errmsg);
                        }
                        houseId = JobQueue.HouseTransferJobPictureProcessQueue.GetItem(houseTransferJobService.PictrueCdning);
                    }
                }
            }
        }
    }
}

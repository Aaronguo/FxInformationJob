using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fx.Domain.FxGoods;
using Fx.Domain.FxGoods.IService;
using FxTask.IService;
using ImageResizer;

namespace FxTask.FxGoods.Transfer
{
    public class GoodsTransferJobPictureProcess : JobBase, IInfoJobPictureProcess
    {
        IGoodsTransferJob goodsTransferJobService;
        public GoodsTransferJobPictureProcess()
        {
            this.JobKey = "FxTask.FxGoods.Transfer.GoodsTransferJobPictureProcess";
            this.goodsTransferJobService = DependencyResolver.Current.GetService<IGoodsTransferJob>();
        }

        protected override void RunJobBusiness()
        {
            PictureProcess();
        }

        public void PictureProcess()
        {
            int goodsId = JobQueue.GoodsTransferJobPictureProcessQueue.GetItem(goodsTransferJobService.PictrueCdning);
            if (goodsId == 0)
            {
                return;
            }
            using (var context = new FxGoodsContext())
            {
                while (goodsId != 0)
                {
                    var car = context.GoodsTransferInfos.Where(r => r.GoodsTransferInfoId == goodsId).FirstOrDefault();
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
                            goodsTransferJobService.PictrueCdnSuccessd(goodsId);
                            goodsTransferJobService.Publish(goodsId);
                        }
                        else
                        {
                            goodsTransferJobService.PictrueCdnFailed(goodsId, errmsg);
                        }
                        goodsId = JobQueue.GoodsTransferJobPictureProcessQueue.GetItem(goodsTransferJobService.PictrueCdning);
                    }
                }
            }
        }

        protected override void Completed()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fx.Domain.FxGoods;
using Fx.Domain.FxGoods.IService;
using FxTask.IService;
using ImageResizer;

namespace FxTask.FxGoods.Buy
{
    public class GoodsBuyJobPictureProcess : JobBase, IInfoJobPictureProcess
    {
        IGoodsBuyJob goodsBuyJobService;
        public GoodsBuyJobPictureProcess()
        {
            this.JobKey = "FxTask.FxGoods.Buy.GoodsBuyJobPictureProcess";
            this.goodsBuyJobService = DependencyResolver.Current.GetService<IGoodsBuyJob>();
        }

        protected override void RunJobBusiness()
        {
            PictureProcess();
        }

        public void PictureProcess()
        {
            int goodsId = JobQueue.GoodsBuyJobPictureProcessQueue.GetItem(goodsBuyJobService.PictrueCdning);
            if (goodsId == 0)
            {
                return;
            }
            using (var context = new FxGoodsContext())
            {
                while (goodsId != 0)
                {
                    var car = context.GoodsBuyInfos.Where(r => r.GoodsBuyInfoId == goodsId).FirstOrDefault();
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
                                ex.LogEx(string.Format("{0} {1} {2}", JobKey, "PictureProcess", picture.BuyPictureId));
                            }
                        }
                        if (error == 0)
                        {
                            goodsBuyJobService.PictrueCdnSuccessd(goodsId);
                            goodsBuyJobService.Publish(goodsId);
                        }
                        else
                        {
                            goodsBuyJobService.PictrueCdnFailed(goodsId, errmsg);
                        }
                        goodsId = JobQueue.GoodsBuyJobPictureProcessQueue.GetItem(goodsBuyJobService.PictrueCdning);
                    }
                }
            }
        }
    }
}

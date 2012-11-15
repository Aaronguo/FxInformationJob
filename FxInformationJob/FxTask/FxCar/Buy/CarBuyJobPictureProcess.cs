using System;
using FxTask.IService;

namespace FxTask.FxCar.Buy
{
    /// <summary>
    /// 后续的因为没有图片 不启用
    /// </summary>
    public class CarBuyJobPictureProcess : JobBase, IInfoJobPictureProcess
    {
        protected override void RunJobBusiness()
        {
            PictureProcess();
        }

        public void PictureProcess()
        {
            throw new NotImplementedException();
        }
    }
}
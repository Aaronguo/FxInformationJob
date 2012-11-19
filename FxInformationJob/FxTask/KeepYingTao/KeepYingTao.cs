using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FxTask.KeepYingTao
{
    public class KeepYingTao : JobBase
    {
        public KeepYingTao()
        {
            this.JobKey = "FxTask.KeepYingTao.KeepYingTao";
        }
        protected override void RunJobBusiness()
        {
            if (!appSettings.TaskShutDown() && !string.IsNullOrEmpty(appSettings.OwnSiteUrl()))
            {
                try
                {                   
                    ChenHttp.HttpHelper http = new ChenHttp.HttpHelper();
                    var res = http.GetRequest(appSettings.YingTaoUrl());                    
                }
                catch (Exception ex)
                {
                    ex.LogEx("FxTask.KeepYingTao");
                }
            }
        }
    }
}

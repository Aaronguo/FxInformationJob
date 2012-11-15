using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Quartz;

namespace FxTask
{
    public class KeepOwn : IJob
    {
        AppSettings appSetting;
        public KeepOwn()
        {
            this.appSetting = DependencyResolver.Current.GetService<AppSettings>();
        }

        public void Execute(IJobExecutionContext context)
        {
            if (!appSetting.TaskShutDown() && !string.IsNullOrEmpty(appSetting.OwnSiteUrl()))
            {                
                try
                {
                    LogWriter logWriter = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
                    ChenHttp.HttpHelper http = new ChenHttp.HttpHelper();
                    var res = http.GetRequest(appSetting.OwnSiteUrl());
                    StreamWriter sw = new StreamWriter(@"D:\Temp\quartz.txt", true);
                    sw.WriteLine(DateTime.Now.ToString());
                    sw.Flush();
                    sw.Close();                   
                }
                catch (Exception ex)
                {
                    ex.LogEx("FxTask.KeepOwn");                    
                }
            }
        }
    }
}

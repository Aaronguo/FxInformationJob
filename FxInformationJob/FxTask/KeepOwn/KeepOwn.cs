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

namespace FxTask.KeepOwn
{
    public class KeepOwn : JobBase
    {
        protected override void RunJobBusiness()
        {
            if (!appSettings.TaskShutDown() && !string.IsNullOrEmpty(appSettings.OwnSiteUrl()))
            {
                try
                {
                    LogWriter logWriter = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
                    ChenHttp.HttpHelper http = new ChenHttp.HttpHelper();
                    var res = http.GetRequest(appSettings.OwnSiteUrl());
                    StreamWriter sw = new StreamWriter(@"D:\JobTxt\quartz.txt", true);
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

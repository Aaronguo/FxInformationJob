using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fx.Infrastructure.Caching;
using Quartz;

namespace FxTask
{
    public class JobBase : IJob
    {
        protected string JobKey = "JobBase";
        protected ICacheManager cacheService;
        protected AppSettings appSettings;
        public JobBase()
        {
            this.cacheService = DependencyResolver.Current.GetService<ICacheManager>(); ;
            this.appSettings = DependencyResolver.Current.GetService<AppSettings>();
        }


        public void Execute(IJobExecutionContext context = null)
        {
            if (!appSettings.TaskShutDown())
            {
                RunJobBusiness();
                Completed();
            }
        }

        protected virtual void RunJobBusiness()
        {

        }

        protected virtual void Completed()
        {

        }
    }
}

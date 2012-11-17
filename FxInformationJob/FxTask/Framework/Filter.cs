using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fx.Infrastructure.Caching;

namespace FxTask
{
    public class Filter
    {
        ICacheManager cacheService;
        AppSettings appSetting;
        public Filter(ICacheManager cacheService, AppSettings appSetting)
        {
            this.cacheService = cacheService;
            this.appSetting = appSetting;
        }

        public FilterResult FilterContent(string content)
        {
            foreach (var item in GetFilterKeys())
            {
                if (!string.IsNullOrEmpty(content) && content.Contains(item))
                {
                    return new FilterResult(item);
                }
            }
            return new FilterResult();
        }

        public List<string> GetFilterKeys()
        {
            return appSetting.FilterKeys();
        }
    }



}

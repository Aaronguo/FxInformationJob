using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Fx.Infrastructure.Caching;

namespace FxTask
{
    public class AppSettings
    {
        ICacheManager cacheService;
        public AppSettings(ICacheManager cacheService)
        {
            this.cacheService = cacheService;
        }


        #region Private Methods

        private  string GetValue(string Key)
        {
            string Value = ConfigurationManager.AppSettings[Key];
            if (!string.IsNullOrEmpty(Value))
            {
                return Value;
            }
            return string.Empty;
        }

        private  string GetString(string Key, string DefaultValue)
        {
            string Setting = GetValue(Key);
            if (!string.IsNullOrEmpty(Setting))
            {
                return Setting;
            }
            return DefaultValue;
        }

        private  bool GetBool(string Key, bool DefaultValue)
        {
            string Setting = GetValue(Key);
            if (!string.IsNullOrEmpty(Setting))
            {
                switch (Setting.ToLower())
                {
                    case "false":
                    case "0":
                    case "n":
                        return false;
                    case "true":
                    case "1":
                    case "y":
                        return true;
                }
            }
            return DefaultValue;
        }

        private  int GetInt(string Key, int DefaultValue)
        {
            string Setting = GetValue(Key);
            if (!string.IsNullOrEmpty(Setting))
            {
                int i;
                if (int.TryParse(Setting, out i))
                {
                    return i;
                }
            }
            return DefaultValue;
        }

        private  double GetDouble(string Key, double DefaultValue)
        {
            string Setting = GetValue(Key);
            if (!string.IsNullOrEmpty(Setting))
            {
                double d;
                if (double.TryParse(Setting, out d))
                {
                    return d;
                }
            }
            return DefaultValue;
        }

        private  byte GetByte(string Key, byte DefaultValue)
        {
            string Setting = GetValue(Key);
            if (!string.IsNullOrEmpty(Setting))
            {
                byte b;
                if (byte.TryParse(Setting, out b))
                {
                    return b;
                }
            }
            return DefaultValue;
        }

        #endregion

        #region Public Properties

        public bool TaskShutDown()
        {
            if (cacheService.Get("TaskShutDown") == null)
            {
                cacheService.Insert("TaskShutDown", GetBool("TaskShutDown", true), 3600, System.Web.Caching.CacheItemPriority.Default);
            }
            object o = cacheService.Get("TaskShutDown");
            if (o != null)
            {
                return Convert.ToBoolean(o);
            }
            return true;
        }

        public List<string> FilterKeys()
        {
            if (cacheService.Get("InfoFilterKeys") == null)
            {
                string keys = GetString("InfoFilterKeys", "");
                List<string> list = keys.Split(',').ToList();
                cacheService.Insert("InfoFilterKeys", list, 3600, System.Web.Caching.CacheItemPriority.Default);
            }
            object o = cacheService.Get("InfoFilterKeys");
            if (o != null)
            {
                return o as List<string>;
            }
            return new List<string>();
        }


        public string OwnSiteUrl()
        {
            if (cacheService.Get("OwnSiteUrl") == null)
            {
                cacheService.Insert("OwnSiteUrl", GetString("OwnSiteUrl", ""), 3600, System.Web.Caching.CacheItemPriority.Default);
            }
            object o = cacheService.Get("OwnSiteUrl");
            if (o != null)
            {
                return o.ToString();
            }
            return "";
        }


        public string YingTaoUrl()
        {
            if (cacheService.Get("YingTaoUrl") == null)
            {
                cacheService.Insert("YingTaoUrl", GetString("YingTaoUrl", "http://yingtao.co.uk"), 3600, System.Web.Caching.CacheItemPriority.Default);
            }
            object o = cacheService.Get("YingTaoUrl");
            if (o != null)
            {
                return o.ToString();
            }
            return "";
        }

        #endregion



    }
}
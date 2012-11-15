using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace FxTask
{
    public static class Logging
    {
        public static LogWriter logWriter = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();

        public static void LogEx(this Exception ex, string msg = null)
        {
            try
            {

                logWriter.Write(ex, msg);//注释中不能包含“--”，并且“-”不能是最后一个字符 配置 文件多了一个- 导致错误。。
            }
            catch (Exception)
            {
                //Logging
                throw ex;
            }
        }
    }
}

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
                logWriter.Write(ex, msg);
            }
            catch (Exception)
            {
                //Logging
                throw ex;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FxInformationJob.App_Start;

namespace FxInformationJob.Tests.Framework
{
    public class BaseUnit
    {
        public BaseUnit()
        {
            SimpleInjectorInitializer.Initialize();
        }
    }
}

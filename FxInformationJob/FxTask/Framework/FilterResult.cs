using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FxTask
{
    public class FilterResult
    {
        public bool Success { get; set; }

        public string Key { get; set; }

        public FilterResult()
        {
            this.Success = true;
        }

        public FilterResult(string key)
        {
            this.Success = false;
            this.Key = key;
        }
    }
}

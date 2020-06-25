using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitViewFilters
{
    public enum ResultType { ok, warning, cancel, error}
    public class MyResult
    {
        public ResultType ResultType;
        public string Message;

        public MyResult(ResultType Result, string Message)
        {
            ResultType = Result;
            this.Message = Message;
        }

    }
}

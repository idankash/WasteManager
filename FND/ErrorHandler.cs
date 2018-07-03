using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FND
{
    public static class ErrorHandler
    {
        public static Exception Handle(Exception ex, object sender)
        {
            Logger.Instance.WriteError(ex, sender);
            return ex;
        }
    }
}

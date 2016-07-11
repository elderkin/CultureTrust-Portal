using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal11.ErrorLog
{
    public class PortalExceptions
    {
    }
    public class InternalErrorException : Exception 
    {
        public InternalErrorException(string message) : base(message) { }
    }

    public class QueryStringException : Exception
    {
        public QueryStringException(string message) : base(message) { }
    }
    
}
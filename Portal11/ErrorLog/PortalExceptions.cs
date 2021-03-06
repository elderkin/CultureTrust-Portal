﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal11.ErrorLog
{
    public class PortalExceptions
    {
    }
    public class Application_ErrorException : Exception
    {
        public Application_ErrorException(string message) : base(message) { }
    }

    public class ApplicationStartException : Exception 
    {
        public ApplicationStartException() : base() { }
    }

    public class EmailErrorException : Exception
    {
        public EmailErrorException(string message) : base(message) { }
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
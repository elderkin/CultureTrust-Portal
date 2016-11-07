using System;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace Portal11.Logic
{
    public class ExceptionActions
    {

        // See if the current exception is caused by a Duplicate Key error.

        public static bool IsDuplicateKeyException(Exception ex)
        {
            if (ex is DbUpdateException)                            // If is this exception is a Database Update exception
            {
                if (ex.InnerException is UpdateException)           // If is still looks like a Database Update exception
                {
                    if (ex.InnerException.InnerException is SqlException) // If is still looks like a Database Update exception
                    {
                        SqlException inner = (SqlException)ex.InnerException.InnerException;
                        if (inner.Number == 2601)                   // If == it is a Duplicate Key error
                        {
                            return true;                            // Tell caller it's a hit
                        }
                    }
                }
            }
            return false;                                           // Else tell caller it's a miss
        }
    }
}
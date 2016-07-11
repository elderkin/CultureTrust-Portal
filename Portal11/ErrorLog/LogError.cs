﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Elmah;
using Portal11.ErrorLog;
using Portal11.Models;
using System.Data.Entity.Validation;
using System.Data;

namespace Portal11.ErrorLog
{
    public class LogError : System.Web.UI.Page
    {

        // Come here when we're in a try block updating the database and something goes awry. In this case, we have a
        // live exception to report, which we do via elmah. Then we throw up a panicky error page that's specific to a database error.

        public static void LogDatabaseError(Exception ex, string pageName, string errorMessage)
        {
            try
            {
                string secondaryError = "";                             // Start with no secondary error message      
                if (ex is DbEntityValidationException)                  // If is error is a validation exception. Interpret details
                {

                    // The Exception is a DbEntityValidation Exception - a new database row failed a validation check on its way
                    // into the database. This Exception contains a lot of detail, which we display in the annotated Exception

                    DbEntityValidationException dbEx = (DbEntityValidationException)ex; // Cast the vanilla exception as a special exception
                    foreach (var validationErrors in dbEx.EntityValidationErrors) // Process the errors in turn
                    {
                        foreach (var validationError in validationErrors.ValidationErrors) // Boxes inside boxes
                        {
                            secondaryError = secondaryError + string.Format("; Property: '{0}' Error: '{1}' ",
                                        validationError.PropertyName,
                                        validationError.ErrorMessage);  // Build up the secondary error message with validation error details
                        }
                    }
                }
                else if (ex.InnerException is DbEntityValidationException) // A validation exception is buried. Interpret as above
                {
                    DbEntityValidationException dbEx = (DbEntityValidationException)ex.InnerException; // Cast as a special exception
                    foreach (var validationErrors in dbEx.EntityValidationErrors) // Process the errors in turn
                    {
                        foreach (var validationError in validationErrors.ValidationErrors) // Boxes inside boxes
                        {
                            secondaryError = secondaryError + string.Format("; Property: '{0}' Error: '{1}' ",
                                        validationError.PropertyName,
                                        validationError.ErrorMessage);  // Build up the secondary error message with validation error details
                        }
                    }
                }
                else if (ex is System.Data.Entity.Infrastructure.DbUpdateException) // A conversion exception is buried.
                {
                    if (ex.InnerException is System.Data.Entity.Core.UpdateException) // It is an error updating the database
                    {
                        System.Data.Entity.Core.UpdateException dbEx = (System.Data.Entity.Core.UpdateException)ex.InnerException; // Cast as database update
                        if (dbEx.InnerException is System.Data.SqlClient.SqlException) // It is an error from SQL
                        {
                            System.Data.SqlClient.SqlException dbEx2 = (System.Data.SqlClient.SqlException)dbEx.InnerException; // Cast as SQL Exception
                            secondaryError = secondaryError + dbEx2.Message;    // Build up secondary error message
                        }
                    }
                }
 
                // Format a detailed error message and log to Elmah

                string msg = "Page '" + pageName + "' experienced database error: " + errorMessage + "." + secondaryError; // Formulate error message
                var annotatedException = new Exception(msg, ex);        // Include context information from the caller with the exception
                ErrorSignal.FromCurrentContext().Raise(annotatedException); // Ask elmah to log this error with the annotation            
                
                HttpContext.Current.Server.Transfer("~/ErrorLog/DatabaseError.aspx?" + PortalConstants.QSErrorText + "=" + msg + "&" +
                                                        PortalConstants.QSPageName + "=" + pageName);
            }
            catch (Exception)
            {
                // uh oh! just keep going
            }
        }

        // Come here when a page detects an internal logic error not associated with a Query String parameter. Examples are a GridView row
        // that does not include a valid ID value, a database lookup of an ID value that comes up empty. Tell elmah to log it, then 
        // report the error to the user via the Error page.

        public static void LogInternalError(string pageName, string errorMessage)
        {
            try
            {
                // log error to Elmah

                string msg = "Page '" + pageName + "' experienced internal error: " + errorMessage; // Formulate error message
                ErrorSignal.FromCurrentContext().Raise(new ErrorLog.InternalErrorException(msg)); // Ask elmah to log this error

                HttpContext.Current.Server.Transfer("~/ErrorLog/FatalError.aspx?" + PortalConstants.QSErrorText + "=" + errorMessage + "&" +
                                                        PortalConstants.QSPageName + "=" + pageName);
            }
            catch (Exception)
            {
                // uh oh! just keep going
            }
        }
        public static void LogInternalException(Exception ex)
        {
            try
            {
                // log error to Elmah

                string msg = "Application_Error caught internal error"; // Formulate error message
                ErrorSignal.FromCurrentContext().Raise(new ErrorLog.InternalErrorException(msg)); // Ask elmah to log this error

                HttpContext.Current.Server.Transfer("~/ErrorLog/FatalError.aspx?" + PortalConstants.QSErrorText + "=" + msg + "&" +
                                                        PortalConstants.QSPageName + "=Application_Error");
            }
            catch (Exception)
            {
                // uh oh! just keep going
            }
        }

        // Come here when a page detects an internal logic error with a Query String parameter. Tell elmah to log it, then 
        // report the error to the user via the Error page.

        public static void LogQueryStringError(string pageName, string errorMessage)
        {
            try
            {
                // log error to Elmah

                string msg = "Page '" + pageName + "' experienced Query String error: " + errorMessage; // Formulate error message
                ErrorSignal.FromCurrentContext().Raise(new ErrorLog.QueryStringException(msg)); // Ask elmah to log this error
                
                HttpContext.Current.Server.Transfer("~/ErrorLog/FatalError.aspx?" + PortalConstants.QSErrorText + "=" + errorMessage + "&" +
                                                        PortalConstants.QSPageName + "=" + pageName);
            }
            catch (Exception)
            {
                // uh oh! just keep going
            }
        }

        // Come here when a page detects an error processing Supporting Documents. We survive, i.e., continue processing after, these
        // errors. So tell elmah to log it, but then return.

        public static void LogSupportingDocumentError(Exception ex, string pageName, string errorMessage)
        {
            try
            {
                // log error to Elmah

                string msg = "Page '" + pageName + "' experienced Supporting Document error: " + errorMessage; // Formulate error message
                var annotatedException = new Exception(msg, ex);        // Include context information from the caller with the exception
                ErrorSignal.FromCurrentContext().Raise(annotatedException); // Ask elmah to log this error with the annotation            

                HttpContext.Current.Server.Transfer("~/ErrorLog/FatalError.aspx?" + PortalConstants.QSErrorText + "=" + errorMessage + "&" +
                                                        PortalConstants.QSPageName + "=" + pageName);
            }
            catch (Exception)
            {
                // uh oh! just keep going
            }
        }

    }
}
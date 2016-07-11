using Portal11.ErrorLog;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal11.Logic
{
    public class QueryStringActions
    {

        // Carefully process the Command, returning a string. If missing, that's an error

        public static string GetCommand()
        {
            string val = HttpContext.Current.Request.QueryString[PortalConstants.QSCommand]; // Fetch the Command, if it is present
            if (val == null || val == "")                                // If == the Query String is missing
            {
                LogError.LogQueryStringError("GetCommand", "Unable to find Query String in search of Command"); // Fatal error
            }
            return val;                                                 // Return Command to caller
        }


//        public static QSValue GetDepID()
//        {
//            return GetEDID(PortalConstants.QSDepID);                    // Fetch the Deposit ID and convert it
//       }

 
//        public static QSValue GetExpID()
//        {
//            return GetEDID(PortalConstants.QSExpID]);                   // Fetch the Expense ID and convert it
//        }

        // Carefully process the RequestID Query String, returning a string and int value. If missing provide an value of zero

        public static QSValue GetRequestID()
        {
            QSValue val = new QSValue();
            val.String = HttpContext.Current.Request.QueryString[PortalConstants.QSRequestID]; // Fetch the Request ID, if it is present
            if (val.String == null || val.String == "")                 // If == the Query String is missing
            {
                val.Int = 0;
                return val;
            }
            return ConvertID(val.String);                               // Convert Query String to int and return
        }

        // Carefully process the ProjectID Query String, returning a string and int value. If absent, check ProjectInfoCookie. If missing or bad, log error.

        public static QSValue GetProjectID()
        {
            QSValue val = new QSValue();
            val.String = HttpContext.Current.Request.QueryString[PortalConstants.QSProjectID]; // Fetch the Project ID, if it is present
            if (val.String == null || val.String == "")                 // If == the Query String is missing
            {
                HttpCookie projectInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Find the Project Info cookie
                if (projectInfoCookie == null)
                {
                    LogError.LogQueryStringError("GetProjectID", "Unable to find Query String or ProjectInfoCookie in search of Project ID"); // Fatal error
                    return val;                                         // Never actually get here...
                }
                val.String = projectInfoCookie[PortalConstants.CProjectID]; // Fetch Project ID from cookie
                if (val.String == "")                                   // If == no Project ID in cookie, which is strange
                    LogError.LogInternalError("GetProjectID", "ProjectInfoCookie does not contain a Project ID value"); // Fatal error
            }
            return ConvertID(val.String);                               // Convert to int and return
        }

        // Carefully process the ProjectName Query String, returning a string alue. If absent, check ProjectInfoCookie. If missing or bad, log error.

        public static string GetProjectName()
        {
            string val = HttpContext.Current.Request.QueryString[PortalConstants.QSProjectName]; // Fetch the Project Name, if it is present
            if (val == null || val == "")                               // If == the Query String is missing
            {
                HttpCookie projectInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Find the Project Info cookie
                if (projectInfoCookie == null)
                {
                    LogError.LogQueryStringError("GetProjectName", "Unable to find Query String or ProjectInfoCookie in search of Project Name"); // Fatal error
                    return val;                                         // Never actually get here...
                }
                val = projectInfoCookie[PortalConstants.CProjectName];  // Fetch Project Name from cookie
                if (val == "")                                          // If == no Project Name in cookie, which is strange
                    LogError.LogInternalError("GetProjectName", "ProjectInfoCookie does not contain a Project Name value"); // Fatal error
            }
            return val;                                                 // Project Name from whatever source
        }

        // Carefully process the UserID Query String, returning a string value. If absent, check UserInfoCookie. If missing or bad, log error.

        public static string GetUserID()
        {
            string val = HttpContext.Current.Request.QueryString[PortalConstants.QSUserID]; // Fetch the User ID, if it is present
            if (val == null || val == "")                               // If == the Query String is missing
            {
                HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Find the User Info cookie
                if (userInfoCookie == null)
                {
                    LogError.LogQueryStringError("GetUserID", "Unable to find Query String or UserInfoCookie in search of User ID"); // Fatal error
                    return val;                                         // Never actually get here...
                }
                val = userInfoCookie[PortalConstants.CUserID];          // Fetch User ID from cookie
            }
            return val;                                                 // Return UserID
        }

        // Carefully convert string version of ID into int version

        public static QSValue ConvertID(string s)
        {
            QSValue val = new QSValue() { String = s };
            try
            {
                val.Int = Convert.ToInt32(s);                           // Convert from string to int
            }
            catch (Exception)
            {
                LogError.LogQueryStringError("ConvertID", String.Format("Invalid Query String 'ID' value of '{0}'", val.String)); // Log fatal error
            }
            return val;
        }
    }
}
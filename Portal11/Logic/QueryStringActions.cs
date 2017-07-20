using Portal11.ErrorLog;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Portal11.Logic
{
    public class QueryStringActions
    {

        // Carefully process the Command, returning a string. If missing, that's an error

        public static string GetCommand()
        {
            string val = HttpContext.Current.Request.QueryString[PortalConstants.QSCommand]; // Fetch the Command, if it is present
            if (string.IsNullOrEmpty(val))                              // If true the Query String is missing
                LogError.LogQueryStringError(GetPageName(), "Unable to find Query String 'Command' in GetCommand"); // Fatal error
            return val;                                                 // Return Command to caller
        }

        // Carefully process the ProjectID Query String, returning a string and int value. If absent, check ProjectInfoCookie. If missing or bad, log error.

        public static QSValue GetProjectID()
        {
            QSValue val = new QSValue();
            val.String = HttpContext.Current.Request.QueryString[PortalConstants.QSProjectID]; // Fetch the Project ID, if it is present
            if (!string.IsNullOrEmpty(val.String))                      // If false the Query String is present
                return ConvertID(val.String);                           // Formulate QSValue and return it

            HttpCookie projectInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Find the Project Info cookie
            if (projectInfoCookie != null)                              // If != cookie is present
            {
                val.String = projectInfoCookie[PortalConstants.CProjectID]; // Fetch Project ID from cookie
                if (!string.IsNullOrEmpty(val.String))                  // If false Project ID in cookie, use it
                    return ConvertID(val.String);                       // Formulate QSValue and return it
            }

            // We get here only if two things are missing: a ProjectID Query String and a ProjectInfo cookie. This means that user has arrived here without logging in

            HttpContext.Current.Response.Redirect(PortalConstants.URLLogin + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSDanger + "&" +
                PortalConstants.QSStatus + "=Please log in before using Portal");
            return ConvertID(val.String);
        }

        // Carefully process the ProjectName Query String, returning a string alue. If absent, check ProjectInfoCookie. If missing or bad, log error.

        public static string GetProjectName()
        {
            string val = HttpContext.Current.Request.QueryString[PortalConstants.QSProjectName]; // Fetch the Project Name, if it is present
            if (string.IsNullOrEmpty(val))                              // If true the Query String is missing
            {
                HttpCookie projectInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Find the Project Info cookie
                if (projectInfoCookie == null)
                {
                    LogError.LogQueryStringError(GetPageName(), "Unable to find Query String or ProjectInfoCookie in GetProjectName"); // Fatal error
                    return val;                                         // Never actually get here...
                }
                val = projectInfoCookie[PortalConstants.CProjectName];  // Fetch Project Name from cookie
                if (val == "")                                          // If == no Project Name in cookie, which is strange
                    LogError.LogInternalError(GetPageName(), "ProjectInfoCookie does not contain a Project Name value in GetProjectName"); // Fatal error
            }
            return val;                                                 // Project Name from whatever source
        }

        // Carefully find the Project Role value in the ProjectInfoCookie. If missing or bad, log error.

        public static string GetProjectRole()
        {
            HttpCookie projectInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Find the Project Info cookie
            if (projectInfoCookie == null)                              // If == no cookie. User is not logged in
            {
                HttpContext.Current.Response.Redirect(PortalConstants.URLLogin + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSDanger + "&" +
                    PortalConstants.QSStatus + "=Please log in before using Portal");
                return "";                                              // Never actually get here...
            }
            string val = projectInfoCookie[PortalConstants.CProjectRole]; // Fetch Project Role from cookie
            if (string.IsNullOrEmpty(val))                              // If true no Project Role in cookie, which is strange
                LogError.LogInternalError(GetPageName(), "ProjectInfoCookie does not contain a Project Role value in GetProjectRole"); // Fatal error
            
            return val;                                                 // Project Role
        }

        // Carefully process the RequestID Query String, returning a string and int value. If missing provide an value of zero

        public static QSValue GetRequestID()
        {
            QSValue val = new QSValue();
            val.String = HttpContext.Current.Request.QueryString[PortalConstants.QSRequestID]; // Fetch the Request ID, if it is present
            if (string.IsNullOrEmpty(val.String))                       // If true the Query String is missing
            {
                val.Int = 0;
                return val;
            }
            return ConvertID(val.String);                               // Convert Query String to int and return
        }

        // Carefully process the Return Query String, returning a string. If missing it's an error

        public static string GetReturn()
        {
            string val = HttpContext.Current.Request.QueryString[PortalConstants.QSReturn]; // Fetch the Return page, if it is present
            if (string.IsNullOrEmpty(val))                              // If true the Query String is missing
                LogError.LogQueryStringError(GetPageName(), "Unable to find Query String 'Return' in GetCommand"); // Fatal error
            return val;
        }

        // Carefully process the UserID Query String, returning a string value. If absent, check UserInfoCookie. If missing or bad, ask user to log in

        public static string GetUserID()
        {
            string userID = HttpContext.Current.Request.QueryString[PortalConstants.QSUserID];  // First check the Query String and find the User
            if (!string.IsNullOrEmpty(userID))                              // If false UserID is present in Query String, return it
                return userID;

            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Find the User Info cookie
            if (userInfoCookie != null)                                     // If != the cookie is present
            {
                userID = userInfoCookie[PortalConstants.CUserID];           // Fetch UserID from cookie
                if (!string.IsNullOrEmpty(userID))                          // If false UserID is present
                    return userID;
            }

            // We get here only if two things are missing: a UserID Query String and a UserInfo cookie. This means that user has arrived here without logging in

            HttpContext.Current.Response.Redirect(PortalConstants.URLLogin + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSDanger + "&" +
                PortalConstants.QSStatus + "=Please log in before using Portal");
            return "";
        }

        // Carefully find both the UserID and UserRole in UserInfo cookie. If missing, ask user to login.

        public static void GetUserIDandRole(out string userID, out string userRole)
        {
            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Find the User Info cookie
            if (userInfoCookie != null)                                     // If != the cookie is present
            {
                userID = userInfoCookie[PortalConstants.CUserID];           // Fetch UserID from cookie
                if (!string.IsNullOrEmpty(userID))                          // If false UserID is present
                {
                    userRole = userInfoCookie[PortalConstants.CUserRole];   // Fetch UserRole from cookie
                    if (!string.IsNullOrEmpty(userRole))                    // If false userRole is present
                        return;                                             // All is well, good values written to caller's arguments
                }
            }

            // We get here only if something is missing. This means that user has arrived here without logging in

            HttpContext.Current.Response.Redirect(PortalConstants.URLLogin + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSDanger + "&" +
                PortalConstants.QSStatus + "=Please log in before using Portal");
            userID = ""; userRole = ""; return;                             // Control never reaches here
        }

        // Find user role within UserInfo cookie. It doesn't involve a query string, but this seems like the right neighborhood for it.

        public static string GetUserRole()
        {
            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Fetch user info cookie for current user
            if (userInfoCookie == null)                                         // If == the cookie is missing. The user is not logged in
                HttpContext.Current.Response.Redirect(PortalConstants.URLLogin + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSDanger + "&" +
                    PortalConstants.QSStatus + "=Please log in before using Portal"); // Force login
            return userInfoCookie[PortalConstants.CUserRole];                   // Fetch user role and return
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
                LogError.LogQueryStringError(GetPageName(), $"Invalid Query String 'ID' value of '{val.String}' in ConvertID"); // Log fatal error
            }
            return val;
        }

        // Fetch the name of the current page for use in error messages

        static string GetPageName()
        {
            Page p = (Page)HttpContext.Current.CurrentHandler;          // Find the object that describes the current page
            if (p != null)                                              // If != something there to look at
                return p.AppRelativeVirtualPath;                        // Find the path to the current page
            return "name unkown";                                       // Else return a marker
        }
    }
}
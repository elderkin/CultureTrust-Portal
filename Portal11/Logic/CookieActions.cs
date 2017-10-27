using Portal11.ErrorLog;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal11.Logic
{
    public class CookieActions
    {

        // Delete cookies from previous executions. This insures a clean slate for context during the current login.

        public static void DeleteLoginInfoCookie()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies[PortalConstants.CLoginInfo] != null) // if != stale Login cookie currently exists, delete it
                {
                    HttpCookie staleCookie = new HttpCookie(PortalConstants.CLoginInfo); // Fetch stale cookie (from last user login)
                    staleCookie.Expires = DateTime.Now.AddDays(-1d);        // Ask cookie to expire immediately
                    HttpContext.Current.Response.Cookies.Add(staleCookie);  // and update cookie into oblivion
                }
            }
            catch (NullReferenceException)
            {
                // Just continue in spite of the error
            }
            return;
        }

        public static void DeleteUserInfoCookie()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo] != null) // if != stale User cookie currently exists, delete it
                {
                    HttpCookie staleCookie = new HttpCookie(PortalConstants.CUserInfo); // Fetch stale cookie (from last user login)
                    staleCookie.Expires = DateTime.Now.AddDays(-1d);        // Ask cookie to expire immediately
                    HttpContext.Current.Response.Cookies.Add(staleCookie);  // and update cookie into oblivion
                }
            }
            catch (NullReferenceException)
            {
                // Just continue in spite of the error
            }
            return;
        }

        public static void DeleteProjectInfoCookie()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo] != null)  // if != stale Project cookie currently exists, delete it
                {
                    HttpCookie staleCookie = new HttpCookie(PortalConstants.CProjectInfo); // Fetch stale cookie (from last user login)
                    staleCookie.Expires = DateTime.Now.AddDays(-1d);        // Ask cookie to expire immediately
                    HttpContext.Current.Response.Cookies.Add(staleCookie);  // and update cookie into oblivion
                }
            }
            catch (NullReferenceException)
            {
                // Just continue in spite of the error
            }
            return;
        }

        public static void DeleteCheckboxCookies()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies[PortalConstants.CProjectCheckboxes] != null) // If != stale Project Checkboxes cookie currently exists, delete it
                {
                    HttpCookie staleCookie = new HttpCookie(PortalConstants.CProjectCheckboxes); // Fetch stale cookie
                    staleCookie.Expires = DateTime.Now.AddDays(-1d);        // Ask cookie to expire immediately
                    HttpContext.Current.Response.Cookies.Add(staleCookie);  // and update cookie into oblivion
                }
                if (HttpContext.Current.Request.Cookies[PortalConstants.CStaffCheckboxes] != null) // If != stale Staff Checkboxes cookie currently exists, delete it
                {
                    HttpCookie staleCookie = new HttpCookie(PortalConstants.CStaffCheckboxes); // Fetch stale cookie
                    staleCookie.Expires = DateTime.Now.AddDays(-1d);        // Ask cookie to expire immediately
                    HttpContext.Current.Response.Cookies.Add(staleCookie);  // and update cookie into oblivion
                }
            }
            catch (NullReferenceException)
            {
                // Just continue in spite of the error
            }
            return;
        }
        public static void DeleteFlowControlCookie()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies[PortalConstants.CFlowControl] != null)  // if != stale Flow Control cookie currently exists, delete it
                {
                    HttpCookie staleCookie = new HttpCookie(PortalConstants.CFlowControl); // Fetch stale cookie (from last user login)
                    staleCookie.Expires = DateTime.Now.AddDays(-1d);        // Ask cookie to expire immediately
                    HttpContext.Current.Response.Cookies.Add(staleCookie);  // and update cookie into oblivion
                }
            }
            catch (NullReferenceException)
            {
                // Just continue in spite of the error
            }
            return;
        }

        // Look in the UserInfoCookie to find a value for number of rows per GridView control. Lots of range checks and boundary conditions

        public static int GetGridViewRows()
        {
            try
            {
                HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Fetch cookie, if it exists
                if (userInfoCookie != null)                                 // If != cookie exists
                {
                    int value = Convert.ToInt32(userInfoCookie[PortalConstants.CUserGridViewRows]); // Ask cookie for value
                    if ((value >= ApplicationUser.GridViewRowsMinimum) && (value <= ApplicationUser.GridViewRowsMaximum)) // If true, value from cookie is in range
                        return value;                                       // Give "legal" value to caller
                }
            }
            catch (Exception)
            {
                // Just continue in spite of the error
            }
            return ApplicationUser.GridViewRowsDefault;                     // Cookie value not available. Return default value
        }

        // Pick out ProjectRole from Project Info cookie. If its missing, force user to login

        public static string GetProjectRole()
        {
            HttpCookie projectInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Find the Project Info cookie
            if (projectInfoCookie != null)                                      // If != ProjectInfoCookie is present
            {
                string projRole = projectInfoCookie[PortalConstants.CProjectRole]; // Fetch ProjectRole from ProjectInfo cookie
                if (!string.IsNullOrEmpty(projRole))                            // If false ProjectRole is present
                    return projRole;
            }

            // We get here only if the ProjectInfo cookie is missing. This means that user has arrived here without logging in

            HttpContext.Current.Response.Redirect(PortalConstants.URLLogin + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSDanger + "&" +
                PortalConstants.QSStatus + "=Please log in before using Portal");
            return "";
        }

        // Make a FlowControlCookie to save current context to facilitate a "return" to this page.
        // One nuance. Browsers don't support ampersand characters in cookies - they're used to delimit multiple values within the cookie. So
        // this code substitutes a "legal" cookie character, "|" for ampersands on the way in and substitutes back on the way out.

        public static void MakeFlowControlCookie(string returnURL)
        {
            HttpCookie flowControlCookie = new HttpCookie(PortalConstants.CFlowControl); // Get a new  ready to go
            flowControlCookie[PortalConstants.CReturnURL] = returnURL.Replace('&', '|'); // Fill the cookie with caller's query string, preserving amps

            HttpContext.Current.Response.Cookies.Add(flowControlCookie);        // Store a new cookie
            return;
        }

        public static string GetFlowControlReturnURL()
        {
            HttpCookie flowControlCookie = HttpContext.Current.Request.Cookies[PortalConstants.CFlowControl]; // Find the Flow Control cookie
            if (flowControlCookie != null)                                      // If != flowControlCookie is present
            {
                string returnURL = flowControlCookie[PortalConstants.CReturnURL]; // Fetch ReturnURL from Flow Control cookie
                if (!string.IsNullOrEmpty(returnURL))                            // If false returnURL is present
                    return returnURL.Replace("|", "&");                         // Restore original ampersands in query strings
            }

            // We get here only if the Flow Control cookie is missing or empty. 

            LogError.LogInternalError ("FetchFlowControlReturnURL","Unable to find Flow Control cookie"); // Fatal error
            return "";
        }

        // Make a ProjectInfoCookie to describe the current User's relationship with their selected project.  Two versions
        // depending on whether we have a UserProject row or a Project row to work with.

        public static void MakeProjectInfoCookie(UserProject up)
        {
            HttpCookie projectInfoCookie = new HttpCookie(PortalConstants.CProjectInfo); // Get a new cookie ready to go
            projectInfoCookie[PortalConstants.CProjectID] = up.ProjectID.ToString(); // Fill the cookie from the UserProject row
            projectInfoCookie[PortalConstants.CProjectName] = up.Project.Name;
            projectInfoCookie[PortalConstants.CProjectRole] = up.ProjectRole.ToString();
            projectInfoCookie[PortalConstants.CProjectRoleDescription] = EnumActions.GetEnumDescription(up.ProjectRole);

            HttpContext.Current.Response.Cookies.Add(projectInfoCookie);    // Store a new cookie
            return;
        }

        public static void MakeProjectInfoCookie(Project p, UserRole u)
        {
            HttpCookie projectInfoCookie = new HttpCookie(PortalConstants.CProjectInfo); // Get a new cookie ready to go
            projectInfoCookie[PortalConstants.CProjectID] = p.ProjectID.ToString(); // Fill the cookie from the UserProject row
            projectInfoCookie[PortalConstants.CProjectName] = p.Name;
            projectInfoCookie[PortalConstants.CProjectRole] = u.ToString();
            projectInfoCookie[PortalConstants.CProjectRoleDescription] = EnumActions.GetEnumDescription(u);

            HttpContext.Current.Response.Cookies.Add(projectInfoCookie);    // Store a new cookie
            return;
        }
    }
}
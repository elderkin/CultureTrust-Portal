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

        // Look in the UserInfoCookie to find a value for number of rows per GridView control. Lots of range checks and boundary conditions

        public static int FindGridViewRows()
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

        // Pick out ProjectRole from cookie. If its missing, force user to login

        public static string FindProjectRole()
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
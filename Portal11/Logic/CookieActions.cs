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

        // Fetch several useful pieces of information about the user and project

        public static bool GetUserCharacteristics(
            out string userID,                          // The lengthy account ID string for the user
            out string fullName,                        // The first and last name of the user
            out UserRole userRole,                      // The role, e.g., Internal Coordinator, Project, President
            out string projectName,                     // The name of the project the user is working on, if any. Can be blank.
            out string projectRoleDescription,          // The role of the user on the project, e.g., Project Director. Note string, not enumerated
            out bool admin,                             // Flag to say that the user is an Administrator
            out string avatarImageTag)                  // The query parameter of the current avatar image
        {
            userID = "";
            avatarImageTag = "";
            fullName = "(Not logged in)";               // Establish default values in case user not logged in
            userRole = UserRole.None;
            projectName = "";                           // Assume not associated with a project
            projectRoleDescription = "";
            admin = false;

            // Start looking for things in the UserInfo cookie, which gets created at login.

            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Ask for the User Info cookie, if any
            if ((HttpContext.Current.User.Identity.Name != "") && (userInfoCookie != null)) // If != user is logged in and has UserInfoCookie
            {
                userID = userInfoCookie[PortalConstants.CUserID]; // ID of logged in user
                fullName = userInfoCookie[PortalConstants.CUserFullName]; // Return full name of logged in user

                //  Fetch project role and return project name, if any

                userRole = EnumActions.ConvertTextToUserRole(userInfoCookie[PortalConstants.CUserRole]); // Fetch role from cookie
                if ((userRole == UserRole.InternalCoordinator) || (userRole == UserRole.Project)) // If == user has a project
                { 
                    HttpCookie projectInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Ask for the Project Info cookie, if any
                    if (projectInfoCookie != null)                          // If != the cookie is present
                    {
                        projectName = projectInfoCookie[PortalConstants.CProjectName]; // Load project name
                        projectRoleDescription = projectInfoCookie[PortalConstants.CProjectRoleDescription]; // Load project role as text
                    }
                }

                //  If the User has Admin powers - regardless of role - turn on the Admin menu

                if (userInfoCookie[PortalConstants.CUserTypeAdmin] != null) // If != User has Administrator powers
                {
                    admin = true;                                // Return that fact
                }

                // Also return the avatar image tag, which helps convince the browser to refresh correctly

                HttpCookie avatarImageCookie = HttpContext.Current.Request.Cookies[PortalConstants.CAvatarImage]; // Ask for the Avatar Image cookie, if any
                if (avatarImageCookie != null)                  // If != cookie exists
                    avatarImageTag =avatarImageCookie[PortalConstants.CAvatarImageTag]; // tag of current avatar image file

                return true;                                    // Report that user is logged in
            }
            return false;                                       // Report that user is not logged in
        }

        // Make a ProjectInfoCookie to describe the current User's relationship with their selected project.  Two versions
        // depending on whether we have a UserProject row or a Project row to work with.

        public static void MakeProjectInfoCookie(UserProject up)
        {
            HttpCookie projectInfoCookie = new HttpCookie(PortalConstants.CProjectInfo); // Get a new cookie ready to go
            projectInfoCookie[PortalConstants.CProjectID] = up.ProjectID.ToString(); // Fill the cookie from the UserProject row (and Project row)
            projectInfoCookie[PortalConstants.CProjectName] = up.Project.Name;
            projectInfoCookie[PortalConstants.CProjectCode] = up.Project.Code;
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

        // We want to change one tiny thing - the tag that Site.Master uses to force a refresh of the Avatar image.
        // Delete the AvatarImage cookie (if it exists) and recreate it using a random value.

        public static void UpdateAvatarImageTag()
        {

            // Delete current cookie, if it exists

            HttpCookie oldCookie = HttpContext.Current.Request.Cookies[PortalConstants.CAvatarImage]; // Fetch current cookie, if any
            if (oldCookie != null)                                      // If != cookie exists and can be deleted
            { 
                oldCookie.Expires = DateTime.Now.AddDays(-1d);          // Ask cookie to expire immediately
                HttpContext.Current.Response.Cookies.Add(oldCookie);    // and update cookie into oblivion
            }

            // Make a new cookie to contain value from caller

            HttpCookie newCookie = new HttpCookie(PortalConstants.CAvatarImage); // Get a new cookie ready to go
            Random rnd = new Random();
            newCookie[PortalConstants.CAvatarImageTag] = rnd.Next(PortalConstants.CAvatarNextTagRangeStart, PortalConstants.CAvatarNextTagRangeEnd).ToString();        // Insert caller's value into cookie
            HttpContext.Current.Response.Cookies.Add(newCookie);        // Store a new cookie
            return;
        }
    }
}
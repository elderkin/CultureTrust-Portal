using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal11.Models;
using Portal11.Logic;

namespace Portal11
{
    public partial class SiteMaster : MasterPage
    {
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;

        protected void Page_Init(object sender, EventArgs e)
        {
            // The code below helps to protect against XSRF attacks
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }

        // Fuss with the Navbar menu, based on context. The LoginDispatch page, invoked during the login, 
        // has done yoeman's work to set up all this context. If the user is logged AND has a UserInfo cookie:
        //  1) Display the User's Full Name
        //  2) If the user is an Admin, turn on the appropriate menus, show role
        //  3) If the user is a Staff, turn on appropriate menus, show role
        //  4) If the user is a Basic, turn on appropriate menus, show Project Name and role

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                HttpCookie userInfoCookie = Request.Cookies[PortalConstants.CUserInfo]; // Ask for the User Info cookie, if any
                if ((HttpContext.Current.User.Identity.Name != "") && (userInfoCookie != null)) // If != user is logged in and has UserInfoCookie
                {

                    //  1) Display their Full Name

                    labFullName.Text = userInfoCookie[PortalConstants.CUserFullName]; //Load label with user's full name from cookie
                    labFullName.ToolTip = "Not associated with a specific project";

                    //  2) If the User has Admin powers - regardless of role - turn on the Admin menu

                    if (userInfoCookie[PortalConstants.CUserTypeAdmin] != null) // If != User has Administrator powers
                    {
                        mnuAdmin.Visible = true;                                // Make visible the Navbar link to the Admin function
                    }

                    //  3) Break out by role and light up the appropriate menus

                    UserRole role = EnumActions.ConvertTextToUserRole(userInfoCookie[PortalConstants.CUserRole]); // Fetch role from cookie
                    switch (role)
                    {
                        case UserRole.Administrator:                            // User is an Admin and nothing else
                        {
                            litAdminRole.Text = EnumActions.GetEnumDescription(UserRole.Administrator); // Show the Administrator in the Navbar
                            mnuAdminRole.Visible = true;                        // Switch on the Role menu item, which links to the Admin Main page
                            break;
                        }
                        case UserRole.InternalCoordinator:
                        case UserRole.Project:
                        {
                            mnuProject.Visible = true;                              // Unconditionally turn on Project menu, which links to Project functions
                            HttpCookie projectInfoCookie = Request.Cookies[PortalConstants.CProjectInfo]; // Ask for the Project Info cookie, if any
                            if (projectInfoCookie != null)                          // If != the cookie is present
                            {
                                mnuHomeProject.Visible = true;                      // Make appropriate Home menu visible
                                                                                    // litProjectRole.Text = projectInfoCookie[PortalConstants.CProjectRoleDescription];
                                labFullName.ToolTip = projectInfoCookie[PortalConstants.CProjectRoleDescription];
                                // Load flyover with user role description specific to this project
                                // mnuProjectRole.Visible = true;                      // Make the menu visible in the Navbar; it links to Project Dashboard
                                litProjectName.Text = projectInfoCookie[PortalConstants.CProjectName]; // Load literal with project name from cookie
                                mnuProjectName.Visible = true;                      // Make the menu visible in the Navbar, it also links to Project Dashboard
                            }
                            else                                                    // Not assigned to a Project - in limbo!
                            {
                                litNoProjectRole.Text = userInfoCookie[PortalConstants.CUserRoleDescription]; // No project available. Use "base" role
                                mnuNoProjectRole.Visible = true;                    // Make menu visible in Navbar; it doesn't link anywhere
                            }
                            break;
                        }
                        case UserRole.Auditor:
                        case UserRole.FinanceDirector:
                        case UserRole.TrustDirector:
                        case UserRole.TrustExecutive:
                        {
                            mnuHomeStaff.Visible = true;                            // Make appropriate Home menu visible
                            litStaffRole.Text = userInfoCookie[PortalConstants.CUserRoleDescription]; // Show that role in the Navbar, link to Staff Dashboard
                            mnuStaffRole.Visible = true;                            // Switch on the Role menu item
//                            mnuAdminRole.Visible = false;                           // Don't display both Admin Role and Staff Role
                            break;
                        }
                        default:                                                    // We shouldn't get here, but don't throw errors in the friggin' Navbar
                        {
                            litNoProjectRole.Text = userInfoCookie[PortalConstants.CUserRoleDescription]; // No project available. Use "base" role
                            mnuNoProjectRole.Visible = true;                        // Make menu visible in Navbar; it doesn't link anywhere
                            break;
                        }
                    }
                }
                else
                {
                    pnlMainBar.Visible = false;                                 // If not logged in, must be logging in. Make main menu bar invisible.
                }

            }
        }

        // User is logging out. If they're present, kill off the UserInfo and ProjectInfo cookies to give the next login a clean start.

        protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            CookieActions.DeleteUserInfoCookie();
            CookieActions.DeleteProjectInfoCookie();
            CookieActions.DeleteCheckboxCookies();                      // Delete cookies that were created during this login session.

            Context.GetOwinContext().Authentication.SignOut();          // Shutdown ASP Identity logged in session
        }
    }
}
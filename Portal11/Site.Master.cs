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
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed. AntiXsrfTokenKey: "
                        + (string)ViewState[AntiXsrfTokenKey] + " AntiXsrfUserNameKey: " + (string)ViewState[AntiXsrfUserNameKey]
                        + " on page: " + HttpContext.Current.Request.Url.AbsolutePath);
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

                // Get information about the user and the project in order to fill in header

                string userID, fullName; UserRole userRole; string projectName, projectRoleDescription; bool admin; string avatarImageTag;
                if (CookieActions.GetUserCharacteristics(out userID,            // Account ID for logged in user
                                                         out fullName,          // User name
                                                         out userRole,          // User Role, e.g., IC, PR
                                                         out projectName,       // Name of project, if any, user is working on now
                                                         out projectRoleDescription, // User role on project, e.g., PD, PS, IC
                                                         out admin,             // Whether user is an administrator
                                                         out avatarImageTag)    // Query string to force browser to reload avatar image
                    )                                           // If true user is logged in
                {

                    // Load user-specific image into Avatar and describe user in flyover help. 

                    imgAvatar.ImageUrl = PortalConstants.AvatarDir + PortalConstants.ImageAvatarName + userID + PortalConstants.ImageExt // Formulate name of user's avatar image using UserID
                        + "?" + avatarImageTag;                     // If avatar image has changed, this value will change and browser will refresh

                    imgAvatar.ToolTip = fullName                    // Create flyover help
                        + Environment.NewLine + EnumActions.GetEnumDescription(userRole); // Assume no project
                    litFullName.Text = fullName;                    // Make user name appear in drop down menu, too

                    //  Break out by role and light up the appropriate menus

                    switch (userRole)
                    {
                        case UserRole.InternalCoordinator:
                        case UserRole.Project:
                            {

                                // These roles work on projects, one project at a time. As soon as we recognize the role,
                                // we can light the "Project" menu, even if no project is currently selected.
                                // If a project is current, we react to it with an enhanced ToolTip and more menu items.

                                mnuProject.Visible = true;              // Turn on the Project menu

                                if (!string.IsNullOrEmpty(projectName))  // If false, user assigned to project
                                {
                                    imgAvatar.ToolTip = fullName        // Redo flyover to include project info
                                        + Environment.NewLine + projectRoleDescription
                                        + Environment.NewLine + projectName;

                                    // Turn on menu options that are specific to a project

                                    mnuHomeProject.Visible = true;      // We now have a Home item that goes to Project Dashboard
                                    mnuEditProjectClasses.Visible = true; // And the Project menu can act on our current project
                                }
                                break;
                            }
                        case UserRole.Auditor:
                        case UserRole.FinanceDirector:
                        case UserRole.CommunityDirector:
                        case UserRole.President:
                            {
                                mnuHomeStaff.Visible = true;        // Make appropriate Home menu visible
                                break;
                            }
                        default:                                    // All other roles - there aren't any - get no other menus
                            {
                                break;
                            }
                    }

                    //  If the User has Admin powers - regardless of role - turn on the Admin menu

                    if (admin)                                      // If true User has Administrator powers
                        mnuAdmin.Visible = true;                    // Make visible the Navbar link to the Admin function
                }
                else
                {
                    pnlMainBar.Visible = false;                     // If not logged in, must be logging in. Make main menu bar invisible.
                }
            }
            return;
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
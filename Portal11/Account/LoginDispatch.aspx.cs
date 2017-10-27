using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Account
{
    public partial class LoginDispatch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // This page completes the Login process. It can be reached in several ways:
            //  1) Somebody came here by typing in the URL. That's no good.
            //  2) As a dispatch path from the Login page. If that's the case, there are Query Strings:
            //      Command of "UserLogin"
            //      Email, which tells us who just logged in
            //      RememberEmail, which tells us whether the "Remember email" checkbox was checked.
            //
            //  Complete their login process and dispatch to the appropriate page.
            //
            // Note that this page is never invoked directly and never displays a page. Therefore, it does not process status messages.

            string cmd = Request.QueryString[PortalConstants.QSCommand]; // Fetch Command,"UserLogin"
            string email = Request.QueryString[PortalConstants.QSEmail]; // Fetch "Email" Query String, if any
            string rememberEmail = Request.QueryString[PortalConstants.QSRememberEmail]; // Fetch "RememberEmail" Query String, if any
//            string status = Request.QueryString[PortalConstants.QSStatus]; // Fetch "Status" Query String, if any

            if (cmd == null)                                        // If == there is no command in the Query String.
                Response.Redirect(PortalConstants.URLDefault + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSDanger 
                          + "&" + PortalConstants.QSStatus + "=" + "The LoginDispatch page was reached in error.");

            // All we have is User's email. We need to hit the database to find User ID and Full Name.

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var userMgr = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)); // Open path to User Manager
                ApplicationUser loggedInUser = userMgr.FindByEmail(email); // Look for an existing user
                if (loggedInUser == null)                               // Cannot re-locate record used during login
                {
                    LogError.LogInternalError("LoginDispatch", $"Unable to find User with email '{email}' in database"); // Fatal error
                }

                // If the User's account is Inactive, log them right back out again.

                if (loggedInUser.Inactive)                              // If true, account is inactive and cannot be used for login
                {
                    Context.GetOwinContext().Authentication.SignOut(); // Log them right back out of there
                    Response.Redirect(PortalConstants.URLLogin + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSDanger + "&" +
                        PortalConstants.QSStatus + "=This account is Inactive and cannot be used to log in");
                }
                string userID = loggedInUser.Id;                        // Copy User ID out of database row and save for later use

                // Check Portal Parameters for Disable Logins flag

                if (!loggedInUser.Administrator)                        // If true user is not an Administrator. Could be locked out by disabled login
                {
                    string fran = SupportingActions.GetFranchiseKey();  // Fetch franchise code
                    PortalParameter portalParameter = context.PortalParameters.Where(p => p.FranchiseKey == fran).FirstOrDefault(); // Lookup by franchise key
                    if (portalParameter == null)                        // If == PortalParameter row is missing
                    {
                        LogError.LogInternalError("LoginDispatch", $"PortalParameter row for franchise '{fran}' is missing."); // Fatal error
                    }
                    if (portalParameter.DisableLogins)                  // If true logins for non-Administrators are disabled
                    {
                        Context.GetOwinContext().Authentication.SignOut(); // Log them right back out of there
                        Response.Redirect(PortalConstants.URLLogin + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSDanger 
                                  + "&" + PortalConstants.QSStatus + "=Portal logins are temporarily disabled. Please try again later."); // Head back to the login page
                    }
                }

                // Write down the fact that we just logged in the user

                try
                {
                    loggedInUser.LoginCount++;                              // Count the login
                    loggedInUser.LastLogin = System.DateTime.Now;           // Record the time of login
                    userMgr.Update(loggedInUser);                           // Write changes to database
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditRegistration", "Unable to update User rows in Identity database"); // Fatal error
                }

                // Start building out the Login and UserInfo cookies, starting with the parts that are there for all users.

                if (rememberEmail == "True")                            // If == User wants us to remember their email for next login
                {
                    HttpCookie loginInfoCookie = new HttpCookie(PortalConstants.CLoginInfo);
                    loginInfoCookie[PortalConstants.CLoginEmail] = email; // Remember email here in case we need to "Remember" it
                    Response.Cookies.Add(loginInfoCookie);              // Store a new Login Info cookie
                }
                else                                                    // Otherwise, obliterate our memory in this regard
                {
                    CookieActions.DeleteLoginInfoCookie();              // Delete cookie that stores email address
                }

                CookieActions.DeleteUserInfoCookie();                   // Wipe out previous information about User
                HttpCookie userInfoCookie = new HttpCookie(PortalConstants.CUserInfo);
                userInfoCookie[PortalConstants.CUserID] = loggedInUser.Id;
                userInfoCookie[PortalConstants.CUserFullName] = loggedInUser.FullName;
                userInfoCookie[PortalConstants.CUserFranchiseKey] = loggedInUser.FranchiseKey; // Insert current User's Franchise Key
                userInfoCookie[PortalConstants.CUserGridViewRows] = loggedInUser.GridViewRows.ToString(); // Find number of rows to display in GridView controls

                // The User could be both an Administrator and something else. So we check for Administrator first and fill in one of its cookie fields. 
                //
                // Since we only login once, we spend a lot of time here writing down things about the User and their roles. That way, SiteMaster, which
                // runs at every page, can be more efficient.

                if (loggedInUser.Administrator)                         // If true, User is an Administrator. Could be other things, too.
                {
                    userInfoCookie[PortalConstants.CUserTypeAdmin] = "true"; // Note this in the under-construction cookie
                }

                // Set up cookie contents based on our user role. This combination of cookies drives a lot of processing downstream. It is carefully
                // documented in the Design Description document.

                // There is logic here for a UserRole of "Administrator." By rule, this UserRole cannot exist - an Administrator must have a Staff role.
                // But this logic is retained here just in case that rule changes.

                switch(loggedInUser.UserRole)
                {
                    case UserRole.Administrator:                            // User is just an administrator, no other role. Currently, this option cannot exist.
                    {
                        userInfoCookie[PortalConstants.CUserRole] = UserRole.Administrator.ToString(); // If no other role, we are an administrator
                        userInfoCookie[PortalConstants.CUserRoleDescription] = EnumActions.GetEnumDescription(UserRole.Administrator); // The formatted version
                        userInfoCookie[PortalConstants.CUserProjectSelector] = PortalConstants.CUserProjectUser; // See one Project, if any assigned
                        break;
                    }
                    case UserRole.Auditor:
                    {
                        userInfoCookie[PortalConstants.CUserTypeStaff] = "true"; // Note the Staff role in cookie
                        userInfoCookie[PortalConstants.CUserRole] = UserRole.Auditor.ToString(); // Note our specific role
                        userInfoCookie[PortalConstants.CUserRoleDescription] = EnumActions.GetEnumDescription(UserRole.Auditor); // The formatted version
                        userInfoCookie[PortalConstants.CUserProjectSelector] = PortalConstants.CUserProjectAll; // See all Projects
                        break;
                    }
                    case UserRole.InternalCoordinator:
                    {
                        userInfoCookie[PortalConstants.CUserTypeInternalCoordinator] = "true"; // Note the Coordinator role in cookie
                        userInfoCookie[PortalConstants.CUserTypeProject] = "true"; // Can also operate on a Project
                        userInfoCookie[PortalConstants.CUserRole] = UserRole.InternalCoordinator.ToString(); // Note our specific role
                        userInfoCookie[PortalConstants.CUserRoleDescription] = EnumActions.GetEnumDescription(UserRole.InternalCoordinator); // The formatted version
                        userInfoCookie[PortalConstants.CUserProjectSelector] = PortalConstants.CUserProjectAll; // See all Projects
                        break;
                    }
                    case UserRole.FinanceDirector:
                    {
                        userInfoCookie[PortalConstants.CUserTypeStaff] = "true"; // Note the Staff role in cookie
                        userInfoCookie[PortalConstants.CUserRole] = UserRole.FinanceDirector.ToString(); // Note our specific role
                        userInfoCookie[PortalConstants.CUserRoleDescription] = EnumActions.GetEnumDescription(UserRole.FinanceDirector); // The formatted version
                        userInfoCookie[PortalConstants.CUserProjectSelector] = PortalConstants.CUserProjectAll; // See all Projects
                        break;
                    }
                    case UserRole.CommunityDirector:
                    {
                        userInfoCookie[PortalConstants.CUserTypeStaff] = "true"; // Note the Staff role in cookie
                        userInfoCookie[PortalConstants.CUserRole] = UserRole.CommunityDirector.ToString(); // Note our specific role
                        userInfoCookie[PortalConstants.CUserRoleDescription] = EnumActions.GetEnumDescription(UserRole.CommunityDirector); // The formatted version
                        userInfoCookie[PortalConstants.CUserProjectSelector] = PortalConstants.CUserProjectAll; // See all Projects
                        break;
                    }
                    case UserRole.President:
                    {
                        userInfoCookie[PortalConstants.CUserTypeStaff] = "true"; // Note the Staff role in cookie
                        userInfoCookie[PortalConstants.CUserRole] = UserRole.President.ToString(); // Note our specific role
                        userInfoCookie[PortalConstants.CUserRoleDescription] = EnumActions.GetEnumDescription(UserRole.President); // The formatted version
                        userInfoCookie[PortalConstants.CUserProjectSelector] = PortalConstants.CUserProjectAll; // See all Projects
                        break;
                    }
                    default:
                    {
                        userInfoCookie[PortalConstants.CUserTypeProject] = "true"; // Note this is a Project User, not Admin or Staff
                        userInfoCookie[PortalConstants.CUserRole] = UserRole.Project.ToString(); // If nothing else, User is a Project user
                        userInfoCookie[PortalConstants.CUserRoleDescription] = EnumActions.GetEnumDescription(UserRole.Project); // The formatted version
                        userInfoCookie[PortalConstants.CUserProjectSelector] = PortalConstants.CUserProjectUser; // See only Projects assigned to us
                        break;
                    }
                }
                Response.Cookies.Add(userInfoCookie);                   // Store a new User Info cookie

                // Dispatch based on whether the User is Project, Admin or Staff.

                CookieActions.DeleteProjectInfoCookie();                // Delete prior ProjectInfo cookie, if any
                if (userInfoCookie[PortalConstants.CUserTypeStaff] != null) // If != the user has a Staff role. Dispatch as Staff
                {
                    Response.Redirect(PortalConstants.URLStaffDashboard); // Transfer to CW Staff dashboard                       }
                }
                if (userInfoCookie[PortalConstants.CUserTypeInternalCoordinator] != null) // If != the user is a Coordinator. Dispatch to pick a project
                {
                    Response.Redirect(PortalConstants.URLSelectProject + "?" + PortalConstants.QSUserID + "=" + userID + "&" +
                                                        PortalConstants.QSCommand + "=" + PortalConstants.QSCommandUserLogin + "&" +
                                                        PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&" +
                                                        PortalConstants.QSStatus + "=Please choose the project you wish to work on.");
                }
                if (userInfoCookie[PortalConstants.CUserTypeAdmin] != null) // If != the User is an Administrator. Dispatch as Admin
                {
                    Response.Redirect(PortalConstants.URLAdminMain);    // Transfer to main Admin page
                }

                // By process of elimination, the User is a Project User. Not Admin, not Staff, not Coordinator
                // For Project Users, we collect Project information and save it in a Project cookie. Then:
                //  Users with zero projects go to the bit bucket - Default page
                //  Users with a single project go to the Project Dashboard page
                //  Users with two or more projects to the Select Project page

                var query2 = from up in context.UserProjects
                            where up.UserID == userID
                            select up;
                List<UserProject> loggedInUserProjects = query2.ToList(); // Fetch the projects associated with the logged in user
                switch (loggedInUserProjects.Count())                   // Determine how many projects the user is associated with
                {
                    case 0:                                             // Zero projects is a corner case - User unassigned to any Project
                    {
                            Response.Redirect(PortalConstants.URLDefault + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSDanger + "&" +
                                              PortalConstants.QSStatus + "=" + 
                                "You are successfully logged in. However you are not associated with any project. Contact CW Staff.");
                            break;
                    }
                    case 1:                                             // One project is the normal case.
                    {

                        // We have Project ID, Project Name and Project Role. Put it in a cookie and dispatch to Project Dashboard.

                        CookieActions.MakeProjectInfoCookie(loggedInUserProjects[0]); 

                        Response.Redirect(PortalConstants.URLProjectDashboard +  "?" + PortalConstants.QSUserID + "=" + userID + "&" +
                                          PortalConstants.QSProjectID + "=" + loggedInUserProjects[0].ProjectID.ToString() );
                        break;
                    }
                    default:                                            // More than one project requires a choice by User
                    {
                                
                        //Ask the user to choose which project to work on

                        Response.Redirect(PortalConstants.URLSelectProject + "?" + PortalConstants.QSUserID + "=" + userID + "&" +
                                        PortalConstants.QSCommand + "=" + PortalConstants.QSCommandUserLogin + "&" +
                                        PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&" +
                                        PortalConstants.QSStatus + "=" + 
                            "You are engaged with more than one Project. Please choose the project you wish to work on.");
                        break;
                    }
                }                                                       // End of switch by Project count
            }                                                           // End of using block
        }
    }
}
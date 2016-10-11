using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using Portal11.Models;
using System.Web.UI.WebControls;
using Portal11.Logic;
using Portal11.ErrorLog;

namespace Portal11.Account
{
    public partial class Register : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                txtFranchiseKey.Text = SupportingActions.GetFranchiseKey();     // Fill in franchise code
                HttpCookie userInfoCookie = Request.Cookies[PortalConstants.CUserInfo];
                if (userInfoCookie == null)                                     // If null, no one logged in yet. Very first execution
                {
                    pnlFranchise.Visible = true;                                // Make this code visible, just in this case
                    litDangerMessage.Text = 
                        "This is the first execution of the Portal. Please create an Administrator User, restart your brower, and login. Make sure Franchise field is correct.";
                }
            }
        }

        // Back out without making any changes

        protected void CancelUser_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLAdminMain);
        }

        // Validate entry then create the new user. But don't log them in.

        protected void CreateUser_Click(object sender, EventArgs e)
        {
            var userMgr = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

            // Make sure the email address is not already in use. Note that the FindByEmail lookup is
            // case insensitive. Also, on a "cold" start of the server following a publish, this is the first touch of the
            // database. So if something is broken with the database, it will break here.

            try {
                var existingUser = userMgr.FindByEmail(Email.Text);                 // Use the email address to find user
                if (existingUser != null)                               // If != user with matching email address found
                {
                    litDangerMessage.Text = "User account with specified email already exists"; // Post the error message
                    return;
                }
            }
            catch (Exception ex)
            {
                LogError.LogDatabaseError(ex, "Register", "Error during FindByEmail"); // Fatal error
                return;
            }

            // Fill the new user account row. Add password a little later

            var newUser = new ApplicationUser()                     // Instantiate and initialize row for new user
            {
                Inactive = false,
                UserName = Email.Text.ToLower(),
                Email = Email.Text.ToLower(),
                FullName = FullName.Text,
                Address = Address.Text,
                PhoneNumber = PhoneNumber.Text
            };

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            //string code = manager.GenerateEmailConfirmationToken(user.Id);
            //string callbackUrl = IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id, Request);
            //manager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.");

            //            signInManager.SignIn( user, isPersistent: false, rememberBrowser: false);

            newUser.UserRole = UserRole.Project;                        // Assume they are a "vanilla" Project User, i.e., Project Director or Project Staff

            // Check for Administrator option

            foreach (ListItem item in cblOptions.Items)
            {
                if (item.Value == UserRole.Administrator.ToString())    // If == this is the Administrator checkbox
                {
                    newUser.Administrator = item.Selected;              // Copy selection into database
                }
            }

            // Update database with Roles from radio buttons

            foreach (ListItem item in rblRole.Items)
            {
                if (item.Value != "")                               // If != this item has a value (cooresponding to a role)
                {
                    if (item.Selected)                              // Box is checked
                    {
                        newUser.UserRole = EnumActions.ConvertTextToUserRole(item.Value); // Convert back into enumeration type
                        break;                                      // Radio buttons - so only one role to a customer
                    }
                }
            }

            if ((newUser.UserRole == UserRole.Project) && (newUser.Administrator)) // If true Administrator checked, but no CS Staff role
            {
                litDangerMessage.Text = "A System Administrator must have a CultureTrust Staff Role.";
                return;
            }

            // Process GridViewRows. A value of zero is legal; it means accept the default value

            newUser.GridViewRows = 0;                               // Supply default
            if (txtGridViewRows.Text != "")                         // If != there is a value available
                newUser.GridViewRows = Convert.ToInt32(txtGridViewRows.Text); // Convert value user supplied

            // Initialize login history information

            newUser.LoginCount = 0;                                 // The user has never logged in
            newUser.LastLogin = System.DateTime.Now;                // Save time account was created

            // Fill in the FranchiseKey for the User

            newUser.FranchiseKey = txtFranchiseKey.Text;            // Fetch this from control. The control is invisible unless its very first user

            // Write the new User row with its Password

            IdentityResult result = userMgr.Create(newUser, Password.Text);
            if (!result.Succeeded)
            {
                litDangerMessage.Text = result.Errors.FirstOrDefault();             // Post however the Create failed
                return;
            }

            Response.Redirect(PortalConstants.URLAdminMain + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                              + PortalConstants.QSStatus + "=" + "User registration successfully added");
        }
    }
}
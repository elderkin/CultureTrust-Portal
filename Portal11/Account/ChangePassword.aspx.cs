using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Portal11.Models;
using Portal11.Logic;

namespace Portal11.Account
{
    public partial class ChangePassword : System.Web.UI.Page
    {

        // Come here to change the password of the current user. This requires different logic than ChangeUserPassword, which changes the password of somebody
        // else. The user can invoke this function from anywhere, so we have to figure out where to return.

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            NavigationActions.NextPage("");                                 // Head back to the user
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
            IdentityResult result = manager.ChangePassword(User.Identity.GetUserId(), txtCurrentPassword.Text, txtNewPassword.Text);
            if (result.Succeeded)                                                   // If true password was successfully changed
            {
                var user = manager.FindById(User.Identity.GetUserId());
                signInManager.SignIn(user, isPersistent: false, rememberBrowser: false); // Sign the user back in

                NavigationActions.NextPage("?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                        + PortalConstants.QSStatus + "=" + "Password successfully updated");
            }
            else
            {
                List<string> err = result.Errors.ToList<string>();                  // Pull errors out of the result
                litDangerMessage.Text = err[0];                                     // Post the first error message and try again                
            }

        }
    }
}
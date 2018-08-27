using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using Portal11.Logic;
using Portal11.Models;

namespace Portal11.Account
{
    public partial class ForgotPassword : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Forgot(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Validate the user's email address. Note that we do not yet implement what ASP.NET Identity calls email verification.

                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                ApplicationUser user = manager.FindByName(Email.Text); // Lookup the specified email
                if (user == null)                           // If == the email was not found || !manager.IsEmailConfirmed(user.Id))
                {
                    FailureText.Text = "This email address is unknown to the Portal. If you need a Portal account, please contract the Administrator.";
                    ErrorMessage.Visible = true;
                    return;
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send email with the code and the redirect to reset password page

                string code = manager.GeneratePasswordResetToken(user.Id); // The ASP.NET Identity string that will trigger the reset

                // We need to synthesize the URL that we will embed in the email message. We get part of this
                // from the URL of the current page and some of it from the name of the reset password page. Then we append that reset token we just made.

                string currentAbsoluteUrl = HttpContext.Current.Request.Url.AbsoluteUri;
                int rootLength = currentAbsoluteUrl.IndexOf(PortalConstants.URLForgot1); // Find path in Url

                string targetUrl = currentAbsoluteUrl.Substring(0, rootLength)  // Pull the path as the root of the tartet page
                    + PortalConstants.URLResetPassword                  // Add the name of the Reset Password page
                    + "?" + PortalConstants.QSEmail + "=" + Email.Text // Add the target email address
                    + "&" + PortalConstants.QSCode + "=" + HttpUtility.UrlEncode(code) // Add the reset code, a parameter for that page
                    ;
                
                EmailActions.SendForgotPasswordToPortalUser(Email.Text, targetUrl); // Format and send message

                // Flip the page to a sayonara message

                loginForm.Visible = false;
                DisplayEmail.Visible = true;
            }
        }
    }
}
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using Portal11.Logic;
using Portal11.Models;

namespace Portal11.Account
{
    public partial class ResetPassword : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Triggered via a hyperlink in a Reset Password email to the user.
                // Parameters:
                //  Code - a password reset code from ASP.NET Identity
                //  Email - the user's email address
                // Fetch and validate Query String parameters. Carefully convert the integer parameters to integers.

                string email = HttpContext.Current.Request.QueryString[PortalConstants.QSEmail]; // Fetch the email address, if it is present
                if (!string.IsNullOrEmpty(email))           // If false, the email parameter was there
                {
                    txtEmail.Text = email;                  // Load the email as a default value
                }
            }
            return;
        }

        protected string StatusMessage
        {
            get;
            private set;
        }

        protected void Reset_Click(object sender, EventArgs e)
        {
            string code = IdentityHelper.GetCodeFromRequest(Request);
            if (code != null)
            {
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

                var user = manager.FindByName(txtEmail.Text);
                if (user == null)                   // If == user not found in Portal
                {
                    ErrorMessage.Text = "Email not found";
                    return;
                }
                var result = manager.ResetPassword(user.Id, code, txtPassword.Text);
                if (result.Succeeded)               // If true password was successfully changed
                {
                    Response.Redirect(PortalConstants.URLResetPasswordConfirmation); // Chain to the success confirmation page
                    return;
                }
                ErrorMessage.Text = result.Errors.FirstOrDefault();
                return;
            }

            ErrorMessage.Text = "An error has occurred";
        }
    }
}
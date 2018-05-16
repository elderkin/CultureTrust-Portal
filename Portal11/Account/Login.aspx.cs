using System;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Portal11.Models;
using Portal11.Logic;
using System.Reflection;

namespace Portal11.Account
{
    public partial class Login1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                // Try to remember the email from the User's last login

                HttpCookie loginInfoCookie = Request.Cookies[PortalConstants.CLoginInfo]; // Find the Login Info cookie, if any
                if (loginInfoCookie != null)                         // If != the cookie exists
                {
                    string cookieEmail = loginInfoCookie[PortalConstants.CLoginEmail]; // Fetch last email, if any
                    if (cookieEmail != null)                        // If != something there
                    {
                        Email.Text = cookieEmail;                   // Place it in the Email text box
                        ckRememberEmail.Checked = true;             // Offer to remember it again
                    }
                }

                //RegisterHyperLink.NavigateUrl = "Register";
                // Enable this once you have account confirmation enabled for password reset functionality
                //ForgotPasswordHyperLink.NavigateUrl = "Forgot";
                //OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
                //var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
                //if (!String.IsNullOrEmpty(returnUrl))
                //{
                //    RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
                //}
            }
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (IsValid)
            {

                // Validate the user password

                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();

                // This doen't count login failures towards account lockout
                // To enable password failures to trigger lockout, change to shouldLockout: true
                var result = signinManager.PasswordSignIn(Email.Text, Password.Text, RememberMe.Checked, shouldLockout: false);

                switch (result)
                {
                    case SignInStatus.LockedOut:
                        Response.Redirect("/Account/Lockout");
                        break;
                    case SignInStatus.RequiresVerification:
                        Response.Redirect(String.Format("/Account/TwoFactorAuthenticationSignIn?ReturnUrl={0}&RememberMe={1}",
                                                        Request.QueryString["ReturnUrl"],
                                                        RememberMe.Checked),
                                          true);
                        break;
                    case SignInStatus.Failure:
                    default:
                        litDangerMessage.Text = "Invalid login attempt";
                        break;
                    case SignInStatus.Success:

                        // On a successful login, we have lots of work to do. But let's go somewhere else to do it, because of an odd
                        // ASP.NET "feature." The HttpContext.Current.User.IsInRole function does not seem to get updated until we flip
                        // into a new page. If we check it here, the return is invariably "false." So let's go to the LoginDispatch page. When
                        // we do the same fuction there, we get a correct answer.

                        Response.Redirect(PortalConstants.URLLoginDispatch + "?" +
                                                         PortalConstants.QSCommand + "=" + PortalConstants.QSCommandUserLogin + "&" +
                                                         PortalConstants.QSEmail + "=" + Email.Text + "&" +
                                                         PortalConstants.QSRememberEmail + "=" + ckRememberEmail.Checked.ToString());
                        break;
                }                                                               // End of switch by login success/failure
            }
        }

        // The image button on the bottom of the login screen has been clicked. This is a back door into the system after the database
        // has been reinitialized. There aren't any users yet, so there's no way to login. The idea is that the administrator will come
        // through here and immediately create an administrator account for themselves.
        // It's not a security breach because since the database has been reitialized, it is empty and there is no data for an intruder to access.

        protected void imgFirst_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect(PortalConstants.URLAdminMain + "?" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandFirst);
        }

        // The user pressed the "What's New" button. Fill the text box and flip its visibility.
        protected void btnNew_Click(object sender, EventArgs e)
        {
            if (pnlNew.Visible)                                 // If true What's New text is currently visible. Hide it.
            {
                pnlVersion.Visible = false; pnlNew.Visible = false; pnlFirst.Visible = true; // Hide What's New, show button
            }
            else
            {

                // Find and display version number. We manually set major, minor, and build in AssemblyInfo.cs. Ignore revision number.

                Assembly web = Assembly.Load(PortalConstants.AssemblyName);
                AssemblyName webName = web.GetName();
                txtVersion.Text = "V " + webName.Version.Major.ToString() // Fetch major version number
                    + "." + webName.Version.Minor.ToString()        // Append minor version number
                    + "." + webName.Version.Build.ToString();       // Append build number
                pnlVersion.Visible = true;                          // Make the panel containing both text boxes visible

                try
                {
                    string serverRoot = HttpContext.Current.Server.MapPath(PortalConstants.ReadmeDir); // Path to Readme directory on server
                    System.IO.StreamReader file = new System.IO.StreamReader(serverRoot + PortalConstants.WhatsNewName); // Open the file for reading

                    string line;                                    // A buffer for a single line of the text file
                    string text = "";                               // Room to accummulate text of What's New
                    bool firstLine = true;                          // Flag to track whether line needs a /n
                    while ((line = file.ReadLine()) != null)        // Read until end-of-file
                    {
                        if (!firstLine)                             // If false not first line of text
                            text += "\n";                           // Append a NewLine
                        text += line;                               // Append the next line 
                        firstLine = false;                          // No longer processing the first line
                    }

                    file.Close();
                    txtNew.Text = text;                             // Store the text in the control
                    pnlNew.Visible = true; pnlFirst.Visible = false; // Make What's New visible, hide button
                }
                catch
                {
                    txtNew.Text = "Cannot access What's New";     // Overwrite build number with bailout message
                }
            }
            return;
        }
    }
}
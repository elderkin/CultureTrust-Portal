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
    public partial class ChangeUserPassword : System.Web.UI.Page
    {

        // Only invoked by an Admin user through the Admin Main page. So that's where we return.
        // Come here from SelectUser, which has identified the User to edit
        //  UserID - the ID of the selected user
        //  FullName - the full name of the selected user.
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                string userID = Request.QueryString[PortalConstants.QSUserID]; // Fetch the query string for User ID
                if (userID == "")                                           // No User ID. We are invoked incorrectly.
                {
                    LogError.LogQueryStringError("ChangeUserPassword", "Invalid Query String UserID"); // Fatal error
                }
                litSavedUserID.Text = userID;                               // Save it in an easier spot to find
                txtFullName.Text = Request.QueryString[PortalConstants.QSFullName]; // Load User's full name for illumination
            }
        }

        // Back out without making any changes

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLStaffDashboard);           // Head for the hills
        }

        // Lookup the user by id, then apply the new password fields

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    var userMgr = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)); // Open path to User Manager
                    userMgr.RemovePassword(litSavedUserID.Text);        // Toss the existing password
                                                                        // If the next step fails, the user account no longer has a password
                    userMgr.AddPassword(litSavedUserID.Text, txtPassword.Text); // Establish the new password

                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ChangeUserPassword", "Unable to Remove and Add Password for user"); // Fatal error
                }
            }

            Response.Redirect(PortalConstants.URLAdminMain + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                        + PortalConstants.QSStatus + "=" + "User password successfully updated");
        }

    }
}
using LinqKit;
using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Admin
{
    public partial class SelectUser : System.Web.UI.Page
    {

        // We get to this page from an Admin menu item that works on a specific user.
        //
        // Query string inputs are:
        //  Command - "Assign" from Assign User To Project
        //  Command - "ChangePassword" from Change Password
        //  Command - "Edit" from Edit User

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                string cmd = Request.QueryString[PortalConstants.QSCommand];  // Look on the query string that invoked this page. Find the command.

                if (cmd == "")                                          // No command. We are invoked incorrectly.
                    LogError.LogQueryStringError("SelectUser", "Missing Query String 'Command'"); // Log fatal error

                litSavedCommand.Text = cmd;                             // Remember the command that invoked this page
                gvPortalUser.PageSize = CookieActions.GetGridViewRows();   // Find number of rows per page from cookie
                LoadgvPortalUser();                                         // Fill the grid
            }
        }

        protected void btnUserSearch_Click(object sender, EventArgs e)
        {
            LoadgvPortalUser();                                              // Refresh the grid using updated search criteria
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void gvPortalUser_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)            // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this User";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvPortalUser, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged

                Label inactiveLabel = (Label)e.Row.FindControl("lblInactive");
                inactiveLabel.Visible = true;                               // Make sure the Inactive column appears if hidden earlier
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void gvPortalUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelectx.Enabled = true;                                  // With a row selected, we can act on a Select button click
        }

        // Deal with pagination of the Grid View controls

        protected void gvPortalUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                gvPortalUser.PageIndex = e.NewPageIndex;                    // Propagate the desired page index
                LoadgvPortalUser();                                         // Fill the grid
                gvPortalUser.SelectedIndex = -1;                            // No row currently selected
            }
        }

        // Cancel, eh? Go to the Staff Dashboard (or wherever).

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            NavigationActions.NextPage("");                             // Figure out where to return to
        }

        // Select button pushed to select a User. Pass this to the another page, based on the Command that invoked us.

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            Label label = (Label)gvPortalUser.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains User ID
            string userID = label.Text;                                 // Extract the text of the control, which is User ID
            if (userID == "")
                LogError.LogInternalError("SelectUser", "UserID not found in selected GridView row"); // Log fatal error

            label = (Label)gvPortalUser.SelectedRow.Cells[1].FindControl("lblFullName"); // Find the label control that contains User Name
            string fullName = label.Text;                               // Pull the full name of the user out as well

            switch (litSavedCommand.Text)                               // Break out by the command that our caller wants us to execute
            {
                case PortalConstants.QSCommandAssign:
                {
                    Response.Redirect(PortalConstants.URLAssignUserToProject + "?" + PortalConstants.QSUserID + "=" + userID + "&" +
                                        PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&" +
                                        PortalConstants.QSStatus + "=" + "Selected user: " + fullName + "&" +
                                        PortalConstants.QSFullName + "=" + fullName);
                    break;
                }
                case PortalConstants.QSCommandChangePassword:
                {
                    Response.Redirect(PortalConstants.URLChangeUserPassword + "?" + PortalConstants.QSUserID + "=" + userID + "&" +
                                        PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&" +
                                        PortalConstants.QSStatus + "=" + "Selected user: " + fullName + "&" +
                                        PortalConstants.QSFullName + "=" + fullName);
                    break;
                }
                case PortalConstants.QSCommandEdit:
                {
                    Response.Redirect(PortalConstants.URLEditRegistration + "?" + PortalConstants.QSUserID + "=" + userID + "&" +
                                        PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&" +
                                        PortalConstants.QSStatus + "=" + "Selected user: " + fullName + "&" +
                                        PortalConstants.QSFullName + "=" + fullName);
                    break;
                }
                default:
                {
                    LogError.LogQueryStringError("SelectUser", string.Format(
                        "Invalid Query String 'Command' value '{0}'", litSavedCommand.Text)); // Log fatal error
                    break;
                }
            }
        }

        protected void chkInactive_CheckedChanged(object sender, EventArgs e)
        {
            LoadgvPortalUser();                                             // Reload the list based on the new un/checked value
        }

        // Fetch all the Users and load them into a GridView

        void LoadgvPortalUser()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // Build a predicate that accounts for the Inactive check box and the Search txe box.

                var pred = PredicateBuilder.True<ApplicationUser>();    // Initialize predicate to select from User table
                if (!chkInactive.Checked)                               // If false, we do not want Inactive Users
                    pred = pred.And(p => !p.Inactive);                  // Only active Users
                string search = txtUser.Text;                           // Fetch the string that the user typed in, if any
                if (search != "")                                       // If != the search string is not blank, use a Contains clause
                    pred = pred.And(p => p.FullName.Contains(search));  // Only Users whose name match our search criteria

                // If we are working for an Assign command, we are only interested in Project users

                if (litSavedCommand.Text == PortalConstants.QSCommandAssign) // If == this is an Assign command
                    pred = pred.And(p => p.UserRole == UserRole.Project); // Select only Project users

                List<ApplicationUser> users = context.Users.AsExpandable().Where(pred).OrderBy(p => p.FullName).ToList(); // Query, sort and make list
                gvPortalUser.DataSource = users;                            // Give it to the GridView control
                gvPortalUser.DataBind();                                    // And display it

                // As a flourish, if the "Include Inactive" checkbox is not checked, do not display the Inactive column

                gvPortalUser.Columns[ApplicationUser.InactiveColumn].Visible = chkInactive.Checked; // If checked, column is visible

                NavigationActions.EnableGridViewNavButtons(gvPortalUser);   // Enable appropriate nav buttons based on page count
            }
        }
    }
}
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

namespace Portal11.Lists
{
    public partial class ListPortalUsers : System.Web.UI.Page
    {

        // Nothing fancy here. Just find all the Portal Users and toss them into a GridView.
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);
                LoadUserView();                                         // Fill the grid
            }
        }

        // Fires as each row of the GridView gets loaded.

        protected void AllPortalUsersView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                Label inactiveLabel = (Label)e.Row.FindControl("lblInactive");
                inactiveLabel.Visible = true;                               // Make sure the Inactive column appears if hidden earlier
            }
            return;
        }

        protected void AllPortalUsersView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                AllPortalUsersView.PageIndex = e.NewPageIndex;          // Propagate the desired page index
                LoadUserView();                                         // Fill the grid
                AllPortalUsersView.SelectedIndex = -1;                  // No row currently selected
            }

        }

        protected void chkInactive_CheckedChanged(object sender, EventArgs e)
        {
            LoadUserView();                                             // Refresh the grid
        }

        protected void btnUserSearch_Click(object sender, EventArgs e)
        {
            LoadUserView();                                             // Refresh the grid
        }

        // Fetch all the Portal Users and load them into a GridView

        void LoadUserView()
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

                List<ApplicationUser> users = context.Users.AsExpandable().Where(pred).OrderBy(p => p.FullName).ToList();
                // Do the query using the constructed predicate, sort the result, and create a list of Dep rows

                // From this list of Deps, build a list of rows for the AllExpView GridView

                List<AllPortalUsersRow> rows = new List<AllPortalUsersRow>(); // Create an empty list for the GridView control
                foreach (var r in users)                                 // Fill the list row-by-row
                {
                    AllPortalUsersRow row = new AllPortalUsersRow()     // Empty row all ready to fill
                    {
                        UserID = r.Id,                                  // Convert ID from int to string for easier retrieval later, if needed
                        FullName = r.FullName,
                        Email = r.Email,
                        UserRoleDesc = EnumActions.GetEnumDescription(r.UserRole), // What the user is up to
                        Administrator = r.Administrator.ToString(),     // Whether user is an Administrator
                        LoginCount = r.LoginCount,                      // How many logins since account created
                        LastLogin = r.LastLogin,                        // Time of last login
                        Inactive = r.Inactive.ToString()                // Whether the account is active
                    };
                    rows.Add(row);                                      // Add the filled-in row to the list of rows
                }

                AllPortalUsersView.DataSource = rows;                   // Give it to the GridView control
                AllPortalUsersView.DataBind();                          // And display it

                // As a flourish, if the "Include Inactive" checkbox is not checked, do not display the Inactive column

                AllPortalUsersView.Columns[AllPortalUsersRow.InactiveColumn].Visible = chkInactive.Checked; // If checked, column is visible

                NavigationActions.EnableGridViewNavButtons(AllPortalUsersView); // Enable appropriate nav buttons based on page count
            }
        }

        protected void btnDone_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLAdminMain);            // Back to Main Admin page
        }

    }
}
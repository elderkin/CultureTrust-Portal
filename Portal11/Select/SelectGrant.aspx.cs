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

namespace Portal11.Select
{
    public partial class SelectGrant : System.Web.UI.Page
    {

        // We get to this page from the Admin user's Edit Grant menu item. The Query String comtains an "Edit" command.
        // We come here to choose the Grant before dispatching to Edit Grant itself.
        //
        // Query string inputs are:
        //  Command - "Edit"

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                string cmd = Request.QueryString[PortalConstants.QSCommand];  // Look on the query string that invoked this page. Find the command.

                if (cmd == "")                                            // If == no command. We are invoked incorrectly.
                    LogError.LogQueryStringError("SelectGrant", "Missing Query String 'Command'"); // Log fatal error

                litSavedCommand.Text = cmd;                               // Remember the command that invoked this page
                LoadGrantView();                                          // Fill the grid
            }
        }

        protected void btnGrantSearch_Click(object sender, EventArgs e)
        {
            LoadGrantView();                                              // Refresh the grid using updated search criteria
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void GrantView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)              // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Grant";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.GrantView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void GrantView_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.Enabled = true;                                     // With a row selected, we can act on a button click
        }

        // Deal with pagination of the Grid View controls

        protected void GrantView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                      // If >= a value that we can handle
            {
                GrantView.PageIndex = e.NewPageIndex;                     // Propagate the desired page index
                LoadGrantView();                                          // Fill the grid
                GrantView.SelectedIndex = -1;                             // No row currently selected
            }
        }

        // Cancel, eh? Go to the Staff Dashboard.

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLStaffDashboard);
        }

        // Select button pushed to select a Grant. Pass this to the EditGrant page.

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            Label label = (Label)GrantView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains RqstID
            string GrantID = label.Text;                                  // Extract the text of the control, which is RqstID
            if (GrantID == "")
                LogError.LogQueryStringError("SelectGrant", 
                    string.Format("GrantID '{0}' from selected GridView row is missing", GrantID)); // Log fatal error

            Response.Redirect("~/Admin/EditGrant?" + PortalConstants.QSGrantID + "=" + GrantID + "&" +
                                               PortalConstants.QSCommand + "=" + litSavedCommand.Text);
        }

        protected void chkInactive_CheckedChanged(object sender, EventArgs e)
        {
            LoadGrantView();                                              // Reload the list based on the new un/checked value
        }

        // Fetch all the Grants and load them into a GridView

        void LoadGrantView()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // Build a predicate that accounts for the Inactive check box and the Search txe box.

                var pred = PredicateBuilder.True<Grant>();            // Initialize predicate to select from Grant table
                if (!chkInactive.Checked)                             // If false, we do not want Inactive Grants
                    pred = pred.And(p => !p.Inactive);                // Only active Grants
                string search = txtGrant.Text;                        // Fetch the string that the user typed in, if any
                if (search != "")                                     // If != the search string is not blank, use a Contains clause
                    pred = pred.And(p => p.Name.Contains(search));    // Only Grants whose name match our search criteria

                List<Grant> projs = context.Grants.AsExpandable().Where(pred).OrderBy(p => p.Name).ToList(); // Query, sort and make list
                GrantView.DataSource = projs;                         // Give it to the GridView control
                GrantView.DataBind();                                 // And display it

                NavigationActions.EnableGridViewNavButtons(GrantView); // Enable appropriate nav buttons based on page count
            }
        }
    }
}
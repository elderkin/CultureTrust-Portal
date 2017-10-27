using LinqKit;
using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Select
{
    public partial class SelectVendor : System.Web.UI.Page
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

                // Fetch and save the Command query string

                if (cmd == "")                                              // No command. We are invoked incorrectly.
                    LogError.LogQueryStringError("SelectVendor", "Missing Query String 'Command'"); // Log fatal error
                litSavedCommand.Text = cmd;                                 // Remember the command that invoked this page

                gvAllVendor.PageSize = CookieActions.GetGridViewRows();  // Find number of rows per page from cookie
                LoadVendorView();                                           // Fill the grid
            }
        }

        protected void btnVendorSearch_Click(object sender, EventArgs e)
        {
            LoadVendorView();                                              // Refresh the grid using updated search criteria
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void gvAllVendor_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Vendor";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvAllVendor, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected. Shared by both controls.

        protected void gvAllVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.Enabled = true;                                       // With a row selected, we can act on a button click
        }

        // Deal with pagination of the Grid View controls

        protected void gvAllVendor_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                gvAllVendor.PageIndex = e.NewPageIndex;                  // Propagate the desired page index
                LoadVendorView();                                          // Fill the grid
                gvAllVendor.SelectedIndex = -1;                          // No row currently selected
            }
        }

        // Cancel, eh? Figure out where to go next.

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            NavigationActions.NextPage("");
        }

        // New button. Invoke Edit Vendor to create a new Vendor

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLEditVendor + "?" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew);
        }

        // Select button pushed to select a Vendor. Dispatch to the page associated with the Command query string parameter - EditVendor.

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            Label vendID = (Label)gvAllVendor.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains VendorID
            Label name = (Label)gvAllVendor.SelectedRow.Cells[1].FindControl("lblName"); // Find the label control that contains Vendor Name
            string vendorID = vendID.Text;                                 // Extract the text of the control, which is VendorID as a string
            if (vendorID == "")
                LogError.LogQueryStringError("SelectVendor", string.Format(
                    "VendorID '{0}' from selected GridView row is missing", vendorID)); // Log fatal error

            Response.Redirect(PortalConstants.URLEditVendor + "?" + PortalConstants.QSVendorID + "=" + vendorID + "&" +
                                            PortalConstants.QSVendorName + "=" + name.Text + "&" +
                                            PortalConstants.QSCommand + "=" + litSavedCommand.Text);
            
        }

        protected void chkInactive_CheckedChanged(object sender, EventArgs e)
        {
            LoadVendorView();                                              // Reload the list based on the new un/checked value
        }

        // Fetch all the Vendors and load them into a GridView. 

        void LoadVendorView()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // Build a predicate that accounts for the Franchise, the Inactive check box and the Search text box.

                var pred = PredicateBuilder.True<Vendor>();            // Initialize predicate to select from Vendor table

                string fran = SupportingActions.GetFranchiseKey();      // Fetch current franchise key
                pred = pred.And(p => p.FranchiseKey == fran);           // Show only Vendors for this franchise

                if (!chkInactive.Checked)                               // If false, we do not want Inactive Vendors
                    pred = pred.And(p => !p.Inactive);                  // Only active Vendors
                string search = txtVendor.Text;                        // Fetch the string that the user typed in, if any
                if (search != "")                                       // If != the search string is not blank, use a Contains clause
                    pred = pred.And(p => p.Name.Contains(search));      // Only Vendors whose name match our search criteria

                List<Vendor> vends = context.Vendors.AsExpandable().Where(pred).OrderBy(p => p.Name).ToList(); // Query, sort and make list
                gvAllVendor.DataSource = vends;                      // Give it to the GridView control
                gvAllVendor.DataBind();                              // And display it

                NavigationActions.EnableGridViewNavButtons(gvAllVendor); // Enable appropriate nav buttons based on page count
            }
        }
    }
}
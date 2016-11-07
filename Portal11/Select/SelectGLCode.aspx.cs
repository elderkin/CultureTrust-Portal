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
    public partial class SelectGLCode : System.Web.UI.Page
    {
        // We get to this page from the Admin user's Edit GLCode menu item. The Query String comtains an "Edit" command.
        // We come here to choose the GLCode before dispatching to Edit GLCode itself.
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

                if (cmd == "")                                              // If == no command. We are invoked incorrectly.
                    LogError.LogQueryStringError("SelectGLCode", "Missing Query String 'Command'"); // Log fatal error

                litSavedCommand.Text = cmd;                                 // Remember the command that invoked this page
                GLCodeView.PageSize = CookieActions.FindGridViewRows();     // Find number of rows per page from cookie
                LoadGLCodeView();                                           // Fill the grid
            }
        }

        protected void btnGLCodeSearch_Click(object sender, EventArgs e)
        {
            LoadGLCodeView();                                              // Refresh the grid using updated search criteria
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void GLCodeView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)              // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this GLCode";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.GLCodeView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged

                Label inactiveLabel = (Label)e.Row.FindControl("lblInactive");
                inactiveLabel.Visible = true;                               // Make sure the Inactive column appears if hidden earlier
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void GLCodeView_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.Enabled = true;                                     // With a row selected, we can act on a button click
        }

        // Deal with pagination of the Grid View controls

        protected void GLCodeView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                      // If >= a value that we can handle
            {
                GLCodeView.PageIndex = e.NewPageIndex;                     // Propagate the desired page index
                LoadGLCodeView();                                          // Fill the grid
                GLCodeView.SelectedIndex = -1;                             // No row currently selected
            }
        }

        // Cancel, eh? Go to the Staff Dashboard (or wherever).

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            NavigationActions.NextPage("");
        }

        // Head for EditGLCode to create a new GL Code

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLEditGLCode + "?" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew);
        }

        // Select button pushed to select a GLCode. Pass this to the EditGLCode page.

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            Label label = (Label)GLCodeView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains RqstID
            string GLCodeID = label.Text;                                  // Extract the text of the control, which is RqstID
            if (GLCodeID == "")
                LogError.LogQueryStringError("SelectGLCode",
                    string.Format("GLCodeID '{0}' from selected GridView row is missing", GLCodeID)); // Log fatal error

            Response.Redirect(PortalConstants.URLEditGLCode + "?" + PortalConstants.QSGLCodeID + "=" + GLCodeID + "&" +
                                               PortalConstants.QSCommand + "=" + litSavedCommand.Text);
        }

        // This procedure services all of the filtering checkboxes.

        protected void chkAny_CheckedChanged(object sender, EventArgs e)
        {
            LoadGLCodeView();                                              // Reload the list based on the new un/checked value
        }

        // Fetch all the GLCodes and load them into a GridView

        void LoadGLCodeView()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // Build a predicate that accounts for the Inactive check box and the Search txe box.

                var pred = PredicateBuilder.True<GLCode>();             // Initialize predicate to select from GLCode table
                if (!chkInactive.Checked)                               // If false, we do not want Inactive GLCodes
                    pred = pred.And(p => !p.Inactive);                  // Only active GLCodes
                if (!chkDeposit.Checked)                                // If false, we do not want Deposit GL Codes
                    pred = pred.And(p => !p.DepCode);                   // No Deposit codes
                if (!chkExpense.Checked)                                // If false, we do not want Expense GL Codes
                    pred = pred.And(p => !p.ExpCode);                   // No Expense codes
                string search = txtGLCode.Text;                         // Fetch the string that the user typed in, if any
                if (search != "")                                       // If != the search string is not blank, use a Contains clause
                    pred = pred.And(p => p.Code.Contains(search));      // Only GLCodes whose code match our search criteria
                string franchiseKey = SupportingActions.GetFranchiseKey(); // Find current key value
                pred = pred.And(p => p.FranchiseKey == franchiseKey);   // Only GLCodes for this Franchise

                List<GLCode> codes = context.GLCodes.AsExpandable().Where(pred).OrderBy(p => p.Code).ToList(); // Query, sort and make list
                GLCodeView.DataSource = codes;                         // Give it to the GridView control
                GLCodeView.DataBind();                                 // And display it

                // As a flourish, if the "Include Inactive" checkbox is not checked, do not display the Inactive column

                GLCodeView.Columns[GLCode.InactiveColumn].Visible = chkInactive.Checked; // If checked, column is visible

                NavigationActions.EnableGridViewNavButtons(GLCodeView); // Enable appropriate nav buttons based on page count
            }
        }
    }
}
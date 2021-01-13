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
    public partial class SelectProjectClass : System.Web.UI.Page
    {

        // We get to this page from the Project menu's Edit Project Class menu item. The Query String comtains an "Edit" command.
        // We come here to choose the Project Class before dispatching to EditProjectClass itself.
        //
        // So far, editing is all we do, so there is no need for a Command Query String.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                //string cmd = Request.QueryString[PortalConstants.QSCommand]; // Look on the query string that invoked this page. Find the command.

                //if (cmd == "")                                              // No command. We are invoked incorrectly.
                //    LogError.LogQueryStringError("EditProjectClasses", "Missing Query String 'Command'"); // Log fatal error

                litSavedCommand.Text = PortalConstants.QSCommandEdit;       // Always Edit, at least for now
                gvProjectClass.PageSize = CookieActions.GetGridViewRows();  // Find number of rows per page from cookie
                LoadgvProjectClass();                                     // Fill the grid
            }
        }

        protected void btnProjectClassSearch_Click(object sender, EventArgs e)
        {
            LoadgvProjectClass();                                         // Refresh the grid using updated search criteria
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void gvProjectClass_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Department";       // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvProjectClass, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged

                Label inactiveLabel = (Label)e.Row.FindControl("lblInactive");
                inactiveLabel.Visible = true;                               // Make sure the Inactive column appears if hidden earlier
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void gvProjectClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.Enabled = true;                                       // With a row selected, we can act on a button click
        }

        // Deal with pagination of the Grid View controls

        protected void gvProjectClass_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                gvProjectClass.PageIndex = e.NewPageIndex;                // Propagate the desired page index
                LoadgvProjectClass();                                     // Fill the grid
                gvProjectClass.SelectedIndex = -1;                        // No row currently selected
            }
        }

        // Cancel, eh? Go to the Staff Dashboard (or wherever).

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            NavigationActions.NextPage("");
        }

        // Select button pushed to select a ProjectClass. Pass this to the EditProjectClass page.

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            Label label = (Label)gvProjectClass.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains RqstID
            string ProjectClassID = label.Text;                             // Extract the text of the control, which is RqstID
            if (ProjectClassID == "")
                LogError.LogQueryStringError("EditProjectClasses", string.Format(
                    "ProjectClassID '{0}' from selected GridView row missing", ProjectClassID)); // Log fatal error


            Response.Redirect(PortalConstants.URLEditProjectClass + "?" + PortalConstants.QSProjectClassID + "=" + ProjectClassID + "&" +
                                               PortalConstants.QSCommand + "=" + litSavedCommand.Text);
        }

        // Create a new ProjectClass by invoking EditProjectClass with a "New" command

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLEditProjectClass + "?" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew);
        }

        protected void chkInactive_CheckedChanged(object sender, EventArgs e)
        {
            LoadgvProjectClass();                                         // Reload the list based on the new un/checked value
        }

        // Fetch all the ProjectClasses and load them into a GridView

        void LoadgvProjectClass()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // Build a predicate that accounts for the Inactive check box and the Search text box.

                var pred = PredicateBuilder.True<ProjectClass>();           // Initialize predicate to select from ProjectClass table
                if (!chkInactive.Checked)                                   // If false, we do not want Inactive ProjectClasss
                    pred = pred.And(p => !p.Inactive);                      // Only active ProjectClasss
                string search = txtProjectClass.Text;                       // Fetch the string that the user typed in, if any
                if (search != "")                                           // If != the search string is not blank, use a Contains clause
                    pred = pred.And(p => p.Name.Contains(search));          // Only ProjectClasss whose name match our search criteria

                // Filter by current project

                HttpCookie projectInfoCookie = Request.Cookies[PortalConstants.CProjectInfo]; // Find the Project Info cookie
                if (projectInfoCookie == null)                              // If == cookie is missing
                    LogError.LogQueryStringError("ProjectDashboard", "Unable to find ProjectID in Query String or Project Info Cookie. User does not have a project"); // Fatal error
                string projID = projectInfoCookie[PortalConstants.CProjectID]; // Fetch ProjectID from cookie
                int projIDint = Convert.ToInt32(projID);                    // Produce int version of this key
                pred = pred.And(p => p.ProjectID == projIDint);             // Restrict search to Project Classes for current project

                List<ProjectClass> projs = context.ProjectClasses.AsExpandable().Where(pred).OrderBy(p => p.Name).ToList(); // Query, sort and make list
                gvProjectClass.DataSource = projs;                        // Give it to the GridView control
                gvProjectClass.DataBind();                                // And display it

                // As a flourish, if the "Include Inactive" checkbox is not checked, do not display the Inactive column

                gvProjectClass.Columns[ProjectClass.InactiveColumn].Visible = chkInactive.Checked; // If checked, column is visible

                NavigationActions.EnableGridViewNavButtons(gvProjectClass); // Enable appropriate nav buttons based on page count
            }
        }
    }
}
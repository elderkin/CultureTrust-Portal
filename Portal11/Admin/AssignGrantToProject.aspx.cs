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
    public partial class AssignGrantToProject : System.Web.UI.Page
    {

        // We need to know which Project we're working on. Query Strings:
        //  ProjectID - which project we're working on (from SelectProject)
        //  ProjectName - it's name

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                string projectIDText = Request.QueryString[PortalConstants.QSProjectID]; // Fetch the query string for Project ID
                if (projectIDText == "")                                    // No project. We are invoked incorrectly.
                    LogError.LogQueryStringError("AssignGrantToProject", "Missing Query String 'ProjectID'"); // Log the fatal error

                litSavedProjectID.Text = projectIDText;                     // Save it in an easier spot to find
                litProjectName.Text = Request.QueryString[PortalConstants.QSProjectName]; // Fetch the Project ID, stash in on-screen literal

                LoadProjectGrantView(projectIDText);                        // Fill the left grid
                LoadAllGrantView(projectIDText);                            // Fill the right grid
            }
        }

        // Buttons to search using specified Contains criteria. As you can see, the real work is elsewhere.

        protected void btnAllGrantSearch_Click(object sender, EventArgs e)
        {
            LoadAllGrantView(litSavedProjectID.Text);                    // Fill list, reacting to user-supplied search string
        }

        protected void btnProjectGrantSearch_Click(object sender, EventArgs e)
        {
            LoadProjectGrantView(litSavedProjectID.Text);                // Fill list, reacting to user-supplied search string
        }

        // Deal with pagination of the Grid View controls

        protected void AllGrantView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                AllGrantView.PageIndex = e.NewPageIndex;                 // Propagate the desired page index
                LoadAllGrantView(litSavedProjectID.Text);                // Reload the grid view control
                AllGrantView.SelectedIndex = -1;                         // No row currently selected
            }
        }

        protected void ProjectGrantView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                ProjectGrantView.PageIndex = e.NewPageIndex;             // Propagate the desired page index
                LoadProjectGrantView(litSavedProjectID.Text);            // Reload the grid view control
                ProjectGrantView.SelectedIndex = -1;                     // No row currently selected
            }
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void AllGrantView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Grant";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.AllGrantView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged
            }
        }

        protected void ProjectGrantView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Grant";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.ProjectGrantView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged
            }
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void AllGrantView_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAddx.Enabled = true;                                          // We can move from All Grants to Project Grants
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void ProjectGrantView_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemovex.Enabled = true;                                      // We can move from Project Grants to All Grants
        }

        // Add button clicked. Fetch the selected row from the Grant table and add its Grant ID to the Project Grant table.

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Label label = (Label)AllGrantView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains GrantID
            if (label.Text == "")                                           // If == there is no ID value. That's an error.
                LogError.LogInternalError("AssignGrantToProject", "Grant ID not found in selected GridView row"); // Log the fatal error

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    ProjectGrant pg = new ProjectGrant()
                    {            // Get an empty record and initialize it
                        StartDate = System.DateTime.Now,                    // Remember when the Grant joined the project
                        ProjectID = Convert.ToInt32(litSavedProjectID.Text), // Stash the ID of this Project
                        GrantID = Convert.ToInt32(label.Text)            // The text of this control contains Grant ID
                    };
                    context.ProjectGrants.Add(pg);                       // Add this new row
                    context.SaveChanges();
                    LoadAllGrantView(litSavedProjectID.Text);            // Refresh the lists
                    LoadProjectGrantView(litSavedProjectID.Text);

                    AllGrantView.SelectedIndex = -1;                     // Unselect all rows
                    litSuccessMessage.Text = "Successfully added Grant to Project"; litDangerMessage.Text = null; // Report success to user
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AssignGrantToProject", "Error writing ProjectGrant row"); // Fatal error
                }
            }
        }

        // Remove button clicked. Delete the row from the Project Grant table. Note that the GridView contains the ProjectGrantID -
        // the primary key of the row.

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            Label label = (Label)ProjectGrantView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains ProjectGrantID
            if (label.Text == "")                                           // If == there is no ID value. That's an error.
                LogError.LogInternalError("AssignGrantToProject", "ProjectGrantID not found in selected GridView row"); // Log fatal error

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    ProjectGrant pg = new ProjectGrant() { ProjectGrantID = Convert.ToInt32(label.Text) }; // Create shell of record
                    context.ProjectGrants.Attach(pg);                    // Don't query it, but just connect to the relevant row
                    context.ProjectGrants.Remove(pg);                    // Delete that row
                    context.SaveChanges();
                    LoadAllGrantView(litSavedProjectID.Text);            // Refresh the lists
                    LoadProjectGrantView(litSavedProjectID.Text);

                    ProjectGrantView.SelectedIndex = -1;                 // Unselect all rows
                    litSuccessMessage.Text = "Successfully removed Grant from Project"; litDangerMessage.Text = null; // Report success to user
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AssignGrantToProject", "Error removing ProjectGrant row"); // Fatal error
                }
            }
        }

        // Remove All button clicked. Just blast everything for this project out of the ProjectGrant database.

        protected void btnRemoveAll_Click(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    string projectIDText = litSavedProjectID.Text;          // Fetch the project ID
                    int projectID = Convert.ToInt32(projectIDText);         // Also produce the integer version
                    context.ProjectGrants.RemoveRange(context.ProjectGrants.Where(p => p.ProjectID == projectID)); // Blow out all the
                    // Grants for the current project
                    context.SaveChanges();
                    LoadAllGrantView(projectIDText);                        // Refresh the lists
                    LoadProjectGrantView(projectIDText);
                    litSuccessMessage.Text = "Successfuly removed all Grants from Project"; // Report success to user
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AssignGrantToProject", "Error removing all ProjectGrant rows"); // Fatal error
                }
            }
        }

        // Done button clicked. Zero grants on a Project is legal. Then return to the Main Admin page.

        protected void btnDone_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLAdminMain);                // Back to home base
        }

        // Fetch all the Grants on this project and load them into a GridView

        void LoadProjectGrantView(string projectIDText)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                int projectID = Convert.ToInt32(projectIDText);         // Convert string version to integer
                var query = from pg in context.ProjectGrants
                            where pg.ProjectID == projectID
                            orderby pg.Grant.Name
                            select pg;
                string search = txtProjectGrant.Text;                // Fetch the string that the user typed in, if any
                if (search != "")                                       // If != the search string is not blank. Use a Contains clause
                {
                    query = from pg in context.ProjectGrants
                            where pg.ProjectID == projectID && pg.Grant.Name.Contains(search)
                            orderby pg.Grant.Name
                            select pg;
                }

                List<ProjectGrant> em = query.ToList();              // Deliver a row for ProjectGrant for the current Project
                ProjectGrantView.DataSource = em;                    // Give it to the GridView cnorol
                ProjectGrantView.DataBind();                         // And display it

                NavigationActions.EnableGridViewNavButtons(ProjectGrantView); // Enable appropriate nav buttons based on page count
                btnRemoveAllx.Enabled = true;                           // Assume that there are rows here which could be removed
                if (em.Count() == 0)                                    // If == there are no rows displayed here
                    btnRemoveAllx.Enabled = false;                      // So the "Remove All" button is not useful here
            }
        }

        // Fetch all the Grants and load them into a GridView. In the process, filter out all of the Grants that are already
        // associated with this project.

        void LoadAllGrantView(string projectIDText)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // Start by finding all the Grants that meet the search criteria

                var queryG = from g in context.Grants                 // Assume search string is blank
                             where !g.Inactive
                             orderby g.Name
                             select g;
                string search = txtAllGrant.Text;                    // Fetch the string that the user typed in, if any
                if (search != "")                                       // If != the search string is not blank. Use a Contains clause
                {
                    queryG = from g in context.Grants
                             where !g.Inactive && g.Name.Contains(search)
                             orderby g.Name
                             select g;
                }

                // Now find all the Project Grants. We don't care about the search string at this point, we're excluding ALL the project Grants

                int projectID = Convert.ToInt32(projectIDText);         // Convert string version to integer
                var queryPG = from pg in context.ProjectGrants
                              where pg.ProjectID == projectID
                              orderby pg.Grant.Name
                              select pg.Grant;

                List<Grant> gr = queryG.Except(queryPG).ToList();    // List of Grants except those that are returned by Project Grant query
                AllGrantView.DataSource = gr;                        // Give it to the GridView cnorol
                AllGrantView.DataBind();                             // And display it

                NavigationActions.EnableGridViewNavButtons(AllGrantView); // Enable appropriate nav buttons based on page count
            }
        }
    }
}
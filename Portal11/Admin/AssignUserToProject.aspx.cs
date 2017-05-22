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
    public partial class AssignUserToProject : System.Web.UI.Page
    {

        // We get to this page from the Admin user's User -> Assign User to Project menu item. It passed control to Select User, which then
        // invoked us. We give our user the chance to make someone a Project Director or Project Staff.
        //
        // Query string inputs are:
        //  UserID - The user ID of the user to be assigned
        //  FullName - The full name of the user to be assigned

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                string userID = Request.QueryString[PortalConstants.QSUserID];  // Look on the query string that invoked this page. Find the user ID.
                string fullName = Request.QueryString[PortalConstants.QSFullName]; // Also look for user's full name, which may be blank

                if (userID == "")                                           // If == no command. We are invoked incorrectly.
                    LogError.LogQueryStringError("AssignUserToProject", "Missing Query String 'UserID'"); // Log fatal error

                litSavedUserID.Text = userID;                               // Remember the selected user's ID
                litSavedFullName.Text = fullName;                           // Also remember user's full name

                gvAssignUserToProject.PageSize = CookieActions.FindGridViewRows(); // Find number of rows per page from cookie

                LoadProjectView();                                          // Fill the grid
            }
        }

        protected void btnProjectSearch_Click(object sender, EventArgs e)
        {
            LoadProjectView();                                              // Refresh the grid using updated search criteria
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void gvAssignUserToProject_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Project";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvAssignUserToProject, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged
            }
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void gvAssignUserToProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label label = (Label)gvAssignUserToProject.SelectedRow.Cells[1].FindControl("lblProjectRole");
            // Find the label control that contains Project Role of User on this Project
            ProjectRole r = EnumActions.ConvertTextToProjectRole(label.Text); // Convert string back into enum datatype
            AdjustButtons(r);                                               // Turn on the appropriate buttons, based on role
        }

        // Deal with pagination of the Grid View controls

        protected void gvAssignUserToProject_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                gvAssignUserToProject.PageIndex = e.NewPageIndex;         // Propagate the desired page index
                LoadProjectView();                                          // Fill the grid
                gvAssignUserToProject.SelectedIndex = -1;                 // No row currently selected
            }
        }

        // Cancel, eh? Go back to where we came.

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLAdminMain);                // Back to the barn. 
        }

        // Add Director, Add Staff and Remove buttons are all quite similar, so we process them together

        protected void btnAddDirector_Click(object sender, EventArgs e)
        {
            ChangeAssociation(ProjectRole.ProjectDirector);                 // Make the User a Project Director for this Project

            litSuccessMessage.Text = "Successfully added User '" + litSavedFullName.Text + "' as " +
                EnumActions.GetEnumDescription(ProjectRole.ProjectDirector); // Tell the user of our success
        }

        protected void btnAddStaff_Click(object sender, EventArgs e)
        {
            ChangeAssociation(ProjectRole.ProjectStaff);                // Make the User a Project Staff for this Project

            litSuccessMessage.Text = "Successfully added User '" + litSavedFullName.Text + "' as " +
                EnumActions.GetEnumDescription(ProjectRole.ProjectStaff); // Tell the user of our success
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            ChangeAssociation(ProjectRole.NoRole);                      // Remove the user from any role on this Project

            litSuccessMessage.Text = "Successfully removed User from Project role"; // Tell the user that worked
        }

        protected void chkInactive_CheckedChanged(object sender, EventArgs e)
        {
            LoadProjectView();                                              // Reload the list based on the new un/checked value
        }

        // Change the User's role on the project.
        //  1) Delete the UserProject row that describes the current relationship, if any.
        //  2) Add a new UserProject row that describes the desired relationship

        void ChangeAssociation(ProjectRole role)
        {
            Label label = (Label)gvAssignUserToProject.SelectedRow.Cells[0].FindControl("lblRowID");
            // Find the label control that contains Project ID
            string p = label.Text;                                          // Extract the text of the control, which is Project ID
            if (p == "")                                                    // If == value is missing. That's an error
                LogError.LogQueryStringError("AssignUserToProject", "ProjectID not found in selected GridView row"); // Log fatal error

            StateActions.ChangeProjectRole(Convert.ToInt32(p), litSavedUserID.Text, role); // Do the heavy lifting

            LoadProjectView();                                          // Refresh the grid view
            AdjustButtons(role);                                        // Switch the appropriate buttons on and off
        }

        // Fetch all the Projects and load them into a GridView. We also display the current Project Director, if any, and whether the
        // current user has a role on the Project.
        //  1) Fetch all of the projects that match our search criteria.
        //  2) Fetch all of the projects this user is AssignUserTod with - there could be many.
        //  3) Craft a list for the gridview, with one row for each project (our first list) and extras from the second list.

        void LoadProjectView()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                //  1) Fetch all of the projects that match our search criteria.
                // Build a predicate that accounts for the Inactive check box and the Search txe box.

                var pred = PredicateBuilder.True<Project>();            // Initialize predicate to select from Project table
                string fran = SupportingActions.GetFranchiseKey();      // Fetch current franchise key
                //if (!chkInactive.Checked)                               // If false, we do not want Inactive Projects
                pred = pred.And(p => !p.Inactive && (p.FranchiseKey == fran)); // Only active Projects for the current franchise
                string search = txtProject.Text;                        // Fetch the string that the user typed in, if any
                if (search != "")                                       // If != the search string is not blank, use a Contains clause
                    pred = pred.And(p => p.Name.Contains(search));      // Only Projects whose name match our search criteria

                List<Project> projs = context.Projects.AsExpandable().Where(pred).OrderBy(p => p.Name).ToList(); // Query, sort and make list

                //  2) Fetch all of the projects this user is assigned to with - there could be many.

                var query = from up in context.UserProjects
                            where up.UserID == litSavedUserID.Text
                            select up;                                  // Find all UserProject rows for this User ID
                List<UserProject> userProjs = query.ToList();           // Fetch all those rows into a new list

                //  3) Craft a list for the gridview, with one row for each project (our first list) and extras from the second list.

                List<rowAssignUserToProject> rows = new List<rowAssignUserToProject>(); // Create an empty list for the GridView control
                foreach (var p in projs)                                // Fill the list row-by-row
                {
                    rowAssignUserToProject row = new rowAssignUserToProject();  // Instantiate empty row all ready to fill
                    row.RowID = p.ProjectID.ToString();                 // Fill the part of the row that's always there
                    row.Name = p.Name;
                    row.Description = p.Description;

                    // See if the Project currently has a Project Director. If so, include it in the row

                    row.CurrentPD = "";                                 // Assume that there is no Project Director
                    var query2 = from up in context.UserProjects
                                 where (up.ProjectID == p.ProjectID) && (up.ProjectRole == ProjectRole.ProjectDirector)
                                 select new { up.User.FullName };       // Find the name of the User who is PD on this Project
                    var f = query2.FirstOrDefault();                    // Pull the row of that user
                    if (f != null)                                      // If != the Project has a Project Director and we found it
                    {
                        row.CurrentPD = f.FullName;                     // Store this value for display. Might be the current user.
                    }

                    // If the User is currently Project Director or Project Staff on the Project, mention that

                    row.UserRole = "-- no role --";                     // Assume User does not have a role on this project
                    row.ProjectRole = ProjectRole.NoRole;               // Also bury that fact as an invisible flag within the row
                    foreach (var up in userProjs)                       // Scan the User's projects one-by-one
                    {
                        if (up.ProjectID == p.ProjectID)                // If == the User has a role on this Project
                        {
                            row.UserRole = EnumActions.GetEnumDescription(up.ProjectRole); // Fill in the User's role on this Project
                            row.ProjectRole = up.ProjectRole;           // Both visible and invisible representations
                            break;                                      // No need to look at other UserProject rows
                        }
                    }
                    rows.Add(row);                                      // Add the filled-in row to the list of rows
                }

                // Just one last thing. If the selected user has a non-blank full name, use it to formulate a column heading in the grid view
                // to provide a little more clarity.

                if (litSavedFullName.Text != "")                        // If != there is a useful full name available
                    gvAssignUserToProject.Columns[5].HeaderText = "'" + litSavedFullName.Text + "' Role"; // Provide a meaningful column header

                gvAssignUserToProject.DataSource = rows;                 // Give it to the GridView control
                gvAssignUserToProject.DataBind();                        // And display it

                NavigationActions.EnableGridViewNavButtons(gvAssignUserToProject); // Enable appropriate nav buttons based on page count
            }
        }

        // A row is selected. Now turn the right buttons on and off, depending on the role that this User now has

        void AdjustButtons(ProjectRole role)
        {
            switch (role)
            {
                case ProjectRole.NoRole:                                    // User currently has no role on the selected project
                    {
                        btnAddDirector.Enabled = true;
                        btnAddStaff.Enabled = true;
                        btnRemove.Enabled = false;
                        break;
                    }
                case ProjectRole.ProjectDirector:                           // User is currently the Project Director of selected project
                    {
                        btnAddDirector.Enabled = false;
                        btnAddStaff.Enabled = true;
                        btnRemove.Enabled = true;
                        break;
                    }
                case ProjectRole.ProjectStaff:                              // User is currently a Project Staff of the selected project
                    {
                        btnAddDirector.Enabled = true;
                        btnAddStaff.Enabled = false;
                        btnRemove.Enabled = true;
                        break;
                    }
                default:
                    {
                        LogError.LogInternalError("AssignUserToProject", string.Format("Invalid ProjectRole value '{0}' in database",
                            role.ToString()));                              // Fatal error
                        break;
                    }
            }
        }
    }
}
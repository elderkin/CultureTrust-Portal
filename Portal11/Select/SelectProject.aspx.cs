using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal11.Models;
using Portal11.Logic;
using Portal11.ErrorLog;
using System.Drawing;

namespace Portal11.Staff
{
    public partial class SelectProject : System.Web.UI.Page
    {

        // We get to this page from lots of different directions.
        //  From LoginDispatch (during User login) when a project must be selected. Dispatch to ProjectDashboard.
        //      Command - "UserLogin"
        //  From Menu item "Select Project". Dispatch to ProjectDashboard. Functionally identical to "UserLogin."
        //      Command - "Menu"
        //  From Admin Main page (Assign Grant command). Dispatch to AssignGrant.
        //      Command - "AssignEntitys"
        //  From Admin Main page (Assign Entitys to Project command). Dispatch to AssignEntitysToProject.
        //      Command - "AssignGrant"
        //  From Admin Main page (Assign Persons to Project command). Dispatch to AssitgnPersonsToProject.
        //      Command = "AssignPersons"
        //  From Admin Main page (Edit Project). Dispatch to EditProject.
        //      Command = "Edit"

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                string cmd = Request.QueryString[PortalConstants.QSCommand];  // Look on the query string that invoked this page. Find the command.

                // Fetch and save the Command query string

                if (cmd == "")                                              // No command. We are invoked incorrectly.
                    LogError.LogQueryStringError("SelectProject", "Missing Query String 'Command'"); // Log fatal error

                litSavedCommand.Text = cmd;                                 // Remember the command that invoked this page

                // Fetch and save the UserID and ProjectSelector from the UserInfoCookie

                HttpCookie userInfoCookie = Request.Cookies[PortalConstants.CUserInfo]; // Ask for the User Info cookie, if any
                string userID = userInfoCookie[PortalConstants.CUserID];    // Fetch ID of current user
                string projectSelector = userInfoCookie[PortalConstants.CUserProjectSelector]; // Fetch project selector of current user
                string role = userInfoCookie[PortalConstants.CUserRole];    // Fetch role of this user
                if (userID == "" || projectSelector == "" || role == "")    // No cookie values. We are invoked incorrectlly
                    LogError.LogInternalError("SelectProject", "Missing UserInfoCookie"); // Log fatal error
                litSavedUserID.Text = userID;                               // Save for later
                litSavedProjectSelector.Text = projectSelector;             // Save for later
                litSavedRole.Text = role;                                   // Save for later

                // Tweak the page based on whether we working "All" or "User" search

                if (projectSelector == PortalConstants.CUserProjectAll)     // If == we're working an "All" search
                {
                    pnlInactive.Visible = true;                             // We do need the Inactive checkbox
                    pnlAllProject.Visible = true; pnlUserProject.Visible = false; // Choose the AllProject panel
                }
                else                                                        // We're working a "User" search
                {
                    pnlInactive.Visible = false;                            // We do not need the Inactive checkbox
                    pnlAllProject.Visible = false; pnlUserProject.Visible = true; // Choose the UserProject panel
                }

                if (cmd == PortalConstants.QSCommandEdit)                   // If == we are selecting a Project to edit
                    btnNew.Enabled = true;                                  // Offer the opportunity to create a new Project

                AllProjectView.PageSize = CookieActions.FindGridViewRows();  // Find number of rows per page from cookie
                UserProjectView.PageSize = CookieActions.FindGridViewRows();  // Find number of rows per page from cookie
                LoadProjectView();                                          // Fill the grid
            }
        }

        protected void btnProjectSearch_Click(object sender, EventArgs e)
        {
            LoadProjectView();                                              // Refresh the grid using updated search criteria
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void AllProjectView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Project";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.AllProjectView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged

                //Label rowIDLabel = (Label)e.Row.FindControl("lblRowID");    // Find the ProjectID
                //int projectID = Convert.ToInt32(rowIDLabel.Text);           // Convert string to number

                Label countLabel = (Label)e.Row.FindControl("lblCount");    // Find the Count
                int count = Convert.ToInt32(countLabel.Text);               // Convert string to number
                if (count > 0)                                              // If >0 there is work to do on this project
                {
                    e.Row.Font.Bold = true;                                 // If we get here, User can act on the row. Bold the row.
                }

                Label inactiveLabel = (Label)e.Row.FindControl("lblInactive");
                inactiveLabel.Visible = true;                               // Make sure the Inactive column appears if hidden earlier
            }
            return;
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void UserProjectView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Project";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.UserProjectView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged

                Label countLabel = (Label)e.Row.FindControl("lblCount");    // Find the Count
                int count = Convert.ToInt32(countLabel.Text);               // Convert string to number
                if (count > 0)                                              // If >0 there is work to do on this project
                {
                    e.Row.Font.Bold = true;                                 // If we get here, User can act on the row. Bold the row.
                }

            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected. Shared by both controls.

        protected void ProjectView_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.Enabled = true;                                       // With a row selected, we can act on a button click
        }

        // Deal with pagination of the Grid View controls

        protected void AllProjectView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                AllProjectView.PageIndex = e.NewPageIndex;                  // Propagate the desired page index
                LoadProjectView();                                          // Fill the grid
                AllProjectView.SelectedIndex = -1;                          // No row currently selected
            }
        }

        // Deal with pagination of the Grid View controls

        protected void UserProjectView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                UserProjectView.PageIndex = e.NewPageIndex;                 // Propagate the desired page index
                LoadProjectView();                                          // Fill the grid
                UserProjectView.SelectedIndex = -1;                         // No row currently selected
            }
        }

        // Cancel, eh? Figure out where to go next.

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            NavigationActions.NextPage("");
        }

        // New button. Invoke Edit Project to create a new Project

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLEditProject + "?" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew);
        }

        // Select button pushed to select a project. Dispatch to the page associated with the Command query string parameter.

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            Label projID = new Label();
            Label name = new Label();
            if (litSavedProjectSelector.Text == PortalConstants.CUserProjectAll) // If == we're working an "All" search
            {
                projID = (Label)AllProjectView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains ProjectID
                name = (Label)AllProjectView.SelectedRow.Cells[1].FindControl("lblName"); // Find the label control that contains Project Name
            }
            else                                                            // We're working on a "User" search
            {
                projID = (Label)UserProjectView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains RqstID
                name = (Label)UserProjectView.SelectedRow.Cells[1].FindControl("lblName"); // Find the label control that contains Project Name
            }
            string projectID = projID.Text;                                 // Extract the text of the control, which is ProjectID as a string
            if (projectID == "")
                LogError.LogQueryStringError("SelectProject", $"ProjectID '{projectID}' from selected GridView row is missing"); // Log fatal error

            switch (litSavedCommand.Text)
            {
                case PortalConstants.QSCommandAssignEntitys:
                    {
                        Response.Redirect(PortalConstants.URLAssignEntitysToProject + "?" + PortalConstants.QSProjectID + "=" + projectID + "&" +
                                                            PortalConstants.QSProjectName + "=" + name.Text + "&" +
                                                            PortalConstants.QSCommand + "=" + litSavedCommand.Text);
                        break;
                    }
                case PortalConstants.QSCommandAssignGrant:
                {
                    Response.Redirect("~/Admin/AssignGrantToProject?" + PortalConstants.QSProjectID + "=" + projectID + "&" +
                                                        PortalConstants.QSProjectName + "=" + name.Text + "&" +
                                                        PortalConstants.QSCommand + "=" + litSavedCommand.Text);
                    break;
                }
                case PortalConstants.QSCommandAssignPersons:
                    {
                        Response.Redirect(PortalConstants.URLAssignPersonsToProject + "?" + PortalConstants.QSProjectID + "=" + projectID + "&" +
                                                            PortalConstants.QSProjectName + "=" + name.Text + "&" +
                                                            PortalConstants.QSCommand + "=" + litSavedCommand.Text);
                        break;
                    }
                case PortalConstants.QSCommandEdit:
                {
                    Response.Redirect(PortalConstants.URLEditProject + "?" + PortalConstants.QSProjectID + "=" + projectID + "&" +
                                                    PortalConstants.QSProjectName + "=" + name.Text + "&" +
                                                    PortalConstants.QSCommand + "=" + litSavedCommand.Text);
                    break;
                }

                // A little work to do for these cases. We have ProjectID, but we need more information to fill in the ProjectInfoCookie. So lookup
                // the UserProject row that the User selected and copy down information. Then dispatch to Project Dashboard.

                case PortalConstants.QSCommandUserLogin:
                case PortalConstants.QSCommandMenu:
                {
                    using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                    {
                        int p = Convert.ToInt32(projectID);                 // Form integer version of ProjectID
                        if (litSavedProjectSelector.Text == PortalConstants.CUserProjectAll) // If == we are choosing among all Projects, not just User's
                        {
                            Project proj = context.Projects.Find(p);        // Find the row for the selected Project
                            if (proj == null)                               // Cannot find the needed row
                                LogError.LogInternalError("SelectProject", $"Unable to find Project with ProjectID '{projectID}' in database"); // Fatal error

                            UserRole role = EnumActions.ConvertTextToUserRole(litSavedRole.Text);
                            CookieActions.MakeProjectInfoCookie(proj, role); // Copy from database row to cookie

                            Response.Redirect(PortalConstants.URLProjectDashboard + "?" + 
                                                    PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&" +
                                                    PortalConstants.QSProjectID + "=" + projectID);
                        }
                        else
                        {
                            var query = from up in context.UserProjects
                                        where up.ProjectID == p && up.UserID == litSavedUserID.Text
                                        select up;
                            UserProject userProject = query.SingleOrDefault(); // Fetch the UserProject record for the logged in user
                            if (userProject == null)                        // Cannot re-locate record used to populate GridView
                            {
                                LogError.LogInternalError("SelectProject", $"Unable to find UserProject with ProjectID '{projectID}' in database"); // Fatal error
                            }

                            CookieActions.MakeProjectInfoCookie(userProject); // Copy from database row to cookie

                            Response.Redirect(PortalConstants.URLProjectDashboard + "?" + PortalConstants.QSUserID + "=" + userProject.UserID + "&" +
                                                    PortalConstants.QSProjectID + "=" + userProject.ProjectID.ToString());
                        }
                    }
                    break;
                }
                default:
                {
                    LogError.LogQueryStringError("SelectProject", string.Format(
                        "Incorrect Query String 'Command' value '{0}'", litSavedCommand.Text)); // Log fatal error
                    break;
                }
            }
        }

        protected void chkInactive_CheckedChanged(object sender, EventArgs e)
        {
            LoadProjectView();                                              // Reload the list based on the new un/checked value
        }

        // Fetch all the Projects and load them into a GridView. 
        //
        // The saved literal "litSavedProjectSelector" determines whether we show projects for the current User or system-wide.
        // These are pretty different cases.
        //  Current User - we query the UserProject table for Projects that the User is associated with
        //  System User - we query the Project table for all Projects

        void LoadProjectView()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                if (litSavedProjectSelector.Text == PortalConstants.CUserProjectAll) // If == we're working an "All" search
                {

                    // Build a predicate that accounts for the Franchise, the Inactive check box and the Search text box.

                    var pred = PredicateBuilder.True<Project>();            // Initialize predicate to select from Project table

                    string fran = SupportingActions.GetFranchiseKey();      // Fetch current franchise key
                    pred = pred.And(p => p.FranchiseKey == fran);           // Show only Projects for this franchise

                    if (!chkInactive.Checked)                               // If false, we do not want Inactive Projects
                        pred = pred.And(p => !p.Inactive);                  // Only active Projects

                    string search = txtProject.Text;                        // Fetch the string that the user typed in, if any
                    if (search != "")                                       // If != the search string is not blank, use a Contains clause
                        pred = pred.And(p => p.Name.Contains(search));      // Only Projects whose name match our search criteria

                    List<Project> projs = context.Projects.AsExpandable().Where(pred).OrderBy(p => p.Name).ToList(); // Query, sort and make list

                    // Go through the list of all projects and determine how many pending requests the current user has.
                    // This depends on the role of the user on each project, so we have some work to do to find that number.
                    // Include that number in the grid.

                    UserRole role = EnumActions.ConvertTextToUserRole(litSavedRole.Text); // Get UserRole into numeric form
                    List<SelectProjectAllViewRow> rows = new List<SelectProjectAllViewRow>(); // Create an empty list for the GridView control
                    foreach (var p in projs)                                // Process the list of projects row-by-row
                    {
                        SelectProjectAllViewRow row = new SelectProjectAllViewRow();    // Instantiate empty row all ready to fill
                        row.ProjectID = p.ProjectID.ToString();             // Fill the part of the row that's always there
                        row.Code = p.Code;
                        row.Name = p.Name;
                        row.Description = p.Description;
                        row.Inactive = p.Inactive.ToString();
                        row.Count = 0;                                      // Initialize count of requests needing attention

                        if (role == UserRole.InternalCoordinator)                   // If == user is a Coordinator; let's go
                        {

                            // Look in each type of Request for something in a State that needs our attention

                            var query = (from app in context.Apps
                                        where app.ProjectID == p.ProjectID
                                            && !app.Archived
                                            && ((app.CurrentState == AppState.UnsubmittedByInternalCoordinator)
                                            || (app.CurrentState == AppState.AwaitingInternalCoordinator))
                                        select app).Count();
                            row.Count = row.Count + query;                  // Accumulate number of interesting requests

                            query = (from dep in context.Deps
                                        where dep.ProjectID == p.ProjectID
                                            && !dep.Archived
                                            && ((dep.CurrentState == DepState.UnsubmittedByInternalCoordinator)
                                            || (dep.CurrentState == DepState.Returned))
                                        select dep).Count();
                            row.Count = row.Count + query;                  // Accumulate number of interesting requests

                            query = (from exp in context.Exps
                                        where exp.ProjectID == p.ProjectID
                                            && !exp.Archived
                                            && (exp.CurrentState == ExpState.UnsubmittedByInternalCoordinator)
                                        select exp).Count();
                            row.Count = row.Count + query;                  // Accumulate number of interesting request
                        }

                        rows.Add(row);                                      // Add the filled-in row to the list of rows
                    }

                    AllProjectView.DataSource = rows;                       // Give it to the GridView control
                    AllProjectView.DataBind();                              // And display it

                    // As a flourish, if the "Include Inactive" checkbox is not checked, do not display the Inactive column

                    AllProjectView.Columns[SelectProjectAllViewRow.InactiveColumn].Visible = chkInactive.Checked; // If checked, column is visible

                    NavigationActions.EnableGridViewNavButtons(AllProjectView); // Enable appropriate nav buttons based on page count
                }
                else                                                        // We're working a "User" search
                {
                    var pred = PredicateBuilder.True<UserProject>();        // Initialize predicate to select from UserProject table
                    pred = pred.And(up => up.UserID == litSavedUserID.Text); // Limit search to Projects associated with this User
                    string search = txtProject.Text;                        // Fetch the string that the user typed in, if any
                    if (search != "")                                       // If != the search string is not blank, use a Contains clause
                        pred = pred.And(up => up.Project.Name.Contains(search)); // Only Projects whose name match our search criteria

                    List<UserProject> projs = context.UserProjects.AsExpandable().Where(pred).OrderBy(up => up.Project.Name).ToList(); // Query, sort and make list

                    // Go through the list of all projects and determine how many pending requests the current user has.
                    // This depends on the role of the user on each project, so we have some work to do to find that number.
                    // Include that number in the grid.

                    UserRole role = EnumActions.ConvertTextToUserRole(litSavedRole.Text); // Get UserRole into numeric form

                    List<SelectProjectUserViewRow> rows = new List<SelectProjectUserViewRow>(); // Create an empty list for the GridView control
                    foreach (var p in projs)                                // Process the list of projects row-by-row
                    {
                        SelectProjectUserViewRow row = new SelectProjectUserViewRow(); // Instantiate empty row all ready to fill
                        row.ProjectID = p.ProjectID.ToString();             // Fill the part of the row that's always there
                        row.Name = p.Project.Name;
                        row.Description = p.Project.Description;
                        row.ProjectRole = EnumActions.GetEnumDescription(p.ProjectRole);
                        row.Count = 0;                                      // Initialize count of requests needing attention

                        if (role == UserRole.Project)                               // If == user is on a Project
                        {

                            // Look in each type of Request for something in a State that needs attention of a Project Director or Project Staff
                            // We don't know which because the User may have a different role on each project. So cast a broad net at this stage.

                            var query = (from app in context.Apps
                                         where app.ProjectID == p.ProjectID
                                             && !app.Archived
                                             && ((app.CurrentState == AppState.AwaitingProjectDirector)
                                             || (app.CurrentState == AppState.Returned)
                                             || (app.CurrentState == AppState.UnsubmittedByProjectDirector)
                                             || (app.CurrentState == AppState.UnsubmittedByProjectStaff))
                                         select app).Count();
                            row.Count = row.Count + query;                  // Accumulate number of interesting request

                            query = (from dep in context.Deps
                                     where dep.ProjectID == p.ProjectID
                                         && !dep.Archived
                                         && ((dep.CurrentState == DepState.AwaitingProjectDirector)
                                         )
                                     select dep).Count();
                            row.Count = row.Count + query;                  // Accumulate number of interesting request

                            query = (from exp in context.Exps
                                     where exp.ProjectID == p.ProjectID
                                         && !exp.Archived
                                         && ((exp.CurrentState == ExpState.AwaitingProjectDirector)
                                         || (exp.CurrentState == ExpState.Returned)
                                         || (exp.CurrentState == ExpState.UnsubmittedByProjectDirector)
                                         || (exp.CurrentState == ExpState.UnsubmittedByProjectStaff))
                                     select exp).Count();
                            row.Count = row.Count + query;                  // Accumulate number of interesting request

                            rows.Add(row);                                      // Add the filled-in row to the list of rows
                        }
                    }

                    UserProjectView.DataSource = rows;                      // Give it to the GridView cnorol
                    UserProjectView.DataBind();                             // And display it

                    NavigationActions.EnableGridViewNavButtons(UserProjectView); // Enable appropriate nav buttons based on page count

                }
            }
        }
    }
}
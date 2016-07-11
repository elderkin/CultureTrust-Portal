using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Portal11.Admin
{
    public partial class EditProject : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Create a new Project, Edit an existing Project. Communication from Project menu is through Query Strings:
            //      Command - "New" or "Edit" (Required)
            //      ProjectID - the database ID of the Project to be edited (Required if Command is "Edit")
            //      ProjectName - the name of the Project to be edited (Required if Command is "Edit")

            if (!Page.IsPostBack)
            {
                int projectID = Convert.ToInt32(Request.QueryString[PortalConstants.QSProjectID]); // Fetch the ProjectID, if present
                string projectName = Request.QueryString[PortalConstants.QSProjectName]; // Fetch the Project Name, if present
                string cmd = Request.QueryString[PortalConstants.QSCommand];    // Fetch the command

                // Stash these parameters into invisible literals on the current page.

                litSavedProjectID.Text = projectID.ToString();
                litSavedCommand.Text = cmd;

                // Dispatch by Command

                switch (cmd)
                {
                    case PortalConstants.QSCommandNew:                  // Process a "New" command. Create new, empty Project for editing
                        {
                            // We don't have to "fill" any of the controls on the page, just use them in their initial state.
                            this.Title = "New Project";                 // Show that we are creating a new project
                            litSuccessMessage.Text = "New Project is ready to edit";
//                            pnlRestrictedGrants.Visible = false;        // No Restricted Grants to show yet
                            FillProjectDirectorDDL();                   // Populate the drop down list
                            break;
                        }
                    case PortalConstants.QSCommandEdit:                 // Process an "Edit" command. Read existing request, save it in same row
                        {
                            if (projectID == 0 || projectName == "")                         // If == Query String was absent. That's an error
                                LogError.LogQueryStringError("EditProject", string.Format(
                                    "Invalid Query String 'ProjectID' value {0}", projectID.ToString())); // Fatal error

                            // Fetch the Project's row from the database. Fill in the panels using data from the existing request. Lotta work!

                            litSuccessMessage.Text = "Project '" + projectName + "' is ready to edit";
                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                Project toEdit = context.Projects.Find(projectID); // Fetch Project row by its key
                                if (toEdit == null)
                                {
                                    LogError.LogQueryStringError("EditProject", string.Format(
                                        "Invalid Query String 'ProjectID' value {0}", projectID.ToString())); // Fatal error
                                }
                                LoadPanels(toEdit);                       // Fill in the visible panels from the request
                            }
                            break;
                        }
                    default:
                        {
                            LogError.LogQueryStringError("EditProject", string.Format(
                                "Invalid Query String 'Command' value '{0}'", cmd)); // Log fatal error
                            return;
                        }
                }
            }
        }

        // Cancel button has been clicked. Just return to the main Admin page.

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLAdminMain);                // Back to the barn. Nothing to save.
        }

        // Save button clicked. "Save" means that we unload all the controls for the Project into a database row. 
        // If the Project is new, we just add a new row. If the Project already exists, we update it and add a history record to show the edit.
        // We can tell a Project is new if the Query String doesn't contain a mention of a ProjectID AND the literal litSavedProjectID is empty.
        // On the first such Save, we stash the ProjectID of the new Project into this literal so that a second Save during the same session will find
        // it and update it, rather than creating a second row.

        protected void Save_Click(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    string fran = SupportingActions.GetFranchiseKey();          // Fetch franchise code
                    int projectID = 0;                                          // Assume no saved ID is available
                    if (litSavedProjectID.Text != "") projectID = Convert.ToInt32(litSavedProjectID.Text); // If != saved ID is available, use it  
                    if (projectID == 0)                                         // If == no doubt about it, Save makes a new row
                    {

                        // Make sure the Name is unique in this Franchise

                        int matchingNames = context.Projects.Where(p => (p.FranchiseKey == fran) && (p.Name == txtName.Text)).Count();
                        // Look for existing row match
                        if (matchingNames != 0)                                 // If != a Project with this Name value already exists in database
                        {
                            Response.Redirect(PortalConstants.URLAdminMain + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSDanger + "&"
                                                + PortalConstants.QSStatus + "=That Project Name already exists. You can edit it.", false); // Back to the barn
                            return;
                        }
                        Project toSave = new Project();                             // Get a place to hold everything
                        toSave.CreatedTime = System.DateTime.Now;                   // Stamp time when Request was first created as "now"
                        UnloadPanels(toSave);                                       // Move from Panels to record
                        toSave.FranchiseKey = fran;                                 // Insert the current franchise key

                        context.Projects.Add(toSave);                               // Save new Project row
                        context.SaveChanges();                                      // Commit the Add
                        projectID = toSave.ProjectID;                               // Pocket the newly assigned Project ID
                        ProjectClassActions.AddMasterProjectClasses(projectID);     // Prime the pump with the standard Project Classes
                    }
                    else
                    {
                        Project toUpdate = context.Projects.Find(projectID);        // Fetch the Project that we want to update
                        UnloadPanels(toUpdate);                                     // Move from Panels to record, modifying it in the process
                        context.SaveChanges();                                      // Commit the Add or Modify
                    }
                    UnloadProjectDirectorDLL(projectID);                            // Assign the Project Director
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditProject", "Error writing Project row"); // Fatal error
                }
            }
            
            NavigationActions.NextPage("");                                         // Now go back to Dashboard (or wherever)
        }

        // Move values from the Project record into the Page.

        void LoadPanels(Project record)
        {
            txtName.Text = record.Name;
            chkInact.Checked = record.Inactive;
            txtDescription.Text = record.Description;
            txtBalanceDate.Text = record.BalanceDate.Date.ToShortDateString();
            txtCurrentFunds.Text = record.CurrentFunds.ToString("C");
            //decimal total = StateActions.LoadGrantsList(record.ProjectID, lstRestrictedGrants); // Fill read-only list box of current grants
            //txtTotalGrants.Text = total.ToString("C");                      // Also display total of all grants
            // Project Director
            FillProjectDirectorDDL();
        }

        // Move the "as edited" values from the Page into a Project record.

        void UnloadPanels(Project record)
        {
            record.Name = txtName.Text;
            record.Inactive = chkInact.Checked;
            record.Description = txtDescription.Text;
            record.BalanceDate = System.DateTime.Now;                   // Assume balances were updated. Set update date as now.
            // Current Funds
            decimal bucks;
            bool convt = decimal.TryParse(txtCurrentFunds.Text, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out bucks);
                                                                        // Use a localization-savvy method to convert string to decimal
            if (convt) record.CurrentFunds = bucks;                     // If true conversion was successful, stash value
            // Restricted Grants

        }

        // Fill the Project Director drop down list from the database

        void FillProjectDirectorDDL()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                DataTable users = new DataTable();
                users.Columns.Add("UserID");
                users.Columns.Add("Name");

                // Pull all of the Users into the dropdown list

                var query = from user in context.Users
                            where !user.Inactive
                            orderby user.FullName
                            select new { user.Id, user.FullName };      // Find all Users that are active

                foreach (var row in query)
                {
                    DataRow dr = users.NewRow();                        // Build a row from the next query output
                    dr["UserID"] = row.Id;
                    dr["Name"] = row.FullName;
                    users.Rows.Add(dr);                                 // Add the new row to the data table
                }
                ddlProjectDirector.DataSource = users;                  // Pass the full table of users to drop down list
                ddlProjectDirector.DataTextField = "Name";              // Display this column in the list
                ddlProjectDirector.DataValueField = "UserID";           // Return this value for selected row
                ddlProjectDirector.DataBind();                          // And get it in gear

                // Since there certainly are users in the database - we're logged in, after all - this process should yield some users

                if (ddlProjectDirector.Items.Count == 0)                // If == the ddl is empty. This is a user error
                {
                    ddlProjectDirector.Items.Insert(0, new ListItem("-- Error: No values are available --", String.Empty));
                    // Place an error message item at top of list to force selection
                    ddlProjectDirector.SelectedIndex = 0;
                    return;
                }
   
                // If it's a new Project, for a new record, then insert a 0th row that forces user to make a selection.
                // If it's an existing Project, for an existing record, see if a Project Director has been selected.

                int projID = Convert.ToInt32(litSavedProjectID.Text);   // Fetch the Project ID, if any
                if (projID != 0)                                        // If != there is a current project
                {
                    var query2 = from up in context.UserProjects
                        where (up.ProjectID == projID) && (up.ProjectRole == ProjectRole.ProjectDirector)
                        select new { up.UserID };                       // Find the User who is PD on this Project
                    var row = query2.FirstOrDefault();                  // Pull the row of that user
                    if (row != null)                                    // If != the Project has a Project Director and we found it
                    {
                        try                                             // It's possible that our Project's selection is no longer in user list
                        {
                            ddlProjectDirector.ClearSelection();        // Get rid of any existing selection
                            ddlProjectDirector.Items.FindByValue(row.UserID).Selected = true; // Mark the selection
                            return;
                        }
                        catch (NullReferenceException) { }              // No harm, just don't select an item in the ddl
                    }
                }

                // We fell through to this point so the Project has no Project Director. Show an empty marker at the top of the drop down list

                ddlProjectDirector.Items.Insert(0, new ListItem("-- No Project Director selected --", String.Empty)); 
                // Place an empty item at top of list to force selection
                ddlProjectDirector.SelectedIndex = 0;                   // Select that row
                return;
            }
        }

        // Take the selection from the Project Director drop down list, if any. Represent it in a UserProject row.
        // If somebody wants to get fancier than this, e.g., make somebody Project Staff, they can do it via AssignUserToProject.

        void UnloadProjectDirectorDLL (int projID)
        {
            string selectedID = ddlProjectDirector.SelectedValue;       // See what was selected from the drop down list
            if (selectedID != "")                                       // If != something was selected
            {
                StateActions.ChangeProjectRole(projID, selectedID, ProjectRole.ProjectDirector); // Do the heavy lifting
            }
        }
    }
}
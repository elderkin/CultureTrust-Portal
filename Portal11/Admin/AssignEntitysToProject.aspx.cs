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
    public partial class AssignEntityToProject : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            // This page assigns Entitys to a Project in roles of Entity and Vendor. The ProjectEntity table intermediates
            // these relationships.
            // Query Strings:
            //    QSEntityRole = role that the caller is interested in selecting (optional, can be comma-delimited list)
            //    QSProjectID = ID of current project
            //    QSReturn - where to go when we finish

            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                QSValue projectID = QueryStringActions.GetProjectID();      // Fetch Project ID from Query String or ProjectInfoCookie
                litSavedProjectID.Text = projectID.String;                  // Save it in an easier spot to find
                litSavedReturn.Text = QueryStringActions.GetReturn();       // Fetch page our caller wants us to return to

                // If EntityRole is specified and value is ExpenseVendor, flip the radio button from DepositEntity to ExpenseVendor

                string entityRoles = Request.QueryString[PortalConstants.QSEntityRole]; // Fetch Query String, if present
                if (!string.IsNullOrEmpty(entityRoles))                      // If false there's something there for us to process
                {

                    // Process a comma-delimited list of EntityRole values. For each value, leave the corresponding button enabled. Disable others.
                    // As we start, all the radio buttons are enabled and the first button is selected.

                    bool selectionComplete = false;                         // Flag to show that we've selected an item
                    foreach (ListItem item in rdoEntityRole.Items)          // Fetch the radio button items one at a time
                    {
                        if (entityRoles.IndexOf(item.Value) != -1)          // If != the radio button value (EntityRole) was found in our list
                            if (!selectionComplete)                         // If false we have not yet made a selection
                            {
                                item.Selected = true;                       // Select this radio button
                                selectionComplete = true;                   // Remember that we have made this selection
                            }
                            else
                            {
                                item.Selected = false;                      // One selection per list is enough
                            }
                        else                                                // This radio button is dead to us
                        {
                            item.Selected = false;                          // De-select this radio button
                            item.Enabled = false;                           // And disable this radio button
                        }
                    }
                }

                LoadgvProjectEntity(projectID.String);                      // Fill the left grid
                LoadgvAllEntity(projectID.String);                          // Fill the right grid
            }
        }

        // Buttons to search using specified Contains criteria. As you can see, the real work is elsewhere.

        protected void btnAllEntitySearch_Click(object sender, EventArgs e)
        {
            LoadgvAllEntity(litSavedProjectID.Text);                      // Fill list, reacting to user-supplied search string
        }

        protected void rdoEntityRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadgvProjectEntity(litSavedProjectID.Text);                  // Fill list, reacting to role button click
            LoadgvAllEntity(litSavedProjectID.Text);                      // Fill the right grid
        }

        protected void btnProjectEntitySearch_Click(object sender, EventArgs e)
        {
            LoadgvProjectEntity(litSavedProjectID.Text);                  // Fill list, reacting to user-supplied search string
        }

        // Deal with pagination of the Grid View controls

        protected void gvAllEntity_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                gvAllEntity.PageIndex = e.NewPageIndex;                 // Propagate the desired page index
                LoadgvAllEntity(litSavedProjectID.Text);                // Reload the grid view control
                gvAllEntity.SelectedIndex = -1;                         // No row currently selected
            }
        }

        protected void gvProjectEntity_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                gvProjectEntity.PageIndex = e.NewPageIndex;             // Propagate the desired page index
                LoadgvProjectEntity(litSavedProjectID.Text);            // Reload the grid view control
                gvProjectEntity.SelectedIndex = -1;                     // No row currently selected
            }
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void gvAllEntity_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Entity";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvAllEntity, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged
            }
        }

        protected void gvProjectEntity_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Entity";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvProjectEntity, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged
            }
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void gvAllEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAdd.Enabled = true;                                          // We can move from All Entitys to Project Entitys
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void gvProjectEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemove.Enabled = true;                                      // We can move from Project Entitys to All Entitys
        }

        // Add button clicked. Fetch the selected row from the Entity table and add its Entity ID to the Project Entity table. Use the Entity Role
        // selected in the radio buttons

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Label label = (Label)gvAllEntity.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains EntityID
            if (label.Text == "")                                           // If == there is no ID value. That's an error.
                LogError.LogQueryStringError("AssignEntity", "EntityID not found in selected GridView row"); // Log fatal error

            EntityRole selectedRole = EnumActions.ConvertTextToEntityRole(rdoEntityRole.SelectedValue); // Fetch selected button, convert to enum

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    ProjectEntity pe = new ProjectEntity()
                    {                                                       // Get an empty record and initialize it
                        StartDate = System.DateTime.Now,                    // Remember when the Entity joined the project
                        ProjectID = Convert.ToInt32(litSavedProjectID.Text), // Stash the ID of this Project
                        EntityID = Convert.ToInt32(label.Text),             // The text of this control contains Entity ID
                        EntityRole = selectedRole                           // The role from the radio buttons
                    };
                    context.ProjectEntitys.Add(pe);                         // Add this new row
                    context.SaveChanges();
                    LoadgvAllEntity(litSavedProjectID.Text);              // Refresh the lists
                    LoadgvProjectEntity(litSavedProjectID.Text);

                    gvAllEntity.SelectedIndex = -1;                       // Unselect all rows
                    btnAdd.Enabled = false; btnRemove.Enabled = false;      // Nothing selected anymore so buttons are turned off
                    litSuccessMessage.Text = "Successfully added '" + EnumActions.GetEnumDescription(selectedRole) + "' to Project";
                    litDangerMessage.Text = null;                           // Report success to user
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AssignEntitys", "Error writing ProjectEntity row"); // Fatal error
                }
            }
        }

        // Remove button clicked. Delete the row from the Project Entity table. Note that the GridView contains the ProjectEntityID -
        // the primary key of the row.

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            Label label = (Label)gvProjectEntity.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains ProjectEntityID
            if (label.Text == "")                                           // If == there is no ID value. That's an error.
                LogError.LogQueryStringError("AssignEntity", "ProjectEntityID not found in selected GridView row"); // Log fatal error

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    ProjectEntity pe = new ProjectEntity()
                    {
                        ProjectEntityID = Convert.ToInt32(label.Text)
                    };                                                      // Create shell of record
                    context.ProjectEntitys.Attach(pe);                      // Don't query it, but just connect to the relevant row
                    context.ProjectEntitys.Remove(pe);                      // Delete that row
                    context.SaveChanges();
                    LoadgvAllEntity(litSavedProjectID.Text);              // Refresh the lists
                    LoadgvProjectEntity(litSavedProjectID.Text);

                    gvProjectEntity.SelectedIndex = -1;                   // Unselect all rows
                    btnAdd.Enabled = false; btnRemove.Enabled = false;      // Nothing selected anymore so buttons are turned off
                    litSuccessMessage.Text = "Successfully removed Entity from Project"; litDangerMessage.Text = null; // Report success to user
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AssignEntitys", "Error deleting ProjectEntity row"); // Fatal error
                }
            }
        }

        // Remove All button clicked. Just blast everything (of this role) for this project out of the ProjectEntity database.

        protected void btnRemoveAll_Click(object sender, EventArgs e)
        {
            EntityRole selectedRole = EnumActions.ConvertTextToEntityRole(rdoEntityRole.SelectedValue); // Fetch selected button, convert to enum
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    string projectIDText = litSavedProjectID.Text;          // Fetch the project ID
                    int projectID = Convert.ToInt32(projectIDText);         // Also produce the integer version
                    context.ProjectEntitys.RemoveRange(context.ProjectEntitys.Where(p => // Delete all the ProjectEntitys
                        (p.ProjectID == projectID) &&                       // for this project
                        (p.EntityRole == selectedRole)));                   // for the selected role

                    context.SaveChanges();
                    LoadgvAllEntity(projectIDText);                       // Refresh the lists
                    LoadgvProjectEntity(projectIDText);

                    gvAllEntity.SelectedIndex = -1; gvProjectEntity.SelectedIndex = -1; // Unselect all rows
                    btnAdd.Enabled = false; btnRemove.Enabled = false;    // Nothing selected anymore so buttons are turned off
                    litSuccessMessage.Text = "Successfuly removed all Entitys from Project"; // Report success to user
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AssignEntitys", "Error deleting ProjectEntity rows"); // Fatal error
                }
            }
        }

        // New button clicked. Invoke the New Entity page to - maybe - create a new entity. Then return to this page.
        // That requires propagating this page's context: ProjectID and ProjectName, Return, EntityRole.

        protected void btnNew_Click(object sender, EventArgs e)
        {
            string running = PortalConstants.URLEditEntity + "?" + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text // Go to EditEntity page
                                                 + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew // With "New" command
                                                 + "&" + PortalConstants.QSReturn + "=" + PortalConstants.URLAssignEntitysToProject; // Return to this page

            if (!string.IsNullOrEmpty(litSavedReturn.Text))                 // If false parameter present, propagate it
                running += "&" + PortalConstants.QSReturn2 + "=" + litSavedReturn.Text; // Propagate caller's return

            string ent = Request.QueryString[PortalConstants.QSEntityRole]; // Fetch EntityRole parameter, if present
            if (!string.IsNullOrEmpty(ent))                                 // If false parameter present, propagate it
                running += "&" + PortalConstants.QSEntityRole + "=" + ent;  // Propagate caller's parameter

            Response.Redirect(running);                                     // Return to the "caller" with QS parameters
        }

        // Done button clicked. Make sure there is at least one Entity assigned to the Project. Then return to the Project Dashboard.

        protected void btnDone_Click(object sender, EventArgs e)
        {
            if (gvProjectEntity.Rows.Count == 0)                    // If == no Entitys are assigned to this Project
            {
                litDangerMessage.Text = "The Project must have at least one Entity assigned to it in this role."; // Display the error
                return;
            }
            Response.Redirect(litSavedReturn.Text);                 // Back to caller-specified page
        }

        // Fetch all the Entitys on this project and load them into a GridView

        void LoadgvProjectEntity(string projectIDText)
        {
            string selectedValue = rdoEntityRole.SelectedValue;         // Pull value of selected radio button
            if (selectedValue != "")                                    // If != a radio button is selected
            {
                EntityRole selectedRole = EnumActions.ConvertTextToEntityRole(selectedValue); // Fetch selected button, convert to enum

                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    int projectID = Convert.ToInt32(projectIDText);         // Convert string version to integer

                    // Build a predicate that accounts for the Search text box.

                    var pred = PredicateBuilder.True<ProjectEntity>();      // Initialize predicate to select from Entity table
                    pred = pred.And(p => p.ProjectID == projectID);         // Only Entitys on this project
                    pred = pred.And(p => p.EntityRole == selectedRole);     // Only in the selected role
                    string search = txtProjectEntity.Text;                  // Fetch the string that the user typed in, if any
                    if (search != "")                                       // If != the search string is not blank, use a Contains clause
                        pred = pred.And(p => p.Entity.Name.Contains(search)); // Only Entitys whose name match our search criteria

                    gvProjectEntity.DataSource = context.ProjectEntitys.AsExpandable().Where(pred) // Query using the Where predicate we just constructed
                                                        .OrderBy(p => p.Entity.Name) // Sort the results by Name
                                                        .ToList();          // Deliver the list to the GridView control
                    gvProjectEntity.DataBind();                             // And display it

                    NavigationActions.EnableGridViewNavButtons(gvProjectEntity); // Enable appropriate nav buttons based on page count

                    //btnRemoveAll.Enabled = true;                            // Assume that there are rows here which could be removed
                    //if (pps.Count() == 0)                                   // If == there are no rows displayed here
                    //    btnRemoveAll.Enabled = false;                       // So the "Remove All" button is not useful
                }
            }
        }

        // Fetch all the Entitys and load them into a GridView. In the process, filter out all of the Entitys that are already
        // associated with this project in the currently selected category.

        void LoadgvAllEntity(string projectIDText)
        {
            UserRole userRole = EnumActions.ConvertTextToUserRole(QueryStringActions.GetUserRole()); // Fetch current user's role from User Cookie
            string selectedValue = rdoEntityRole.SelectedValue;         // Pull value of selected radio button
            EntityRole selectedRole = EntityRole.DepositEntity;         // An arbitrary default falue. There should always be a button chosen

            if (!string.IsNullOrEmpty(selectedValue))                    // If false a radio button is selected
                selectedRole = EnumActions.ConvertTextToEntityRole(selectedValue); // Fetch selected button, convert to enum

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                string search = txtAllEntity.Text;                      // Fetch the string that the user typed in, if any
                gvAllEntity.DataSource = null;                          // Assume we have nothing to display
                if (!((userRole == UserRole.Project) & (string.IsNullOrEmpty(search))))  // If true user is a Project user and search string is blank
                {

                    // Find all the Project Entitys in the selected role. We don't care about the search string at this point.

                    int projectID = Convert.ToInt32(projectIDText);     // Convert string version to integer
                    var queryPE = from pe in context.ProjectEntitys
                                  where (pe.ProjectID == projectID) && (pe.EntityRole == selectedRole) // On project, in role
                                  select pe.Entity;

                    // Build a predicate that accounts for the Inactive check box and the Search text box.

                    var pred = PredicateBuilder.True<Entity>();         // Initialize predicate to select from Entity table
                    pred = pred.And(p => !p.Inactive);                  // Only active Entitys
                    if (!string.IsNullOrEmpty(search))                  // If false the search string is not blank, use a Contains clause
                        pred = pred.And(p => p.Name.Contains(search));  // Only Entitys whose name match our search criteria
                    string franchiseKey = SupportingActions.GetFranchiseKey(); // Fetch the current key
                    pred = pred.And(p => p.FranchiseKey == franchiseKey); // Only for this Franchise

                    List<Entity> ps = context.Entitys.AsExpandable().Where(pred).Except(queryPE).OrderBy(p => p.Name).ToList(); // Query, exclude,sort and make list

                    gvAllEntity.DataSource = context.Entitys.AsExpandable().Where(pred) // Query using the predicate we just created
                                                    .Except(queryPE)    // Exclude Entities already in selected role for project
                                                    .OrderBy(p => p.Name) // Sort the Entities by name
                                                    .ToList();          // Deliver the list to the GridView control
                }
                gvAllEntity.DataBind();                                 // And display it

                NavigationActions.EnableGridViewNavButtons(gvAllEntity); // Enable appropriate nav buttons based on page count
            }
        }
    }
}
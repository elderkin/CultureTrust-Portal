using LinqKit;
using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Admin
{
    public partial class AssignPersonsToProject : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            // This page assigns Persons to a Project in roles of Contractor, Employee, Holder and Recipient. The ProjectPerson table intermediates
            // these relationships.
            // Query Strings:
            //    QSPersonRole - role that the caller is interested in selecting (optional, can be comma-delimited list)
            //    QSProjectID = ID of current project
            //    QSReturn - where to go when we finish

            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                QSValue projectID = QueryStringActions.GetProjectID();      // Fetch Project ID from Query String or ProjectInfoCookie
                litSavedProjectID.Text = projectID.String;                  // Save it in an easier spot to find
                litSavedReturn.Text = QueryStringActions.GetReturn();       // Fetch page our caller wants us to return to

                // If PersonRole is specified, flip the radio button to match

                string personRoles = Request.QueryString[PortalConstants.QSPersonRole]; // Fetch Query String, if present. If absent, allow all roles
                if (!string.IsNullOrEmpty(personRoles))                      // If false there's something there for us to process

                    // Process a comma-delimited list of PersonRole values. For each value, leave the corresponding button enabled. Disable others.
                    // As we start, all the radio buttons are enabled and the first button - Contractor - is selected.

                {
                    bool selectionComplete = false;                         // Flag to show that we've selected an item
                    foreach (ListItem item in rdoPersonRole.Items)          // Fetch the radio button items one at a time
                    {
                        if (personRoles.IndexOf(item.Value) != -1)          // If != the radio button value (PersonRole) was found in our list
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

                LoadgvProjectPerson(projectID.String);                      // Fill the left grid
                LoadgvAllPerson(projectID.String);                          // Fill the right grid
            }
        }

        // Buttons to search using specified Contains criteria. As you can see, the real work is elsewhere.

        protected void btnAllPersonSearch_Click(object sender, EventArgs e)
        {
            LoadgvAllPerson(litSavedProjectID.Text);                      // Fill list, reacting to user-supplied search string
        }

        protected void rdoPersonRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadgvProjectPerson(litSavedProjectID.Text);                  // Fill list, reacting to role button click
            LoadgvAllPerson(litSavedProjectID.Text);                      // Fill the right grid
        }

        protected void btnProjectPersonSearch_Click(object sender, EventArgs e)
        {
            LoadgvProjectPerson(litSavedProjectID.Text);                  // Fill list, reacting to user-supplied search string
        }

        // Deal with pagination of the Grid View controls

        protected void gvAllPerson_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                gvAllPerson.PageIndex = e.NewPageIndex;                 // Propagate the desired page index
                LoadgvAllPerson(litSavedProjectID.Text);                // Reload the grid view control
                gvAllPerson.SelectedIndex = -1;                         // No row currently selected
            }
        }

        protected void gvProjectPerson_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                gvProjectPerson.PageIndex = e.NewPageIndex;             // Propagate the desired page index
                LoadgvProjectPerson(litSavedProjectID.Text);            // Reload the grid view control
                gvProjectPerson.SelectedIndex = -1;                     // No row currently selected
            }
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void gvAllPerson_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Person";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvAllPerson, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged
            }
        }

        protected void gvProjectPerson_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Person";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvProjectPerson, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged
            }
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void gvAllPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAdd.Enabled = true;                                          // We can move from All Persons to Project Persons
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void gvProjectPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemove.Enabled = true;                                      // We can move from Project Persons to All Persons
        }

        // Add button clicked. Fetch the selected row from the Person table and add its Person ID to the Project Person table. Use the Person Role
        // selected in the radio buttons

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Label label = (Label)gvAllPerson.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains PersonID
            if (label.Text == "")                                           // If == there is no ID value. That's an error.
                LogError.LogQueryStringError("AssignPerson", "PersonID not found in selected GridView row"); // Log fatal error

            PersonRole selectedRole = EnumActions.ConvertTextToPersonRole(rdoPersonRole.SelectedValue); // Fetch selected button, convert to enum

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    ProjectPerson pe = new ProjectPerson()
                    {                                                       // Get an empty record and initialize it
                        StartDate = System.DateTime.Now,                    // Remember when the Person joined the project
                        ProjectID = Convert.ToInt32(litSavedProjectID.Text), // Stash the ID of this Project
                        PersonID = Convert.ToInt32(label.Text),             // The text of this control contains Person ID
                        PersonRole = selectedRole                           // The role from the radio buttons
                    };
                    context.ProjectPersons.Add(pe);                         // Add this new row
                    context.SaveChanges();
                    LoadgvAllPerson(litSavedProjectID.Text);              // Refresh the lists
                    LoadgvProjectPerson(litSavedProjectID.Text);

                    gvAllPerson.SelectedIndex = -1;                       // Unselect all rows
                    btnAdd.Enabled = false; btnRemove.Enabled = false;    // Nothing selected anymore so buttons are turned off
                    litSuccessMessage.Text = "Successfully added '" + EnumActions.GetEnumDescription(selectedRole) + "' to Project";
                    litDangerMessage.Text = null;                           // Report success to user
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AssignPersons", "Error writing ProjectPerson row"); // Fatal error
                }
            }
        }

        // Remove button clicked. Delete the row from the Project Person table. Note that the GridView contains the ProjectPersonID -
        // the primary key of the row.

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            Label label = (Label)gvProjectPerson.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains ProjectPersonID
            if (label.Text == "")                                           // If == there is no ID value. That's an error.
                LogError.LogQueryStringError("AssignPerson", "ProjectPersonID not found in selected GridView row"); // Log fatal error

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    ProjectPerson pe = new ProjectPerson()
                    {
                        ProjectPersonID = Convert.ToInt32(label.Text)
                    };                                                      // Create shell of record
                    context.ProjectPersons.Attach(pe);                      // Don't query it, but just connect to the relevant row
                    context.ProjectPersons.Remove(pe);                      // Delete that row
                    context.SaveChanges();
                    LoadgvAllPerson(litSavedProjectID.Text);              // Refresh the lists
                    LoadgvProjectPerson(litSavedProjectID.Text);

                    gvProjectPerson.SelectedIndex = -1;                   // Unselect all rows
                    btnAdd.Enabled = false; btnRemove.Enabled = false;    // Nothing selected anymore so buttons are turned off
                    litSuccessMessage.Text = "Successfully removed Person from Project"; litDangerMessage.Text = null; // Report success to user
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AssignPersons", "Error deleting ProjectPerson row"); // Fatal error
                }
            }
        }

        // Remove All button clicked. Just blast everything of the selected Role for this project out of the ProjectPerson database.

        protected void btnRemoveAll_Click(object sender, EventArgs e)
        {
            PersonRole selectedRole = EnumActions.ConvertTextToPersonRole(rdoPersonRole.SelectedValue); // Fetch selected button, convert to enum
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    string projectIDText = litSavedProjectID.Text;          // Fetch the project ID
                    int projectID = Convert.ToInt32(projectIDText);         // Also produce the integer version
                    context.ProjectPersons.RemoveRange(context.ProjectPersons.Where(p => // Blow out all the ProjectPersons role
                                                            (p.ProjectID == projectID) && // For this project
                                                            (p.PersonRole == selectedRole))); // For the selected role

                    context.SaveChanges();
                    LoadgvAllPerson(projectIDText);                       // Refresh the lists
                    LoadgvProjectPerson(projectIDText);

                    gvAllPerson.SelectedIndex = -1; gvProjectPerson.SelectedIndex = -1; // Unselect all rows
                    btnAdd.Enabled = false; btnRemove.Enabled = false;    // Nothing selected anymore so buttons are turned off
                    litSuccessMessage.Text = "Successfuly removed all Persons from Project"; // Report success to user
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AssignPersons", "Error deleting ProjectPerson rows"); // Fatal error
                }
            }
        }

        // New button clicked. Invoke the New Entity page to - maybe - create a new entity. Then return to this page.
        // That requires propagating this page's context: ProjectID and ProjectName.

        protected void btnNew_Click(object sender, EventArgs e)
        {
            string running = PortalConstants.URLEditPerson + "?" + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text // Go to EditEntity page
                                                 + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew // With "New" command
                                                 + "&" + PortalConstants.QSReturn + "=" + PortalConstants.URLAssignPersonsToProject; // Return to this page

            if (!string.IsNullOrEmpty(litSavedReturn.Text))                 // If false parameter present, propagate it
                running += "&" + PortalConstants.QSReturn2 + "=" + litSavedReturn.Text; // Propagate caller's return

            string per = Request.QueryString[PortalConstants.QSPersonRole]; // Fetch PersonRole parameter, if present
            if (!string.IsNullOrEmpty(per))                                 // If false parameter present, propagate it
                running += "&" + PortalConstants.QSPersonRole + "=" + per;  // Propagate caller's parameter

            Response.Redirect(running);                                     // Return to the "caller" with QS parameters
        }

        // Done button clicked. Make sure there is at least one Person assigned to the Project. Then return to the Project Dashboard.

        protected void btnDone_Click(object sender, EventArgs e)
        {
            if (gvProjectPerson.Rows.Count == 0)                        // If == no Persons are assigned to this Project
            {
                litDangerMessage.Text = "The Project must have at least one Person assigned to it in this role."; // Display the error
                return;
            }
            Response.Redirect(litSavedReturn.Text);                     // Back to caller-specified page
        }

        // Fetch all the Persons on this project and load them into a GridView

        void LoadgvProjectPerson(string projectIDText)
        {
            string selectedValue = rdoPersonRole.SelectedValue;         // Pull value of selected radio button
            if (selectedValue != "")                                    // If != a radio button is selected
            {
                PersonRole selectedRole = EnumActions.ConvertTextToPersonRole(selectedValue); // Fetch selected button, convert to enum

                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    int projectID = Convert.ToInt32(projectIDText);         // Convert string version to integer

                    // Build a predicate that accounts for the Search text box.

                    var pred = PredicateBuilder.True<ProjectPerson>();      // Initialize predicate to select from Person table
                    pred = pred.And(p => p.ProjectID == projectID);         // Only Persons on this project
                    pred = pred.And(p => p.PersonRole == selectedRole);     // Only in the selected role
                    string search = txtProjectPerson.Text;                  // Fetch the string that the user typed in, if any
                    if (search != "")                                       // If != the search string is not blank, use a Contains clause
                        pred = pred.And(p => p.Person.Name.Contains(search)); // Only Persons whose name match our search criteria

                    //List<ProjectPerson> pps = context.ProjectPersons.AsExpandable().Where(pred) // Query ProjectPerson table using Where predicate just constructed
                    //                                .OrderBy(p => p.Person.Name) // Sort the result by Name
                    //                                .ToList();              // Deliver as a list ready to add to GridView

                    gvProjectPerson.Columns[PortalConstants.gvProjectPersonW9Column].Visible = (selectedRole == PersonRole.Contractor); 
                                                                            // If showing Contractors, display W-9 information
                    //gvProjectPerson.DataSource = pps;                       // Give it to the GridView cnorol
                    gvProjectPerson.DataSource = context.ProjectPersons.AsExpandable().Where(pred) // Query ProjectPerson table using Where predicate just constructed
                                                    .OrderBy(p => p.Person.Name) // Sort the result by Name
                                                    .ToList();              // Deliver result to GridView

                    gvProjectPerson.DataBind();                             // And display it


                    NavigationActions.EnableGridViewNavButtons(gvProjectPerson); // Enable appropriate nav buttons based on page count

                    //btnremoveall.enabled = true;                            // assume that there are rows here which could be removed
                    //if (pps.count() == 0)                                   // if == there are no rows displayed here
                    //    btnremoveall.enabled = false;                       // so the "remove all" button is not useful
                }
            }
        }

        // Fetch all the Persons and load them into a GridView. In the process, filter out all of the Persons that are already
        // associated with this project in the currently selected category.
        //
        // If the user is not staff, we restrict what we show them. They must supply a search string and they don't see email addresses.

        void LoadgvAllPerson(string projectIDText)
        {
            UserRole userRole = EnumActions.ConvertTextToUserRole(QueryStringActions.GetUserRole()); // Fetch current user's role from User Cookie
            string selectedValue = rdoPersonRole.SelectedValue;         // Pull value of selected radio button
            PersonRole selectedRole = PersonRole.Contractor;            // An arbitrary default falue. There should always be a button chosen

            if (!string.IsNullOrEmpty(selectedValue))                   // If false a radio button is selected
                selectedRole = EnumActions.ConvertTextToPersonRole(selectedValue); // Fetch selected button, convert to enum

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                string search = txtAllPerson.Text;                      // Fetch the string that the user typed in, if any
                List<Person> ps = null;                                 // Assume we have nothing to display
                if (!((userRole == UserRole.Project) & (string.IsNullOrEmpty(search))))  // If true user is a Project user and search string is blank
                {

                    // Find all the Project Persons in the selected role. We don't care about the search string at this point.

                    int projectID = Convert.ToInt32(projectIDText);     // Convert string version to integer
                    var queryPE = from pe in context.ProjectPersons
                                  where (pe.ProjectID == projectID) && (pe.PersonRole == selectedRole) // On project, in role
                                  select pe.Person;

                    // Build a predicate that accounts for the Inactive check box and the Search text box.

                    var pred = PredicateBuilder.True<Person>();         // Initialize predicate to select from Person table
                    pred = pred.And(p => !p.Inactive);                  // Only active Persons
                    if (!string.IsNullOrEmpty(search))                  // If false the search string is not blank, use a Contains clause
                        pred = pred.And(p => p.Name.Contains(search));  // Only Persons whose name match our search criteria
                    string franchiseKey = SupportingActions.GetFranchiseKey(); // Fetch the current key
                    pred = pred.And(p => p.FranchiseKey == franchiseKey); // Only for this Franchise

                    ps = context.Persons.AsExpandable().Where(pred)     // Query using the Where predicate we constructed
                                                        .Except(queryPE) // Exclude Persons already assigned to Project in this role
                                                        .OrderBy(p => p.Name) // Sort the result by name
                                                        .ToList();      // Deliver result as a list we can jam into the gv
                }
                if (userRole == UserRole.Project)                       // If == user is a Project user, not staff
                    gvAllPerson.Columns[PortalConstants.gvAllPersonEmailColumn].Visible = false; // Such a user cannot see email address

                gvAllPerson.Columns[PortalConstants.gvAllPersonW9Column].Visible = (selectedRole == PersonRole.Contractor);
                                                                        // If showing Contractors, display W-9 information
                gvAllPerson.DataSource = ps;                            // Give it to the GridView cnorol
                gvAllPerson.DataBind();                                 // And display it

                NavigationActions.EnableGridViewNavButtons(gvAllPerson); // Enable appropriate nav buttons based on page count
            }
        }
    }
}
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

            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                QSValue projectID = QueryStringActions.GetProjectID();      // Fetch Project ID from Query String or ProjectInfoCookie
                litSavedProjectID.Text = projectID.String;                  // Save it in an easier spot to find
                litProjectName.Text = QueryStringActions.GetProjectName();  // Display Project Name from Query String or ProjectInfoCookie
                
                LoadgvProjectPerson(projectID.String);                    // Fill the left grid
                LoadgvAllPerson(projectID.String);                        // Fill the right grid
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

        // Done button clicked. Make sure there is at least one Person assigned to the Project. Then return to the Project Dashboard.

        protected void btnDone_Click(object sender, EventArgs e)
        {
            if (gvProjectPerson.Rows.Count == 0)                      // If == no Persons are assigned to this Project
            {
                litDangerMessage.Text = "The Project must have at least one Person assigned to it in this role."; // Display the error
                return;
            }
            Response.Redirect(PortalConstants.URLAdminMain);            // Back to Main Admin page
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

                    List<ProjectPerson> pps = context.ProjectPersons.AsExpandable().Where(pred).OrderBy(p => p.Person.Name).ToList(); // Query, exclude, sort and make list

                    if (selectedRole == PersonRole.Contractor)              // If == we are being asked for Contractors
                        gvProjectPerson.Columns[PortalConstants.gvProjectPersonW9Column].Visible = true; // Display W-9 information
                    else
                        gvProjectPerson.Columns[PortalConstants.gvProjectPersonW9Column].Visible = false; // Do not display W-9 information

                    gvProjectPerson.DataSource = pps;                     // Give it to the GridView cnorol
                    gvProjectPerson.DataBind();                           // And display it


                    NavigationActions.EnableGridViewNavButtons(gvProjectPerson); // Enable appropriate nav buttons based on page count

                    btnRemoveAll.Enabled = true;                            // Assume that there are rows here which could be removed
                    if (pps.Count() == 0)                                   // If == there are no rows displayed here
                        btnRemoveAll.Enabled = false;                       // So the "Remove All" button is not useful
                }
            }
        }

        // Fetch all the Persons and load them into a GridView. In the process, filter out all of the Persons that are already
        // associated with this project in the currently selected category.

        void LoadgvAllPerson(string projectIDText)
        {
            string selectedValue = rdoPersonRole.SelectedValue;         // Pull value of selected radio button
            PersonRole selectedRole = PersonRole.Contractor;            // An arbitrary default falue. There should always be a button chosen

            if (selectedValue != "")                                    // If != a radio button is selected
                selectedRole = EnumActions.ConvertTextToPersonRole(selectedValue); // Fetch selected button, convert to enum

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // Find all the Project Persons in the selected role. We don't care about the search string at this point.

                int projectID = Convert.ToInt32(projectIDText);         // Convert string version to integer
                var queryPE = from pe in context.ProjectPersons
                              where (pe.ProjectID == projectID) && (pe.PersonRole == selectedRole) // On project, in role
                              select pe.Person;

                // Build a predicate that accounts for the Inactive check box and the Search text box.

                var pred = PredicateBuilder.True<Person>();             // Initialize predicate to select from Person table
                    pred = pred.And(p => !p.Inactive);                  // Only active Persons
                string search = txtAllPerson.Text;                      // Fetch the string that the user typed in, if any
                if (search != "")                                       // If != the search string is not blank, use a Contains clause
                    pred = pred.And(p => p.Name.Contains(search));      // Only Persons whose name match our search criteria
                string franchiseKey = SupportingActions.GetFranchiseKey(); // Fetch the current key
                pred = pred.And(p => p.FranchiseKey == franchiseKey);   // Only for this Franchise

                List<Person> ps = context.Persons.AsExpandable().Where(pred).Except(queryPE).OrderBy(p => p.Name).ToList(); // Query, exclude,sort and make list

                if (selectedRole == PersonRole.Contractor)              // If == we are being asked for Contractors
                    gvAllPerson.Columns[PortalConstants.gvAllPersonW9Column].Visible = true; // Display W-9 information
                else
                    gvAllPerson.Columns[PortalConstants.gvAllPersonW9Column].Visible = false; // Do not display W-9 information

                gvAllPerson.DataSource = ps;                          // Give it to the GridView cnorol
                gvAllPerson.DataBind();                               // And display it

                NavigationActions.EnableGridViewNavButtons(gvAllPerson); // Enable appropriate nav buttons based on page count
            }
        }
    }
}
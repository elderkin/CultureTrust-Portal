using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal11.Models;
using Portal11.Logic;
using Portal11.ErrorLog;

namespace Portal11.Rqsts
{
    public partial class AssignEmployees : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                HttpCookie projectInfoCookie = Request.Cookies[PortalConstants.CProjectInfo]; // Ask for the Project Info cookie
                if (projectInfoCookie == null)                              // If == no project
                    LogError.LogQueryStringError("AssignEmployee", "Missing ProjectInfo Cookie"); // Log fatal error

                string projectIDText = projectInfoCookie[PortalConstants.CProjectID]; // Fetch ProjectID from cookie
                if (projectIDText == "")                                    // No project. We are invoked incorrectly.
                    LogError.LogQueryStringError("AssignEmployee", "Missing ProjectInfo Cookie value 'ProjectID'"); // Log fatal error

                litSavedProjectID.Text = projectIDText;                     // Save it in an easier spot to find
                LoadProjectEmployeeView(projectIDText);                     // Fill the left grid
                LoadAllEmployeeView(projectIDText);                         // Fill the right grid
            }
        }

        // Buttons to search using specified Contains criteria. As you can see, the real work is elsewhere.

        protected void btnAllEmployeeSearch_Click(object sender, EventArgs e)
        {
            LoadAllEmployeeView(litSavedProjectID.Text);                    // Fill list, reacting to user-supplied search string
        }

        protected void btnProjectEmployeeSearch_Click(object sender, EventArgs e)
        {
            LoadProjectEmployeeView(litSavedProjectID.Text);                // Fill list, reacting to user-supplied search string
        }

        // Deal with pagination of the Grid View controls

        protected void AllEmployeeView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                AllEmployeeView.PageIndex = e.NewPageIndex;                 // Propagate the desired page index
                LoadAllEmployeeView(litSavedProjectID.Text);                // Reload the grid view control
                AllEmployeeView.SelectedIndex = -1;                         // No row currently selected
            }
        }

        protected void ProjectEmployeeView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                ProjectEmployeeView.PageIndex = e.NewPageIndex;             // Propagate the desired page index
                LoadProjectEmployeeView(litSavedProjectID.Text);            // Reload the grid view control
                ProjectEmployeeView.SelectedIndex = -1;                     // No row currently selected
            }
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void AllEmployeeView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Employee";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.AllEmployeeView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged
            }
        }

        protected void ProjectEmployeeView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Employee";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.ProjectEmployeeView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged
            }
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void AllEmployeeView_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAddx.Enabled = true;                                          // We can move from All Employees to Project Employees
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void ProjectEmployeeView_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemovex.Enabled = true;                                      // We can move from Project Employees to All Employees
        }

        // Add button clicked. Fetch the selected row from the Employee table and add its Employee ID to the Project Employee table.

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Label label = (Label)AllEmployeeView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains EmployeeID
            if (label.Text == "")                                           // If == there is no ID value. That's an error.
                LogError.LogQueryStringError("AssignEmployee", "EmployeeID not found in selected GridView row"); // Log fatal error

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    ProjectEmployee pe = new ProjectEmployee() {            // Get an empty record and initialize it
                        StartDate = System.DateTime.Now,                    // Remember when the employee joined the project
                        ProjectID = Convert.ToInt32(litSavedProjectID.Text), // Stash the ID of this Project
                        EmployeeID = Convert.ToInt32(label.Text)            // The text of this control contains Employee ID
                    };
                    context.ProjectEmployees.Add(pe);                       // Add this new row
                    context.SaveChanges();
                    LoadAllEmployeeView(litSavedProjectID.Text);            // Refresh the lists
                    LoadProjectEmployeeView(litSavedProjectID.Text);

                    AllEmployeeView.SelectedIndex = -1;                     // Unselect all rows
                    litSuccessMessage.Text = "Successfully added Employee to Project"; litDangerMessage.Text = null; // Report success to user
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AssignEmployees", "Error writing ProjectEmployee row"); // Fatal error
                }
            }
        }

        // Remove button clicked. Delete the row from the Project Employee table. Note that the GridView contains the ProjectEmployeeID -
        // the primary key of the row.

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            Label label = (Label)ProjectEmployeeView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains ProjectEmployeeID
            if (label.Text == "")                                           // If == there is no ID value. That's an error.
                LogError.LogQueryStringError("AssignEmployee", "ProjectEmployeeID not found in selected GridView row"); // Log fatal error

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    ProjectEmployee pe  = new ProjectEmployee() { 
                        ProjectEmployeeID = Convert.ToInt32(label.Text) 
                        };                                                  // Create shell of record
                    context.ProjectEmployees.Attach(pe);                    // Don't query it, but just connect to the relevant row
                    context.ProjectEmployees.Remove(pe);                    // Delete that row
                    context.SaveChanges();
                    LoadAllEmployeeView(litSavedProjectID.Text);            // Refresh the lists
                    LoadProjectEmployeeView(litSavedProjectID.Text);

                    ProjectEmployeeView.SelectedIndex = -1;                 // Unselect all rows
                    litSuccessMessage.Text = "Successfully removed Employee from Project"; litDangerMessage.Text = null; // Report success to user
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AssignEmployees", "Error deleting ProjectEmployee row"); // Fatal error
                }
            }
        }

        // Remove All button clicked. Just blast everything for this project out of the ProjectEmployee database.

        protected void btnRemoveAll_Click(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    string projectIDText = litSavedProjectID.Text;          // Fetch the project ID
                    int projectID = Convert.ToInt32(projectIDText);         // Also produce the integer version
                    context.ProjectEmployees.RemoveRange(context.ProjectEmployees.Where(p => p.ProjectID == projectID)); // Blow out all the
                                                                            // employees for the current project
                    context.SaveChanges();
                    LoadAllEmployeeView(projectIDText);                     // Refresh the lists
                    LoadProjectEmployeeView(projectIDText);
                    litSuccessMessage.Text = "Successfuly removed all Employees from Project"; // Report success to user
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AssignEmployees", "Error deleting ProjectEmployee rows"); // Fatal error
                }
            }
        }

        // Done button clicked. Make sure there is at least one Employee assigned to the Project. Then return to the Project Dashboard.

        protected void btnDone_Click(object sender, EventArgs e)
        {
            if (ProjectEmployeeView.Rows.Count == 0)                      // If == no Employees are assigned to this Project
            {
                litDangerMessage.Text = "The Project must have at least one Employee assigned to it."; // Display the error
                return;
            }
            Response.Redirect("~/Rqsts/ProjectDashboard");               // Back to Project Dashboard
        }
        
        // Fetch all the Employees on this project and load them into a GridView

        void LoadProjectEmployeeView(string projectIDText)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                int projectID = Convert.ToInt32(projectIDText);         // Convert string version to integer
                var query = from pe in context.ProjectEmployees
                            where pe.ProjectID == projectID
                            orderby pe.Employee.Name
                            select pe;
                string search = txtProjectEmployee.Text;                // Fetch the string that the user typed in, if any
                if (search != "")                                       // If != the search string is not blank. Use a Contains clause
                {
                    query = from pe in context.ProjectEmployees
                            where pe.ProjectID == projectID && pe.Employee.Name.Contains(search)
                            orderby pe.Employee.Name
                            select pe;
                }

                List<ProjectEmployee> em = query.ToList();              // Deliver a row for ProjectEmployee for the current Project
                ProjectEmployeeView.DataSource = em;                    // Give it to the GridView cnorol
                ProjectEmployeeView.DataBind();                         // And display it

                NavigationActions.EnableGridViewNavButtons(ProjectEmployeeView); // Enable appropriate nav buttons based on page count
                btnRemoveAllx.Enabled = true;                           // Assume that there are rows here which could be removed
                if (em.Count() == 0)                                    // If == there are no rows displayed here
                    btnRemoveAllx.Enabled = false;                      // So the "Remove All" button is not useful here
            }
        }

        // Fetch all the Employees and load them into a GridView. In the process, filter out all of the Employees that are already
        // associated with this project.

        void LoadAllEmployeeView(string projectIDText)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // Start by finding all the Employees that meet the search criteria

                var queryE = from e in context.Employees                 // Assume search string is blank
                            where !e.Inactive
                            orderby e.Name
                            select e;
                string searchE = txtAllEmployee.Text;                    // Fetch the string that the user typed in, if any
                if (searchE != "")                                       // If != the search string is not blank. Use a Contains clause
                {
                    queryE = from e in context.Employees
                            where !e.Inactive && e.Name.Contains(searchE)
                            orderby e.Name
                            select e;
                }

                // Now find all the Project Employees. We don't care about the search string at this point, we're excluding ALL the project employees

                int projectID = Convert.ToInt32(projectIDText);         // Convert string version to integer
                var queryPE = from pe in context.ProjectEmployees
                            where pe.ProjectID == projectID
                            orderby pe.Employee.Name
                            select pe.Employee;

                List<Employee> em = queryE.Except(queryPE).ToList();    // List of Employees except those that are returned by Project Employee query
                AllEmployeeView.DataSource = em;                        // Give it to the GridView cnorol
                AllEmployeeView.DataBind();                             // And display it

                NavigationActions.EnableGridViewNavButtons(AllEmployeeView); // Enable appropriate nav buttons based on page count
            }
        }
    }
}
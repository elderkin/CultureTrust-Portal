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
    public partial class SelectPerson : System.Web.UI.Page
    {

        // We get to this page from the Admin user's Edit Person menu item. The Query String comtains an "Edit" command.
        // We come here to choose the Person before dispatching to Edit Person itself.
        //
        // Query string inputs are:
        //  Command - "Edit"

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                string cmd = Request.QueryString[PortalConstants.QSCommand]; // Look on the query string that invoked this page. Find the command.

                if (cmd == "")                                              // No command. We are invoked incorrectly.
                    LogError.LogQueryStringError("SelectPerson", "Missing Query String 'Command'"); // Log fatal error

                litSavedCommand.Text = cmd;                                 // Remember the command that invoked this page
                LoadPersonView();                                         // Fill the grid
            }
        }

        protected void btnPersonSearch_Click(object sender, EventArgs e)
        {
            LoadPersonView();                                              // Refresh the grid using updated search criteria
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void PersonView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Person";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.PersonView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void PersonView_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.Enabled = true;                                           // With a row selected, we can act on a button click
        }

        // Deal with pagination of the Grid View controls

        protected void PersonView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                PersonView.PageIndex = e.NewPageIndex;                     // Propagate the desired page index
                LoadPersonView();                                          // Fill the grid
                PersonView.SelectedIndex = -1;                             // No row currently selected
            }
        }

        // Cancel, eh? Go to the Staff Dashboard (or wherever).

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            NavigationActions.NextPage("");
        }

        // Select button pushed to select a Person. Pass this to the EditPerson page.

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            Label label = (Label)PersonView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains RqstID
            string PersonID = label.Text;                                  // Extract the text of the control, which is RqstID
            if (PersonID == "")
                LogError.LogQueryStringError("SelectPerson", string.Format(
                    "PersonID '{0}' from selected GridView row missing", PersonID)); // Log fatal error


            Response.Redirect(PortalConstants.URLEditPerson + "?" + PortalConstants.QSPersonID + "=" + PersonID + "&" +
                                               PortalConstants.QSCommand + "=" + litSavedCommand.Text);
        }

        // Create a new Person by invoking EditPerson with a "New" command

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLEditPerson + "?" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew);
        }

        protected void chkInactive_CheckedChanged(object sender, EventArgs e)
        {
            LoadPersonView();                                              // Reload the list based on the new un/checked value
        }

        // Fetch all the Persons and load them into a GridView

        void LoadPersonView()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // Build a predicate that accounts for the Inactive check box and the Search txe box.

                var pred = PredicateBuilder.True<Person>();             // Initialize predicate to select from Person table
                if (!chkInactive.Checked)                               // If false, we do not want Inactive Persons
                    pred = pred.And(p => !p.Inactive);                  // Only active Persons
                string search = txtPerson.Text;                         // Fetch the string that the user typed in, if any
                if (search != "")                                       // If != the search string is not blank, use a Contains clause
                    pred = pred.And(p => p.Name.Contains(search));      // Only Persons whose name match our search criteria
                string franchiseKey = SupportingActions.GetFranchiseKey(); // Fetch the current key
                pred = pred.And(p => p.FranchiseKey == franchiseKey);   // Only for this Franchise

                List<Person> persons = context.Persons.AsExpandable().Where(pred).OrderBy(p => p.Name).ToList(); // Query, sort and make list
                PersonView.DataSource = persons;                        // Give it to the GridView control
                PersonView.DataBind();                                  // And display it

                NavigationActions.EnableGridViewNavButtons(PersonView); // Enable appropriate nav buttons based on page count
            }
        }
    }
}
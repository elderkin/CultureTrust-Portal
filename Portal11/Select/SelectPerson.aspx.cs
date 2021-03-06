﻿using LinqKit;
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
        //  Return - Propagated from caller

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                litSavedCommand.Text = QueryStringActions.GetCommand();     // Look on the query string that invoked this page. Find the command.
                litSavedReturn.Text = QueryStringActions.GetReturn();       // Find the caller's return address

                gvPerson.PageSize = CookieActions.GetGridViewRows();     // Find number of rows per page from cookie
                LoadgvPerson();                                           // Fill the grid
            }
        }

        protected void btnPersonSearch_Click(object sender, EventArgs e)
        {
            LoadgvPerson();                                              // Refresh the grid using updated search criteria
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void gvPerson_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Person";            // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvPerson, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged

                Label inactiveLabel = (Label)e.Row.FindControl("lblInactive");
                inactiveLabel.Visible = true;                               // Make sure the Inactive column appears if hidden earlier
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected.

        protected void gvPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.Enabled = true;                                           // With a row selected, we can act on a button click
        }

        // Deal with pagination of the Grid View controls

        protected void gvPerson_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                gvPerson.PageIndex = e.NewPageIndex;                     // Propagate the desired page index
                LoadgvPerson();                                          // Fill the grid
                gvPerson.SelectedIndex = -1;                             // No row currently selected
            }
        }

        // Cancel, eh? Go to the Staff Dashboard (or wherever).

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //            NavigationActions.NextPage("");
            Response.Redirect(litSavedReturn.Text);
        }

        // New button. Invoke Edit Person to create a new Person

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLEditPerson
                      + "?" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew
                      + "&" + PortalConstants.QSReturn + "=" + litSavedReturn.Text);
        }
        // Select button pushed to select a Person. Pass this to the EditPerson page.

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            Label label = (Label)gvPerson.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains RqstID
            string personID = label.Text;                                  // Extract the text of the control, which is RqstID
            if (personID == "")
                LogError.LogQueryStringError("SelectPerson", $"PersonID '{personID}' from selected GridView row missing"); // Log fatal error

            Response.Redirect(PortalConstants.URLEditPerson
                      + "?" + PortalConstants.QSPersonID + "=" + personID
                      + "&" + PortalConstants.QSCommand + "=" + litSavedCommand.Text
                      + "&" + PortalConstants.QSReturn + "=" + litSavedReturn.Text);
        }

        protected void chkInactive_CheckedChanged(object sender, EventArgs e)
        {
            LoadgvPerson();                                              // Reload the list based on the new un/checked value
        }

        // Fetch all the Persons and load them into a GridView

        void LoadgvPerson()
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
                gvPerson.DataSource = persons;                        // Give it to the GridView control
                gvPerson.DataBind();                                  // And display it

                // As a flourish, if the "Include Inactive" checkbox is not checked, do not display the Inactive column

                gvPerson.Columns[Person.InactiveColumn].Visible = chkInactive.Checked; // If checked, column is visible
                NavigationActions.EnableGridViewNavButtons(gvPerson); // Enable appropriate nav buttons based on page count
            }
        }
    }
}
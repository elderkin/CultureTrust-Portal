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
    public partial class SelectEntity : System.Web.UI.Page
    {

        // We get to this page from the Admin user's Edit Entity menu item. The Query String comtains an "Edit" command.
        // We come here to choose the Entity before dispatching to Edit Entity itself.
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
                gvAllEntity.PageSize = CookieActions.GetGridViewRows();    // Find number of rows per page from cookie
                LoadEntityView();                                           // Fill the grid
            }
        }

        protected void btnEntitySearch_Click(object sender, EventArgs e)
        {
            LoadEntityView();                                              // Refresh the grid using updated search criteria
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

                Label inactiveLabel = (Label)e.Row.FindControl("lblInactive");
                inactiveLabel.Visible = true;                               // Make sure the Inactive column appears if hidden earlier
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected. Shared by both controls.

        protected void gvAllEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.Enabled = true;                                       // With a row selected, we can act on a button click
        }

        // Deal with pagination of the Grid View controls

        protected void gvAllEntity_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                gvAllEntity.PageIndex = e.NewPageIndex;                 // Propagate the desired page index
                LoadEntityView();                                       // Fill the grid
                gvAllEntity.SelectedIndex = -1;                         // No row currently selected
            }
        }

        // Cancel, eh? Figure out where to go next.

        protected void btnCancel_Click(object sender, EventArgs e)
        {
//            NavigationActions.NextPage("");
            Response.Redirect(litSavedReturn.Text);                     
        }

        // New button. Invoke Edit Entity to create a new Entity

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLEditEntity 
                      + "?" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew
                      + "&" + PortalConstants.QSReturn + "=" + litSavedReturn.Text);
        }

        // Select button pushed to select a Entity. Dispatch to the page associated with the Command query string parameter - EditEntity.

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            Label entID = (Label)gvAllEntity.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains EntityID
            Label name = (Label)gvAllEntity.SelectedRow.Cells[1].FindControl("lblName"); // Find the label control that contains Entity Name
            string entityID = entID.Text;                                   // Extract the text of the control, which is EntityID as a string
            if (entityID == "")
                LogError.LogQueryStringError("SelectEntity", $"EntityID '{entityID}' from selected GridView row is missing"); // Log fatal error

            Response.Redirect(PortalConstants.URLEditEntity 
                      + "?" + PortalConstants.QSEntityID + "=" + entityID 
                      + "&" + PortalConstants.QSEntityName + "=" + name.Text 
                      + "&" + PortalConstants.QSCommand + "=" + litSavedCommand.Text
                      + "&" + PortalConstants.QSReturn + "=" + litSavedReturn.Text);

        }

        protected void chkInactive_CheckedChanged(object sender, EventArgs e)
        {
            LoadEntityView();                                              // Reload the list based on the new un/checked value
        }

        // Fetch all the Entitys and load them into a GridView. 

        void LoadEntityView()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // Build a predicate that accounts for the Franchise, the Inactive check box and the Search text box.

                var pred = PredicateBuilder.True<Entity>();             // Initialize predicate to select from Entity table

                string fran = SupportingActions.GetFranchiseKey();      // Fetch current franchise key
                pred = pred.And(p => p.FranchiseKey == fran);           // Show only Entitys for this franchise

                if (!chkInactive.Checked)                               // If false, we do not want Inactive Entitys
                    pred = pred.And(p => !p.Inactive);                  // Only active Entitys
                string search = txtEntity.Text;                         // Fetch the string that the user typed in, if any
                if (search != "")                                       // If != the search string is not blank, use a Contains clause
                    pred = pred.And(p => p.Name.Contains(search));      // Only Entitys whose name match our search criteria

                List<Entity> ents = context.Entitys.AsExpandable().Where(pred).OrderBy(p => p.Name).ToList(); // Query, sort and make list
                gvAllEntity.DataSource = ents;                        // Give it to the GridView control
                gvAllEntity.DataBind();                               // And display it

                // As a flourish, if the "Include Inactive" checkbox is not checked, do not display the Inactive column

                gvAllEntity.Columns[Entity.InactiveColumn].Visible = chkInactive.Checked; // If checked, column is visible

                NavigationActions.EnableGridViewNavButtons(gvAllEntity); // Enable appropriate nav buttons based on page count
            }
        }
    }
}
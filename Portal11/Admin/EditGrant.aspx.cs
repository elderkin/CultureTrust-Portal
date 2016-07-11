using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Admin
{
    public partial class EditGrant : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Create a new Grant, Edit an existing Grant. Communication from Grant menu is through Query Strings:
            //      Command - "New" or "Edit" (Required)
            //      GrantID - the database ID of the Grant that owns this Request. If absent, invoke Select Grant to find the Grant.

            if (!Page.IsPostBack)
            {
                int GrantID = Convert.ToInt32(Request.QueryString[PortalConstants.QSGrantID]); // Fetch the GrantID, if present
                string cmd = Request.QueryString[PortalConstants.QSCommand];    // Fetch the command

                // Stash these parameters into invisible literals on the current page.

                litSavedGrantID.Text = GrantID.ToString();
                litSavedCommand.Text = cmd;

                // Dispatch by Command

                switch (cmd)
                {
                    case PortalConstants.QSCommandNew:                  // Process a "New" command. Create new, empty Grant for editing
                        {
                            // We don't have to "fill" any of the controls on the page, just use them in their initial state.
                            FillGrantMakerDDL(null);                    // Except for this drop down list, that is!
                            this.Title = "New Grant";                   // Show that we are creating a new Grant
                            litSuccessMessage.Text = "New Grant is ready to edit";
                            break;
                        }
                    case PortalConstants.QSCommandEdit:                 // Process an "Edit" command. Read existing request, save it in same row
                        {

                            // If GrantID is blank, we don't know which Grant to Edit. Invoke SelectGrant page to figure that out and come back here.

                            if (GrantID == 0)                            // If == Query String was absent. Go find which Grant
                            {
                                Response.Redirect("~/Select/SelectGrant?" + PortalConstants.QSGrantID + "=" + GrantID + "&" +
                                                                     PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit);
                            }

                            // Fetch the row from the database. Fill in the panels using data rom the existing request. Lotta work!
                            litSuccessMessage.Text = "Selected Grant is ready to edit";
                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                Grant toEdit = context.Grants.Find(GrantID); // Fetch Grant row by its key
                                if (toEdit == null)
                                {
                                    LogError.LogInternalError("EditGrant", string.Format("Unable to find GrantID '{0}' in database",
                                        GrantID.ToString())); // Fatal error
                                }
                                LoadPanels(toEdit);                         // Fill in the visible panels from the request
                                FillGrantMakerDDL(toEdit.GrantMakerID);     // Except for this drop down list, that is!
                            }
                            break;
                        }
                    default:
                        LogError.LogQueryStringError("EditGrant", "Missing Query String 'Command'"); // Log the fatal error
                        return;                                             // Don't return any Data to caller
                }
            }
        }

        // Cancel button has been clicked. Just return to the main Admin page.

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLAdminMain);                // Back to the barn. Nothing to save.
        }

        // Save button clicked. "Save" means that we unload all the controls for the Grant into a database row. 
        // If the Grant is new, we just add a new row. If the Grant already exists, we update it and add a history record to show the edit.
        // We can tell a Grant is new if the Query String doesn't contain a mention of a GrantID AND the literal litSavedGrantID is empty.
        // On the first such Save, we stash the GrantID of the new Grant into this literal so that a second Save during the same session will find
        // it and update it, rather than creating a second row.

        protected void Save_Click(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    int GrantID = 0;                                            // Assume no saved ID is available
                    if (litSavedGrantID.Text != "") GrantID = Convert.ToInt32(litSavedGrantID.Text); // If != saved ID is available, use it  
                    if (GrantID == 0)                                           // If == no doubt about it, Save makes a new row
                    {
                        Grant toSave = new Grant();                             // Get a place to hold everything
                        toSave.CreatedTime = System.DateTime.Now;               // Stamp time when Grant was first created as "now"
                        UnloadPanels(toSave);                                   // Move from Panels to record
                        if (toSave.CurrentFunds > toSave.OriginalFunds)         // If < more money now than originally. That's bad
                        {
                            litDangerMessage.Text = "Current Funds cannot exceed Original Funds"; // Post the error message
                            litSuccessMessage.Text = "";
                            return;                                             // Try again
                        }
                        context.Grants.Add(toSave);                             // Save new Rqst row
                        context.SaveChanges();                                  // Commit the Add
                    }
                    else
                    {
                        Grant toUpdate = context.Grants.Find(GrantID);          // Fetch the Grant that we want to update
                        UnloadPanels(toUpdate);                                 // Move from Panels to record, modifying it in the process
                        if (toUpdate.CurrentFunds > toUpdate.OriginalFunds)         // If < more money now than originally. That's bad
                        {
                            litDangerMessage.Text = "Current Funds cannot exceed Original Funds"; // Post the error message
                            litSuccessMessage.Text = "";
                            return;                                             // Try again
                        }
                        context.SaveChanges();                                  // Commit the Add or Modify
                    }
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditGrant", "Error writing Grant row"); // Fatal error
                }
            }
            // Now go back to Dashboard
            Response.Redirect(PortalConstants.URLAdminMain + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                                + PortalConstants.QSStatus + "=Grant saved");
        }

        // Based on the selected Request Type, enable and disable the appropriate panels on the display.

        // Move values from the Grant record into the Page.

        void LoadPanels(Grant record)
        {
            // Grant Name
            txtName.Text = record.Name;
            // Inactive flag
            chkInact.Checked = record.Inactive;
            // Description
            txtDescription.Text = record.Description;
            // Original Funds
            txtOriginalFunds.Text = record.OriginalFunds.ToString("C");
            // Current Funds
            txtCurrentFunds.Text = record.CurrentFunds.ToString("C");
            // Grant Maker
        }

        // Move the "as edited" values from the Page into a Grant record.

        void UnloadPanels(Grant record)
        {
            // Request Summary
            record.Name = txtName.Text;
            // Inactive Flag
            record.Inactive = chkInact.Checked;
            // Address
            record.Description = txtDescription.Text;
            // Original Funds
            record.OriginalFunds = (decimal)ConvertCurrency(txtOriginalFunds.Text); // Carefully convert currency from text to decimal
            // Current Funds
            record.CurrentFunds = (decimal)ConvertCurrency(txtCurrentFunds.Text); // Carefully convert currency from text to decimal
            // Grant Maker
            record.GrantMakerID = (int)StateActions.UnloadDdl(ddlGrantMaker); // Carefully pull the Grant Maker ID
        }

        decimal? ConvertCurrency(string amount)
        {
            decimal bucks;
            bool convt = decimal.TryParse(amount, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out bucks);
            // Use a localization-savvy method to convert string to decimal
            if (convt) return bucks;                            // If true conversion was successful, stash value
            return null;                                        // Else, do no damage
        }

        // Gill the Grant Maker drop down list from the database

        void FillGrantMakerDDL(int? grantMakerID)
        {
            if (ddlGrantMaker.Items.Count == 0)             // If = the control is empty. Fill it
            {
                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    DataTable grantMakers = new DataTable();
                    grantMakers.Columns.Add("ID");
                    grantMakers.Columns.Add("Name");

                    var query = from gm in context.GrantMakers
                                where !gm.Inactive
                                select new { gm.GrantMakerID, gm.Name }; // Find all Grant Makers that are active

                    foreach (var row in query)
                    {
                        DataRow dr = grantMakers.NewRow();  // Build a row from the next query output
                        dr["ID"] = row.GrantMakerID;
                        dr["Name"] = row.Name;
                        grantMakers.Rows.Add(dr);           // Add the new row to the data table
                    }
                    StateActions.LoadDdl(ddlGrantMaker, grantMakerID, grantMakers, 
                        "-- Error: No Grant Makers in database --", "-----------------"); // Put the cherry on top
                }
            }
        }
    }
}
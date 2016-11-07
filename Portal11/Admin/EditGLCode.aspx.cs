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
    public partial class EditGLCode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Create a new GLCode, Edit an existing GLCode. Communication from GLCode menu is through Query Strings:
            //      Command - "New" or "Edit" (Required)
            //      GLCodeID - the database ID of the GLCode that owns this Request. If absent, invoke Select GLCode to find the GLCode.

            if (!Page.IsPostBack)
            {
                int GLCodeID = Convert.ToInt32(Request.QueryString[PortalConstants.QSGLCodeID]); // Fetch the GLCodeID, if present
                string cmd = Request.QueryString[PortalConstants.QSCommand];    // Fetch the command

                // Stash these parameters into invisible literals on the current page.

                litSavedGLCodeID.Text = GLCodeID.ToString();
                litSavedCommand.Text = cmd;

                // Dispatch by Command

                switch (cmd)
                {
                    case PortalConstants.QSCommandNew:                  // Process a "New" command. Create new, empty GLCode for editing
                        {
                            // We don't have to "fill" any of the controls on the page, just use them in their initial state.
                            this.Title = "New GLCode";                   // Show that we are creating a new GLCode
                            //litSuccessMessage.Text = "New GLCode is ready to edit";
                            break;
                        }
                    case PortalConstants.QSCommandEdit:                 // Process an "Edit" command. Read existing request, save it in same row
                        {

                            // If GLCodeID is blank, we don't know which GLCode to Edit. Invoke SelectGLCode page to figure that out and come back here.

                            if (GLCodeID == 0)                            // If == Query String was absent. Go find which GLCode, then come back to this page
                            {
                                Response.Redirect(PortalConstants.URLSelectGLCode + "?" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit);
                            }

                            // Fetch the row from the database. Fill in the panels using data rom the existing request. Lotta work!
                            litSuccessMessage.Text = "Selected GLCode is ready to edit";
                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                GLCode toEdit = context.GLCodes.Find(GLCodeID); // Fetch GLCode row by its key
                                if (toEdit == null)
                                {
                                    LogError.LogInternalError("EditGLCode", string.Format("Unable to find GLCodeID '{0}' in database",
                                        GLCodeID.ToString())); // Fatal error
                                }
                                LoadPanels(toEdit);                         // Fill in the visible panels from the request
                            }
                            break;
                        }
                    default:
                        LogError.LogQueryStringError("EditGLCode", "Missing Query String 'Command'"); // Log the fatal error
                        return;                                             // Don't return any Data to caller
                }
            }
        }

        // Cancel button has been clicked. Just return to the main Admin page.

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLAdminMain);                // Back to the barn. Nothing to save.
        }

        // Save button clicked. "Save" means that we unload all the controls for the GLCode into a database row. 
        // If the GLCode is new, we just add a new row. If the GLCode already exists, we update it and add a history record to show the edit.
        // We can tell a GLCode is new if the Query String doesn't contain a mention of a GLCodeID AND the literal litSavedGLCodeID is empty.
        // On the first such Save, we stash the GLCodeID of the new GLCode into this literal so that a second Save during the same session will find
        // it and update it, rather than creating a second row.

        protected void Save_Click(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    // Break out by whether this is a new code or an existing code

                    string fran = SupportingActions.GetFranchiseKey();          // Fetch current franchise key
                    int GLCodeID = 0;                                           // Assume no saved ID is available
                    if (litSavedGLCodeID.Text != "") GLCodeID = Convert.ToInt32(litSavedGLCodeID.Text); // If != saved ID is available, use it  
                    if (GLCodeID == 0)                                          // If == no doubt about it, Save makes a new row
                    {

                        // Make sure the GL Code is unique in this Franchise

                        int matchingCodes = context.GLCodes.Where(p => (p.FranchiseKey == fran) && (p.Code == txtCode.Text)).Count();
                        // Look for existing row match
                        if (matchingCodes != 0)                                 // If != a GLCode with this Code value already exists in database
                        {
                            Response.Redirect(PortalConstants.URLAdminMain + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSDanger + "&"
                                                + PortalConstants.QSStatus + "=That GL Code already exists. You can edit it.", false); // Back to the barn
                            return;
                        }

                        // OK, we're a new Code. Fill a record and write it to database as a new row

                        GLCode toSave = new GLCode()                            // Get a place to hold everything
                        {
                            FranchiseKey = fran,                                // Insert current franchise key
                            CreatedTime = System.DateTime.Now                   // Stamp time when GLCode was created as "now"
                        };
                        UnloadPanels(toSave);                                   // Move from Panels to record
                        context.GLCodes.Add(toSave);                            // Save new GLCode row
                        context.SaveChanges();                                  // Commit the Add
                    }
                    else
                    {
                        //TODO: There's a corner case in which the  GLCode gets its Code changed to something that already exists. Would anyone do that?
                        //TODO: Maybe Code should be immutable once created.
                        GLCode toUpdate = context.GLCodes.Find(GLCodeID);       // Fetch the ID of the GLCode that we want to update
                        UnloadPanels(toUpdate);                                 // Move from Panels to record, modifying it in the process
                        context.SaveChanges();                                  // Commit the Add or Modify
                    }
                }
                catch (Exception ex)
                {
                    if (ExceptionActions.IsDuplicateKeyException(ex))       // If true this is a Duplicate Key exception
                    {
                        litDangerMessage.Text = $"Another GLCode with the code '{txtCode.Text}' already exists. GLCodes must be unique.";
                        return;                                             // Report the error to user and try again
                    }
                    LogError.LogDatabaseError(ex, "EditGLCode", "Error writing GLCode row"); // Fatal error
                }
            }
            // Now go back to Main Admin page
            Response.Redirect(PortalConstants.URLAdminMain + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                                + PortalConstants.QSStatus + "=GLCode saved");
        }

        // Based on the selected Request Type, enable and disable the appropriate panels on the display.

        // Move values from the GLCode record into the Page.

        void LoadPanels(GLCode record)
        {
            // GLCode 
            txtCode.Text = record.Code;
            // Inactive flag
            chkInact.Checked = record.Inactive;
            // ExpCode flag
            chkExp.Checked = record.ExpCode;
            // DepCode flag
            chkDep.Checked = record.DepCode;
            // Description
            txtDescription.Text = record.Description;
        }

        // Move the "as edited" values from the Page into a GLCode record.

        void UnloadPanels(GLCode record)
        {
            // GLCode
            record.Code = txtCode.Text;
            // Inactive Flag
            record.Inactive = chkInact.Checked;
            // Exp flag
            record.ExpCode = chkExp.Checked;
            // Dep flag
            record.DepCode = chkDep.Checked;
            // Description
            record.Description = txtDescription.Text;
        }
    }
}
﻿using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Web.UI;

namespace Portal11.Admin
{
    public partial class EditEntity : System.Web.UI.Page
    {
        // Create a new Entity, Edit an existing Entity. Communication from Entity menu is through Query Strings:
        //      Command - "New" or "Edit" (Required)
        //      UserID - the ID of the user making this request (optional)
        //      EntityID - the database ID of the Entity to be edited. If absent, invoke Select Entity to find the Entity.
        //      ProjectID - propagated to caller
        //      Return - the URL to which we return when processing is complete. If blank, we return to the Admin page. (required)
        //      Return2 and EntityRole - Optional stuff from our caller. If it's there, we send it back

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string userID = QueryStringActions.GetUserID();             // Fetch User ID. If absent, check UserInfoCookie
                QSValue entityID = QueryStringActions.GetEntityID();        // Fetch the EntityID, if present
                string cmd = QueryStringActions.GetCommand();               // Fetch the command
                litSavedReturn.Text = QueryStringActions.GetReturn();       // Fetch the return page name and save for later

                // Stash these parameters into invisible literals on the current page.

                litSavedUserID.Text = userID;
                litSavedEntityID.Text = entityID.String;
                litSavedCommand.Text = cmd;

                SupportingActions.CleanupTemp(userID, litDangerMessage);    // Cleanup supporting docs from previous executions for this user

                // Dispatch by Command

                switch (cmd)
                {
                    case PortalConstants.QSCommandNew:                      // Process a "New" command. Create new, empty Entity for editing
                        {
                            // We don't have to "fill" any of the controls on the page, just use them in their initial state.
                            this.Title = "New Entity";                      // Show that we are creating a new Entity
                            //litSuccessMessage.Text = "New Entity is ready to edit";
                            break;
                        }
                    case PortalConstants.QSCommandEdit:                     // Process an "Edit" command. Read existing request, save it in same row
                        {
                            if (entityID.Int == 0)                          // If == Query String was absent. Go find which Entity then come back here
                            {
                                Response.Redirect(PortalConstants.URLSelectEntity 
                                          + "?" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit
                                          + "=" + PortalConstants.QSReturn + "=" + litSavedReturn.Text);
                            }

                            // Fetch the row from the database. Fill in the panels using data rom the existing request. Lotta work!

                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                Entity toEdit = context.Entitys.Find(entityID.Int); // Fetch Entity row by its key
                                if (toEdit == null)
                                {
                                    LogError.LogInternalError("EditEntity", $"Unable to find EntityID '{entityID}' in database"); // Fatal error
                                }
                                LoadPanels(toEdit);                         // Fill in the visible panels from the request
                                SupportingActions.LoadDocs(RequestType.Entity, entityID.Int, lstSupporting, litDangerMessage); // Do the heavy lifting
                            }
                            litSuccessMessage.Text = "Selected Entity is ready to edit";
                            break;
                        }
                    default:
                        LogError.LogQueryStringError("EditEntity", "Missing Query String 'Command'"); // Log fatal error (and don't return here)
                        return;                                             // Don't return any Data to caller
                }
            }
            else                                                    // We are in Postback. See if FileUpload control has anything for us
            {
                litDangerMessage.Text = ""; litSuccessMessage.Text = ""; // Start with a clean slate of message displays
                if (fupUpload.HasFile)                              // If true PostBack was caused by File Upload control
                    SupportingActions.UploadDoc(fupUpload, lstSupporting, litSavedUserID, litSuccessMessage, litDangerMessage); // Do heavy lifting to get file
                                                                    // and record information in new SupportingDocTemp row
            }
        }

        // User has clicked on a row of the Supporting Docs list. Just turn on the buttons that work now.

        protected void lstSupporting_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnView.Enabled = true;
            btnRem.Enabled = true;                              // Otherwise, enable the Remove button
            return;
        }

        // Remove the selected Supporting Doc.

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            SupportingActions.RemoveDoc(lstSupporting, litSuccessMessage, litDangerMessage); // Do all the heavy lifting
            return;
        }

        // View a selection from the Supporting Listbox. This is a request to download the selected doc.

        protected void btnView_Click(object sender, EventArgs e)
        {
            SupportingActions.ViewDoc(lstSupporting, litDangerMessage);
        }

        // Cancel button has been clicked. Just return to the main Admin page.

        protected void Cancel_Click(object sender, EventArgs e)
        {
            ReturnToCaller("");                                             // Leave with the guy that brung us (without status)
        }

        // Save button clicked. "Save" means that we unload all the controls for the Entity into a database row. 
        // If the Entity is new, we just add a new row. If the Entity already exists, we update it and add a history record to show the edit.
        // We can tell a Entity is new if the Query String doesn't contain a mention of a EntityID AND the literal litSavedEntityID is empty.

        protected void Save_Click(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    int entityID = 0;                                       // Assume no saved ID is available
                    if (litSavedEntityID.Text != "") entityID = Convert.ToInt32(litSavedEntityID.Text); // If != saved ID is available, use it  
                    if (entityID == 0)                                      // If == no doubt about it, Save makes a new row
                    {
                        Entity toSave = new Entity();                       // Get a place to hold everything
                        toSave.CreatedTime = System.DateTime.Now;           // Stamp time when Entity was first created as "now"
                        UnloadPanels(toSave);                               // Move from Panels to record
                        toSave.FranchiseKey = SupportingActions.GetFranchiseKey(); // Fetch current franchise key
                        context.Entitys.Add(toSave);                        // Save new Rqst row
                        context.SaveChanges();                              // Commit the Add
                        entityID = toSave.EntityID;                         // Pocket the newly assigned Entity ID
                    }
                    else
                    {
                        Entity toUpdate = context.Entitys.Find(entityID);   // Fetch the Entity that we want to update
                        UnloadPanels(toUpdate);                             // Move from Panels to record, modifying it in the process
                        context.SaveChanges();                              // Commit the Add or Modify
                    }
                    SupportingActions.UnloadDocs(lstSupporting, litSavedUserID.Text, RequestType.Entity, entityID, litDangerMessage);
                }
                catch (Exception ex)
                {
                    if (ExceptionActions.IsDuplicateKeyException(ex))       // If true this is a Duplicate Key exception
                    {
                        litDangerMessage.Text = $"Another Entity with the name '{txtName.Text}' already exists. Entity Names must be unique.";
                        return;                                             // Report the error to user and try again
                    }
                    LogError.LogDatabaseError(ex, "EditEntity", "Error updating Entity row"); // Otherwise, log a Fatal error and don't return here
                }
            }
            ReturnToCaller("Entity saved");                                 // Report success on the way out
        }

        // Return to the "caller", propagating query string parameters

        void ReturnToCaller(string status)
        {
            string running = litSavedReturn.Text + "?" + PortalConstants.QSNull; // Fetch the Return URL. That's our destination. At least one parameter

            if (!string.IsNullOrEmpty(status))                              // If false, propagate status
                running += "&" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess
                         + "&" + PortalConstants.QSStatus + "=" + status;   // Tack on Severity and Status query strings

            string proj = Request.QueryString[PortalConstants.QSProjectID]; // Fetch ProjectID query string, if present
            if (!string.IsNullOrEmpty(proj))                                // If false propagate ProjectID
                running += "&" + PortalConstants.QSProjectID + "=" + proj;  // Propagate Project ID

            string ret2 = Request.QueryString[PortalConstants.QSReturn2];   // Fetch Return2 parameter, if present
            if (!string.IsNullOrEmpty(ret2))                                // If false parameter present, propagate it
                running += "&" + PortalConstants.QSReturn + "=" + ret2;     // Propagate caller's return

            string ent = Request.QueryString[PortalConstants.QSEntityRole]; // Fetch EntityRole parameter, if present
            if (!string.IsNullOrEmpty(ent))                                 // If false parameter present, propagate it
                running += "&" + PortalConstants.QSEntityRole + "=" + ent;  // Propagate caller's parameter

            Response.Redirect(running);                                     // Return to the "caller" with QS parameters
        }

        // Move values from the Entity record into the Page.

        void LoadPanels(Entity record)
        {
            txtName.Text = record.Name;
            chkInact.Checked = record.Inactive;
            txtAddress.Text = record.Address;
            txtPhone.Text = record.Phone;
            txtEmail.Text = record.Email;
            txtURL.Text = record.URL;
            txtNotes.Text = record.Notes;
        }

        // Move the "as edited" values from the Page into a Entity record.

        void UnloadPanels(Entity record)
        {
            record.Name = txtName.Text;
            record.Inactive = chkInact.Checked;
            record.EntityType = EntityType.ReservedForFutureUse;
            record.Address = txtAddress.Text;
            record.Phone = txtPhone.Text;
            record.Email = txtEmail.Text;
            record.URL = txtURL.Text;
            record.Notes = txtNotes.Text;
        }

    }
}
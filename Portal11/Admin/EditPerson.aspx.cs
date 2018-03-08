using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Web.UI;

namespace Portal11.Admin
{
    public partial class EditPerson : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Create a new Person, Edit an existing Person. Communication from Person menu is through Query Strings:
            //      Command - "New" or "Edit" (Required)
            //      UserID - the ID of the user making this request (optional)
            //      PersonID - the database ID of the Person to be edited. If absent, invoke Select Entity to find the Entity.
            //      ProjectID - propagated to caller
            //      Return - the URL to which we return when processing is complete. If blank, we return to the Admin page. (required)
            //      Return2, PersonRole - the caller's stuff. We propagate this (optional)


            if (!Page.IsPostBack)
            {
                string userID = QueryStringActions.GetUserID();             // Fetch User ID. If absent, check UserInfoCookie
                QSValue personID = QueryStringActions.GetPersonID();        // Fetch the PersonID, if present
                string cmd = Request.QueryString[PortalConstants.QSCommand];    // Fetch the command
                litSavedReturn.Text = QueryStringActions.GetReturn();       // Fetch the return page name and save for later

                // Stash these parameters into invisible literals on the current page.

                litSavedUserID.Text = userID;
                litSavedPersonID.Text = personID.String;
                litSavedCommand.Text = cmd;

                SupportingActions.CleanupTemp(userID, litDangerMessage);    // Cleanup supporting docs from previous executions for this user

                // Dispatch by Command

                switch (cmd)
                {
                    case PortalConstants.QSCommandNew:                      // Process a "New" command. Create new, empty Person for editing
                        {
                            // We don't have to "fill" any of the controls on the page, just use them in their initial state.
                            this.Title = "New Person";                      // Show that we are creating a new Employee
                            //litSuccessMessage.Text = "New Person is ready to edit";
                            break;
                        }
                    case PortalConstants.QSCommandEdit:                     // Process an "Edit" command. Read existing request, save it in same row
                        {

                            // If PersonID is blank, we don't know which Person to Edit. Invoke SelectPerson page to figure that out and come back here.

                            if (personID.Int == 0)                          // If == Query String was absent. Go find which Person then come back here
                            {
                                Response.Redirect(PortalConstants.URLSelectPerson
                                          + "?" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit
                                          + "=" + PortalConstants.QSReturn + "=" + litSavedReturn.Text);
                            }

                            // Fetch the row from the database. Fill in the panels using data rom the existing request. Lotta work!
                            litSuccessMessage.Text = "Selected Person is ready to edit";
                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                Person toEdit = context.Persons.Find(personID.Int); // Fetch Person row by its key
                                if (toEdit == null)
                                {
                                    LogError.LogInternalError("EditPerson", $"Unable to find PersonID '{personID.String}' in database"); // Fatal error
                                }
                                LoadPanels(toEdit);                         // Fill in the visible panels from the request
                                SupportingActions.LoadDocs(RequestType.Person, personID.Int, lstSupporting, litDangerMessage); // Do the heavy lifting
                            }
                            break;
                        }
                    default:
                        LogError.LogQueryStringError("EditPerson", "Missing Query String 'Command'"); // Log fatal error (and don't return here)
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
            return;
        }

        // Cancel button has been clicked. Just return to the main Admin page.

        protected void Cancel_Click(object sender, EventArgs e)
        {
            ReturnToCaller("");                                             // Leave with the guy that brung us (without status)
        }

        // Save button clicked. "Save" means that we unload all the controls for the Person into a database row. 
        // If the Person is new, we just add a new row. If the Person already exists, we update it and add a history record to show the edit.
        // We can tell a Person is new if the Query String doesn't contain a mention of a PersonID AND the literal litSavedPersonID is empty.
        // On the first such Save, we stash the PersonID of the new Person into this literal so that a second Save during the same session will find
        // it and update it, rather than creating a second row.

        protected void Save_Click(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    int personID = 0;                                            // Assume no saved ID is available
                    if (litSavedPersonID.Text != "") personID = Convert.ToInt32(litSavedPersonID.Text); // If != saved ID is available, use it  
                    if (personID == 0)                                           // If == no doubt about it, Save makes a new row
                    {
                        Person toSave = new Person();                          // Get a place to hold everything
                        toSave.CreatedTime = System.DateTime.Now;                  // Stamp time when Request was first created as "now"
                        UnloadPanels(toSave);                                      // Move from Panels to record
                        toSave.FranchiseKey = SupportingActions.GetFranchiseKey(); // Fetch current franchise key
                        context.Persons.Add(toSave);                             // Save new Rqst row
                        context.SaveChanges();                                     // Commit the Add
                        personID = toSave.PersonID;                            // Pocket the newly assigned Person ID
                    }
                    else
                    {
                        Person toUpdate = context.Persons.Find(personID);    // Fetch the Person that we want to update
                        UnloadPanels(toUpdate);                                    // Move from Panels to record, modifying it in the process
                        context.SaveChanges();                                     // Commit the Add or Modify
                    }
                    SupportingActions.UnloadDocs(lstSupporting, litSavedUserID.Text, RequestType.Person, personID, litDangerMessage);
                }
                catch (Exception ex)
                {
                    if (ExceptionActions.IsDuplicateKeyException(ex))       // If true this is a Duplicate Key exception
                    {
                        litDangerMessage.Text = $"Another Person with the name '{txtName.Text}' already exists. Person Names must be unique.";
                        return;                                             // Report the error to user and try again
                    }
                    LogError.LogDatabaseError(ex, "EditPerson", "Error updating Person row"); // Fatal error
                }
            }
            ReturnToCaller("Person saved");                                 // Report success on the way out
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

            string ent = Request.QueryString[PortalConstants.QSPersonRole]; // Fetch PersonRole parameter, if present
            if (!string.IsNullOrEmpty(ent))                                 // If false parameter present, propagate it
                running += "&" + PortalConstants.QSPersonRole + "=" + ent;  // Propagate caller's parameter

            Response.Redirect(running);                                     // Return to the "caller" with QS parameters
        }

        // Move values from the Person record into the Page.

        void LoadPanels(Person record)
        {
            txtName.Text = record.Name;
            chkInact.Checked = record.Inactive;
            txtAddress.Text = record.Address;
            txtPhone.Text = record.Phone;
            txtEmail.Text = record.Email;
            chkW9Present.Checked = record.W9Present;
            txtNotes.Text = record.Notes;
        }

        // Move the "as edited" values from the Page into a Person record.

        void UnloadPanels(Person record)
        {
            record.Name = txtName.Text;
            record.Inactive = chkInact.Checked;
            record.Address = txtAddress.Text;
            record.Phone = txtPhone.Text;
            record.Email = txtEmail.Text;
            record.W9Present = chkW9Present.Checked;
            record.Notes = txtNotes.Text;
        }
    }
}
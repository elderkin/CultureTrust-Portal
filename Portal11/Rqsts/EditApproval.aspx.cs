using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Rqsts
{
    public partial class EditApproval : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // Find the User's role on the Project. In the process, make sure user is logged in.

                string projRole = QueryStringActions.GetProjectRole();  // Fetch user's Project Role from cookie

                // Fetch and validate the four Query String parameters. Carefully convert the integer parameters to integers.

                string userID = QueryStringActions.GetUserID();         // First parameter - User ID. If absent, check UserInfoCookie
                QSValue projectID = QueryStringActions.GetProjectID();  // Second parameter - Project ID. If absent, check ProjectInfoCookie
                QSValue appID = QueryStringActions.GetRequestID();      // Third parameter - Approval ID. If absent, must be New command
                string cmd = QueryStringActions.GetCommand();           // Fourth parameter - Command. Must be present

                // Stash these parameters into invisible literals on the current page.

                litSavedAppID.Text = "";                                // Assume New or Copy - do not modify an existing row, but add a new one
                litSavedCommand.Text = cmd;
                litSavedProjectID.Text = projectID.String;
                litSavedProjectRole.Text = projRole;                    // Save in a faster spot for later
                litSavedUserID.Text = userID;

                SupportingActions.CleanupTemp(userID, litDangerMessage); // Cleanup supporting docs from previous executions for this user

                // Dispatch by Command

                switch (cmd)
                {
                    case PortalConstants.QSCommandCopy:                 // Process a "Copy" command. Read existing request, save it in new row
                        {
                            appID.Int = CopyApp(appID.Int);             // Copy the source App into a new destination App
                            appID.String = appID.Int.ToString();        // Also update the string version of the App ID
                            if (appID.Int == 0)                         // If == the copy process failed
                                LogError.LogInternalError("EditApproval", "Unable to copy existing Approval Request"); // Log fatal error

                            goto case PortalConstants.QSCommandEdit;    // Now edit the destination Exp
                        }
                    case PortalConstants.QSCommandEdit:                 // Process an "Edit" command. Read existing Approval, save it in same row
                        {

                            // Fetch the row from the database. Set the radio button corresponding to our Approval type,
                            // then disable the radio button group so the user can't change the Approval type.
                            // Make appropriate panels visible, then fill in the panels using data rom the existing Approval. Lotta work!

                            if (appID.Int == 0)                         // If == cannot edit a missing Exp
                                LogError.LogQueryStringError("EditApproval", "Missing Query String 'RequestID'"); // Log fatal error

                            litSavedAppID.Text = appID.String;          // Remember to write record back to its original spot, i.e., modify the row
                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                App app = context.Apps.Find(appID.Int); // Fetch App row by its key
                                if (app == null)
                                    LogError.LogInternalError("EditApproval", $"Unable to locate ApprovalID {appID.String} in database");
                                // Log fatal error

                                // Note: The rdo displays TEXT that matches the Description of each AppType, but the VALUE contains the enumerated AppType
                                rdoAppType.SelectedValue = app.AppType.ToString(); // Select the type of Approval
                                rdoAppType.Enabled = false;             // Disable the control - cannot change type of existing Approval

                                EnablePanels(app.AppType);              // Make the relevant panels visible
                                LoadPanels(app);                        // Fill in the visible panels from the Approval
                                LoadSupportingDocs(app);                // Fill in the Supporting Docs - it takes extra work
                            }
                            litSuccessMessage.Text = "Approval Request is ready to edit";
                            break;
                        }
                    case PortalConstants.QSCommandNew:              // Process a "New" command. Create new, empty Approval, save it in new row
                        {

                            // We don't have to "fill" any of the controls on the page, just use them in their initial state.
                            // The radio button group for Approval type wakes up "enabled" with nothing selected. No other controls are visible.
                            // When the user selects a Approval type, we'll use that click to make the appropriate panels visible.

                            AppState newState = StateActions.FindUnsubmittedAppState(litSavedProjectRole.Text); // Use User project role to find state
                            litSavedStateEnum.Text = newState.ToString(); // Stash actual enum value of AppState
                            txtState.Text = EnumActions.GetEnumDescription(newState); // Display "English" version of enum
                            litSuccessMessage.Text = "New Approval Request is ready to edit";
                            break;
                        }
                    case PortalConstants.QSCommandView:             // Process an "View" command. Read existing Approval, disable edit and save
                        {
                            litSavedAppID.Text = appID.String;      // Remember the ID of the row that we will view

                            // Fetch the row from the database. Set the radio button corresponding to our Approval type,
                            // then disable the radio button group so the user can't change the Approval type.
                            // Make appropriate panels visible, then fill in the panels using data from the existing Approval. 
                            // Set everything to readonly to prevent editing. Disable Save and Submit. Lotta work!

                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                App app = context.Apps.Find(appID.Int); // Fetch App row by its primary key
                                if (app == null)
                                    LogError.LogInternalError("EditApproval", $"Unable to find Approval ID '{appID.String}' in database"); // Fatal error

                                rdoAppType.SelectedValue = app.AppType.ToString(); // Select our Approval type
                                rdoAppType.Enabled = false;         // Disable the control - cannot change type of existing Approval
                                EnablePanels(app.AppType);          // Make the relevant panels visible
                                LoadPanels(app);                    // Fill in the visible panels from the Approval
                                LoadSupportingDocs(app);            // Fill in the Supporting Docs - it takes extra work
                                ReadOnlyControls();                 // Make all controls readonly for viewing

                                // Adjust the button settings for the View function

                                btnCancel.Text = "Done";            // Provide a friendly way out of this page
                                btnSave.Enabled = false;            // No "Save" for you!
                                btnSubmit.Enabled = false;
                                if (app.CurrentState == AppState.Returned) // If == viewing a returned Exp - special case
                                    btnRevise.Visible = true;       // Turn on the "Revise" button - specific to this case
                                btnShowHistory.Visible = true;      // We can show the history so make button visible 
                                pnlAdd.Visible = false;             // Can't figure out how to disable Add button, so make it invisible
                                btnRem.Visible = false;             // Remove button on Supporting Docs is also invisible
                                Page.Title = "View Approval Request";    // Note that we are viewing, not editing
                                litSuccessMessage.Text = "Existing Approval Request is ready to view";
                            }
                            break;
                        }
                    default:
                        LogError.LogQueryStringError("EditApproval", "Invalid Query String 'Command'"); // Log fatal error
                        return;                                     // Don't return any Data to caller
                }
            }
            else                                                    // We are in Postback. See if FileUpload control has anything for us
            {
                litDangerMessage.Text = ""; litSuccessMessage.Text = ""; // Start with a clean slate of message displays
                if (fupUpload.HasFile)                              // If true PostBack was caused by File Upload control
                    SupportingActions.UploadDoc(fupUpload, lstSupporting, litSavedUserID, litSuccessMessage, litDangerMessage); // Do heavy lifting to get file
                                                                                                                                // and record information in new SupportingDocTemp row
            }
            return;

        }

        // User pressed a radio button to select a Approval type. Each Approval type uses a different combination of panels.
        // Make visible the appropriate panels and fill the appropriate dropdown lists. Then do some housekeeping to get the
        // appropriate defaults applied. There ought to be a better way to do this, but I can't find it. By the way, this
        // control is only enabled when the record is unsaved. So the buttons work for New but not for Edit.

        protected void rdoAppType_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppType selected = EnumActions.ConvertTextToAppType(rdoAppType.SelectedValue); // Fetch selected button, convert value to enum
            EnablePanels(selected);                                     // Turn on/off controls based on selected Exp Type
            FillDropDownLists();                                        // Load dropdown lists with unselected database values
            return;
        }

        // User has clicked on a row of the Supporting Docs list. Just turn on the buttons that work now.
        // And don't ask me why, but unless we act, the page title reverts to its original value. So for the case of View, set it again.

        protected void lstSupporting_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnViewLink.Enabled = true; btnViewLink.Visible = true;
            btnViewLink.NavigateUrl = SupportingActions.ViewDocUrl((ListBox)sender); // Formulate URL to launch viewer page
            if (litSavedCommand.Text == PortalConstants.QSCommandView) // If == it is a View command
                Page.Title = "View Approval Request";                // Reset the page title. Don't know why it changed, but just reset it
            else
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

        //protected void btnView_Click(object sender, EventArgs e)
        //{
        //    SupportingActions.ViewDoc(lstSupporting, litDangerMessage);
        //    return;
        //}
        protected void gvEDHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                gvEDHistory.PageIndex = e.NewPageIndex;             // Propagate the desired page index
                NavigationActions.LoadAllAppHistorys(Convert.ToInt32(litSavedAppID.Text), gvEDHistory); // Fill the list from the database
                gvEDHistory.SelectedIndex = -1;                     // No row currently selected
            }
            return;
        }

        // Cancel button has been clicked. Just return to the Dashboard.

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLProjectDashboard);
            return;
        }

        // Revise button clicked. This is the case where a App is in the "Returned" state - it was submitted, but failed during the review process.
        // To "Revise," we: 
        //  1) Get a copy of the Request
        //  2) Create a History row to audit this change.
        //  3) Change the State of the Request from "Returned" to "Under Construction," erase the ReturnNote comments and save it.
        //  4) Call this page back and invoke the "Edit" command for an existing Approval.

        protected void btnRevise_Click(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  1) Get a copy of the App

                    int depID = Convert.ToInt32(litSavedAppID.Text); // Fetch the ID of the App row to be revised
                    App toRevise = context.Apps.Find(depID);        // Fetch the App that we want to update
                    if (toRevise == null)                           // If == the target App not found
                        LogError.LogInternalError("EditApproval", $"ApprovalID from Query String '{depID.ToString()}' could not be found in database");                     // Log fatal error

                    //  2) Create a AppHistory row to audit this change.

                    AppHistory hist = new AppHistory();               // Get a place to build a new History row
                    StateActions.CopyPreviousState(toRevise, hist, "Revised"); // Create a History log row from "old" version of Request
//                    hist.ReturnNote = toRevise.ReturnNote;          // Save the Return Note from the Returned Rqst

                    //  3) Change the State of the Rqst from "Returned" to "Unsubmitted," erase the ReturnNote comments and save it.

                    StateActions.SetNewAppState(toRevise, StateActions.FindUnsubmittedAppState(litSavedProjectRole.Text), litSavedUserID.Text, hist); 
                                                                    // Write down our new State and authorship
                    toRevise.ReturnNote = "";                       // Erase the note
                    context.AppHistorys.Add(hist);                  // Save new History row
                    context.SaveChanges();                          // Commit the Add and Modify
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditApproval", "Error processing AppHistory and App rows"); // Fatal error
                }

                //  4) Call this page back and invoke the "Edit" command for an existing Expense.

                Response.Redirect(PortalConstants.URLEditApproval + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                            + PortalConstants.QSRequestID + "=" + Request.QueryString[PortalConstants.QSRequestID] + "&"
                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit); // Start with an existing App
            }
        }

        // Save button clicked. "Save" means that we unload all the controls for the Approval into a database row. 
        // If the Approval is new, we just add a new row. If the Approval already exists, we update it and add a history record to show the edit.
        // We can tell a Approval is new if the Query String doesn't contain a mention of a AppID AND the literal litSavedAppID is empty.
        // On the first such Save, we stash the AppID of the new Approval into this literal so that a second Save during the same session will find
        // it and update it, rather than creating a second row.

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int dummy = SaveApp();                                  // Do the heavy lifting
            if (dummy == 0) return;                                 // If == hit an error. Let user retry

            // Now go back to Dashboard

            Response.Redirect(PortalConstants.URLProjectDashboard + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                                  + PortalConstants.QSStatus + "=Approval Request saved");
        }

        // Submit button clicked. Save what we've been working on, set the Approval state to "Submitted," then go back to the dashboard.

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            // Before saving, make sure that a sufficient number of supporting documents are present. Note that a user can "Save" with
            // insufficent supporting docs, but can only "Submit" if the minimum is present.

            if (lstSupporting.Items.Count < Convert.ToInt32(litSupportingDocMin.Text)) // If < the minimum number of docs is not present
            {
                litDangerMessage.Text = "The Approval Request must include a minimum of " + litSupportingDocMin.Text + " Supporting Document.";
                litSuccessMessage.Text = "";                        // Just the danger message, not a stale success message
                return;                                             // Back for more punishment
            }

            int savedAppID = SaveApp();                             // Do the heavy lifting to save the current Approval
            if (savedAppID == 0)                                    // If == SaveApp encountered an error. Go no further
                LogError.LogInternalError("EditApproval", "Unable to save Approval before Submitting"); // Fatal error

            // SaveApp just saved the Request, which may or may not have written a AppHistory row. But now, let's write another
            // AppHistory row to describe the Submit action.

            string emailSent = "";
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    App toSubmit = context.Apps.Find(savedAppID);       // Find the App that we want to update
                    AppHistory hist = new AppHistory();                 // Get a place to build a new AppHistory row
                    StateActions.CopyPreviousState(toSubmit, hist, "Submitted"); // Fill the AppHistory row from "old" version of Approval

                    AppState nextState = StateActions.FindNextState(toSubmit.CurrentState, toSubmit.AppReviewType); // Figure out who gets it next. Nuanced.
                    StateActions.SetNewAppState(toSubmit, nextState, litSavedUserID.Text, hist); // Move to that state
                    toSubmit.SubmitUserID = litSavedUserID.Text;        // Remember who submitted this App. They get notification on Return.

                    context.AppHistorys.Add(hist);                      // Save new AppHistory row
                    context.SaveChanges();                              // Update the App, create the AppHistory

                    emailSent = EmailActions.SendEmailToReviewer(false, // Send "non-rush" email to next reviewer
                        StateActions.UserRoleToApproveRequest(nextState), // Who is in this role
                        toSubmit.ProjectID,                             // Request is associated with this project
                        toSubmit.Project.Name,                          // Project has this name (for parameter substitution)
                        EnumActions.GetEnumDescription(RequestType.Approval), // This is an Approval Request
                        EnumActions.GetEnumDescription(nextState),      // Here is its next state
                        PortalConstants.CEmailDefaultApprovalApprovedSubject, PortalConstants.CEmailDefaultApprovalApprovedBody); // Use this subject and body, if needed
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditApproval", "Unable to update Approval into Submitted state."); // Fatal error
                }
            }

            // Now go back to Dashboard

            Response.Redirect(PortalConstants.URLProjectDashboard + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                                  + PortalConstants.QSStatus + "=Approval Request submitted." + emailSent);
        }

        // Show History Button clicked. Open up and fill a GridView of all the AppHistory rows for this Approval Request

        protected void btnShowHistory_Click(object sender, EventArgs e)
        {
            NavigationActions.LoadAllAppHistorys(Convert.ToInt32(litSavedAppID.Text), gvEDHistory); // Fill the list from the database
            return;
        }

        // Copy an existing Rqst.
        //  1) Copy the Rqst row into a new row
        //  2) Copy the Supporting Docs
        //  3) Create a AppHistory row to describe the copy

        private int CopyApp(int sourceID)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                App src = context.Apps.Find(sourceID);              // Fetch Rqst row by its key
                if (src == null)                                    // If == source Rqst could not be found. Internal error
                    LogError.LogInternalError("EditApproval", $"Approval ID '{sourceID.ToString()}' from selected GridView row not found in database"); // Fatal error
                try
                {

                    //  1) Copy the Rqst row into a new row

                    App dest = new App()
                    {                                               // Instantiate and fill the destination Rqst
                        Inactive = false,
                        Archived = false,
                        CreatedTime = System.DateTime.Now,          // Rqst is created right now
                        AppType = src.AppType,
                        AppReviewType = src.AppReviewType,
                        Description = src.Description + " (copy)",  // Note that this is a copy
                        Notes = src.Notes,
                        ProjectID = src.ProjectID,                  // New Rqst is in the same project
                        ReturnNote = "",                            // Clear out the return note, if any
                        StaffNote = "",                             // Clear out staff note, if any
                        CurrentTime = System.DateTime.Now,
                        CurrentState = StateActions.FindUnsubmittedAppState(litSavedProjectRole.Text),
                        CurrentUserID = litSavedUserID.Text         // Current user becomes creator of new Rqst
                    };

                    context.Apps.Add(dest);                         // Store the new row
                    context.SaveChanges();                          // Commit the change to product the new Rqst ID

                    //  2) Copy the Supporting Docs. We know that in the source Rqst, all the supporting docs are "permanent," i.e., we don't need
                    //  to worry about "temporary" docs.

                    SupportingActions.CopyDocs(context, RequestType.Approval, src.AppID, dest.AppID); // Copy each of the Supporting Docs

                    //  3) Create a AppHistory row to describe the copy

                    AppHistory hist = new AppHistory();
                    StateActions.CopyPreviousState(src, hist, "Copied"); // Fill fields of new record
                    hist.AppID = dest.AppID; hist.NewAppState = dest.CurrentState; // Add fields from copied App

                    context.AppHistorys.Add(hist);                  // Add the row
                    context.SaveChanges();                          // Commit the change
                    return dest.AppID;                              // Return the ID of the new Approval Rqst
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditApproval", "Error copying Approval, AppHistory and RqstSupporting rows"); // Fatal error
                }
                return 0;
            }
        }

        // Package the work of the Save so that Submit can do it as well.

        private int SaveApp()
        {
            litDangerMessage.Text = ""; litSuccessMessage.Text = ""; // Start with a clean slate of message displays
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    int appID = 0;                                 // Assume no saved ID is available
                    if (litSavedAppID.Text != "") appID = QueryStringActions.ConvertID(litSavedAppID.Text).Int; // If != saved ID is available, use it  
                    if (appID == 0)                                // If == no doubt about it, Save makes a new row
                    {

                        // To make a new App row:
                        //  1) Instantiate a new App record
                        //  2) Fill in the fixed fields, e.g., the Project ID that owns us and various times
                        //  3) unload on-screen fields to the growing record
                        //  4) Save the record to the database producing an AppID
                        //  5) Unload the supporting documents

                        App toSave = new App()
                        {                                           // Get a place to hold everything
                            Inactive = false,
                            Archived = false,
                            ProjectID = QueryStringActions.ConvertID(litSavedProjectID.Text).Int, // Connect Approval to Project
                            CreatedTime = System.DateTime.Now       // Stamp time when Approval was first created as "now"
                        };
                        UnloadPanels(toSave);                       // Move from Panels to record
                        StateActions.SetNewAppState(toSave, EnumActions.ConvertTextToAppState(litSavedStateEnum.Text), litSavedUserID.Text);
                        // Write down our current State and authorship
                        context.Apps.Add(toSave);                   // Save new Rqst row
                        context.SaveChanges();                      // Commit the Add
                        litSavedAppID.Text = toSave.AppID.ToString(); // Show that we've saved it once
                        UnloadSupportingDocs();                     // Save all the supporting documents
                        //TODO what if there's an error here?
                        litSuccessMessage.Text = "Approval Request successfully saved"; // Let user know we're good
                        return toSave.AppID;                        // Return the finished Rqst
                    }
                    else                                            // Update an existing Exp row
                    {

                        // To update an existing App row:
                        //  1) Fetch the existing record using the AppID key
                        //  2) Unload on-screen fields overwriting the existing record
                        //  3) Update the record to the database
                        //  4) Create a new AppHistory row to preserve information about the previous Save
                        //  5) Unload the supporting documents

                        App toUpdate = context.Apps.Find(appID);    // Fetch the Rqst that we want to update
                        UnloadPanels(toUpdate);                     // Move from Panels to Rqst, modifying it in the process

                        AppHistory hist = new AppHistory();         // Get a place to build a new AppHistory row
                        StateActions.CopyPreviousState(toUpdate, hist, "Saved"); // Create a AppHistory log row from "old" version of Approval
                        StateActions.SetNewAppState(toUpdate, hist.PriorAppState, litSavedUserID.Text, hist);
                        // Write down our current State (which doesn't change here) and authorship
                        context.AppHistorys.Add(hist);              // Save new AppHistory row
                        context.SaveChanges();                      // Commit the Add or Modify
                        UnloadSupportingDocs();                     // Save all the new supporting documents
                        litSuccessMessage.Text = "Approval Request successfully updated";    // Let user know we're good
                        return toUpdate.AppID;                      // Return the finished Rqst
                    }
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditApproval", "Error writing App row"); // Fatal error
                }
            }
            return 0;                                               // We never get this far
        }

        // Based on the selected Approval Type, enable and disable the appropriate panels on the display.

        void EnablePanels(AppType type)
        {
            pnlDescription.Visible = true;
            pnlReviewType.Visible = true;
            pnlNotes.Visible = true;
            pnlReturnNote.Visible = false;
            if (litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString()) // If == the user is an IC
                pnlStaffNote.Visible = true;                        // ICs can see the Staff Note
            else
                pnlStaffNote.Visible = false;                       // But no other project role can
            pnlState.Visible = true;
            pnlSupporting.Visible = true;
            switch (type)
            {
                case AppType.Contract:
                case AppType.Grant:
                case AppType.Campaign:
                case AppType.Report:
                    {
                        rdoReviewType.Items[App.AppTypeICOnly].Enabled = false; rdoReviewType.Items[App.AppTypeICOnly].Selected = false; // COI
                        rdoReviewType.Items[App.AppTypeExpress].Enabled = true; rdoReviewType.Items[App.AppTypeExpress].Selected = false; // Express
                        rdoReviewType.Items[App.AppTypeFull].Enabled = true; rdoReviewType.Items[App.AppTypeFull].Selected = true; // Full
                        litSupportingDocMin.Text = "1";
                        break;
                    }
                case AppType.Certificate:
                    {
                        rdoReviewType.Items[App.AppTypeICOnly].Enabled = true; rdoReviewType.Items[App.AppTypeICOnly].Selected = true; // COI
                        rdoReviewType.Items[App.AppTypeExpress].Enabled = false; rdoReviewType.Items[App.AppTypeExpress].Selected = false; // Express
                        rdoReviewType.Items[App.AppTypeFull].Enabled = false; rdoReviewType.Items[App.AppTypeFull].Selected = false; // Full
                        litSupportingDocMin.Text = "0";
                        break;
                    }
                default:
                    {
                        LogError.LogInternalError("EditApproval", $"Invalid AppType value '{type.ToString()}' in database"); // Fatal error
                        break;
                    }
            }
            return;
        }

        // Fill in the "big" dropdown lists for a Approval. (None yet) Fill Entity and Person even though they might not be visible yet.

        void FillDropDownLists()
        {
            return;
        }

        // Move values from the Approval record into the Page. Only move the values that are "Visible" for this Approval Type

        void LoadPanels(App record)
        {
            litSavedStateEnum.Text = record.CurrentState.ToString();  // Stash enum version of state
            txtState.Text = EnumActions.GetEnumDescription(record.CurrentState); // Display "English" version of state

            txtDescription.Text = record.Description;

            ExtensionActions.LoadEnumIntoRdo(record.AppReviewType, rdoReviewType); // Load enum value into Radio Button List

            txtNotes.Text = record.Notes;

            if (record.CurrentState == AppState.Returned)               // If == the Rqst has been returned, so a Return Note may be present
            {
                pnlReturnNote.Visible = true;                           // Make this panel visible
                txtReturnNote.Text = record.ReturnNote;                 // Copy the text of the Return Note
            }
            txtStaffNote.Text = record.StaffNote;

            // Supporting Documents handled elsewhere

            return;
        }

        // Make all controls readonly to prevent editing during View function

        void ReadOnlyControls()
        {
            txtDescription.Enabled = false;
            // Approval Type - already set
            // Approval State - already set
            rdoReviewType.Enabled = false;
            txtNotes.Enabled = false;
            txtStaffNote.Enabled = false;
            // Suporting Docs - leave Listbox filled and enabled so that double click still works
            if (pnlSupporting.Visible)
                fupUpload.Enabled = false;   // But no new items
            return;
        }

        // Move the "as edited" values from the Page into a App record. Only move the values that are "Visible" for this Approval Type

        void UnloadPanels(App record)
        {
            record.AppType = EnumActions.ConvertTextToAppType(rdoAppType.SelectedValue); // Approval Type - convert from string VALUE to enumerated RqstType

            record.AppReviewType = EnumActions.ConvertTextToAppReviewType(rdoReviewType.SelectedValue); // Convert selection to bool

            record.Description = txtDescription.Text;

            if (pnlNotes.Visible)
                record.Notes = txtNotes.Text;

            if (pnlStaffNote.Visible)
                record.StaffNote = txtStaffNote.Text;

            // Return Note is read-only, so not unloaded

            return;
        }

        // Load the Supporting Listbox control from the database.

        void LoadSupportingDocs(App rqst)
        {
            if (pnlSupporting.Visible)                              // If true the ListBox is visible so we need to fill it
            {
                SupportingActions.LoadDocs(RequestType.Approval, rqst.AppID, lstSupporting, litDangerMessage); // Do the heavy lifting
            }
            return;
        }

        // Unload files from Listbox control. That is, move a supporting file from temporary to permanent status.

        void UnloadSupportingDocs()
        {
            if (pnlSupporting.Visible)                              // If true, panel containing ListBox control is visible. Work to do
            {
                SupportingActions.UnloadDocs(lstSupporting, litSavedUserID.Text, RequestType.Approval, QueryStringActions.ConvertID(litSavedAppID.Text).Int, litDangerMessage);
            }
            return;
        }

        protected void rdoReviewType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
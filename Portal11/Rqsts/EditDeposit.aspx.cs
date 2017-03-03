using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Rqsts
{
    public partial class EditDeposit : System.Web.UI.Page
    {
        // Create a new Deposit Request, Copy (and Edit) an existing Deposit Request, or Edit an existing Deposit Request. 
        // Communication from Project Dashboard is through Query Strings:
        //      UserID - the database ID of the Project Director making these changes (Required)
        //      ProjectID - the database ID of the Project that owns this Deposit (Required)
        //      DepID - the database ID of the existing Deposit, if any, to process
        //      Command - "New," "Copy," "Edit," "View," or "Revise" (Required)

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // Fetch and validate the four Query String parameters. Carefully convert the integer parameters to integers.

                string userID = QueryStringActions.GetUserID();         // First parameter - User ID. If absent, check UserInfoCookie
                QSValue projectID = QueryStringActions.GetProjectID();  // Second parameter - Project ID. If absent, check ProjectInfoCookie
                QSValue depID = QueryStringActions.GetRequestID();      // Third parameter - Deposit ID. If absent, must be a New command
                string cmd = QueryStringActions.GetCommand();           // Fourth parameter - Command. Must be present

                // Find the User's role on the Project

                HttpCookie projectInfoCookie = Request.Cookies[PortalConstants.CProjectInfo]; // Find the Project Info cookie
                string projRole = projectInfoCookie[PortalConstants.CProjectRole]; // Fetch user's Project Role from cookie
                if (projRole == "")                                     // If == that's blank. We have an error
                    LogError.LogQueryStringError("EditDeposit", "Unable to find Project Role in Project Info Cookie. User does not have a project"); // Fatal error

                // Stash these parameters into invisible literals on the current page.

                litSavedUserID.Text = userID;
                litSavedProjectID.Text = projectID.String;
                litSavedDepID.Text = "";                                // Assume New or Copy - do not modify an existing row, but add a new one
                litSavedCommand.Text = cmd;
                litSavedProjectRole.Text = projRole;                    // Save in a faster spot for later

                SupportingActions.CleanupTemp(userID, litDangerMessage); // Cleanup supporting docs from previous executions for this user

                // Dispatch by Command

                switch (cmd)
                {
                    case PortalConstants.QSCommandCopy:                 // Process a "Copy" command. Read existing request, save it in new row
                        {
                            depID.Int = CopyDep(depID.Int);             // Copy the source Dep into a new destination Dep
                            depID.String = depID.Int.ToString();        // Also update the string version of the Dep ID
                            if (depID.Int == 0)                         // If == the copy process failed
                                LogError.LogInternalError("EditDeposit", "Unable to copy existing Deposit Request"); // Log fatal error

                            goto case PortalConstants.QSCommandEdit;    // Now edit the destination Exp
                        }
                    case PortalConstants.QSCommandEdit:                 // Process an "Edit" command. Read existing Deposit, save it in same row
                        {

                            // Fetch the row from the database. Set the radio button corresponding to our Deposit type,
                            // then disable the radio button group so the user can't change the Deposit type.
                            // Make appropriate panels visible, then fill in the panels using data rom the existing Deposit. Lotta work!

                            if (depID.Int == 0)                         // If == cannot edit a missing Exp
                                LogError.LogQueryStringError("EditDeposit", "Missing Query String 'RequestID'"); // Log fatal error

                            litSavedDepID.Text = depID.String;          // Remember to write record back to its original spot, i.e., modify the row
                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                Dep dep = context.Deps.Find(depID.Int); // Fetch Dep row by its key
                                if (dep == null)
                                    LogError.LogInternalError("EditDeposit", string.Format("Unable to locate DepositID '{0}' in database", depID.String));
                                // Log fatal error

                                // Note: The rdo displays TEXT that matches the Description of each DepType, but the VALUE contains the enumerated DepType
                                rdoDepType.SelectedValue = dep.DepType.ToString(); // Select the button corresponding to our type
                                rdoDepType.Enabled = false;             // Disable the control - cannot change type of existing Deposit

                                EnablePanels(dep.DepType);              // Make the relevant panels visible
                                LoadPanels(dep);                        // Fill in the visible panels from the Deposit
                                LoadSupportingDocs(dep);                // Fill in the Supporting Docs - it takes extra work
                            }
                            litSuccessMessage.Text = "Deposit Request is ready to edit";
                            break;
                        }
                    case PortalConstants.QSCommandNew:              // Process a "New" command. Create new, empty Deposit, save it in new row
                        {

                            // We don't have to "fill" any of the controls on the page, just use them in their initial state.
                            // The radio button group for Deposit type wakes up "enabled" with nothing selected. No other controls are visible.
                            // When the user selects a Deposit type, we'll use that click to make the appropriate panels visible.

                            DepState newState = StateActions.FindUnsubmittedDepState(litSavedProjectRole.Text); // New state depends on User's project role
                            litSavedStateEnum.Text = newState.ToString(); // Stash actual enum value of DepState
                            txtState.Text = EnumActions.GetEnumDescription(newState); // Display "English" version of enum
                            litSuccessMessage.Text = "New Deposit Request is ready to edit";
                            break;
                        }
                    case PortalConstants.QSCommandView:             // Process an "View" command. Read existing Deposit, disable edit and save
                        {
                            litSavedDepID.Text = depID.String;      // Remember the ID of the row that we will view

                            // Fetch the row from the database. Set the radio button corresponding to our Deposit type,
                            // then disable the radio button group so the user can't change the Deposit type.
                            // Make appropriate panels visible, then fill in the panels using data from the existing Deposit. 
                            // Set everything to readonly to prevent editing. Disable Save and Submit. Lotta work!

                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                Dep dep = context.Deps.Find(depID.Int); // Fetch Dep row by its primary key
                                if (dep == null)
                                    LogError.LogInternalError("EditDeposit", string.Format("Unable to find Deposit ID '{0}' in database",
                                        depID.String));         // Fatal error

                                rdoDepType.SelectedValue = dep.DepType.ToString(); // Select the button corresponding to our type
                                rdoDepType.Enabled = false;         // Disable the control - cannot change type of existing Deposit
                                EnablePanels(dep.DepType);          // Make the relevant panels visible
                                LoadPanels(dep);                    // Fill in the visible panels from the Deposit
                                LoadSupportingDocs(dep);            // Fill in the Supporting Docs - it takes extra work
                                ReadOnlyControls();                 // Make all controls readonly for viewing

                                // Adjust the button settings for the View function

                                btnCancel.Text = "Done";            // Provide a friendly way out of this page
                                btnSave.Enabled = false;            // No "Save" for you!
                                btnSubmit.Enabled = false;

                                // OK, who can revise a returned Deposit Request? 
                                // Let's say that it's only the Coordinator, since they're the only one who could create it.

                                if ((dep.CurrentState == DepState.Returned) &&
                                    (litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString())) // If true it's a Coordinator and a Returned Request
                                    btnRevise.Visible = true;       // Turn on the "Revise" button - specific to this case
                                btnShowHistory.Visible = true;      // We can show the history so make button visible 
                                pnlAdd.Visible = false;             // Can't figure out how to disable Add button, so make it invisible
                                btnRem.Visible = false;             // Remove button on Supporting Docs is also invisible
                                Page.Title = "View Deposit Request";    // Note that we are viewing, not editing
                                litSuccessMessage.Text = "Existing Deposit Request is ready to view";
                            }
                            break;
                        }
                    default:
                        LogError.LogQueryStringError("EditDeposit", "Invalid Query String 'Command'"); // Log fatal error
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

        protected void txtAmount_TextChanged(object sender, EventArgs e)
        {
            ExtensionActions.ReloadDecimalText((TextBox)sender);    // Pretty up the text just entered
        }

        // User pressed a radio button to select a Deposit type. Each Deposit type uses a different combination of panels.
        // Make visible the appropriate panels and fill the appropriate dropdown lists. Then do some housekeeping to get the
        // appropriate defaults applied. There ought to be a better way to do this, but I can't find it. By the way, this
        // control is only enabled when the record is unsaved. So the buttons work for New but not for Edit.

        protected void rdoDepType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DepType selected = EnumActions.ConvertTextToDepType(rdoDepType.SelectedValue); // Fetch selected button, convert value to enum
            EnablePanels(selected);                                     // Turn on/off controls based on selected Exp Type
            FillDropDownLists(selected);                                // Load dropdown lists with unselected database values
            return;
        }

        protected void txtDateOfDeposit_TextChanged(object sender, EventArgs e)
        {
            DateActions.ValidateDateInput(txtDateOfDeposit, lblDateOfDeposit.Text, litDangerMessage); // Do the heavy lifting
            return;
        }

        // Toggle the visibility of a calendar. If becoming visible, load with date from text box, if any

        protected void btnDateOfDeposit_Click(object sender, EventArgs e)
        {
            calClick(txtDateOfDeposit, calDateOfDeposit);                   // Do the heavy lifting in common code
            return;
        }

        void calClick(TextBox txt, System.Web.UI.WebControls.Calendar cal)
        {
            if (cal.Visible)                                                    // If true the calendar control is currently visible
                cal.Visible = false;                                            // Hide it
            else
            {
                cal.Visible = true;                                             // Make the calendar control visible
                DateActions.LoadTxtIntoCal(txt, cal);                           // Pull date from text box, store it in calendar
            }
            return;
        }

        protected void calDateOfDeposit_SelectionChanged(object sender, EventArgs e)
        {
            DateTime start = calDateOfDeposit.SelectedDate;
            txtDateOfDeposit.Text = DateActions.LoadDateIntoTxt(start);             // Convert date to text 
            DateTime last = start;
            calDateOfDeposit.Visible = false;                                       // One click and the calendar is gone
            return;
        }

        // One of the radio buttons in the "Source of Funds" panel has clicked. Switch the Person drop down list on or off.

        protected void rdoSourceOfFunds_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (rdoSourceOfFunds.SelectedValue)
            {
                case PortalConstants.RDONotApplicable:
                    {
                        pnlEntity.Visible = false; pnlPerson.Visible = false; // No Individual or Entity to be selected
                        break;
                    }
                case PortalConstants.RDOEntity:
                    {
                        pnlEntity.Visible = true; pnlPerson.Visible = false; // The Entity DDL is already populated. It may even have a row selected.
                        //TODO More work here
                        break;
                    }
                case PortalConstants.RDOIndividual:
                    {
                        pnlEntity.Visible = false; pnlPerson.Visible = true; // The Person DDL is already populated. It may even have a row selected.
                        break;
                    }
                default:
                    {
                        LogError.LogInternalError("EditDeposit", string.Format("Invalid SourceOfFunds value '{0}' returned by RDO",
                            rdoSourceOfFunds.SelectedValue)); // Fatal error
                        break;
                    }
            }
            return;
        }

        // The Anonymous checkbox has been checked or cleared

        protected void cblEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cblEntity.Items.FindByValue(Dep.EntityAnonymous).Selected)  // If true checkbox is checked
                pnlEntityDDL.Visible = false;                       // Anonymous means no drop down list
            else
                pnlEntityDDL.Visible = true;                        // Not anonymous means we need to see the list
        }

        protected void cblPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cblPerson.Items.FindByValue(Dep.PersonAnonymous).Selected)  // If true checkbox is checked
                pnlPersonDDL.Visible = false;                       // Anonymous means no drop down list
            else
                pnlPersonDDL.Visible = true;                        // Not anonymous means we need to see the list
        }

        // One of the radio buttons in the "Destination of Funds" panel has clicked. Switch the Project Class drop down list on or off.

        protected void rdoDestOfFunds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rdoDestOfFunds.SelectedValue == PortalConstants.RDOFundsRestricted) // If == Restricted button was clicked. Turn on drop down list
                pnlProjectClass.Visible = true;                     // Turn on drop down list
            else
                pnlProjectClass.Visible = false;                    // Turn off drop down list
            return;
        }

        // User has clicked on a row of the Supporting Docs list. Just turn on the buttons that work now.
        // And don't ask me why, but unless we act, the page title reverts to its original value. So for the case of View, set it again.

        protected void lstSupporting_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnView.Enabled = true;
            if (litSavedCommand.Text == PortalConstants.QSCommandView) // If == it is a View command
                Page.Title = "View Deposit Request";                // Reset the page title. Don't know why it changed, but just reset it
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

        protected void btnView_Click(object sender, EventArgs e)
        {
            SupportingActions.ViewDoc(lstSupporting, litDangerMessage);
            return;
        }
        protected void EDHistoryView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                EDHistoryView.PageIndex = e.NewPageIndex;             // Propagate the desired page index
                NavigationActions.LoadAllDepHistorys(Convert.ToInt32(litSavedDepID.Text), EDHistoryView); // Fill the list from the database
                EDHistoryView.SelectedIndex = -1;                     // No row currently selected
            }
            return;
        }

        // Cancel button has been clicked. Just return to the Dashboard.

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLProjectDashboard);
            return;
        }

        // Revise button clicked. This is the case where a Dep is in the "Returned" state - it was submitted, but failed during the review process.
        // To "Revise," we: 
        //  1) Get a copy of the Request
        //  2) Create a History row to audit this change.
        //  3) Change the State of the Request from "Returned" to "Under Construction," erase the ReturnNote comments and save it.
        //  4) Call this page back and invoke the "Edit" command for an existing Deposit.

        protected void btnRevise_Click(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  1) Get a copy of the Dep

                    int depID = Convert.ToInt32(litSavedDepID.Text); // Fetch the ID of the Dep row to be revised
                    Dep toRevise = context.Deps.Find(depID);        // Fetch the Dep that we want to update
                    if (toRevise == null)                           // If == the target Dep not found
                        LogError.LogInternalError("EditDeposit", string.Format("DepositID from Query String '{0}' could not be found in database",
                            depID.ToString()));                     // Log fatal error

                    //  2) Create a DepHistory row to audit this change.

                    DepHistory hist = new DepHistory();               // Get a place to build a new History row
                    StateActions.CopyPreviousState(toRevise, hist, "Revised"); // Create a History log row from "old" version of Request
                                                                               //                    hist.ReturnNote = toRevise.ReturnNote;          // Save the Note from the Returned Rqst

                    //  3) Change the State of the Rqst from "Returned" to "Unsubmitted," erase the ReturnNote comments and save it.

                    StateActions.SetNewDepState(toRevise, StateActions.FindUnsubmittedDepState(litSavedProjectRole.Text), litSavedUserID.Text, hist); // Write down our new State and authorship
                    toRevise.ReturnNote = "";                       // Erase the note
                    toRevise.EntityNeeded = false; toRevise.PersonNeeded = false; // Assume "Returner" did as we asked
                    context.DepHistorys.Add(hist);                  // Save new History row
                    context.SaveChanges();                          // Commit the Add and Modify
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditDeposit", "Error processing DepHistory and Dep rows"); // Fatal error
                }

                //  4) Call this page back and invoke the "Edit" command for an existing Expense.

                Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                            + PortalConstants.QSRequestID + "=" + Request.QueryString[PortalConstants.QSRequestID] + "&"
                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit); // Start with an existing Dep
            }
        }

        // Save button clicked. "Save" means that we unload all the controls for the Deposit into a database row. 
        // If the Deposit is new, we just add a new row. If the Deposit already exists, we update it and add a history record to show the edit.
        // We can tell a Deposit is new if the Query String doesn't contain a mention of a DepID AND the literal litSavedDepID is empty.
        // On the first such Save, we stash the DepID of the new Deposit into this literal so that a second Save during the same session will find
        // it and update it, rather than creating a second row.

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int dummy = SaveDep();                                  // Do the heavy lifting
            if (dummy == 0) return;                                 // If == hit an error. Let user retry

            // Now go back to Dashboard

            Response.Redirect(PortalConstants.URLProjectDashboard + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                                  + PortalConstants.QSStatus + "=Deposit Request saved");
        }

        // Submit button clicked. Save what we've been working on, set the Deposit state to "Submitted," then go back to the dashboard.

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            // Before saving, make sure that a sufficient number of supporting documents are present. Note that a user can "Save" with
            // insufficent supporting docs, but can only "Submit" if the minimum is present.

            if (lstSupporting.Items.Count < Convert.ToInt32(litSupportingDocMin.Text)) // If < the minimum number of docs is not present
            {
                litDangerMessage.Text = "The Deposit Request must include a minimum of " + litSupportingDocMin.Text + " Supporting Document.";
                litSuccessMessage.Text = "";                        // Just the danger message, not a stale success message
                return;                                             // Back for more punishment
            }

            int savedDepID = SaveDep();                             // Do the heavy lifting to save the current Deposit
            if (savedDepID == 0)                                    // If == SaveDep encountered an error. Go no further
                LogError.LogInternalError("EditDeposit", "Unable to save Deposit before Submitting"); // Fatal error

            // SaveDep just saved the Request, which may or may not have written a DepHistory row. But now, let's write another
            // DepHistory row to describe the Submit action.

            string emailSent = "";
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    Dep toSubmit = context.Deps.Find(savedDepID);   // Find the Dep that we want to update
                    DepHistory hist = new DepHistory();             // Get a place to build a new DepHistory row
                    StateActions.CopyPreviousState(toSubmit, hist, "Submitted"); // Fill the DepHistory row from "old" version of Deposit

                    DepState nextState = StateActions.FindNextState(toSubmit.CurrentState); // Figure out who gets it next. Nuanced.
                    StateActions.SetNewDepState(toSubmit, nextState, litSavedUserID.Text, hist); // Move to that state
                    toSubmit.SubmitUserID = litSavedUserID.Text;    // Remember who submitted this Dep. They get notification on Return.

                    context.DepHistorys.Add(hist);                  // Save new DepHistory row
                    context.SaveChanges();                          // Update the Dep, create the DepHistory

                    emailSent = EmailActions.SendEmailToReviewer(false, // Send "non-rush" email to next reviewer
                        StateActions.UserRoleToApproveRequest(nextState), // Who is in this role
                        toSubmit.ProjectID,                         // Request is associated with this project
                        toSubmit.Project.Name,                      // Project has this name (for parameter substitution)
                        EnumActions.GetEnumDescription(RequestType.Deposit), // This is a Deposit Request
                        EnumActions.GetEnumDescription(nextState),      // Here is its next state
                        PortalConstants.CEmailDefaultDepositApprovedSubject, PortalConstants.CEmailDefaultDepositApprovedBody); // Use this subject and body, if needed
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditDeposit", "Unable to update Deposit into Submitted state"); // Fatal error
                }
            }

            // Now go back to Dashboard

            Response.Redirect(PortalConstants.URLProjectDashboard + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                                  + PortalConstants.QSStatus + "=Deposit Request submitted." + emailSent);
        }

        // Show History Button clicked. Open up and fill a GridView of all the DepHistory rows for this Deposit Request

        protected void btnShowHistory_Click(object sender, EventArgs e)
        {
            NavigationActions.LoadAllDepHistorys(Convert.ToInt32(litSavedDepID.Text), EDHistoryView); // Fill the list from the database
            return;
        }

        // Copy an existing Rqst.
        //  1) Copy the Rqst row into a new row
        //  2) Copy the Supporting Docs
        //  3) Create a DepHistory row to describe the copy

        private int CopyDep(int sourceID)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                Dep src = context.Deps.Find(sourceID);              // Fetch Rqst row by its key
                if (src == null)                                    // If == source Rqst could not be found. Internal error
                    LogError.LogInternalError("EditDeposit", string.Format("Deposit ID '{0}' from selected GridView row not found in database",
                        sourceID.ToString()));                      // Fatal error
                try
                {

                    //  1) Copy the Rqst row into a new row

                    Dep dest = new Dep()
                    {                                               // Instantiate and fill the destination Rqst
                        Inactive = false,
                        Archived = false,
                        Amount = src.Amount,
                        CreatedTime = System.DateTime.Now,          // Rqst is created right now
                        DateOfDeposit = src.DateOfDeposit,
                        DepType = src.DepType,
                        Description = src.Description + " (copy)",  // Note that this is a copy
                        DestOfFunds = src.DestOfFunds,
                        ProjectClassID = src.ProjectClassID,
                        EntityIsAnonymous = src.EntityIsAnonymous,
                        EntityNeeded = src.EntityNeeded,
                        EntityRole = src.EntityRole,
                        EntityID = src.EntityID,
                        GLCodeID = src.GLCodeID,
                        Notes = src.Notes,
                        PersonIsAnonymous = src.PersonIsAnonymous,
                        PersonNeeded = src.PersonNeeded,
                        PersonRole = src.PersonRole,
                        PersonID = src.PersonID,
                        PledgePayment = src.PledgePayment,
                        ProjectID = src.ProjectID,                  // New Rqst is in the same project
                        ReturnNote = "",                            // Clear out the return note, if any
                        StaffNote = "",                             // Clear out the staff note, if any
                        SourceOfFunds = src.SourceOfFunds,
                        CurrentTime = System.DateTime.Now,
                        CurrentState = StateActions.FindUnsubmittedDepState(litSavedProjectRole.Text),
                        CurrentUserID = litSavedUserID.Text         // Current user becomes creator of new Rqst
                    };

                    context.Deps.Add(dest);                         // Store the new row
                    context.SaveChanges();                          // Commit the change to product the new Rqst ID

                    //  2) Copy the Supporting Docs. We know that in the source Rqst, all the supporting docs are "permanent," i.e., we don't need
                    //  to worry about "temporary" docs.

                    SupportingActions.CopyDocs(context, RequestType.Deposit, src.DepID, dest.DepID); // Copy each of the Supporting Docs

                    //  3) Create a DepHistory row to describe the copy

                    DepHistory hist = new DepHistory();
                    StateActions.CopyPreviousState(src, hist, "Copied"); // Fill fields of new record
                    hist.DepID = dest.DepID; hist.NewDepState = dest.CurrentState; // Add fields from copied Dep

                    context.DepHistorys.Add(hist);                  // Add the row
                    context.SaveChanges();                          // Commit the change
                    return dest.DepID;                              // Return the ID of the new Deposit Rqst
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditDeposit", "Error copying Deposit, DepHistory and RqstSupporting rows"); // Fatal error
                }
                return 0;
            }
        }

        // Package the work of the Save so that Submit can do it as well.

        private int SaveDep()
        {
            litDangerMessage.Text = ""; litSuccessMessage.Text = ""; // Start with a clean slate of message displays
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    int depID = 0;                                 // Assume no saved ID is available
                    if (litSavedDepID.Text != "") depID = QueryStringActions.ConvertID(litSavedDepID.Text).Int; // If != saved ID is available, use it  
                    if (depID == 0)                                // If == no doubt about it, Save makes a new row
                    {

                        // To make a new Dep row:
                        //  1) Instantiate a new Dep record
                        //  2) Fill in the fixed fields, e.g., the Project ID that owns us and various times
                        //  3) unload on-screen fields to the growing record
                        //  4) Save the record to the database producing an DepID
                        //  5) Unload the supporting documents

                        Dep toSave = new Dep()
                        {                                           // Get a place to hold everything
                            Inactive = false,
                            Archived = false,
                            ProjectID = QueryStringActions.ConvertID(litSavedProjectID.Text).Int, // Connect Deposit to Project
                            CreatedTime = System.DateTime.Now       // Stamp time when Deposit was first created as "now"
                        };
                        UnloadPanels(toSave);                       // Move from Panels to record
                        StateActions.SetNewDepState(toSave, EnumActions.ConvertTextToDepState(litSavedStateEnum.Text), litSavedUserID.Text);
                        // Write down our current State and authorship
                        context.Deps.Add(toSave);                   // Save new Rqst row
                        context.SaveChanges();                      // Commit the Add
                        litSavedDepID.Text = toSave.DepID.ToString(); // Show that we've saved it once
                        UnloadSupportingDocs();                     // Save all the supporting documents
                        //TODO what if there's an error here?
                        litSuccessMessage.Text = "Deposit Request successfully saved"; // Let user know we're good
                        return toSave.DepID;                        // Return the finished Rqst
                    }
                    else                                            // Update an existing Exp row
                    {

                        // To update an existing Dep row:
                        //  1) Fetch the existing record using the DepID key
                        //  2) Unload on-screen fields overwriting the existing record
                        //  3) Update the record to the database
                        //  4) Create a new DepHistory row to preserve information about the previous Save
                        //  5) Unload the supporting documents

                        Dep toUpdate = context.Deps.Find(depID);    // Fetch the Rqst that we want to update
                        UnloadPanels(toUpdate);                     // Move from Panels to Rqst, modifying it in the process

                        DepHistory hist = new DepHistory();         // Get a place to build a new DepHistory row
                        StateActions.CopyPreviousState(toUpdate, hist, "Saved"); // Create a DepHistory log row from "old" version of Deposit
                        StateActions.SetNewDepState(toUpdate, hist.PriorDepState, litSavedUserID.Text, hist);
                        // Write down our current State (which doesn't change here) and authorship
                        context.DepHistorys.Add(hist);              // Save new DepHistory row
                        context.SaveChanges();                      // Commit the Add or Modify
                        UnloadSupportingDocs();                     // Save all the new supporting documents
                        litSuccessMessage.Text = "Deposit Request successfully updated";    // Let user know we're good
                        return toUpdate.DepID;                      // Return the finished Rqst
                    }
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditDeposit", "Error writing Dep row"); // Fatal error
                }
            }
            return 0;                                               // We never get this far
        }

        // Based on the selected Deposit Type, enable and disable the appropriate panels on the display.

        void EnablePanels(DepType type)
        {
            pnlAmount.Visible = true;                                // Unconditionally visible (or invisible) for all Deposit types
            pnlDateOfDeposit.Visible = true;
            pnlDescription.Visible = true;
            pnlDestOfFunds.Visible = true;
            pnlGLCode.Visible = true;
            pnlNotes.Visible = true;
            pnlReturnNote.Visible = false;
            litSavedEntityEnum.Text = EntityRole.DepositEntity.ToString(); // Set preferred entity role for Deposit use
            pnlSourceOfFunds.Visible = true; litSavedPersonEnum.Text = PersonRole.Donor.ToString(); // Only type of Person is Donor (so far ;-)
            if (litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString()) // If == user is an IC
                pnlStaffNote.Visible = true;                        // The IC can see the staff note
            else
                pnlStaffNote.Visible = false;                       // But no one else can
            pnlState.Visible = true;
            pnlSupporting.Visible = true;
            switch (type)
            {
                case DepType.Cash:
                    {
                        lblAmount.Text = "Dollar Amount";
                        lblDateOfDeposit.Text = "Date Received"; txtDateOfDeposit.ToolTip = "Please indicate the date that the cash was received.";
                        pnlOptions.Visible = true;
                        litSupportingDocMin.Text = "1";
                        break;
                    }
                case DepType.Check:
                    {
                        lblAmount.Text = "Dollar Amount";
                        lblDateOfDeposit.Text = "Date on Check"; txtDateOfDeposit.ToolTip = "Please indicate the date on the check.";
                        pnlOptions.Visible = true;
                        litSupportingDocMin.Text = "1";
                        break;
                    }
                case DepType.EFT:
                    {
                        lblAmount.Text = "Dollar Amount";
                        lblDateOfDeposit.Text = "Date of Draft"; txtDateOfDeposit.ToolTip = "Please indicate the date that the transfer was received.";
                        pnlOptions.Visible = true;
                        litSupportingDocMin.Text = "1";
                        break;
                    }
                case DepType.InKind:
                    {
                        lblAmount.Text = "Fair Market Value";
                        lblDateOfDeposit.Text = "Date Goods/Services Received"; txtDateOfDeposit.ToolTip = "Please indicate the date that the goods or services were received.";
                        pnlOptions.Visible = false;
                        litSupportingDocMin.Text = "1";
                        break;
                    }
                case DepType.Pledge:
                    {
                        lblAmount.Text = "Dollar Amount";
                        lblDateOfDeposit.Text = "Date on Pledge/Contract"; txtDateOfDeposit.ToolTip = "Please indicate the date that the goods or services were received.";
                        pnlOptions.Visible = false;
                        litSupportingDocMin.Text = "1";
                        break;
                    }
                default:
                    {
                        LogError.LogInternalError("EditDeposit", string.Format("Invalid DepType value '{0}' in database",
                            type.ToString())); // Fatal error
                        break;
                    }
            }
            if (UseOnlyDepositGLCodes(type))                        // If true only use Deposit GL Codes
                labGLCode.Text = "Income Account";
            else                                                    // Otherwise use all GL Codes
                labGLCode.Text = "Income or Expense Account";
            return;
        }

        // Fill in the "big" dropdown lists for a Deposit. Fill Entity and Person even though they might not be visible yet.

        void FillDropDownLists(DepType type)
        {
            FillEntityDDL(null);
            FillGLCodeDDL(null, UseOnlyDepositGLCodes(type));       // Load only Deposit GL codes or all GL codes based on Deposit Type
            FillPersonDDL(null);
            FillProjectClassDDL(null);
            txtDateOfDeposit.Text = DateTime.Now.ToString("MM/dd/yyyy"); // Supply today's date as default
            return;
        }

        // Fill the drop down list of Entitys from the database.

        void FillEntityDDL(int? entityID, bool needed = false)
        {
            int projID = QueryStringActions.ConvertID(litSavedProjectID.Text).Int; // Find ID of current project
            EntityRole selectedRole = EnumActions.ConvertTextToEntityRole(litSavedEntityEnum.Text); // Role for this Request Type

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var query = from pe in context.ProjectEntitys
                            where pe.ProjectID == projID && (pe.EntityRole == selectedRole) // This project, our Entity Role
                            orderby pe.Entity.Name
                            select new { EntityID = pe.EntityID, pe.Entity.Name }; // Find all Entitys that are assigned to this project

                DataTable rows = new DataTable();
                rows.Columns.Add(PortalConstants.DdlID);
                rows.Columns.Add(PortalConstants.DdlName);

                foreach (var row in query)
                {
                    DataRow dr = rows.NewRow();                 // Build a row from the next query output
                    dr[PortalConstants.DdlID] = row.EntityID;
                    dr[PortalConstants.DdlName] = row.Name;
                    rows.Rows.Add(dr);                          // Add the new row to the data table
                }

                StateActions.LoadDdl(ddlEntity, entityID, rows,
                    "", "-- none selected --",
                    needed, "-- Please add new " + lblEntity.Text + " --"); // Put the cherry on top

            }
            return;
        }

        // If the GLCode panel is enabled, fill the drop down list from the database

        void FillGLCodeDDL(int? glcodeID, bool useOnlyDepositCodes)
        {
            if (pnlGLCode.Visible)                                  // If true need to populate list
            {
//                if (ddlGLCode.Items.Count == 0)                     // If = the control is empty. Fill it
//                {
                    using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                    {
                        string franchiseKey = SupportingActions.GetFranchiseKey(); // Find current key value
                        var query = from gl in context.GLCodes
                                    where !gl.Inactive && gl.FranchiseKey == franchiseKey
                                    orderby gl.Code
                                    select new { gl.GLCodeID, gl.Code, gl.DepCode }; // Find all codes that are active - Dep and Exp

                        DataTable rows = new DataTable();            // Create an empty DataTable to hold rows returned by Query
                        rows.Columns.Add(PortalConstants.DdlID);
                        rows.Columns.Add(PortalConstants.DdlName);

                        foreach (var row in query)
                        {
                            if (useOnlyDepositCodes && (!row.DepCode)) // If true we're only using Deposit codes and this isn't one of them
                                continue;                            // Skip just this row of the query 
                            DataRow dr = rows.NewRow();              // Build a row from the next query output
                            dr[PortalConstants.DdlID] = row.GLCodeID;
                            dr[PortalConstants.DdlName] = row.Code;
                            rows.Rows.Add(dr);                       // Add the new row to the data table
                        }

                        StateActions.LoadDdl(ddlGLCode, glcodeID, rows, "-- Error: No GL Codes in Database --",
                            "-- none selected --"); // Put the cherry on top

                    }
//                }
            }
            return;
        }

        // Fill the drop down list of Persons from the database. Note that these values are drawn from
        // the ProjectPerson table, which shows Persons assigned to this project. Figure out the person role that corresponds to the request type.
        // Unlike Expense, Deposit has only one role for a Person. But we'll keep the mechanics that we copied from Expense in case that changes. 

        void FillPersonDDL(int? personID, bool needed = false)
        {
            int projID = QueryStringActions.ConvertID(litSavedProjectID.Text).Int; // Find ID of current project
            PersonRole selectedRole = EnumActions.ConvertTextToPersonRole(litSavedPersonEnum.Text); // Role for this Request Type

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var query = from pe in context.ProjectPersons
                            where pe.ProjectID == projID && pe.PersonRole == selectedRole // This project, this role
                            orderby pe.Person.Name
                            select new { PersonID = pe.PersonID, pe.Person.Name }; // Find all employees that are assigned to this project

                DataTable rows = new DataTable();
                rows.Columns.Add(PortalConstants.DdlID);
                rows.Columns.Add(PortalConstants.DdlName);

                foreach (var row in query)
                {
                    DataRow dr = rows.NewRow();                 // Build a row from the next query output
                    dr[PortalConstants.DdlID] = row.PersonID;
                    dr[PortalConstants.DdlName] = row.Name;
                    rows.Rows.Add(dr);                          // Add the new row to the data table
                }

                StateActions.LoadDdl(ddlPerson, personID, rows,
                    "", "-- none selected --",
                    needed, "-- Please add new " + lblPerson.Text + " --"); // Put the cherry on top

            }
            return;
        }

        // If the Funds panel is enabled, fill the drop down list from the database. Note that these values are drawn from
        // the ProjectClass table, which shows Project Classes assigned to this project. The nuance here is that the list of
        // Project Classes for this project may contain a Default - a Project Class that should be suggested to the User as their default choice.

        void FillProjectClassDDL(int? pcID)
        {
            if (pnlDestOfFunds.Visible)                                   // If true need to populate list
            {
                if (ddlProjectClass.Items.Count == 0)                   // If = the control is empty. Fill it
                {
                    int? defaultID = 0;
                    int projID = QueryStringActions.ConvertID(litSavedProjectID.Text).Int; // Find ID of current project
                    using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                    {
                        var query = from pc in context.ProjectClasses
                                    where pc.ProjectID == projID && !pc.Inactive // Fetch by ProjectID implicitly limits us to current Franchise
                                    orderby pc.Name
                                    select new { pc.ProjectClassID, pc.Name, pc.Default }; // Find all project classes that are assigned to this project

                        DataTable rows = new DataTable();
                        rows.Columns.Add(PortalConstants.DdlID);
                        rows.Columns.Add(PortalConstants.DdlName);

                        foreach (var row in query)
                        {
                            DataRow dr = rows.NewRow();                  // Build a row from the next query output
                            dr[PortalConstants.DdlID] = row.ProjectClassID;
                            dr[PortalConstants.DdlName] = row.Name;
                            rows.Rows.Add(dr);                           // Add the new row to the data table
                            if (row.Default)                            // If true, this is the default Project Class
                                defaultID = row.ProjectClassID;         // Save this for later use
                        }

                        if (pcID != null)                                   // If != caller specified something
                        {
                            if (pcID != 0)                                  // If != caller specified a row to select
                                defaultID = pcID;                           // Position the DDL to that row
                        }

                        StateActions.LoadDdl(ddlProjectClass, defaultID, rows, " -- Error: No Project Classes assigned to Project --", "-- none selected --");
                    }
                }
            }
            return;
        }

        // Move values from the Deposit record into the Page. Only move the values that are "Visible" for this Deposit Type

        void LoadPanels(Dep record)
        {
            litSavedStateEnum.Text = record.CurrentState.ToString();  // Stash enum version of state
            txtState.Text = EnumActions.GetEnumDescription(record.CurrentState); // Display "English" version of state

            if (pnlAmount.Visible)
                txtAmount.Text = ExtensionActions.LoadDecimalIntoTxt(record.Amount); // Fill textbox accounting for blank field

            if (pnlDateOfDeposit.Visible)
                DateActions.LoadDateIntoTxtCal(record.DateOfDeposit, txtDateOfDeposit, calDateOfDeposit);

            txtDescription.Text = record.Description;

            if (pnlDestOfFunds.Visible)                               // If true, process Destination of Funds and Project Class
            {
                if (record.DestOfFunds == SourceOfExpFunds.Restricted) // If == the Source of Funds is a Project Class
                {
                    rdoDestOfFunds.SelectedValue = PortalConstants.RDOFundsRestricted; // Select the button corresponding to restricted funds
                    pnlProjectClass.Visible = true;
                }
                else if (record.DestOfFunds == SourceOfExpFunds.Unrestricted) // If == the Dest of Funds does not use a Project Class
                {
                    rdoDestOfFunds.SelectedValue = PortalConstants.RDOFundsUnrestricted; // Select the other button
                    pnlProjectClass.Visible = false;                    // Unrestricted means no Project Class so don't show the list
                }
                FillProjectClassDDL(record.ProjectClassID);             // Fill the DDL even if it's not visible yet. User could change that
            }

            if (pnlGLCode.Visible)
                FillGLCodeDDL(record.GLCodeID, UseOnlyDepositGLCodes(record.DepType)); // Fill drop down list and hightlight the selected item

            if (pnlNotes.Visible)
                txtNotes.Text = record.Notes;

            if (pnlOptions.Visible)
                cblOptions.Items.FindByValue(Dep.OptionPledgePayment).Selected = record.PledgePayment; // Fill value of checkbox

            if (record.CurrentState == DepState.Returned)               // If == the Rqst has been returned, so a Return Note may be present
            {
                pnlReturnNote.Visible = true;                           // Make this panel visible
                txtReturnNote.Text = record.ReturnNote;                 // Copy the text of the Return Note
            }

            if (pnlSourceOfFunds.Visible)                               // If true the Deposit does have a Destination of Funds
            {
                FillEntityDDL(record.EntityID, record.EntityNeeded);    // Load the drop down list. Highlight prior selection, if any
                FillPersonDDL(record.PersonID, record.PersonNeeded);    // Load the drop down list. Highlight prior selection, if any.
                switch (record.SourceOfFunds)
                {
                    case SourceOfDepFunds.NA:
                        {
                            rdoSourceOfFunds.SelectedValue = PortalConstants.RDONotApplicable; // Select the corresponding button
                            break;
                        }
                    case SourceOfDepFunds.Entity:
                        {
                            rdoSourceOfFunds.SelectedValue = PortalConstants.RDOEntity; // Select the corresponding button
                            pnlEntity.Visible = true;                       // Make the drop down list appear
                            if (record.EntityIsAnonymous)                   // If true the Anonymous button is set
                            {
                                cblEntity.Items.FindByValue(Dep.EntityAnonymous).Selected = true; // Set the checkbox
                                pnlEntityDDL.Visible = false;               // That means the Entity ddl is not visible
                            }
                            break;
                        }
                    case SourceOfDepFunds.Individual:
                        {
                            rdoSourceOfFunds.SelectedValue = PortalConstants.RDOIndividual; // Select the corresponding button
                            pnlPerson.Visible = true;                       // Make the checkbox and drop down list appear.
                            if (record.PersonIsAnonymous)                   // If true the Anonymous button is set
                            {
                                cblPerson.Items.FindByValue(Dep.PersonAnonymous).Selected = true; // Set the checkbox
                                pnlPersonDDL.Visible = false;               // That means the Person ddl is not visible
                            }
                            break;
                        }
                    default:
                        {
                            LogError.LogInternalError("EditDeposit", string.Format("Invalid SourceOfDepFunds value '{0}' found in database",
                                record.SourceOfFunds.ToString())); // Fatal error
                            break;
                        }
                }
            }
            if (pnlStaffNote.Visible)
                txtStaffNote.Text = record.StaffNote;

            // Supporting Documents handled elsewhere

            return;
        }

        // Make all controls readonly to prevent editing during View function

        void ReadOnlyControls()
        {
            txtAmount.Enabled = false;
            txtDateOfDeposit.Enabled = false; btnDateOfDeposit.Enabled = false;
            txtDescription.Enabled = false;
            // Deposit Type - already set
            // Deposit State - already set
            pnlDestOfFunds.Enabled = false; ddlProjectClass.Enabled = false;
            ddlGLCode.Enabled = false;
            txtNotes.Enabled = false;
            pnlOptions.Enabled = false;
            rdoSourceOfFunds.Enabled = false; ddlPerson.Enabled = false; ddlEntity.Enabled = false;
            pnlStaffNote.Enabled = false;
            // Suporting Docs - leave Listbox filled and enabled so that double click still works
            if (pnlSupporting.Visible)
                fupUpload.Enabled = false;   // But no new items
            return;
        }

        // Move the "as edited" values from the Page into a Dep record. Only move the values that are "Visible" for this Deposit Type

        void UnloadPanels(Dep record)
        {
            if (pnlAmount.Visible)
                record.Amount = ExtensionActions.LoadTxtIntoDecimal(txtAmount); // Convert into decimal and store

            if (pnlDateOfDeposit.Visible)
                record.DateOfDeposit = DateActions.LoadTxtIntoDate(txtDateOfDeposit);

            record.DepType = EnumActions.ConvertTextToDepType(rdoDepType.SelectedValue); // Deposit Type - convert from string VALUE to enumerated RqstType

            record.Description = txtDescription.Text;

            if (pnlDestOfFunds.Visible)                                 // If true the Deposit does have a Destination of Funds
            {
                if (rdoDestOfFunds.SelectedValue == PortalConstants.RDOFundsRestricted) // If == Restricted button is clicked. 
                {
                    record.DestOfFunds = SourceOfExpFunds.Restricted;   // Remember this setting
                    record.ProjectClassID = StateActions.UnloadDdl(ddlProjectClass); // Pull the selected Project Class from the DDL
                }
                else
                    record.DestOfFunds = SourceOfExpFunds.Unrestricted; // Remember this non-setting
            }
            else
                record.DestOfFunds = SourceOfExpFunds.NA;               // Not applicable to this type of Expense Request

            if (pnlGLCode.Visible)
                record.GLCodeID = StateActions.UnloadDdl(ddlGLCode);    // Carefully pull selected value into record

            if (pnlNotes.Visible)
                record.Notes = txtNotes.Text;

            if (pnlOptions.Visible)
                record.PledgePayment = cblOptions.Items.FindByValue(Dep.OptionPledgePayment).Selected; // Stash value of checkbox

            // Return Note is read-only, so not unloaded

            if (pnlSourceOfFunds.Visible)                               // If true the Deposit does have a Destination of Funds
            {
                switch (rdoSourceOfFunds.SelectedValue)
                {
                    case PortalConstants.RDONotApplicable:
                        {
                            record.SourceOfFunds = SourceOfDepFunds.NA; // Record the selection
                            break;
                        }
                    case PortalConstants.RDOEntity:
                        {
                            record.SourceOfFunds = SourceOfDepFunds.Entity; // Record the selection
                            record.EntityIsAnonymous = cblEntity.Items.FindByValue(Dep.EntityAnonymous).Selected; // Stash value of checkbox
                            {
                                int? id; bool needed;
                                StateActions.UnloadDdl(ddlEntity, out id, out needed); // Carefully pull selected value into record
                                record.EntityID = id; record.EntityNeeded = needed;
                            }
                            record.EntityRole = EnumActions.ConvertTextToEntityRole(litSavedEntityEnum.Text); // Convert role to enum
                            break;
                        }
                    case PortalConstants.RDOIndividual:
                        {
                            record.SourceOfFunds = SourceOfDepFunds.Individual; // Record the selection
                            record.PersonIsAnonymous = cblPerson.Items.FindByValue(Dep.PersonAnonymous).Selected; // Stash value of checkbox
                            {
                                int? id; bool needed;
                                StateActions.UnloadDdl(ddlPerson, out id, out needed); // Carefully pull selected value into record
                                record.PersonID = id; record.PersonNeeded = needed;
                            }
                            record.PersonRole = EnumActions.ConvertTextToPersonRole(litSavedPersonEnum.Text); // Convert relevant role to enum
                            break;
                        }
                    default:
                        {
                            LogError.LogInternalError("EditDeposit", string.Format("Invalid SourceOfDepFunds value '{0}' returned by RDO",
                                rdoSourceOfFunds.SelectedValue)); // Fatal error
                            break;
                        }
                }
            }

            if (pnlStaffNote.Visible)
                record.StaffNote = txtStaffNote.Text;

            return;
        }

        // Load the Supporting Listbox control from the database.

        void LoadSupportingDocs(Dep rqst)
        {
            if (pnlSupporting.Visible)                              // If true the ListBox is visible so we need to fill it
            {
                SupportingActions.LoadDocs(RequestType.Deposit, rqst.DepID, lstSupporting, litDangerMessage); // Do the heavy lifting
            }
            return;
        }

        // Unload files from Listbox control. That is, move a supporting file from temporary to permanent status.

        void UnloadSupportingDocs()
        {
            if (pnlSupporting.Visible)                              // If true, panel containing ListBox control is visible. Work to do
            {
                SupportingActions.UnloadDocs(lstSupporting, litSavedUserID.Text, RequestType.Deposit, QueryStringActions.ConvertID(litSavedDepID.Text).Int, litDangerMessage);
            }
            return;
        }

        // For a given DepType, figure out whether to use just Deposit GL Codes or all GL Codes

        bool UseOnlyDepositGLCodes(DepType type)
        {
            switch (type)
            {
                case DepType.Check:
                case DepType.EFT:
                    return false;                                   // Use all GL codes

                case DepType.Cash:
                case DepType.InKind:
                case DepType.Pledge:
                    return true;                                    // Use just Deposit GL codes

                default:
                    {
                        LogError.LogInternalError("EditDeposit", $"Invalid DepTypes value '{type}' found"); // Fatal error
                        return false;
                    }
            }
        }
    }
}
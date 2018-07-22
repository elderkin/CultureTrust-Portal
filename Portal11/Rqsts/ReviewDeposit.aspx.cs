using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Rqsts
{
    public partial class ReviewDeposit : System.Web.UI.Page
    {
        // Review a Deposit Request. Communication from Staff Dashboard is through Query Strings:
        //      UserRole - the role of the current user, e.g., Project, CD, FD, PR.
        //      RequestID - the database ID of the existing Request to process (Required)
        //      Command - "Review" (Required)
        //      Return - name of the page to invoke on completion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // Fetch information about current user. Do first to catch missing user login.

                string userID, userRole;
                QueryStringActions.GetUserIDandRole(out userID, out userRole); //Fetch user ID and role
                litSavedUserID.Text = userID; litSavedUserRole.Text = userRole; // Save for later

                QSValue depID = QueryStringActions.GetRequestID();      // Fetch the Deposit ID
                string cmd = QueryStringActions.GetCommand();           // Fetch the command
                string ret = QueryStringActions.GetReturn();            // Fetch the return page

                // Fetch the Dep row

                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    Dep dep = context.Deps.Find(depID.Int);             // Fetch Dep row by its key
                    if (dep == null)
                        LogError.LogInternalError("ReviewDeposit", $"Invalid DepID value '{depID.String}' could not be found in database"); // Log fatal error

                    // Stash these parameters into invisible literals on the current page.

                    litSavedDepID.Text = depID.String;
                    litSavedDepType.Text = EnumActions.GetEnumDescription(dep.DepType); // Type of Deposit Request
                    litSavedCommand.Text = cmd;
                    litSavedProjectID.Text = dep.ProjectID.ToString();
                    litSavedProjectName.Text = dep.Project.Name;
                    litSavedProjectCode.Text = dep.Project.Code;
                    litSavedReturn.Text = ret;

                    // If the user is actually in the process of revising this request, the Dashboard comes here anyway to keep the dashboard processing from getting too complex.
                    // All we have to do is act like the user pressed the "Revise" button and dispatch to Edit Deposit.

                    if (StateActions.RequestIsRevising(dep.CurrentState)) // If true, this request is being revised
                        DispatchRevise();                               // Break out of this stream of processing

                    // Load Dep fields into the page

                    EnablePanels(dep.DepType);                          // Make the relevant panels visible
                    LoadPanels(dep);                                    // Move values from record to page

                    // See if this user has the role to Approve the Dep in its current state. If not, just let them view the Dep

                    if (!StateActions.UserCanApproveRequest(dep.CurrentState)) // If false user can not Approve this Dep. Disable functions
                    {
                        txtReturnNote.Enabled = false; btnReturnNoteClear.Visible = false; // Can see this note but not edit it
                        txtStaffNote.Enabled = false; btnStaffNoteClear.Visible = false; // Can see this note but not edit it
                        btnApprove.Enabled = false;                     // Cannot "Approve" the Dep
                        btnReturn.Enabled = false;                      // Cannot "Return" the Dep
                        btnRevise.Enabled = false;                      // Cannot "Revise" the Dep
                        litDangerMessage.Text = "You can view this Deposit Request, but you cannot approve it."; // Explain that to user
                    }
                    //else if (!StateActions.UserCanReviseRequest(dep.CurrentState)) // If false user cannot Revise this Dep. Disable function
                    //{
                    //    btnRevise.Enabled = false;                      // Cannot "Revise" the Dep
                    //}
                }
            }
            return;
        }

        // The user clicked on a row of the Supporting Docs list. Enable the View button and wait for action.

        protected void lstSupporting_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnViewLink.Enabled = true; btnViewLink.Visible = true;
            btnViewLink.NavigateUrl = SupportingActions.ViewDocUrl((ListBox)sender); // Formulate URL to launch viewer page
            return;
        }

        // The user clicked on the Zip button of the Supporting Docs list.

        protected void btnZip_Click(object sender, EventArgs e)
        {
            SupportingActions.DownloadDocsZip(RequestType.Deposit,  // What type of request
                                Convert.ToInt32(litSavedDepID.Text), // Which request ID
                                litSavedUserID.Text,                // Which user
                                litSavedProjectCode.Text,           // Which project code
                                litSDSuccess, litDangerMessage);    // Error reporting
        }

        protected void btnReturnNoteClear_Click(object sender, EventArgs e)
        {
            txtReturnNote.Text = "";
            litReturnNoteError.Visible = false;                     // Hide error message, if any
            return;
        }

        protected void btnStaffNoteClear_Click(object sender, EventArgs e)
        {
            txtStaffNote.Text = "";
            return;
        }

        protected void gvEDHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                // If >= a value that we can handle
            {
                gvEDHistory.PageIndex = e.NewPageIndex;             // Propagate the desired page index
                NavigationActions.LoadAllDepHistorys(Convert.ToInt32(litSavedDepID.Text), gvEDHistory); // Re-fill the GridView control
                gvEDHistory.SelectedIndex = -1;                     // No row currently selected
            }
            return;
        }

        // User clicked Cancel. This is easy: Just head back to the StaffDashboard.

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(litSavedReturn.Text);                 // Adios!
        }

        // User clicked Approve. "Advance" the State according to complex rules and write down the Return Note, if any.
        // Update the Dep and create a DepHistory. Then head back to the StaffDashboard.

        protected void btnApprove_Click(object sender, EventArgs e)
        {

            // If the Return Note field is non-blank, the user has probably pressed "Approve" by accident. What they probably want
            // is "Return." So report an error if Return Note is non-blank. But don't erase - the user might still want to press "Return."

            if (!string.IsNullOrEmpty(txtReturnNote.Text))          // If false text is erroneously present
            {
                litReturnNoteError.Text = PortalConstants.ReturnNotePresent; // Report the error
                litReturnNoteError.Visible = true;                  // Make the error message visible
                return;                                             // Go back for more punishment
            }
            DepState currentState = EnumActions.ConvertTextToDepState(litSavedState.Text); // Pull ToString version; convert to enum type
            DepState nextState = StateActions.FindNextState(currentState, ReviewAction.Approve); // Now what?

            SaveDep(nextState, ReviewAction.Approve, "Approved");   // Update Dep; write new History row

            string emailSent = EmailActions.SendEmailToReviewer(false, // Send "non-rush" email to next reviewer
                StateActions.UserRoleToProcessRequest(nextState),   // Who is in this role
                Convert.ToInt32(litSavedProjectID.Text),            // Request is associated with this project
                litSavedProjectName.Text,                           // Project has a name
                EnumActions.GetEnumDescription(RequestType.Deposit), // This is a Deposit Request
                litSavedDepType.Text,                               // Here is its type
                EnumActions.GetEnumDescription(nextState),          // Here is its next state
                PortalConstants.CEmailDefaultDepositApprovedSubject, PortalConstants.CEmailDefaultDepositApprovedBody); // Use this subject and body, if needed

            Response.Redirect(litSavedReturn.Text + "?"
                                + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                + PortalConstants.QSStatus + "=" + "Request Appoved and Advanced to '"
                                + EnumActions.GetEnumDescription(nextState) + "' status." + emailSent);
        }

        // User clicked Return. Set the state to Returned and process the request just like an Approve. Then notify the user of the glitch.

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtReturnNote.Text))           // If true the Return Note is missing
            {
                litReturnNoteError.Text = PortalConstants.ReturnNoteMissing; // Report the error to the user
                litReturnNoteError.Visible = true;
                return;
            }
            DepState currentState = EnumActions.ConvertTextToDepState(litSavedState.Text); // Pull ToString version; convert to enum type
            SaveDep(StateActions.FindNextState(currentState, ReviewAction.Return), ReviewAction.Return, "Returned"); // Update Dep; write new History row

            string emailSent = EmailActions.SendEmailToReviewer(false, // Send "non-rush" email to next reviewer
                StateActions.UserRoleToProcessRequest(DepState.Returned), // Who is in this role
                Convert.ToInt32(litSavedProjectID.Text),            // Request is associated with this project
                litSavedProjectName.Text,                           // Project has a name
                EnumActions.GetEnumDescription(RequestType.Deposit), // This is a Deposit Request
                litSavedDepType.Text,                               // Here is its type
                EnumActions.GetEnumDescription(DepState.Returned),  // Here is its next state
                PortalConstants.CEmailDefaultDepositApprovedSubject, PortalConstants.CEmailDefaultDepositApprovedBody); // Use this subject and body, if needed

            Response.Redirect(litSavedReturn.Text + "?"
                                + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                + PortalConstants.QSStatus + "=" + "Request returned to Internal Coordinator." + emailSent);
        }

        // User clicked Revise. Set the state to Revising and save it. Then invoke Edit Deposit to make the revisions. No email in this path.

        protected void btnRevise_Click(object sender, EventArgs e)
        {
            DepState currentState = EnumActions.ConvertTextToDepState(litSavedState.Text); // Pull ToString version; convert to enum type
            SaveDep(StateActions.FindNextState(currentState, ReviewAction.Revise), ReviewAction.Revise, "Revising"); // Update Dep; write new History row
            DispatchRevise();                                       // Dispatch to Edit Deposit
        }

        void DispatchRevise()
        { 
            
            // Find current role and map to the role to be used for editing.

            UserRole userRole = EnumActions.ConvertTextToUserRole(QueryStringActions.GetUserRole()); // Fetch User Role from UserInfo cookie and stash
            string reviseRole = RoleActions.GetRevisingRole(userRole).ToString(); // Find role that EditDeposit should use during edits

            Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + reviseRole + "&"
                                            + PortalConstants.QSRequestID + "=" + litSavedDepID.Text + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandRevise + "&" // Start with an existing request
                                            + PortalConstants.QSReturn + "=" + litSavedReturn.Text);
        }

        // User pressed History button. Fetch all the DepHistory rows for this Dep and fill a GridView.

        protected void btnHistory_Click(object sender, EventArgs e)
        {
            if (pnlHistory.Visible)                                     // If true the History panel is visible
                pnlHistory.Visible = false;                             // Make it invisible
            else
            {
                NavigationActions.LoadAllDepHistorys(Convert.ToInt32(litSavedDepID.Text), gvEDHistory); // Fill the grid
                pnlHistory.Visible = true;                              // Make it visible
            }
            return;
        }

        // Package the work of the Save so that Submit and Revise can do it as well.
        //  1) Fetch the Dep row to be updated.
        //  2) Create a new DepHistory row and fill it.
        //  3) Update the Dep row with new State and Return Note.
        //  4) Flip the value of the ReviseUserRole under certain circumstances

        private void SaveDep(DepState nextState, ReviewAction action, string verb)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    int depID = Convert.ToInt32(litSavedDepID.Text);            // Fetch ID of this Dep
                    Dep toUpdate = context.Deps.Find(depID);                    // Fetch the Dep row that we want to update
                    DepHistory hist = new DepHistory();                         // Get a place to build a new Request History row
                    toUpdate.ReturnNote = txtReturnNote.Text;                   // Fetch updated content of the note, if any
                    hist.ReturnNote = txtReturnNote.Text;                       // Preserve this note in the History trail
                    toUpdate.StaffNote = txtStaffNote.Text;                     // Fetch updated content of the note, if any
                    StateActions.CopyPreviousState(toUpdate, hist, verb);       // Create a Request History log row from "old" version of Request
                    StateActions.SetNewState(toUpdate, nextState, litSavedUserID.Text, hist); // Write down our current State and authorship

                    UserRole newReviewUserRole = RoleActions.UpdateReviseUserRole(action, EnumActions.ConvertTextToUserRole(litSavedUserRole.Text), toUpdate.ReviseUserRole);
                    toUpdate.ReviseUserRole = newReviewUserRole;                // Start or end a revise cycle by updating ReviseUserRole

                    context.DepHistorys.Add(hist);                              // Save new DepHistory row
                    context.SaveChanges();                                      // Commit the Add and Modify
                    return;
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ReviewDeposit", "Unable to update Dep or DepHistory rows"); // Fatal error
                }
            }
        }

        // Based on the selected Deposit Type, enable and disable the appropriate panels on the display.
        // Note: I know that all of these "static" settings could be done right in the .aspx file. But having it here
        // is a reminder of what's live on this page.

        void EnablePanels(DepType type)
        {
            pnlAmount.Visible = true;                                           // Unconditionally visible for all Deposit types
            pnlDescription.Visible = true;
            pnlEstablishedBy.Visible = true;
            pnlEstablishedTime.Visible = true;
            pnlGLCode.Visible = true;
            pnlNotes.Visible = true;
            pnlReturnNote.Visible = true;
            if (RoleActions.UserRoleIsStaff(EnumActions.ConvertTextToUserRole(litSavedUserRole.Text))) // If true, user is a staff member
                pnlStaffNote.Visible = true;                                    // Staff users can see it
            else
                pnlStaffNote.Visible = false;                                   // Others don't see it
            pnlState.Visible = true;
            pnlSupporting.Visible = true;
            switch (type)
            {
                case DepType.Cash:
                    {
                        txtAmount.Text = "Dollar Amount";
                        lblDateOfDeposit.Text = "Date Received";
                        pnlOptions.Visible = true;
                        break;
                    }
                case DepType.Check:
                    {
                        txtAmount.Text = "Dollar Amount";
                        lblDateOfDeposit.Text = "Date on Check";
                        pnlOptions.Visible = true;
                        break;
                    }
                case DepType.EFT:
                    {
                        txtAmount.Text = "Dollar Amount";
                        lblDateOfDeposit.Text = "Date of Draft";
                        pnlOptions.Visible = true;
                        break;
                    }
                case DepType.InKind:
                    {
                        txtAmount.Text = "Fair Market Value";
                        lblDateOfDeposit.Text = "Date Goods/Services Received";
                        pnlOptions.Visible = true;
                        break;
                    }
                case DepType.Pledge:
                    {
                        txtAmount.Text = "Dollar Amount";
                        lblDateOfDeposit.Text = "Date on Pledge/Contract";
                        pnlOptions.Visible = false;
                        break;
                    }
                default:
                    {
                        LogError.LogInternalError("ReviewDeposit", $"Invalid DepType value '{type.ToString()}' in database"); // Fatal error
                        break;
                    }
            }
            return;
        }

        // Move values from the Request record into the Page. Only move the values that are "Visible" for this Request Type

        void LoadPanels(Dep record)
        {
            txtStateDescription.Text = EnumActions.GetEnumDescription(record.CurrentState); // Convert enum value to English
            litSavedState.Text = record.CurrentState.ToString();            // Saved enum value for later "case"

            txtEstablishedTime.Text = DateActions.LoadDateTimeIntoTxt(record.CurrentTime);
            txtEstablishedBy.Text = record.CurrentUser.FullName;

            txtAmount.Text = ExtensionActions.LoadDecimalIntoTxt(record.Amount);

            txtDateOfDeposit.Text = DateActions.LoadDateIntoTxt(record.DateOfDeposit);

            txtDescription.Text = record.Description;

            txtTypeDescription.Text = EnumActions.GetEnumDescription((Enum)record.DepType); // Convert enum value to English value for display

            //if (record.DestOfFunds == SourceOfExpFunds.Restricted) // If == the Source of Funds is a Project Class
            //{
            //    rdoDestOfFunds.SelectedValue = PortalConstants.RDOFundsRestricted; // Select "restricted" button
            //    pnlProjectClass.Visible = true;
            //}
            //else if (record.DestOfFunds == SourceOfExpFunds.Unrestricted) // If == the Dest of Funds does not use a Project Class
            //{
            //    rdoDestOfFunds.SelectedValue = PortalConstants.RDOFundsUnrestricted; // Select "unrestricted" button
            //    pnlProjectClass.Visible = false;                    // Unrestricted means no Project Class so don't show the list
            //}
            if (record.ProjectClassID != null)
                txtProjectClass.Text = record.ProjectClass.Name;

            if (record.GLCodeID != null)
                txtGLCode.Text = record.GLCode.Code;

            if (pnlNotes.Visible)
                txtNotes.Text = record.Notes;

            if (pnlOptions.Visible)
            {
                foreach (ListItem item in cblOptions.Items)
                {
                    if (item.Value == "PledgePayment")                  // If == load Inactive option from database
                        item.Selected = record.PledgePayment;           // Load checkbox from database
                }
            }

            txtProjectName.Text = record.Project.Name;

            txtReturnNote.Text = record.ReturnNote;                     // Copy the text of the Return Note, even if blank

            switch (record.SourceOfFunds)
            {
                case SourceOfDepFunds.NA:
                    {
                        rdoSourceOfFunds.SelectedValue = PortalConstants.RDONotApplicable; // Select "Not Applicable" button
                        break;
                    }
                case SourceOfDepFunds.Entity:
                    {
                        rdoSourceOfFunds.SelectedValue = PortalConstants.RDOEntity; // Select "Entity" button
                        if (record.EntityIsAnonymous)                   // If true entity is anonymous. No entity to display
                        {
                            pnlEntityAnonymous.Visible = true;          // Turn on the marker for anonymous
                        }
                        else
                        {
                            pnlEntity.Visible = true;                   // Show the slot for the entity's name
                            if (record.EntityID != null)                // If != there is an Entity to display
                                txtEntity.Text = record.Entity.Name;    // Drop it in the slot
                            else if (record.EntityNeeded)                   // If true, PD asks us to create a new Entity
                                txtEntity.Text = "-- Please create a new " + lblEntity.Text + "--"; // Make that request
                        }
                        break;
                    }
                case SourceOfDepFunds.Individual:
                    {
                        rdoSourceOfFunds.SelectedValue = PortalConstants.RDOIndividual; // Select "Individual" button
                        if (record.PersonIsAnonymous)                   // If true person is anonymous. No person to display
                        {
                            pnlPersonAnonymous.Visible = true;          // Turn on the marker for anonymous
                        }
                        else
                        {
                            pnlPerson.Visible = true;                   // Show the slot for the person's name
                            if (record.PersonID != null)                // If != there is a Person to display
                                txtPerson.Text = record.Person.Name;
                            else if (record.PersonNeeded)               // If true, PD asks us to create a new Person
                                txtPerson.Text = "-- Please create a new " + lblPerson.Text + " --"; // Make that request
                        }
                        break;
                    }
                default:
                    {
                        LogError.LogInternalError("EditDeposit", $"Invalid SourceOfDepFunds value '{record.SourceOfFunds.ToString()}' found in database"); // Fatal error
                        break;
                    }
            }
            if (SplitActions.LoadSplitRowsForView(RequestType.Deposit, record.DepID, gvDepSplit)) // If true, ExpSplits existed and were loaded
                EnergizeSplit();                                        // Adjust page to accommodate split gridview

            txtStaffNote.Text = record.StaffNote;                       // Load current value of note, if any

            SupportingActions.LoadDocs(RequestType.Deposit, record.DepID, lstSupporting, litDangerMessage); // Load them into list box

            return;
        }

        // We have a full split gridview. Now adjust the operation of the page to process splits

        void EnergizeSplit()
        {
            pnlProjectClass.Visible = false;                        // Can't see "Project Class" field any more
//            pnlAmount.Visible = false;                              // Can't see "Total Dollar Amount" field any more
            pnlGLCode.Visible = false;                              // Can't see "Expense Account" drop down list any more
            pnlDepositSplit.Visible = true;                         // Turn on the grid for splits
            return;
        }
    }
}
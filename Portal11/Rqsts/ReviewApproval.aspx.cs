using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Rqsts
{
    public partial class ReviewApproval : System.Web.UI.Page
    {

        // Review an Approval Request. Communication from Staff Dashboard is through Query Strings:
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

                QSValue appID = QueryStringActions.GetRequestID();      // Fetch the Request ID
                string cmd = QueryStringActions.GetCommand();           // Fetch the command
                string ret = QueryStringActions.GetReturn();            // Fetch the return page

                // Fetch the App row and fill the page

                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    App app = context.Apps.Find(appID.Int);             // Fetch App row by its key
                    if (app == null)
                        LogError.LogInternalError("ReviewApproval", $"Invalid AppID value '{appID.String}' could not be found in database"); // Log fatal error

                    EnablePanels(app.AppType);                          // Make the relevant panels visible
                    LoadPanels(app);                                    // Move values from record to page

                    // See if this user has the role to approve the App in its current state. If not, just let them view the App

                    if (!StateActions.UserCanApproveRequest(app.CurrentState)) // If false user can not Approve this App. Disable functions
                    {
                        txtReturnNote.Enabled = false;                  // Can see this note but not edit it
                        txtStaffNote.Enabled = false;                   // Can see this note but not edit it
                        btnApprove.Enabled = false;                     // Cannot "Approve" the App
                        btnReturn.Enabled = false;                      // Cannot "Return" the App
                        litDangerMessage.Text = "You can view this Approval Request, but you cannot approve it."; // Explain that to user
                    }

                    // Stash these parameters into invisible literals on the current page.

                    litSavedAppID.Text = appID.String;
                    litSavedCommand.Text = cmd;
                    litSavedProjectID.Text = app.ProjectID.ToString();
                    litSavedReturn.Text = ret;
                    litSavedReviewType.Text = app.AppReviewType.ToString(); // Save for later
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

        protected void gvEDHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                gvEDHistory.PageIndex = e.NewPageIndex;             // Propagate the desired page index
                LoadAllAppHistorys();                                  // Re-fill the GridView control
                gvEDHistory.SelectedIndex = -1;                     // No row currently selected
            }
            return;
        }

        // View a Supporting Document We replicate the logic from the EditApproval page and download the selected Supporting Document file.
        // This case is simpler than EditApproval because all the Docs are "permanent," described in SupportingDoc rows.

        //protected void btnView_Click(object sender, EventArgs e)
        //{
        //    SupportingActions.ViewDoc(lstSupporting, litDangerMessage); // Do the heavy lifting
        //    return;
        //}

        // User clicked Cancel. This is easy: Just head back to the StaffDashboard.

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(litSavedReturn.Text);                 // Adios!
        }

        // User clicked Approve. "Advance" the State according to complex rules and write down the Return Note, if any.
        // A twist is that the Approver may have changed the Review Type, impacting the nextState. So check that early in the flow.
        // Update the App and create a AppHistory. Then head back to the StaffDashboard.

        protected void btnApprove_Click(object sender, EventArgs e)
        {

            // If the Return Note field is non-blank, the user has probably pressed "Approve" by accident. What they probably want
            // is "Return." So report an error if Return Note is non-blank. But don't erase - the user might still want to press "Return."

            if (txtReturnNote.Text != "")                           // If != then text is erroneously present
            {
                litDangerMessage.Text = PortalConstants.ReturnNoteError; // Report the error
                Page.MaintainScrollPositionOnPostBack = false;      // Scrool back to top of page where error message lives
                return;                                             // Go back for more punishment
            } 
            AppState currentState = EnumActions.ConvertTextToAppState(litSavedState.Text); // Pull ToString version; convert to enum type
            AppReviewType newAppReviewType = EnumActions.ConvertTextToAppReviewType(rdoReviewType.SelectedValue); // Convert selection to enum
            AppState nextState = StateActions.FindNextState(currentState, newAppReviewType ); // Now what?

            int projectID = new int(); string projectName = "";
            SaveApp(nextState, "Advanced", out projectID, out projectName); // Update App; write new History row

            string emailSent = EmailActions.SendEmailToReviewer(false, // Send "non-rush" email to next reviewer
                StateActions.UserRoleToApproveRequest(nextState),   // Who is in this role
                projectID,                                          // Request is associated with this project
                projectName,                                        // Project has a name
                EnumActions.GetEnumDescription(RequestType.Approval), // This is an Approval Request
                EnumActions.GetEnumDescription(nextState),          // Here is its next state
                PortalConstants.CEmailDefaultApprovalApprovedSubject, PortalConstants.CEmailDefaultApprovalApprovedBody); // Use this subject and body, if needed

            Response.Redirect(litSavedReturn.Text + "?"
                                + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                + PortalConstants.QSStatus + "=" + "Request Appoved and Advanced to '"
                                + EnumActions.GetEnumDescription(nextState) + "' status." + emailSent);
        }

        // User clicked Return. Set the state to Returned and process the request just like an Approve. Then notify the user of the glitch.

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            int projectID = new int(); string projectName = "";
            SaveApp(AppState.Returned, "Returned", out projectID, out projectName); // Update App; write new History row
            string emailSent = EmailActions.SendEmailToReviewer(false, // Send "non-rush" email to next reviewer
                StateActions.UserRoleToApproveRequest(AppState.Returned), // Who is in this role
                projectID,                                          // Request is associated with this project
                projectName,                                        // Project has a name
                EnumActions.GetEnumDescription(RequestType.Approval), // This is an Approval Request
                EnumActions.GetEnumDescription(AppState.Returned),  // Here is its next state
                PortalConstants.CEmailDefaultApprovalReturnedSubject, PortalConstants.CEmailDefaultApprovalReturnedBody); // Use this subject and body, if needed

            Response.Redirect(litSavedReturn.Text + "?"
                                + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                + PortalConstants.QSStatus + "=" + "Request returned to Project Direcctor." + emailSent);
        }

        // User pressed History button. Fetch all the AppHistory rows for this App and fill a GridView.

        protected void btnHistory_Click(object sender, EventArgs e)
        {
            LoadAllAppHistorys();                                      // Fill the grid
            return;
        }

        // Package the work of the Save so that Submit and Revise can do it as well.
        //  1) Fetch the App row to be updated.
        //  2) Create a new AppHistory row and fill it.
        //  3) Update the App row with new State, new ReviewType and Return Note.

        private void SaveApp(AppState nextState, string verb, out int projectID, out string projectName)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    int appID = Convert.ToInt32(litSavedAppID.Text);            // Fetch ID of this App
                    App toUpdate = context.Apps.Find(appID);                    // Fetch the App row that we want to update
                    AppHistory hist = new AppHistory();                         // Get a place to build a new Request History row
                    toUpdate.AppReviewType = EnumActions.ConvertTextToAppReviewType(rdoReviewType.SelectedValue); // Convert selection to enum
                    toUpdate.ReturnNote = txtReturnNote.Text;                   // Fetch updated content of the note, if any
                    hist.ReturnNote = txtReturnNote.Text;                       // Preserve this note in the History trail
                    toUpdate.StaffNote = txtStaffNote.Text;                     // Fetch updated content of this note, if any
                    StateActions.CopyPreviousState(toUpdate, hist, verb);       // Create a Request History log row from "old" version of Request
                    StateActions.SetNewAppState(toUpdate, nextState, litSavedUserID.Text, hist); // Write down our current State and authorship
                    context.AppHistorys.Add(hist);                              // Save new AppHistory row
                    context.SaveChanges();                                      // Commit the Add and Modify
                    projectID = (int)toUpdate.ProjectID;                        // Report project ID to caller
                    projectName = toUpdate.Project.Name;                        // Report project name to caller
                    return;
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ReviewApproval", "Unable to update App or AppHistory rows"); // Fatal error
                }
            }
            projectID = 0; projectName = "";
        }

        // Based on the selected Approval Type, enable and disable the appropriate panels on the display.

        void EnablePanels(AppType type)
        {
            pnlDescription.Visible = true;
            pnlEstablishedBy.Visible = true;
            pnlEstablishedTime.Visible = true;
            pnlNotes.Visible = true;
            pnlReturnNote.Visible = true;
            pnlReviewType.Visible = true;
            if ((litSavedUserRole.Text == UserRole.Project.ToString())
                || (litSavedUserRole.Text == UserRole.Auditor.ToString()))      // If true, user is a Project Director or Auditor
                pnlStaffNote.Visible = false;                                   // They don't see Staff Note
            else
                pnlStaffNote.Visible = true;                                    // But Staff users can see it
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
                        rdoReviewType.Items[App.AppTypeFull].Enabled = true; rdoReviewType.Items[App.AppTypeFull].Selected = false; // Full
                        break;
                    }
                case AppType.Certificate:
                    {
                        rdoReviewType.Items[App.AppTypeICOnly].Enabled = false; rdoReviewType.Items[App.AppTypeICOnly].Selected = false; // COI
                        rdoReviewType.Items[App.AppTypeExpress].Enabled = false; rdoReviewType.Items[App.AppTypeExpress].Selected = false; // Express
                        rdoReviewType.Items[App.AppTypeFull].Enabled = false; rdoReviewType.Items[App.AppTypeFull].Selected = false; // Full
                        break;
                    }
                default:
                    {
                        LogError.LogInternalError("ReviewApproval", $"Invalid AppType value '{type.ToString()}' in database"); // Fatal error
                        break;
                    }
            }
            return;
        }

        // Move values from the Request record into the Page. Only move the values that are "Visible" for this Request Type

        void LoadPanels(App record)
        {
            txtStateDescription.Text = EnumActions.GetEnumDescription(record.CurrentState); // Convert enum value to English
            litSavedState.Text = record.CurrentState.ToString();            // Saved enum value for later "case"

            txtEstablishedTime.Text = DateActions.LoadDateIntoTxt(record.CurrentTime);
            txtEstablishedBy.Text = record.CurrentUser.FullName;

            txtDescription.Text = record.Description;

            txtTypeDescription.Text = EnumActions.GetEnumDescription((Enum)record.AppType); // Convert enum value to English value for display

            ExtensionActions.LoadEnumIntoRdo(record.AppReviewType, rdoReviewType); // Load enum value into Radio Button List

            if (pnlNotes.Visible)
                txtNotes.Text = record.Notes;

            txtProjectName.Text = record.Project.Name;

            if (record.CurrentState == AppState.Returned)               // If == the Rqst has been returned, so a Return Note may be present
            {
                pnlReturnNote.Visible = true;                           // Make this panel visible
                txtReturnNote.Text = record.ReturnNote;                 // Copy the text of the Return Note
            }

            txtStaffNote.Text = record.StaffNote;                       // Load value, if any

            SupportingActions.LoadDocs(RequestType.Approval, record.AppID, lstSupporting, litDangerMessage); // Load them into list box

            return;
        }

        void LoadAllAppHistorys()
        {
            NavigationActions.LoadAllAppHistorys(Convert.ToInt32(litSavedAppID.Text), gvEDHistory); // Fill the list from the database
            return;
        }
    }
}
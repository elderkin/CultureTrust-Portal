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
        //      RequestID - the database ID of the existing Request to process (Required)
        //      Command - "Review" (Required)
        //      Return - name of the page to invoke on completion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                QSValue depID = QueryStringActions.GetRequestID();      // Fetch the Deposit ID
                string cmd = Request.QueryString[PortalConstants.QSCommand]; // Fetch the command
                string ret = Request.QueryString[PortalConstants.QSReturn]; // Fetch the return page
                if (depID.Int == 0 || cmd != PortalConstants.QSCommandReview || ret == "") // If null or blank, this form invoked incorrectly
                    LogError.LogQueryStringError("ReviewDeposit", "Invalid Query String 'Command' or 'DepID'"); // Log fatal error

                // Fetch the Dep row and fill the page

                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    Dep dep = context.Deps.Find(depID.Int);             // Fetch Dep row by its key
                    if (dep == null)
                        LogError.LogInternalError("ReviewDeposit", $"Invalid DepID value '{depID.String}' could not be found in database"); // Log fatal error

                    EnablePanels(dep.DepType);                          // Make the relevant panels visible
                    LoadPanels(dep);                                    // Move values from record to page

                    // See if this user has the role to Approve the Dep in its current state. If not, just let them view the Dep

                    if (!StateActions.UserCanApproveRequest(dep.CurrentState)) // If false user can not Approve this Dep. Disable functions
                    {
                        btnApprove.Enabled = false;                     // Cannot "Approve" the Dep
                        btnReturn.Enabled = false;                      // Cannot "Return" the Dep
                        litDangerMessage.Text = "You can view this Deposit Request, but you cannot approve it."; // Explain that to user
                    }

                    // Stash these parameters into invisible literals on the current page.

                    litSavedDepID.Text = depID.String;
                    litSavedCommand.Text = cmd;
                    litSavedProjectID.Text = dep.ProjectID.ToString();
                    litSavedReturn.Text = ret;
                }
            }
            return;
        }

        // The user clicked on a row of the Supporting Docs list. Enable the View button and wait for action.

        protected void lstSupporting_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnView.Enabled = true;
            return;
        }

        protected void EDHistoryView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                EDHistoryView.PageIndex = e.NewPageIndex;             // Propagate the desired page index
                LoadAllDepHistorys();                                  // Re-fill the GridView control
                EDHistoryView.SelectedIndex = -1;                     // No row currently selected
            }
            return;
        }

        // View a Supporting Document We replicate the logic from the EditDeposit page and download the selected Supporting Document file.
        // This case is simpler than EditDeposit because all the Docs are "permanent," described in SupportingDoc rows.

        protected void btnView_Click(object sender, EventArgs e)
        {
            SupportingActions.ViewDoc(lstSupporting, litDangerMessage); // Do the heavy lifting
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
            DepState currentState = EnumActions.ConvertTextToDepState(litSavedState.Text); // Pull ToString version; convert to enum type
            DepState nextState = StateActions.FindNextState(currentState); // Now what?
            SaveDep(nextState, "Approved");                         // Update Dep; write new History row

            string emailSent = EmailActions.SendEmailToReviewer(StateActions.UserRoleToApproveRequest(nextState), // Tell next reviewer, who is in this role
                Convert.ToInt32(litSavedProjectID.Text),            // Request is associated with this project
                PortalConstants.CEmailDefaultDepositApprovedSubject, PortalConstants.CEmailDefaultDepositApprovedBody); // Use this subject and body, if needed

            Response.Redirect(litSavedReturn.Text + "?"
                                + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                + PortalConstants.QSStatus + "=" + "Request Appoved and Advanced to '"
                                + EnumActions.GetEnumDescription(nextState) + "' status." + emailSent);
        }

        // User clicked Return. Set the state to Returned and process the request just like an Approve. Then notify the user of the glitch.

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            SaveDep(DepState.Returned, "Returned");                 // Update Dep; write new History row

            string emailSent = EmailActions.SendEmailToReviewer(StateActions.UserRoleToApproveRequest(DepState.Returned), // Tell next reviewer, who is in this role
                Convert.ToInt32(litSavedProjectID.Text),            // Request is associated with this project
                PortalConstants.CEmailDefaultDepositApprovedSubject, PortalConstants.CEmailDefaultDepositApprovedBody); // Use this subject and body, if needed

            Response.Redirect(litSavedReturn.Text + "?"
                                + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                + PortalConstants.QSStatus + "=" + "Request returned to Internal Coordinator." + emailSent);
        }

        // User pressed History button. Fetch all the DepHistory rows for this Dep and fill a GridView.

        protected void btnHistory_Click(object sender, EventArgs e)
        {
            LoadAllDepHistorys();                                      // Fill the grid
            return;
        }

        // Package the work of the Save so that Submit and Revise can do it as well.
        //  1) Fetch the Dep row to be updated.
        //  2) Create a new DepHistory row and fill it.
        //  3) Update the Dep row with new State and Return Note.

        private void SaveDep(DepState nextState, string verb)
        {
            HttpCookie userInfoCookie = Request.Cookies[PortalConstants.CUserInfo]; // Ask for the User Info cookie
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    int depID = Convert.ToInt32(litSavedDepID.Text);            // Fetch ID of this Dep
                    Dep toUpdate = context.Deps.Find(depID);                    // Fetch the Dep row that we want to update
                    DepHistory hist = new DepHistory();                         // Get a place to build a new Request History row
                    toUpdate.ReturnNote = txtReturnNote.Text;                   // Fetch updated content of the note, if any
                    hist.ReturnNote = txtReturnNote.Text;                       // Preserve this note in the History trail
                    StateActions.CopyPreviousState(toUpdate, hist, verb);       // Create a Request History log row from "old" version of Request
                    StateActions.SetNewDepState(toUpdate, nextState, userInfoCookie[PortalConstants.CUserID], hist); // Write down our current State and authorship
                    context.DepHistorys.Add(hist);                              // Save new DepHistory row
                    context.SaveChanges();                                      // Commit the Add and Modify
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ReviewDeposit", "Unable to update Dep or DepHistory rows"); // Fatal error
                }
            }
            return;
        }

        // Based on the selected Deposit Type, enable and disable the appropriate panels on the display.

        void EnablePanels(DepType type)
        {
            pnlAmount.Visible = true;                                // Unconditionally visible for all Deposit types
            pnlDescription.Visible = true;
            pnlEstablishedBy.Visible = true;
            pnlEstablishedTime.Visible = true;
            pnlGLCode.Visible = true;
            pnlNotes.Visible = true;
            pnlReturnNote.Visible = true;
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

            txtEstablishedTime.Text = DateActions.LoadDateIntoTxt(record.CurrentTime);
            txtEstablishedBy.Text = record.CurrentUser.FullName;

            txtAmount.Text = record.Amount.ToString("C");

            txtDateOfDeposit.Text = DateActions.LoadDateIntoTxt(record.DateOfDeposit);

            txtDescription.Text = record.Description;

            txtTypeDescription.Text = EnumActions.GetEnumDescription((Enum)record.DepType); // Convert enum value to English value for display

            if (record.DestOfFunds == SourceOfExpFunds.Restricted) // If == the Source of Funds is a Project Class
            {
                rdoDestOfFunds.Items.FindByValue(PortalConstants.RDOFundsRestricted).Selected = true; // Select the button corresponding to restricted funds
                pnlProjectClass.Visible = true;
            }
            else if (record.DestOfFunds == SourceOfExpFunds.Unrestricted) // If == the Dest of Funds does not use a Project Class
            {
                rdoDestOfFunds.Items.FindByValue(PortalConstants.RDOFundsUnrestricted).Selected = true; // Select the other button
                pnlProjectClass.Visible = false;                    // Unrestricted means no Project Class so don't show the list
            }
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

            if (record.CurrentState == DepState.Returned)               // If == the Rqst has been returned, so a Return Note may be present
            {
                pnlReturnNote.Visible = true;                           // Make this panel visible
                txtReturnNote.Text = record.ReturnNote;                 // Copy the text of the Return Note
            }

            switch (record.SourceOfFunds)
            {
                case SourceOfDepFunds.NA:
                    {
                        rdoSourceOfFunds.Items.FindByValue(PortalConstants.RDONotApplicable).Selected = true; // Select the corresponding button
                        break;
                    }
                case SourceOfDepFunds.Entity:
                    {
                        rdoSourceOfFunds.Items.FindByValue(PortalConstants.RDOEntity).Selected = true; // Select the corresponding button
                        pnlEntity.Visible = true;                       // Make the drop down list appear
                        if (record.EntityID != null)                    // If != there is an Entity to display
                            txtEntity.Text = record.Entity.Name;
                        else if (record.EntityNeeded)                   // If true, PD asks us to create a new Entity
                            txtEntity.Text = "-- Please create a new " + lblEntity.Text + "--"; // Make that request
                        break;
                    }
                case SourceOfDepFunds.Individual:
                    {
                        rdoSourceOfFunds.Items.FindByValue(PortalConstants.RDOIndividual).Selected = true; // Select the corresponding button
                        pnlPerson.Visible = true;                       // Make the drop down list appear.
                        if (record.PersonID != null)                    // If != there is a Person to display
                            txtPerson.Text = record.Person.Name;
                        else if (record.PersonNeeded)                   // If true, PD asks us to create a new Person
                            txtPerson.Text = "-- Please create a new " + lblPerson.Text + " --"; // Make that request
                        break;
                    }
                default:
                    {
                        LogError.LogInternalError("EditDeposit", $"Invalid SourceOfDepFunds value '{record.SourceOfFunds.ToString()}' found in database"); // Fatal error
                        break;
                    }
            }

            SupportingActions.LoadDocs(RequestType.Deposit, record.DepID, lstSupporting, litDangerMessage); // Load them into list box

            return;
        }

        void LoadAllDepHistorys()
        {
            NavigationActions.LoadAllDepHistorys(Convert.ToInt32(litSavedDepID.Text), EDHistoryView); // Fill the list from the database
            return;
        }
    }
}
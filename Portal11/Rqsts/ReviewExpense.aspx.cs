using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Rqsts
{
    public partial class ReviewExpense : System.Web.UI.Page
    {
        // Review an Expense Request. Communication from Staff Dashboard is through Query Strings:
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

                QSValue expID = QueryStringActions.GetRequestID();      // Fetch the Expense ID
                string cmd = QueryStringActions.GetCommand();           // Fetch the command
                string ret = QueryStringActions.GetReturn();            // Fetch the return page

                // Fetch the Exp row and fill the page

                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    Exp exp = context.Exps.Find(expID.Int);             // Fetch Exp row by its key
                    if (exp == null)
                        LogError.LogInternalError("ReviewExpense", $"Invalid ExpID value '{expID}' could not be found in database"); // Log fatal error

                    // Stash these parameters into invisible literals on the current page.

                    litSavedCommand.Text = cmd;
                    litSavedExpID.Text = expID.String;
                    litSavedExpType.Text = EnumActions.GetEnumDescription(exp.ExpType);
                    litSavedProjectID.Text = exp.ProjectID.ToString();
                    litSavedProjectName.Text = exp.Project.Name;
                    litSavedProjectCode.Text = exp.Project.Code;
                    litSavedReturn.Text = ret;
                    litSavedRush.Text = exp.Rush.ToString();            // Whether or not it is a rush request
                    litSavedSubmitProjectRole.Text = exp.SubmitProjectRole.ToString(); // Project Role that submitted request

                    // If the user is actually in the process of revising this request, the Dashboard comes here anyway to keep the dashboard processing from getting too complex.
                    // All we have to do is act like the user pressed the "Revise" button and dispatch to Edit Deposit.

                    if (StateActions.RequestIsRevising(exp.CurrentState)) // If true, this request is being revised
                        DispatchRevise();                               // Break out of this stream of processing

                    // Load Exp fields into the page

                    EnablePanels(exp.ExpType);                          // Make the relevant panels visible
                    LoadPanels(exp);                                    // Move values from record to page

                    // See if this user has the role to Approve the Exp in its current state. If not, just let them view the Exp

                    if (!StateActions.UserCanApproveRequest(exp.CurrentState)) // If false user can not Approve this Exp. Disable functions
                    {
                        txtReturnNote.Enabled = false; btnReturnNoteClear.Visible = false; // Can see this note but not edit it
                        txtStaffNote.Enabled = false; btnStaffNoteClear.Visible = false; // Can see this note but not edit it
                        btnApprove.Enabled = false;                     // Cannot "Approve" the Exp
                        btnReturn.Enabled = false;                      // Cannot "Return" the Exp
                        btnRevise.Enabled = false;                      // Cannot "Revise" the Exp
                        litDangerMessage.Text = "You can view this Expense Request, but you cannot approve it."; // Explain that to user
                    }
                    else if (!StateActions.UserCanReviseRequest(exp.CurrentState)) // If false user cannot Revise this Dep. Disable function
                    {
                        btnRevise.Enabled = false;                      // Cannot "Revise" the Exp
                    }
                    btnViewLink.Enabled = false;                        // Kludge to get the View button disabled until row selected
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
            SupportingActions.DownloadDocsZip(RequestType.Expense, // What type of request
                                Convert.ToInt32(litSavedExpID.Text), // Which request ID
                                litSavedUserID.Text,                // Which user
                                litSavedProjectCode.Text,           // Which project code
                                litSDSuccess, litDangerMessage);    // Error reporting
        }

        protected void btnReturnNoteClear_Click(object sender, EventArgs e)
        {
            txtReturnNote.Text = "";
            litReturnNoteError.Visible = false;                     // Hide the error message, if any
        }

        protected void btnStaffNoteClear_Click(object sender, EventArgs e)
        {
            txtStaffNote.Text = "";
        }

        protected void gvEDHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                gvEDHistory.PageIndex = e.NewPageIndex;             // Propagate the desired page index
                NavigationActions.LoadAllExpHistorys(Convert.ToInt32(litSavedExpID.Text), gvEDHistory); // Fill the list from the database
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
        // Update the Exp and create a ExpHistory. Send next reviewer an email. Then head back to the relevant Dashboard.

        protected void btnApprove_Click(object sender, EventArgs e)
        {

            // If the Return Note field is non-blank, the user has probably pressed "Approve" by accident. What they probably want
            // is "Return." So report an error if Return Note is non-blank. But don't erase - the user might still want to press "Return."

            if (!string.IsNullOrEmpty(txtReturnNote.Text))          // If false text is erroneously present
            {
                litReturnNoteError.Text = PortalConstants.ReturnNotePresent; // Report the error
                litReturnNoteError.Visible = true;                  // Make the message visible
                return;                                             // Go back for more punishment
            }
            ExpState currentState = EnumActions.ConvertTextToExpState(litSavedState.Text); // Pull ToString version; convert to enum type
            ExpState nextState = StateActions.FindNextState(currentState, ReviewAction.Approve); // Now what?
            SaveExp(nextState,                                      // Advance request to this state
                    ReviewAction.Approve, "Approved");              // Note that we are approving the request. Update Exp; write new History row

            string emailSent = EmailActions.SendEmailToReviewer(    // Send email to next reviewer
                EnumActions.ConvertTextToBool(litSavedRush.Text),   // Whether the request is "rush"
                StateActions.UserRoleToProcessRequest(nextState),   // Who is in this role
                Convert.ToInt32(litSavedProjectID.Text),            // Request is associated with this project
                litSavedProjectName.Text,                           // Project has this name
                EnumActions.GetEnumDescription(RequestType.Expense), // This is an Expense Request
                litSavedExpType.Text,                               // Of this type, e.g., Payroll
                EnumActions.GetEnumDescription(nextState),          // Here is its next state
                PortalConstants.CEmailDefaultExpenseApprovedSubject, PortalConstants.CEmailDefaultExpenseApprovedBody); // Use this subject and body, if needed

            Response.Redirect(litSavedReturn.Text + "?" 
                                + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                + PortalConstants.QSStatus + "=" + "Request Appoved and Advanced to '" 
                                + EnumActions.GetEnumDescription(nextState) + "' status." + emailSent);
        }

        // User clicked Return. Set the state to Returned and process the request just like an Approve. Then notify the user of the glitch.

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtReturnNote.Text))          // If true text is missing
            {
                litReturnNoteError.Text = PortalConstants.ReturnNoteMissing; // Report the error
                litReturnNoteError.Visible = true;                  // Make the message visible
                return;                                             // Go back for more punishment
            }
            ExpState currentState = EnumActions.ConvertTextToExpState(litSavedState.Text); // Pull ToString version; convert to enum type
            ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedSubmitProjectRole.Text); // Pull string version, convert to enum type
            SaveExp(StateActions.FindNextState(currentState, ReviewAction.Return, projectRole), // Advance to the next state
                ReviewAction.Return, "Returned");                   // We are returning this request. Update Exp; write new History row

            string emailSent = EmailActions.SendEmailToReviewer(    // Send email to next reviewer
                EnumActions.ConvertTextToBool(litSavedRush.Text),   // Whether the request is "rush"
                StateActions.UserRoleToProcessRequest(ExpState.ReturnedToProjectDirector), // Who is in this role
                Convert.ToInt32(litSavedProjectID.Text),            // Request is associated with this project
                litSavedProjectName.Text,                           // Project has this name
                EnumActions.GetEnumDescription(RequestType.Expense), // This is an Expense Request
                litSavedExpType.Text,                               // Of this type, e.g., Payroll
                EnumActions.GetEnumDescription(ExpState.ReturnedToProjectDirector),  // Here is its next state
                PortalConstants.CEmailDefaultExpenseReturnedSubject, PortalConstants.CEmailDefaultExpenseReturnedBody); // Use this subject and body, if needed

            Response.Redirect(litSavedReturn.Text + "?"
                                + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                + PortalConstants.QSStatus + "=" + "Request Returned to originator." + emailSent);
        }

        // User clicked Revise. Set the state to Revising and save it. Then invoke Edit Expense to make the revisions. No email in this path.

        protected void btnRevise_Click(object sender, EventArgs e)
        {
            ExpState currentState = EnumActions.ConvertTextToExpState(litSavedState.Text); // Pull ToString version; convert to enum type  
            SaveExp(StateActions.FindNextState(currentState, ReviewAction.Revise),  // Find next state for revised request
                    ReviewAction.Revise, "Revising");               // We are revising Update Exp; write new History row
            DispatchRevise();                                       // Dispatch to Edit Deposit
        }

        void DispatchRevise()
        {

            // Find current role and map to the role to be used for editing.

            UserRole userRole = EnumActions.ConvertTextToUserRole(QueryStringActions.GetUserRole()); // Fetch User Role from UserInfo cookie and stash
            string reviseRole = RoleActions.GetRevisingRole(userRole).ToString(); // Find role that EditExpense should use during edits

            Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + reviseRole + "&"
                                            + PortalConstants.QSRequestID + "=" + litSavedExpID.Text + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandRevise + "&" // Start with an existing request
                                            + PortalConstants.QSReturn + "=" + litSavedReturn.Text);
        }

        // User pressed History button. Flip the GridView open or closed.

        protected void btnHistory_Click(object sender, EventArgs e)
        {
            if (pnlHistory.Visible)                                     // If true the History panel is visible
                pnlHistory.Visible = false;                             // Make it invisible
            else
            {
                NavigationActions.LoadAllExpHistorys(Convert.ToInt32(litSavedExpID.Text), gvEDHistory); // Fill the list from the database
                pnlHistory.Visible = true;                              // Make it visible
            }
            return;
        }

        // Package the work of the Save so that Submit and Return can do it as well.
        //  1) Fetch the Exp row to be updated.
        //  2) Create a new ExpHistory row and fill it.
        //  3) Update the Exp row with new State and Return Note.
        //  4) Flip the value of the ReviseUserRole under certain circumstances

        private void SaveExp(ExpState nextState, ReviewAction action, string verb)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    int expID = Convert.ToInt32(litSavedExpID.Text);            // Fetch ID of this Exp
                    Exp toUpdate = context.Exps.Find(expID);                    // Fetch the Exp row that we want to update
                    ExpHistory hist = new ExpHistory();                         // Get a place to build a new Request History row
                    toUpdate.ReturnNote = txtReturnNote.Text;                   // Fetch updated content of the note, if any
                    hist.ReturnNote = txtReturnNote.Text;                       // Preserve this note in the History trail
                    toUpdate.StaffNote = txtStaffNote.Text;                     // Fetch updated content of this note, if any
                    StateActions.CopyPreviousState(toUpdate, hist, verb);       // Create a Request History log row from "old" version of Request
                    StateActions.SetNewState(toUpdate, nextState, litSavedUserID.Text, hist); // Write down our current State and authorship

                    UserRole newReviewUserRole = RoleActions.UpdateReviseUserRole(action, EnumActions.ConvertTextToUserRole(litSavedUserRole.Text), toUpdate.ReviseUserRole);
                    toUpdate.ReviseUserRole = newReviewUserRole;                // Start or end a revise cycle by updating ReviseUserRole

                    context.ExpHistorys.Add(hist);                              // Save new ExpHistory row
                    context.SaveChanges();                                      // Commit the Add and Modify
                    return;
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ReviewExpense", "Unable to update Exp or ExpHistory rows"); // Fatal error
                }
            }
        }

        // Based on the selected Expense Type, enable and disable the appropriate panels on the display.

        void EnablePanels(ExpType type)
        {
            pnlAmount.Visible = true;                                           // Unconditionally visible for all Expense types
            pnlDescription.Visible = true;
            pnlEstablishedBy.Visible = true;
            pnlEstablishedTime.Visible = true;
            pnlGLCode.Visible = true;
            pnlNotes.Visible = true;
            pnlProjectClass.Visible = true;
            pnlReturnNote.Visible = true;
            if (RoleActions.UserRoleIsStaff(EnumActions.ConvertTextToUserRole(litSavedUserRole.Text))) // If true, user is a staff member
                pnlStaffNote.Visible = true;                                    // Staff users can see it
            else
                pnlStaffNote.Visible = false;                                   // Others don't see it
            pnlState.Visible = true;
            pnlSupporting.Visible = true;
            switch (type)
            {
                case ExpType.ContractorInvoice:
                    {
                        lblAmount.Text = "Dollar Amount";
                        pnlBeginningEnding.Visible = false;
                        pnlCards.Visible = false;
                        pnlContractQuestions.Visible = true;
                        pnlDateNeeded.Visible = false;
                        pnlDateOfInvoice.Visible = true;
                        pnlDeliveryInstructions.Visible = true; pnlDeliveryAddress.Visible = true;
                        pnlEntity.Visible = false;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true;
                        pnlPerson.Visible = true; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.Contractor);
                        pnlPOFulfillmentInstructions.Visible = false;
                        break;
                    }
                case ExpType.PEXCard:
                    {
                        lblAmount.Text = "Total Amount";
                        pnlBeginningEnding.Visible = false;
                        pnlCards.Visible = true;
                        pnlContractQuestions.Visible = false;
                        pnlDateNeeded.Visible = true;
                        pnlDateOfInvoice.Visible = false;
                        pnlDeliveryInstructions.Visible = false; pnlDeliveryAddress.Visible = false;
                        pnlEntity.Visible = false;
                        pnlGLCode.Visible = false;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = false;
                        pnlPerson.Visible = true; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.ResponsiblePerson);
                        pnlPOFulfillmentInstructions.Visible = false;
                        break;
                    }
                case ExpType.Payroll:
                    {
                        lblAmount.Text = "Gross Pay";
                        pnlBeginningEnding.Visible = true;
                        pnlCards.Visible = false;
                        pnlContractQuestions.Visible = false;
                        pnlDateNeeded.Visible = false;
                        pnlDateOfInvoice.Visible = false;
                        pnlDeliveryInstructions.Visible = true; pnlDeliveryAddress.Visible = true;
                        pnlEntity.Visible = false;
                        pnlGLCode.Visible = false;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true;
                        pnlPerson.Visible = false; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.Employee); // Only visible inside split
                        pnlProjectClass.Visible = false;                    // Only visible inside split
                        pnlPOFulfillmentInstructions.Visible = false;
                        pnlSplitPayroll.Visible = true;                     // Panel of split(s) is unconditionally visible - always at least one split row
                        break;
                    }
                case ExpType.PurchaseOrder:
                    {
                        lblAmount.Text = "Dollar Amount";
                        pnlBeginningEnding.Visible = false;
                        pnlCards.Visible = false;
                        pnlContractQuestions.Visible = false;
                        pnlDateNeeded.Visible = true;
                        pnlDateOfInvoice.Visible = false;
                        pnlDeliveryInstructions.Visible = true; pnlDeliveryAddress.Visible = true;
                        pnlEntity.Visible = true;
                        pnlGoods.Visible = true;
                        pnlPaymentMethod.Visible = true;
                        pnlPerson.Visible = false;
                        pnlPOFulfillmentInstructions.Visible = true;
                        break;
                    }
                case ExpType.Reimbursement:
                    {
                        lblAmount.Text = "Total Dollar Amount";
                        pnlBeginningEnding.Visible = false;
                        pnlCards.Visible = false;
                        pnlContractQuestions.Visible = false;
                        pnlDateNeeded.Visible = false;
                        pnlDateOfInvoice.Visible = false;
                        pnlDeliveryInstructions.Visible = true; pnlDeliveryAddress.Visible = true;
                        pnlEntity.Visible = false;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true;
                        pnlPerson.Visible = true; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.Recipient);
                        pnlPOFulfillmentInstructions.Visible = false;
                        break;
                    }
                case ExpType.VendorInvoice:
                    {
                        lblAmount.Text = "Dollar Amount";
                        pnlBeginningEnding.Visible = false;
                        pnlCards.Visible = false;
                        pnlContractQuestions.Visible = true;
                        pnlDateNeeded.Visible = false;
                        pnlDateOfInvoice.Visible = true;
                        pnlDeliveryInstructions.Visible = true; pnlDeliveryAddress.Visible = true;
                        pnlEntity.Visible = true;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true;
                        pnlPerson.Visible = false;
                        pnlPOFulfillmentInstructions.Visible = false;
                        break;
                    }
                default:
                    {
                        LogError.LogInternalError("ReviewExpense", $"Invalid ExpType value '{type}' in database"); // Fatal error
                        break;
                    }
            }
            return;
        }

        // Move values from the Request record into the Page. Only move the values that are "Visible" for this Request Type

        void LoadPanels(Exp record)
        {
            litSavedState.Text = record.CurrentState.ToString();            // Saved enum value for later "case"
            txtStateDescription.Text = RequestActions.AdjustCurrentStateDesc(record); // Produce nuanced version of current state

            txtAmount.Text = ExtensionActions.LoadDecimalIntoTxt(record.Amount);

            txtEstablishedTime.Text = DateActions.LoadDateTimeIntoTxt(record.CurrentTime);
            txtEstablishedBy.Text = record.CurrentUser.FullName;

            if (record.BeginningDate > SqlDateTime.MinValue)
                txtBeginningDate.Text = DateActions.LoadDateIntoTxt(record.BeginningDate);
            if (record.EndingDate > SqlDateTime.MinValue)
                txtEndingDate.Text = DateActions.LoadDateIntoTxt(record.EndingDate);

            txtNumberOfCards.Text = record.CardsQuantity.ToString();
            txtEachCard.Text = ExtensionActions.LoadDecimalIntoTxt(record.CardsValueEach);

            txtExistingContract.Text = EnumActions.GetEnumDescription(record.ContractComing);
            if (record.ContractReason != null)
                txtContractReason.Text = record.ContractReason;

            if (record.DateNeeded > SqlDateTime.MinValue)
                txtDateNeeded.Text = DateActions.LoadDateIntoTxt(record.DateNeeded);

            if (record.DateOfInvoice > SqlDateTime.MinValue)
                txtDateOfInvoice.Text = DateActions.LoadDateIntoTxt(record.DateOfInvoice);
            txtInvoiceNumber.Text = record.InvoiceNumber;

            if (record.Rush)
                pnlDeliveryModeRush.Visible = true;

            if (record.Description != null)
                txtDescription.Text = record.Description;

            txtTypeDescription.Text = EnumActions.GetEnumDescription(record.ExpType); // Convert enum value to English value for display

            string entityPersonAddress = "";                                // Hold address of Entity or Person for Delivery Address
            if (record.EntityID != null)
            {
                txtEntity.Text = record.Entity.Name;
                entityPersonAddress = record.Entity.Address;                // Save address, too, in case Delivery Address needs it
            }
            else if (record.EntityNeeded)                                   // If true, PD asks us to create a new Entity
                txtEntity.Text = "-- Please create a new " + lblEntity.Text + "--"; // Make that request

            if (record.GLCodeID != null)
                txtGLCode.Text = record.GLCode.Code;

            if (record.GoodsDescription != null)
                txtGoodsDescription.Text = record.GoodsDescription;
            if (record.GoodsSKU != null)
                txtGoodsSKU.Text = record.GoodsSKU;
            txtGoodsQuantity.Text = record.GoodsQuantity.ToString();
            txtGoodsCostPerUnit.Text = ExtensionActions.LoadDecimalIntoTxt(record.GoodsCostPerUnit);

            if (record.Notes != null)
                txtNotes.Text = record.Notes;

            if (pnlPaymentMethod.Visible)
            {
                txtPaymentMethod.Text = EnumActions.GetEnumDescription(record.PaymentMethod);
                if (record.PaymentMethod == PaymentMethod.EFT)                  // If == its an EFT so Delivery Instructions don't make sense
                {
                    pnlDeliveryInstructions.Visible = false;                    // Hide Delivery Instructions
                    pnlDeliveryAddress.Visible = false;                         // Hide Delivery Address
                }
            }

            if (record.PersonID != null)                                    // If != there is a Person to display
            {
                txtPerson.Text = record.Person.Name;
                entityPersonAddress = record.Person.Address;                // Save address, too, in case Delivery Address needs it
            }
            else if (record.PersonNeeded)                                   // If true, PD asks us to create a new Person
                txtPerson.Text = "-- Please create a new " + lblPerson.Text + " --"; // Make that request

            txtPOVendorMode.Text = EnumActions.GetEnumDescription(record.POVendorMode);

            txtProjectName.Text = record.Project.Name;

            if (record.ReturnNote != null)
                txtReturnNote.Text = record.ReturnNote;

            // For a while, we used an enum called SourceOfFunds to select various options, one of which, Restricted, 
            // required a Project Class. Then SourceOfFunds went away and everything became Restricted. To process legacy
            // rows correctly, handle cases where the Project Class is null or wrong, supplying a default vaule for Project Class.

            try
            {
                txtProjectClass.Text = record.ProjectClass.Name;            // Show the Project Class
            }
            catch
            {
                txtProjectClass.Text = EnumActions.GetEnumDescription(SourceOfExpFunds.Unrestricted); // Supply default value
            }

            txtStaffNote.Text = record.StaffNote;                       // Load value, if any

            // Deal with splits. If Payroll, the split window is unconditionally open. Otherwise, its only open if we find a split in the database.

            if (record.ExpType == ExpType.Payroll)                      // If == this is a payroll request. Special treatment
            {
                if (!SplitActions.LoadSplitRowsForView(RequestType.Expense, record.ExpID, gvSplitPayroll)) // If false a "legacy" payroll request with no split row
                    LoadFirstSplitPayrollRow(record.Amount, record.ProjectClassID, record.PersonID); // Synthesize a first (and only) row from legacy data
            }
            else
            {
                if (SplitActions.LoadSplitRowsForView(RequestType.Expense, record.ExpID, gvSplitGL)) // If true, GLCodeSplits existed and were loaded
                    EnergizeSplitGL();                            // Adjust page to accommodate split gridview
            }

            // Delivery Mode and PO Delivery Mode both use Delivery Instructions and Delivery Address. Delivery Address only makes sense in some modes. 
            // EditExpense has packed this information and saved it in the database. Now we have to unpack it.

            switch (record.ExpType)
            {
                case ExpType.PEXCard:                                   // PEX card has no Delivery Mode, so no Delivery Address
                    {
                        break;    
                    }
                case ExpType.PurchaseOrder:                             // PO has its own PO Delivery Mode
                    {
                        txtDeliveryInstructions.Text = EnumActions.GetEnumDescription(record.PODeliveryMode); // Fill Delivery Instructions
                        if (record.PODeliveryMode == PODeliveryMode.DeliverAddress) // If == deliver to a specific address
                            txtDeliveryAddress.Text = record.DeliveryAddress; // Fill from database
                        break;
                    }
                default:                                                // Everybody else takes some digging
                    {
                        txtDeliveryInstructions.Text = EnumActions.GetEnumDescription(record.DeliveryMode); // Fill Delivery Instructions
                        switch (record.DeliveryMode)                    // Break out by Delivery Mode
                        {
                            case DeliveryMode.MailAddress:
                                {
                                    txtDeliveryAddress.Text = record.DeliveryAddress; // Delivery to a specified address. Fill from database
                                    break;
                                }
                            case DeliveryMode.MailPayee:
                                {
                                    txtDeliveryAddress.Text = entityPersonAddress;    // Deliver to an Entity or Person's address on file
                                    break;
                                }
                            case DeliveryMode.Pickup:
                            default:
                                pnlDeliveryAddress.Visible = false;     // No delivery address to display, so hide the text box
                                break;
                        }
                        break;
                    }
            }

            SupportingActions.LoadDocs(RequestType.Expense, record.ExpID, lstSupporting, litDangerMessage); // Load into list box
            return;
        }

        // A payroll without splits - something from a prior version of the Portal. Here, we're not invoked by a "Split" button. 
        // Instead, an old request gets a single row.

        void LoadFirstSplitPayrollRow(decimal amount = 0, int? ProjectClassID = null, int? PersonID = null)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // Case 1) This request has never had splits before.

                List<rowGLCodeSplit> rows = new List<rowGLCodeSplit>(); // Create a list of rows for the gridview
                rowGLCodeSplit row1 = new rowGLCodeSplit()              // Prime the first row of the grid from "parent" fields of the page
                {
                    TotalRows = 1,                                      // This row only
                    Amount = ExtensionActions.LoadDecimalIntoTxt(amount),
                    SelectedProjectClassID = ProjectClassID.ToString(),
                    SelectedPersonID = PersonID.ToString()
                };
                rows.Add(row1);                                         // Add the first row

                gvSplitPayroll.DataSource = rows;                       // Give the rows to the gridview
                gvSplitPayroll.DataBind();                              // And bind them to display the rows, firing RowDataBound for each row
            }
            return;
        }

        // We have a full split gridview. Now adjust the operation of the page to process splits

        void EnergizeSplitGL()
        {
            pnlProjectClass.Visible = false;                        // Can't see "Project Class" field any more
//            pnlAmount.Visible = false;                              // Can't see "Total Dollar Amount" field any more
            pnlGLCode.Visible = false;                              // Can't see "Expense Account" drop down list any more
            pnlsplitGL.Visible = true;                         // Turn on the grid for splits
            return;
        }
    }
}
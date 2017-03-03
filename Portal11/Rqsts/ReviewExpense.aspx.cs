﻿using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
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
                QSValue expID = QueryStringActions.GetRequestID();      // Fetch the Expense ID
                string cmd = Request.QueryString[PortalConstants.QSCommand]; // Fetch the command
                string ret = Request.QueryString[PortalConstants.QSReturn]; // Fetch the return page
                if (expID.Int == 0 || cmd != PortalConstants.QSCommandReview || ret == "") // If null or blank, this form invoked incorrectly
                    LogError.LogQueryStringError("ReviewExpense", "Invalid Query String 'Command' or 'ExpID'"); // Log fatal error

                HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Fetch user info cookie for current user
                litSavedUserRole.Text = userInfoCookie[PortalConstants.CUserRole]; //Fetch user role, save for later

                // Fetch the Exp row and fill the page

                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    Exp exp = context.Exps.Find(expID.Int);             // Fetch Exp row by its key
                    if (exp == null)
                        LogError.LogInternalError("ReviewExpense", string.Format(
                            "Invalid ExpID value '{0}' could not be found in database", expID.String)); // Log fatal error

                    EnablePanels(exp.ExpType);                          // Make the relevant panels visible
                    LoadPanels(exp);                                    // Move values from record to page

                    // See if this user has the role to Approve the Exp in its current state. If not, just let them view the Exp

                    if (!StateActions.UserCanApproveRequest(exp.CurrentState)) // If false user can not Approve this Exp. Disable functions
                    {
                        txtReturnNote.Enabled = false;                  // Can see this note but not edit it
                        txtStaffNote.Enabled = false;                   // Can see this note but not edit it
                        btnApprove.Enabled = false;                     // Cannot "Approve" the Exp
                        btnReturn.Enabled = false;                      // Cannot "Return" the Exp
                        litDangerMessage.Text = "You can view this Expense Request, but you cannot approve it."; // Explain that to user
                    }

                    // Stash these parameters into invisible literals on the current page.

                    litSavedCommand.Text = cmd;
                    litSavedExpID.Text = expID.String;
                    litSavedProjectID.Text = exp.ProjectID.ToString();
                    litSavedReturn.Text = ret;
                    litSavedRush.Text = exp.Rush.ToString();            // Whether or not it is a rush request
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
                LoadAllExpHistorys();                                  // Re-fill the GridView control
                EDHistoryView.SelectedIndex = -1;                     // No row currently selected
            }
            return;
        }

        // View a Supporting Document We replicate the logic from the EditExpense page and download the selected Supporting Document file.
        // This case is simpler than EditExpense because all the Docs are "permanent," described in SupportingDoc rows.

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
        // Update the Exp and create a ExpHistory. Send next reviewer an email. Then head back to the relevant Dashboard.

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            ExpState currentState = EnumActions.ConvertTextToExpState(litSavedState.Text); // Pull ToString version; convert to enum type
            ExpState nextState = StateActions.FindNextState(currentState); // Now what?
            int projectID = new int(); string projectName = "";
            SaveExp(nextState, "Approved", out projectID, out projectName); // Update Exp; write new History row

            string emailSent = EmailActions.SendEmailToReviewer(    // Send email to next reviewer
                EnumActions.ConvertTextToBool(litSavedRush.Text),   // Whether the request is "rush"
                StateActions.UserRoleToApproveRequest(nextState),   // Who is in this role
                projectID,                                          // Request is associated with this project
                projectName,                                        // Project has this name
                EnumActions.GetEnumDescription(RequestType.Expense), // This is an Expense Request
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
            int projectID = new int(); string projectName = "";
            SaveExp(ExpState.Returned, "Returned", out projectID, out projectName); // Update Exp; write new History row

            string emailSent = EmailActions.SendEmailToReviewer(    // Send email to next reviewer
                EnumActions.ConvertTextToBool(litSavedRush.Text),   // Whether the request is "rush"
                StateActions.UserRoleToApproveRequest(ExpState.Returned), // Who is in this role
                projectID,                                          // Request is associated with this project
                projectName,                                        // Project has this name
                EnumActions.GetEnumDescription(RequestType.Expense), // This is an Expense Request
                EnumActions.GetEnumDescription(ExpState.Returned),  // Here is its next state
                PortalConstants.CEmailDefaultExpenseReturnedSubject, PortalConstants.CEmailDefaultExpenseReturnedBody); // Use this subject and body, if needed

            Response.Redirect(litSavedReturn.Text + "?"
                                + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                + PortalConstants.QSStatus + "=" + "Request Returned to Project Director." + emailSent);
        }

        // User pressed History button. Fetch all the ExpHistory rows for this Exp and fill a GridView.

        protected void btnHistory_Click(object sender, EventArgs e)
        {
            LoadAllExpHistorys();                                      // Fill the grid
            return;
        }

        // Package the work of the Save so that Submit and Revise can do it as well.
        //  1) Fetch the Exp row to be updated.
        //  2) Create a new ExpHistory row and fill it.
        //  3) Update the Exp row with new State and Return Note.

        private void SaveExp(ExpState nextState, string verb, out int projectID, out string projectName)
        {
            HttpCookie userInfoCookie = Request.Cookies[PortalConstants.CUserInfo]; // Ask for the User Info cookie
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    int expID = Convert.ToInt32(litSavedExpID.Text);            // Fetch ID of this Exp
                    Exp toUpdate = context.Exps.Find(expID);                        // Fetch the Exp row that we want to update
                    ExpHistory hist = new ExpHistory();                         // Get a place to build a new Request History row
                    hist.ReturnNote = toUpdate.ReturnNote;                      // Preserve former value of the note
                    toUpdate.ReturnNote = txtReturnNote.Text;                   // Fetch updated content of the note, if any
                    toUpdate.StaffNote = txtStaffNote.Text;                     // Fetch updated content of this note, if any
                    StateActions.CopyPreviousState(toUpdate, hist, verb);       // Create a Request History log row from "old" version of Request
                    StateActions.SetNewExpState(toUpdate, nextState, userInfoCookie[PortalConstants.CUserID], hist);
                                                                                // Write down our current State and authorship
                    context.ExpHistorys.Add(hist);                              // Save new ExpHistory row
                    context.SaveChanges();                                      // Commit the Add and Modify
                    projectID = (int)toUpdate.ProjectID;                        // Report Project ID to caller
                    projectName = toUpdate.Project.Name;                        // Report Project Name to caller
                    return;
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ReviewExpense", "Unable to update Exp or ExpHistory rows"); // Fatal error
                }
            }
            projectID = 0; projectName = "";                                    // Control never reaches this point, so irrelevant
        }

        // Based on the selected Expense Type, enable and disable the appropriate panels on the display.

        void EnablePanels(ExpType type)
        {
            pnlAmount.Visible = true;                                           // Unconditionally visible for all Expense types
            pnlDescription.Visible = true;
            pnlEstablishedBy.Visible = true;
            pnlEstablishedTime.Visible = true;
            pnlFunds.Visible = true;
            pnlGLCode.Visible = true;
            pnlNotes.Visible = true;
            pnlReturnNote.Visible = true;
            if ((litSavedUserRole.Text == UserRole.Project.ToString())
                || (litSavedUserRole.Text == UserRole.Auditor.ToString()))      // If true, user is a Project Director or Auditor
                pnlStaffNote.Visible = false;                                   // They don't see Staff Note
            else
                pnlStaffNote.Visible = true;                                    // But Staff users can see it
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
                        pnlDeliveryInstructions.Visible = true; pnlPODeliveryInstructions.Visible = false; pnlDeliveryAddress.Visible = true;
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
                        pnlDeliveryInstructions.Visible = true; pnlPODeliveryInstructions.Visible = false; pnlDeliveryAddress.Visible = true;
                        pnlEntity.Visible = false;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = false;
                        pnlPerson.Visible = true; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.ResponsiblePerson);
                        pnlPOFulfillmentInstructions.Visible = false;
                        break;
                    }
                case ExpType.Paycheck:
                    {
                        lblAmount.Text = "Gross Pay";
                        pnlBeginningEnding.Visible = true;
                        pnlCards.Visible = false;
                        pnlContractQuestions.Visible = false;
                        pnlDateNeeded.Visible = false;
                        pnlDateOfInvoice.Visible = false;
                        pnlDeliveryInstructions.Visible = true; pnlPODeliveryInstructions.Visible = false; pnlDeliveryAddress.Visible = true;
                        pnlEntity.Visible = false;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true;
                        pnlPerson.Visible = true; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.Employee);
                        pnlPOFulfillmentInstructions.Visible = false;
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
                        pnlDeliveryInstructions.Visible = false; pnlPODeliveryInstructions.Visible = true; pnlDeliveryAddress.Visible = true;
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
                        pnlDeliveryInstructions.Visible = true; pnlPODeliveryInstructions.Visible = false; pnlDeliveryAddress.Visible = true;
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
                        pnlDeliveryInstructions.Visible = true; pnlPODeliveryInstructions.Visible = false; pnlDeliveryAddress.Visible = true;
                        pnlEntity.Visible = true;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true;
                        pnlPerson.Visible = false;
                        break;
                    }
                default:
                    {
                        LogError.LogInternalError("ReviewExpense", string.Format("Invalid ExpType value '{0}' in database",
                            type.ToString())); // Fatal error
                        break;
                    }
            }
            return;
        }

        // Move values from the Request record into the Page. Only move the values that are "Visible" for this Request Type

        void LoadPanels(Exp record)
        {
            txtStateDescription.Text = EnumActions.GetEnumDescription(record.CurrentState); // Convert enum value to English
            litSavedState.Text = record.CurrentState.ToString();            // Saved enum value for later "case"

            txtAmount.Text = record.Amount.ToString("C");

            txtEstablishedTime.Text = DateActions.LoadDateIntoTxt(record.CurrentTime);
            txtEstablishedBy.Text = record.CurrentUser.FullName;

            if (record.BeginningDate > SqlDateTime.MinValue)
                txtBeginningDate.Text = DateActions.LoadDateIntoTxt(record.BeginningDate);
            if (record.EndingDate > SqlDateTime.MinValue)
                txtEndingDate.Text = DateActions.LoadDateIntoTxt(record.EndingDate);

            txtNumberOfCards.Text = record.CardsQuantity.ToString();
            txtEachCard.Text = record.CardsValueEach.ToString("C");

            txtExistingContract.Text = EnumActions.GetEnumDescription(record.ContractComing);
            if (record.ContractReason != null)
                txtContractReason.Text = record.ContractReason;

            if (record.DateNeeded > SqlDateTime.MinValue)
                txtDateNeeded.Text = DateActions.LoadDateIntoTxt(record.DateNeeded);

            if (record.DateOfInvoice > SqlDateTime.MinValue)
                txtDateOfInvoice.Text = DateActions.LoadDateIntoTxt(record.DateOfInvoice);

            txtDeliveryInstructions.Text = EnumActions.GetEnumDescription(record.DeliveryMode);
            if (record.Rush)
                txtDeliveryInstructionsRush.Visible = true;
            txtPODeliveryInstructions.Text = EnumActions.GetEnumDescription(record.PODeliveryMode);
            if (record.DeliveryAddress != null)
                txtDeliveryAddress.Text = record.DeliveryAddress;

            if (record.Description != null)
                txtDescription.Text = record.Description;

            txtTypeDescription.Text = EnumActions.GetEnumDescription(record.ExpType); // Convert enum value to English value for display

            if (record.GLCodeID != null)
                txtGLCode.Text = record.GLCode.Code;

            if (record.GoodsDescription != null)
                txtGoodsDescription.Text = record.GoodsDescription;
            if (record.GoodsSKU != null)
                txtGoodsSKU.Text = record.GoodsSKU;
            txtGoodsQuantity.Text = record.GoodsQuantity.ToString();
            txtGoodsCostPerUnit.Text = record.GoodsCostPerUnit.ToString("C");

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
                txtPerson.Text = record.Person.Name;
            else if (record.PersonNeeded)                                   // If true, PD asks us to create a new Person
                txtPerson.Text = "-- Please create a new " + lblPerson.Text + " --"; // Make that request

            txtPOVendorMode.Text = EnumActions.GetEnumDescription(record.POVendorMode);

            txtProjectName.Text = record.Project.Name;

            if (record.ReturnNote != null)
                txtReturnNote.Text = record.ReturnNote;

            if (record.EntityID != null)
                txtEntity.Text = record.Entity.Name;
            else if (record.EntityNeeded)                               // If true, PD asks us to create a new Entity
                txtEntity.Text = "-- Please create a new " + lblEntity.Text + "--"; // Make that request

            // Source of Funds. At a minimum, use radio buttons to show source. If it's Restricted, also show the Project Class.

            switch (record.SourceOfFunds)
            {
                case SourceOfExpFunds.NA:
                {
                    rdoSourceOfFunds.SelectedValue = SourceOfExpFunds.NA.ToString(); // Light the Not Applicable button
                    pnlProjectClass.Visible = false;                    // No need to display Project Class, since there isn't one
                    break;
                }
                case SourceOfExpFunds.Restricted:
                {
                    rdoSourceOfFunds.SelectedValue = SourceOfExpFunds.Restricted.ToString(); // Light the Restricted button
                    txtProjectClass.Text = record.ProjectClass.Name;    // Show the Project Class
                    break;
                }
                case SourceOfExpFunds.Unrestricted:
                {
                    rdoSourceOfFunds.SelectedValue = SourceOfExpFunds.Unrestricted.ToString(); // Light the Unrestricted button
                    pnlProjectClass.Visible = false;                    // No need to display Project Class, since there isn't one
                    break;
                }
                default:
                {
                    LogError.LogInternalError("ReviewExpense", string.Format(
                        "Invalid SourceOfFunds value '{0}' found in Exp record", record.SourceOfFunds)); // Log fatal error
                    break;
                }
            }

            txtStaffNote.Text = record.StaffNote;                       // Load value, if any

            if (SplitActions.LoadSplitRowsForView(record.ExpID, gvExpSplit)) // If true, ExpSplits existed and were loaded into the split gridview
                EnergizeSplit();                                        // Adjust page to accommodate split gridview

            SupportingActions.LoadDocs(RequestType.Expense, record.ExpID, lstSupporting, litDangerMessage); // Load into list box
            return;
        }

        void LoadAllExpHistorys()
        {
            NavigationActions.LoadAllExpHistorys(Convert.ToInt32(litSavedExpID.Text), EDHistoryView); // Fill the list from the database
            return;
        }

        // We have a full split gridview. Now adjust the operation of the page to process splits

        void EnergizeSplit()
        {
            pnlProjectClass.Visible = false;                        // Can't see "Project Class" field any more
//            pnlAmount.Visible = false;                              // Can't see "Total Dollar Amount" field any more
            pnlGLCode.Visible = false;                              // Can't see "Expense Account" drop down list any more
            pnlExpenseSplit.Visible = true;                         // Turn on the grid for splits
            return;
        }      
    }
}
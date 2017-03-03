using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Rqsts
{
    public partial class EditExpense : System.Web.UI.Page
    {
        // Create a new Expense Request, Copy (and Edit) an existing Expense Request, or Edit an existing Expense Request. 
        // Communication from Project Dashboard is through Query Strings:
        //      UserID - the database ID of the Project Director making these changes (Required)
        //      ProjectID - the database ID of the Project that owns this Expense (Required)
        //      RequestID - the database ID of the existing Expense Request, if any, to process
        //      Command - "New," "Copy," "Edit," "View," or "Revise" (Required)

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // Fetch and validate the four Query String parameters. Carefully convert the integer parameters to integers.

                string userID = QueryStringActions.GetUserID();         // First parameter - User ID. If absent, check UserInfoCookie
                QSValue projectID = QueryStringActions.GetProjectID();  // Second parameter - Project ID. If absent, check ProjectInfoCookie
                QSValue expID = QueryStringActions.GetRequestID();      // Third parameter - Expense ID. If absent, must be a New command
                string cmd = QueryStringActions.GetCommand();           // Fourth parameter - Command. Must be present

                // Find the User's role on the Project

                HttpCookie projectInfoCookie = Request.Cookies[PortalConstants.CProjectInfo]; // Find the Project Info cookie
                string projRole = projectInfoCookie[PortalConstants.CProjectRole]; // Fetch user's Project Role from cookie
                if (projRole == "")                                     // If == that's blank. We have an error
                    LogError.LogQueryStringError("EditExpense", "Unable to find Project Role in Project Info Cookie. User does not have a project"); // Fatal error

                // Stash these parameters into invisible literals on the current page.

                litSavedUserID.Text = userID;
                litSavedProjectID.Text = projectID.String;
                litSavedExpID.Text = "";                                // Assume New or Copy - do not modify an existing row, but add a new one
                litSavedCommand.Text = cmd;
                litSavedProjectRole.Text = projRole;                    // Save in a faster spot for later

                SupportingActions.CleanupTemp(userID, litDangerMessage); // Cleanup supporting docs from previous executions for this user

                // Dispatch by Command

                switch (cmd)
                {
                    case PortalConstants.QSCommandCopy:                 // Process a "Copy" command. Read existing request, save it in new row
                        {
                            expID.Int = CopyExp(expID.Int);             // Copy the source Exp into a new destination Exp
                            expID.String = expID.Int.ToString();        // Also update the string version of the Exp ID
                            if (expID.Int == 0)                         // If == the copy process failed
                                LogError.LogInternalError("EditExpense", "Unable to copy existing Expense Request"); // Log fatal error

                            goto case PortalConstants.QSCommandEdit;    // Now edit the destination Exp
                        }
                    case PortalConstants.QSCommandEdit:                 // Process an "Edit" command. Read existing Expense, save it in same row
                        {

                            // Fetch the row from the database. Set the radio button corresponding to our Expense type,
                            // then disable the radio button group so the user can't change the Expense type.
                            // Make appropriate panels visible, then fill in the panels using data rom the existing Expense. Lotta work!

                            if (expID.Int == 0)                     // If == cannot edit a missing Exp
                                LogError.LogQueryStringError("EditExpense", "Missing Query String 'RequestID'"); // Log fatal error

                            litSavedExpID.Text = expID.String;      // Remember to write record back to its original spot, i.e., modify the row
                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                Exp exp = context.Exps.Find(expID.Int); // Fetch Exp row by its key
                                if (exp == null)
                                    LogError.LogInternalError("EditExpense", $"Unable to locate ExpenseID '{expID.String}' in database"); // Log fatal error

                                // Note: The rdo displays TEXT that matches the Description of each ExpType, but the VALUE contains the enumerated ExpType
                                rdoExpType.SelectedValue = exp.ExpType.ToString(); // Select the button corresponding to our type
                                rdoExpType.Enabled = false;         // Disable the control - cannot change type of existing Expense
                                EnablePanels(exp.ExpType);          // Make the relevant panels visible
                                LoadPanels(exp);                    // Fill in the visible panels from the Expense
                                if (SplitActions.LoadSplitRows(exp.ExpID, gvExpSplit)) // If true, GLCodeSplits existed and were loaded into the split gridview
                                    EnergizeSplit();                // Adjust page to accommodate split gridview

                                LoadSupportingDocs(exp);            // Fill in the Supporting Docs list
                            }
                            litSuccessMessage.Text = "Expense Request is ready to edit";
                            break;
                        }
                    case PortalConstants.QSCommandNew:              // Process a "New" command. Create new, empty Expense, save it in new row
                        {

                            // We don't have to "fill" any of the controls on the page, just use them in their initial state.
                            // The radio button group for Expense type wakes up "enabled" with nothing selected. No other controls are visible.
                            // When the user selects a Expense type, we'll use that click to make the appropriate panels visible.

                            ExpState newState = StateActions.FindUnsubmittedExpState(projRole); // Figure out which flavor of Unsubmitted state
                            litSavedStateEnum.Text = newState.ToString(); // Stash actual enum value of ExpState
                            txtState.Text = EnumActions.GetEnumDescription(newState); // Display "English" version of enum

                            litSuccessMessage.Text = "New Expense Request is ready to edit";

                            break;
                        }
                    case PortalConstants.QSCommandView:             // Process an "View" command. Read existing Expense, disable edit and save
                        {
                            litSavedExpID.Text = expID.String;      // Remember the ID of the row that we will view

                            // Fetch the row from the database. Set the radio button corresponding to our Expense type,
                            // then disable the radio button group so the user can't change the Expense type.
                            // Make appropriate panels visible, then fill in the panels using data from the existing Expense. 
                            // Set everything to readonly to prevent editing. Disable Save and Submit. Lotta work!

                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                Exp exp = context.Exps.Find(expID.Int); // Fetch Exp row by its primary key
                                if (exp == null)
                                    LogError.LogInternalError("EditExpense", $"Unable to find Expense ID '{expID.String}' in database"); // Fatal error

                                rdoExpType.SelectedValue = exp.ExpType.ToString(); // Select the button corresponding to our type
                                rdoExpType.Enabled = false;         // Disable the control - cannot change type of existing Expense
                                EnablePanels(exp.ExpType);          // Make the relevant panels visible
                                LoadPanels(exp);                    // Fill in the visible panels from the Expense
                                if (SplitActions.LoadSplitRows(exp.ExpID, gvExpSplit)) // If true, GLCodeSplits existed and were loaded into the split gridview
                                    EnergizeSplit();                // Adjust page to accommodate split gridview

                                LoadSupportingDocs(exp);            // Fill in the Supporting Docs - it takes extra work
                                ReadOnlyControls();                 // Make all controls readonly for viewing

                                // Adjust the button settings for the View function

                                btnCancel.Text = "Done";            // Provide a friendly way out of this page
                                btnSave.Enabled = false;            // No "Save" for you!
                                btnSubmit.Enabled = false;
                                if (exp.CurrentState == ExpState.Returned) // If == viewing a returned Exp - special case
                                    btnRevise.Visible = true;       // Turn on the "Revise" button - specific to this case
                                btnShowHistory.Visible = true;      // We can show the history so make button visible 
                                pnlAdd.Visible = false;             // Can't figure out how to disable Add button, so make it invisible
                                btnRem.Visible = false;             // Remove button on Supporting Docs is also invisible
                                Page.Title = "View Expense Request";    // Note that we are viewing, not editing
                                litSuccessMessage.Text = "Existing Expense Request is ready to view";
                            }
                            break;
                        }
                    default:
                        LogError.LogQueryStringError("EditExpense", "Invalid Query String 'Command'"); // Log fatal error
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

        // User pressed a radio button to select a Expense type. Each Expense type uses a different combination of panels.
        // Make visible the appropriate panels and fill the appropriate dropdown lists. Then do some housekeeping to get the
        // appropriate defaults applied. There ought to be a better way to do this, but I can't find it.

        protected void rdoExpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExpType selected = EnumActions.ConvertTextToExpType(rdoExpType.SelectedValue); // Fetch selected button, convert value to enum
            EnablePanels(selected);                                     // Turn on/off controls based on selected Exp Type
            FillDropDownLists();                                        // Load dropdown lists with unselected database values
            rdoPaymentMethod.SelectedValue = PaymentMethod.Check.ToString(); // Reestablish default payment method. Every Type accepts checks
            if (pnlPOFulFillmentInstructions.Visible)                   // If true Vendor Mode radio buttons are visible. Apply the current setting
                rdoPOVendorMode_SelectedIndexChanged(0, e);             // React to the setting, which may involve hiding the Vendor DDL again
            return;
        }

        // User is in the midst of entering quantity, cost per item and amount. Try to calculate the amount for them.

        protected void txtNumberOfCards_TextChanged(object sender, EventArgs e)
        {
            ExtensionActions.FillQCA(txtNumberOfCards, txtEachCard, txtAmount); // Try to get cute and fill a missing field
        }

        protected void txtGoodsCostPerUnit_TextChanged(object sender, EventArgs e)
        {
            ExtensionActions.FillQCA(txtGoodsQuantity, txtGoodsCostPerUnit, txtAmount); // Try to get cute and fill a missing field
        }

        // Text boxes for date values

        protected void txtBeginningDate_TextChanged(object sender, EventArgs e)
        {
            DateActions.ValidateDateInput(txtBeginningDate, "Beginning Date", litDangerMessage); // Do the heavy lifting
            return;
        }

        protected void txtEndingDate_TextChanged(object sender, EventArgs e)
        {
            DateActions.ValidateDateInput(txtEndingDate, "Ending Date", litDangerMessage); // Do the heavy lifting
            return;
        }

        protected void txtDateOfInvoice_TextChanged(object sender, EventArgs e)
        {
            DateActions.ValidateDateInput(txtDateOfInvoice, "Invoice Date", litDangerMessage); // Do the heavy lifting
            return;
        }

        protected void txtDateNeeded_TextChanged(object sender, EventArgs e)
        {
            DateActions.ValidateDateInput(txtDateNeeded, "Date Needed", litDangerMessage); // Do the heavy lifting
            return;
        }

        // The user has clicked on a date or range in a calendar. Let's see what's up! Suck out the selected date and hide the calendar.
        // BeginningDate and EndingDate are conjoined. Selecting a date or range in BeginningDate primes the date in EndingDate.

        protected void calBeginningDate_SelectionChanged(object sender, EventArgs e)
        {
            DateTime start = calBeginningDate.SelectedDate;
            txtBeginningDate.Text = DateActions.LoadDateIntoTxt(start);              // Convert date to text 
            DateTime last = start;
            foreach (DateTime day in calBeginningDate.SelectedDates)
            {
                last = day;
            }
            txtEndingDate.Text = DateActions.LoadDateIntoTxt(last);                 // Place the last selected date into the "To" text box
            calEndingDate.SelectedDate = last;                                      // Also place the date into the "To" calendar
            calEndingDate.VisibleDate = last;                                       // And position "To" calendar to that date
            calBeginningDate.Visible = false;                                       // One click and the calendar is gone
            return;
        }

        protected void calEndingDate_SelectionChanged(object sender, EventArgs e)
        {
            calSelectionChanged(txtEndingDate, calEndingDate);                  // Do the heavy lifting
            return;
        }

        protected void calDateNeeded_SelectionChanged(object sender, EventArgs e)
        {
            calSelectionChanged(txtDateNeeded, calDateNeeded);                  // Do the heavy lifting
            return;
        }

        protected void calDateOfInvoice_SelectionChanged(object sender, EventArgs e)
        {
            calSelectionChanged(txtDateOfInvoice, calDateOfInvoice);            // Do the heavy lifting
            return;
        }

        void calSelectionChanged(TextBox txt, System.Web.UI.WebControls.Calendar cal)
        {
            txt.Text = DateActions.LoadDateIntoTxt(cal.SelectedDate);               // Convert date to text
            cal.Visible = false;                                                    // One click and the calendar is gone
            return;
        }

        // Toggle the visibility of a calendar. If becoming visible, load with date from text box, if any

        protected void btnBeginningDate_Click(object sender, EventArgs e)
        {
            calClick(txtBeginningDate, calBeginningDate);                       // Do the heavy lifting in common code
            return;
        }

        protected void btnEndingDate_Click(object sender, EventArgs e)
        {
            calClick(txtEndingDate, calEndingDate);                             // Do the heavy lifting in common code
            return;
        }

        protected void btnDateNeeded_Click(object sender, EventArgs e)
        {
            calClick(txtDateNeeded, calDateNeeded);                             // Do the heavy lifting in common code
            return;
        }

        protected void btnDateOfInvoice_Click(object sender, EventArgs e)
        {
            calClick(txtDateOfInvoice, calDateOfInvoice);                       // Do the heavy lifting in common code
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

        // Contract Questions include two sets of radio buttons that turn various things on and off. We handle the case in which
        // neither the Yes nor No buttons are pressed, though we don't let a record in that state to be saved.

        protected void rdoContractOnFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (rdoContractOnFile.SelectedValue)
            {
                case PortalConstants.CYes:
                    {
                        pnlContractReason.Visible = true;                   // Contract Reason field appears
                        pnlContractComing.Visible = false;                  // Contract Coming question disappears
                        break;
                    }
                case PortalConstants.CNo:
                    {
                        pnlContractReason.Visible = false;                  // Contract Reason field disappears
                        pnlContractComing.Visible = true;                   // Contract Coming question appears
                        rdoContractComing.SelectedIndex = -1;               // with no answer selected
                        break;
                    }
                default:                                                    // No button pressed
                    {
                        pnlContractReason.Visible = false;                  // Contract Reason field disappears
                        pnlContractComing.Visible = false;                  // Contract Coming question disappears
                        break;
                    }
            }
            litContractNeeded.Visible = false;                              // With responses to Contract Coming question
            litContractNone.Visible = false;                                //  hidden
            return;
        }

        protected void rdoContractComing_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (rdoContractComing.SelectedValue)
            {
                case PortalConstants.CYes:
                    {
                        litContractNeeded.Visible = true;
                        litContractNone.Visible = false;
                        break;
                    }
                case PortalConstants.CNo:
                    {
                        litContractNeeded.Visible = false;
                        litContractNone.Visible = true;
                        break;
                    }
                default:                                                     // Neither button pressed
                    {
                        litContractNeeded.Visible = false;
                        litContractNone.Visible = false;
                        break;
                    }
            }
            return;
        }

        // One of the radio buttons in the "Payment Method" panel has clicked. Switch the Delivery Instructions panel on or off.
        protected void rdoPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rdoPaymentMethod.SelectedValue == PaymentMethod.EFT.ToString()) // If == EFT button was clicked. Flip Delivery Instructions
            {
                pnlDeliveryInstructions.Visible = false;            // EFT doesn't need delivery instructions. Hide panel
                pnlDeliveryAddress.Visible = false;                 // Turn off delivery address
            }
            else
            {
                if (!pnlDeliveryInstructions.Visible)               // If false panel is currently invisible; last selection was EFT
                {
                    pnlDeliveryInstructions.Visible = true;         // Not EFT so need delivery instructions. Enable panel
                    rdoDeliveryMode.SelectedIndex = 0;              // Reset radio buttons to default value
                    pnlDeliveryAddress.Visible = false;             // Turn off delivery address
                }
            }
            return;
        }

        // One of the radio buttons in the "Source of Funds" panel has clicked. Switch the Project Class drop down list on or off.
        // In addition, redraw the split gridview to reflect the appearance or disappearance of the Project Code.

        protected void rdoSourceOfFunds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rdoSourceOfFunds.SelectedValue == PortalConstants.RDOFundsRestricted) // If == Restricted button was clicked. Turn on drop down list
                pnlProjectClass.Visible = true;                     // Turn on drop down list
            else
                pnlProjectClass.Visible = false;                    // Turn off drop down list
            if (!ddlGLCode.Enabled)                                 // If false, the ddl is turned off so Split must be on. Refresh
                CopySplitGridView();                                // Refresh the Split grid view, reflecting the change in the Source of Funds button
            return;
        }

        // One of the radio buttons in the "Fulfilment Instructions" and "Delivery Instructions" has clicked.

        protected void rdoDeliveryMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rdoDeliveryMode.SelectedValue == PortalConstants.DeliveryModeMailAddress) // If == delivery address needs to be specified
                pnlDeliveryAddress.Visible = true;                  // Turn on delivery address
            else
                pnlDeliveryAddress.Visible = false;                 // Turn off delivery address
            return;
        }

        protected void rdoPODeliveryMode_SelectedIndexChanged(object sender, EventArgs e) // Special for Purchase Orders
        {
            if (rdoPODeliveryMode.SelectedValue == PortalConstants.PODeliveryModeDeliverAddress) // If == delivery address needs to be specified
                pnlDeliveryAddress.Visible = true;                  // Turn on delivery address
            else
                pnlDeliveryAddress.Visible = false;                 // Turn off delivery address
            return;
        }

        protected void rdoPOVendorMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rdoPOVendorMode.SelectedValue == PortalConstants.POVendorModeYes) // If == any mode will do
                pnlEntity.Visible = false;                          // Any vendor will do, so no need to select vendor
            else
                pnlEntity.Visible = true;                           // Specific vendor needed, so show DDL
            return;
        }

        // The user has pressed the Split button. Flip the split-age.

        protected void btnSplit_Click(object sender, EventArgs e)
        {
            if (ddlGLCode.Enabled)                                      // If true, the function is "Split"
            {
                LoadFirstSplitRow();                                    // Load gridview with initial values
                EnergizeSplit();                                        // Turn the gridview on and other things off
            }
            else                                                        // The function is "Cancel"
            {
                ddlProjectClass.Enabled = true;                         // "Project Class" drop down list is back
                txtAmount.Enabled = true; txtAmount.Text = "";          // "Total Dollar Amount" field is back and empty
                ddlGLCode.Enabled = true;                               // "Expense Account" drop down list is back
                rfvGLCode.Enabled = true;                               // RequiredFieldValidation of GL code is back on
                btnSplit.Text = "Split";                                // Reverse the meaning of the Split button
                pnlExpenseSplit.Visible = false;                        // Turn off the grid for splits
            }
        }

        // As the Split gridview is being loaded, fill the ddls in each row and light appropriate buttons. Complicated!

        protected void gvExpSplit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)        // If == This is a data row of the grid
            {

                // Find all the controls and items for this row. We don't need all of them in every case, but it seems simpler this way.

                Label totalRows = (Label)e.Row.FindControl("txtTotalRows"); // Find control within the row
                Label selectedProjectClassID = (Label)e.Row.FindControl("txtSelectedProjectClassID"); // Locate item in the gridview row
                Label selectedGLCodeID = (Label)e.Row.FindControl("txtSelectedGLCodeID"); // Locate item in the gridview row
                DropDownList splitProjectClass = (DropDownList)e.Row.FindControl("ddlSplitProjectClass"); // Find drop down list within the row
                TextBox splitAmount = (TextBox)e.Row.FindControl("txtSplitAmount"); // Locate text box containing Amount
                DropDownList splitGLCode = (DropDownList)e.Row.FindControl("ddlSplitGLCode"); // Find drop down list within the row
                TextBox splitNote = (TextBox)e.Row.FindControl("txtSplitNote"); // Locate text box containing Note
                Button splitAdd = (Button)e.Row.FindControl("btnSplitAdd"); // Locate Add button
                Button splitRemove = (Button)e.Row.FindControl("btnSplitRemove"); // Locate the Remove button

                // Copy the entire ddlGLCode drop down list into this row's empty drop down list. Instantiate each ListItem so that
                // the new ddl is distinct from the ddlGLCode ddl. That way, we can select items from each list separately.

                splitGLCode.Items.AddRange(ddlGLCode.Items.Cast<ListItem>().Select(x => new ListItem(x.Text, x.Value)).ToArray());
                // Copy each ListItem from ddlGLCode ddl to SplitGLCode ddl

                if (selectedGLCodeID.Text != "")                        // If != there is a selected row. Selection is the GLCodeID of the row.
                    splitGLCode.SelectedValue = selectedGLCodeID.Text;  // Select that row

                // Repeat the process with the ddlProjectClass drop down list, if it is relevant

                if (rdoSourceOfFunds.SelectedValue == PortalConstants.RDOFundsRestricted) // If == the "Restricted" radio button is pressed; we're relevant
                {
                    splitProjectClass.Items.AddRange(ddlProjectClass.Items.Cast<ListItem>().Select(x => new ListItem(x.Text, x.Value)).ToArray());
                    // Copy each ListItem from ddlProjectClass to splitProjectClass ddl

                    if (selectedProjectClassID.Text != "")              // If != there is a selected row. Selection is the ProjectClassID of the row
                        splitProjectClass.SelectedValue = selectedProjectClassID.Text; // Select that row
                    else
                        splitProjectClass.SelectedValue = litSavedDefaultProjectClassID.Text; // Select the "global" default row
                }
                else
                    splitProjectClass.Enabled = false;                  // If irrelevant, disable it

                // If this is the only row of the gridview, disable the Rem button. Can't delete the only row.

                if (Convert.ToInt32(totalRows.Text) == 1)           // If == this is the only row of the gridview
                {
                    splitRemove.Enabled = false;                      // Disable the button
                }

                // If this is a View command, everything goes read-only

                if (litSavedCommand.Text == PortalConstants.QSCommandView) // If == this is a View command
                {
                    splitProjectClass.Enabled = false;              // Disable all the controls on this line, one-by-one
                    splitAmount.Enabled = false;
                    splitGLCode.Enabled = false;
                    splitNote.Enabled = false;
                    splitAdd.Enabled = false;
                    splitRemove.Enabled = false;
                }
            }
        }

        // Buttons and processing within the split expense gridview

        protected void btnSplitAdd_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;                              // Treat sender argument as a button
            GridViewRow r = (GridViewRow)b.NamingContainer;         // Find the object that contains the button - a gridview row
            CopySplitGridView(RowToAdd: r.RowIndex);                // Copy the grid, adding a blank row where indicated
        }

        protected void btnSplitRemove_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;                              // Treat sender argument as a button
            GridViewRow r = (GridViewRow)b.NamingContainer;         // Find the object that contains the button - a gridview row
            CopySplitGridView(RowToRemove: r.RowIndex);             // Copy the grid, removing row where indicated
        }

        protected void txtSplitAmount_TextChanged(object sender, EventArgs e)
        {
            ExtensionActions.ReloadDecimalText((TextBox)sender);    // Pretty up the text just entered
            RecalculateTotalDollarAmount();                         // Update "master" amount
        }

        // User has clicked on a row of the Supporting Docs list. Just turn on the buttons that work now.
        // And don't ask me why, but unless we act, the page title reverts to its original value. So for the case of View, set it again.

        protected void lstSupporting_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnView.Enabled = true;
            if (litSavedCommand.Text == PortalConstants.QSCommandView) // If == it is a View command
                Page.Title = "View Expense Request";                // Reset the page title. Don't know why it changed, but just reset it
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

        // Cancel button has been clicked. Just return to the Dashboard.

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLProjectDashboard);
            return;
        }

        protected void EDHistoryView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                EDHistoryView.PageIndex = e.NewPageIndex;             // Propagate the desired page index
                NavigationActions.LoadAllExpHistorys(Convert.ToInt32(litSavedExpID.Text), EDHistoryView); // Fill the list from the database
                EDHistoryView.SelectedIndex = -1;                     // No row currently selected
            }
            return;
        }

        // Revise button clicked. This is the case where a Exp is in the "Returned" state - it was submitted, but failed during the review process.
        // To "Revise," we: 
        //  1) Get a copy of the Exp
        //  2) Create a ExpHistory row to audit this change.
        //  3) Change the State of the Rsq from "Returned" to "Under Construction," erase the ReturnNote comments and save it.
        //  4) Call this page back and invoke the "Edit" command for an existing Expense.

        protected void btnRevise_Click(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  1) Get a copy of the Exp

                    int expID = Convert.ToInt32(litSavedExpID.Text); // Fetch the ID of the Exp row to be revised
                    Exp toRevise = context.Exps.Find(expID);        // Fetch the Exp that we want to update
                    if (toRevise == null)                           // If == the target Exp not found
                        LogError.LogInternalError("EditExpense", $"ExpenseID from Query String '{expID.ToString()}' could not be found in database"); // Log fatal error

                    //  2) Create a ExpHistory row to audit this change.

                    ExpHistory hist = new ExpHistory();             // Get a place to build a new ExpHistory row
                    StateActions.CopyPreviousState(toRevise, hist, "Revised"); // Create a ExpHistory log row from "old" version of Expense
//                    hist.ReturnNote = toRevise.ReturnNote;          // Save the Note from the Returned Rqst

                    //  3) Change the State of the Rqst from "Returned" to "Unsubmitted," erase the ReturnNote comments and save it.

                    StateActions.SetNewExpState(toRevise, StateActions.FindUnsubmittedExpState(litSavedProjectRole.Text), litSavedUserID.Text, hist); // Write down our current State and authorship
                    toRevise.ReturnNote = "";                       // Erase the note
                    toRevise.EntityNeeded = false; toRevise.PersonNeeded = false; // Assume "Returner" did as we asked
                    context.ExpHistorys.Add(hist);                  // Save new ExpHistory row
                    context.SaveChanges();                          // Commit the Add and Modify
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditExpense", "Error processing ExpHistory and Rqst rows"); // Fatal error
                }

                //  4) Call this page back and invoke the "Edit" command for an existing Expense.

                Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                            + PortalConstants.QSRequestID + "=" + Request.QueryString[PortalConstants.QSRequestID] + "&"
                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit); // Start with an existing Expense
            }
        }

        // Save button clicked. "Save" means that we unload all the controls for the Expense into a database row. 
        // If the Expense is new, we just add a new row. If the Expense already exists, we update it and add a history record to show the edit.
        // We can tell a Expense is new if the Query String doesn't contain a mention of a ExpID AND the literal litSavedExpID is empty.
        // On the first such Save, we stash the ExpID of the new Expense into this literal so that a second Save during the same session will find
        // it and update it, rather than creating a second row.

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int dummy = SaveExp();                                  // Do the heavy lifting
            if (dummy == 0) return;                                 // If == hit an error. Let user retry

            // Now go back to Dashboard

            Response.Redirect(PortalConstants.URLProjectDashboard + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                                  + PortalConstants.QSStatus + "=Expense Request saved");
        }

        // Submit button clicked. Save what we've been working on, set the Expense state to "Submitted," then go back to the dashboard.

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            // Before saving, make sure the Expense Split gridview contains healthy values. It's hard to validate these on-the-fly so do it here.

            if (!ValidateSplitGridView())                           // If false, there's a problem. Report the error
                return;                                             // Give user a chance to fix the problem

            // Before saving, make sure that a sufficient number of supporting documents are present. Note that a user can "Save" with
            // insufficent supporting docs, but can only "Submit" if the minimum is present.

            if (lstSupporting.Items.Count < Convert.ToInt32(litSupportingDocMin.Text)) // If < the minimum number of docs is not present
            {
                litDangerMessage.Text = "The Expense Request must include a minimum of " + litSupportingDocMin.Text + " Supporting Document.";
                litSuccessMessage.Text = "";                        // Just the danger message, not a stale success message
                return;                                             // Back for more punishment
            }

            int savedExpID = SaveExp();                             // Do the heavy lifting to save the current Expense
            if (savedExpID == 0)                                    // If == SaveExp encountered an error. Go no further
                LogError.LogInternalError("EditExpense", "Unable to save Expense before Submitting"); // Fatal error

            // SaveExp just saved the Request, which may or may not have written a ExpHistory row. But now, let's write another
            // ExpHistory row to describe the Submit action.

            string emailSent = "";
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    Exp toSubmit = context.Exps.Find(savedExpID);       // Find the Exp that we want to update
                    ExpHistory hist = new ExpHistory();                 // Get a place to build a new ExpHistory row

                    StateActions.CopyPreviousState(toSubmit, hist, "Submitted"); // Fill the ExpHistory row from "old" version of Expense

                    ExpState nextState = StateActions.FindNextState(toSubmit.CurrentState); // Figure out what next state is. Nuanced.
                    StateActions.SetNewExpState(toSubmit, nextState, litSavedUserID.Text, hist); // Move to that state
                    toSubmit.SubmitUserID = litSavedUserID.Text;        // Remember who submitted this Exp. They get notification on Return.

                    context.ExpHistorys.Add(hist);                      // Save new ExpHistory row
                    context.SaveChanges();                              // Update the Exp with new fields

                    // If it's a rush request, first send a broadcast "INCOMING!" email to project staff. Then send the regular email with rush status

                    if (toSubmit.Rush)                                  // If true, this is a rush request. Send broadcast email to alert staff
                      EmailActions.BroadcastEmailToAllStaff(toSubmit.Rush, // Send email to all staff members. Rush email if rush request
                        StateActions.UserRoleToApproveRequest(nextState), // Tell next reviewer, who is in this role
                        toSubmit.ProjectID,                             // Request is associated with this project
                        toSubmit.Project.Name,                          // Project has this name (for parameter substitution)
                        EnumActions.GetEnumDescription(RequestType.Expense), // This is an Expense Request
                        EnumActions.GetEnumDescription(nextState),      // Here is its next state
                        PortalConstants.CEmailDefaultExpenseBroadcastSubject, PortalConstants.CEmailDefaultExpenseBroadcastBody); // Use this subject and body, if needed

                    emailSent = EmailActions.SendEmailToReviewer(toSubmit.Rush, // Send email to next reviewer. Rush email if rush request
                        StateActions.UserRoleToApproveRequest(nextState), // Tell next reviewer, who is in this role
                        toSubmit.ProjectID,                             // Request is associated with this project
                        toSubmit.Project.Name,                          // Project has this name (for parameter substitution)
                        EnumActions.GetEnumDescription(RequestType.Expense), // This is an Expense Request
                        EnumActions.GetEnumDescription(nextState),      // Here is its next state
                        PortalConstants.CEmailDefaultExpenseApprovedSubject, PortalConstants.CEmailDefaultExpenseApprovedBody); // Use this subject and body, if needed
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditExpense", "Unable to update Expense into Submitted state"); // Fatal error
                }
            }

            // Now go back to Dashboard

            Response.Redirect(PortalConstants.URLProjectDashboard + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                                  + PortalConstants.QSStatus + "=Expense Request submitted." + emailSent);
        }

        // Show History Button clicked. Open up and fill a GridView of all the ExpHistory rows for this ExpenseRequest

        protected void btnShowHistory_Click(object sender, EventArgs e)
        {
            NavigationActions.LoadAllExpHistorys(Convert.ToInt32(litSavedExpID.Text), EDHistoryView); // Fill the list from the database
            return;
        }

        // Copy an existing Rqst.
        //  1) Copy the Rqst row into a new row
        //  2) Copy the splits
        //  3) Copy the Supporting Docs
        //  4) Create a ExpHistory row to describe the copy

        private int CopyExp(int sourceID)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                Exp src = context.Exps.Find(sourceID);              // Fetch Rqst row by its key
                if (src == null)                                    // If == source Rqst could not be found. Internal error
                    LogError.LogInternalError("EditExpense", $"ExpenseID '{sourceID.ToString()}' from selected GridView row not found in database"); // Fatal error
                try
                {

                    //  1) Copy the Rqst row into a new row

                    Exp dest = new Exp()
                    {                                               // Instantiate and fill the destination Rqst
                        Inactive = false,
                        Archived = false,
                        Amount = src.Amount,
                        BeginningDate = src.BeginningDate, EndingDate = src.EndingDate,
                        CardsQuantity = src.CardsQuantity, CardsValueEach = src.CardsValueEach,
                        ContractOnFile = src.ContractOnFile,
                        ContractReason = src.ContractReason,
                        ContractComing = src.ContractComing,
                        CreatedTime = System.DateTime.Now,          // Rqst is created right now
                        DateOfInvoice = src.DateOfInvoice,
                        DateNeeded = src.DateNeeded,
                        DeliveryMode = src.DeliveryMode, DeliveryAddress = src.DeliveryAddress,
                        Description = src.Description + " (copy)",  // Note that this is a copy
                        EntityNeeded = src.EntityNeeded, EntityRole = src.EntityRole, EntityID = src.EntityID,
                        ExpType = src.ExpType,
                        GLCodeID = src.GLCodeID,
                        GoodsCostPerUnit = src.GoodsCostPerUnit, GoodsDescription = src.GoodsDescription,
                        GoodsQuantity = src.GoodsQuantity, GoodsSKU = src.GoodsSKU,
                        InvoiceNumber = src.InvoiceNumber,
                        Notes = src.Notes,
                        PaymentMethod = src.PaymentMethod,
                        PersonNeeded = src.PersonNeeded, PersonRole = src.PersonRole, PersonID = src.PersonID,
                        PODeliveryMode = src.PODeliveryMode, POVendorMode = src.POVendorMode,
                        ProjectID = src.ProjectID,                  // New Rqst is in the same project
                        ReturnNote = "",                            // Clear out the return note, if any
                        SourceOfFunds = src.SourceOfFunds, ProjectClassID = src.ProjectClassID,
                        StaffNote = "",                             // Clear out the staff note, if any
                        Summary = src.Summary + " (copy)",          // Note that this is a copy
                        CurrentTime = System.DateTime.Now,
                        CurrentState = StateActions.FindUnsubmittedExpState(litSavedProjectRole.Text),
                        CurrentUserID = litSavedUserID.Text         // Current user becomes creator of new Rqst
                    };

                    context.Exps.Add(dest);                         // Store the new row
                    context.SaveChanges();                          // Commit the change to product the new Rqst ID

                    //  2) Copy the split rows, if any.

                    SplitActions.CopySplitRows(src.ExpID, dest.ExpID); // Copy all the split rows, if any

                    //  3) Copy the Supporting Docs. We know that in the source Rqst, all the supporting docs are "permanent," i.e., we don't need
                    //  to worry about "temporary" docs.

                    SupportingActions.CopyDocs(context, RequestType.Expense, src.ExpID, dest.ExpID); // Copy each of the Supporting Docs

                    //  4) Create a ExpHistory row to describe the copy

                    ExpHistory hist = new ExpHistory();
                    StateActions.CopyPreviousState(src, hist, "Copied"); // Fill fields of new record
                    hist.ExpID = dest.ExpID; hist.NewExpState = dest.CurrentState; // Add fields from copied App

                    context.ExpHistorys.Add(hist);                  // Add the row
                    context.SaveChanges();                          // Commit the change
                    return dest.ExpID;                              // Return the ID of the new Expense Rqst
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditExpense", "Error processing ExpHistory and RqstSupporting rows"); // Fatal error
                }
                return 0;
            }
        }

        // Package the work of the Save so that Submit can do it as well.

        private int SaveExp()
        {
            litDangerMessage.Text = ""; litSuccessMessage.Text = ""; // Start with a clean slate of message displays
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    int expID = 0;                                 // Assume no saved ID is available
                    if (litSavedExpID.Text != "") expID = QueryStringActions.ConvertID(litSavedExpID.Text).Int; // If != saved ID is available, use it  
                    if (expID == 0)                                // If == no doubt about it, Save makes a new row
                    {

                        // To make a new Exp row:
                        //  1) Instantiate a new Exp record
                        //  2) Fill in the fixed fields, e.g., the Project ID that owns us and various times
                        //  3) unload on-screen fields to the growing record
                        //  4) Save the record to the database producing an ExpID
                        //  5) Unload the supporting documents

                        Exp toSave = new Exp(){                     // Get a place to hold everything
                            Inactive = false,
                            Archived = false,
                            ProjectID = QueryStringActions.ConvertID(litSavedProjectID.Text).Int, // Connect Expense to Project
                            CreatedTime = System.DateTime.Now,      // Stamp time when Expense was first created as "now"
                            BeginningDate = (DateTime)SqlDateTime.MinValue, // Non-null values to satisfy SQL
                            EndingDate = (DateTime)SqlDateTime.MinValue,
                            DateNeeded = (DateTime)SqlDateTime.MinValue,
                            DateOfInvoice = (DateTime)SqlDateTime.MinValue
                        };
                        UnloadPanels(toSave);                       // Move from Panels to record
                        StateActions.SetNewExpState(toSave, EnumActions.ConvertTextToExpState(litSavedStateEnum.Text), litSavedUserID.Text);
                                                                    // Write down our current State and authorship
                        context.Exps.Add(toSave);                   // Save new Rqst row
                        context.SaveChanges();                      // Commit the Add
                        litSavedExpID.Text = toSave.ExpID.ToString(); // Show that we've saved it once
                        UnloadSplitRows(toSave.ExpID, gvExpSplit); // Save the splits from the gridview to the database
                        UnloadSupportingDocs(toSave.ExpID);         // Save all the supporting documents
                        litSuccessMessage.Text = "Expense Request successfully saved"; // Let user know we're good
                        return toSave.ExpID;                        // Return the finished Rqst
                    }
                    else                                            // Update an existing Exp row
                    {

                        // To update an existing Exp row:
                        //  1) Fetch the existing record using the ExpID key
                        //  2) Unload on-screen fields overwriting the existing record
                        //  3) Update the record to the database
                        //  4) Create a new ExpHistory row to preserve information about the previous Save
                        //  5) Unload the supporting documents

                        Exp toUpdate = context.Exps.Find(expID);    // Fetch the Rqst that we want to update
                        UnloadPanels(toUpdate);                     // Move from Panels to Rqst, modifying it in the process
                        ExpHistory hist = new ExpHistory();         // Get a place to build a new ExpHistory row
                        StateActions.CopyPreviousState(toUpdate, hist, "Saved"); // Create a ExpHistory log row from "old" version of Expense
                        StateActions.SetNewExpState(toUpdate, hist.PriorExpState, litSavedUserID.Text, hist);
                            // Write down our current State and authorship
                        context.ExpHistorys.Add(hist);              // Save new ExpHistory row
                        context.SaveChanges();                      // Commit the Add or Modify
                        UnloadSplitRows(toUpdate.ExpID, gvExpSplit); // Save the splits from the gridview to the database
                        UnloadSupportingDocs(toUpdate.ExpID);       // Save all the new supporting documents
                        litSuccessMessage.Text = "Expense Request successfully updated";    // Let user know we're good
                        return toUpdate.ExpID;                      // Return the finished Rqst
                    }
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditExpense", "Error writing Exp row"); // Fatal error
                }
            }
            return 0;                                               // We never get this far
        }

        // Based on the selected Expense Type, enable and disable the appropriate panels on the display.

        void EnablePanels(ExpType type)
        {
            pnlAmount.Visible = true; txtAmount.Enabled = true; btnSplit.Visible = false; // Unconditionally visible (or invisible) for all Expense types
            pnlDescription.Visible = true;
            pnlDeliveryAddress.Visible = false;
            pnlExpenseSplit.Visible = false; btnSplit.Text = "Split";
            pnlSourceOfFunds.Visible = true;
            pnlGLCode.Visible = true; ddlGLCode.Enabled = true;
            pnlNotes.Visible = true;
            ddlProjectClass.Enabled = true;                         // Doesn't affect the panel's visibility, just enables the ddl
            pnlReturnNote.Visible = false;
            if (litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString()) // If == user is an IC
                pnlStaffNote.Visible = true;                        // The IC can see the staff note
            else
                pnlStaffNote.Visible = false;                       // But no one else can
            pnlState.Visible = true;
            pnlSupporting.Visible = true;
            pnlURL.Visible = false;                                 // Probably obsolete, but let's keep it around a little longer
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
                        pnlDeliveryInstructions.Visible = true; pnlPODeliveryInstructions.Visible = false;
                        pnlEntity.Visible = false;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true; ExtensionActions.EnableRdoListItems(rdoPaymentMethod); // Enable all methods
                        pnlPerson.Visible = true; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.Contractor); litSavedPersonEnum.Text = PersonRole.Contractor.ToString();
                        pnlPOFulFillmentInstructions.Visible = false;
                        litSupportingDocMin.Text = "1";
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
                        pnlDeliveryInstructions.Visible = true; pnlPODeliveryInstructions.Visible = false;
                        pnlGoods.Visible = false;
                        pnlEntity.Visible = false;
                        pnlPaymentMethod.Visible = false;
                        pnlPerson.Visible = true; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.ResponsiblePerson); litSavedPersonEnum.Text = PersonRole.ResponsiblePerson.ToString();
                        pnlPOFulFillmentInstructions.Visible = false;
                        litSupportingDocMin.Text = "1";
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
                        pnlDeliveryInstructions.Visible = true; pnlPODeliveryInstructions.Visible = false;
                        pnlEntity.Visible = false;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true; ExtensionActions.EnableRdoListItems(rdoPaymentMethod, creditcard:false, invoice:false); // Enable correct methods
                        pnlPerson.Visible = true; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.Employee); litSavedPersonEnum.Text = PersonRole.Employee.ToString();
                        pnlPOFulFillmentInstructions.Visible = false;
                        litSupportingDocMin.Text = "1";
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
                        pnlDeliveryInstructions.Visible = false; pnlPODeliveryInstructions.Visible = true;
                        pnlEntity.Visible = true; litSavedEntityEnum.Text = EntityRole.ExpenseVendor.ToString();
                        pnlGoods.Visible = true;
                        pnlPaymentMethod.Visible = true; ExtensionActions.EnableRdoListItems(rdoPaymentMethod); // Enable all methods
                        pnlPerson.Visible = false;
                        pnlPOFulFillmentInstructions.Visible = true;
                        litSupportingDocMin.Text = "1";
                        break;
                    }
                case ExpType.Reimbursement:
                    {
                        lblAmount.Text = "Total Dollar Amount"; btnSplit.Visible = true; // Reimbursements can be split
                        pnlBeginningEnding.Visible = false;
                        pnlCards.Visible = false;
                        pnlContractQuestions.Visible = false;
                        pnlDateNeeded.Visible = false;
                        pnlDateOfInvoice.Visible = false;
                        pnlDeliveryInstructions.Visible = true; pnlPODeliveryInstructions.Visible = false;
                        pnlEntity.Visible = false;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true; ExtensionActions.EnableRdoListItems(rdoPaymentMethod, creditcard:false, eft:false, invoice:false); // Enable relevant methods
                        pnlPerson.Visible = true; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.Recipient); litSavedPersonEnum.Text = PersonRole.Recipient.ToString();
                        pnlPOFulFillmentInstructions.Visible = false;
                        litSupportingDocMin.Text = "1";
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
                        pnlDeliveryInstructions.Visible = true; pnlPODeliveryInstructions.Visible = false;
                        pnlEntity.Visible = true; litSavedEntityEnum.Text = EntityRole.ExpenseVendor.ToString();
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true; ExtensionActions.EnableRdoListItems(rdoPaymentMethod); // Enable all methods
                        pnlPerson.Visible = false;
                        pnlPOFulFillmentInstructions.Visible = false;
                        litSupportingDocMin.Text = "1";
                        break;
                    }
                default:
                    {
                        LogError.LogInternalError("EditExpense", $"Invalid ExpType value '{type.ToString()}' in database"); // Fatal error
                        break;
                    }
            }
            return;
        }

        // Fill in the three "big" dropdown lists for a new Expense

        void FillDropDownLists()
        {
            FillEntityDDL(null);
            FillGLCodeDDL(null);
            FillPersonDDL(null);
            FillProjectClassDDL(null);
            return;
        }

        // Fill the drop down list from the database. 

        void FillEntityDDL(int? entityID, bool needed = false)
        {
            if (pnlEntity.Visible && (ddlEntity.Items.Count == 0))      // If true control is visible and empty. Fill it
            {
                int projID = QueryStringActions.ConvertID(litSavedProjectID.Text).Int; // Find ID of current project
                EntityRole selectedRole = EnumActions.ConvertTextToEntityRole(litSavedEntityEnum.Text); // Role for this Request Type

                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    var query = from pe in context.ProjectEntitys
                                where pe.ProjectID == projID && pe.EntityRole == selectedRole // This project, this role
                                orderby pe.Entity.Name
                                select new { EntityID = pe.EntityID, pe.Entity.Name }; // Find all Entitys that are assigned to this project

                    DataTable rows = new DataTable();
                    rows.Columns.Add(PortalConstants.DdlID);
                    rows.Columns.Add(PortalConstants.DdlName);

                    foreach (var row in query)
                    {
                        DataRow dr = rows.NewRow();                      // Build a row from the next query output
                        dr[PortalConstants.DdlID] = row.EntityID;
                        dr[PortalConstants.DdlName] = row.Name;
                        rows.Rows.Add(dr);                               // Add the new row to the data table
                    }
                    ddlEntity.Items.Clear();                            // Remove items of another Vendor Role, if any

                    StateActions.LoadDdl(ddlEntity, entityID, rows,
                        "", "-- none selected --",
                        needed, "-- Please add new " + lblEntity.Text + " --"); // Put the cherry on top

                }
            }
            return;
        }

        // If the GLCode panel is enabled, fill the drop down list from the database

        void FillGLCodeDDL(int? glcodeID)
        {
            if (pnlGLCode.Visible && (ddlGLCode.Items.Count == 0))  // If true control is visible and empty. Fill it
            {
                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    string franchiseKey = SupportingActions.GetFranchiseKey(); // Find current key value
                    var query = from gl in context.GLCodes
                                where !gl.Inactive && gl.ExpCode && gl.FranchiseKey == franchiseKey
                                orderby gl.Code
                                select new { gl.GLCodeID, gl.Code }; // Find all Expense-specific codes that are active

                    DataTable rows = new DataTable();            // Create an empty DataTable to hold rows returned by Query
                    rows.Columns.Add(PortalConstants.DdlID);
                    rows.Columns.Add(PortalConstants.DdlName);

                    foreach (var row in query)
                    {
                        DataRow dr = rows.NewRow();              // Build a row from the next query output
                        dr[PortalConstants.DdlID] = row.GLCodeID;
                        dr[PortalConstants.DdlName] = row.Code;
                        rows.Rows.Add(dr);                       // Add the new row to the data table
                    }

                    StateActions.LoadDdl(ddlGLCode, glcodeID, rows,
                        "-- Error: No GL Codes in Database --", "-- none selected --"); // Put the cherry on top

                }
            }
            return;
        }

        // If the Person panel is enabled, fill the drop down list from the database. Note that these values are drawn from
        // the ProjectPerson table, which shows Persons assigned to this project. Figure out the person role that corresponds to the request type.

        void FillPersonDDL(int? personID, bool needed=false)
        {
            if (pnlPerson.Visible)                                // If true need to populate list
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
                    ddlPerson.Items.Clear();                        // Remove items of another Person Role, if any

                    StateActions.LoadDdl(ddlPerson, personID, rows,
                        "", "-- none selected --",
                        needed, "-- Please add new " + lblPerson.Text + " --"); // Put the cherry on top


                }
            }
            return;
        }

        // If the Funds panel is enabled, fill the drop down list from the database. Note that these values are drawn from
        // the ProjectClass table, which shows Project Classes assigned to this project. The nuance here is that the list of
        // Project Classes for this project may contain a Default - a Project Class that should be suggested to the User as their default choice.

        void FillProjectClassDDL(int? pcID)
        {
            if (pnlSourceOfFunds.Visible)                               // If true need to populate list
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
                            {
                                defaultID = row.ProjectClassID;         // Save this for later use
                                litSavedDefaultProjectClassID.Text = row.ProjectClassID.ToString(); // Save default for use in split expense rows
                            }
                        }

                        if (pcID != null)                               // If != caller specified something
                        {
                            if (pcID != 0)                              // If != caller specified a row to select
                                defaultID = pcID;                       // Position the DDL to that row
                        }

                        StateActions.LoadDdl(ddlProjectClass, defaultID, rows, 
                            " -- Error: No Project Classes assigned to Project --", "-- none selected --"); // Put the cherry on top

                    }

                    //StateActions.FinishDdlLoad(defaultID, ddlProjectClass, " -- Error: No Project Classes assigned to Project --", "-- none selected --"); 
                    //    // Put the cherry on top
                }
            }
            return;
        }

        // Move values from the Expense record into the Page. Only move the values that are "Visible" for this Expense Type

        void LoadPanels(Exp record)
        {
            litSavedStateEnum.Text = record.CurrentState.ToString();  // Stash enum version of state
            txtState.Text = EnumActions.GetEnumDescription(record.CurrentState); // Display "English" version of state

            if (pnlAmount.Visible)
                txtAmount.Text = ExtensionActions.LoadDecimalIntoTxt(record.Amount);

            if (pnlCards.Visible)
            {
                txtNumberOfCards.Text = record.CardsQuantity.ToString();
                txtEachCard.Text = record.CardsValueEach.ToString("C");
            }

            if (pnlContractQuestions.Visible)
            {
                EventArgs e = new EventArgs();                              // Dummy argument
                ExtensionActions.LoadYesNoIntoRdo(record.ContractOnFile, rdoContractOnFile); // Load enum value into Radio Button List
                rdoContractOnFile_SelectedIndexChanged(0, e);               // React to the setting

                txtContractReason.Text = record.ContractReason;

                ExtensionActions.LoadYesNoIntoRdo(record.ContractComing, rdoContractComing); // Load enum value into Radio Button List
                rdoContractComing_SelectedIndexChanged(0, e);               // React to the setting
            }
            if (pnlDateOfInvoice.Visible)
            {
                DateActions.LoadDateIntoTxtCal(record.DateOfInvoice, txtDateOfInvoice, calDateOfInvoice);
                txtInvoiceNumber.Text = record.InvoiceNumber;
            }

            if (pnlDateNeeded.Visible)
                DateActions.LoadDateIntoTxtCal(record.DateNeeded, txtDateNeeded, calDateNeeded);

            if (pnlDeliveryInstructions.Visible)
            {
                ExtensionActions.LoadEnumIntoRdo(record.DeliveryMode, rdoDeliveryMode); // Load enum value into Radio Button List
                if (record.DeliveryMode == DeliveryMode.MailAddress)        // If true Delivery Mode requires an address
                {
                    pnlDeliveryAddress.Visible = true;                      // Turn on the Delivery Address
                    txtDeliveryAddress.Text = record.DeliveryAddress;       // And fill it
                }
                cblDeliveryInstructions.Items.FindByValue(Exp.DeliveryInstructionsRush).Selected = record.Rush; // Fill value of checkbox
            }

            txtDescription.Text = record.Description;
            
            if (pnlBeginningEnding.Visible)
            {
                DateActions.LoadDateIntoTxtCal(record.BeginningDate, txtBeginningDate, calBeginningDate);
                DateActions.LoadDateIntoTxtCal(record.EndingDate, txtEndingDate, calEndingDate);
            }

            if (pnlEntity.Visible)
            {
                FillEntityDDL(record.EntityID, record.EntityNeeded);    // Pull the VendorID and load the DDL
                if (pnlPOFulFillmentInstructions.Visible)               // If true, controls that include the Vendor DDL are also visible
                {
                    ExtensionActions.LoadYesNoIntoRdo(record.POVendorMode, rdoPOVendorMode); // Load enum value into Radio Button List

                    EventArgs e = new EventArgs();                      // Dummy argument
                    rdoPOVendorMode_SelectedIndexChanged(0, e);         // React to the setting, which may involve hiding the Vendor DDL again
                }
            }

            if (pnlGLCode.Visible)
                FillGLCodeDDL(record.GLCodeID);                         // Fill drop down list and hightlight the selected item

            if (pnlGoods.Visible)
            {
                txtGoodsDescription.Text = record.GoodsDescription;
                txtGoodsSKU.Text = record.GoodsSKU;
                txtGoodsQuantity.Text = record.GoodsQuantity.ToString();
                txtGoodsCostPerUnit.Text = record.GoodsCostPerUnit.ToString("C");
            }

            if (pnlNotes.Visible)
                txtNotes.Text = record.Notes;

            if (pnlPaymentMethod.Visible)
            {
                EventArgs e = new EventArgs();                              // Dummy argument
                ExtensionActions.LoadEnumIntoRdo(record.PaymentMethod, rdoPaymentMethod); // Load enum value into Radio Button List
                rdoPaymentMethod_SelectedIndexChanged(0, e);                // React to the loaded value
            }

            if (pnlPerson.Visible)
                FillPersonDDL(record.PersonID, record.PersonNeeded);        // Fill drop down list and highlight the selected item, if any

            if (pnlPODeliveryInstructions.Visible)
            {
                ExtensionActions.LoadEnumIntoRdo(record.PODeliveryMode, rdoPODeliveryMode); // Load enum value into Radio Button List
                if (record.PODeliveryMode == PODeliveryMode.DeliverAddress) // If true Delivery Mode requires an address
                {
                    pnlDeliveryAddress.Visible = true;                  // Turn on the Delivery Address
                    txtDeliveryAddress.Text = record.DeliveryAddress;   // And fill it
                }
            }

            if (record.CurrentState == ExpState.Returned)               // If == the Rqst has been returned, so a Return Note may be present
            {
                pnlReturnNote.Visible = true;                           // Make this panel visible
                txtReturnNote.Text = record.ReturnNote;                 // Copy the text of the Return Note
            }

            if (pnlSourceOfFunds.Visible)                               // If true, process Source of Funds and Project Class
            {
                if (record.SourceOfFunds == SourceOfExpFunds.Restricted)   // If == the Source of Funds is a Project Class
                {
                    rdoSourceOfFunds.SelectedValue = PortalConstants.RDOFundsRestricted; // Select the Restricted button
                    pnlProjectClass.Visible = true;
                }
                else if (record.SourceOfFunds == SourceOfExpFunds.Unrestricted) // If == the Source of Funds does not use a Project Class
                {
                    rdoSourceOfFunds.SelectedValue = PortalConstants.RDOFundsUnrestricted; // Select the Unrestricted button
                    pnlProjectClass.Visible = false;                    // Unrestricted means no Project Class so don't show the list
                }
                FillProjectClassDDL(record.ProjectClassID);             // Fill the DDL even if it's not visible yet. User could change that
            }

            if (pnlStaffNote.Visible)
                txtStaffNote.Text = record.StaffNote;

            //if (txtURL.Visible)
            //    txtURL.Text = record.URL;

            // Supporting Documents handled elsewhere
            return;
        }

        // Move the "as edited" values from the Page into a Expense record. Only move the values that are "Visible" for this Expense Type

        void UnloadPanels(Exp record)
        {
            if (pnlAmount.Visible)
                record.Amount = ExtensionActions.LoadTxtIntoDecimal(txtAmount); // Convert into decimal and store

            if (pnlBeginningEnding.Visible)
            {
                record.BeginningDate = DateActions.LoadTxtIntoDate(txtBeginningDate);
                record.EndingDate = DateActions.LoadTxtIntoDate(txtEndingDate);
            }

            if (pnlCards.Visible)
            {
                record.CardsQuantity = ExtensionActions.LoadTxtIntoInt(txtNumberOfCards); // Convert into int and store
                record.CardsValueEach = ExtensionActions.LoadTxtIntoDecimal(txtEachCard); // Convert into decimal and store
            }

            if (pnlContractQuestions.Visible)
            {
                record.ContractOnFile = ExtensionActions.LoadRdoIntoYesNo(rdoContractOnFile); // Convert Radio Buttons into YesNo enum
                record.ContractReason = txtContractReason.Text;
                record.ContractComing = ExtensionActions.LoadRdoIntoYesNo(rdoContractComing); // Convert Radio Buttons into YesNo enum
            }

            if (pnlDateNeeded.Visible)
                record.DateNeeded = DateActions.LoadTxtIntoDate(txtDateNeeded);

            if (pnlDateOfInvoice.Visible)
            {
                record.DateOfInvoice = DateActions.LoadTxtIntoDate(txtDateOfInvoice);
                record.InvoiceNumber = txtInvoiceNumber.Text;
            }

            if (pnlDeliveryAddress.Visible)
                record.DeliveryAddress = txtDeliveryAddress.Text;

            if (pnlDeliveryInstructions.Visible)
            {
                record.DeliveryMode = EnumActions.ConvertTextToDeliveryMode(rdoDeliveryMode.SelectedValue); // Convert selection to enum
                record.Rush = cblDeliveryInstructions.Items.FindByValue(Exp.DeliveryInstructionsRush).Selected; // Stash value of checkbox
            }

            record.Description = txtDescription.Text;

            if (pnlEntity.Visible)
            {
                int? id; bool needed;
                StateActions.UnloadDdl(ddlEntity, out id, out needed);  // Carefully pull selected value into record
                record.EntityID = id; record.EntityNeeded = needed;
            }
            record.ExpType = EnumActions.ConvertTextToExpType(rdoExpType.SelectedValue); // Expense Type - convert from string VALUE to enumerated RqstType

            if (pnlGLCode.Visible)
                record.GLCodeID = StateActions.UnloadDdl(ddlGLCode);    // Carefully pull selected value into record

            if (pnlGoods.Visible)
            {
                record.GoodsDescription = txtGoodsDescription.Text;
                record.GoodsSKU = txtGoodsSKU.Text;
                record.GoodsQuantity = ExtensionActions.LoadTxtIntoInt(txtGoodsQuantity); // Convert to int and store
                record.GoodsCostPerUnit = ExtensionActions.LoadTxtIntoDecimal(txtGoodsCostPerUnit); // Convert into decimal and store
            }

            if (pnlNotes.Visible)
                record.Notes = txtNotes.Text;

            if (pnlPaymentMethod.Visible)
                record.PaymentMethod = EnumActions.ConvertTextToPaymentMethod(rdoPaymentMethod.SelectedValue); // Convert to enum and store

            if (pnlPerson.Visible)
            {
                {
                    int? id; bool needed;
                    StateActions.UnloadDdl(ddlPerson, out id, out needed);  // Carefully pull selected value into record
                    record.PersonID = id; record.PersonNeeded = needed;
                }
                record.PersonRole = EnumActions.ConvertTextToPersonRole(litSavedPersonEnum.Text); // Convert relevant role to enum
            }

            if (pnlPODeliveryInstructions.Visible)
                record.PODeliveryMode = EnumActions.ConvertTextToPODeliveryMode(rdoPODeliveryMode.SelectedValue); // Convert selection to bool

            if (pnlPOFulFillmentInstructions.Visible)
                record.POVendorMode = ExtensionActions.LoadRdoIntoYesNo(rdoPOVendorMode); // Convert Yes/No to enum

            // Return Note is read-only, so not unloaded
            
            if (pnlSourceOfFunds.Visible)                                    // If true the Expense does have a Source of Funds
            {
                if (rdoSourceOfFunds.SelectedValue == PortalConstants.RDOFundsRestricted) // If == Restricted button is clicked. 
                {
                    record.SourceOfFunds = SourceOfExpFunds.Restricted;     // Remember this setting
                    record.ProjectClassID = StateActions.UnloadDdl(ddlProjectClass); // Pull the selected Project Class from the DDL
                }
                else
                    record.SourceOfFunds = SourceOfExpFunds.Unrestricted;   // Remember this non-setting
            }
            else
                record.SourceOfFunds = SourceOfExpFunds.NA;                 // Not applicable to this type of Expense Request

            if (pnlStaffNote.Visible)
                record.StaffNote = txtStaffNote.Text;
                
            //if (pnlURL.Visible) 
            //    record.URL = txtURL.Text;

            return;
        }

        // Copy the most recent state, which is about to be overwritten, into new History row. This preserves the previous state of the Expense

        void CopyPreviousState(Exp rqst, ExpHistory hist)
        {
            hist.PriorExpState = rqst.CurrentState;
            hist.HistoryTime = rqst.CurrentTime;
            hist.HistoryUserID = rqst.CurrentUserID;
            hist.HistoryNote = "Revision saved by User";            // Explain why this revision appeared
            hist.ExpID = rqst.ExpID;                                // Connect history row to original Expense row
            return;
        }

        // Make all controls readonly to prevent editing during View function

        void ReadOnlyControls()
        {
            txtAmount.Enabled = false;
            txtBeginningDate.Enabled = false; btnBeginningDate.Enabled = false; txtEndingDate.Enabled = false; btnEndingDate.Enabled = false;
            rdoContractOnFile.Enabled = false; txtContractReason.Enabled = false; rdoContractComing.Enabled = false;
            txtDateOfInvoice.Enabled = false;
            txtDateNeeded.Enabled = false; btnDateNeeded.Enabled = false;
            txtDeliveryAddress.Enabled = false;
            rdoDeliveryMode.Enabled = false; cblDeliveryInstructions.Enabled = false;
            txtDescription.Enabled = false;
            // Expense Type - already set
            // Expense State - already set
            txtEachCard.Enabled = false;
            ddlGLCode.Enabled = false;
            txtGoodsDescription.Enabled = false; txtGoodsSKU.Enabled = false; txtGoodsQuantity.Enabled = false; txtGoodsCostPerUnit.Enabled = false;
            txtInvoiceNumber.Enabled = false;
            txtNotes.Enabled = false;
            txtNumberOfCards.Enabled = false;
            rdoPaymentMethod.Enabled = false;
            ddlPerson.Enabled = false;
            rdoPODeliveryMode.Enabled = false;
            rdoPOVendorMode.Enabled = false;
            rdoSourceOfFunds.Enabled = false; ddlProjectClass.Enabled = false;
            btnSplit.Visible = false;
//            txtURL.Enabled = false;
            ddlEntity.Enabled = false;
            txtStaffNote.Enabled = false;
            // Suporting Docs - leave Listbox filled and enabled so that double click still works
            if (pnlSupporting.Visible) 
                fupUpload.Enabled = false;   // But no new items
            return;
        }

        // Add or remove a row from the gvExpSplit gridview. That means making a row-by-row copy of the contents and rebinding the gridview

        void CopySplitGridView(int RowToAdd = -1, int RowToRemove = -1)
        {
            int totalRows = gvExpSplit.Rows.Count + ((RowToAdd == -1) ? 0 : 1) - ((RowToRemove == -1) ? 0 : 1); // Calculate final row count. Cute, eh?

            List<GLCodeSplitRow> rows = new List<GLCodeSplitRow>(); // A list of rows for the refreshed gridview control
            for (int i = 0; i < gvExpSplit.Rows.Count; i++)  // Cycle through existing rows, one-by-one
            {
                if (i != RowToRemove)                               // If != this is not the row to skip, so press on
                {
                    DropDownList ddlSplitGLCode = (DropDownList)gvExpSplit.Rows[i].FindControl("ddlSplitGLCode"); // Find drop down list
                    DropDownList ddlSplitProjectClass = (DropDownList)gvExpSplit.Rows[i].FindControl("ddlSplitProjectClass"); // And the other one
                    TextBox txtSplitAmount = (TextBox)gvExpSplit.Rows[i].FindControl("txtSplitAmount"); // Find the text box within gridview row
                    TextBox txtSplitNote = (TextBox)gvExpSplit.Rows[i].FindControl("txtSplitNote"); // Find the text box within gridview row

                    GLCodeSplitRow row = new GLCodeSplitRow()     // Create container for us to build up a row
                    {
                        TotalRows = totalRows,                      // Note row count of gridview to help RowDataBound get cute
                        SelectedGLCodeID = ddlSplitGLCode.SelectedValue, // Spot the selected row of the drop down list, if any. This is the GLCode ID
                        SelectedProjectClassID = ddlSplitProjectClass.SelectedValue, // Repeat for Project Class drop down list
                        Amount = txtSplitAmount.Text,               // Copy amount value, if any, into refresh row
                        Note = txtSplitNote.Text                    // Copy note, if any, into refresh row
                    };
                    rows.Add(row);                                  // Add the row to the list of rows

                    if (i == RowToAdd)                              // If == we're at the right point to add a new row, as requested
                    {
                        GLCodeSplitRow addedrow = new GLCodeSplitRow();   // Create an empty row
                        rows.Add(addedrow);                         // Add it to the list
                    }
                }
            }
            gvExpSplit.DataSource = rows;                         // Give the rows to the gridview
            gvExpSplit.DataBind();

            RecalculateTotalDollarAmount();                         // Update the "master" amount
            return;
        }

        // We have a full split gridview. Now adjust the operation of the page to process splits

        void EnergizeSplit()
        {
            ddlProjectClass.Enabled = false;                        // Can't use "Project Class" field any more
            txtAmount.Enabled = false;                              // Can't use "Total Dollar Amount" field any more
            ddlGLCode.Enabled = false;                              // Can't use "Expense Account" drop down list any more
            ddlGLCode.ClearSelection();                             // Deselect this item in the "parent" list - it's inactive now
            rfvGLCode.Enabled = false;                              // Turn off the RequiredFieldValidation of GL code - it's not required now
            btnSplit.Text = "Cancel";                               // Reverse the meaning of the Split button
            pnlExpenseSplit.Visible = true;                         // Turn on the grid for splits
            return;
        }

        // We have a request with no split rows and the "Split" button has been pressed.
        // Create an "empty" gridview row and prime it with existing fields from the page, if any

        void LoadFirstSplitRow()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // Case 1) This request has never had splits before.

                List<GLCodeSplitRow> rows = new List<GLCodeSplitRow>(); // Create a list of rows for the gridview
                GLCodeSplitRow row = new GLCodeSplitRow()               // Prime the first row of the grid from "parent" fields of the page
                {
                    TotalRows = 1,                                      // This is the only row in the grid
                    SelectedGLCodeID = ddlGLCode.SelectedValue,         // Copy the selected GL Code, if any, from the ddl
                    SelectedProjectClassID = ddlProjectClass.SelectedValue, // Copy the selected Project Code, if any, from the ddl
                };
                decimal a = ExtensionActions.LoadTxtIntoDecimal(txtAmount); // Fetch current value of Total Dollar Amount field
                row.Amount = ExtensionActions.LoadDecimalIntoTxt(a);    // Pretty up that amount for display

                rows.Add(row);                                          // Add the first (and only) row

                gvExpSplit.DataSource = rows;                           // Give the rows to the gridview
                gvExpSplit.DataBind();                                  // And bind them to display the rows, firing RowDataBound for each row
                litSplitError.Visible = false;                          // Begin with the error message turned off
            }
            return;
        }

        // Unload split expense lines into database rows

        void UnloadSplitRows(int expID, GridView gv)
        {
            SplitActions.DeleteSplitRows(expID);                        // Get rid of any existing GLCodeSplit rows for this Request
            if (pnlExpenseSplit.Visible)                                // If true, splits are alive and rows need to be written to database
            {
                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    try
                    {
                        foreach (GridViewRow row in gv.Rows)            // Process rows of GridView one-by-one
                        {

                            // For this row, find the things we want to write down in the database

                            DropDownList ddlSplitGLCode = (DropDownList)row.FindControl("ddlSplitGLCode"); // Find drop down list
                            DropDownList ddlSplitProjectClass = (DropDownList)row.FindControl("ddlSplitProjectClass"); // And the other one
                            TextBox txtSplitAmount = (TextBox)row.FindControl("txtSplitAmount"); // Find the text box within gridview row
                            TextBox txtSplitNote = (TextBox)row.FindControl("txtSplitNote"); // Find the text box within gridview row

                            GLCodeSplit toSave = new GLCodeSplit         // Fill a new object for the row
                            {
                                RqstID = expID,                          // The Expense Request that owns us
                                GLCodeID = StateActions.UnloadDdl(ddlSplitGLCode), // The selected GL Code
                                ProjectClassID = StateActions.UnloadDdl(ddlSplitProjectClass), // The selected Project Class
                                Amount = ExtensionActions.LoadTxtIntoDecimal(txtSplitAmount), // The dollar amount
                                Note = txtSplitNote.Text                // The note text, if any
                            };
                            context.GLCodeSplits.Add(toSave);           // Save new Rqst row
                        }
                        context.SaveChanges();                          // Commit the Adds
                    }
                    catch (Exception ex)
                    {
                        LogError.LogDatabaseError(ex, "EditExpense", "Error writing GLCodeSplit rows"); // Fatal error
                    }
                }
            }
            return;
        }

        // Validate the Split gridview. Check that every row has a valid amount and selected expense account

        bool ValidateSplitGridView()
        {
            if (pnlExpenseSplit.Visible)                                // If true, splits are alive
            {
                foreach (GridViewRow r in gvExpSplit.Rows)              // Cycle through rows, one-by-one
                {
                    DropDownList ddlSplitGLCode = (DropDownList)r.FindControl("ddlSplitGLCode"); // Find drop down list
                    DropDownList ddlSplitProjectClass = (DropDownList)r.FindControl("ddlSplitProjectClass"); // And the other one
                    TextBox txtSplitAmount = (TextBox)r.FindControl("txtSplitAmount"); // Find the text box within gridview row
                    TextBox txtSplitNote = (TextBox)r.FindControl("txtSplitNote"); // Find the text box within gridview row

                    if ((ExtensionActions.LoadTxtIntoDecimal(txtSplitAmount) == -1) || // Carefull check for a valid decimal value in amount. If == error
                        (ddlSplitGLCode.SelectedIndex <= 0))      // If <= nothing selected, that's an error
                    {
                        litSplitError.Visible = true;               // Turn on the error message
                        return false;                               // Report the error
                    }
                }
            }
            litSplitError.Visible = false;                          // In case error message had been on before, turn it off
            return true;                                            // Return a green light
        }

        // Load the Supporting Listbox control from the database.

        void LoadSupportingDocs(Exp rqst)
        {
            if (pnlSupporting.Visible)                              // If true the ListBox is visible so we need to fill it
            {
                SupportingActions.LoadDocs(RequestType.Expense, rqst.ExpID, lstSupporting, litDangerMessage); // Do the heavy lifting
            }
            return;
        }

        // Unload files from Listbox control. That is, move a supporting file from temporary to permanent status.

        void UnloadSupportingDocs(int expID)
        {
            if (pnlSupporting.Visible)                              // If true, panel containing ListBox control is visible. Work to do
            {
                SupportingActions.UnloadDocs(lstSupporting, litSavedUserID.Text, RequestType.Expense, expID, litDangerMessage);
            }
            return;
        }

        // Sum all the amount fields in the gridview. Use it to update the "master" amount field.

        void RecalculateTotalDollarAmount ()
        {
            decimal totalDollarAmount = 0;                          // Accumulator for dollar amount
            foreach (GridViewRow r in gvExpSplit.Rows)              // Let's try this construction for goofs
            {
                TextBox txtSplitAmount = (TextBox)r.FindControl("txtSplitAmount"); // Find the text box within gridview row
                decimal rowAmount = ExtensionActions.LoadTxtIntoDecimal(txtSplitAmount); // Convert the text into decimal, carefully
                if (rowAmount != -1)                                // If != there's a legitimate value here
                    totalDollarAmount += rowAmount;                 // Accumulate amount from this row     
            }
            txtAmount.Text = ExtensionActions.LoadDecimalIntoTxt(totalDollarAmount); // Update "master" amount with total of all rows
            return;
        }
    }
}
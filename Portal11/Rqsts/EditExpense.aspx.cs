using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
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
        //      ProjectRole - the role of the user on this project (Optional. ProjectInfo cookie used as backup.)
        //      RequestID - the database ID of the existing Expense Request, if any, to process
        //      Command - "New," "Copy," "Edit," "View," or "Revise" (Required)
        //      Return - the URL of the page that invoked us (Required)

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // Find the User's role on the Project. In the process, make sure user is logged in.

                string projRole = QueryStringActions.GetProjectRole();  // Fetch user's Project Role from query string or cookie

                // Fetch and validate the other Query String parameters. Carefully convert the integer parameters to integers.

                string userID = QueryStringActions.GetUserID();         // First parameter - User ID. If absent, check UserInfoCookie
                QSValue projectID = QueryStringActions.GetProjectID();  // Second parameter - Project ID. If absent, check ProjectInfoCookie
                QSValue expID = QueryStringActions.GetRequestID();      // Third parameter - Expense ID. If absent, must be a New command
                string cmd = QueryStringActions.GetCommand();           // Fourth parameter - Command. Must be present
                string ret = QueryStringActions.GetReturn();

                // Stash these parameters into invisible literals on the current page.

                litSavedCommand.Text = cmd;
                litSavedExpID.Text = "";                                // Assume New or Copy - do not modify an existing row, but add a new one
                litSavedProjectID.Text = projectID.String;
                litSavedProjectRole.Text = projRole;                    // Save in a faster spot for later
                litSavedReturn.Text = ret;
                litSavedUserID.Text = userID;

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

                    // Fetch the row from the database. Set the radio button corresponding to our Expense type,
                    // then disable the radio button group so the user can't change the Expense type.
                    // Make appropriate panels visible, then fill in the panels using data rom the existing Expense. Lotta work!

                    case PortalConstants.QSCommandEdit:                 // Process an "Edit" command. Read existing Expense, save it in same row
                        {
                            if (expID.Int == 0)                     // If == cannot edit a missing Exp
                                LogError.LogQueryStringError("EditExpense", "Missing Query String 'RequestID'"); // Log fatal error

                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                Exp exp = context.Exps.Find(expID.Int); // Fetch Exp row by its key
                                if (exp == null)
                                    LogError.LogInternalError("EditExpense", $"Unable to locate ExpenseID '{expID.String}' in database"); // Log fatal error

                                CommonEditReviseView(exp);          // Do common work
 
                                // Tidy up the Return Note based on the state of the request. The form wakes up with 
                                //      pnlReturnNote.Visible = false
                                //      txtReturnNote.Enabled = false

                                if (StateActions.StateToEditReturnNote(exp.CurrentState)) // If true, this state can be edited
                                {
                                    pnlReturnNote.Visible = true;       // Make the panel visible
                                    txtReturnNote.Enabled = true;       // Make panel editable
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(txtReturnNote.Text)) // If true there is text in the Return Note
                                    {
                                        pnlReturnNote.Visible = true;   // Make the panel visible. Leave it disabled
                                        btnReturnNoteClear.Visible = false; // Can't clear panel, so no Clear button
                                    }
                                }
                            }
                            litSuccessMessage.Text = "Expense Request is ready to edit";
                            break;
                        }

                    // We don't have to "fill" any of the controls on the page, just use them in their initial state.
                    // The radio button group for Expense type wakes up "enabled" with nothing selected. No other controls are visible.
                    // When the user selects a Expense type, we'll use that click to make the appropriate panels visible.

                    case PortalConstants.QSCommandNew:              // Process a "New" command. Create new, empty Expense, save it in new row
                        {
                            ExpState newState = StateActions.FindUnsubmittedExpState(projRole); // Figure out which flavor of Unsubmitted state
                            litSavedStateEnum.Text = newState.ToString(); // Stash actual enum value of ExpState
                            txtState.Text = EnumActions.GetEnumDescription(newState); // Display "English" version of enum
                            btnShowHistory.Visible = false;             // New request has no history, so don't show meaningless button

                            litSuccessMessage.Text = "New Expense Request is ready to edit";
                            break;
                        }
                    case PortalConstants.QSCommandRevise:               // Like an Edit at the start
                        {
                            Page.Title = "Revise Expense Request";      // Note that we are revising, not editing
                            goto case PortalConstants.QSCommandEdit;    // Proceed as though editing
                        }

                    // Fetch the row from the database. Set the radio button corresponding to our Expense type,
                    // then disable the radio button group so the user can't change the Expense type.
                    // Make appropriate panels visible, then fill in the panels using data from the existing Expense. 
                    // Set everything to readonly to prevent editing. Disable Save and Submit. Lotta work!

                    case PortalConstants.QSCommandView:             // Process an "View" command. Read existing Expense, disable edit and save
                        {
                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                Exp exp = context.Exps.Find(expID.Int); // Fetch Exp row by its primary key
                                if (exp == null)
                                    LogError.LogInternalError("EditExpense", $"Unable to find Expense ID '{expID.String}' in database"); // Fatal error

                                CommonEditReviseView(exp);          // Do the common setup work
                                ReadOnlyControls();                 // Make all controls readonly for viewing

                                if (!string.IsNullOrEmpty(txtReturnNote.Text)) // If false there is text in the Return Note. Display it
                                    pnlReturnNote.Visible = true;   // Return Note is always visible while Viewing

                                // Adjust the button settings for the View function

                                btnNewEntity.Visible = false; btnNewPerson.Visible = false; // Can't add new stuff while viewing
                                btnCancel.Text = "Done";            // Provide a friendly way out of this page
                                btnSave.Enabled = false;            // No "Save" for you!
                                btnSubmit.Enabled = false;

                                pnlAdd.Visible = false;             // Can't figure out how to disable Add button, so make it invisible
                                btnRem.Visible = false;             // Remove button on Supporting Docs is also invisible

                                Page.Title = "View Expense Request";    // Note that we are viewing, not editing
                                litSuccessMessage.Text = "Expense Request is ready to view";
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
                litDangerMessage.Text = ""; litSuccessMessage.Text = ""; litSupportingError.Visible = false; // Start with a clean slate of message displays
                if (fupUpload.HasFile)                              // If true PostBack was caused by File Upload control
                {
                    SupportingActions.UploadDoc(fupUpload, lstSupporting, litSavedUserID, litSuccessMessage, litDangerMessage); // Do heavy lifting to get file
                                                                                                                                // and record information in new SupportingDocTemp row
                    litSDError.Text = litDangerMessage.Text;        // Replicate message, if any, in more visible location                                                                                                                    
                }
            }
            return;
        }

        // Things that Edit, Revise, and View have in common

        void CommonEditReviseView(Exp exp)
        {
            litSavedExpID.Text = exp.ExpID.ToString();      // Remember to write record back to its original spot, i.e., modify the row

            // Note: The rdo displays TEXT that matches the Description of each ExpType, but the VALUE contains the enumerated ExpType

            rdoExpType.SelectedValue = exp.ExpType.ToString(); // Select the button corresponding to our type
            rdoExpType.Enabled = false;                     // Disable the control - cannot change type of existing Expense

            EnablePanels(exp.ExpType);                      // Make the relevant panels visible
            LoadPanels(exp);                                // Fill in the visible panels from the Expense

            if (SplitActions.LoadSplitRows(RequestType.Expense, exp.ExpID, gvExpSplit)) // If true, GLCodeSplits existed and were loaded
                EnergizeSplit();                            // Adjust page to accommodate split gridview

            LoadSupportingDocs(exp);                        // Fill in the Supporting Docs list
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
            txtAmount.Text = "";                                        // Wipe out result of previous computation, if any
            return;
        }

        // User is in the midst of entering quantity, cost per item and amount. Try to calculate the amount for them.

        protected void txtNumberOfCards_TextChanged(object sender, EventArgs e)
        {
            if (ExtensionActions.FillQCA(txtNumberOfCards, txtEachCard, txtAmount)) // if true successfully formatted everything
                txtAmount.Focus();                                      // Focus on to the read-only amount field so user can keep tabbing through page
        }

        protected void txtGoodsCostPerUnit_TextChanged(object sender, EventArgs e)
        {
            if (ExtensionActions.FillQCA(txtGoodsQuantity, txtGoodsCostPerUnit, txtAmount)) // If true successfully formatted everything
                txtAmount.Focus();                                      // Focus on to the read-only amount field so user can keep tabbing through page
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

        // One of the radio buttons in the "Payment Method" panel has clicked. 
        // There's only one button that makes a difference: EFT, which wants Delivery Instructions off.
        // Which Delivery Instructions are visible depends on 1) Expense Type (PO gets special treatment) and 2) Payment Method (EFT gets special treatment).
        // So we do this in a common method.

        protected void rdoPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdjustDeliveryInstructions("");                             // Make appropriate panels turn on and off
            return;
        }

        // One of the radio buttons in the "Source of Funds" panel has clicked. Switch the Project Class drop down list on or off.
        // In addition, redraw the split gridview to reflect the appearance or disappearance of the Project Code.

        //protected void rdoSourceOfFunds_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (rdoSourceOfFunds.SelectedValue == PortalConstants.RDOFundsRestricted) // If == Restricted button was clicked. Turn on drop down list
        //        pnlProjectClass.Visible = true;                     // Turn on drop down list
        //    else
        //        pnlProjectClass.Visible = false;                    // Turn off drop down list
        //    if (!ddlGLCode.Enabled)                                 // If false, the ddl is turned off so Split must be on. Refresh
        //        CopySplitGridView();                                // Refresh the Split grid view, reflecting the change in the Source of Funds button
        //    return;
        //}

        // Delivery Instructions - radio buttons and check boxes.

        protected void rdoDeliveryModeReg_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdjustDeliveryModeReg("");                                  // Turn the right things on and off
            return;
        }

        protected void rdoDeliveryModePO_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdjustDeliveryModePO("");                                   // Turn the right things on and off
            return;
        }

        // One of the Delivery Mode checkboxes has been checked. Now truth be told, there's only one checkbox - rush

        protected void cblDeliveryModeRush_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cblDeliveryModeRush.Items.FindByValue(PortalConstants.DeliveryInstructionsRush).Selected) // If true Rush is checked
            {
                ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Fetch project role and convert to enum
                if (RoleActions.ProjectRoleIsStaff(projectRole))        // If true user is a staff member. No need for modal dialog box to confirm.
                    return;                                             // Leave the Rush box checked
                else
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "divModalRush", "$('#divModalRush').modal();", true); // Open the modal dialog to confirm
            }
            return;
        }

        // The user has pressed the "No" button in the modal dialog box for Rush

        protected void btnModalRushNo_Click(object sender, EventArgs e)
        {
            cblDeliveryModeRush.Items.FindByValue(PortalConstants.DeliveryInstructionsRush).Selected = false; // Uncheck the Rush checkbox
            return;
        }
        //protected void rdoDeliveryMode_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    DeliveryMode mode = EnumActions.ConvertTextToDeliveryMode(rdoDeliveryMode.SelectedValue); // Fetch selected value (text) and convert to Delivery Mode (enum)
        //    switch (mode)
        //    {
        //        case DeliveryMode.Pickup:
        //            {
        //                pnlDeliveryAddress.Visible = false;             // Turn off delivery address
        //                return;
        //            }
        //        case DeliveryMode.MailPayee:
        //            {
        //                FillDeliveryAddress(litSavedEntityPersonFlag.Text, ddlEntity, ddlPerson, txtDeliveryAddress);
        //                pnlDeliveryAddress.Visible = true;              // Turn on the panel
        //                txtDeliveryAddress.ReadOnly = true;             // Only for viewing
        //                return;
        //            }
        //        case DeliveryMode.MailAddress:
        //            {
        //                txtDeliveryAddress.Text = "";                   // Clear out any prior address entry
        //                pnlDeliveryAddress.Visible = true;              // Turn on delivery address
        //                txtDeliveryAddress.ReadOnly = false;            // Here, it's editable
        //                return;
        //            }
        //        default:
        //            LogError.LogInternalError("EditExpense", $"Invalid DeliveryMode value '{mode.ToString()}' encountered"); // Fatal error
        //            break;
        //    }
        //    return;
        //}

        //protected void rdoPODeliveryMode_SelectedIndexChanged(object sender, EventArgs e) // Special for Purchase Orders
        //{
        //    pnlDeliveryAddress.Visible = (rdoPODeliveryMode.SelectedValue == PODeliveryMode.DeliverAddress.ToString()); // If == Yes, need to look for an Address
        //    return;
        //}

        protected void rdoPOVendorMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlEntity.Visible = (rdoPOVendorMode.SelectedValue != POVendorMode.Yes.ToString()); // If != Yes, need to look for an Entity
            return;
        }

        // The user has pressed the New button next to the Entity DDL or Person DDL.
        //  1) Open a modal dialog box asking if its OK to save the current request.
        //  2) If Yes is pressed, save the current request. Note: This may be its first save, so we may get a new requestID out of the process.
        //  3) Use the FowControl cookie to remember that we want to come back to Edit Deposit and resume editing.
        //  4) Open the AssignEntitysToProject/AssignPersonsToProject page with its return set to FlowControl.

        protected void btnNewEntity_Click(object sender, EventArgs e)
        {
            litSavedEntityPersonFlag.Text = RequestType.Entity.ToString(); // Remember where we go to process the "New" command
            CommonNewButton(sender, e);                             // Continue in processing common to New Entity and New Person
            return;                                                 // Open that modal dialog box
        }

        protected void btnNewPerson_Click(object sender, EventArgs e)
        {
            litSavedEntityPersonFlag.Text = RequestType.Person.ToString();  // Remember where we go to process the "New" command
            CommonNewButton(sender, e);                             // Continue in processing common to New Entity and New Person
            return;                                                 // Open that modal dialog box
        }

        void CommonNewButton(object sender, EventArgs e)
        {
            ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Fetch project role and convert to enum
            if (RoleActions.ProjectRoleIsStaff(projectRole))        // If true user is a staff member. No need for modal dialog box to confirm.
                btnModalNewYes_Click(sender, e);                    // Act as though the "Yes" button in the modal dialog box was pushed.
            else
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "divModalNew", "$('#divModalNew').modal();", true); // Open the modal dialog to confirm
            return;                                                 // Open that modal dialog box
        }

        // The "Yes" button for the modal dialog box associated with the New button on Entity and Person. Continue the magic.

        protected void btnModalNewYes_Click(object sender, EventArgs e)
        {
            int expID = SaveExp();                                  // Save the request, return the request ID
            if (expID == 0) return;                                 // If == hit an error. Let user retry
            CookieActions.MakeFlowControlCookie(PortalConstants.URLEditExpense + "?"
                                            + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + expID.ToString() + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit + "&" // Start with request just saved
                                            + PortalConstants.QSReturn + "=" + litSavedReturn.Text);
            if (litSavedEntityPersonFlag.Text == RequestType.Entity.ToString()) // If == we are processing an Entity
            {
                RedirectRequest(PortalConstants.URLAssignEntitysToProject, // Do the heavy lifting to get there. Return here to continue editing.
                    "&" + PortalConstants.QSEntityRole + "=" + EntityRole.ExpenseVendor.ToString()); // Tell Assign the type of Entity
            }
            else if (litSavedEntityPersonFlag.Text == RequestType.Person.ToString()) // If == we are processing a Person
            {
                RedirectRequest(PortalConstants.URLAssignPersonsToProject, // Do the heavy lifting to get there. Return to continue editing.
                    "&" + PortalConstants.QSPersonRole + "=" + litSavedPersonEnum.Text); // Tell Assign the type of Person
            }
            LogError.LogInternalError("EditExpense", $"Invalid SavedEntityPersonFlag value '{litSavedEntityPersonFlag.Text}' encountered"); // Fatal error
        }

        void RedirectRequest(string dest, string qs)
        {
            Response.Redirect(dest + "?" + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text 
                                    + qs
                                    + "&" + PortalConstants.QSReturn + "=" + PortalConstants.URLFlowControl); // When done, return to FLow Control to sort things out
        }

        // The user has pressed the Split button. Flip the split-age.

        protected void btnSplit_Click(object sender, EventArgs e)
        {
            if (ddlGLCode.Enabled)                                      // If true, the function is "Split"
            {
                LoadFirstSplitRows();                                   // Load gridview with initial values
                EnergizeSplit();                                        // Turn the gridview on and other things off
            }
            else                                                        // The function is "Cancel"
            {
                DeEnergizeSplit();                                      // Turn everything off
                if (!string.IsNullOrEmpty(litSavedExpID.Text))          // If false we have an ExpID in the database, so we might have split rows
                    SplitActions.DeleteSplitRows(RequestType.Expense, Convert.ToInt32(litSavedExpID.Text)); // Delete the split rows
            }
            return;
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
                else
                    splitGLCode.ClearSelection();                       // Otherwise, no row selected

                // Repeat the process with the ddlProjectClass drop down list

                splitProjectClass.Items.AddRange(ddlProjectClass.Items.Cast<ListItem>().Select(x => new ListItem(x.Text, x.Value)).ToArray());
                // Copy each ListItem from ddlProjectClass to splitProjectClass ddl

                if (selectedProjectClassID.Text != "")              // If != there is a selected row. Selection is the ProjectClassID of the row
                    splitProjectClass.SelectedValue = selectedProjectClassID.Text; // Select that row
                else
                    splitProjectClass.SelectedValue = litSavedDefaultProjectClassID.Text; // Select the "global" default row

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
            btnViewLink.Enabled = true; btnViewLink.Visible = true;
            btnViewLink.NavigateUrl = SupportingActions.ViewDocUrl((ListBox)sender); // Formulate URL to launch viewer page
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

        // Clear buttons on each Note text box

        protected void btnNotesClear_Click(object sender, EventArgs e)
        {
            txtNotes.Text = "";
            return;
        }

        protected void btnReturnNoteClear_Click(object sender, EventArgs e)
        {
            txtReturnNote.Text = "";
            return;
        }
        protected void btnStaffNoteClear_Click(object sender, EventArgs e)
        {
            txtStaffNote.Text = "";
            return;
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

        // Cancel button has been clicked. Just return to the Dashboard.

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(litSavedReturn.Text);
        }

        // Revise button clicked. This is the case where a Exp is in the "Returned" state - it was submitted, but failed during the review process.
        // To "Revise," we: 
        //  1) Get a copy of the Exp
        //  2) Create a ExpHistory row to audit this change.
        //  3) Change the State of the Rsq from "Returned" to "Under Construction," erase the ReturnNote comments and save it.
        //  4) Call this page back and invoke the "Edit" command for an existing Expense.

//        protected void btnRevise_Click(object sender, EventArgs e)
//        {
//            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
//            {
//                try
//                {

//                    //  1) Get a copy of the Exp

//                    int expID = Convert.ToInt32(litSavedExpID.Text); // Fetch the ID of the Exp row to be revised
//                    Exp toRevise = context.Exps.Find(expID);        // Fetch the Exp that we want to update
//                    if (toRevise == null)                           // If == the target Exp not found
//                        LogError.LogInternalError("EditExpense", $"ExpenseID from Query String '{expID.ToString()}' could not be found in database"); // Log fatal error

//                    //  2) Create a ExpHistory row to audit this change.

//                    ExpHistory hist = new ExpHistory();             // Get a place to build a new ExpHistory row
//                    StateActions.CopyPreviousState(toRevise, hist, "Revised"); // Create a ExpHistory log row from "old" version of Expense
////                    hist.ReturnNote = toRevise.ReturnNote;          // Save the Note from the Returned Rqst

//                    //  3) Change the State of the Rqst from "Returned" to "Unsubmitted," erase the ReturnNote comments and save it.

//                    StateActions.SetNewExpState(toRevise, StateActions.FindUnsubmittedExpState(litSavedProjectRole.Text), litSavedUserID.Text, hist); // Write down our current State and authorship
//                    toRevise.ReturnNote = "";                       // Erase the note
//                    toRevise.EntityNeeded = false; toRevise.PersonNeeded = false; // Assume "Returner" did as we asked
//                    context.ExpHistorys.Add(hist);                  // Save new ExpHistory row
//                    context.SaveChanges();                          // Commit the Add and Modify
//                }
//                catch (Exception ex)
//                {
//                    LogError.LogDatabaseError(ex, "EditExpense", "Error processing ExpHistory and Rqst rows"); // Fatal error
//                }

//                //  4) Call this page back and invoke the "Edit" command for an existing Expense.

//                Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text
//                            + "&" + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text
//                            + "&" + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text
//                            + "&" + PortalConstants.QSRequestID + "=" + Request.QueryString[PortalConstants.QSRequestID]
//                            + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit // Start with an existing Expense
//                            + "&" + PortalConstants.QSReturn + "=" + litSavedReturn.Text); // Propagate return page from caller
//            }
//        }

        // Save button clicked. Throw a modal to make sure the user really wants to save, not submit.
        // "Save" means that we unload all the controls for the Expense into a database row. 
        // If the Expense is new, we just add a new row. If the Expense already exists, we update it and add a history record to show the edit.
        // We can tell a Expense is new if the Query String doesn't contain a mention of a ExpID AND the literal litSavedExpID is empty.
        // On the first such Save, we stash the ExpID of the new Expense into this literal so that a second Save during the same session will find
        // it and update it, rather than creating a second row.

        protected void btnSave_Click(object sender, EventArgs e)
        {

            // Make sure the user really wants to save, not submit.

            ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Fetch project role and convert to enum
            if (!RoleActions.ProjectRoleIsStaff(projectRole))   // If false user is a project member. Throw the modal
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "divModalSave", "$('#divModalSave').modal();", true); // Open the modal dialog to confirm
                return;                                         // See you again in a minute
            }
            btnSave1_Click(sender, e);                          // Staff users don't need the modal. Do the heavy lifting.
            return;
        }

        // Split into two parts to help the modal dialog box work correctly

        protected void btnSave1_Click(object sender, EventArgs e)
        {

            int dummy = SaveExp();                                  // Do the heavy lifting
            if (dummy == 0) return;                                 // If == hit an error. Let user retry

            // Now go back to Dashboard

            Response.Redirect(litSavedReturn.Text + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                                  + PortalConstants.QSStatus + "=Expense Request saved");
        }

        // Submit button clicked. Save what we've been working on, set the Expense state to "Submitted," then go back to the dashboard.

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            // Before saving, make sure the Expense Split gridview contains healthy values. It's hard to validate these on-the-fly so do it here.

            if (!ValidateSplitGridView())                           // If false, there's a problem. Report the error
                return;                                             // Give user a chance to fix the problem

            // Before saving, make sure that a sufficient number of supporting documents are present. Note that a user can "Save" with
            // insufficent supporting docs, but can only "Submit" if the minimum is present. Write this error in two places.

            if (lstSupporting.Items.Count < Convert.ToInt32(litSupportingDocMin.Text)) // If < the minimum number of docs is not present
            {
                litDangerMessage.Text = "The Expense Request must include a minimum of " + litSupportingDocMin.Text + " Supporting Document.";
                litSuccessMessage.Text = "";                        // Just the danger message, not a stale success message
                litSupportingError.Text = litDangerMessage.Text;    // Copy the error to a second place, right at the Supporting Doc list box
                litSupportingError.Visible = true;                  // Make that error message visible
                return;                                             // Back for more punishment
            }

            int savedExpID = SaveExp();                             // Do the heavy lifting to save the current Expense
            if (savedExpID == 0)                                    // If == SaveExp encountered an error. Go no further
                LogError.LogInternalError("EditExpense", "Unable to save Expense before Submitting"); // Fatal error

            // SaveExp just saved the Request, which may or may not have written a ExpHistory row. But now, let's write another
            // ExpHistory row to describe the Submit action.

            string emailSent = ""; bool revising = false;
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    Exp toSubmit = context.Exps.Find(savedExpID);   // Find the Exp that we want to update
                    if (toSubmit == null)                           // If == could not find the Dep
                        LogError.LogInternalError("EditExpense", $"Unable to find saved Expense ID '{savedExpID}' in database"); // Fatal error

                    string temp = litSavedStateEnum.Text;                           // ** Temp **

                    // If this request is being submitted for the first time, write down the project role that did the submit.
                    // If the request has to be returned, we'll use this to return it to the originator role.

                    if (StateActions.RequestIsUnsubmitted(toSubmit.CurrentState)) // If true, request is in an unsubmitted state
                        toSubmit.SubmitProjectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Note which role did Submit.

                    ExpHistory hist = new ExpHistory();                 // Get a place to build a new ExpHistory row
                    StateActions.CopyPreviousState(toSubmit, hist, "Submitted"); // Fill the ExpHistory row from "old" version of Expense

                    // Deal with Return Note. This is the only place in the workflow where the Return Note gets cleared out.

                    if (!StateActions.StateToEditReturnNote(toSubmit.CurrentState)) // If false current user couldn't edit the note
                        toSubmit.ReturnNote = "";                   // So clear it out

                    revising = StateActions.RequestIsRevising(toSubmit.CurrentState); // Remember where we are in the process
                    ExpState nextState = StateActions.FindNextState(toSubmit.CurrentState, ReviewAction.Submit, toSubmit.SubmitProjectRole); // Figure out what next state is. Nuanced.
                    StateActions.SetNewState(toSubmit, nextState, litSavedUserID.Text, hist); // Move to that state
                    toSubmit.SubmitUserID = litSavedUserID.Text;        // Remember who submitted this Exp. They get notification on Return.

                    context.ExpHistorys.Add(hist);                      // Save new ExpHistory row
                    context.SaveChanges();                              // Update the Exp with new fields

                    // If it's a rush request, first send a broadcast "INCOMING!" email to project staff. Then send the regular email with rush status

                    if (!revising)                                      // If false request progressing normally, send email(s)
                    {
                        if (toSubmit.Rush)                                  // If true, this is a rush request. Send broadcast email to alert staff
                            EmailActions.BroadcastEmailToAllStaff(toSubmit.Rush, // Send email to all staff members. Rush email if rush request
                              StateActions.UserRoleToProcessRequest(nextState), // Tell next reviewer, who is in this role
                              toSubmit.ProjectID,                             // Request is associated with this project
                              toSubmit.Project.Name,                          // Project has this name (for parameter substitution)
                              EnumActions.GetEnumDescription(RequestType.Expense), // This is an Expense Request
                              EnumActions.GetEnumDescription(toSubmit.ExpType), // Type of Expense Request, e.g., PEX Card
                              EnumActions.GetEnumDescription(nextState),      // Here is its next state
                              PortalConstants.CEmailDefaultExpenseBroadcastSubject, PortalConstants.CEmailDefaultExpenseBroadcastBody); // Use this subject and body, if needed

                        emailSent = EmailActions.SendEmailToReviewer(toSubmit.Rush, // Send email to next reviewer. Rush email if rush request
                            StateActions.UserRoleToProcessRequest(nextState), // Tell next reviewer, who is in this role
                            toSubmit.ProjectID,                             // Request is associated with this project
                            toSubmit.Project.Name,                          // Project has this name (for parameter substitution)
                            EnumActions.GetEnumDescription(RequestType.Expense), // This is an Expense Request
                            EnumActions.GetEnumDescription(nextState),      // Here is its next state
                            EnumActions.GetEnumDescription(toSubmit.ExpType), // Type of Expense Request, e.g., PEX Card
                            PortalConstants.CEmailDefaultExpenseApprovedSubject, PortalConstants.CEmailDefaultExpenseApprovedBody); // Use this subject and body, if needed
                    }
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditExpense", "Unable to update Expense into Submitted state"); // Fatal error
                }
            }

            // Now go back to Dashboard or wherever

            if (!revising)                                          // If false just a regular next step
            {
                Response.Redirect(litSavedReturn.Text
                    + "?" + PortalConstants.QSSeverity
                    + "=" + PortalConstants.QSSuccess
                    + "&" + PortalConstants.QSStatus + "=Expense Request submitted." + emailSent);
            }
            else
            {
                Response.Redirect(PortalConstants.URLReviewExpense
                    + "?" + PortalConstants.QSSeverity
                    + "=" + PortalConstants.QSSuccess
                    + "&" + PortalConstants.QSStatus + "=Document Request revised. Please continue review."
                    + "&" + PortalConstants.QSRequestID + "=" + savedExpID.ToString()  // Start with just-saved request
                    + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview // Review it
                    + "&" + PortalConstants.QSReturn + "=" + litSavedReturn.Text); // Return to our caller when done

            }
        }

        // Show History Button clicked. Open up and fill a GridView of all the ExpHistory rows for this ExpenseRequest

        protected void btnShowHistory_Click(object sender, EventArgs e)
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
                        ProjectClassID = src.ProjectClassID,
                        ReviseUserRole = UserRole.None,             // No revisions of a new request
                        StaffNote = "",                             // Clear out the staff note, if any
                        CurrentTime = System.DateTime.Now,
                        CurrentState = StateActions.FindUnsubmittedExpState(litSavedProjectRole.Text),
                        CurrentUserID = litSavedUserID.Text         // Current user becomes creator of new Rqst
                    };

                    context.Exps.Add(dest);                         // Store the new row
                    context.SaveChanges();                          // Commit the change to product the new Rqst ID

                    //  2) Copy the split rows, if any.

                    SplitActions.CopySplitRows(RequestType.Expense, src.ExpID, dest.ExpID); // Copy all the split rows, if any

                    //  3) Copy the Supporting Docs. We know that in the source Rqst, all the supporting docs are "permanent," i.e., we don't need
                    //  to worry about "temporary" docs.

                    SupportingActions.CopyDocs(context, RequestType.Expense, src.ExpID, dest.ExpID); // Copy each of the Supporting Docs

                    //  4) Create a ExpHistory row to describe the copy

                    ExpHistory hist = new ExpHistory();
                    StateActions.CopyPreviousState(src, hist, "Copied"); // Fill fields of new record
                    hist.ExpID = dest.ExpID; hist.NewExpState = dest.CurrentState; hist.ReturnNote = dest.ReturnNote; // Add fields from copied App

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
                        //  5) Unload the splits and supporting documents

                        Exp toSave = new Exp(){                     // Get a place to hold everything
                            Inactive = false,
                            Archived = false,
                            ProjectID = QueryStringActions.ConvertID(litSavedProjectID.Text).Int, // Connect Expense to Project
                            ReviseUserRole = UserRole.None,         // No revisions (yet) of a new request
                            CreatedTime = System.DateTime.Now,      // Stamp time when Expense was first created as "now"
                            BeginningDate = (DateTime)SqlDateTime.MinValue, // Non-null values to satisfy SQL
                            EndingDate = (DateTime)SqlDateTime.MinValue,
                            DateNeeded = (DateTime)SqlDateTime.MinValue,
                            DateOfInvoice = (DateTime)SqlDateTime.MinValue
                        };
                        UnloadPanels(toSave);                       // Move from Panels to record
                        StateActions.SetNewState(toSave, EnumActions.ConvertTextToExpState(litSavedStateEnum.Text), litSavedUserID.Text);
                                                                    // Write down our current State and authorship
                        context.Exps.Add(toSave);                   // Save new Rqst row
                        context.SaveChanges();                      // Commit the Add
                        litSavedExpID.Text = toSave.ExpID.ToString(); // Show that we've saved it once

                        // Save split rows and supporting documents, if any

                        if (pnlExpenseSplit.Visible)                // If true, splits are alive and rows need to be written to database
                            SplitActions.UnloadSplitRows(RequestType.Expense, toSave.ExpID, gvExpSplit); // Save the splits from the gridview to the database
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
                        //  4) Create a new ExpHistory row to preserve information about the previous Save (removed as obsolete)
                        //  5) Unload the splits and supporting documents

                        Exp toUpdate = context.Exps.Find(expID);    // Fetch the Rqst that we want to update
                        UnloadPanels(toUpdate);                     // Move from Panels to Rqst, modifying it in the process
                        //ExpHistory hist = new ExpHistory();         // Get a place to build a new ExpHistory row
                        //StateActions.CopyPreviousState(toUpdate, hist, "Saved"); // Create a ExpHistory log row from "old" version of Expense
                        //StateActions.SetNewState(toUpdate, hist.PriorExpState, litSavedUserID.Text, hist); // Write down our current State and authorship
                        //context.ExpHistorys.Add(hist);              // Save new ExpHistory row
                        context.SaveChanges();                      // Commit the Add or Modify

                        // Save split rows and supporting documents, if any

                        if (pnlExpenseSplit.Visible)                // If true, splits are alive and rows need to be written to database
                            SplitActions.UnloadSplitRows(RequestType.Expense, toUpdate.ExpID, gvExpSplit); // Save the splits from the gridview to the database
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
            pnlAmount.Visible = true; txtAmount.Enabled = true;     // Unconditionally visible (or invisible) for all Expense types
            pnlDescription.Visible = true;
            pnlDeliveryAddress.Visible = false;
            pnlExpenseSplit.Visible = false; btnSplit.Text = "Split";
            pnlGLCode.Visible = true; ddlGLCode.Enabled = true; btnSplit.Visible = false;
            pnlNotes.Visible = true;
            pnlProjectClass.Visible = true;
            if (RoleActions.ProjectRoleIsStaff(EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text))) // If true user is a staff member; can see staff note
                pnlStaffNote.Visible = true;                        // The staff member can see the staff note
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
                        pnlDeliveryInstructions.Visible = true; pnlDeliveryModeReg.Visible = true; pnlDeliveryModePO.Visible = false; pnlDeliveryModeRush.Visible = true;
                        pnlEntity.Visible = false;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true; ExtensionActions.EnableRdoListItems(rdoPaymentMethod, eft: false);
                        pnlPerson.Visible = true; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.Contractor); litSavedPersonEnum.Text = PersonRole.Contractor.ToString();
                        pnlPOFulFillmentInstructions.Visible = false;
                        litSavedEntityPersonFlag.Text = RequestType.Person.ToString(); // Remember that we are dealing with a Person
                        litSupportingDocMin.Text = "1";
                        break;
                    }
                case ExpType.PEXCard:
                    {
                        lblAmount.Text = "Total Amount"; txtAmount.ReadOnly = true;
                        pnlBeginningEnding.Visible = false;
                        pnlCards.Visible = true; txtAmount.Enabled = true;      // Cards calculate total amount. User can't fill it.
                        pnlContractQuestions.Visible = false;
                        pnlDateNeeded.Visible = true;
                        pnlDateOfInvoice.Visible = false;
                        pnlDeliveryInstructions.Visible = true; pnlDeliveryModeReg.Visible = false; pnlDeliveryModePO.Visible = false; pnlDeliveryModeRush.Visible = true;
                        pnlGLCode.Visible = false;
                        pnlGoods.Visible = false;
                        pnlEntity.Visible = false;
                        pnlPaymentMethod.Visible = false;
                        pnlPerson.Visible = true; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.ResponsiblePerson); litSavedPersonEnum.Text = PersonRole.ResponsiblePerson.ToString();
                        pnlPOFulFillmentInstructions.Visible = false;
                        litSavedEntityPersonFlag.Text = RequestType.Person.ToString(); // Remember that we are dealing with a Person
                        litSupportingDocMin.Text = "1";
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
                        pnlDeliveryInstructions.Visible = true; pnlDeliveryModeReg.Visible = true; pnlDeliveryModePO.Visible = false; pnlDeliveryModeRush.Visible = true;
                        pnlEntity.Visible = false;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true; ExtensionActions.EnableRdoListItems(rdoPaymentMethod, creditcard:false, invoice:false); // Enable correct methods
                        pnlPerson.Visible = true; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.Employee); litSavedPersonEnum.Text = PersonRole.Employee.ToString();
                        pnlPOFulFillmentInstructions.Visible = false;
                        litSavedEntityPersonFlag.Text = RequestType.Person.ToString(); // Remember that we are dealing with a Person
                        litSupportingDocMin.Text = "1";
                        break;
                    }
                case ExpType.PurchaseOrder:
                    {
                        lblAmount.Text = "Dollar Amount"; txtAmount.ReadOnly = true;
                        pnlBeginningEnding.Visible = false;
                        pnlCards.Visible = false;
                        pnlContractQuestions.Visible = false;
                        pnlDateNeeded.Visible = true;
                        pnlDateOfInvoice.Visible = false;
                        pnlDeliveryInstructions.Visible = true; pnlDeliveryModeReg.Visible = false; pnlDeliveryModePO.Visible = true; pnlDeliveryModeRush.Visible = true;
                        pnlEntity.Visible = true; litSavedEntityEnum.Text = EntityRole.ExpenseVendor.ToString();
                        pnlGoods.Visible = true;
                        pnlPaymentMethod.Visible = true; ExtensionActions.EnableRdoListItems(rdoPaymentMethod, eft: false);
                        pnlPerson.Visible = false;
                        pnlPOFulFillmentInstructions.Visible = true;
                        litSavedEntityPersonFlag.Text = RequestType.Entity.ToString(); // Remember that we are dealing with an Entity
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
                        pnlDeliveryInstructions.Visible = true; pnlDeliveryModeReg.Visible = true; pnlDeliveryModePO.Visible = false; pnlDeliveryModeRush.Visible = true;
                        pnlEntity.Visible = false;
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true; ExtensionActions.EnableRdoListItems(rdoPaymentMethod, creditcard:false, eft:false, invoice:false); // Enable relevant methods
                        pnlPerson.Visible = true; lblPerson.Text = EnumActions.GetEnumDescription(PersonRole.Recipient); litSavedPersonEnum.Text = PersonRole.Recipient.ToString();
                        pnlPOFulFillmentInstructions.Visible = false;
                        litSavedEntityPersonFlag.Text = RequestType.Person.ToString(); // Remember that we are dealing with a Person
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
                        pnlDeliveryInstructions.Visible = true; pnlDeliveryModeReg.Visible = true; pnlDeliveryModePO.Visible = false; pnlDeliveryModeRush.Visible = true;
                        pnlEntity.Visible = true; litSavedEntityEnum.Text = EntityRole.ExpenseVendor.ToString();
                        pnlGoods.Visible = false;
                        pnlPaymentMethod.Visible = true; ExtensionActions.EnableRdoListItems(rdoPaymentMethod, eft:false); // Enable almost all methods
                        pnlPerson.Visible = false;
                        pnlPOFulFillmentInstructions.Visible = false;
                        litSavedEntityPersonFlag.Text = RequestType.Entity.ToString(); // Remember that we are dealing with a Entity
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
                DdlActions.FillEntityDDL(ddlEntity, projID, litSavedEntityEnum.Text, entityID, needed); // Do the heavy lifting
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

                    DdlActions.LoadDdl(ddlGLCode, glcodeID, rows,
                        "-- Error: No GL Codes in Database --", "-- none selected --", alwaysDisplayLeader:true); // Put the cherry on top

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
                DdlActions.FillPersonDDL(ddlPerson, projID, litSavedPersonEnum.Text, personID, needed); // Do the heavy lifting
            }
            return;
        }

        // If the Funds panel is enabled, fill the drop down list from the database. Note that these values are drawn from
        // the ProjectClass table, which shows Project Classes assigned to this project. The nuance here is that the list of
        // Project Classes for this project may contain a Default - a Project Class that should be suggested to the User as their default choice.

        void FillProjectClassDDL(int? pcID)
        {
            if (pnlProjectClass.Visible)                                // If true need to populate list
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

                        DdlActions.LoadDdl(ddlProjectClass, defaultID, rows, 
                            " -- Error: No Project Classes assigned to Project --", "-- none selected --"); // Put the cherry on top
                    }
                }
            }
            return;
        }

        // Move values from the Expense record into the Page. Only move the values that are "Visible" for this Expense Type

        void LoadPanels(Exp record)
        {
            litSavedStateEnum.Text = record.CurrentState.ToString();  // Stash enum version of state
            txtState.Text = RequestActions.AdjustCurrentStateDesc(record); // Produce nuanced version of current state

            if (pnlAmount.Visible)
                txtAmount.Text = ExtensionActions.LoadDecimalIntoTxt(record.Amount);

            if (pnlBeginningEnding.Visible)
            {
                DateActions.LoadDateIntoTxtCal(record.BeginningDate, txtBeginningDate, calBeginningDate);
                DateActions.LoadDateIntoTxtCal(record.EndingDate, txtEndingDate, calEndingDate);
            }

            if (pnlCards.Visible)
            {
                txtNumberOfCards.Text = record.CardsQuantity.ToString();
                txtEachCard.Text = record.CardsValueEach.ToString("C");
                ExtensionActions.FillQCA(txtNumberOfCards, txtEachCard, txtAmount); // Try to get cute and fill a missing field
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

            if (pnlDateNeeded.Visible)
                DateActions.LoadDateIntoTxtCal(record.DateNeeded, txtDateNeeded, calDateNeeded);

            if (pnlDateOfInvoice.Visible)
            {
                DateActions.LoadDateIntoTxtCal(record.DateOfInvoice, txtDateOfInvoice, calDateOfInvoice);
                txtInvoiceNumber.Text = record.InvoiceNumber;
            }

            txtDescription.Text = record.Description;
            
            if (pnlEntity.Visible)
            {
                FillEntityDDL(record.EntityID, record.EntityNeeded);    // Pull the VendorID and load the DDL
                if (pnlPOFulFillmentInstructions.Visible)               // If true, controls that include the Vendor DDL are also visible
                {
                    ExtensionActions.LoadPOVendorModeIntoRdo(record.POVendorMode, rdoPOVendorMode); // Load enum value into Radio Button List

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
                ExtensionActions.FillQCA(txtGoodsQuantity, txtGoodsCostPerUnit, txtAmount); // Try to get cute and fill a missing field
            }

            if (pnlNotes.Visible)
                txtNotes.Text = record.Notes;

            if (pnlPaymentMethod.Visible)
                ExtensionActions.LoadEnumIntoRdo(record.PaymentMethod, rdoPaymentMethod); // Load enum value into Radio Button List
                                                                            // We will adjust delivery instructions below

            if (pnlPerson.Visible)
                FillPersonDDL(record.PersonID, record.PersonNeeded);        // Fill drop down list and highlight the selected item, if any

            if (pnlProjectClass.Visible)                                // If true, process Project Class
                FillProjectClassDDL(record.ProjectClassID);             // Fill the DDL 

            txtReturnNote.Text = record.ReturnNote;                     // Copy the text of the Return Note.

            if (pnlStaffNote.Visible)
                txtStaffNote.Text = record.StaffNote;

            //if (txtURL.Visible)
            //    txtURL.Text = record.URL;

            // Delivery Instructions can't be loaded until Entity and Person are loaded. Unconditionally load the radio buttons and checkbox.
            // Then react to the environment to make the right things visible.

            ExtensionActions.LoadEnumIntoRdo(record.DeliveryMode, rdoDeliveryModeReg); // Load enum value into Radio Button List
            ExtensionActions.LoadEnumIntoRdo(record.PODeliveryMode, rdoDeliveryModePO); // Load enum value into Radio Button list
            cblDeliveryModeRush.Items.FindByValue(PortalConstants.DeliveryInstructionsRush).Selected = record.Rush; // Fill value of checkbox

            AdjustDeliveryInstructions(record.DeliveryAddress);         // Turn the right panels on and off, based on other settings
                
            // Supporting Documents handled elsewhere

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
            rdoDeliveryModeReg.Enabled = false; rdoDeliveryModePO.Enabled = false; cblDeliveryModeRush.Enabled = false;
            txtDescription.Enabled = false;
            // Expense Type - already set
            // Expense State - already set
            txtEachCard.Enabled = false;
            ddlGLCode.Enabled = false;
            txtGoodsDescription.Enabled = false; txtGoodsSKU.Enabled = false; txtGoodsQuantity.Enabled = false; txtGoodsCostPerUnit.Enabled = false;
            txtInvoiceNumber.Enabled = false;
            txtNotes.Enabled = false; btnNotesClear.Visible = false;
            txtNumberOfCards.Enabled = false;
            rdoPaymentMethod.Enabled = false;
            ddlPerson.Enabled = false;
            rdoPOVendorMode.Enabled = false;
            ddlProjectClass.Enabled = false;
            txtReturnNote.Enabled = false; btnReturnNoteClear.Visible = false;
            btnSplit.Visible = false;
            //            txtURL.Enabled = false;
            ddlEntity.Enabled = false;
            txtStaffNote.Enabled = false;
            // Suporting Docs - leave Listbox filled and enabled so that double click still works
            if (pnlSupporting.Visible)
                fupUpload.Enabled = false;   // But no new items
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

            record.DeliveryMode = EnumActions.ConvertTextToDeliveryMode(rdoDeliveryModeReg.SelectedValue); // Convert selection to enum

            record.PODeliveryMode = EnumActions.ConvertTextToPODeliveryMode(rdoDeliveryModePO.SelectedValue); // Convert selection to bool

            record.Rush = cblDeliveryModeRush.Items.FindByValue(PortalConstants.DeliveryInstructionsRush).Selected; // Stash value of checkbox

            // Lots of permutations here - the DeliveryAddress may or may not be meaningful depending on DeliveryMode and PODeliveryMode. (They share txtDeliveryAddress.)
            // Rather than replicating all that logic here, just store whatever is in the text box. When we load it again, either here or in ReviewExpense,
            // we'll figure out if it should be visible.

            record.DeliveryAddress = txtDeliveryAddress.Text;

            record.Description = txtDescription.Text;

            if (pnlEntity.Visible)
            {
                int? id; bool needed;
                DdlActions.UnloadDdl(ddlEntity, out id, out needed);  // Carefully pull selected value into record
                record.EntityID = id; record.EntityNeeded = needed;
            }
            record.ExpType = EnumActions.ConvertTextToExpType(rdoExpType.SelectedValue); // Expense Type - convert from string VALUE to enumerated RqstType

            if (pnlGLCode.Visible)
                record.GLCodeID = DdlActions.UnloadDdl(ddlGLCode);    // Carefully pull selected value into record

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
                    DdlActions.UnloadDdl(ddlPerson, out id, out needed);  // Carefully pull selected value into record
                    record.PersonID = id; record.PersonNeeded = needed;
                }
                record.PersonRole = EnumActions.ConvertTextToPersonRole(litSavedPersonEnum.Text); // Convert relevant role to enum
            }

            if (pnlPOFulFillmentInstructions.Visible)
                record.POVendorMode = ExtensionActions.LoadRdoIntoPOVendorMode(rdoPOVendorMode); // Convert Yes/No to enum

            if (pnlProjectClass.Visible)                                    // If true the Expense does have a Project Class
                record.ProjectClassID = DdlActions.UnloadDdl(ddlProjectClass); // Pull the selected Project Class from the DDL

            record.ReturnNote = txtReturnNote.Text;                         // Save it in case staff changed it

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

        // Add or remove a row from the gvExpSplit gridview. That means making a row-by-row copy of the contents and rebinding the gridview

        void CopySplitGridView(int RowToAdd = -1, int RowToRemove = -1)
        {
            int totalRows = gvExpSplit.Rows.Count + ((RowToAdd == -1) ? 0 : 1) - ((RowToRemove == -1) ? 0 : 1); // Calculate final row count. Cute, eh?

            List<rowGLCodeSplit> rows = new List<rowGLCodeSplit>(); // A list of rows for the refreshed gridview control
            for (int i = 0; i < gvExpSplit.Rows.Count; i++)  // Cycle through existing rows, one-by-one
            {
                if (i != RowToRemove)                               // If != this is not the row to skip, so press on
                {
                    DropDownList ddlSplitGLCode = (DropDownList)gvExpSplit.Rows[i].FindControl("ddlSplitGLCode"); // Find drop down list
                    DropDownList ddlSplitProjectClass = (DropDownList)gvExpSplit.Rows[i].FindControl("ddlSplitProjectClass"); // And the other one
                    TextBox txtSplitAmount = (TextBox)gvExpSplit.Rows[i].FindControl("txtSplitAmount"); // Find the text box within gridview row
                    TextBox txtSplitNote = (TextBox)gvExpSplit.Rows[i].FindControl("txtSplitNote"); // Find the text box within gridview row

                    rowGLCodeSplit row = new rowGLCodeSplit()       // Create container for us to build up a row
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
                        rowGLCodeSplit addedrow = new rowGLCodeSplit();   // Create an empty row
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
            txtAmount.Enabled = false; rfvAmount.Enabled = false;   // Can't use "Total Dollar Amount" field any more; don't validate
            ddlGLCode.Enabled = false;                              // Can't use "Expense Account" drop down list any more
            ddlGLCode.ClearSelection();                             // Deselect this item in the "parent" list - it's inactive now
            rfvGLCode.Enabled = false;                              // Turn off the RequiredFieldValidation of GL code - it's not required now
            btnSplit.Text = "Cancel";                               // Reverse the meaning of the Split button
            pnlExpenseSplit.Visible = true;                         // Turn on the grid for splits
            return;
        }

        void DeEnergizeSplit()
        {
            ddlProjectClass.Enabled = true;                         // "Project Class" drop down list is back
            txtAmount.Enabled = true; txtAmount.Text = ""; rfvAmount.Enabled = true; // "Total Dollar Amount" field is back and empty
            ddlGLCode.Enabled = true;                               // "Expense Account" drop down list is back
            rfvGLCode.Enabled = true;                               // RequiredFieldValidation of GL code is back on
            btnSplit.Text = "Split";                                // Reverse the meaning of the Split button
            pnlExpenseSplit.Visible = false;                        // Turn off the grid for splits
            return;
        }

        // We have a request with no split rows and the "Split" button has been pressed.
        // Create an "empty" gridview row and prime it with existing fields from the page, if any
        // Then create a second, even emptier row, as though the user had pressed the "Add" button on the first row

        void LoadFirstSplitRows()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // Case 1) This request has never had splits before.

                List<rowGLCodeSplit> rows = new List<rowGLCodeSplit>(); // Create a list of rows for the gridview
                rowGLCodeSplit row1 = new rowGLCodeSplit()               // Prime the first row of the grid from "parent" fields of the page
                {
                    TotalRows = 2,                                      // This row plus an empty one
                    SelectedGLCodeID = ddlGLCode.SelectedValue,         // Copy the selected GL Code, if any, from the ddl
                    SelectedProjectClassID = ddlProjectClass.SelectedValue, // Copy the selected Project Code, if any, from the ddl
                };
                decimal a = ExtensionActions.LoadTxtIntoDecimal(txtAmount); // Fetch current value of Total Dollar Amount field
                row1.Amount = ExtensionActions.LoadDecimalIntoTxt(a);   // Pretty up that amount for display
                rows.Add(row1);                                         // Add the first row

                rowGLCodeSplit row2 = new rowGLCodeSplit();             // Create an empty row
                rows.Add(row2);                                         // Add it to the list

                gvExpSplit.DataSource = rows;                           // Give the rows to the gridview
                gvExpSplit.DataBind();                                  // And bind them to display the rows, firing RowDataBound for each row
                litSplitError.Visible = false;                          // Begin with the error message turned off
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

                    if ((ExtensionActions.LoadTxtIntoDecimal(txtSplitAmount) == -1) || // Carefully check for a valid decimal value in amount. If == error
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

        // Turn appropriate panels in Delivery Instructions on and off based on other settings

        void AdjustDeliveryInstructions(string deliveryAddress)
        {
            if (rdoPaymentMethod.SelectedValue == PaymentMethod.EFT.ToString()) // If == payment method is EFT
            {
                pnlDeliveryModeReg.Visible = false;
                pnlDeliveryModePO.Visible = false;
                pnlDeliveryModeRush.Visible = true;
                pnlDeliveryAddress.Visible = false;                 // No delivery mode, no delivery address
                return;                                             // No need to look any further
            }
            switch (EnumActions.ConvertTextToExpType(rdoExpType.SelectedValue)) // Break out by Expense Type
            {
                case ExpType.ContractorInvoice:
                case ExpType.Payroll:
                case ExpType.Reimbursement:
                case ExpType.VendorInvoice:
                    {
                        pnlDeliveryModeReg.Visible = true;
                        pnlDeliveryModePO.Visible = false;
                        pnlDeliveryModeRush.Visible = true;
                        break;
                    }
                case ExpType.PEXCard:
                    {
                        pnlDeliveryModeReg.Visible = false;
                        pnlDeliveryModePO.Visible = false;
                        pnlDeliveryModeRush.Visible = true;         // Only Rush is visible
                        pnlDeliveryAddress.Visible = false;
                        return;
                    }
                case ExpType.PurchaseOrder:
                    {
                        pnlDeliveryModeReg.Visible = false;
                        pnlDeliveryModePO.Visible = true;
                        pnlDeliveryModeRush.Visible = true;
                        break;
                    }
                default:
                    {
                        LogError.LogInternalError("EditExpense", $"Invalid ExpenseType value '{rdoExpType.SelectedValue}' encountered"); // Fatal error
                        break;
                    }
            }
            AdjustDeliveryModeReg(deliveryAddress);                 // DeliveryAddress on/off/filled
            return;
        }

        //  React to the settings of Delivery Mode (regular) radio buttons

        void AdjustDeliveryModeReg(string deliveryAddress)
        {
            switch (EnumActions.ConvertTextToDeliveryMode(rdoDeliveryModeReg.SelectedValue)) // Break out by button pushed
            {
                case DeliveryMode.Pickup:
                    {
                        pnlDeliveryAddress.Visible = false;             // Turn off delivery address
                        return;
                    }
                case DeliveryMode.MailPayee:
                    {
                        FillDeliveryAddress(litSavedEntityPersonFlag.Text, ddlEntity, ddlPerson, txtDeliveryAddress);
                        pnlDeliveryAddress.Visible = true;              // Turn on the panel containing delivery address
                        txtDeliveryAddress.ReadOnly = true;             // Only for viewing
                        return;
                    }
                case DeliveryMode.MailAddress:
                    {
                        txtDeliveryAddress.Text = deliveryAddress;      // Load or clear, as caller requests
                        pnlDeliveryAddress.Visible = true;              // Turn on delivery address
                        txtDeliveryAddress.ReadOnly = false;            // Here, it's editable
                        return;
                    }
                default:
                    {
                        LogError.LogInternalError("EditExpense", $"Invalid DeliveryMode value '{rdoDeliveryModeReg.SelectedValue}' encountered"); // Fatal error
                        break;
                    }
            }
        }

        // React to the setting of the Delivery Mode (PO) radio buttons

        void AdjustDeliveryModePO(string deliveryAddress)
        {
            if (rdoDeliveryModePO.SelectedValue == PODeliveryMode.DeliverAddress.ToString()) // If == Yes, need to look for an Address
            {
                txtDeliveryAddress.Text = deliveryAddress;          // Set or Clear field as caller specifies
                pnlDeliveryAddress.Visible = true;                  // Flip the delivery address on
            }
            else
            {
                pnlDeliveryAddress.Visible = false;                 // Not a "Delivery Address" option, so field go invisible
            }
            return;
        }
        void FillDeliveryAddress(string entityPersonFlag, DropDownList ddlEntity, DropDownList ddlPerson, TextBox deliveryAddress)
        {
            deliveryAddress.Text = "";                              // Clear existing text from text box
            if (entityPersonFlag == RequestType.Entity.ToString())  // If == we are dealing with an Entity
            {
                int? entityID = DdlActions.UnloadDdl(ddlEntity);    // Fetch selected value - entity ID - if any
                if (entityID != null)                               // If != a row of the DDL is selected. Process it
                {
                    using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                    {
                        Entity entity = context.Entitys.Find(entityID); // Use the Entity ID to track down the Entity
                        if (entity == null)                         // If == no Entity row found. That's a fatal error
                            LogError.LogInternalError("EditExpense", $"Unable to locate EntityID '{entityID.ToString()}' in database"); // Log fatal error
                        deliveryAddress.Text = entity.Address;      // Load the text box with the address of the Entity
                    }
                }
            }
            else if (entityPersonFlag == RequestType.Person.ToString()) // If == we are dealing with a Person
            {
                int? personID = DdlActions.UnloadDdl(ddlPerson);    // Fetch selected value - person ID - if any
                if (personID != null)                               // If != a row of the DDL is selected. Process it
                {
                    using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                    {
                        Person person = context.Persons.Find(personID); // Use the person ID to track down the person
                        if (person == null)                         // If == no person row found. That's a fatal error
                            LogError.LogInternalError("EditExpense", $"Unable to locate personID '{personID.ToString()}' in database"); // Log fatal error
                        deliveryAddress.Text = person.Address;      // Load the text box with the address of the person
                    }
                }
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
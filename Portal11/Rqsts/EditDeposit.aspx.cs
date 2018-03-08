using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Rqsts
{
    public partial class EditDeposit : System.Web.UI.Page
    {
        // Create a new Deposit Notification, Copy (and Edit) an existing Deposit Notification, or Edit an existing Deposit Notification. 
        // Communication from Project Dashboard is through Query Strings:
        //      UserID - the database ID of the Project Director making these changes (Required)
        //      ProjectID - the database ID of the Project that owns this Deposit (Required). 
        //      ProjectRole - the role of the user on this project (Optional. ProjectInfo cookie used as backup.)
        //      RequestID - the database ID of the existing Deposit, if any, to process
        //      Command - "New," "Copy," "Edit," "View," or "Revise" (Required)
        //      Return - name of the page to invoke on completion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Fetch and validate Query String parameters. Carefully convert the integer parameters to integers.

                string userID = QueryStringActions.GetUserID();         // First parameter - User ID. If absent, check UserInfoCookie
                QSValue projectID = QueryStringActions.GetProjectID();  // Second parameter - Project ID. If absent, check ProjectInfoCookie
                string projRole = QueryStringActions.GetProjectRole();  // Fetch user's Project Role from query string or cookie
                QSValue depID = QueryStringActions.GetRequestID();      // Third parameter - Deposit ID. If absent, must be a New command
                string cmd = QueryStringActions.GetCommand();           // Fourth parameter - Command. Must be present
                string ret = QueryStringActions.GetReturn();            // Fifth parameter - Return page.

                // Stash these parameters into invisible literals on the current page.

                litSavedCommand.Text = cmd;
                litSavedDepID.Text = "";                                // Assume New or Copy - do not modify an existing row, but add a new one
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
                            if (depID.Int == 0)                         // If == cannot revise a missing request
                                LogError.LogQueryStringError("EditDeposit", "Missing Query String 'RequestID'"); // Log fatal error

                            depID.Int = CopyDep(depID.Int);             // Copy the source Dep into a new destination Dep
                            depID.String = depID.Int.ToString();        // Also update the string version of the Dep ID
                            if (depID.Int == 0)                         // If == the copy process failed
                                LogError.LogInternalError("EditDeposit", "Unable to copy existing Deposit Notification"); // Log fatal error

                            goto case PortalConstants.QSCommandEdit;    // Now edit the destination Exp
                        }

                    // Fetch the row from the database. Set the radio button corresponding to our Deposit type,
                    // then disable the radio button group so the user can't change the Deposit type.
                    // Make appropriate panels visible, then fill in the panels using data rom the existing Deposit. Lotta work!

                    case PortalConstants.QSCommandEdit:                 // Process an "Edit" command. Read existing Deposit, save it in same row
                        {
                            if (depID.Int == 0)                         // If == cannot edit a missing request
                                LogError.LogQueryStringError("EditDeposit", "Missing Query String 'RequestID'"); // Log fatal error

                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                Dep dep = context.Deps.Find(depID.Int); // Fetch Dep row by its key
                                if (dep == null)
                                    LogError.LogInternalError("EditDeposit", $"Unable to locate DepositID '{depID.String}' in database"); // Log fatal error

                                CommonEditReviseView(dep);              // Do common setup steps

                                // Tidy up the Return Note based on the state of the request. The form wakes up with 
                                //      pnlReturnNote.Visible = false
                                //      txtReturnNote.Enabled = false

                                if (StateActions.StateToEditReturnNote(dep.CurrentState)) // If true, this state can be edited
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
                            litSuccessMessage.Text = "Deposit Notification is ready to edit";
                            break;
                        }

                    // We don't have to "fill" any of the controls on the page, just use them in their initial state.
                    // The radio button group for Deposit type wakes up "enabled" with nothing selected. No other controls are visible.
                    // When the user selects a Deposit type, we'll use that click to make the appropriate panels visible.

                    case PortalConstants.QSCommandNew:                  // Process a "New" command. Create new, empty Deposit, save it in new row
                        {
                            DepState newState = StateActions.FindUnsubmittedDepState(litSavedProjectRole.Text); // New state depends on User's project role
                            litSavedStateEnum.Text = newState.ToString(); // Stash actual enum value of DepState
                            txtState.Text = EnumActions.GetEnumDescription(newState); // Display "English" version of enum
                            btnShowHistory.Visible = false;             // New request has no history, so don't show meaningless button

                            Page.Title = "New Deposit Notification";         // Note that we are creating a new request, not editing
                            litSuccessMessage.Text = "New Deposit Notification is ready to edit";
                            break;
                        }

                    case PortalConstants.QSCommandRevise:               // Like an Edit at the start
                        {
                            Page.Title = "Revise Deposit Notification"; // Note that we are revising, not editing
                            goto case PortalConstants.QSCommandEdit;    // Continue in common code
                        }

                    // Fetch the row from the database. Set the radio button corresponding to our Deposit type,
                    // then disable the radio button group so the user can't change the Deposit type.
                    // Make appropriate panels visible, then fill in the panels using data from the existing Deposit. 
                    // Set everything to readonly to prevent editing. Disable Save and Submit. Lotta work!

                    case PortalConstants.QSCommandView:             
                        {
                            if (depID.Int == 0)                         // If == cannot revise a missing request
                                LogError.LogQueryStringError("EditDeposit", "Missing Query String 'RequestID'"); // Log fatal error

                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                Dep dep = context.Deps.Find(depID.Int); // Fetch Dep row by its primary key
                                if (dep == null)
                                    LogError.LogInternalError("EditDeposit", $"Unable to find Deposit ID '{depID.String}' in database"); // Fatal error

                                CommonEditReviseView(dep);              // Do common setup steps

                                if (!string.IsNullOrEmpty(txtReturnNote.Text)) // If false there is text in the Return Note. Display it
                                    pnlReturnNote.Visible = true;       // Return Note is always visible while Viewing

                                ReadOnlyControls();                     // Make all controls readonly for viewing

                                // Adjust the button settings for the View function

                                btnNewEntity.Visible = false; btnNewPerson.Visible = false; // Can't add new stuff while viewing
                                btnCancel.Text = "Done";                // Provide a friendly way out of this page
                                btnSave.Enabled = false;                // No "Save" for you!
                                btnSubmit.Enabled = false;

                                pnlAdd.Visible = false;                 // Can't figure out how to disable Add button, so make it invisible
                                btnRem.Visible = false;                 // Remove button on Supporting Docs is also invisible

                                Page.Title = "View Deposit Notification";
                                litSuccessMessage.Text = "Deposit Notification is ready to view";
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
                litDangerMessage.Text = ""; litSuccessMessage.Text = ""; litSupportingError.Visible = false; // Start with a clean slate of message displays
                if (fupUpload.HasFile)                              // If true PostBack was caused by File Upload control
                {
                    SupportingActions.UploadDoc(fupUpload, lstSupporting, litSavedUserID, litSuccessMessage, litDangerMessage); // Do heavy lifting to get file
                                                                                                                                // and record information in new SupportingDocTemp row
                    litSDError.Text = litDangerMessage.Text;            // Replicate error, if any, in more visible location                                                                                                                    
                }
            }
            return;
        }

        // Do setup steps that are common to Edit, Revise, and View functions

        void CommonEditReviseView(Dep dep)
        {
            litSavedDepID.Text = dep.DepID.ToString(); // Remember to write record back to its original spot, i.e., modify the row
            rdoDepType.SelectedValue = dep.DepType.ToString(); // Select the button corresponding to our type
            rdoDepType.Enabled = false;             // Disable the control - cannot change type of existing Deposit

            EnablePanels(dep.DepType);              // Make the relevant panels visible
            LoadPanels(dep);                        // Fill in the visible panels from the Deposit

            if (SplitActions.LoadSplitRows(RequestType.Deposit, dep.DepID, gvDepSplit)) // If true, GLCodeSplits existed and were loaded
                EnergizeSplit();                    // Adjust page to accommodate split gridview

            LoadSupportingDocs(dep);                // Fill in the Supporting Docs - it takes extra work
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
                        LogError.LogInternalError("EditDeposit", $"Invalid SourceOfFunds value '{rdoSourceOfFunds.SelectedValue}' returned by RDO"); // Fatal error
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
                btnModalYes_Click(sender, e);                       // Act as though the "Yes" button in the modal dialog box was pushed.
            else
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "divModalNew", "$('#divModalNew').modal();", true); // Open the modal dialog to confirm
            return;                                                 // Open that modal dialog box
        }

        // The "Yes" button for the modal dialog box associated with the New button on Entity and Person. Continue the magic.

        protected void btnModalYes_Click(object sender, EventArgs e)
        {
            int depID = SaveDep();                                  // Save the request, return the request ID
            if (depID == 0) return;                                 // If == hit an error. Let user retry
            CookieActions.MakeFlowControlCookie(PortalConstants.URLEditDeposit + "?"
                                            + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + depID.ToString() + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit + "&" // Start with request just saved
                                            + PortalConstants.QSReturn + "=" + litSavedReturn.Text);
            if (litSavedEntityPersonFlag.Text == RequestType.Entity.ToString()) // If == we are processing an Entity
            {
                RedirectRequest(PortalConstants.URLAssignEntitysToProject, // Do the heavy lifting to get there. Return here to continue editing.
                    "&" + PortalConstants.QSEntityRole + "=" + EntityRole.DepositEntity.ToString()); // Tell Assign who we're interested in
            }
            else if (litSavedEntityPersonFlag.Text == RequestType.Person.ToString()) // If == we are processing a Person
            {
                RedirectRequest(PortalConstants.URLAssignPersonsToProject, // Do the heavy lifting to get there. Return to continue editing.
                    "&" + PortalConstants.QSPersonRole + "=" + PersonRole.Donor.ToString()); // Tell Assign who we're interested in
            }
            LogError.LogInternalError("EditDeposit", $"Invalid SavedEntityPersonFlag value '{litSavedEntityPersonFlag.Text}' encountered"); // Fatal error
        }

        void RedirectRequest (string dest, string qs)
        {
            Response.Redirect(dest + "?" + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text 
                                    + qs
                                    + "&" + PortalConstants.QSReturn + "=" + PortalConstants.URLFlowControl); // When done, return to Flow Control to sort things out
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
                DeEnergizeSplit();                                      // Turn the gridview and its friends off
                if (!string.IsNullOrEmpty(litSavedDepID.Text))          // If false we have an DepID in the database, so we might have split rows
                    SplitActions.DeleteSplitRows(RequestType.Deposit, Convert.ToInt32(litSavedDepID.Text)); // Delete the split rows
            }
            return;
        }

        // As the Split gridview is being loaded, fill the ddls in each row and light appropriate buttons. Complicated!

        protected void gvDepSplit_RowDataBound(object sender, GridViewRowEventArgs e)
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

                if (selectedGLCodeID.Text != "")                    // If != there is a selected row. Selection is the GLCodeID of the row.
                    splitGLCode.SelectedValue = selectedGLCodeID.Text;  // Select that row
                else
                    splitGLCode.ClearSelection();                   // Back to the 0th row

                // Repeat the process with the ddlProjectClass drop down list

                splitProjectClass.Items.AddRange(ddlProjectClass.Items.Cast<ListItem>().Select(x => new ListItem(x.Text, x.Value)).ToArray());
                // Copy each ListItem from ddlProjectClass to splitProjectClass ddl

                if (selectedProjectClassID.Text != "")              // If != there is a selected row. Selection is the ProjectClassID of the row
                    splitProjectClass.SelectedValue = selectedProjectClassID.Text; // Select that row
                else
                    splitProjectClass.SelectedValue = litSavedDefaultProjectClassID.Text; // Select the "global" default row

                // If the Destination of Funds is "Restricted" the ProjectClass panel is visible. Enable our ProjectClass ddl. Otherwise, disable it.

                if (pnlProjectClass.Visible)                        // If true Project Class DDL is visible
                    splitProjectClass.Enabled = true;               // Turn on drop down list
                else
                    splitProjectClass.Enabled = false;              // Turn off drop down list

                // If this is the only row of the gridview, disable the Rem button. Can't delete the only row.

                if (Convert.ToInt32(totalRows.Text) == 1)           // If == this is the only row of the gridview
                {
                    splitRemove.Enabled = false;                    // Disable the button
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
                Page.Title = "View Deposit Notification";                // Reset the page title. Don't know why it changed, but just reset it
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

        // Simply clear the text of various notes text boxes

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
                NavigationActions.LoadAllDepHistorys(Convert.ToInt32(litSavedDepID.Text), gvEDHistory); // Fill the list from the database
                gvEDHistory.SelectedIndex = -1;                     // No row currently selected
            }
            return;
        }

        // Cancel button has been clicked. Just return to the Dashboard.

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(litSavedReturn.Text);
            return;
        }

        // Revise button clicked. This is the case where a Dep is in the "Returned" state - it was submitted, but failed during the review process.
        // To "Revise," we: 
        //  1) Get a copy of the Request
        //  2) Create a History row to audit this change.
        //  3) Change the State of the Request from "Returned" to "Under Construction," erase the ReturnNote comments and save it.
        //  4) Call this page back and invoke the "Edit" command for an existing Deposit.

        //protected void btnRevise_Click(object sender, EventArgs e)
        //{
        //    using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
        //    {
        //        try
        //        {

        //            //  1) Get a copy of the Dep

        //            int depID = Convert.ToInt32(litSavedDepID.Text); // Fetch the ID of the Dep row to be revised
        //            Dep toRevise = context.Deps.Find(depID);        // Fetch the Dep that we want to update
        //            if (toRevise == null)                           // If == the target Dep not found
        //                LogError.LogInternalError("EditDeposit", string.Format("DepositID from Query String '{0}' could not be found in database",
        //                    depID.ToString()));                     // Log fatal error

        //            //  2) Create a DepHistory row to audit this change.

        //            DepHistory hist = new DepHistory();               // Get a place to build a new History row
        //            StateActions.CopyPreviousState(toRevise, hist, "Revised"); // Create a History log row from "old" version of Request

        //            //  3) Change the State of the Rqst from "Returned" to "Unsubmitted," erase the ReturnNote comments and save it.

        //            DepState currentState = toRevise.CurrentState;  // Get a scratch copy
        //            StateActions.SetNewDepState(toRevise, StateActions.FindNextState(currentState, ReviewAction.Revise), litSavedUserID.Text, hist); // Write down our new State and authorship
        //            toRevise.ReturnNote = "";                       // Erase the note
        //            toRevise.EntityNeeded = false; toRevise.PersonNeeded = false; // Assume "Returner" did as we asked
        //            context.DepHistorys.Add(hist);                  // Save new History row
        //            context.SaveChanges();                          // Commit the Add and Modify
        //        }
        //        catch (Exception ex)
        //        {
        //            LogError.LogDatabaseError(ex, "EditDeposit", "Error processing DepHistory and Dep rows"); // Fatal error
        //        }

        //        //  4) Call this page back and invoke the "Edit" command for an existing Expense.

        //        Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text
        //                     + "&" + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text
        //                     + "&" + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text
        //                     + "&" + PortalConstants.QSRequestID + "=" + Request.QueryString[PortalConstants.QSRequestID]
        //                     + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit // Start with an existing Dep
        //                     + "&" + PortalConstants.QSReturn + "=" + litSavedReturn.Text); // Propagate return page
        //    }
        //}

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

            Response.Redirect(litSavedReturn.Text + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                                  + PortalConstants.QSStatus + "=Deposit Notification saved");
        }

        // Submit button clicked. Save what we've been working on, set the Deposit state to "Submitted," then go back to the dashboard.

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            // Before saving, make sure the Expense Split gridview contains healthy values. It's hard to validate these on-the-fly so do it here.

            if (!ValidateSplitGridView())                           // If false, there's a problem. Report the error
                return;                                             // Give user a chance to fix the problem

            // Before saving, make sure that a sufficient number of supporting documents are present. Note that a user can "Save" with
            // insufficent supporting docs, but can only "Submit" if the minimum is present.

            if (lstSupporting.Items.Count < Convert.ToInt32(litSupportingDocMin.Text)) // If < the minimum number of docs is not present
            {
                litDangerMessage.Text = "The Deposit Notification must include a minimum of " + litSupportingDocMin.Text + " Supporting Document.";
                litSuccessMessage.Text = "";                        // Just the danger message, not a stale success message
                litSupportingError.Text = litDangerMessage.Text;    // Copy the error to a second place, right at the Supporting Doc list box
                litSupportingError.Visible = true;                  // Make that error message visible
                return;                                             // Back for more punishment
            }

            int savedDepID = SaveDep();                             // Do the heavy lifting to save the current Deposit
            if (savedDepID == 0)                                    // If == SaveDep encountered an error. Go no further
                LogError.LogInternalError("EditDeposit", "Unable to save Deposit before Submitting"); // Fatal error

            // SaveDep just saved the Request, which may or may not have written a DepHistory row. But now, let's write another
            // DepHistory row to describe the Submit action.

            string emailSent = ""; bool revising = false;
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    Dep toSubmit = context.Deps.Find(savedDepID);   // Find the Dep that we want to update
                    if (toSubmit == null)                           // If == could not find the Dep
                        LogError.LogInternalError("EditDeposit", $"Unable to find saved Deposit ID '{savedDepID}' in database"); // Fatal error

                    DepHistory hist = new DepHistory();             // Get a place to build a new DepHistory row
                    StateActions.CopyPreviousState(toSubmit, hist, "Submitted"); // Fill the DepHistory row from "old" version of Deposit

                    // Deal with Return Note. This is the only place in the workflow where the Return Note gets cleared out.

                    if (!StateActions.StateToEditReturnNote(toSubmit.CurrentState)) // If false current user couldn't edit the note
                        toSubmit.ReturnNote = "";                   // So clear it out

                    revising = StateActions.RequestIsRevising(toSubmit.CurrentState); // Remember where we are in the process
                    DepState nextState = StateActions.FindNextState(toSubmit.CurrentState, ReviewAction.Submit); // Figure out who gets it next. Nuanced.
                    StateActions.SetNewState(toSubmit, nextState, litSavedUserID.Text, hist); // Move to that state
                    toSubmit.SubmitUserID = litSavedUserID.Text;    // Remember who submitted this Dep. They get notification on Return.

                    context.DepHistorys.Add(hist);                  // Save new DepHistory row
                    context.SaveChanges();                          // Update the Dep, create the DepHistory

                    if (!revising)                                  // If false request progressing normally, send email(s)
                    {
                        emailSent = EmailActions.SendEmailToReviewer(false, // Send "non-rush" email to next reviewer
                        StateActions.UserRoleToProcessRequest(nextState), // Who is in this role
                        toSubmit.ProjectID,                         // Request is associated with this project
                        toSubmit.Project.Name,                      // Project has this name (for parameter substitution)
                        EnumActions.GetEnumDescription(RequestType.Deposit), // This is a Deposit Request
                        EnumActions.GetEnumDescription(toSubmit.DepType), // Of this type
                        EnumActions.GetEnumDescription(nextState),      // Here is its next state
                        PortalConstants.CEmailDefaultDepositApprovedSubject, PortalConstants.CEmailDefaultDepositApprovedBody); // Use this subject and body, if needed
                    }
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditDeposit", "Unable to update Deposit into Submitted state"); // Fatal error
                }
            }

            // Now go back to Dashboard

            if (!revising)                                          // If false just a regular next step
            {
                Response.Redirect(litSavedReturn.Text 
                    + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess 
                    + "&" + PortalConstants.QSStatus + "=Deposit Notification submitted." + emailSent);
            }
            else                                            // Revision complete, go back for another review
            {
                Response.Redirect(PortalConstants.URLReviewDeposit
                    + "?" + PortalConstants.QSSeverity
                    + "=" + PortalConstants.QSSuccess
                    + "&" + PortalConstants.QSStatus + "=Deposit notification revised. Please continue review."
                    + "&" + PortalConstants.QSRequestID + "=" + savedDepID.ToString()  // Start with just-saved request
                    + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview // Review it
                    + "&" + PortalConstants.QSReturn + "=" + litSavedReturn.Text); // Return to our caller when done
            }
        }

            // Show History Button clicked. Open up and fill a GridView of all the DepHistory rows for this Deposit Request

            protected void btnShowHistory_Click(object sender, EventArgs e)
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
                    LogError.LogInternalError("EditDeposit", $"Deposit ID '{sourceID}' from selected GridView row not found in database"); // Fatal error
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
                        ReviseUserRole = UserRole.None,             // No revisions of a new request
                        StaffNote = "",                             // Clear out the staff note, if any
                        SourceOfFunds = src.SourceOfFunds,
                        CurrentTime = System.DateTime.Now,
                        CurrentState = StateActions.FindUnsubmittedDepState(litSavedProjectRole.Text),
                        CurrentUserID = litSavedUserID.Text         // Current user becomes creator of new Rqst
                    };

                    context.Deps.Add(dest);                         // Store the new row
                    context.SaveChanges();                          // Commit the change to product the new Rqst ID

                    //  2) Copy the split rows, if any.

                    SplitActions.CopySplitRows(RequestType.Deposit, src.DepID, dest.DepID); // Copy all the split rows, if any

                    //  3) Copy the Supporting Docs. We know that in the source Rqst, all the supporting docs are "permanent," i.e., we don't need
                    //  to worry about "temporary" docs.

                    SupportingActions.CopyDocs(context, RequestType.Deposit, src.DepID, dest.DepID); // Copy each of the Supporting Docs

                    //  4) Create a DepHistory row to describe the copy

                    DepHistory hist = new DepHistory();
                    StateActions.CopyPreviousState(src, hist, "Copied"); // Fill fields of new record
                    hist.DepID = dest.DepID; hist.NewDepState = dest.CurrentState; hist.ReturnNote = dest.ReturnNote; // Add fields from copied Dep

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
                    if (litSavedDepID.Text != "")                   // If != we have a saved ID. This request has been saved before.
                        depID = QueryStringActions.ConvertID(litSavedDepID.Text).Int; // If != saved ID is available, use it  
                    if (depID == 0)                                // If == no doubt about it, Save makes a new row
                    {

                        // To make a new Dep row:
                        //  1) Instantiate a new Dep record
                        //  2) Fill in the fixed fields, e.g., the Project ID that owns us and various items
                        //  3) unload on-screen fields to the growing record
                        //  4) Save the record to the database producing an DepID
                        //  5) Unload the splits and supporting documents

                        Dep toSave = new Dep()
                        {                                           // Get a place to hold everything
                            Inactive = false,
                            Archived = false,
                            ProjectID = QueryStringActions.ConvertID(litSavedProjectID.Text).Int, // Connect Deposit to Project
                            ReviseUserRole = UserRole.None,         // No revisions (yet) of a new request
                            CreatedTime = System.DateTime.Now       // Stamp time when Deposit was first created as "now"
                        };
                        UnloadPanels(toSave);                       // Move from Panels to record
                        StateActions.SetNewState(toSave, EnumActions.ConvertTextToDepState(litSavedStateEnum.Text), litSavedUserID.Text); // Write down our current State and authorship
                        context.Deps.Add(toSave);                   // Save new Rqst row
                        context.SaveChanges();                      // Commit the Add
                        litSavedDepID.Text = toSave.DepID.ToString(); // Show that we've saved it once

                        // Unload split rows and supporting documents from page into database

                        if (pnlDepSplit.Visible)                // If true, splits are alive and rows need to be written to database
                            SplitActions.UnloadSplitRows(RequestType.Deposit, toSave.DepID, gvDepSplit); // Save the splits from the gridview to the database
                        UnloadSupportingDocs();                     // Save all the supporting documents
                        //TODO what if there's an error here?
                        litSuccessMessage.Text = "Deposit Notification successfully saved"; // Let user know we're good
                        return toSave.DepID;                        // Return the finished Rqst
                    }
                    else                                            // Update an existing Exp row
                    {

                        // To update an existing Dep row:
                        //  1) Fetch the existing record using the DepID key
                        //  2) Unload on-screen fields overwriting the existing record
                        //  3) Update the record to the database
                        //  4) Create a new DepHistory row to preserve information about the previous Save (removed as obsolete)
                        //  5) Unload the splits and supporting documents

                        Dep toUpdate = context.Deps.Find(depID);    // Fetch the Rqst that we want to update
                        if (toUpdate == null)                       // If == did not successfully locate Dep
                            LogError.LogInternalError("EditDeposit", $"Unable to find Deposit ID '{depID}' in database"); // Fatal error
                        UnloadPanels(toUpdate);                     // Move from Panels to Rqst, modifying it in the process

                        //DepHistory hist = new DepHistory();         // Get a place to build a new DepHistory row
                        //StateActions.CopyPreviousState(toUpdate, hist, "Saved"); // Create a DepHistory log row from "old" version of Deposit
                        //StateActions.SetNewState(toUpdate, hist.PriorDepState, litSavedUserID.Text, hist); // Write down our current State (which doesn't change here) and authorship
                        //context.DepHistorys.Add(hist);              // Write new DepHistory row

                        context.SaveChanges();                      // Commit the Add or Modify

                        // Unload split rows and supporting documents from page into database

                        if (pnlDepSplit.Visible)                // If true, splits are alive and rows need to be written to database
                            SplitActions.UnloadSplitRows(RequestType.Deposit, toUpdate.DepID, gvDepSplit); // Save the splits from the gridview to the database
                        UnloadSupportingDocs();                     // Save all the new supporting documents
                        litSuccessMessage.Text = "Deposit Notification successfully updated";    // Let user know we're good
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
            litSavedEntityEnum.Text = EntityRole.DepositEntity.ToString(); // Set preferred entity role for Deposit use
            pnlSourceOfFunds.Visible = true; litSavedPersonEnum.Text = PersonRole.Donor.ToString(); // Only type of Person is Donor (so far ;-)
            btnSplit.Visible = false;
            if (RoleActions.ProjectRoleIsStaff(EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text))) // If true user is a staff member; can see staff note
                pnlStaffNote.Visible = true;                        // The staff member can see the staff note
            else
                pnlStaffNote.Visible = false;                       // But no one else can
            pnlState.Visible = true; btnSplit.Text = "Split";
            pnlSupporting.Visible = true;

            // Turn on things based on Deposit Type

            switch (type)
            {
                case DepType.Cash:
                    {
                        lblAmount.Text = "Dollar Amount";
                        lblDateOfDeposit.Text = "Date Received"; txtDateOfDeposit.ToolTip = "Please indicate the date that the cash was received.";
                        pnlOptions.Visible = true;
                        btnSplit.Visible = true;                    // Cash can be split
                        litSupportingDocMin.Text = "1";
                        break;
                    }
                case DepType.Check:
                    {
                        lblAmount.Text = "Dollar Amount";
                        lblDateOfDeposit.Text = "Date on Check"; txtDateOfDeposit.ToolTip = "Please indicate the date on the check.";
                        pnlOptions.Visible = true;
                        btnSplit.Visible = true;                    // Checks can be split
                        litSupportingDocMin.Text = "1";
                        break;
                    }
                case DepType.EFT:
                    {
                        lblAmount.Text = "Dollar Amount";
                        lblDateOfDeposit.Text = "Date of Draft"; txtDateOfDeposit.ToolTip = "Please indicate the date that the transfer was received.";
                        pnlOptions.Visible = true;
                        btnSplit.Visible = true;                    // EFTs can be split
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
                        LogError.LogInternalError("EditDeposit", $"Invalid DepType value '{type}' in database"); // Fatal error
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
            DdlActions.FillEntityDDL(ddlEntity, projID, litSavedEntityEnum.Text, entityID, needed); // Do the heavy lifting
            //EntityRole selectedRole = EnumActions.ConvertTextToEntityRole(litSavedEntityEnum.Text); // Role for this Request Type

            //using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            //{
            //    var query = from pe in context.ProjectEntitys
            //                where pe.ProjectID == projID && (pe.EntityRole == selectedRole) // This project, our Entity Role
            //                orderby pe.Entity.Name
            //                select new { EntityID = pe.EntityID, pe.Entity.Name }; // Find all Entitys that are assigned to this project

            //    DataTable rows = new DataTable();
            //    rows.Columns.Add(PortalConstants.DdlID);
            //    rows.Columns.Add(PortalConstants.DdlName);

            //    foreach (var row in query)
            //    {
            //        DataRow dr = rows.NewRow();                 // Build a row from the next query output
            //        dr[PortalConstants.DdlID] = row.EntityID;
            //        dr[PortalConstants.DdlName] = row.Name;
            //        rows.Rows.Add(dr);                          // Add the new row to the data table
            //    }

            //    // The "needed" option is no longer supported in new requests. But we may have requests in the database that still have the needed flag set.

            //    if (needed)                                     // If true old request has needed flag set
            //    {
            //        DdlActions.LoadDdl(ddlEntity, entityID, rows,
            //            "", "-- none selected --",
            //            needed, "-- Please add new " + lblEntity.Text + " --"); // Put the cherry on top
            //    }
            //    else                                            // If false, new request. "Needed" option no longer supported
            //    {
            //        DdlActions.LoadDdl(ddlEntity, entityID, rows,
            //            "", "-- none selected --");
            //    }

//            }
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

                        DdlActions.LoadDdl(ddlGLCode, glcodeID, rows, "-- Error: No GL Codes in Database --",
                            "-- none selected --", alwaysDisplayLeader: true); // Put the cherry on top

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
            DdlActions.FillPersonDDL(ddlPerson, projID, litSavedPersonEnum.Text, personID, needed); // Do the heavy lifting
            //PersonRole selectedRole = EnumActions.ConvertTextToPersonRole(litSavedPersonEnum.Text); // Role for this Request Type

            //using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            //{
            //    var query = from pe in context.ProjectPersons
            //                where pe.ProjectID == projID && pe.PersonRole == selectedRole // This project, this role
            //                orderby pe.Person.Name
            //                select new { PersonID = pe.PersonID, pe.Person.Name }; // Find all employees that are assigned to this project

            //    DataTable rows = new DataTable();
            //    rows.Columns.Add(PortalConstants.DdlID);
            //    rows.Columns.Add(PortalConstants.DdlName);

            //    foreach (var row in query)
            //    {
            //        DataRow dr = rows.NewRow();                 // Build a row from the next query output
            //        dr[PortalConstants.DdlID] = row.PersonID;
            //        dr[PortalConstants.DdlName] = row.Name;
            //        rows.Rows.Add(dr);                          // Add the new row to the data table
            //    }

            //    // The "needed" option is no longer supported in new requests. But we may have requests in the database that still have the needed flag set.

            //    if (needed)                                     // If true old request has needed flag set
            //    {
            //        DdlActions.LoadDdl(ddlPerson, personID, rows,
            //        "", "-- none selected --",
            //        needed, "-- Please add new " + lblPerson.Text + " --"); // Put the cherry on top
            //    }
            //    else
            //    {
            //        DdlActions.LoadDdl(ddlPerson, personID, rows,
            //        "", "-- none selected --");
            //    }
            //}
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
                            {
                                defaultID = row.ProjectClassID;         // Save this for later use
                                litSavedDefaultProjectClassID.Text = row.ProjectClassID.ToString(); // Save default for use in split expense rows
                            }
                        }

                        if (pcID != null)                                   // If != caller specified something
                        {
                            if (pcID != 0)                                  // If != caller specified a row to select
                                defaultID = pcID;                           // Position the DDL to that row
                        }

                        DdlActions.LoadDdl(ddlProjectClass, defaultID, rows, " -- Error: No Project Classes assigned to Project --", "-- none selected --");
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

            txtReturnNote.Text = record.ReturnNote;                     // Copy the text of the Return Note. Caller decides whether to make it visible

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
            txtNotes.Enabled = false; btnNotesClear.Visible = false;
            pnlOptions.Enabled = false;
            txtReturnNote.Enabled = false; btnReturnNoteClear.Visible = false;
            rdoSourceOfFunds.Enabled = false; ddlPerson.Enabled = false; ddlEntity.Enabled = false;
            btnSplit.Visible = false;
            pnlStaffNote.Enabled = false; btnStaffNoteClear.Visible = false;
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
                    record.ProjectClassID = DdlActions.UnloadDdl(ddlProjectClass); // Pull the selected Project Class from the DDL
                }
                else
                    record.DestOfFunds = SourceOfExpFunds.Unrestricted; // Remember this non-setting
            }
            else
                record.DestOfFunds = SourceOfExpFunds.NA;               // Not applicable to this type of Expense Request

            if (pnlGLCode.Visible)
                record.GLCodeID = DdlActions.UnloadDdl(ddlGLCode);    // Carefully pull selected value into record

            if (pnlNotes.Visible)
                record.Notes = txtNotes.Text;

            if (pnlOptions.Visible)
                record.PledgePayment = cblOptions.Items.FindByValue(Dep.OptionPledgePayment).Selected; // Stash value of checkbox

            record.ReturnNote = txtReturnNote.Text;                     // Save it in case staff changed it

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
                            if (!record.EntityIsAnonymous)                  // If false, we have an Entity to unload
                            {
                                int? id; bool needed;
                                DdlActions.UnloadDdl(ddlEntity, out id, out needed); // Carefully pull selected value into record
                                record.EntityID = id; record.EntityNeeded = needed;
                            }
                            record.EntityRole = EnumActions.ConvertTextToEntityRole(litSavedEntityEnum.Text); // Convert role to enum
                            break;
                        }
                    case PortalConstants.RDOIndividual:
                        {
                            record.SourceOfFunds = SourceOfDepFunds.Individual; // Record the selection
                            record.PersonIsAnonymous = cblPerson.Items.FindByValue(Dep.PersonAnonymous).Selected; // Stash value of checkbox
                            if (!record.PersonIsAnonymous)                  // If false, we have a Person to unload
                            {
                                int? id; bool needed;
                                DdlActions.UnloadDdl(ddlPerson, out id, out needed); // Carefully pull selected value into record
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

        // Add or remove a row from the gvDepSplit gridview. That means making a row-by-row copy of the contents and rebinding the gridview

        void CopySplitGridView(int RowToAdd = -1, int RowToRemove = -1)
        {
            int totalRows = gvDepSplit.Rows.Count + ((RowToAdd == -1) ? 0 : 1) - ((RowToRemove == -1) ? 0 : 1); // Calculate final row count. Cute, eh?

            List<rowGLCodeSplit> rows = new List<rowGLCodeSplit>(); // A list of rows for the refreshed gridview control
            for (int i = 0; i < gvDepSplit.Rows.Count; i++)  // Cycle through existing rows, one-by-one
            {
                if (i != RowToRemove)                               // If != this is not the row to skip, so press on
                {
                    DropDownList ddlSplitGLCode = (DropDownList)gvDepSplit.Rows[i].FindControl("ddlSplitGLCode"); // Find drop down list
                    DropDownList ddlSplitProjectClass = (DropDownList)gvDepSplit.Rows[i].FindControl("ddlSplitProjectClass"); // And the other one
                    TextBox txtSplitAmount = (TextBox)gvDepSplit.Rows[i].FindControl("txtSplitAmount"); // Find the text box within gridview row
                    TextBox txtSplitNote = (TextBox)gvDepSplit.Rows[i].FindControl("txtSplitNote"); // Find the text box within gridview row

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
            gvDepSplit.DataSource = rows;                         // Give the rows to the gridview
            gvDepSplit.DataBind();

            RecalculateTotalDollarAmount();                         // Update the "master" amount
            return;
        }

        // We have a full split gridview. Now adjust the operation of the page to process splits

        void EnergizeSplit()
        {
            rdoDestOfFunds.Enabled = false;                         // Can't change destination of funds radio buttons any more
            ddlProjectClass.Enabled = false;                        // Can't use "Project Class" field any more
            txtAmount.Enabled = false; rfvAmount.Enabled = false;   // Can't use "Total Dollar Amount" field any more
            ddlGLCode.Enabled = false;                              // Can't use "Expense Account" drop down list any more
            ddlGLCode.ClearSelection();                             // Deselect this item in the "parent" list - it's inactive now
            rfvGLCode.Enabled = false;                              // Turn off the RequiredFieldValidation of GL code - it's not required now
            btnSplit.Text = "Cancel";                               // Reverse the meaning of the Split button
            pnlDepSplit.Visible = true;                             // Turn on the grid for splits
            return;
        }

        void DeEnergizeSplit()
        {
            rdoDestOfFunds.Enabled = true;                          // Radio buttons are back
            ddlProjectClass.Enabled = true;                         // "Project Class" drop down list is back
            txtAmount.Enabled = true; txtAmount.Text = ""; rfvAmount.Enabled = true; // "Total Dollar Amount" field is back and empty
            ddlGLCode.Enabled = true;                               // "Expense Account" drop down list is back
            rfvGLCode.Enabled = true;                               // RequiredFieldValidation of GL code is back on
            btnSplit.Text = "Split";                                // Reverse the meaning of the Split button
            pnlDepSplit.Visible = false;                            // Turn off the grid for splits
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
                rowGLCodeSplit row1 = new rowGLCodeSplit()              // Prime the first row of the grid from "parent" fields of the page
                {
                    TotalRows = 2,                                      // This is the only row in the grid
                    SelectedGLCodeID = ddlGLCode.SelectedValue,         // Copy the selected GL Code, if any, from the ddl
                    SelectedProjectClassID = ddlProjectClass.SelectedValue, // Copy the selected Project Code, if any, from the ddl
                };
                decimal a = ExtensionActions.LoadTxtIntoDecimal(txtAmount); // Fetch current value of Total Dollar Amount field
                row1.Amount = ExtensionActions.LoadDecimalIntoTxt(a);   // Pretty up that amount for display

                rows.Add(row1);                                         // Add the first (and only) row

                rowGLCodeSplit row2 = new rowGLCodeSplit();             // Create an empty row
                rows.Add(row2);                                         // Add it to the list

                gvDepSplit.DataSource = rows;                           // Give the rows to the gridview
                gvDepSplit.DataBind();                                  // And bind them to display the rows, firing RowDataBound for each row
                litSplitError.Visible = false;                          // Begin with the error message turned off
            }
            return;
        }

        // Validate the Split gridview. Check that every row has a valid amount and selected expense account

        bool ValidateSplitGridView()
        {
            if (pnlDepSplit.Visible)                                // If true, splits are alive
            {
                foreach (GridViewRow r in gvDepSplit.Rows)          // Cycle through rows, one-by-one
                {
                    DropDownList ddlSplitGLCode = (DropDownList)r.FindControl("ddlSplitGLCode"); // Find drop down list
                    DropDownList ddlSplitProjectClass = (DropDownList)r.FindControl("ddlSplitProjectClass"); // And the other one
                    TextBox txtSplitAmount = (TextBox)r.FindControl("txtSplitAmount"); // Find the text box within gridview row
                    TextBox txtSplitNote = (TextBox)r.FindControl("txtSplitNote"); // Find the text box within gridview row

                    if ((ExtensionActions.LoadTxtIntoDecimal(txtSplitAmount) == -1) || // Carefully check for a valid decimal value in amount. If == error
                        (ddlSplitGLCode.SelectedIndex <= 0))        // If <= nothing selected, that's an error
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

        // Sum all the amount fields in the gridview. Use it to update the "master" amount field.

        void RecalculateTotalDollarAmount()
        {
            decimal totalDollarAmount = 0;                          // Accumulator for dollar amount
            foreach (GridViewRow r in gvDepSplit.Rows)              // Let's try this construction for goofs
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
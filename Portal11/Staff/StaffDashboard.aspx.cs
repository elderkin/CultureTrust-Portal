﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal11.Models;
using Portal11.Logic;
using Portal11.ErrorLog;
using System.Data;
using LinqKit;

namespace Portal11.Rqsts
{
    public partial class StaffDashboard : System.Web.UI.Page
    {

        // This page has two sets of checkboxes that filter the requests we display. We use a cookie to preserve the settings of 
        // those checkboxes as we wander off to do work. Then if the cookie is present, we use it to restore those settings. A lot of
        // work for a small feature.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                HttpCookie userInfoCookie = Request.Cookies[PortalConstants.CUserInfo]; // Find the User Info cookie
                litSavedUserRole.Text = userInfoCookie[PortalConstants.CUserRole];  // Fetch User Role from cookie and stash

                // Make adjustments for Auditor role

                if (litSavedUserRole.Text == UserRole.Auditor.ToString())   // If == this is an Auditor
                {
                    pnlNextReviewer.Visible = false;                        // Next Review is irrelevant, so hide the check boxes
                }

                RestoreCheckboxes();                                        // Read cookie, restore checkbox settings
                LoadProjectDDL();                                           // Load the list of projects
                LoadAllApps();                                              // Load the grid view of Approvals
                LoadAllDeps();                                              // Load the grid view of Deposits
                LoadAllExps();                                              // Load the grid view of Expenses
            }
            return;
        }

        // A Project has been selected from the drop down list. Refresh the lists of requests

        protected void ddlProjectName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAllApps();                                                  // Load the grid view of Approvals
            LoadAllDeps();                                                  // Load the grid view of Deposits
            LoadAllExps();                                                  // Load the grid view of Expenses
            return;
        }

        // The user clicked or unclicked one of the "Next Reviewer" check boxes for Request. Recreate the grid view with new Filter criteria.

        protected void ckRReviewer_CheckedChanged(object sender, EventArgs e)
        {
            LoadAllApps();                                                  // Recreate the grid view
            StaffAppView.SelectedIndex = -1;                                // No selected row any more
            LoadAllDeps();                                                  // Load the grid view of Deposits
            StaffDepView.SelectedIndex = -1;                                // No selected row any more
            LoadAllExps();                                                  // Load the grid view of Expenses
            StaffExpView.SelectedIndex = -1;                                // No selected row any more
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
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

        // The user has clicked on a date or range in a calendar. Let's see what's up! Suck out the selected date and hide the calendar.
        // BeginningDate and EndingDate are conjoined. Selecting a date or range in BeginningDate primes the date in EndingDate.

        protected void calBeginningDate_SelectionChanged(object sender, EventArgs e)
        {
            DateTime start = calBeginningDate.SelectedDate;
            txtBeginningDate.Text = DateActions.LoadDateIntoTxt(start);         // Convert date to text 
            DateTime last = start;
            foreach (DateTime day in calBeginningDate.SelectedDates)
            {
                last = day;                                                     // This will end up being the last date in the range
            }
            txtEndingDate.Text = DateActions.LoadDateIntoTxt(last);             // Place the last selected date into the "To" text box
            calEndingDate.SelectedDate = last;                                  // Also place the date into the "To" calendar
            calEndingDate.VisibleDate = last;                                   // And position "To" calendar to that date
            calBeginningDate.Visible = false;                                   // One click and the calendar is gone
            return;
        }

        protected void calEndingDate_SelectionChanged(object sender, EventArgs e)
        {
            calSelectionChanged(txtEndingDate, calEndingDate);                  // Do the heavy lifting
            return;
        }

        void calSelectionChanged(TextBox txt, System.Web.UI.WebControls.Calendar cal)
        {
            txt.Text = DateActions.LoadDateIntoTxt(cal.SelectedDate);           // Convert date to text
            cal.Visible = false;                                                // One click and the calendar is gone
            return;
        }

        // A From date or a To date is complete. Carefully validate the text value and report an error if there's trouble.
        // Then redo the grid using the value that was entered, even if it was blank.

        protected void txtBeginningDate_TextChanged(object sender, EventArgs e)
        {
            txtSelectionChanged(txtBeginningDate, "From");                      // Do the heavy lifting
            ckRReviewer_CheckedChanged(sender, e);                              // Refresh the grids
            return;
        }

        protected void txtEndingDate_TextChanged(object sender, EventArgs e)
        {
            txtSelectionChanged(txtEndingDate, "To");                           // Do the heavy lifting
            ckRReviewer_CheckedChanged(sender, e);                              // Refresh the grids
            return;
        }

        void txtSelectionChanged(TextBox txt, string source)
        {
            if (txt.Text != "")                                                 // If != there is a value in the text box
            {
                try
                {
                    DateTime temp;
                    temp = Convert.ToDateTime(txt.Text);                        // Attempt to convert string to date
                                                                                // If it worked, leave it right in the text box for later
                    litDangerMessage.Text = "";                                 // Erase prior error message
                }
                catch (FormatException)                                         // A date conversion error
                {
                    txt.Text = "";                                              // For now, just clear the bad date
                    litDangerMessage.Text = "Invalid '" + source + "' date value. Format is mm/dd/yyyy";
                }
            }
            return;
        }

        // Collapse/Expand the panels of A's, D's and E's

        protected void btnAppCollapse_Click(object sender, EventArgs e)
        {
            CollapseAppPanel();                                             // Do the heavy lifting
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }

        void CollapseAppPanel()
        {
            pnlApp.Visible = false;
            btnAppCollapse.Visible = false;
            btnAppExpand.Visible = true;
            return;
        }

        protected void btnAppExpand_Click(object sender, EventArgs e)
        {
            ExpandAppPanel();
            LoadAllApps();
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }

        void ExpandAppPanel()
        {
            pnlApp.Visible = true;
            btnAppCollapse.Visible = true;
            btnAppExpand.Visible = false;
            return;
        }

        protected void btnDepCollapse_Click(object sender, EventArgs e)
        {
            CollapseDepPanel();                                             // Do the heavy lifting
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }

        void CollapseDepPanel()
        {
            pnlDep.Visible = false;
            btnDepCollapse.Visible = false;
            btnDepExpand.Visible = true;
            return;
        }

        protected void btnDepExpand_Click(object sender, EventArgs e)
        {
            ExpandDepPanel();
            LoadAllDeps();
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }

        void ExpandDepPanel()
        {
            pnlDep.Visible = true;
            btnDepCollapse.Visible = true;
            btnDepExpand.Visible = false;
            return;
        }

        protected void btnExpCollapse_Click(object sender, EventArgs e)
        {
            CollapseExpPanel();
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }

        void CollapseExpPanel()
        {
            pnlExp.Visible = false;                                         // The panel body becomes invisible
            btnExpCollapse.Visible = false;                                 // The collapse button becomes invisible
            btnExpExpand.Visible = true;                                    // The expand button becomes visible
            return;
        }

        protected void btnExpExpand_Click(object sender, EventArgs e)
        {
            ExpandExpPanel();
            LoadAllExps();                                                  // Refresh the contents of the gridview
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }

        void ExpandExpPanel()
        {
            pnlExp.Visible = true;                                          // The panel body becomes visible
            btnExpCollapse.Visible = true;                                  // The collapse button becomes visible
            btnExpExpand.Visible = false;                                   // The expand button becomes invisible
            return;
        }

        // Flip a page of the grid view control

        protected void StaffAppView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                StaffAppView.PageIndex = e.NewPageIndex;                    // Propagate the desired page index
                LoadAllApps();                                              // Reload the grid view control
                StaffAppView.SelectedIndex = -1;                            // No row selected
            }
            return;
        }

        protected void StaffDepView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                StaffDepView.PageIndex = e.NewPageIndex;                    // Propagate the desired page index
                LoadAllDeps();                                              // Reload the grid view control
                StaffDepView.SelectedIndex = -1;                            // No row selected
            }
            return;
        }

        protected void StaffExpView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                StaffExpView.PageIndex = e.NewPageIndex;                    // Propagate the desired page index
                LoadAllExps();                                              // Reload the grid view control
                StaffExpView.SelectedIndex = -1;                            // No row selected
            }
            return;
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void StaffAppView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                //                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';"; // No need to highlight anything this way
                e.Row.ToolTip = "Click to select this request";             // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.StaffAppView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged

                // See whether the User is next to act on this row

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                AppState state = EnumActions.ConvertTextToAppState(label.Text); // Carefully convert back into enumeration type
                bool bingo = false;                                         // To bold or not to bold
                if (litSavedUserRole.Text == UserRole.TrustDirector.ToString()) // If == User is a Trust Director
                {
                    if (state == AppState.AwaitingTrustDirector)            // User is next approver
                        bingo = true;
                }
                else if (litSavedUserRole.Text == UserRole.FinanceDirector.ToString()) // If == User is a Finance Director
                {
                    if ((state == AppState.AwaitingFinanceDirector) || (state == AppState.Approved) ) // Ours
                        bingo = true;
                }
                else if (litSavedUserRole.Text == UserRole.TrustExecutive.ToString()) // If == User is a Finance Director
                {
                    if (state == AppState.AwaitingTrustExecutive) // Ours
                        bingo = true;
                }
                if (bingo)
                    e.Row.Cells[StaffAppViewRow.OwnerRow].Font.Bold = true; // If we get here, User can act on the row. Bold Status cell.
            }
            return;
        }

        protected void StaffDepView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                //                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';"; // No need to highlight anything this way
                e.Row.ToolTip = "Click to select this deposit";             // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.StaffDepView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged

                // See whether the User is next to act on this row

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                DepState state = EnumActions.ConvertTextToDepState(label.Text); // Carefully convert back into enumeration type
                bool bingo = false;                                         // To bold or not to bold
                if (litSavedUserRole.Text == UserRole.TrustDirector.ToString()) // If == User is a Trust Director
                {
                    if (state == DepState.AwaitingTrustDirector)            // User is next approver
                        bingo = true;
                }
                else if (litSavedUserRole.Text == UserRole.FinanceDirector.ToString()) // If == User is a Finance Director
                {
                    if ((state == DepState.AwaitingFinanceDirector) || (state == DepState.ApprovedReadyToDeposit)) // Ours
                        bingo = true;
                }
                if (bingo)
                    e.Row.Cells[StaffDepViewRow.OwnerRow].Font.Bold = true; // If we get here, User can act on the row. Bold Status cell.
            }
            return;
        }

        protected void StaffExpView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                //                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';"; // No need to highlight anything this way
                e.Row.ToolTip = "Click to select this request";             // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.StaffExpView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged

                // See whether the User is next to act on this row

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                ExpState state = EnumActions.ConvertTextToExpState(label.Text); // Carefully convert back into enumeration type
                bool bingo = false;                                         // To bold or not to bold
                if (litSavedUserRole.Text == UserRole.TrustDirector.ToString()) // If == User is a Trust Director
                {
                    if (state == ExpState.AwaitingTrustDirector)            // User is next approver
                        bingo = true;
                }
                else if (litSavedUserRole.Text == UserRole.FinanceDirector.ToString()) // If == User is a Finance Director
                {
                    if ((state == ExpState.AwaitingFinanceDirector) || (state == ExpState.Approved) || (state == ExpState.PaymentSent)) // Ours
                        bingo = true;
                }
                else if (litSavedUserRole.Text == UserRole.TrustExecutive.ToString()) // If == User is a Finance Director
                {
                    if (state == ExpState.AwaitingTrustExecutive) // Ours
                        bingo = true;
                }
                if (bingo)
                    e.Row.Cells[StaffExpViewRow.OwnerRow].Font.Bold = true; // If we get here, User can act on the row. Bold Status cell.
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected. This code assumes that
        // Cells[5] contains the enum value (not enum description) of the CurrentState

        protected void StaffAppView_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label label = (Label)StaffAppView.SelectedRow.Cells[StaffAppViewRow.CurrentStateCell].FindControl("lblCurrentState"); // Find the label control that contains Current State
            AppState state = EnumActions.ConvertTextToAppState(label.Text); // Convert back into enumeration type

            btnAppReview.Enabled = true;
            return;
        }

        protected void StaffDepView_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label label = (Label)StaffDepView.SelectedRow.Cells[StaffDepViewRow.CurrentStateCell].FindControl("lblCurrentState"); // Find the label control that contains Current State
            DepState state = EnumActions.ConvertTextToDepState(label.Text); // Convert back into enumeration type

            // Unlike Expenses, Deposits are created by Staff. But Project Director (not Staff) approves Submitted Deposits.

            if (state != DepState.AwaitingProjectDirector) // If != Request is editable by staff
            {
                btnDepReview.Enabled = true;
            }
            return;
        }

        protected void StaffExpView_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label label = (Label)StaffExpView.SelectedRow.Cells[StaffExpViewRow.CurrentStateCell].FindControl("lblCurrentState"); // Find the label control that contains Current State
            ExpState state = EnumActions.ConvertTextToExpState(label.Text); // Convert back into enumeration type

            if (!(state == ExpState.UnsubmittedByCoordinator || state == ExpState.Returned)) // If ! Request is editable by staff
            {
                btnExpReview.Enabled = true;
            }
            return;
        }

        // Review Request. From the selected row, pull the ID and head for the Review page.

        protected void btnAppReview_Click(object sender, EventArgs e)
        {
            Label label = (Label)StaffAppView.SelectedRow.Cells[StaffAppViewRow.RowIDCell].FindControl("lblRowID"); // Find the label control that contains RqstID
            string rqstID = label.Text;                                     // Extract the text of the control, which is RqstID
            if (rqstID == "")                                               // If == the RqstID is missing. That's an error.
                LogError.LogInternalError("StaffDashboard", "RqstID not found in selected GridView row"); // Log fatal error

            // Unconditionally send Rqst to ReviewApproval. It is possible that the user does not have the authority to review the rqst in
            // its current state. But we'll let Review display all the detail for the Rqst and then deny editing.

            Response.Redirect(PortalConstants.URLReviewApproval + "?" + PortalConstants.QSRequestID + "=" + rqstID + "&" // Start with an existing request
                                              + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview + "&" // Review it
                                              + PortalConstants.QSReturn + "=" + PortalConstants.URLStaffDashboard); // Return to this page when done
        }

        protected void btnDepReview_Click(object sender, EventArgs e)
        {
            Label label = (Label)StaffDepView.SelectedRow.Cells[StaffDepViewRow.RowIDCell].FindControl("lblRowID"); // Find the label control that contains RqstID
            string rqstID = label.Text;                                     // Extract the text of the control, which is RqstID
            if (rqstID == "")                                               // If == the RqstID is missing. That's an error.
                LogError.LogInternalError("StaffDashboard", "RqstID not found in selected GridView row"); // Log fatal error

            // Unconditionally send Rqst to ReviewDeposit. It is possible that the user does not have the authority to review the rqst in
            // its current state. But we'll let Review display all the detail for the Rqst and then deny editing.

            Response.Redirect(PortalConstants.URLReviewDeposit + "?" + PortalConstants.QSRequestID + "=" + rqstID + "&" // Start with an existing request
                                              + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview + "&" // Review it
                                              + PortalConstants.QSReturn + "=" + PortalConstants.URLStaffDashboard); // Return to this page when done
        }

        protected void btnExpReview_Click(object sender, EventArgs e)
        {
            Label label = (Label)StaffExpView.SelectedRow.Cells[StaffExpViewRow.RowIDCell].FindControl("lblRowID"); // Find the label control that contains RqstID
            string rqstID = label.Text;                                     // Extract the text of the control, which is RqstID
            if (rqstID == "")                                               // If == the RqstID is missing. That's an error.
                LogError.LogInternalError("StaffDashboard", "RqstID not found in selected GridView row"); // Log fatal error

            // Unconditionally send Rqst to ReviewRequest. It is possible that the user does not have the authority to review the rqst in
            // its current state. But we'll let ReviewRequest display all the detail for the Rqst and then deny editing.

            Response.Redirect(PortalConstants.URLReviewExpense + "?" + PortalConstants.QSRequestID + "=" + rqstID + "&" // Start with an existing request
                                              + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview + "&" // Review it
                                              + PortalConstants.QSReturn + "=" + PortalConstants.URLStaffDashboard); // Return to this page when done
        }

        // Fetch all of the Approvals for CS Staff, subject to further search constraints. Display in a GridView

        void LoadAllApps()
        {
            if (!pnlApp.Visible)                                            // If false the panel is not visible
                return;                                                     // Don't waste time on filling it

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // There are so many permutations of this query that we have to build it up piece by piece.

                var pred = PredicateBuilder.True<App>();                    // Initialize predicate to select from App table
                pred = pred.And(r => !r.Inactive);                          // Only active Apps

                // If a specific project is selected, only select Apps from that project

                if (ddlProjectName.SelectedIndex != 0)                      // If != Project is selected. Fetch rqsts only for that Projects
                {
                    int projectID = Convert.ToInt32(ddlProjectName.SelectedValue);  // Convert ID of selected Project
                    pred = pred.And(r => r.ProjectID == projectID);         // Only active Apps from current project
                }

                // Now look for Requests in the right state. For staff, it everything submitted for review. For auditor, it's only complete requests.

                if (litSavedUserRole.Text != UserRole.Auditor.ToString())   // If != user is not an Auditor, but is a staff member
                {
                    pred = pred.And(r => (r.CurrentState != AppState.UnsubmittedByCoordinator)); // Ignore Requests that haven't been submitted yet
                    pred = pred.And(r => (r.CurrentState != AppState.UnsubmittedByProjectDirector));
                    pred = pred.And(r => (r.CurrentState != AppState.UnsubmittedByProjectStaff));
                }
                else
                {
                    pred = pred.And(r => (r.CurrentState == AppState.Approved)); // Auditors only see Approved Requests
                }

                // Deal with date range filter. Remember that we've already validated the date strings, so we ca just use them.

                if (txtBeginningDate.Text != "")
                {
                    DateTime date = Convert.ToDateTime(txtBeginningDate.Text);  // Convert to a date value
                    pred = pred.And(r => (r.CurrentTime >= date));
                }

                if (txtEndingDate.Text != "")
                {
                    DateTime date = Convert.ToDateTime(txtEndingDate.Text);     // Convert to a date value
                    TimeSpan day = new TimeSpan(23, 59, 59);                    // Figure out the length of a day
                    DateTime endOfDay = date.Add(day);                          // Set query to END of the specified day
                    pred = pred.And(r => (r.CurrentTime <= endOfDay));          // Query to end END of the specified day
                }
                
                // The predicate is ready. Do the Query and sort most recent first. creating a list of App rows

                List<App> apps = context.Apps.AsExpandable().Where(pred).OrderByDescending(o => o.CurrentTime).ToList();

                // From this list of Rqsts, build a list of rows for the StaffAppView GridView based on the selection criteria provided by the user.

                List<StaffAppViewRow> rows = new List<StaffAppViewRow>();   // Create the empty list
                foreach (var a in apps)
                {
                    bool useRow = false;                                    // Assume we're not interested in the row
                    UserRole own = UserRole.Undefined;                      // A place to stash the Next Reviewer, also known as "Owner"

                    // Process the Approval Type checkboxes. If the "All Types" box is checked, take every row. Otherwise, only take rows
                    // whose ApprovalType matches a checked checkbox.

                    switch (a.AppType)                                  // By ApprovalType, look for relevant checkbox
                    {
                        case AppType.Contract:
                        case AppType.Grant: // TODO: Finish logic
                        case AppType.Campaign:
                        case AppType.Certificate:
                            {
                                if (ckAExpress.Checked)                 // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        case AppType.Report:
                            {
                                if (ckAFull.Checked)                    // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        default:
                            {
                                LogError.LogInternalError("StaffDashboard", $"Invalid AppType in ApprovalID '{a.AppID.ToString()}'"); // Log fatal error
                                break;
                            }
                    }
                    if (useRow)                                         // If true row passes the Request Type screen
                    {
                        if (litSavedUserRole.Text != UserRole.Auditor.ToString()) // If true User is not an Auditor. Auditors don't check Next Reviewer
                        {
                            own = StateActions.UserRoleToApproveRequest(a.CurrentState, a.AppReviewType); // Find which role will approve this Request

                            // Apply the Next Reviewer screen using the Current State of the row. Current State determines the identity of the next
                            // reviewer. It's Next Reviewer that's screen via checkboxes.

                            useRow = false;                                     // Assume this next screen will prove negative for the row

                            switch (own)                                    // By Reviewer Type, look for relevant checkbox
                            {
                                case UserRole.Coordinator:
                                    {
                                        if (ckRCoordinator.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case UserRole.FinanceDirector:
                                    {
                                        if (ckRFinanceDirector.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case UserRole.Project:  //TODO Verify this logic
                                    {
                                        if (ckRProjectMember.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case UserRole.TrustDirector:
                                    {
                                        if (ckRTrustDirector.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case UserRole.TrustExecutive:
                                    {
                                        if (ckRTrustExecutive.Checked)
                                            useRow = true;
                                        break;
                                    }
                                default:
                                    {
                                        LogError.LogInternalError("StaffDashboard", $"Invalid NextReviewer in ApprovalID '{a.AppID.ToString()}'"); // Log fatal error
                                        break;
                                    }
                            }
                        }
                        if (useRow)                                         // If true passed the second screen
                        {
                            StaffAppViewRow row = new StaffAppViewRow()
                            {                                               // Empty row all ready to fill
                                RowID = a.AppID.ToString(),                 // Convert ID from int to string for easier retrieval later
                                ProjectName = a.Project.Name,               // Fetch project name
                                CurrentTime = a.CurrentTime,                // When request was last updated
                                AppTypeDesc = EnumActions.GetEnumDescription((Enum)a.AppType), // Convert enum form to English for display
                                AppReviewType = a.AppReviewType.ToString(), // Stick with the value, it's brief and row is crowded
                                CurrentState = a.CurrentState,              // Put this in so we can get it out later to dispatch; it's not Visible
                                CurrentStateDesc = EnumActions.GetEnumDescription(a.CurrentState), // Convert enum form to English for display
                                ReturnNote = a.ReturnNote,
                                Owner = EnumActions.GetEnumDescription(own), // Fetch "English" version of owner
                                Description = a.Description
                            };

                            rows.Add(row);                                  // Add the filled-in row to the list of rows
                        }
                    }
                }

                StaffAppView.DataSource = rows;                             // Give it to the GridView control
                StaffAppView.DataBind();                                    // And get it in gear

                // Not enough to merit this                AllRqstRowCount.Text = rqsts.Count().ToString();     // Show total number of rows for amusement 

                NavigationActions.EnableGridViewNavButtons(StaffAppView);   // Enable appropriate nav buttons based on page count
            }
            return;
        }

        // Fetch all of the Deposits for CS Staff, subject to further search constraints. Display in a GridView

        void LoadAllDeps()
        {
            if (!pnlDep.Visible)                                            // If false the panel is not visible
                return;                                                     // Don't waste time on filling it

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // There are so many permutations of this query that we have to build it up piece by piece.

                var pred = PredicateBuilder.True<Dep>();                    // Initialize predicate to select from Dep table
                pred = pred.And(r => !r.Inactive);                          // Only active Requests

                // If a specific project is selected, only select Deps from that project

                if (ddlProjectName.SelectedIndex != 0)                      // If != Project is selected. Fetch rqsts only for that Projects
                {
                    int projectID = Convert.ToInt32(ddlProjectName.SelectedValue);  // Convert ID of selected Project
                    pred = pred.And(r => r.ProjectID == projectID);         // Only active Apps from current project
                }

                // Now look for Requests in the right state. For staff, it everything submitted for review. For auditor, it's only complete requests.

                if (litSavedUserRole.Text != UserRole.Auditor.ToString())   // If != user is not an Auditor, but is a staff member
                {
                    pred = pred.And(r => (r.CurrentState != DepState.UnsubmittedByCoordinator)); // Ignore Requests that haven't been submitted yet
                }
                else
                {
                    pred = pred.And(r => (r.CurrentState == DepState.DepositComplete)); // Auditors only see Complete Requests
                }

                // Deal with date range filter. Remember that we've already validated the date strings, so we ca just use them.

                if (txtBeginningDate.Text != "")
                {
                    DateTime date = Convert.ToDateTime(txtBeginningDate.Text);  // Convert to a date value
                    pred = pred.And(r => (r.CurrentTime >= date));
                }

                if (txtEndingDate.Text != "")
                {
                    DateTime date = Convert.ToDateTime(txtEndingDate.Text);     // Convert to a date value
                    TimeSpan day = new TimeSpan(23, 59, 59);                    // Figure out the length of a day
                    DateTime endOfDay = date.Add(day);                          // Set query to END of the specified day
                    pred = pred.And(r => (r.CurrentTime <= endOfDay));          // Query to end END of the specified day
                }

                // The predicate is ready. Do the Query and sort most recent first. creating a list of App rows

                List<Dep> deps = context.Deps.AsExpandable().Where(pred).OrderByDescending(o => o.CurrentTime).ToList();
                // Pull all possible Rqsts into a list - everything that's not inactive and in a relevant state.

                // From this list of Rqsts, build a list of rows for the StaffDeptView GridView based on the selection criteria provided by the user.

                List<StaffDepViewRow> rows = new List<StaffDepViewRow>();   // Create the empty list
                foreach (var d in deps)
                {
                    bool useRow = false;                                    // Assume we're not interested in the row
                    UserRole own = UserRole.Undefined;                      // A place to stash the Next Reviewer, also known as "Owner"

                    // Process the Deposit Type checkboxes. If the "All Types" box is checked, take every row. Otherwise, only take rows
                    // whose DepositType matches a checked checkbox.

                    switch (d.DepType)                              // By DepositType, look for relevant checkbox
                    {
                        case DepType.Cash:
                            {
                                if (ckDCash.Checked)                    // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        case DepType.Check:
                            {
                                if (ckDCheck.Checked)                   // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        case DepType.EFT:
                            {
                                if (ckDEFT.Checked)                     // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        case DepType.InKind:
                            {
                                if (ckDInKind.Checked)                  // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        case DepType.Pledge:
                            {
                                if (ckDPledge.Checked)                  // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        default:
                            {
                                LogError.LogInternalError("StaffDashboard", $"Invalid DepositType in DepositID '{d.DepID.ToString()}'"); // Log fatal error
                                break;
                            }
                    }
                    if (useRow)                                             // If true row passes the Request Type screen
                    {
                        if (litSavedUserRole.Text != UserRole.Auditor.ToString()) // If true User is not an Auditor. Auditors don't check Next Reviewer
                        {
                            own = StateActions.UserRoleToApproveRequest(d.CurrentState);   // Find which role will approve this Deposit

                            // Apply the Next Reviewer screen using the Current State of the row. Current State determines the identity of the next
                            // reviewer. It's Next Reviewer that's screen via checkboxes.

                            useRow = false;                                     // Assume this next screen will prove negative for the row

                            switch (own)                                        // By Reviewer Type, look for relevant checkbox
                            {
                                case UserRole.FinanceDirector:
                                    {
                                        if (ckRFinanceDirector.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case UserRole.Project:  //TODO Verify this logic
                                    {
                                        if (ckRProjectMember.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case UserRole.TrustDirector:
                                    {
                                        if (ckRTrustDirector.Checked)
                                            useRow = true;
                                        break;
                                    }
                                default:
                                    {
                                        LogError.LogInternalError("StaffDashboard", $"Invalid NextReviewer in DepositID '{d.DepID.ToString()}'"); // Log fatal error
                                        break;
                                    }
                            }
                        }
                        if (useRow)                                         // If true passed the second screen
                        {
                            StaffDepViewRow row = new StaffDepViewRow()
                            {                                               // Empty row all ready to fill
                                RowID = d.DepID.ToString(),                 // Convert ID from int to string for easier retrieval later
                                ProjectName = d.Project.Name,               // Fetch project name
                                CurrentTime = d.CurrentTime,                // When request was last updated
                                DepTypeDesc = EnumActions.GetEnumDescription((Enum)d.DepType), // Convert enum form to English for display
                                Amount = d.Amount,
                                CurrentState = d.CurrentState,              // Put this in so we can get it out later to dispatch; it's not Visible
                                CurrentStateDesc = EnumActions.GetEnumDescription(d.CurrentState), // Convert enum form to English for display
                                ReturnNote = d.ReturnNote,
                                Owner = EnumActions.GetEnumDescription(own), // Fetch "English" version of owner
                                Description = d.Description
                            };

                            rows.Add(row);                                  // Add the filled-in row to the list of rows
                        }
                    }
                }

                StaffDepView.DataSource = rows;                             // Give it to the GridView control
                StaffDepView.DataBind();                                    // And get it in gear

                // Not enough to merit this                AllRqstRowCount.Text = rqsts.Count().ToString();     // Show total number of rows for amusement 

                NavigationActions.EnableGridViewNavButtons(StaffDepView);   // Enable appropriate nav buttons based on page count
            }
            return;
        }

        // Fetch all of the Expenses for CS Staff, subject to further search constraints. Display in a GridView

        void LoadAllExps()
        {
            if (!pnlExp.Visible)                                            // If false the panel is not visible
                return;                                                     // Don't waste time on filling it

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // There are so many permutations of this query that we have to build it up piece by piece.

                var pred = PredicateBuilder.True<Exp>();                    // Initialize predicate to select from Exp table
                pred = pred.And(r => !r.Inactive);                          // Only active Requests

                // If a specific project is selected, only select Deps from that project

                if (ddlProjectName.SelectedIndex != 0)                      // If != Project is selected. Fetch rqsts only for that Projects
                {
                    int projectID = Convert.ToInt32(ddlProjectName.SelectedValue);  // Convert ID of selected Project
                    pred = pred.And(r => r.ProjectID == projectID);         // Only active Apps from current project
                }

                // Now look for Requests in the right state. For staff, it everything submitted for review. For auditor, it's only complete requests.

                if (litSavedUserRole.Text != UserRole.Auditor.ToString())   // If != user is not an Auditor, but is a staff member
                {
                    pred = pred.And(r => (r.CurrentState != ExpState.UnsubmittedByCoordinator)); // Ignore Requests that haven't been submitted yet
                    pred = pred.And(r => (r.CurrentState != ExpState.UnsubmittedByProjectDirector));
                    pred = pred.And(r => (r.CurrentState != ExpState.UnsubmittedByProjectStaff));
                }
                else
                {
                    pred = pred.And(r => (r.CurrentState == ExpState.Approved)); // Auditors only see Complete Requests
                }

                // Deal with date range filter. Remember that we've already validated the date strings, so we ca just use them.

                if (txtBeginningDate.Text != "")
                {
                    DateTime date = Convert.ToDateTime(txtBeginningDate.Text);  // Convert to a date value
                    pred = pred.And(r => (r.CurrentTime >= date));
                }

                if (txtEndingDate.Text != "")
                {
                    DateTime date = Convert.ToDateTime(txtEndingDate.Text);     // Convert to a date value
                    TimeSpan day = new TimeSpan(23, 59, 59);                    // Figure out the length of a day
                    DateTime endOfDay = date.Add(day);                          // Set query to END of the specified day
                    pred = pred.And(r => (r.CurrentTime <= endOfDay));          // Query to end END of the specified day
                }

                // The predicate is ready. Do the Query and sort most recent first. creating a list of App rows

                List<Exp> exps = context.Exps.AsExpandable().Where(pred).OrderByDescending(o => o.CurrentTime).ToList();
                // Pull all possible Rqsts into a list - everything that's not inactive and in a relevant state.


                // From this list of Rqsts, build a list of rows for the AllRqstView GridView based on the selection criteria provided by the user.

                List<StaffExpViewRow> rows = new List<StaffExpViewRow>();   // Create the empty list
                foreach (var r in exps)
                {
                    bool useRow = false;                                    // Assume we're not interested in the row
                    UserRole own = UserRole.Undefined;                      // A place to stash the Next Reviewer, also known as "Owner"

                    // Process the Request Type checkboxes. If the "All Types" box is checked, take every row. Otherwise, only take rows
                    // whose RqstType matches a checked checkbox.

                    switch (r.ExpType)                                  // By RqstType, look for relevant checkbox
                    {
                        case ExpType.ContractorInvoice:
                            {
                                if (ckEContractorInvoice.Checked)       // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        case ExpType.GiftCard:
                            {
                                if (ckEGiftCard.Checked)                // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        case ExpType.Paycheck:
                            {
                                if (ckEPaycheck.Checked)                // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        case ExpType.PurchaseOrder:
                            {
                                if (ckEPurchaseOrder.Checked)           // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        case ExpType.Reimbursement:
                            {
                                if (ckEReimbursement.Checked)           // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        case ExpType.VendorInvoice:
                            {
                                if (ckEVendorInvoice.Checked)           // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        default:
                            {
                                LogError.LogInternalError("StaffDashboard", $"Invalid RqstType in RqstID '{r.ExpID.ToString()}'"); // Log fatal error
                                break;
                            }
                    }
                    if (useRow)                                             // If true row passes the Expense Type screen
                    {
                        if (litSavedUserRole.Text != UserRole.Auditor.ToString()) // If true User is not an Auditor. Auditors don't check Next Reviewer
                        {
                            own = StateActions.UserRoleToApproveRequest(r.CurrentState);   // Find which role will approve this Expense

                            // Apply the Next Reviewer screen using the Current State of the row. Current State determines the identity of the next
                            // reviewer. It's Next Reviewer that's screen via checkboxes.

                            useRow = false;                                     // Assume this next screen will prove negative for the row

                            switch (own)                                        // By Reviewer Type, look for relevant checkbox
                            {
                                case UserRole.FinanceDirector:
                                    {
                                        if (ckRFinanceDirector.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case UserRole.Project:
                                    {
                                        if (ckRProjectMember.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case UserRole.TrustDirector:
                                    {
                                        if (ckRTrustDirector.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case UserRole.TrustExecutive:
                                    {
                                        if (ckRTrustExecutive.Checked)
                                            useRow = true;
                                        break;
                                    }
                                default:
                                    {
                                        LogError.LogInternalError("StaffDashboard", $"Invalid NextReviewer in RqstID '{r.ExpID.ToString()}'"); // Log fatal error
                                        break;
                                    }
                            }
                        }
                        if (useRow)                                         // If true passed the second screen
                        {
                            StaffExpViewRow row = new StaffExpViewRow()
                            {          // Empty row all ready to fill
                                RowID = r.ExpID.ToString(),                 // Convert ID from int to string for easier retrieval later
                                ProjectName = r.Project.Name,               // Fetch project name
                                CurrentTime = r.CurrentTime,                // When request was last updated
                                ExpTypeDesc = EnumActions.GetEnumDescription(r.ExpType), // Convert enum form to English for display
                                Amount = r.Amount,
                                CurrentState = r.CurrentState,              // Put this in so we can get it out later to dispatch; it's not Visible
                                StateDesc = EnumActions.GetEnumDescription(r.CurrentState), // Convert enum form to English for display
                                ReturnNote = r.ReturnNote,
                                Owner = EnumActions.GetEnumDescription(own) // Fetch "English" version of owner
                            };

                            // Fill "Target" with Vendor Name or Employee Name or Recipient. Only one will be present, depending on RqstType.

                            if (r.Entity != null)                           // If != a Vendor is present in the Request
                            {
                                if (r.Entity.Name != null)
                                    row.Target = r.Entity.Name;
                            }
                            else if (r.Person != null)                    // If != an Employee is present in the Request
                            {
                                if (r.Person.Name != null)
                                    row.Target = r.Person.Name;
                            }
                            row.Summary = r.Summary;
                            rows.Add(row);                                  // Add the filled-in row to the list of rows
                        }
                    }
                }
                StaffExpView.DataSource = rows;                             // Give it to the GridView control
                StaffExpView.DataBind();                                    // And get it in gear

                // Not enough to merit this                AllRqstRowCount.Text = rqsts.Count().ToString();     // Show total number of rows for amusement 

                NavigationActions.EnableGridViewNavButtons(StaffExpView);   // Enable appropriate nav buttons based on page count
            }
            return;
        }

        // Fill a Drop Down List with available Project Names

        void LoadProjectDDL()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                string fran = SupportingActions.GetFranchiseKey();  // Fetch current franchise key
                var query = from proj in context.Projects
                            where !proj.Inactive                    // All active projects
                                && (proj.FranchiseKey == fran)      // For this franchise
                            orderby proj.Name
                            select new { proj.ProjectID, proj.Name };

                DataTable rows = new DataTable();
                rows.Columns.Add(PortalConstants.DdlID);
                rows.Columns.Add(PortalConstants.DdlName);

                foreach (var row in query)
                {
                    DataRow dr = rows.NewRow();                      // Build a row from the next query output
                    dr[PortalConstants.DdlID] = row.ProjectID;
                    dr[PortalConstants.DdlName] = row.Name;
                    rows.Rows.Add(dr);                               // Add the new row to the data table
                }

                StateActions.LoadDdl(ddlProjectName, null, rows,
                    " -- Error: No Projects defined in Portal --", "-- All Projects --"); // Put the cherry on top
            }
            return;
        }

        // Save the current settings of the checkboxes in a (big) cookie

        void SaveCheckboxes()
        {
            HttpCookie staffCheckboxesCookie = new HttpCookie(PortalConstants.CStaffCheckboxes);

            staffCheckboxesCookie[PortalConstants.CStaffCkRCoordinator] = ckRCoordinator.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkRFinanceDirector] = ckRFinanceDirector.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkRProjectMember] = ckRProjectMember.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkRTrustDirector] = ckRTrustDirector.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkRTrustExecutive] = ckRTrustExecutive.Checked.ToString();

            staffCheckboxesCookie[PortalConstants.CStaffFromDate] = txtBeginningDate.Text;
            staffCheckboxesCookie[PortalConstants.CStaffToDate] = txtEndingDate.Text;

            staffCheckboxesCookie[PortalConstants.CStaffApprovalsVisible] = pnlApp.Visible.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkAExpress] = ckAExpress.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkAFull] = ckAFull.Checked.ToString();

            staffCheckboxesCookie[PortalConstants.CStaffDepositsVisible] = pnlDep.Visible.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkDCheck] = ckDCheck.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkDEFT] = ckDEFT.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkDCash] = ckDCash.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkDInKind] = ckDInKind.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkDPledge] = ckDPledge.Checked.ToString();

            staffCheckboxesCookie[PortalConstants.CStaffExpVisible] = pnlExp.Visible.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkEContractorInvoice] = ckEContractorInvoice.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkEGiftCard] = ckEGiftCard.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkEPaycheck] = ckEPaycheck.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkEPurchaseOrder] = ckEPurchaseOrder.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkEReimbursement] = ckEReimbursement.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkEVendorInvoice] = ckEVendorInvoice.Checked.ToString();

            Response.Cookies.Add(staffCheckboxesCookie);                    // Store a new cookie
            return;
        }

        // Read that cookie, if present, and restore the current settings of the checkboxes

        void RestoreCheckboxes()
        {
            HttpCookie staffCheckboxesCookie = Request.Cookies[PortalConstants.CStaffCheckboxes]; // Ask for the Staff Checkbox cookie
            if (staffCheckboxesCookie != null)                              // If != the cookie is present
            {

                // Check boxes at top of page

                if (staffCheckboxesCookie[PortalConstants.CStaffCkRCoordinator] == "True") ckRCoordinator.Checked = true;
                else ckRCoordinator.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkRFinanceDirector] == "True") ckRFinanceDirector.Checked = true;
                else ckRFinanceDirector.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkRProjectMember] == "True") ckRProjectMember.Checked = true;
                else ckRProjectMember.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkRTrustDirector] == "True") ckRTrustDirector.Checked = true;
                else ckRTrustDirector.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkRTrustExecutive] == "True") ckRTrustExecutive.Checked = true;
                else ckRTrustExecutive.Checked = false;

                // Date Range

                txtBeginningDate.Text = staffCheckboxesCookie[PortalConstants.CStaffFromDate];
                txtEndingDate.Text = staffCheckboxesCookie[PortalConstants.CStaffToDate];

                // Approval Requests panel

                if (staffCheckboxesCookie[PortalConstants.CStaffApprovalsVisible] == "True")
                    ExpandAppPanel();                                       // Expand, but don't fill Approvals panel
                else
                    CollapseAppPanel();                                     // Start with Approvals panel collapsed

                if (staffCheckboxesCookie[PortalConstants.CStaffCkAExpress] == "True") ckAExpress.Checked = true;
                else ckAExpress.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkAFull] == "True") ckAFull.Checked = true;
                else ckAFull.Checked = false;

                // Deposit Requests panel

                if (staffCheckboxesCookie[PortalConstants.CStaffDepositsVisible] == "True")
                    ExpandDepPanel();                                       // Expand, but don't fill Deposits panel
                else
                    CollapseDepPanel();                                     // Start with Deposits panel collapsed

                if (staffCheckboxesCookie[PortalConstants.CStaffCkDCheck] == "True") ckDCheck.Checked = true;
                else ckDCheck.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkDEFT] == "True") ckDEFT.Checked = true;
                else ckDEFT.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkDCash] == "True") ckDCash.Checked = true;
                else ckDCash.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkDInKind] == "True") ckDInKind.Checked = true;
                else ckDInKind.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkDPledge] == "True") ckDPledge.Checked = true;
                else ckDPledge.Checked = false;

                // Expense Requests panel

                if (staffCheckboxesCookie[PortalConstants.CStaffExpVisible] == "True")
                    ExpandExpPanel();                                       // Expand, but don't fill Requests panel
                else
                    CollapseExpPanel();                                     // Start with Requests panel collapsed

                if (staffCheckboxesCookie[PortalConstants.CStaffCkEContractorInvoice] == "True") ckEContractorInvoice.Checked = true;
                else ckEContractorInvoice.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkEGiftCard] == "True") ckEGiftCard.Checked = true;
                else ckEGiftCard.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkEPaycheck] == "True") ckEPaycheck.Checked = true;
                else ckEPaycheck.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkEPurchaseOrder] == "True") ckEPurchaseOrder.Checked = true;
                else ckEPurchaseOrder.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkEReimbursement] == "True") ckEReimbursement.Checked = true;
                else ckEReimbursement.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkEVendorInvoice] == "True") ckEVendorInvoice.Checked = true;
                else ckEVendorInvoice.Checked = false;
            }
            return;
        }
    }
}
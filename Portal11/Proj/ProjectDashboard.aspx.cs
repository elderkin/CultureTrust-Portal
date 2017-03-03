using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal11.Models;
using Portal11.Logic;
using System.Data;
using Portal11.ErrorLog;
using System.Drawing;

namespace Portal11.Proj
{
    public partial class ProjectDashboard : System.Web.UI.Page
    {

        // We've got two separate pages here, one for Deposits and one for Expenses. The result is a lot of code in this module.
        // Page_Load goes first. Then all the routines for Deposits, in top-to-bottom order on the page. Then the same for Expenses.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Display grids of Expenses and Deposits for a project. Communication is through Query Strings:
                //      UserID - the database ID of the Project Director (if missing, use cookie)
                //      ProjectID - the database ID of the Project whose Requests are to be displayed (if missing, use cookie)
                //  also
                //      Severity and Status - to allow callers to ask us to display exiting success or danger messages

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                // Pick out UserID and ProjectID and Role from query string or cookie

                string userID = Request.QueryString[PortalConstants.QSUserID];  // First check the Query String and find the User
                if (string.IsNullOrEmpty(userID))                       // If true UserID not specified. Check cookie.
                {
                    HttpCookie userInfoCookie = Request.Cookies[PortalConstants.CUserInfo]; // Find the User Info cookie
                    if (userInfoCookie == null)                         // If == the cookie is missing
                        LogError.LogInternalError("ProjectDashboard", "Missing UserInfoCookie"); // Fatal error

                    userID = userInfoCookie[PortalConstants.CUserID];   // Fetch UserID from cookie
                    if (userID == "")                                   // If == that's blank, too. Now we have an error
                        LogError.LogQueryStringError("ProjectDashboard", "Unable to find UserID in Query String or UserID Cookie. User is not logged in"); // Fatal error
                }
                litSavedUserID.Text = userID;                           // Save in a "faster" place for later reference

                string projID = Request.QueryString[PortalConstants.QSProjectID]; // Look on the query string for Project ID
                HttpCookie projectInfoCookie = Request.Cookies[PortalConstants.CProjectInfo]; // Find the Project Info cookie
                if (projectInfoCookie == null)                          // If == ProjectInfoCookie is null
                    LogError.LogInternalError("ProjectDashboard", "Missing ProjectInfoCookie"); // Fatal error

                if (string.IsNullOrEmpty(projID))                       // If true Project ID not specified as Query String. Check cookie
                {
                    projID = projectInfoCookie[PortalConstants.CProjectID]; // Fetch ProjectID from cookie
                    if (projID == "")                                   // If == that's blank, too. Now we have an error
                        LogError.LogQueryStringError("ProjectDashboard", "Unable to find ProjectID in Query String or Project Info Cookie. User does not have a project"); // Fatal error
                }
                litSavedProjectID.Text = projID;                        // Save in a faster spot for later
                int projIDint = Convert.ToInt32(projID);                // Produce int version of this key

                string projRole = projectInfoCookie[PortalConstants.CProjectRole]; // Fetch user's Project Role from cookie
                if (projRole == "")                                     // If == that's blank. We have an error
                    LogError.LogQueryStringError("ProjectDashboard", "Unable to find Project Role in Project Info Cookie. User does not have a project"); // Fatal error
                litSavedProjectRole.Text = projRole;                    // Save in a faster spot for later

                // Fill project-specific date and amount fields at the top of the page.

                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    Project proj = context.Projects.Find(projIDint);    // Fetch target project by ID
                    if (proj == null)                                   // If == couldn't find the project
                        LogError.LogInternalError("ProjectDashboard", string.Format("Unable to find ProjectID '{0}' in database", 
                            projIDint)); // Fatal error

                    litBalance.Text = proj.BalanceDate.ToString("MM/dd/yyyy");
                    litCurrentFunds.Text = proj.CurrentFunds.ToString("C");
                }

                // If the User is a Coordinator, they can create a new Deposit.

                if (litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString()) // If == User is a InternalCoordinator. Powerful
                    btnDepNew.Enabled = true;                           // Allow them to create a new Deposit

                RestoreCheckboxes();                                    // Restore more recent checkbox settings from a cookie

                int rows = CookieActions.FindGridViewRows();            // Find number of rows per page from cookie
                gvAllApp.PageSize = rows;                             // Adjust grids accordingly
                gvAllDep.PageSize = rows;
                gvAllExp.PageSize = rows;

                LoadAllApps();
                LoadAllDeps();
                LoadAllExps();                                          // Load the grid view for display
            }
            return;
        }

        // Expand and Collapse the panel of search filters

        protected void btnSearchCollapse_Click(object sender, EventArgs e)
        {
            CollapseSearchPanel();                                          // Do the heavy lifting
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }

        void CollapseSearchPanel()
        {
            pnlSearch.Visible = false;
            btnSearchCollapse.Visible = false;
            btnSearchExpand.Visible = true;
            return;
        }

        protected void btnSearchExpand_Click(object sender, EventArgs e)
        {
            ExpandSearchPanel();
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }

        void ExpandSearchPanel()
        {
            pnlSearch.Visible = true;
            btnSearchCollapse.Visible = true;
            btnSearchExpand.Visible = false;
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
            SearchCriteriaChanged(sender, e);                                   // Refresh the grids
            return;
        }

        protected void calEndingDate_SelectionChanged(object sender, EventArgs e)
        {
            calSelectionChanged(txtEndingDate, calEndingDate);                  // Do the heavy lifting
            SearchCriteriaChanged(sender, e);                                   // Refresh the grids
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
            DateActions.ValidateDateInput(txtBeginningDate, "From Date", litDangerMessage); // Do the heavy lifting
            SearchCriteriaChanged(sender, e);                                   // Refresh the grids
            return;
        }

        protected void txtEndingDate_TextChanged(object sender, EventArgs e)
        {
            DateActions.ValidateDateInput(txtEndingDate, "To Date", litDangerMessage); // Do the heavy lifting
            SearchCriteriaChanged(sender, e);                                   // Refresh the grids
            return;
        }

        // The user changed one of the Filter selections check boxes. Recreate the grid views with new Filter criteria.

        protected void SearchCriteriaChanged(object sender, EventArgs e)
        {
            LoadAllApps();                                              // Reload the GridView using new criteria
            ResetAppContext();                                          // No selected row, no live buttons

            LoadAllDeps();                                              // Reload the GridView using new criteria
            ResetDepContext();                                          // No selected row, no live buttons

            LoadAllExps();                                              // Reload the GridView using new criteria
            ResetExpContext();                                          // No selected row, no live buttons

            SaveCheckboxes();                                           // Save current checkbox settings in a cookie
            return;
        }

        // Collapse/Expand the panels of Approvals

        protected void btnAppCollapse_Click(object sender, EventArgs e)
        {
            CollapseAppPanel();
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
            ResetAppContext();                                          // No selected row, no live buttons
            SaveCheckboxes();                                           // Save current checkbox settings in a cookie
            return;
        }

        void ExpandAppPanel()
        {
            pnlApp.Visible = true;
            btnAppCollapse.Visible = true;
            btnAppExpand.Visible = false;
            return;
        }

        // Expand/Collapse panel of Deposits

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
            ResetDepContext();                                          // No selected row, no live buttons
            SaveCheckboxes();                                           // Save current checkbox settings in a cookie
            return;
        }

        void ExpandDepPanel()
        {
            pnlDep.Visible = true;
            btnDepCollapse.Visible = true;
            btnDepExpand.Visible = false;
            return;
        }

        // Collapse/Expand the panels of Expenses

        protected void btnExpCollapse_Click(object sender, EventArgs e)
        {
            CollapseExpPanel();
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }

        void CollapseExpPanel()
        {
            pnlExp.Visible = false;                                     // The panel body becomes invisible
            btnExpCollapse.Visible = false;                             // The collapse button becomes invisible
            btnExpExpand.Visible = true;                                // The expand button becomes visible
            return;
        }

        protected void btnExpExpand_Click(object sender, EventArgs e)
        {
            ExpandExpPanel();
            LoadAllExps();                                              // Refresh the contents of the gridview
            ResetExpContext();                                          // No selected row, no live buttons
            SaveCheckboxes();                                           // Save current checkbox settings in a cookie
            return;
        }

        void ExpandExpPanel()
        {
            pnlExp.Visible = true;                                      // The panel body becomes visible
            btnExpCollapse.Visible = true;                              // The collapse button becomes visible
            btnExpExpand.Visible = false;                               // The expand button becomes invisible
            return;
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button. Also, bold the Status cell if this user can operate on the row.

        protected void gvAllApp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)            // If == this is indeed a row of our GridView control
            {
                Common_RowDataBound(sender, e);

                // See if the Request awaits action by the User. Find the right cell by its ID. The cell contains the text of the AppState enum value. 

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                AppState state = EnumActions.ConvertTextToAppState(label.Text); // Carefully convert back into enumeration type

                if (StateActions.ProjectRoleToProcessRequest(state) == EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text)) // If == user can operate on Request
                    e.Row.Cells[ProjectAppViewRow.CurrentStateDescRow].Font.Bold = true; // Bold the Status cell of this row
            }
            return;
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button. Also, bold the Status cell if this user can operate on the row.

        protected void gvAllDep_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)            // If == this is indeed a row of our GridView control
            {
                Common_RowDataBound(sender, e);

                // See if the Request awaits action by the User. Find the right cell by its ID. The cell contains the text of the DepState enum value.
                // For Deposits, the only such situation is for a Project Director and a Deposit Request in "Awaiting Project Director" or "Returned" state.

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                DepState state = EnumActions.ConvertTextToDepState(label.Text); // Carefully convert back into enumeration type

                if (StateActions.ProjectRoleToProcessRequest(state) == EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text)) // If == user can operate on Request
                    e.Row.Cells[ProjectDepViewRow.CurrentStateDescRow].Font.Bold = true; // Bold Status cell.
            }
            return;
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void gvAllExp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)            // If == this is indeed a row of our GridView control
            {
                Common_RowDataBound(sender, e);

                // See if the Request awaits action by the User. Find the right cell by its ID. The cell contains the text of the DepState enum value.
                // For Deposits, the only such situation is for a Project Director and a Deposit Request in "Awaiting Project Director" or "Returned" state.

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                ExpState state = EnumActions.ConvertTextToExpState(label.Text); // Carefully convert back into enumeration type

                if (StateActions.ProjectRoleToProcessRequest(state) == EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text)) // If == user can operate on Request
                    e.Row.Cells[ProjectExpViewRow.CurrentStateDescRow].Font.Bold = true; // Bold Status cell.

                // See if the row is Rush

                label = (Label)e.Row.FindControl("lblRush");                // Find the label control that contains Rush in this row
                if (label.Text == "True")                                   // If == this record is Rush
                    e.Row.ForeColor = Color.Red;                            // Use color to indicate Rush status
            }
            return;
        }

        // Perform the operations that all three gridviews share

        void Common_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
            e.Row.ToolTip = "Click to select this Request";                 // Establish tool tip during flyover
            e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink((GridView)sender, "Select$" + e.Row.RowIndex);
            // Mark the row "Selected" on a click. That will fire SelectedIndexChanged

            // See if the row is Archived

            Label label = (Label)e.Row.FindControl("lblArchived");          // Find the label control that contains Archived in this row
            if (label.Text == "True")                                       // If == this record is Archived
                e.Row.Font.Italic = true;                                   // Use italics to indicate Archived status
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected. This code assumes that
        // the row contains the enum value (not enum desription) of CurrentState. It also assumes that only the New and View buttons are
        // enabled all the time.

        protected void gvAllApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label label = (Label)gvAllApp.SelectedRow.FindControl("lblCurrentState"); // Find the label control that contains Current State
            AppState state = EnumActions.ConvertTextToAppState(label.Text); // Convert back into enumeration type
            btnAppArchive.Enabled = false;                              // Assume User cannot Archive Request
            btnAppCopy.Enabled = true;                                  // If any Request is selected, the user can always copy it
            btnAppDelete.Enabled = false;                               // Assume User cannot Delete Request
            btnAppEdit.Enabled = false;                                 // Assume User cannot Edit Request
            btnAppReview.Enabled = false;                               // Assume User cannot Review Request
            btnAppView.Enabled = true;                                  // If any Request is selected, the user can always view it

            switch (state)
            {
                case AppState.UnsubmittedByInternalCoordinator:
                    {
                        if (litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString()) // If == the user is a coordinator
                        {
                            btnAppDelete.Enabled = true;
                            btnAppEdit.Enabled = true;
                        }
                        break;
                    }
                case AppState.UnsubmittedByProjectDirector:
                    {
                        if (litSavedProjectRole.Text == ProjectRole.ProjectDirector.ToString()) // If == the user is a PD
                        {
                            btnAppDelete.Enabled = true;
                            btnAppEdit.Enabled = true;
                        }
                        break;
                    }
                case AppState.UnsubmittedByProjectStaff:
                    {
                        if (litSavedProjectRole.Text == ProjectRole.ProjectStaff.ToString()) // If == the user is a PS
                        {
                            btnAppDelete.Enabled = true;
                            btnAppEdit.Enabled = true;
                        }
                        break;
                    }
                case AppState.AwaitingProjectDirector:
                    {
                        if (litSavedProjectRole.Text == ProjectRole.ProjectDirector.ToString()) // If == user is a Project Director, can Review
                            btnAppReview.Enabled = true;
                        break;
                    }
                case AppState.AwaitingInternalCoordinator:
                    {
                        if (litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString()) // If == the user is a coordinator
                        {
                            btnAppReview.Enabled = true;
                        }
                        break;
                    }
                case AppState.Approved:                             // Maybe Returned too?
                    {
                        if ((litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString())
                            || litSavedProjectRole.Text == ProjectRole.ProjectDirector.ToString()) // If == the user can Archive
                        {
                            Label archivedLabel = (Label)gvAllApp.SelectedRow.FindControl("lblArchived"); // Find the label control that contains Archived
                            if (archivedLabel.Text == "False")      // If == not currently archived.
                                btnAppArchive.Enabled = true;       // Light Archive button. Can't archive if already archived
                        }
                        break;
                    }
                case AppState.AwaitingTrustDirector:
                case AppState.AwaitingFinanceDirector:
                case AppState.AwaitingTrustExecutive:
                case AppState.Returned:
                    {
                        break;                                      // The button setup is just fine. No editing or deleting here
                    }
                default:
                    LogError.LogInternalError("ProjectDashboard", string.Format("Invalid AppState '{0}' from database",
                        state)); // Fatal error
                    break;
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected. This code assumes that only the View buttons are
        // enabled all the time. Every other button is nuanced based on role. We don't mess with the New button. It works independently of
        // which Request is selected.

        protected void gvAllDep_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label label = (Label)gvAllDep.SelectedRow.FindControl("lblCurrentState"); // Find the label control that contains Current State
            DepState state = EnumActions.ConvertTextToDepState(label.Text); // Carefully convert back into enumeration type
            btnDepArchive.Enabled = false;
            btnDepCopy.Enabled = false;                                 // Assume a powerless user
            btnDepDelete.Enabled = false;                               // and turn off all buttons
            btnDepEdit.Enabled = false;
            // Leave the New button in its former state
            btnDepReview.Enabled = false;
            btnDepView.Enabled = true;                                  // Everybody can View any Request

            if (litSavedProjectRole.Text == ProjectRole.ProjectStaff.ToString()) // If == User is a Project Staff. Powerless.
                return;

            if (litSavedProjectRole.Text == ProjectRole.ProjectDirector.ToString()) // If == User is a Project Director
            {
                // Note: Project Director cannot copy a Deposit Request - only IC can do that
                if (state == DepState.AwaitingProjectDirector)          // If == the Request is waiting our action
                {
                    btnDepReview.Enabled = true;                        // A Project Director can review this Request
                }
                else if (state == DepState.DepositComplete)             // If == the Request is complete; could be archived
                {
                    Label archivedLabel = (Label)gvAllDep.SelectedRow.FindControl("lblArchived"); // Find the label control that contains Archived
                    if (archivedLabel.Text == "False")                  // If == not currently archived.
                        btnDepArchive.Enabled = true;                   // Light Archive button. Can't archive if already archived
                }
                return;                                                 // Otherwise, Project Director is powerless
            }

            if (litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString()) // If == User is a Project InternalCoordinator. Powerful
            {
                btnDepCopy.Enabled = true;                              // No match what state, we can copy the request
                if (state == DepState.UnsubmittedByInternalCoordinator) // If == the Request is under construction by us
                {
                    btnDepDelete.Enabled = true;                        // Turn on other buttons
                    btnDepEdit.Enabled = true;                          // that make sense in this state

                }
                else if (state == DepState.DepositComplete)             // If == the Request is complete; could be archived
                {
                    Label archivedLabel = (Label)gvAllDep.SelectedRow.FindControl("lblArchived"); // Find the label control that contains Archived
                    if (archivedLabel.Text == "False")                  // If == not currently archived.
                        btnDepArchive.Enabled = true;                   // Light Archive button. Can't archive if already archived
                }
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected. This code assumes that
        // the Row contains the enum value (not enum desription) of CurrentState. It also assumes that only the New and View buttons are
        // enabled all the time.

        protected void gvAllExp_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label label = (Label)gvAllExp.SelectedRow.FindControl("lblCurrentState"); // Find the label control that contains Current State
            ExpState state = EnumActions.ConvertTextToExpState(label.Text); // Convert back into enumeration type
            btnExpArchive.Enabled = false;
            btnExpCopy.Enabled = true;                                  // If any Request is selected, the user can always copy it
            btnExpDelete.Enabled = false;                               // Assume User cannot Delete Request
            btnExpEdit.Enabled = false;                                 // Assume User cannot Edit Request
            btnExpReview.Enabled = false;                               // Assume User cannot Review Request
            btnExpView.Enabled = true;                                  // If any Request is selected, the user can always view it

            switch (state)
            {
                case ExpState.UnsubmittedByInternalCoordinator:
                    {
                        if (litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString()) // If == the user is a coordinator
                        {
                            btnExpDelete.Enabled = true;
                            btnExpEdit.Enabled = true;
                        }
                        break;
                    }
                case ExpState.UnsubmittedByProjectDirector:
                    {
                        if (litSavedProjectRole.Text == ProjectRole.ProjectDirector.ToString()) // If == the user is a PD
                        {
                            btnExpDelete.Enabled = true;
                            btnExpEdit.Enabled = true;
                        }
                        break;
                    }
                case ExpState.UnsubmittedByProjectStaff:
                    {
                        if (litSavedProjectRole.Text == ProjectRole.ProjectStaff.ToString()) // If == the user is a PS
                        {
                            btnExpDelete.Enabled = true;
                            btnExpEdit.Enabled = true;
                        }
                        break;
                    }
                case ExpState.AwaitingProjectDirector:
                    {
                        if (litSavedProjectRole.Text == ProjectRole.ProjectDirector.ToString()) // If == user is a Project Director, can Review
                            btnExpReview.Enabled = true;
                        break;
                    }
                case ExpState.Paid:
                    {
                        if ((litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString())
                            || litSavedProjectRole.Text == ProjectRole.ProjectDirector.ToString()) // If == the user can Archive
                        {
                            Label archivedLabel = (Label)gvAllExp.SelectedRow.FindControl("lblArchived"); // Find the label control that contains Archived
                            if (archivedLabel.Text == "False")      // If == not currently archived.
                                btnExpArchive.Enabled = true;       // Light Archive button. Can't archive if already archived
                        }
                        break;
                    }
                case ExpState.AwaitingTrustDirector:
                case ExpState.AwaitingFinanceDirector:
                case ExpState.AwaitingTrustExecutive:
                case ExpState.Approved:
                case ExpState.PaymentSent:
                case ExpState.Returned:
                    {
                        break;                                      // The button setup is just fine. No editing or deleting here
                    }
                default:
                    LogError.LogInternalError("ProjectDashboard", string.Format("Invalid ExpState '{0}' from database",
                        state)); // Fatal error
                    break;
            }
            return;
        }

        // Flip a page of the grid view control

        protected void gvAllApp_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                LoadAllApps(e.NewPageIndex);                            // Reload the grid view control to the specified page
                ResetAppContext();                                      // No selected row, no live buttons after a page flip
            }
            return;
        }

        // Flip a page of the grid view control

        protected void gvAllDep_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                LoadAllDeps(e.NewPageIndex);                            // Reload the grid view control to the specified page
                ResetDepContext();                                      // No selected row, no live buttons after a page flip
            }
            return;
        }

        // Flip a page of the grid view control

        protected void gvAllExp_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                LoadAllExps(e.NewPageIndex);                            // Reload the grid view control, position to new page
                ResetExpContext();                                      // No selected row, no live buttons after a page flip
            }
            return;
        }

        // APPROVAL functions

        // Archive button pressed. Just flip on the Archived flag

        protected void btnAppArchive_Click(object sender, EventArgs e)
        {
            int appID = QueryStringActions.ConvertID(FetchSelectedRowLabel(gvAllApp)).Int; // Get ready for action with an int version of the App ID

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    App toUpdate = context.Apps.Find(appID);            // Fetch App row by its key
                    if (toUpdate == null)
                        LogError.LogInternalError("ProjectDashboard", $"Unable to locate ApprovalID {appID.ToString()} in database");
                    // Log fatal error

                    toUpdate.Archived = true;                           // Mark request as Archived
                    AppHistory hist = new AppHistory();                 // Get a place to build a new AppHistory row
                    StateActions.CopyPreviousState(toUpdate, hist, "Archived"); // Create a AppHistory log row from "old" version of Approval
                    StateActions.SetNewAppState(toUpdate, hist.PriorAppState, litSavedUserID.Text, hist);
                    // Write down our current State (which doesn't change here) and authorship
                    context.AppHistorys.Add(hist);                      // Save new AppHistory row
                    context.SaveChanges();                              // Commit the Add or Modify
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ProjectDashboard",
                        "Error updating App and AppHistory rows");      // Fatal error
                }
                SearchCriteriaChanged(sender, e);                          // Update the grid view and buttons for display
                litSuccessMessage.Text = "Approval Request successfully Archived";    // Let user know we're good
            }
        }

        // Copy button pressed. Simple dispatch to EditApproval.

        protected void btnAppCopy_Click(object sender, EventArgs e)
        {
            string appID = FetchSelectedRowLabel(gvAllApp);               // Extract the text of the control, which is AppID
            Response.Redirect(PortalConstants.URLEditApproval + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + appID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandCopy); // Start with existing request
        }

        // Delete button pressed. We do this here.

        protected void btnAppDelete_Click(object sender, EventArgs e)
        {
            int appID = QueryStringActions.ConvertID(FetchSelectedRowLabel(gvAllApp)).Int; // Get ready for action with an int version of the Exp ID

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  1) Blow off the Supporting Docs associated with the Request. This means deleting the files and SupportingDoc rows.

                    SupportingActions.DeleteDocs(RequestType.Approval, appID);                // Great idea. Do that!

                    //  2) Delete the AppHistory rows associated with the Request

                    context.AppHistorys.RemoveRange(context.AppHistorys.Where(x => x.AppID == appID)); // Delete all of them

                    //  3) Kill off the Request itself. Do this last so that if something earlier breaks, we can recover - by deleting again.

                    App app = new App { AppID = appID };                // Instantiate an Exp object with the selected row's ID
                    context.Apps.Attach(app);                           // Find that record, but don't fetch it
                    context.Apps.Remove(app);                           // Delete that row
                    context.SaveChanges();                              // Commit the deletions
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ProjectDashboard",
                        "Error deleting App, SupportingDoc and AppHistory rows or deleting Supporting Document"); // Fatal error
                }
                SearchCriteriaChanged(sender, e);                          // Update the grid view and buttons for display
                litSuccessMessage.Text = "Approval deleted";            // Report success to our user
            }
        }

        // Edit button clicked. Fetch the ID of the selected row and dispatch to EditApproval.

        protected void btnAppEdit_Click(object sender, EventArgs e)
        {
            string appID = FetchSelectedRowLabel(gvAllApp);               // Extract the text of the control, which is AppID
            Response.Redirect(PortalConstants.URLEditApproval + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + appID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit); // Start with an existing request

        }

        protected void btnAppNew_Click(object sender, EventArgs e)
        {

            // Propagage the UserID and ProjectID that we were called with. No AppID means a new Request.

            Response.Redirect(PortalConstants.URLEditApproval + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew); // Start with an empty request
        }

        protected void btnAppReview_Click(object sender, EventArgs e)
        {
            string appID = FetchSelectedRowLabel(gvAllApp);               // Extract the text of the control, which is DepID

            // Unconditionally send Request to ReviewRequest. It is possible that the user does not have the authority to review the Request in
            // its current state. But we'll let ReviewRequest display all the detail for the Dep and then deny editing.

            Response.Redirect(PortalConstants.URLReviewApproval + "?" + PortalConstants.QSRequestID + "=" + appID + "&" // Start with an existing request
                                              + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview + "&" // Review it
                                              + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done
        }

        protected void btnAppView_Click(object sender, EventArgs e)
        {
            string appID = FetchSelectedRowLabel(gvAllApp);               // Extract the text of the control, which is AppID
            Response.Redirect(PortalConstants.URLEditApproval + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + appID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandView); // Start with existing request
        }

        // Start of DEPOSIT section

        // Archive button pressed. Just flip on the Archived flag

        protected void btnDepArchive_Click(object sender, EventArgs e)
        {
            int depID = QueryStringActions.ConvertID(FetchSelectedRowLabel(gvAllDep)).Int; // Get ready for action with an int version of the Dep ID

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    Dep toUpdate = context.Deps.Find(depID);            // Fetch Dep row by its key
                    if (toUpdate == null)
                        LogError.LogInternalError("ProjectDashboard", $"Unable to locate DepositID {depID.ToString()} in database");
                    // Log fatal error

                    toUpdate.Archived = true;                           // Mark request as Archived
                    DepHistory hist = new DepHistory();                 // Get a place to build a new DepHistory row
                    StateActions.CopyPreviousState(toUpdate, hist, "Archived"); // Create a DepHistory log row from "old" version of Deposit
                    StateActions.SetNewDepState(toUpdate, hist.PriorDepState, litSavedUserID.Text, hist);
                    // Write down our current State (which doesn't change here) and authorship
                    context.DepHistorys.Add(hist);                      // Save new DepHistory row
                    context.SaveChanges();                              // Commit the Add or Modify
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ProjectDashboard",
                        "Error updating Dep and DepHistory rows");      // Fatal error
                }
                SearchCriteriaChanged(sender, e);                          // Update the grid view and buttons for display
                litSuccessMessage.Text = "Deposit Request successfully Archived";    // Let user know we're good
            }
        }

        // Copy button pushed. Just dispatch
        protected void btnDepCopy_Click(object sender, EventArgs e)
        {
            string depID = FetchSelectedRowLabel(gvAllDep);               // Extract the text of the control, which is ExpID
            Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + depID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandCopy); // Start with existing request
        }

        // Delete button pushed. Clean up all the parts of the Deposit Request including supporting documents and history.

        protected void btnDepDelete_Click(object sender, EventArgs e)
        {
            int depID = QueryStringActions.ConvertID(FetchSelectedRowLabel(gvAllDep)).Int; // Extract the text of the control, which is DepID
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  1) Blow off the Supporting Docs associated with the Dep. This means deleting the files and SupportingDoc rows.

                    SupportingActions.DeleteDocs(RequestType.Deposit, depID); // Great idea. Do that!

                    //  2) Delete the DepHistory rows associated with the Dep

                    context.DepHistorys.RemoveRange(context.DepHistorys.Where(x => x.DepID == depID)); // Delete all of them

                    //  3) Kill off the Dep itself. Do this last so that if something earlier breaks, we can recover - by deleting again.

                    Dep dep = new Dep { DepID = depID };                // Instantiate an Dep object with the selected row's ID
                    context.Deps.Attach(dep);                           // Find that record, but don't fetch it
                    context.Deps.Remove(dep);                           // Delete that row
                    context.SaveChanges();                              // Commit the deletions
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ProjectDashboard",
                        "Error deleting Deposit, SupportingDoc and DepHistory rows or deleting Supporting Document"); // Fatal error
                }
                SearchCriteriaChanged(sender, e);                          // Update the grid view and buttons for display
                litSuccessMessage.Text = "Deposit deleted";             // Report success to our user
            }
            return;
        }

        // Edit button clicked. Fetch the DepID of the selected row and dispatch to EditdEPOSIT.

        protected void btnDepEdit_Click(object sender, EventArgs e)
        {
            string depID = FetchSelectedRowLabel(gvAllDep);               // Extract the text of the control, which is DepID
            Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + depID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit); // Start with an existing request
        }

        protected void btnDepNew_Click(object sender, EventArgs e)
        {

            // Propagage the UserID and ProjectID that we were called with. No DepID means a new Request.

            Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew); // Start with an empty request
        }

        // Review Request button clicked. This is a "Staff" function that only the Project Director can access. Dispatch to the ReviewExpense
        // page.

        protected void btnDepReview_Click(object sender, EventArgs e)
        {
            string depID = FetchSelectedRowLabel(gvAllDep);               // Extract the text of the control, which is DepID

            // Unconditionally send Dep to ReviewRequest. It is possible that the user does not have the authority to review the Dep in
            // its current state. But we'll let ReviewRequest display all the detail for the Dep and then deny editing.

            Response.Redirect(PortalConstants.URLReviewDeposit + "?" + PortalConstants.QSRequestID + "=" + depID + "&" // Start with an existing request
                                              + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview + "&" // Review it
                                              + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done
        }

        // View button clicked.
        protected void btnDepView_Click(object sender, EventArgs e)
        {
            string depID = FetchSelectedRowLabel(gvAllDep);               // Extract the text of the control, which is DepID
            Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + depID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandView); // Start with existing request
        }

        // Archive button pressed. Just flip on the Archived flag

        protected void btnExpArchive_Click(object sender, EventArgs e)
        {
            int expID = QueryStringActions.ConvertID(FetchSelectedRowLabel(gvAllExp)).Int; // Get ready for action with an int version of the Exp ID

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    Exp toUpdate = context.Exps.Find(expID);            // Fetch Exp row by its key
                    if (toUpdate == null)
                        LogError.LogInternalError("ProjectDashboard", $"Unable to locate ExpenseID {expID.ToString()} in database");
                    // Log fatal error

                    toUpdate.Archived = true;                           // Mark request as Archived
                    ExpHistory hist = new ExpHistory();                 // Get a place to build a new ExpHistory row
                    StateActions.CopyPreviousState(toUpdate, hist, "Archived"); // Create a ExpHistory log row from "old" version of Expense
                    StateActions.SetNewExpState(toUpdate, hist.PriorExpState, litSavedUserID.Text, hist);
                    // Write down our current State (which doesn't change here) and authorship
                    context.ExpHistorys.Add(hist);                      // Save new ExpHistory row
                    context.SaveChanges();                              // Commit the Add or Modify
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ProjectDashboard",
                        "Error updating Exp and ExpHistory rows");      // Fatal error
                }
                SearchCriteriaChanged(sender, e);                          // Update the grid view and buttons for display
                litSuccessMessage.Text = "Expense Request successfully Archived";    // Let user know we're good
            }
        }

        // Copy button clicked. Conclude what we've been working on, then hit the road.

        protected void btnExpCopy_Click(object sender, EventArgs e)
        {
            string expID = FetchSelectedRowLabel(gvAllExp);               // Extract the text of the control, which is ExpID
            Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + expID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandCopy); // Start with existing request
        }

        // Delete button clicked. Wipe out the Request and it's associated rows and files.

        protected void btnExpDelete_Click(object sender, EventArgs e)
        {
            int expID = QueryStringActions.ConvertID(FetchSelectedRowLabel(gvAllExp)).Int; // Get ready for action with an int version of the Exp ID

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  0) Delete the ExpSplit rows associated with this Exp.

                    SplitActions.DeleteSplitRows(expID);                // Delete all ExpSplit rows with this ExpID

                    //  1) Blow off the Supporting Docs associated with the Exp. This means deleting the files and SupportingDoc rows.

                    SupportingActions.DeleteDocs(RequestType.Expense, expID);                // Great idea. Do that!

                    //  2) Delete the ExpHistory rows associated with the Exp

                    context.ExpHistorys.RemoveRange(context.ExpHistorys.Where(x => x.ExpID == expID)); // Delete all of them

                    //  3) Kill off the Exp itself. Do this last so that if something earlier breaks, we can recover - by deleting again.

                    Exp exp = new Exp { ExpID = expID };                // Instantiate an Exp object with the selected row's ID
                    context.Exps.Attach(exp);                           // Find that record, but don't fetch it
                    context.Exps.Remove(exp);                           // Delete that row
                    context.SaveChanges();                              // Commit the deletions
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ProjectDashboard", 
                        "Error deleting Exp, ExpSplit, SupportingDoc and ExpHistory rows or deleting Supporting Document"); // Fatal error
                }
                SearchCriteriaChanged(sender, e);                       // Update the grid view and buttons for display
                litSuccessMessage.Text = "Expense request deleted";     // Report success to our user
            }
       }

        // Edit button clicked. Fetch the ExpID of the selected row and dispatch to EditExpense.

        protected void btnExpEdit_Click(object sender, EventArgs e)
        {
            string expID = FetchSelectedRowLabel(gvAllExp);               // Extract the text of the control, which is ExpID
            Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + expID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit); // Start with an existing request
        }

        // New Request button has been clicked. Dispatch to Edit Detail

        protected void btnExpNew_Click(object sender, EventArgs e)
        {

            // Propagage the UserID and ProjectID that we were called with. No ExpID means a new Request.
            
            Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew); // Start with an empty request
        }

        // Review Request button clicked. This is a "Staff" function that only the Project Director can access. Dispatch to the ReviewExpense
        // page.

        protected void btnExpReview_Click(object sender, EventArgs e)
        {
            string expID = FetchSelectedRowLabel(gvAllExp);               // Extract the text of the control, which is ExpID

            // Unconditionally send Exp to ReviewRequest. It is possible that the user does not have the authority to review the exp in
            // its current state. But we'll let ReviewRequest display all the detail for the Exp and then deny editing.

            Response.Redirect(PortalConstants.URLReviewExpense + "?" + PortalConstants.QSRequestID + "=" + expID + "&" // Start with an existing request
                                              + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview + "&" // Review it
                                              + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done

        }

        // View button clicked. 

        protected void btnExpView_Click(object sender, EventArgs e)
        {
            string expID = FetchSelectedRowLabel(gvAllExp);               // Extract the text of the control, which is ExpID
            Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + expID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandView); // Start with existing request
        }

        // Fetch all of the Requests for this project, subject to further search constraints. Display in a GridView.
        // Find current project ID in listSavedProjectID

        void LoadAllApps(int pageIndex = 0)
        {
            if (!pnlApp.Visible)                                        // If ! the panel is hidden, no need to refresh
                return;

            gvAllApp.PageIndex = pageIndex;                             // Go to the page specified by the caller
            int projectID = Convert.ToInt32(litSavedProjectID.Text);    // Fetch ID of current project as an int
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var pred = PredicateBuilder.True<App>();                // Initialize predicate to select from App table
                pred = pred.And(r => r.ProjectID == projectID && !r.Inactive);  // Select requests for this project only and not Inactive

                // Process "Active" and "Archived" flags. They're not independent, so we have to grind through their combinations

                if (ckRActive.Checked && ckRArchived.Checked)           // If true, take them all
                    ;                                                   // Don't ignore anything
                else if (ckRActive.Checked && !ckRArchived.Checked)     // If true, only Active checked
                    pred = pred.And(r => !r.Archived);                  // Ignore Archived requests
                else if (!ckRActive.Checked && ckRArchived.Checked)     // If true, only Archived checked
                    pred = pred.And(r => r.Archived);                   // Ignore Active requests
                else                                                    // Both boxes are unchecked
                    pred = pred.And(r => r.Archived && !r.Archived);    // Nonsensical. Returns nothing

                // Deal with date range filter. Remember that we've already validated the date strings, so we can just use them.

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

                List<App> apps = context.Apps.AsExpandable().Where(pred).OrderByDescending(o => o.CurrentTime).ToList();
                // Do the query using the constructed predicate, sort the result, and create a list of App rows

                // From this list of Apps, build a list of rows for the gvAllApp GridView

                List<ProjectAppViewRow> rows = new List<ProjectAppViewRow>(); // Create an empty list for the GridView control
                foreach (var r in apps)                                 // Fill the list row-by-row
                {
                    bool useRow = false;                                // Assume that we skip this row
                    switch(r.CurrentState)
                    {
                        case AppState.UnsubmittedByInternalCoordinator:
                        case AppState.UnsubmittedByProjectDirector:
                        case AppState.UnsubmittedByProjectStaff:
                        case AppState.AwaitingProjectDirector:
                            if (ckRUnsubmitted.Checked)                 // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case AppState.AwaitingFinanceDirector:
                        case AppState.AwaitingInternalCoordinator:
                        case AppState.AwaitingTrustDirector:
                        case AppState.AwaitingTrustExecutive:
                            if (ckRAwaitingCWStaff.Checked)             // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case AppState.Approved:
                            if (ckRApproved.Checked)                    // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case AppState.Returned:
                            if (ckRReturned.Checked)                    // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        default:                                        // For all other oddballs, just skip the row
                            break;
                    }

                    if (useRow)                                         // If true. checkboxes indicate that we should use the row
                    {
                        ProjectAppViewRow row = new ProjectAppViewRow()     // Empty row all ready to fill
                        {
                            RowID = r.AppID.ToString(),                     // Convert ID from int to string for easier retrieval later
                            CurrentTime = r.CurrentTime,                    // When request was last updated
                            AppTypeDesc = EnumActions.GetEnumDescription(r.AppType), // Convert enum version to English version for display
                            Description = r.Description,                    // Free text description of deposit
                            CurrentState = r.CurrentState,                  // Load enum version for use when row is selected. But not visible
                            CurrentStateDesc = EnumActions.GetEnumDescription(r.CurrentState), // Convert enum version to English version for display
                            Archived = r.Archived                           // Load archived state

                        };
                        if (r.Archived)                                     // If true row is Archived
                            row.CurrentStateDesc = row.CurrentStateDesc + " (Archived)"; // Append indication that it's archifed

                        rows.Add(row);                                      // Add the filled-in row to the list of rows
                    }
                }
                gvAllApp.DataSource = rows;                           // Give it to the GridView control
                gvAllApp.DataBind();                                  // And get it in gear

                NavigationActions.EnableGridViewNavButtons(gvAllApp); // Enable appropriate nav buttons based on page count
                gvAllApp.SelectedIndex = -1;                          // No selected row any more
            }
            return;
        }

        void LoadAllDeps(int pageIndex = 0)
        {
            if (!pnlDep.Visible)                                        // If ! the panel is hidden, no need to refresh
                return;

            gvAllDep.PageIndex = pageIndex;                           // Go to the page specified by the caller
            int projectID = Convert.ToInt32(litSavedProjectID.Text);    // Fetch ID of current project as an int
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var pred = PredicateBuilder.True<Dep>();               // Initialize predicate to select from Dep table
                pred = pred.And(r => r.ProjectID == projectID && !r.Inactive);  // Select requests for this project only and not Inactive

                // Process "Active" and "Archived" flags. They're not independent, so we have to grind through their combinations

                if (ckRActive.Checked && ckRArchived.Checked)           // If true, take them all
                    ;                                                   // Don't ignore anything
                else if (ckRActive.Checked && !ckRArchived.Checked)     // If true, only Active checked
                    pred = pred.And(r => !r.Archived);                  // Ignore Archived requests
                else if (!ckRActive.Checked && ckRArchived.Checked)     // If true, only Archived checked
                    pred = pred.And(r => r.Archived);                   // Ignore Active requests
                else                                                    // Both boxes are unchecked
                    pred = pred.And(r => r.Archived && !r.Archived);    // Nonsensical. Returns nothing

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

                // If a specific Entity is selected, only select requests for that Entity

                if (ddlEntityName.SelectedIndex > 0)                       // If > Entity is selected. Fetch rqsts only for that Entity
                {
                    int id = Convert.ToInt32(ddlEntityName.SelectedValue);  // Convert ID of selected Entity
                    pred = pred.And(r => r.EntityID == id);                 // Only requests from selected Entity
                }

                // If a specific Person is selected, only select requests from that Person

                if (ddlPersonName.SelectedIndex > 0)                        // If > Person is selected. Fetch rqsts only for that Person
                {
                    int id = Convert.ToInt32(ddlPersonName.SelectedValue);  // Convert ID of selected Person
                    pred = pred.And(r => r.PersonID == id);                 // Only requests from selected Person
                }

                List<Dep> deps = context.Deps.AsExpandable().Where(pred).OrderByDescending(o => o.CurrentTime).ToList();
                // Do the query using the constructed predicate, sort the result, and create a list of Dep rows

                // From this list of Deps, build a list of rows for the gvAllExp GridView

                List<ProjectDepViewRow> rows = new List<ProjectDepViewRow>(); // Create an empty list for the GridView control
                foreach (var r in deps)                                 // Fill the list row-by-row
                {
                    bool useRow = false;                                // Assume that we skip this row
                    switch (r.CurrentState)
                    {
                        case DepState.UnsubmittedByInternalCoordinator:
                        case DepState.AwaitingProjectDirector:
                            if (ckRUnsubmitted.Checked)                 // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case DepState.AwaitingFinanceDirector:
                        case DepState.AwaitingTrustDirector:
                            if (ckRAwaitingCWStaff.Checked)             // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case DepState.DepositComplete:
                            if (ckRApproved.Checked)                    // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case DepState.Returned:
                            if (ckRReturned.Checked)                    // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        default:                                        // For all other oddballs, just skip the row
                            break;
                    }

                    if (useRow)                                         // If true. checkboxes indicate that we should use the row
                    {
                        ProjectDepViewRow row = new ProjectDepViewRow()     // Empty row all ready to fill
                        {
                            RowID = r.DepID.ToString(),                     // Convert ID from int to string for easier retrieval later
                            CurrentTime = r.CurrentTime,                    // When request was last updated
                            DepTypeDesc = EnumActions.GetEnumDescription(r.DepType), // Convert enum version to English version for display
                            Description = r.Description,                    // Free text description of deposit
                                                                            // Source Of Funds is trickier, so done below
                            Amount = ExtensionActions.LoadDecimalIntoTxt(r.Amount), // Carefully load decimal amount into text field
                            CurrentState = r.CurrentState,                  // Load enum version for use when row is selected. But not visible
                            CurrentStateDesc = EnumActions.GetEnumDescription(r.CurrentState), // Convert enum version to English version for display
                            Archived = r.Archived                           // Load archived state

                        };

                        // Produce an elaborate Source Of Funds description

                        switch (r.SourceOfFunds)
                        {
                            case SourceOfDepFunds.NA:
                                {
                                    row.SourceOfFunds = EnumActions.GetEnumDescription(SourceOfDepFunds.NA);
                                    break;
                                }
                            case SourceOfDepFunds.Entity:
                                {
                                    if (r.EntityID != null)                     // If != ID of Entity is available, show it off
                                        row.SourceOfFunds = EnumActions.GetEnumDescription(SourceOfDepFunds.Entity) + ": " + r.Entity.Name;
                                    else
                                        row.SourceOfFunds = EnumActions.GetEnumDescription(SourceOfDepFunds.Entity);
                                    break;
                                }
                            case SourceOfDepFunds.Individual:
                                {
                                    if (r.PersonID != null)                     // If != ID of Person is available, show it off
                                        row.SourceOfFunds = EnumActions.GetEnumDescription(SourceOfDepFunds.Individual) + ": " + r.Person.Name;
                                    else
                                        row.SourceOfFunds = EnumActions.GetEnumDescription(SourceOfDepFunds.Individual);
                                    break;
                                }
                            default:
                                {
                                    LogError.LogInternalError("ProjectDashboard", string.Format("Invalid SourceOfDepFunds value '{0}' found in database",
                                        r.SourceOfFunds.ToString())); // Fatal error
                                    break;
                                }
                        }
                        if (r.Archived)                                     // If true row is Archived
                            row.CurrentStateDesc = row.CurrentStateDesc + " (Archived)"; // Append indication that it's archifed

                        rows.Add(row);                                      // Add the filled-in row to the list of rows
                    }
                }
                gvAllDep.DataSource = rows;                           // Give it to the GridView control
                gvAllDep.DataBind();                                  // And get it in gear

                NavigationActions.EnableGridViewNavButtons(gvAllDep); // Enable appropriate nav buttons based on page count
                gvAllDep.SelectedIndex = -1;                          // No selected row any more
            }
            return;
        }

        // Fetch all of the Requests for this project, subject to further search constraints. Display in a GridView.
        // Find current project ID in listSavedProjectID

        void LoadAllExps(int pageIndex = 0)
        {
            if (!pnlExp.Visible)                                        // If ! the panel is hidden, no need to refresh
                return;

            gvAllExp.PageIndex = pageIndex;                           // Select which page of grid to display
            int projectID = Convert.ToInt32(litSavedProjectID.Text);    // Fetch ID of current project as an int
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var pred = PredicateBuilder.True<Exp>();               // Initialize predicate to select from Exp table
                pred = pred.And(r => r.ProjectID == projectID && !r.Inactive);  // Select requests for this project only and not Inactive

                // Process "Active" and "Archived" flags. They're not independent, so we have to grind through their combinations

                if (ckRActive.Checked && ckRArchived.Checked)           // If true, take them all
                    ;                                                   // Don't ignore anything
                else if (ckRActive.Checked && !ckRArchived.Checked)     // If true, only Active checked
                    pred = pred.And(r => !r.Archived);                  // Ignore Archived requests
                else if (!ckRActive.Checked && ckRArchived.Checked)     // If true, only Archived checked
                    pred = pred.And(r => r.Archived);                   // Ignore Active requests
                else                                                    // Both boxes are unchecked
                    pred = pred.And(r => r.Archived && !r.Archived);    // Nonsensical. Returns nothing

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

                // If a specific Entity is selected, only select requests for that Entity

                if (ddlEntityName.SelectedIndex > 0)                       // If > Entity is selected. Fetch rqsts only for that Entity
                {
                    int id = Convert.ToInt32(ddlEntityName.SelectedValue);  // Convert ID of selected Entity
                    pred = pred.And(r => r.EntityID == id);                 // Only requests from selected Entity
                }

                // If a specific Person is selected, only select requests from that Person

                if (ddlPersonName.SelectedIndex > 0)                        // If > Person is selected. Fetch rqsts only for that Person
                {
                    int id = Convert.ToInt32(ddlPersonName.SelectedValue);  // Convert ID of selected Person
                    pred = pred.And(r => r.PersonID == id);                 // Only requests from selected Person
                }

                List<Exp> exps = context.Exps.AsExpandable().Where(pred).OrderByDescending(o => o.CurrentTime).ToList(); 
                // Do the query using the constructed predicate, sort the result, and create a list of Exp rows

                // From this list of Exps, build a list of rows for the gvAllExp GridView

                List<ProjectExpViewRow> rows = new List<ProjectExpViewRow>(); // Create an empty list for the GridView control
                foreach (var r in exps)                                // Fill the list row-by-row
                {
                    bool useRow = false;                                // Assume that we skip this row
                    switch (r.CurrentState)
                    {
                        case ExpState.UnsubmittedByInternalCoordinator:
                        case ExpState.UnsubmittedByProjectDirector:
                        case ExpState.UnsubmittedByProjectStaff:
                        case ExpState.AwaitingProjectDirector:
                            if (ckRUnsubmitted.Checked)                 // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case ExpState.AwaitingFinanceDirector:
                        case ExpState.AwaitingTrustDirector:
                        case ExpState.AwaitingTrustExecutive:
                        case ExpState.Approved:
                        case ExpState.PaymentSent:
                            if (ckRAwaitingCWStaff.Checked)             // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case ExpState.Paid:
                            if (ckRApproved.Checked)                    // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case ExpState.Returned:
                            if (ckRReturned.Checked)                    // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        default:                                        // For all other oddballs, just skip the row
                            break;
                    }

                    if (useRow)                                         // If true. checkboxes indicate that we should use the row
                    {
                        ProjectExpViewRow row = new ProjectExpViewRow()   // Empty row all ready to fill
                        {
                            RowID = r.ExpID.ToString(),                    // Convert ID from int to string for easier retrieval later
                            CurrentTime = r.CurrentTime,                    // When request was last updated
                            ExpTypeDesc = EnumActions.GetEnumDescription(r.ExpType), // Convert enum version to English version for display
                            Amount = ExtensionActions.LoadDecimalIntoTxt(r.Amount), // Carefully load decimal amount into text field
                            CurrentState = r.CurrentState,                  // Load enum version for use when row is selected
                            CurrentStateDesc = EnumActions.GetEnumDescription(r.CurrentState), // Convert enum version to English version for display
                            Archived = r.Archived,                          // Whether the request is archived
                            Rush = r.Rush                                   // Whether the request is Rush
                        };

                        // Fill "Target" with Vendor Name or Person Name. Only one will be present, depending on ExpType.

                        if (r.Entity != null)                               // If != a Vendor is present in the Request
                        {
                            if (r.Entity.Name != null)
                                row.Target = r.Entity.Name;
                        }
                        else if (r.Person != null)                          // If != an Employee is present in the Request
                        {
                            if (r.Person.Name != null)
                                row.Target = r.Person.Name;
                        }
                        row.Description = r.Description.TrimString(40);     // Load the description, but don't let it get too long

                        if (r.Archived)                                     // If true row is Archived
                            row.CurrentStateDesc = row.CurrentStateDesc + " (Archived)"; // Append indication that it's archifed

                        rows.Add(row);                                      // Add the filled-in row to the list of rows
                    }
                }
                gvAllExp.DataSource = rows;                           // Give it to the GridView control
                gvAllExp.DataBind();                                  // And get it in gear

                NavigationActions.EnableGridViewNavButtons(gvAllExp); // Enable appropriate nav buttons based on page count
                gvAllExp.SelectedIndex = -1;                          // No selected row any more
            }
        }

        // We no longer have a selected Request. Adjust buttons

        void ResetAppContext()
        {
            btnAppArchive.Enabled = false;
            btnAppCopy.Enabled = false;
            btnAppDelete.Enabled = false;
            btnAppEdit.Enabled = false;                                 // Therefore, the buttons don't work
                                                                        // Leave New button in its former state
            btnAppReview.Enabled = false;
            btnAppView.Enabled = false;
        }

        void ResetDepContext()
        {
            btnDepArchive.Enabled = false;
            btnDepCopy.Enabled = false;
            btnDepDelete.Enabled = false;
            btnDepEdit.Enabled = false;                                 // Therefore, the buttons don't work
                                                                        // Leave New button in its former state
            btnDepReview.Enabled = false;
            btnDepView.Enabled = false;
        }

        void ResetExpContext()
        {
            btnExpArchive.Enabled = false;
            btnExpCopy.Enabled = false;
            btnExpDelete.Enabled = false;
            btnExpEdit.Enabled = false;                                 // Therefore, the buttons don't work
            btnExpReview.Enabled = false;
            btnExpView.Enabled = false;
        }

        // A little common fragment that pulls the request ID from the selected row of the GridView control and makes sure it's not blank.

        string FetchSelectedRowLabel(GridView allview)
        {
            Label label = (Label)allview.SelectedRow.FindControl("lblRowID"); // Find the label control that contains DepID or ExpID
            string ID = label.Text;                                     // Extract the text of the control, which is ExpID
            if (ID == "")
                LogError.LogInternalError("ProjectDashboard", string.Format(
                    "Unable to find Deposit/Expense ID '{0}' from selected GridView row in database", ID)); // Fatal error
            return ID;
        }

        // Fill a Drop Down List with available Entity Names

        void FillEntityDDL(int? entityID)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                int projID = QueryStringActions.ConvertID(litSavedProjectID.Text).Int; // Find ID of current project

                var query = (from pe in context.ProjectEntitys
                            where pe.ProjectID == projID            // This project only, all roles
                            select new { EntityID = pe.EntityID, Name = pe.Entity.Name }
                            ).Distinct().OrderBy(x => x.Name);      // Find all Entitys that are assigned to this project

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

                StateActions.LoadDdl(ddlEntityName, entityID, rows,
                    "", "-- All Project Entities --", alwaysDisplayLeader: true); // Put the cherry on top
            }
            return;
        }

        // Fill a Drop Down List with available Person Names

        void FillPersonDDL(int? personID)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                int projID = QueryStringActions.ConvertID(litSavedProjectID.Text).Int; // Find ID of current project

                var query = (from pp in context.ProjectPersons
                            where pp.ProjectID == projID            // This project only
                            select new { PersonID = pp.PersonID, Name = pp.Person.Name }
                            ).Distinct().OrderBy(x => x.Name);      // Find all Persons in all roles

                DataTable rows = new DataTable();
                rows.Columns.Add(PortalConstants.DdlID);
                rows.Columns.Add(PortalConstants.DdlName);

                foreach (var row in query)
                {
                    DataRow dr = rows.NewRow();                      // Build a row from the next query output
                    dr[PortalConstants.DdlID] = row.PersonID;
                    dr[PortalConstants.DdlName] = row.Name;
                    rows.Rows.Add(dr);                               // Add the new row to the data table
                }

                StateActions.LoadDdl(ddlPersonName, personID, rows,
                    "", "-- All Project Persons --", alwaysDisplayLeader: true); // Put the cherry on top
            }
            return;
        }

        // Save the current settings of the checkboxes in a (big) cookie

        void SaveCheckboxes()
        {
            HttpCookie projectCheckboxesCookie = new HttpCookie(PortalConstants.CProjectCheckboxes);

            projectCheckboxesCookie[PortalConstants.CProjectCkSearchVisible] = pnlSearch.Visible.ToString();

            projectCheckboxesCookie[PortalConstants.CProjectCkRUnsubmitted] = ckRUnsubmitted.Checked.ToString();
            projectCheckboxesCookie[PortalConstants.CProjectCkRAwaitingCWStaff] = ckRAwaitingCWStaff.Checked.ToString();
            projectCheckboxesCookie[PortalConstants.CProjectCkRApproved] = ckRApproved.Checked.ToString();
            projectCheckboxesCookie[PortalConstants.CProjectCkRReturned] = ckRReturned.Checked.ToString();

            projectCheckboxesCookie[PortalConstants.CProjectFromDate] = txtBeginningDate.Text;
            projectCheckboxesCookie[PortalConstants.CProjectToDate] = txtEndingDate.Text;

            projectCheckboxesCookie[PortalConstants.CProjectCkRActive] = ckRActive.Checked.ToString();
            projectCheckboxesCookie[PortalConstants.CProjectCkRArchived] = ckRArchived.Checked.ToString();

            int? selection = StateActions.UnloadDdl(ddlEntityName);
            if (selection != null)                                      // If != something is selected
                projectCheckboxesCookie[PortalConstants.CProjectDdlEntityID] = selection.ToString();
            selection = StateActions.UnloadDdl(ddlPersonName);
            if (selection != null)                                      // If != something is selected
                projectCheckboxesCookie[PortalConstants.CProjectDdlPersonID] = selection.ToString();

            projectCheckboxesCookie[PortalConstants.CProjectAppVisible] = pnlApp.Visible.ToString();
            projectCheckboxesCookie[PortalConstants.CProjectDepVisible] = pnlDep.Visible.ToString();
            projectCheckboxesCookie[PortalConstants.CProjectExpVisible] = pnlExp.Visible.ToString();

            Response.Cookies.Add(projectCheckboxesCookie);                    // Store a new cookie
        }

        // Read that cookie, if present, and restore the current settings of the checkboxes

        void RestoreCheckboxes()
        {
            HttpCookie projectCheckboxesCookie = Request.Cookies[PortalConstants.CProjectCheckboxes]; // Ask for the Staff Checkbox cookie
            if (projectCheckboxesCookie != null)                              // If != the cookie is present
            {

                // Search panel

                if (projectCheckboxesCookie[PortalConstants.CProjectCkSearchVisible] == "True")
                    ExpandSearchPanel();
                else
                    CollapseSearchPanel();

                // Filter Panel

                ckRUnsubmitted.Checked = false;                        // Assume box should be unchecked
                if (projectCheckboxesCookie[PortalConstants.CProjectCkRUnsubmitted] == "True") ckRUnsubmitted.Checked = true;

                ckRAwaitingCWStaff.Checked = false;                             // Assume box should be unchecked
                if (projectCheckboxesCookie[PortalConstants.CProjectCkRAwaitingCWStaff] == "True") ckRAwaitingCWStaff.Checked = true;

                ckRApproved.Checked = false;                                // Assume box should be unchecked
                if (projectCheckboxesCookie[PortalConstants.CProjectCkRApproved] == "True") ckRApproved.Checked = true;

                ckRReturned.Checked = false;                                    // Assume box should be unchecked
                if (projectCheckboxesCookie[PortalConstants.CProjectCkRReturned] == "True") ckRReturned.Checked = true;

                ckRActive.Checked = false;                                    // Assume box should be unchecked
                if (projectCheckboxesCookie[PortalConstants.CProjectCkRActive] == "True") ckRActive.Checked = true;

                ckRArchived.Checked = false;                                    // Assume box should be unchecked
                if (projectCheckboxesCookie[PortalConstants.CProjectCkRArchived] == "True") ckRArchived.Checked = true;

                // Date Range

                txtBeginningDate.Text = projectCheckboxesCookie[PortalConstants.CProjectFromDate];
                txtEndingDate.Text = projectCheckboxesCookie[PortalConstants.CProjectToDate];

                // Dropdown Lists

                string entityID = projectCheckboxesCookie[PortalConstants.CProjectDdlEntityID];
                if (entityID == null)                                       // If = cookie doesn't contain an Entity ID; no selection
                    FillEntityDDL(null);                                    // Fill the list, no selection
                else
                    FillEntityDDL(Convert.ToInt32(entityID));               // Fill the list, highlight selection

                string personID = projectCheckboxesCookie[PortalConstants.CProjectDdlPersonID];
                if (personID == null)                                       // If = cookie doesn't contain an Entity ID; no selection
                    FillPersonDDL(null);                                    // Fill the list, no selection
                else
                    FillPersonDDL(Convert.ToInt32(personID));               // Fill the list, highlight selection

                // Archive check boxes

                string active = projectCheckboxesCookie[PortalConstants.CProjectCkRActive];
                if (active != null)                             // If != value is present
                {
                    if (active == "True")                       // If == checkbox was checked
                        ckRActive.Checked = true;
                    else
                        ckRActive.Checked = false;
                }

                string archive = projectCheckboxesCookie[PortalConstants.CProjectCkRArchived];
                if (archive != null)                            // If != value is present
                {
                    if (archive == "True")                      // If == checkbox was checked
                        ckRArchived.Checked = true;
                    else
                        ckRArchived.Checked = false;
                }

                // Approvel Panel

                if (projectCheckboxesCookie[PortalConstants.CProjectAppVisible] == "True")
                    ExpandAppPanel();                                           // Expand, but don't fill Approval panel
                else
                    CollapseAppPanel();                                         // Start with Approval panel collapsed

                // Deposit Panel

                if (projectCheckboxesCookie[PortalConstants.CProjectDepVisible] == "True")
                    ExpandDepPanel();                                           // Expand, but don't fill Deposits panel
                else
                    CollapseDepPanel();                                         // Start with Deposits panel collapsed

                // Expense Panel

                if (projectCheckboxesCookie[PortalConstants.CProjectExpVisible] == "True")
                    ExpandExpPanel();                                           // Expand, but don't fill Requests panel
                else
                    CollapseExpPanel();                                         // Start with Requests panel collapsed
            }
            else                                                        // Cookie doesn't exist
            {
                FillEntityDDL(null);                                    // Fill the list, no selection
                FillPersonDDL(null);
            }
        }
    }
}
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
using System.Globalization;

namespace Portal11.Proj
{
    public partial class ProjectDashboard : System.Web.UI.Page
    {

        // We've got three separate pages here, one for Deposits one for Approvals, and one for Expenses. 
        // The result is a lot of code in this module.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Display grids of Deposits, Documents and Expenses for a project. Communication is through Query Strings:
                //      UserID - the database ID of the Project Director (if missing, use cookie)
                //      ProjectID - the database ID of the Project whose Requests are to be displayed (if missing, use cookie)
                //  also
                //      Severity and Status - to allow callers to ask us to display exiting success or danger messages

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                litSavedUserID.Text = QueryStringActions.GetUserID();   // Track down User ID and save in a "faster" place for later reference

                QSValue projID = QueryStringActions.GetProjectID();     // Look on the query string for Project ID
                litSavedProjectID.Text = projID.String;                 // Save in a faster spot for later

                litSavedProjectRole.Text = QueryStringActions.GetProjectRole(); // Track down role and save in a faster spot for later

                // Fill project-specific date and amount fields at the top of the page.

                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    Project proj = context.Projects.Find(projID.Int);   // Fetch target project by ID
                    if (proj == null)                                   // If == couldn't find the project
                        LogError.LogInternalError("ProjectDashboard", $"Unable to find ProjectID '{projID.Int}' in database"); // Fatal error

                    litBalance.Text = proj.BalanceDate.ToString("MM/dd/yyyy", DateTimeFormatInfo.CurrentInfo);
                    litCurrentFunds.Text = ExtensionActions.LoadDecimalIntoTxt(proj.CurrentFunds);
                }

                // If the User is a Coordinator, they can create a new Deposit.

                if (litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString()) // If == User is a InternalCoordinator. Powerful
                    btnDepNew.Enabled = true;                           // Allow them to create a new Deposit

                RestoreCheckboxes();                                    // Restore more recent checkbox settings from a cookie

                int rows = CookieActions.GetGridViewRows();            // Find number of rows per page from cookie
                //gvAllApp.PageSize = rows;                               // Adjust grids accordingly
                gvAllDep.PageSize = rows;
                gvAllDoc.PageSize = rows;
                gvAllExp.PageSize = rows;

                //LoadAllApps();
                LoadAllDeps();
                LoadAllDocs();
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
            //LoadAllApps();                                              // Reload the GridView using new criteria
            //ResetAppContext();                                          // No selected row, no live buttons

            LoadAllDeps();                                              // Reload the GridView using new criteria
            ResetDepContext();                                          // No selected row, no live buttons

            LoadAllDocs();                                              // Reload the GridView using new criteria
            ResetDocContext();                                          // No selected row, no live buttons

            LoadAllExps();                                              // Reload the GridView using new criteria
            ResetExpContext();                                          // No selected row, no live buttons

            SaveCheckboxes();                                           // Save current checkbox settings in a cookie
            return;
        }

        // Collapse/Expand the panels of Approvals

        //protected void btnAppCollapse_Click(object sender, EventArgs e)
        //{
        //    CollapseAppPanel();
        //    SaveCheckboxes();                                               // Save current checkbox settings in a cookie
        //    return;
        //}

        //void CollapseAppPanel()
        //{
        //    pnlApp.Visible = false;
        //    btnAppCollapse.Visible = false;
        //    btnAppExpand.Visible = true;
        //    return;
        //}

        //protected void btnAppExpand_Click(object sender, EventArgs e)
        //{
        //    ExpandAppPanel();
        //    LoadAllApps();
        //    ResetAppContext();                                          // No selected row, no live buttons
        //    SaveCheckboxes();                                           // Save current checkbox settings in a cookie
        //    return;
        //}

        //void ExpandAppPanel()
        //{
        //    pnlApp.Visible = true;
        //    btnAppCollapse.Visible = true;
        //    btnAppExpand.Visible = false;
        //    return;
        //}

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

        // Collapse/Expand the panels of Documents

        protected void btnDocCollapse_Click(object sender, EventArgs e)
        {
            CollapseDocPanel();
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }

        void CollapseDocPanel()
        {
            pnlDoc.Visible = false;
            btnDocCollapse.Visible = false;
            btnDocExpand.Visible = true;
            return;
        }

        protected void btnDocExpand_Click(object sender, EventArgs e)
        {
            ExpandDocPanel();
            LoadAllDocs();
            ResetDocContext();                                          // No selected row, no live buttons
            SaveCheckboxes();                                           // Save current checkbox settings in a cookie
            return;
        }

        void ExpandDocPanel()
        {
            pnlDoc.Visible = true;
            btnDocCollapse.Visible = true;
            btnDocExpand.Visible = false;
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

        //protected void gvAllApp_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)            // If == this is indeed a row of our GridView control
        //    {
        //        Common_RowDataBound(sender, e);

        //        // See if the Request awaits action by the User. Find the right cell by its ID. The cell contains the text of the AppState enum value. 

        //        Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
        //        AppState state = EnumActions.ConvertTextToAppState(label.Text); // Carefully convert back into enumeration type

        //        if (StateActions.ProjectRoleToProcessRequest(state) == EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text)) // If == user can operate on Request
        //            e.Row.Cells[rowProjectAppView.CurrentStateDescRow].Font.Bold = true; // Bold the Status cell of this row
        //    }
        //    return;
        //}

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
                    e.Row.Cells[rowProjectDepView.CurrentStateDescRow].Font.Bold = true; // Bold Status cell.

                // See if the row is in revision by staff

                //label = (Label)e.Row.FindControl("lblReviseUserRole");  // Find the label control that contains ReviseUserRole in this row
                //UserRole reviseRole = EnumActions.ConvertTextToUserRole(label.Text); // Carefully convert back into enumeration type
                //if (reviseRole != UserRole.None)                        // If != row is in revision
                //{
                //    ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Fetch Project Role 
                //    if (RoleActions.ProjectRoleIsStaff(projectRole))    // If true user is staff
                //        e.Row.ForeColor = PortalConstants.DashboardRevising; // How about something in a nice teal?
                //}
            }
            else if (e.Row.RowType == DataControlRowType.Header)        // If == this is a header row
            {
                ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Fetch Project Role 
                if (RoleActions.ProjectRoleIsStaff(projectRole))        // If true user is staff
                    e.Row.Cells[rowProjectExpView.TimeRow].Text = "Date Created"; // They see time row was created
            }
            return;
        }

        protected void gvAllDoc_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)            // If == this is indeed a row of our GridView control
            {
                Common_RowDataBound(sender, e);

                // See if the Request awaits action by the User. Find the right cell by its ID. The cell contains the text of the DocState enum value. 

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                DocState state = EnumActions.ConvertTextToDocState(label.Text); // Carefully convert back into enumeration type

                if (StateActions.ProjectRoleToProcessRequest(state) == EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text)) // If == user can operate on Request
                    e.Row.Cells[rowProjectDocView.CurrentStateDescRow].Font.Bold = true; // Bold the Status cell of this row

                // See if the row is in revision by staff

                //label = (Label)e.Row.FindControl("lblReviseUserRole");  // Find the label control that contains ReviseUserRole in this row
                //UserRole reviseRole = EnumActions.ConvertTextToUserRole(label.Text); // Carefully convert back into enumeration type
                //if (reviseRole != UserRole.None)                        // If != row is in revision
                //{
                //    ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Fetch Project Role 
                //    if (RoleActions.ProjectRoleIsStaff(projectRole))    // If true user is staff
                //        e.Row.ForeColor = PortalConstants.DashboardRevising; // How about something in a nice teal?
                //}

                // See if the row is Rush

                label = (Label)e.Row.FindControl("lblRush");            // Find the label control that contains Rush in this row
                if (label.Text == "True")                               // If == this record is Rush
                    e.Row.ForeColor = PortalConstants.DashboardRush;    // Use color to indicate Rush status
            }
            else if (e.Row.RowType == DataControlRowType.Header)        // If == this is a header row
            {
                ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Fetch Project Role 
                if (RoleActions.ProjectRoleIsStaff(projectRole))        // If true user is staff
                    e.Row.Cells[rowProjectExpView.TimeRow].Text = "Date Created"; // They see time row was created
            }
            return;
        }

        protected void gvAllExp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)            // If == this is indeed a row of our GridView control
            {
                Common_RowDataBound(sender, e);                         // Do the standard hookups

                // See if user is the next to act on this row

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                ExpState state = EnumActions.ConvertTextToExpState(label.Text); // Carefully convert back into enumeration type
                if (StateActions.ProjectRoleToProcessRequest(state) == EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text)) // If == user can operate on Request
                    e.Row.Cells[rowProjectExpView.CurrentStateDescRow].Font.Bold = true; // Bold Status cell.

                // See if the row is in revision by staff

                //label = (Label)e.Row.FindControl("lblReviseUserRole");  // Find the label control that contains ReviseUserRole in this row
                //UserRole reviseRole = EnumActions.ConvertTextToUserRole(label.Text); // Carefully convert back into enumeration type
                //if (reviseRole != UserRole.None)                        // If != row is in revision
                //{
                //    ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Fetch Project Role 
                //    if (RoleActions.ProjectRoleIsStaff(projectRole))    // If true user is staff
                //        e.Row.ForeColor = PortalConstants.DashboardRevising; // How about something in a nice teal?
                //}

                // See if the row is Rush

                label = (Label)e.Row.FindControl("lblRush");            // Find the label control that contains Rush in this row
                if (label.Text == "True")                               // If == this record is Rush
                    e.Row.ForeColor = PortalConstants.DashboardRush;    // Use color to indicate Rush status
            }
            else if (e.Row.RowType == DataControlRowType.Header)        // If == this is a header row
            {
                ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Fetch Project Role 
                if (RoleActions.ProjectRoleIsStaff(projectRole))        // If true user is staff
                    e.Row.Cells[rowProjectExpView.TimeRow].Text = "Date Created"; // They see time row was created
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

        //protected void gvAllApp_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    Label label = (Label)gvAllApp.SelectedRow.FindControl("lblCurrentState"); // Find the label control that contains Current State
        //    AppState state = EnumActions.ConvertTextToAppState(label.Text); // Convert back into enumeration type
        //    btnAppArchive.Enabled = false;                              // Assume User cannot Archive Request
        //    btnAppCopy.Enabled = true;                                  // If any Request is selected, the user can always copy it
        //    btnAppDelete.Enabled = false;                               // Assume User cannot Delete Request
        //    btnAppEdit.Enabled = false;                                 // Assume User cannot Edit Request
        //    btnAppReview.Enabled = false;                               // Assume User cannot Review Request
        //    btnAppView.Enabled = true;                                  // If any Request is selected, the user can always view it

        //    switch (state)
        //    {
        //        case AppState.UnsubmittedByInternalCoordinator:
        //            {
        //                if (litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString()) // If == the user is a coordinator
        //                {
        //                    btnAppDelete.Enabled = true;
        //                    btnAppEdit.Enabled = true;
        //                }
        //                break;
        //            }
        //        case AppState.UnsubmittedByProjectDirector:
        //            {
        //                if (litSavedProjectRole.Text == ProjectRole.ProjectDirector.ToString()) // If == the user is a PD
        //                {
        //                    btnAppDelete.Enabled = true;
        //                    btnAppEdit.Enabled = true;
        //                }
        //                break;
        //            }
        //        case AppState.UnsubmittedByProjectStaff:
        //            {
        //                if (litSavedProjectRole.Text == ProjectRole.ProjectStaff.ToString()) // If == the user is a PS
        //                {
        //                    btnAppDelete.Enabled = true;
        //                    btnAppEdit.Enabled = true;
        //                }
        //                break;
        //            }
        //        case AppState.AwaitingProjectDirector:
        //            {
        //                if (litSavedProjectRole.Text == ProjectRole.ProjectDirector.ToString()) // If == user is a Project Director, can Review
        //                    btnAppReview.Enabled = true;
        //                break;
        //            }
        //        case AppState.AwaitingInternalCoordinator:
        //            {
        //                if (litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString()) // If == the user is a coordinator
        //                {
        //                    btnAppReview.Enabled = true;
        //                }
        //                break;
        //            }
        //        case AppState.Approved:                             // Maybe Returned too?
        //            {
        //                if ((litSavedProjectRole.Text == ProjectRole.InternalCoordinator.ToString())
        //                    || litSavedProjectRole.Text == ProjectRole.ProjectDirector.ToString()) // If == the user can Archive
        //                {
        //                    Label archivedLabel = (Label)gvAllApp.SelectedRow.FindControl("lblArchived"); // Find the label control that contains Archived
        //                    if (archivedLabel.Text == "False")      // If == not currently archived.
        //                        btnAppArchive.Enabled = true;       // Light Archive button. Can't archive if already archived
        //                }
        //                break;
        //            }
        //        case AppState.AwaitingCommunityDirector:
        //        case AppState.AwaitingFinanceDirector:
        //        case AppState.AwaitingPresident:
        //        case AppState.Returned:
        //            {
        //                break;                                      // The button setup is just fine. No editing or deleting here
        //            }
        //        default:
        //            LogError.LogInternalError("ProjectDashboard", string.Format("Invalid AppState '{0}' from database",
        //                state)); // Fatal error
        //            break;
        //    }
        //    return;
        //}

        protected void gvAllDep_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDepArchive.Enabled = false;
            btnDepCopy.Enabled = false;                                 // Assume a powerless user
            btnDepDelete.Enabled = false;                               // and turn off all buttons
            btnDepEdit.Enabled = false;
            // Leave the New button in its former state
            btnDepReview.Enabled = false;
            btnDepView.Enabled = true;                                  // Everybody can View any Request

            // Build up some context for the next set of decisions

            DepState state = EnumActions.ConvertTextToDepState(((Label)gvAllDep.SelectedRow.FindControl("lblCurrentState")).Text); // Carefully find state of row
            bool archived = EnumActions.ConvertTextToBool(((Label)gvAllDep.SelectedRow.FindControl("lblArchived")).Text); // Carefully find Archived flag of row
            ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Carefully find Project Role of user

            switch(projectRole)
            {
                case ProjectRole.InternalCoordinator:
                    {
                        btnDepCopy.Enabled = true;                      // No matter what state, IC can copy the request
                        switch (state)
                        {
                            case DepState.UnsubmittedByInternalCoordinator:
                                {
                                    btnDepDelete.Enabled = true;        // We created it, we can delete it
                                    btnDepEdit.Enabled = true;          // In mid-revision, IC can continue editing
                                    break;
                                }
                            case DepState.Returned:
                                {
                                    btnDepReview.Enabled = true;        // IC can Review a Returned request
                                    break;
                                }
                            case DepState.DepositComplete:
                                {
                                    if (!archived)                      // If false row not archived (yet)
                                        btnDepArchive.Enabled = true;   // Allow it to be archived
                                    break;
                                }
                            default:
                                break;                                  // IC is powerless for all other states
                        }
                        break;
                    }
                case ProjectRole.ProjectDirector:
                    {
                        switch (state)
                        {
                            case DepState.AwaitingProjectDirector:      // PD can review a submitted request
                            case DepState.RevisedByFinanceDirector:     // PD can review request that FD has revised
                                {
                                    btnDepReview.Enabled = true;
                                    break;
                                }
                            case DepState.RevisingByProjectDirector:
                                {
                                    btnDepEdit.Enabled = true;          // In mid-revision, PD can continue editing
                                    break;
                                }
                            case DepState.DepositComplete:
                                {
                                    if (!archived)                      // If false row not archived (yet)
                                        btnDepArchive.Enabled = true;   // Allow it to be archived
                                    break;
                                }
                            default:
                                break;                                  // PD is powerless for all other states
                        }
                        break;
                    }
                case ProjectRole.RevisingFinanceDirector:
                    {
                        //** I don't think we can get here. FD has no access to Project Dashboard **
                        switch (state)
                        {
                            case DepState.RevisingByFinanceDirector:
                                {
                                    btnDepEdit.Enabled = true;          // In mid-revision, FD can continue editing
                                    break;
                                }
                            default:
                                break;                                  // FD is powerless for all other states
                        }
                        break;
                    }
                default:
                    break;                                              // Others are powerless - all buttons off
            }
            return;
        }

        protected void gvAllDoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDocArchive.Enabled = false;
            btnDocCopy.Enabled = true;                                  // Any request can be copied
            btnDocDelete.Enabled = false;                               // and turn off all buttons
            btnDocEdit.Enabled = false;
            // Leave the New button in its former state
            btnDocReview.Enabled = false;
            btnDocView.Enabled = true;                                  // Everybody can View any Request

            // Build up some context for the next set of decisions

            DocState state = EnumActions.ConvertTextToDocState(((Label)gvAllDoc.SelectedRow.FindControl("lblCurrentState")).Text); // Carefully find state of row
            bool archived = EnumActions.ConvertTextToBool(((Label)gvAllDoc.SelectedRow.FindControl("lblArchived")).Text); // Carefully find Archived flag of row
            ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Carefully find Project Role of user

            switch (projectRole)
            {
                case ProjectRole.InternalCoordinator:
                    {
                        switch (state)
                        {
                            case DocState.AwaitingInternalCoordinator:
                            case DocState.ReturnedToInternalCoordinator:
                                {
                                    btnDocReview.Enabled = true;        // IC can Review a Returned request
                                    break;
                                }
                            case DocState.UnsubmittedByInternalCoordinator:
                                {
                                    btnDocDelete.Enabled = true;        // IC can Delete own request
                                    btnDocEdit.Enabled = true;          // In mid-creation, IC can continue editing
                                    break;
                                }
                            case DocState.RevisingByInternalCoordinator:
                                {
                                    btnDocEdit.Enabled = true;          // In mid-revision, IC can continue editing
                                    break;
                                }
                            case DocState.Executed:
                                {
                                    if (!archived)                      // If false row not archived (yet)
                                        btnDocArchive.Enabled = true;   // Allow it to be archived
                                    break;
                                }
                            default:
                                break;                                  // IC is powerless for all other states
                        }
                        break;
                    }
                case ProjectRole.ProjectStaff:
                    {
                        switch (state)
                        {
                            case DocState.ReturnedToProjectStaff:
                                {
                                    btnDocReview.Enabled = true;        // PS can Review a Returned request
                                    break;
                                }
                            case DocState.UnsubmittedByProjectStaff:
                                {
                                    btnDocDelete.Enabled = true;        // PS can Delete own request
                                    btnDocEdit.Enabled = true;          // In mid-revision, PS can continue editing
                                    break;
                                }
                            default:
                                break;                                  // PS is powerless for all other states
                        }
                        break;
                    }
                case ProjectRole.ProjectDirector:
                    {
                        switch (state)
                        {
                            case DocState.ReturnedToProjectDirector:    // PD can review a returned request
                            case DocState.AwaitingProjectDirector:      // PD can review a submitted request
                            case DocState.RevisedByFinanceDirector:
                            case DocState.RevisedByCommunityDirector:   // PD can review a revised request
                            case DocState.RevisedByInternalCoordinator:
                            case DocState.RevisedByPresident:
                                {
                                    btnDocReview.Enabled = true;
                                    break;
                                }
                            case DocState.UnsubmittedByProjectDirector: // PD can edit an unsubmitted request
                                {
                                    btnDocDelete.Enabled = true;        // PD can deelte own request
                                    btnDocEdit.Enabled = true;          // PD can edit own request
                                    break;
                                }
                            case DocState.RevisingByProjectDirector:    // PD can edit a revising request
                                {
                                    btnDocEdit.Enabled = true;          // PD can edit own request
                                    break;
                                }
                            case DocState.Executed:                     // PD can archive a completed request
                                {
                                    if (!archived)                      // If false row not archived (yet)
                                        btnDocArchive.Enabled = true;   // Allow it to be archived
                                    break;
                                }
                            default:
                                break;                                  // PD is powerless for all other states
                        }
                        break;
                    }
                default:
                    break;                                              // Others are powerless - all buttons off
            }
            return;
        }

        protected void gvAllExp_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnExpArchive.Enabled = false;
            btnExpCopy.Enabled = true;                                  // If any Request is selected, the project user can always copy it
            btnExpDelete.Enabled = false;                               // Assume User cannot Delete Request
            btnExpEdit.Enabled = false;                                 // Assume User cannot Edit Request
            btnExpReview.Enabled = false;                               // Assume User cannot Review Request
            btnExpView.Enabled = true;                                  // If any Request is selected, the user can always view it


            // Build up some context for the next set of decisions

            ExpState state = EnumActions.ConvertTextToExpState(((Label)gvAllExp.SelectedRow.FindControl("lblCurrentState")).Text); // Carefully find state of row
            bool archived = EnumActions.ConvertTextToBool(((Label)gvAllExp.SelectedRow.FindControl("lblArchived")).Text); // Carefully find Archived flag of row
            ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Carefully find Project Role of user

            switch (projectRole)
            {
                case ProjectRole.InternalCoordinator:
                    {
                        switch (state)
                        {
                            case ExpState.AwaitingInternalCoordinator:
                            case ExpState.ReturnedToInternalCoordinator:
                                {
                                    btnExpReview.Enabled = true;        // IC can Review a Returned request
                                    break;
                                }
                            case ExpState.UnsubmittedByInternalCoordinator:
                                {
                                    btnExpDelete.Enabled = true;        // IC can Delete own request
                                    btnExpEdit.Enabled = true;          // In mid-revision, IC can continue editing
                                    break;
                                }
                            case ExpState.Paid:
                                {
                                    if (!archived)                      // If false row not archived (yet)
                                        btnExpArchive.Enabled = true;   // Allow it to be archived
                                    break;
                                }
                            case ExpState.RevisingByInternalCoordinator: // IC can edit a revising request
                                {
                                    btnExpEdit.Enabled = true;          // PD can edit own request
                                    break;
                                }
                            default:
                                break;                                  // IC is powerless for all other states
                        }
                        break;
                    }
                case ProjectRole.ProjectStaff:
                    {
                        switch (state)
                        {
                            case ExpState.ReturnedToProjectStaff:
                                {
                                    btnExpReview.Enabled = true;        // PS can Review a Returned request
                                    break;
                                }
                            case ExpState.UnsubmittedByProjectStaff:
                                {
                                    btnExpDelete.Enabled = true;        // PS can Delete own request
                                    btnExpEdit.Enabled = true;          // In mid-revision, PS can continue editing
                                    break;
                                }
                            default:
                                break;                                  // PS is powerless for all other states
                        }
                        break;
                    }
                case ProjectRole.ProjectDirector:
                    {
                        switch (state)
                        {
                            case ExpState.ReturnedToProjectDirector:    // PD can review a returned request
                            case ExpState.AwaitingProjectDirector:      // PD can review a submitted request
                            case ExpState.RevisedByCommunityDirector:   // PD can review a revised request
                            case ExpState.RevisedByFinanceDirector:
                            case ExpState.RevisedByInternalCoordinator:
                            case ExpState.RevisedByPresident:
                                {
                                    btnExpReview.Enabled = true;
                                    break;
                                }
                            case ExpState.UnsubmittedByProjectDirector: // PD can edit an unsubmitted request
                                {
                                    btnExpDelete.Enabled = true;        // PD can deelte own request
                                    btnExpEdit.Enabled = true;          // PD can edit own request
                                    break;
                                }
                            case ExpState.RevisingByProjectDirector:    // PD can edit a revising request
                                {
                                    btnExpEdit.Enabled = true;          // PD can edit own request
                                    break;
                                }
                            case ExpState.Paid:                         // PD can archive a completed request
                                {
                                    if (!archived)                      // If false row not archived (yet)
                                        btnExpArchive.Enabled = true;   // Allow it to be archived
                                    break;
                                }
                            default:
                                break;                                  // PD is powerless for all other states
                        }
                        break;
                    }
                default:
                    break;                                              // Others are powerless - all buttons off
            }
            return;
        }

        // Flip a page of the grid view control

        //protected void gvAllApp_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
        //    {
        //        LoadAllApps(e.NewPageIndex);                            // Reload the grid view control to the specified page
        //        ResetAppContext();                                      // No selected row, no live buttons after a page flip
        //    }
        //    return;
        //}

        protected void gvAllDep_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                LoadAllDeps(e.NewPageIndex);                            // Reload the grid view control to the specified page
                ResetDepContext();                                      // No selected row, no live buttons after a page flip
            }
            return;
        }

        protected void gvAllDoc_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                LoadAllDocs(e.NewPageIndex);                            // Reload the grid view control to the specified page
                ResetDocContext();                                      // No selected row, no live buttons after a page flip
            }
            return;
        }

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

        //protected void btnAppArchive_Click(object sender, EventArgs e)
        //{
        //    int appID = QueryStringActions.ConvertID(GetSelectedRowID(gvAllApp)).Int; // Get ready for action with an int version of the App ID

        //    using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
        //    {
        //        try
        //        {
        //            App toUpdate = context.Apps.Find(appID);            // Fetch App row by its key
        //            if (toUpdate == null)
        //                LogError.LogInternalError("ProjectDashboard", $"Unable to locate ApprovalID {appID.ToString()} in database");
        //            // Log fatal error

        //            toUpdate.Archived = true;                           // Mark request as Archived
        //            AppHistory hist = new AppHistory();                 // Get a place to build a new AppHistory row
        //            StateActions.CopyPreviousState(toUpdate, hist, "Archived"); // Create a AppHistory log row from "old" version of Approval
        //            StateActions.SetNewAppState(toUpdate, hist.PriorAppState, litSavedUserID.Text, hist);
        //            // Write down our current State (which doesn't change here) and authorship
        //            context.AppHistorys.Add(hist);                      // Save new AppHistory row
        //            context.SaveChanges();                              // Commit the Add or Modify
        //        }
        //        catch (Exception ex)
        //        {
        //            LogError.LogDatabaseError(ex, "ProjectDashboard",
        //                "Error updating App and AppHistory rows");      // Fatal error
        //        }
        //        SearchCriteriaChanged(sender, e);                          // Update the grid view and buttons for display
        //        litSuccessMessage.Text = "Approval Request successfully Archived";    // Let user know we're good
        //    }
        //}

        //// Copy button pressed. Simple dispatch to EditApproval.

        //protected void btnAppCopy_Click(object sender, EventArgs e)
        //{
        //    string appID = GetSelectedRowID(gvAllApp);               // Extract the text of the control, which is AppID
        //    Response.Redirect(PortalConstants.URLEditApproval + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
        //                                    + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
        //                                    + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
        //                                    + PortalConstants.QSRequestID + "=" + appID + "&"
        //                                    + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandCopy + "&" // Start with existing request
        //                                    + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done
        //}

        //// Delete button pressed. We do this here.

        //protected void btnAppDelete_Click(object sender, EventArgs e)
        //{
        //    int appID = QueryStringActions.ConvertID(GetSelectedRowID(gvAllApp)).Int; // Get ready for action with an int version of the Exp ID

        //    using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
        //    {
        //        try
        //        {

        //            //  1) Blow off the Supporting Docs associated with the Request. This means deleting the files and SupportingDoc rows.

        //            SupportingActions.DeleteDocs(RequestType.Approval, appID);                // Great idea. Do that!

        //            //  2) Delete the AppHistory rows associated with the Request

        //            context.AppHistorys.RemoveRange(context.AppHistorys.Where(x => x.AppID == appID)); // Delete all of them

        //            //  3) Kill off the Request itself. Do this last so that if something earlier breaks, we can recover - by deleting again.

        //            App app = new App { AppID = appID };                // Instantiate an Exp object with the selected row's ID
        //            context.Apps.Attach(app);                           // Find that record, but don't fetch it
        //            context.Apps.Remove(app);                           // Delete that row
        //            context.SaveChanges();                              // Commit the deletions
        //        }
        //        catch (Exception ex)
        //        {
        //            LogError.LogDatabaseError(ex, "ProjectDashboard",
        //                "Error deleting App, SupportingDoc and AppHistory rows or deleting Supporting Document"); // Fatal error
        //        }
        //        SearchCriteriaChanged(sender, e);                          // Update the grid view and buttons for display
        //        litSuccessMessage.Text = "Approval deleted";            // Report success to our user
        //    }
        //}

        //// Edit button clicked. Fetch the ID of the selected row and dispatch to EditApproval.

        //protected void btnAppEdit_Click(object sender, EventArgs e)
        //{
        //    string appID = GetSelectedRowID(gvAllApp);               // Extract the text of the control, which is AppID
        //    Response.Redirect(PortalConstants.URLEditApproval + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
        //                                    + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
        //                                    + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
        //                                    + PortalConstants.QSRequestID + "=" + appID + "&"
        //                                    + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit + "&" // Start with an existing request
        //                                      + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done
        //}

        //protected void btnAppNew_Click(object sender, EventArgs e)
        //{

        //    // Propagage the UserID and ProjectID that we were called with. No AppID means a new Request.

        //    Response.Redirect(PortalConstants.URLEditDocument + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
        //                                    + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
        //                                    + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
        //                                    + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew + "&" // Start with an empty request
        //                                    + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done
        //}

        //protected void btnAppReview_Click(object sender, EventArgs e)
        //{
        //    string appID = GetSelectedRowID(gvAllApp);               // Extract the text of the control, which is DepID

        //    // Unconditionally send Request to ReviewRequest. It is possible that the user does not have the authority to review the Request in
        //    // its current state. But we'll let ReviewRequest display all the detail for the Dep and then deny editing.

        //    Response.Redirect(PortalConstants.URLReviewApproval + "?" + PortalConstants.QSRequestID + "=" + appID + "&" // Start with an existing request
        //                                    + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview + "&" // Review it
        //                                    + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done
        //}

        //protected void btnAppView_Click(object sender, EventArgs e)
        //{
        //    string appID = GetSelectedRowID(gvAllApp);               // Extract the text of the control, which is AppID
        //    Response.Redirect(PortalConstants.URLEditApproval + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
        //                                    + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
        //                                    + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
        //                                    + PortalConstants.QSRequestID + "=" + appID + "&"
        //                                    + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandView + "&" // Start with existing request
        //                                    + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done
        //}

        // Start of DEPOSIT section

        // Archive button pressed. Just flip on the Archived flag

        protected void btnDepArchive_Click(object sender, EventArgs e)
        {
            int depID = QueryStringActions.ConvertID(GetSelectedRowID(gvAllDep)).Int; // Get ready for action with an int version of the Dep ID

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    Dep toArchive = context.Deps.Find(depID);            // Fetch Dep row by its key
                    if (toArchive == null)
                        LogError.LogInternalError("ProjectDashboard", $"Unable to locate DepositID {depID.ToString()} in database");
                    // Log fatal error

                    toArchive.Archived = true;                          // Mark request as Archived
                    DepHistory hist = new DepHistory();                 // Get a place to build a new DepHistory row
                    StateActions.CopyPreviousState(toArchive, hist, "Archived"); // Create a DepHistory log row from "old" version of Deposit
                    StateActions.SetNewState(toArchive, hist.PriorDepState, litSavedUserID.Text, hist);
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
            string depID = GetSelectedRowID(gvAllDep);               // Extract the text of the control, which is ExpID
            Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + depID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandCopy + "&" // Start with existing request
                                            + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard);
        }

        // Delete button pushed. Clean up all the parts of the Deposit Request including supporting documents and history.

        protected void btnDepDelete_Click(object sender, EventArgs e)
        {
            int depID = QueryStringActions.ConvertID(GetSelectedRowID(gvAllDep)).Int; // Extract the text of the control, which is DepID
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  0) Delete the GLCodeSplit rows associated with this Dep.

                    SplitActions.DeleteSplitRows(RequestType.Deposit, depID); // Delete all GLCodeSplit rows with this DepID

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
            string depID = GetSelectedRowID(gvAllDep);               // Extract the text of the control, which is DepID
            DepState state = EnumActions.ConvertTextToDepState(((Label)gvAllDep.SelectedRow.FindControl("lblCurrentState")).Text); // Carefully find state of row
            string cmd = PortalConstants.QSCommandEdit;             // Assume the request is editing, not revising
            if (StateActions.RequestIsRevising(state))              // If true, request is in the midst of a revision
                cmd = PortalConstants.QSCommandRevise;              // Tell the Edit page that we are revising

            Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + depID + "&"
                                            + PortalConstants.QSCommand + "=" + cmd + "&" // Edit or Revise
                                            + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard);
        }

        protected void btnDepNew_Click(object sender, EventArgs e)
        {

            // Propagage the UserID and ProjectID that we were called with. No DepID means a new Request.

            Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew + "&" // Start with an empty request
                                            + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard);
        }

        // Review Request button clicked. One of two situations:
        //  1) The IC is revising a Returned request. This triggers a Revise function, via a dispatch to Edit Deposit. (Unfortunate name overload of "Review".)
        //  2) The PD is reviewing an Awaiting request. This triggers a Review function. Dispatch to Review Deposit.

        protected void btnDepReview_Click(object sender, EventArgs e)
        {
            string ID = GetSelectedRowID(gvAllDep);                         // Extract the text of the rowID control, which is DepID
            DepState currState = EnumActions.ConvertTextToDepState(GetSelectedRowCurrentState(gvAllDep)); // Extract and convert Current State of request

            if (StateActions.RequestIsReturned(currState))                  // If true dispatch to EditDeposit to revise the request
            {
                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    try
                    {
                        int depID = QueryStringActions.ConvertID(ID).Int;   // Convert to Deposit ID
                        Dep toRevise = context.Deps.Find(depID);            // Fetch Dep row by its key
                        if (toRevise == null)
                            LogError.LogInternalError("ProjectDashboard", $"Unable to locate DepositID {depID.ToString()} in database"); // Log fatal error

                        DepHistory hist = new DepHistory();                 // Get a place to build a new DepHistory row
                        StateActions.CopyPreviousState(toRevise, hist, "Reviewed"); // Create a DepHistory log row from "old" version of Deposit
                        StateActions.SetNewState(toRevise, StateActions.FindNextState(toRevise.CurrentState, ReviewAction.Revise), litSavedUserID.Text, hist);
                        // Write down our new state and authorship
                        context.DepHistorys.Add(hist);                      // Save new DepHistory row
                        context.SaveChanges();                              // Commit the Add or Modify
                    }
                    catch (Exception ex)
                    {
                        LogError.LogDatabaseError(ex, "ProjectDashboard",
                            "Error updating Dep and DepHistory rows");      // Fatal error
                    }
                }
                Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text
                             + "&" + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text
                             + "&" + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text
                             + "&" + PortalConstants.QSRequestID + "=" + ID // Use selected row's ID
                             + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandRevise // Revise the returned request
                             + "&" + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done

            }
            else                                                    // Otherwise dispatch to ReviewDeposit to review the request
            {

            // It is possible that the user does not have the authority to review the Dep in
            // its current state. But we'll let ReviewRequest display all the detail for the Dep and then deny editing.

            Response.Redirect(PortalConstants.URLReviewDeposit + "?" + PortalConstants.QSRequestID + "=" + ID  // Start with an existing request
                            + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview // Review it
                            + "&" + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done
            }
        }

        // View button clicked.
        protected void btnDepView_Click(object sender, EventArgs e)
        {
            string depID = GetSelectedRowID(gvAllDep);               // Extract the text of the control, which is DepID
            Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + depID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandView + "&" // Start with existing request
                                            + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard);
        }

        // DOCUMENT functions

        // Archive button pressed. Just flip on the Archived flag

        protected void btnDocArchive_Click(object sender, EventArgs e)
        {
            int docID = QueryStringActions.ConvertID(GetSelectedRowID(gvAllDoc)).Int; // Get ready for action with an int version of the Doc ID

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    Doc toArchive = context.Docs.Find(docID);            // Fetch Doc row by its key
                    if (toArchive == null)
                        LogError.LogInternalError("ProjectDashboard", $"Unable to locate DocumentID {docID.ToString()} in database");
                    // Log fatal error

                    toArchive.Archived = true;                          // Mark request as Archived
                    DocHistory hist = new DocHistory();                 // Get a place to build a new DocHistory row
                    StateActions.CopyPreviousState(toArchive, hist, "Archived"); // Create a DocHistory log row from "old" version of Docosit
                    StateActions.SetNewState(toArchive, hist.PriorDocState, litSavedUserID.Text, hist);
                    // Write down our current State (which doesn't change here) and authorship
                    context.DocHistorys.Add(hist);                      // Save new DocHistory row
                    context.SaveChanges();                              // Commit the Add or Modify
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ProjectDashboard",
                        "Error updating Doc and DocHistory rows");      // Fatal error
                }
                SearchCriteriaChanged(sender, e);                          // Update the grid view and buttons for display
                litSuccessMessage.Text = "Document Request successfully Archived";    // Let user know we're good
            }
        }

        // Copy button pushed. Just dispatch

        protected void btnDocCopy_Click(object sender, EventArgs e)
        {
            string docID = GetSelectedRowID(gvAllDoc);               // Extract the text of the control, which is ExpID
            Response.Redirect(PortalConstants.URLEditDocument + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + docID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandCopy + "&" // Start with existing request
                                            + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard);
        }

        // Delete button pushed. Clean up all the parts of the Document Request including supporting documents and history.

        protected void btnDocDelete_Click(object sender, EventArgs e)
        {
            int docID = QueryStringActions.ConvertID(GetSelectedRowID(gvAllDoc)).Int; // Extract the text of the control, which is DocID
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  1) Blow off the Supporting Docs associated with the Doc. This means deleting the files and SupportingDoc rows.

                    SupportingActions.DeleteDocs(RequestType.Document, docID); // Great idea. Do that!

                    //  2) Delete the DocHistory rows associated with the Doc

                    context.DocHistorys.RemoveRange(context.DocHistorys.Where(x => x.DocID == docID)); // Delete all of them

                    //  3) Kill off the Doc itself. Do this last so that if something earlier breaks, we can recover - by deleting again.

                    Doc Doc = new Doc { DocID = docID };                // Instantiate an Doc object with the selected row's ID
                    context.Docs.Attach(Doc);                           // Find that record, but don't fetch it
                    context.Docs.Remove(Doc);                           // Delete that row
                    context.SaveChanges();                              // Commit the deletions
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ProjectDashboard",
                        "Error deleting Document Request, SupportingDoc and DocHistory rows or deleting Supporting Document"); // Fatal error
                }
                SearchCriteriaChanged(sender, e);                        // Update the grid view and buttons for display
                litSuccessMessage.Text = "Document deleted";             // Report success to our user
            }
            return;
        }

        // Edit button clicked. Fetch the DocID of the selected row and dispatch to EditDocument.

        protected void btnDocEdit_Click(object sender, EventArgs e)
        {
            string docID = GetSelectedRowID(gvAllDoc);               // Extract the text of the control, which is DocID
            DocState state = EnumActions.ConvertTextToDocState(((Label)gvAllDoc.SelectedRow.FindControl("lblCurrentState")).Text); // Carefully find state of row
            string cmd = PortalConstants.QSCommandEdit;             // Assume the request is editing, not revising
            if (StateActions.RequestIsRevising(state))              // If true, request is in the midst of a revision
                cmd = PortalConstants.QSCommandRevise;              // Tell the Edit page that we are revising

            Response.Redirect(PortalConstants.URLEditDocument + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + docID + "&"
                                            + PortalConstants.QSCommand + "=" + cmd + "&" // Start with an existing request and Revise or Edit
                                            + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard);
        }

        protected void btnDocNew_Click(object sender, EventArgs e)
        {

            // Propagage the UserID and ProjectID that we were called with. No DocID means a new Request.

            Response.Redirect(PortalConstants.URLEditDocument + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew + "&" // Start with an empty request
                                            + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard);
        }

        // Review Request button clicked. One of two situations:
        //  1) The PD/PS/IC is revising a Returned request. This triggers a Revise function, via a dispatch to Edit Document. (Unfortunate name overload of "Review".)
        //  2) The PD is reviewing an Awaiting request. This triggers a Review function. Dispatch to Review Document.

        protected void btnDocReview_Click(object sender, EventArgs e)
        {
            string ID = GetSelectedRowID(gvAllDoc);                         // Extract the text of the rowID control, which is DocID
            DocState currState = EnumActions.ConvertTextToDocState(GetSelectedRowCurrentState(gvAllDoc)); // Extract and convert Current State of request

            if (StateActions.RequestIsReturned(currState))                  // If true dispatch to EditDocument to revise the request
            {
                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    try
                    {
                        int DocID = QueryStringActions.ConvertID(ID).Int;   // Convert to Docosit ID
                        Doc toRevise = context.Docs.Find(DocID);            // Fetch Doc row by its key
                        if (toRevise == null)
                            LogError.LogInternalError("ProjectDashboard", $"Unable to locate DocumentID {DocID.ToString()} in database"); // Log fatal error

                        DocHistory hist = new DocHistory();                 // Get a place to build a new DocHistory row
                        StateActions.CopyPreviousState(toRevise, hist, "Reviewed"); // Create a DocHistory log row from "old" version of Docosit
                        StateActions.SetNewState(toRevise, StateActions.FindNextState(toRevise.CurrentState, ReviewAction.Revise, toRevise.DocType),
                            litSavedUserID.Text, hist);
                        // Write down our new state and authorship
                        context.DocHistorys.Add(hist);                      // Save new DocHistory row
                        context.SaveChanges();                              // Commit the Add or Modify
                    }
                    catch (Exception ex)
                    {
                        LogError.LogDatabaseError(ex, "ProjectDashboard",
                            "Error updating Doc and DocHistory rows");      // Fatal error
                    }
                }
                Response.Redirect(PortalConstants.URLEditDocument + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text
                             + "&" + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text
                             + "&" + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text
                             + "&" + PortalConstants.QSRequestID + "=" + ID // Use selected row's ID
                             + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandRevise // Revise the returned request
                             + "&" + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done

            }
            else                                                    // Otherwise dispatch to ReviewDocument to review the request
            {

                // It is possible that the user does not have the authority to review the Doc in
                // its current state. But we'll let ReviewRequest display all the detail for the Doc and then deny editing.

                Response.Redirect(PortalConstants.URLReviewDocument + "?" + PortalConstants.QSRequestID + "=" + ID  // Start with an existing request
                                + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview // Review it
                                + "&" + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done
            }
        }

        // View button clicked.
        protected void btnDocView_Click(object sender, EventArgs e)
        {
            string docID = GetSelectedRowID(gvAllDoc);               // Extract the text of the control, which is DocID
            Response.Redirect(PortalConstants.URLEditDocument + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + docID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandView + "&" // Start with existing request
                                            + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard);
        }

        // EXPENSE functions

        // Archive button pressed. Just flip on the Archived flag

        protected void btnExpArchive_Click(object sender, EventArgs e)
        {
            int expID = QueryStringActions.ConvertID(GetSelectedRowID(gvAllExp)).Int; // Get ready for action with an int version of the Exp ID

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
                    StateActions.SetNewState(toUpdate, hist.PriorExpState, litSavedUserID.Text, hist);
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
            string expID = GetSelectedRowID(gvAllExp);               // Extract the text of the control, which is ExpID
            Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + expID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandCopy + "&" // Start with existing request
                                            + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page
        }

        // Delete button clicked. Wipe out the Request and it's associated rows and files.

        protected void btnExpDelete_Click(object sender, EventArgs e)
        {
            int expID = QueryStringActions.ConvertID(GetSelectedRowID(gvAllExp)).Int; // Get ready for action with an int version of the Exp ID

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  0) Delete the GLCodeSplit rows associated with this Exp.

                    SplitActions.DeleteSplitRows(RequestType.Expense, expID); // Delete all GLCodeSplit rows with this ExpID

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
            string expID = GetSelectedRowID(gvAllExp);               // Extract the text of the control, which is ExpID
            ExpState state = EnumActions.ConvertTextToExpState(((Label)gvAllExp.SelectedRow.FindControl("lblCurrentState")).Text); // Carefully find state of row
            string cmd = PortalConstants.QSCommandEdit;             // Assume the request is editing, not revising
            if (StateActions.RequestIsRevising(state))              // If true, request is in the midst of a revision
                cmd = PortalConstants.QSCommandRevise;              // Tell the Edit page that we are revising

            Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + expID + "&"
                                            + PortalConstants.QSCommand + "=" + cmd + "&" // Edit or Revise
                                            + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page
        }

        // New Request button has been clicked. Dispatch to Edit Detail

        protected void btnExpNew_Click(object sender, EventArgs e)
        {

            // Propagage the UserID and ProjectID that we were called with. No ExpID means a new Request.
            
            Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew + "&" // Start with an empty request
                                            + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page
        }

        // Review Request button clicked. One of two situations:
        //  1) The originator (IC/PD/PS) is revising a Returned request. This triggers a Revise function, via a dispatch to Edit Expense. (Unfortunate name overload of "Review".)
        //  2) The PD is reviewing an Awaiting request. This triggers a Review function. Dispatch to Review Expense.

        protected void btnExpReview_Click(object sender, EventArgs e)
        {
            string ID = GetSelectedRowID(gvAllExp);                         // Extract the text of the rowID control, which is ExpID
            ExpState currState = EnumActions.ConvertTextToExpState(GetSelectedRowCurrentState(gvAllExp)); // Extract and convert Current State of request

            if (StateActions.RequestIsReturned(currState))            // If true dispatch to EditExpense to revise the request
            {
                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    try
                    {
                        int expID = QueryStringActions.ConvertID(ID).Int;   // Convert to Expense ID
                        Exp toRevise = context.Exps.Find(expID);            // Fetch Exp row by its key
                        if (toRevise == null)
                            LogError.LogInternalError("ProjectDashboard", $"Unable to locate ExpenseID {expID.ToString()} in database"); // Log fatal error

                        ExpHistory hist = new ExpHistory();                 // Get a place to build a new ExpHistory row
                        StateActions.CopyPreviousState(toRevise, hist, "Reviewed"); // Create a ExpHistory log row from "old" version of Expense
                        StateActions.SetNewState(toRevise, StateActions.FindNextState(toRevise.CurrentState, ReviewAction.Revise), litSavedUserID.Text, hist);
                        // Write down our new state and authorship
                        context.ExpHistorys.Add(hist);                      // Save new ExpHistory row
                        context.SaveChanges();                              // Commit the Add or Modify
                    }
                    catch (Exception ex)
                    {
                        LogError.LogDatabaseError(ex, "ProjectDashboard", "Error updating Exp and ExpHistory rows");      // Fatal error
                    }
                }
                Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text
                             + "&" + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text
                             + "&" + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text
                             + "&" + PortalConstants.QSRequestID + "=" + ID // Use selected row's ID
                             + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandRevise // Revise the returned request
                             + "&" + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done

            }
            else                                                    // Otherwise dispatch to ReviewExpense to review the request
            {

                // It is possible that the user does not have the authority to review the Exp in
                // its current state. But we'll let ReviewRequest display all the detail for the Exp and then deny editing.

                Response.Redirect(PortalConstants.URLReviewExpense + "?" + PortalConstants.QSRequestID + "=" + ID  // Start with an existing request
                                + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview // Review it
                                + "&" + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done
            }
        }

        // View button clicked. 

        protected void btnExpView_Click(object sender, EventArgs e)
        {
            string expID = GetSelectedRowID(gvAllExp);               // Extract the text of the control, which is ExpID
            Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + litSavedProjectRole.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + expID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandView + "&" // Start with existing request
                                            + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page
        }

        // Fetch all of the Requests for this project, subject to further search constraints. Display in a GridView.
        // Find current project ID in listSavedProjectID

        //void LoadAllApps(int pageIndex = 0)
        //{
        //    if (!pnlApp.Visible)                                        // If ! the panel is hidden, no need to refresh
        //        return;

        //    gvAllApp.PageIndex = pageIndex;                             // Go to the page specified by the caller
        //    int projectID = Convert.ToInt32(litSavedProjectID.Text);    // Fetch ID of current project as an int
        //    using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
        //    {
        //        var pred = PredicateBuilder.True<App>();                // Initialize predicate to select from App table
        //        pred = pred.And(r => r.ProjectID == projectID && !r.Inactive);  // Select requests for this project only and not Inactive

        //        // Process "Active" and "Archived" flags. They're not independent, so we have to grind through their combinations

        //        if (ckRActive.Checked && ckRArchived.Checked)           // If true, take them all
        //            ;                                                   // Don't ignore anything
        //        else if (ckRActive.Checked && !ckRArchived.Checked)     // If true, only Active checked
        //            pred = pred.And(r => !r.Archived);                  // Ignore Archived requests
        //        else if (!ckRActive.Checked && ckRArchived.Checked)     // If true, only Archived checked
        //            pred = pred.And(r => r.Archived);                   // Ignore Active requests
        //        else                                                    // Both boxes are unchecked
        //            pred = pred.And(r => r.Archived && !r.Archived);    // Nonsensical. Returns nothing

        //        // Deal with date range filter. Remember that we've already validated the date strings, so we can just use them.

        //        if (txtBeginningDate.Text != "")
        //        {
        //            DateTime date = Convert.ToDateTime(txtBeginningDate.Text);  // Convert to a date value
        //            pred = pred.And(r => (r.CurrentTime >= date));
        //        }

        //        if (txtEndingDate.Text != "")
        //        {
        //            DateTime date = Convert.ToDateTime(txtEndingDate.Text);     // Convert to a date value
        //            TimeSpan day = new TimeSpan(23, 59, 59);                    // Figure out the length of a day
        //            DateTime endOfDay = date.Add(day);                          // Set query to END of the specified day
        //            pred = pred.And(r => (r.CurrentTime <= endOfDay));          // Query to end END of the specified day
        //        }

        //        List<App> apps = context.Apps.AsExpandable().Where(pred).OrderByDescending(o => o.CurrentTime).ToList();
        //        // Do the query using the constructed predicate, sort the result, and create a list of App rows

        //        // From this list of Apps, build a list of rows for the gvAllApp GridView

        //        List<rowProjectAppView> rows = new List<rowProjectAppView>(); // Create an empty list for the GridView control
        //        foreach (var r in apps)                                 // Fill the list row-by-row
        //        {
        //            bool useRow = false;                                // Assume that we skip this row
        //            switch(r.CurrentState)
        //            {
        //                case AppState.UnsubmittedByInternalCoordinator:
        //                case AppState.UnsubmittedByProjectDirector:
        //                case AppState.UnsubmittedByProjectStaff:
        //                case AppState.AwaitingProjectDirector:
        //                    if (ckRUnsubmitted.Checked)                 // If true, interested in these states
        //                        useRow = true;                          // Process the row, don't skip it
        //                    break;

        //                case AppState.AwaitingFinanceDirector:
        //                case AppState.AwaitingInternalCoordinator:
        //                case AppState.AwaitingCommunityDirector:
        //                case AppState.AwaitingPresident:
        //                    if (ckRAwaitingCWStaff.Checked)             // If true, interested in these states
        //                        useRow = true;                          // Process the row, don't skip it
        //                    break;

        //                case AppState.Approved:
        //                    if (ckRApproved.Checked)                    // If true, interested in these states
        //                        useRow = true;                          // Process the row, don't skip it
        //                    break;

        //                case AppState.Returned:
        //                    if (ckRReturned.Checked)                    // If true, interested in these states
        //                        useRow = true;                          // Process the row, don't skip it
        //                    break;

        //                default:                                        // For all other oddballs, just skip the row
        //                    break;
        //            }

        //            if (useRow)                                         // If true. checkboxes indicate that we should use the row
        //            {
        //                rowProjectAppView row = new rowProjectAppView()     // Empty row all ready to fill
        //                {
        //                    RowID = r.AppID.ToString(),                     // Convert ID from int to string for easier retrieval later
        //                    CurrentTime = r.CurrentTime,                    // When request was last updated
        //                    AppTypeDesc = EnumActions.GetEnumDescription(r.AppType), // Convert enum version to English version for display
        //                    Description = r.Description,                    // Free text description of deposit
        //                    CurrentState = r.CurrentState,                  // Load enum version for use when row is selected. But not visible
        //                    CurrentStateDesc = EnumActions.GetEnumDescription(r.CurrentState), // Convert enum version to English version for display
        //                    Archived = r.Archived                           // Load archived state

        //                };
        //                if (r.Archived)                                     // If true row is Archived
        //                    row.CurrentStateDesc = row.CurrentStateDesc + " (Archived)"; // Append indication that it's archifed

        //                rows.Add(row);                                      // Add the filled-in row to the list of rows
        //            }
        //        }
        //        gvAllApp.DataSource = rows;                           // Give it to the GridView control
        //        gvAllApp.DataBind();                                  // And get it in gear

        //        NavigationActions.EnableGridViewNavButtons(gvAllApp); // Enable appropriate nav buttons based on page count
        //        gvAllApp.SelectedIndex = -1;                          // No selected row any more
        //    }
        //    return;
        //}

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

                // If a specific GLCode is selected, only select requests for that GLCode

                if (ddlGLCode.SelectedIndex > 0)                            // If > GLCode is selected. Fetch rqsts only for that GLCode
                {
                    int id = Convert.ToInt32(ddlGLCode.SelectedValue);      // Convert ID of selected GLCode
                    pred = pred.And(r => r.GLCodeID == id);                 // Only requests from selected GLCode
                }

                // If a specific Person is selected, only select requests from that Person

                if (ddlPersonName.SelectedIndex > 0)                        // If > Person is selected. Fetch rqsts only for that Person
                {
                    int id = Convert.ToInt32(ddlPersonName.SelectedValue);  // Convert ID of selected Person
                    pred = pred.And(r => r.PersonID == id);                 // Only requests from selected Person
                }

                // If a specific ProjectClass is selected, only select requests from that ProjectClass

                if (ddlProjectClass.SelectedIndex > 0)                      // If > ProjectClass is selected. Fetch rqsts only for that ProjectClass
                {
                    int id = Convert.ToInt32(ddlProjectClass.SelectedValue); // Convert ID of selected ProjectClass
                    pred = pred.And(r => r.ProjectClassID == id);           // Only requests from selected ProjectClass
                }

                List<Dep> deps = context.Deps.AsExpandable().Where(pred).OrderByDescending(o => o.CurrentTime).ToList();
                // Do the query using the constructed predicate, sort the result, and create a list of Dep rows

                // From this list of Deps, build a list of rows for the gvAllExp GridView

                ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Fetch user's project role and convert to enum
                List<rowProjectDepView> rows = new List<rowProjectDepView>(); // Create an empty list for the GridView control
                foreach (var r in deps)                                 // Fill the list row-by-row
                {
                    bool useRow = false;                                // Assume that we skip this row
                    switch (r.CurrentState)
                    {
                        case DepState.UnsubmittedByInternalCoordinator:
                        case DepState.AwaitingProjectDirector:
                        case DepState.RevisingByProjectDirector:
                        case DepState.RevisedByFinanceDirector:
                            if (ckRUnsubmitted.Checked)                 // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case DepState.AwaitingFinanceDirector:
                        case DepState.AwaitingCommunityDirector:
                        case DepState.RevisingByFinanceDirector:
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
                        rowProjectDepView row = new rowProjectDepView()     // Empty row all ready to fill
                        {
                            RowID = r.DepID.ToString(),                     // Convert ID from int to string for easier retrieval later
                            Time = r.CurrentTime,                    // When request was last updated
                            DepTypeDesc = EnumActions.GetEnumDescription(r.DepType), // Convert enum version to English version for display
                            Description = r.Description,                    // Free text description of deposit
                                                                            // Source Of Funds is trickier, so done below
                            Amount = ExtensionActions.LoadDecimalIntoTxt(r.Amount), // Carefully load decimal amount into text field
                            CurrentState = r.CurrentState,                  // Load enum version for use when row is selected. But not visible
                            CurrentStateDesc = EnumActions.GetEnumDescription(r.CurrentState), // Convert enum version to English version for display
                            Archived = r.Archived,                          // Load archived state
                            ReviseUserRole = r.ReviseUserRole               // User Role that revised request if any

                        };

                        // Insert the CreatedTime for project members to see, CurrentTime for staff to see

                        if (!RoleActions.ProjectRoleIsStaff(projectRole))   // If false user is a project member.
                            row.Time = r.CurrentTime;                       // Show project member the time of the most recent action on the request
                        else
                            row.Time = r.CreatedTime;                       // Show staff member the time when the request was created

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
                                    LogError.LogInternalError("ProjectDashboard", $"Invalid SourceOfDepFunds value '{r.SourceOfFunds}' found in database"); // Fatal error
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

        void LoadAllDocs(int pageIndex = 0)
        {
            if (!pnlDoc.Visible)                                        // If ! the panel is hidden, no need to refresh
                return;

            gvAllDoc.PageIndex = pageIndex;                             // Go to the page specified by the caller
            int projectID = Convert.ToInt32(litSavedProjectID.Text);    // Fetch ID of current project as an int
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var pred = PredicateBuilder.True<Doc>();                // Initialize predicate to select from Doc table
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
                    pred = pred.And(r => r.EntityID == id);         // Only requests from selected Entity
                }

                // If a specific GLCode is selected, only select requests for that GLCode

                //if (ddlGLCode.SelectedIndex > 0)                            // If > GLCode is selected. Fetch rqsts only for that GLCode
                //{
                //    int id = Convert.ToInt32(ddlGLCode.SelectedValue);      // Convert ID of selected GLCode
                //    pred = pred.And(r => r.GLCodeID == id);                 // Only requests from selected GLCode
                //}

                // If a specific Person is selected, only select requests from that Person

                if (ddlPersonName.SelectedIndex > 0)                        // If > Person is selected. Fetch rqsts only for that Person
                {
                    int id = Convert.ToInt32(ddlPersonName.SelectedValue);  // Convert ID of selected Person
                    pred = pred.And(r => r.PersonID == id);                 // Only requests from selected Person
                }

                // If a specific ProjectClass is selected, only select requests from that ProjectClass

                //if (ddlProjectClass.SelectedIndex > 0)                      // If > ProjectClass is selected. Fetch rqsts only for that ProjectClass
                //{
                //    int id = Convert.ToInt32(ddlProjectClass.SelectedValue); // Convert ID of selected ProjectClass
                //    pred = pred.And(r => r.ProjectClassID == id);           // Only requests from selected ProjectClass
                //}

                List<Doc> Docs = context.Docs.AsExpandable().Where(pred).OrderByDescending(o => o.CurrentTime).ToList();
                // Do the query using the constructed predicate, sort the result, and create a list of Doc rows

                // From this list of Docs, build a list of rows for the gvAllExp GridView

                ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Fetch user's project role and convert to enum
                List<rowProjectDocView> rows = new List<rowProjectDocView>(); // Create an empty list for the GridView control
                foreach (var r in Docs)                                 // Fill the list row-by-row
                {
                    bool useRow = false;                                // Assume that we skip this row
                    switch (r.CurrentState)
                    {
                        case DocState.AwaitingProjectDirector:
                        case DocState.RevisingByProjectDirector:
                        case DocState.RevisedByInternalCoordinator:
                        case DocState.RevisedByFinanceDirector:
                        case DocState.RevisedByCommunityDirector:
                        case DocState.RevisedByPresident:
                        case DocState.UnsubmittedByInternalCoordinator:
                        case DocState.UnsubmittedByProjectDirector:
                        case DocState.UnsubmittedByProjectStaff:
                            if (ckRUnsubmitted.Checked)                 // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case DocState.AwaitingCommunityDirector:
                        case DocState.AwaitingFinanceDirector:
                        case DocState.AwaitingInternalCoordinator:
                        case DocState.AwaitingPresident:
                        case DocState.RevisingByInternalCoordinator:
                        case DocState.RevisingByFinanceDirector:
                        case DocState.RevisingByCommunityDirector:
                        case DocState.RevisingByPresident:
                            if (ckRAwaitingCWStaff.Checked)             // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case DocState.Executed:
                            if (ckRApproved.Checked)                    // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case DocState.ReturnedToInternalCoordinator:
                        case DocState.ReturnedToProjectDirector:
                        case DocState.ReturnedToProjectStaff:
                            if (ckRReturned.Checked)                    // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        default:                                        // For all other oddballs, just skip the row
                            break;
                    }

                    if (useRow)                                         // If true. checkboxes indicate that we should use the row
                    {
                        rowProjectDocView row = new rowProjectDocView()     // Empty row all ready to fill
                        {
                            RowID = r.DocID.ToString(),                     // Convert ID from int to string for easier retrieval later
                            DocTypeDesc = EnumActions.GetEnumDescription(r.DocType), // Convert enum version to English version for display
                            Description = r.Description,                    // Free text description of document
                            CurrentState = r.CurrentState,                  // Load enum version for use when row is selected. But not visible
                            CurrentStateDesc = EnumActions.GetEnumDescription(r.CurrentState), // Convert enum version to English version for display
                            Archived = r.Archived,                          // Load archived state
                            Rush = r.Rush,                                  // Whether the request is Rush
                            ReviseUserRole = r.ReviseUserRole               // User Role that revised request if any
                        };

                        // Insert the CreatedTime for project members to see, CurrentTime for staff to see

                        if (!RoleActions.ProjectRoleIsStaff(projectRole))   // If false user is a project member.
                            row.Time = r.CurrentTime;                       // Show project member the time of the most recent action on the request
                        else
                            row.Time = r.CreatedTime;                       // Show staff member the time when the request was created

                        if (r.Archived)                                     // If true row is Archived
                            row.CurrentStateDesc = row.CurrentStateDesc + " (Archived)"; // Append indication that it's archifed

                        rows.Add(row);                                      // Add the filled-in row to the list of rows
                    }
                }
                gvAllDoc.DataSource = rows;                           // Give it to the GridView control
                gvAllDoc.DataBind();                                  // And get it in gear

                NavigationActions.EnableGridViewNavButtons(gvAllDoc); // Enable appropriate nav buttons based on page count
                gvAllDoc.SelectedIndex = -1;                          // No selected row any more
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

                // If a specific GLCode is selected, only select requests for that GLCode

                if (ddlGLCode.SelectedIndex > 0)                            // If > GLCode is selected. Fetch rqsts only for that GLCode
                {
                    int id = Convert.ToInt32(ddlGLCode.SelectedValue);      // Convert ID of selected GLCode
                    pred = pred.And(r => r.GLCodeID == id);                 // Only requests from selected GLCode
                }

                // If a specific Person is selected, only select requests from that Person

                if (ddlPersonName.SelectedIndex > 0)                        // If > Person is selected. Fetch rqsts only for that Person
                {
                    int id = Convert.ToInt32(ddlPersonName.SelectedValue);  // Convert ID of selected Person
                    pred = pred.And(r => r.PersonID == id);                 // Only requests from selected Person
                }

                // If a specific ProjectClass is selected, only select requests from that ProjectClass

                if (ddlProjectClass.SelectedIndex > 0)                      // If > ProjectClass is selected. Fetch rqsts only for that ProjectClass
                {
                    int id = Convert.ToInt32(ddlProjectClass.SelectedValue); // Convert ID of selected ProjectClass
                    pred = pred.And(r => r.ProjectClassID == id);           // Only requests from selected ProjectClass
                }

                List<Exp> exps = context.Exps.AsExpandable().Where(pred).OrderByDescending(o => o.CurrentTime).ToList();
                // Do the query using the constructed predicate, sort the result, and create a list of Exp rows

                // From this list of Exps, build a list of rows for the gvAllExp GridView

                ProjectRole projectRole = EnumActions.ConvertTextToProjectRole(litSavedProjectRole.Text); // Fetch user's project role and convert to enum
                List<rowProjectExpView> rows = new List<rowProjectExpView>(); // Create an empty list for the GridView control
                foreach (var r in exps)                                // Fill the list row-by-row
                {
                    bool useRow = false;                                // Assume that we skip this row
                    switch (r.CurrentState)
                    {
                        case ExpState.AwaitingProjectDirector:
                        case ExpState.RevisedByCommunityDirector:
                        case ExpState.RevisedByFinanceDirector:
                        case ExpState.RevisedByInternalCoordinator:
                        case ExpState.RevisedByPresident:
                        case ExpState.RevisingByProjectDirector:
                        case ExpState.UnsubmittedByInternalCoordinator:
                        case ExpState.UnsubmittedByProjectDirector:
                        case ExpState.UnsubmittedByProjectStaff:
                            if (ckRUnsubmitted.Checked)                 // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case ExpState.Approved:
                        case ExpState.AwaitingCommunityDirector:
                        case ExpState.AwaitingFinanceDirector:
                        case ExpState.AwaitingInternalCoordinator:
                        case ExpState.AwaitingPresident:
                        case ExpState.PaymentSent:
                        case ExpState.RevisingByCommunityDirector:
                        case ExpState.RevisingByFinanceDirector:
                        case ExpState.RevisingByFinanceDirectorLate:
                        case ExpState.RevisingByInternalCoordinator:
                        case ExpState.RevisingByPresident:
                            if (ckRAwaitingCWStaff.Checked)             // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case ExpState.Paid:
                            if (ckRApproved.Checked)                    // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        case ExpState.ReturnedToInternalCoordinator:
                        case ExpState.ReturnedToProjectDirector:
                        case ExpState.ReturnedToProjectStaff:
                            if (ckRReturned.Checked)                    // If true, interested in these states
                                useRow = true;                          // Process the row, don't skip it
                            break;

                        default:                                        // For all other oddballs, just skip the row
                            break;
                    }

                        if (useRow)                                     // If true. checkboxes indicate that we should use the row
                    {
                        rowProjectExpView row = new rowProjectExpView() // Empty row all ready to fill
                        {
                            RowID = r.ExpID.ToString(),                     // Convert ID from int to string for easier retrieval later
                            ExpTypeDesc = EnumActions.GetEnumDescription(r.ExpType), // Convert enum version to English version for display
                            Amount = ExtensionActions.LoadDecimalIntoTxt(r.Amount), // Carefully load decimal amount into text field
                            CurrentState = r.CurrentState,                  // Load enum version for use when row is selected
                            Archived = r.Archived,                          // Whether the request is archived
                            Rush = r.Rush,                                  // Whether the request is Rush
                            ReviseUserRole = r.ReviseUserRole               // User Role that revised request if any
                        };

                        // Insert the CreatedTime for project members to see, CurrentTime for staff to see

                        if (!RoleActions.ProjectRoleIsStaff(projectRole))   // If false user is a project member.
                            row.Time = r.CurrentTime;                       // Show project member the time of the most recent action on the request
                        else
                            row.Time = r.CreatedTime;                       // Show staff member the time when the request was created

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

                        row.CurrentStateDesc = RequestActions.AdjustCurrentStateDesc(r); // Produce appropriate CurrentStateDesc based on special cases

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

        //void ResetAppContext()
        //{
        //    btnAppArchive.Enabled = false;
        //    btnAppCopy.Enabled = false;
        //    btnAppDelete.Enabled = false;
        //    btnAppEdit.Enabled = false;                                 // Therefore, the buttons don't work
        //                                                                // Leave New button in its former state
        //    btnAppReview.Enabled = false;
        //    btnAppView.Enabled = false;
        //}

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

        void ResetDocContext()
        {
            btnDocArchive.Enabled = false;
            btnDocCopy.Enabled = false;
            btnDocDelete.Enabled = false;
            btnDocEdit.Enabled = false;                                 // Therefore, the buttons don't work
                                                                        // Leave New button in its former state
            btnDocReview.Enabled = false;
            btnDocView.Enabled = false;
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

        string GetSelectedRowID(GridView allview)
        {
            Label label = (Label)allview.SelectedRow.FindControl("lblRowID"); // Find the label control that contains DepID or ExpID
            string ID = label.Text;                                     // Extract the text of the control, which is ExpID
            if (string.IsNullOrEmpty(ID))                               // If true ID is missing
                LogError.LogInternalError("ProjectDashboard", $"Unable to find Deposit/Expense ID '{ID}' from selected GridView row in database"); // Fatal error
            return ID;
        }

        string GetSelectedRowCurrentState(GridView allview)
        {
            Label label = (Label)allview.SelectedRow.FindControl("lblCurrentState"); // Find the label control that contains DepID or ExpID
            string state = label.Text;                                     // Extract the text of the control, which is ExpID
            if (string.IsNullOrEmpty(state))                               // If true state is missing
                LogError.LogInternalError("ProjectDashboard", $"Unable to find Current State '{state}' from selected GridView row in database"); // Fatal error
            return state;
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

                DdlActions.LoadDdl(ddlEntityName, entityID, rows,
                    "", "-- All Project Entities --", alwaysDisplayLeader: true); // Put the cherry on top
            }
            return;
        }

        // Fill a Drop Down List with available GLCode Names

        void FillGLCodeDDL(int? GLCodeID)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                string fran = SupportingActions.GetFranchiseKey();  // Fetch current franchise key
                var query = from gl in context.GLCodes
                            where !gl.Inactive                    // All active projects
                                && (gl.FranchiseKey == fran)      // For this franchise
                            orderby gl.Code
                            select new { gl.GLCodeID, gl.Code };

                DataTable rows = new DataTable();
                rows.Columns.Add(PortalConstants.DdlID);
                rows.Columns.Add(PortalConstants.DdlName);

                foreach (var row in query)
                {
                    DataRow dr = rows.NewRow();                      // Build a row from the next query output
                    dr[PortalConstants.DdlID] = row.GLCodeID;
                    dr[PortalConstants.DdlName] = row.Code;
                    rows.Rows.Add(dr);                               // Add the new row to the data table
                }

                DdlActions.LoadDdl(ddlGLCode, GLCodeID, rows,
                    " -- Error: No Codes defined in Portal --", "-- All Codes --", alwaysDisplayLeader: true); // Put the cherry on top
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

                DdlActions.LoadDdl(ddlPersonName, personID, rows,
                    "", "-- All Project Persons --", alwaysDisplayLeader: true); // Put the cherry on top
            }
            return;
        }

        // Fill a Drop Down List with available ProjectClasses

        void FillProjectClassDDL(int? projectClassID)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                int projID = QueryStringActions.ConvertID(litSavedProjectID.Text).Int; // Find ID of current project

                var query = (from pc in context.ProjectClasses
                             where pc.ProjectID == projID            // This project only
                             select new { ProjectClassID = pc.ProjectClassID, Name = pc.Name }
                            ).Distinct().OrderBy(x => x.Name);      // Find all Project Classes for this project

                DataTable rows = new DataTable();
                rows.Columns.Add(PortalConstants.DdlID);
                rows.Columns.Add(PortalConstants.DdlName);

                foreach (var row in query)
                {
                    DataRow dr = rows.NewRow();                      // Build a row from the next query output
                    dr[PortalConstants.DdlID] = row.ProjectClassID;
                    dr[PortalConstants.DdlName] = row.Name;
                    rows.Rows.Add(dr);                               // Add the new row to the data table
                }

                DdlActions.LoadDdl(ddlProjectClass, projectClassID, rows,
                    "", "-- All Project Classes --", alwaysDisplayLeader: true); // Put the cherry on top
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

            int? selection = DdlActions.UnloadDdl(ddlEntityName);
            if (selection != null)                                      // If != something is selected
                projectCheckboxesCookie[PortalConstants.CProjectDdlEntityID] = selection.ToString();
            selection = DdlActions.UnloadDdl(ddlGLCode);
            if (selection != null)                                      // If != something is selected
                projectCheckboxesCookie[PortalConstants.CProjectDdlGLCodeID] = selection.ToString();
            selection = DdlActions.UnloadDdl(ddlPersonName);
            if (selection != null)                                      // If != something is selected
                projectCheckboxesCookie[PortalConstants.CProjectDdlPersonID] = selection.ToString();
            selection = DdlActions.UnloadDdl(ddlProjectClass);
            if (selection != null)                                      // If != something is selected
                projectCheckboxesCookie[PortalConstants.CProjectDdlProjectClassID] = selection.ToString();

//            projectCheckboxesCookie[PortalConstants.CProjectAppVisible] = pnlApp.Visible.ToString();
            projectCheckboxesCookie[PortalConstants.CProjectDepVisible] = pnlDep.Visible.ToString();
            projectCheckboxesCookie[PortalConstants.CProjectDocVisible] = pnlDoc.Visible.ToString();
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
                if (entityID == null)                                       // If = cookie doesn't contain an ID; no selection
                    FillEntityDDL(null);                                    // Fill the list, no selection
                else
                    FillEntityDDL(Convert.ToInt32(entityID));               // Fill the list, highlight selection

                string glCodeID = projectCheckboxesCookie[PortalConstants.CProjectDdlGLCodeID];
                if (glCodeID == null)                                       // If = cookie doesn't contain an ID; no selection
                    FillGLCodeDDL(null);                                    // Fill the list, no selection
                else
                    FillGLCodeDDL(Convert.ToInt32(glCodeID));               // Fill the list, highlight selection

                string personID = projectCheckboxesCookie[PortalConstants.CProjectDdlPersonID];
                if (personID == null)                                       // If = cookie doesn't contain an ID; no selection
                    FillPersonDDL(null);                                    // Fill the list, no selection
                else
                    FillPersonDDL(Convert.ToInt32(personID));               // Fill the list, highlight selection

                string projectClassID = projectCheckboxesCookie[PortalConstants.CProjectDdlProjectClassID];
                if (projectClassID == null)                                 // If = cookie doesn't contain an ID; no selection
                    FillProjectClassDDL(null);                              // Fill the list, no selection
                else
                    FillProjectClassDDL(Convert.ToInt32(projectClassID));   // Fill the list, highlight selection

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

                //if (projectCheckboxesCookie[PortalConstants.CProjectAppVisible] == "True")
                //    ExpandAppPanel();                                           // Expand, but don't fill Document panel
                //else
                //    CollapseAppPanel();                                         // Start with Document panel collapsed

                // Deposit Panel

                if (projectCheckboxesCookie[PortalConstants.CProjectDepVisible] == "True")
                    ExpandDepPanel();                                           // Expand, but don't fill Deposits panel
                else
                    CollapseDepPanel();                                         // Start with Deposits panel collapsed

                // Document Panel

                if (projectCheckboxesCookie[PortalConstants.CProjectDocVisible] == "True")
                    ExpandDocPanel();                                           // Expand, but don't fill Document panel
                else
                    CollapseDocPanel();                                         // Start with Document panel collapsed


                // Expense Panel

                if (projectCheckboxesCookie[PortalConstants.CProjectExpVisible] == "True")
                    ExpandExpPanel();                                           // Expand, but don't fill Requests panel
                else
                    CollapseExpPanel();                                         // Start with Requests panel collapsed
            }
            else                                                        // Cookie doesn't exist
            {
                FillEntityDDL(null);                                    // Fill the list, no selection
                FillGLCodeDDL(null);
                FillPersonDDL(null);
                FillProjectClassDDL(null);
            }
        }
    }
}
using System;
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
using System.Drawing;

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

                string userID, userRole;
                QueryStringActions.GetUserIDandRole(out userID, out userRole); // Fetch UserID and UserRole from UserInfo cookie. If cookie missing, force login
                litSavedUserID.Text = userID; litSavedUserRole.Text = userRole; // Save both values for later

                // We may not belong here - user may have snuck in by typing our URL. Catch that!

                // ** MORE HERE **

                // Make adjustments for Auditor role

                if (litSavedUserRole.Text == UserRole.Auditor.ToString())   // If == this is an Auditor
                    pnlNextReviewer.Enabled = false;                        // Next Review is irrelevant, so dim the check boxes

                RestoreCheckboxes();                                        // Read cookie, restore checkbox settings, fill DDLs

                int rows = CookieActions.GetGridViewRows();                // Find number of rows per page from cookie
                gvStaffApp.PageSize = rows;                                 // Adjust each GridView accordingly
                gvStaffDep.PageSize = rows;
                gvStaffExp.PageSize = rows;

                LoadAllApps(RestorePageIndex(PortalConstants.CStaffPIApp)); // Load the grid view of Approvals and reposition
                LoadAllDeps(RestorePageIndex(PortalConstants.CStaffPIDep)); // Load the grid view of Deposits and reposition
                LoadAllExps(RestorePageIndex(PortalConstants.CStaffPIExp)); // Load the grid view of Expenses and reposition
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

        // A search criteria has changed. Refresh the lists of requests

        protected void SearchCriteriaChanged(object sender, EventArgs e)
        {
            LoadAllApps();                                                  // Recreate the grid view of Approvals
            LoadAllDeps();                                                  // Load the grid view of Deposits
            LoadAllExps();                                                  // Load the grid view of Expenses

            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            SaveStaffPageIndexes();                                         // Save position within each gridview
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
            SaveStaffPageIndexes();                                         // Save position within each gridview
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
            SaveStaffPageIndexes();                                         // Save position within each gridview
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
            SaveStaffPageIndexes();                                         // Save position within each gridview
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

        protected void gvStaffApp_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                LoadAllApps(e.NewPageIndex);                                // Reload the grid view control, position to target page
                SaveStaffPageIndexes();                                     // Save position within each gridview
            }
            return;
        }

        protected void gvStaffDep_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                LoadAllDeps(e.NewPageIndex);                                // Reload the grid view control, position to target page
                SaveStaffPageIndexes();                                     // Save position within each gridview
            }
            return;
        }

        protected void gvStaffExp_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                        // If >= a value that we can handle
            {
                LoadAllExps(e.NewPageIndex);                                // Reload the grid view control, position to target page
                SaveStaffPageIndexes();                                     // Save position within each gridview
            }
            return;
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void gvStaffApp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                Common_RowDataBound(sender, e, "btnAppDblClick");           // Do the common setup work for the row

                // See whether the User is next to act on this row

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                AppState state = EnumActions.ConvertTextToAppState(label.Text); // Carefully convert back into enumeration type
                if (StateActions.UserRoleToProcessRequest(state) == EnumActions.ConvertTextToUserRole(litSavedUserRole.Text)) // If == user can approve request
                    e.Row.Cells[rowStaffApp.OwnerColumn].Font.Bold = true; // Bold Status cell.
            }
            return;
        }

        protected void gvStaffDep_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                Common_RowDataBound(sender, e, "btnDepDblClick");           // Do the common setup work for the row

                // See whether the User is next to act on this row

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                DepState state = EnumActions.ConvertTextToDepState(label.Text); // Carefully convert back into enumeration type
                if (StateActions.UserRoleToProcessRequest(state) == EnumActions.ConvertTextToUserRole(litSavedUserRole.Text)) // If == user can process request
                {
                    e.Row.Cells[rowStaffDep.OwnerColumn].Font.Bold = true;  // Bold Status cell.
                    if (StateActions.RequestIsRevising(state))         // If true this request is being revised now. Revision can continue, not review
                    {
                        Button btn = (Button)e.Row.FindControl("btnGridReview"); // Find the button on the row
                        if (btn == null)                                    // If true the button is missing. That's an error.
                            LogError.LogInternalError("StaffDashboard", $"btnGridReview not found in selected GridView row"); // Log fatal error
                        btn.Text = "Revise";                                // Change button to show a revise, not a review
                        btn.ToolTip = "Click to continue editing this request"; // Change flyover help as well
                    }
                }

            }
            return;
        }

        protected void gvStaffExp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)                // If == this is indeed a row of our GridView control
            {
                Common_RowDataBound(sender, e, "btnExpDblClick");           // Do the common setup work for the row

                // See whether the User is next to act on this row

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                ExpState state = EnumActions.ConvertTextToExpState(label.Text); // Carefully convert back into enumeration type

                // See if the row is Rush

                label = (Label)e.Row.FindControl("lblRush");                // Find the label control that contains Rush in this row
                if (label.Text == "True")                                   // If == this record is Rush
                    e.Row.ForeColor = Color.Red;                            // Use color to indicate Rush status

                // See if user can approve row

                if (StateActions.UserRoleToProcessRequest(state) == EnumActions.ConvertTextToUserRole(litSavedUserRole.Text)) // If == user can approve request
                {
                    e.Row.Cells[rowStaffExp.OwnerColumn].Font.Bold = true; // Bold Status cell.

                    // If row is waiting for FD for the second time, highlight it

                    if (StateActions.NextReviewIsSecondFD(state))           // If true next review is second FD review
                    {
                        //e.Row.Cells[rowStaffExp.StateColumn].ForeColor = Color.Purple; // Use color to indicate this condition
                        //e.Row.Cells[rowStaffExp.StateColumn].Font.Bold = true; // Also make it bold, since purple doesn't show up very well
                        e.Row.ForeColor = Color.Purple;                     // Make the whole row purple
                        e.Row.Font.Bold = true;                             // Make the whole row bold
                    }

                    if (StateActions.RequestIsRevising(state))         // If true this request is being revised now. Revision can continue, not review
                    {
                        Button btn = (Button)e.Row.FindControl("btnGridReview"); // Find the button on the row
                        if (btn == null)                                    // If true the button is missing. That's an error.
                            LogError.LogInternalError("StaffDashboard", $"btnGridReview not found in selected GridView row"); // Log fatal error
                        btn.Text = "Revise";                                // Change button to show a revise, not a review
                        btn.ToolTip = "Click to continue editing this request"; // Change flyover help as well
                    }
                }
            }
            return;
        }

        // Common code for _RowDataBound methods. Set several attributes of the row.

        void Common_RowDataBound(object sender, GridViewRowEventArgs e, string btnName)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
            e.Row.ToolTip = "Click button at right to process this request";      // Establish tool tip during flyover

            // To implement double click, find a button (name supplied by caller) that we have hidden in the row.
            // Tell the row to fire that button when the row is double clicked.

            Button dblClick = (Button)e.Row.FindControl(btnName);       // Find hidden the dblclick button control
            e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(dblClick, ""); // Double clicking row fires button postback

            // See if the row is Archived

            Label label = (Label)e.Row.FindControl("lblArchived");      // Find the label control that contains Archived in this row
            if (label.Text == "True")                                   // If == this record is Archived
                e.Row.Font.Italic = true;                               // Use italics to indicate Archived status

            return;
        }

        // The Review button within a GridView row has been clicked. Dispatch to the Review page.

        protected void btnAppGridReview_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((Button)sender).DataItemContainer;  // Find the row that contains the button that just clicked
            DispatchReview(((Button)(row.FindControl("btnAppDblClick"))).Text,  // OK, within that row, find the double click button. Its text is rowID
                PortalConstants.URLReviewApproval);                             // Dispatch request ID to Review page
        }

        protected void btnDepGridReview_Click(object sender, EventArgs e)
        {
            DispatchReviewRevise((GridViewRow)((Button)sender).DataItemContainer, PortalConstants.URLReviewDeposit, PortalConstants.URLEditDeposit); // Do it!
        }

        protected void btnExpGridReview_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((Button)sender).DataItemContainer;  // Find the row that contains the button that just clicked
            DispatchReview(((Button)(row.FindControl("btnExpDblClick"))).Text,  // OK, within that row, find the double click button. Its text is rowID
                PortalConstants.URLReviewExpense);                              // Dispatch request ID to Review page
        }

        // A row in the GridView has been double clicked. We dispatch to the Review function for the selected row.

        protected void btnAppDblClick_Click(object sender, EventArgs e)
        {
            DispatchReview(((Button)sender).Text, PortalConstants.URLReviewApproval); // Dispatch request ID to Review Approval page
        }

        protected void btnDepDblClick_Click(object sender, EventArgs e)
        {
            DispatchReviewRevise((GridViewRow)((Button)sender).DataItemContainer, PortalConstants.URLReviewDeposit, PortalConstants.URLEditDeposit); // Do it!
        }

        protected void btnExpDblClick_Click(object sender, EventArgs e)
        {
            DispatchReviewRevise((GridViewRow)((Button)sender).DataItemContainer, PortalConstants.URLReviewExpense, PortalConstants.URLEditExpense); // Do it!
        }

        // Code that is common to all "Review" and "Revise" buttons and double clicks. If the Request ID is sensible, redirect to the Review page.

        void DispatchReview(string rqstID, string url)
        {
            if (string.IsNullOrEmpty(rqstID))                               // If true the RqstID is missing. That's an error.
                LogError.LogInternalError("StaffDashboard", $"RqstID not found in selected GridView row"); // Log fatal error

            // It is possible that the user does not have the authority to review the rqst in
            // its current state. But we'll let Review display all the detail for the Rqst and then deny editing.

            Response.Redirect(url + "?" + PortalConstants.QSUserRole + "=" + litSavedUserRole.Text
                                  + "&" + PortalConstants.QSRequestID + "=" + rqstID  // Start with an existing request
                                  + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview // Review it
                                  + "&" + PortalConstants.QSReturn + "=" + PortalConstants.URLStaffDashboard); // Return to this page when done
        }

        // Code that is common to all "Review" and "Revise" buttons and double clicks. 
        // For "Review" we pass control to the Review page.
        // For "Revise" we pass control to the Edit page. This is harder because we need more context (i.e., Query Strings) to make it go.
        // Context is loaded in invisible cells of the row.

        void DispatchReviewRevise(GridViewRow row, string reviewURL, string reviseURL)
        {
            Label rqstID = (Label)row.FindControl("lblRequestID");       // Locate the cell containing the row ID
            Label projectID = (Label)row.FindControl("lblProjectID");       // Locate the cell containing the project ID (if any)
            Button btn = (Button)row.FindControl("btnGridReview");          // Locate the button on the row
            if ((rqstID == null) || (projectID == null) || (btn == null)) // If == couldn't find the label. Fatal error
                LogError.LogInternalError("StaffDashboard", $"lblRequestID or lblProjectID or btnGridReview control not found in selected GridView row"); // Log fatal error

            // Figure out whether the Name of the button was "Review" or "Revise," indicating which action we should take

            if (btn.Text == "Revise")                                           // If == this is a Revise function
            {
                // Find current role and map to the role to be used for editing.

                UserRole userRole = EnumActions.ConvertTextToUserRole(QueryStringActions.GetUserRole()); // Fetch User Role from UserInfo cookie and stash
                string reviseRole = RoleActions.GetRevisingRole(userRole).ToString(); // Find role that EditDeposit should use during edits

                // Invoke the Edit function just like our coworker "ProjectDashboard" would. We pretend that we're a project member for this operation.

                Response.Redirect(reviseURL + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + projectID.Text + "&"
                                            + PortalConstants.QSProjectRole + "=" + reviseRole + "&"
                                            + PortalConstants.QSRequestID + "=" + rqstID.Text + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandRevise + "&" // Start with an existing request
                                            + PortalConstants.QSReturn + "=" + PortalConstants.URLStaffDashboard); // Return to this page when done
            }
            else
            {

                // Invoke the Review function. This does not take as much context - we don't have to associate ourselves with the project.

                Response.Redirect(reviewURL + "?" + PortalConstants.QSUserRole + "=" + litSavedUserRole.Text
                                      + "&" + PortalConstants.QSRequestID + "=" + rqstID.Text  // Start with an existing request
                                      + "&" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview // Review it
                                      + "&" + PortalConstants.QSReturn + "=" + PortalConstants.URLStaffDashboard); // Return to this page when done

            }
        }

        // Fetch all of the Approvals for CS Staff, subject to further search constraints. Display in a GridView

        void LoadAllApps(int pageIndex = 0)
        {
            if (!pnlApp.Visible)                                            // If false the panel is not visible
                return;                                                     // Don't waste time on filling it

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // There are so many permutations of this query that we have to build it up piece by piece.

                var pred = PredicateBuilder.True<App>();                    // Initialize predicate to select from App table
                pred = pred.And(r => !r.Inactive);                          // Only active Apps

                // Process "Active" and "Archived" flags. They're not independent, so we have to grind through their combinations

                if (ckRActive.Checked && ckRArchived.Checked)           // If true, take them all
                    ;                                                   // Don't ignore anything
                else if (ckRActive.Checked && !ckRArchived.Checked)     // If true, only Active checked
                    pred = pred.And(r => !r.Archived);                  // Ignore Archived requests
                else if (!ckRActive.Checked && ckRArchived.Checked)     // If true, only Archived checked
                    pred = pred.And(r => r.Archived);                   // Ignore Active requests
                else                                                    // Both boxes are unchecked
                    pred = pred.And(r => r.Archived && !r.Archived);    // Nonsensical. Returns nothing

                // If a specific project is selected, only select Apps from that project

                if (ddlProjectName.SelectedIndex != 0)                      // If != Project is selected. Fetch rqsts only for that Projects
                {
                    int projectID = Convert.ToInt32(ddlProjectName.SelectedValue);  // Convert ID of selected Project
                    pred = pred.And(r => r.ProjectID == projectID);         // Only active Apps from current project
                }

                // Now look for Requests in the right state. For auditor, it's only complete requests.

                if (litSavedUserRole.Text == UserRole.Auditor.ToString())   // If == user is an Auditor
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

                // From this list of Rqsts, build a list of rows for the gvStaffApp GridView based on the selection criteria provided by the user.

                List<rowStaffApp> rows = new List<rowStaffApp>();   // Create the empty list
                foreach (var a in apps)
                {
                    bool useRow = false;                                    // Assume we're not interested in the row

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
                                LogError.LogInternalError("StaffDashboard", $"Invalid AppType in Approval RqstID '{a.AppID.ToString()}'"); // Log fatal error
                                break;
                            }
                    }
                    if (useRow)                                         // If true row passes the Request Type screen
                    {
                        if (litSavedUserRole.Text != UserRole.Auditor.ToString()) // If true User is not an Auditor. Auditors don't check Next Reviewer
                        {

                            // Apply the Next Reviewer screen using the Current State of the row. Current State determines the identity of the next
                            // reviewer. It's Next Reviewer that's screened via checkboxes.

                            useRow = false;                             // Assume this next screen will prove negative for the row

                            switch (a.CurrentState)                     // By Current State, look for relevant Next Reviewer checkbox
                            {
                                case AppState.Approved:
                                    {
                                        if (ckRCompleted.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case AppState.AwaitingFinanceDirector:
                                    {
                                        if (ckRFinanceDirector.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case AppState.AwaitingInternalCoordinator:
                                    {
                                        if (ckRInternalCoordinator.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case AppState.AwaitingProjectDirector:
                                case AppState.UnsubmittedByInternalCoordinator:
                                case AppState.UnsubmittedByProjectDirector:
                                case AppState.UnsubmittedByProjectStaff:
                                    {
                                        if (ckRUnsubmitted.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case AppState.AwaitingCommunityDirector:
                                    {
                                        if (ckRCommunityDirector.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case AppState.AwaitingPresident:
                                    {
                                        if (ckRPresident.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case AppState.Returned:
                                    {
                                        if (ckRReturned.Checked)
                                            useRow = true;
                                        break;
                                    }
                                default:
                                    {
                                        LogError.LogInternalError("StaffDashboard", $"Invalid CurrentState in Approval RqstID '{a.AppID.ToString()}'"); // Log fatal error
                                        break;
                                    }
                            }
                        }
                        if (useRow)                                         // If true passed the second screen
                        {
                            rowStaffApp row = new rowStaffApp()
                            {                                               // Empty row all ready to fill
                                RowID = a.AppID.ToString(),                 // Convert ID from int to string for easier retrieval later
                                ProjectID = a.ProjectID.ToString(),         // Same treatment for project ID
                                ProjectName = a.Project.Name,               // Fetch project name
                                CurrentTime = a.CurrentTime,                // When request was last updated
                                AppTypeDesc = EnumActions.GetEnumDescription((Enum)a.AppType), // Convert enum form to English for display
                                AppReviewType = a.AppReviewType.ToString(), // Stick with the value, it's brief and row is crowded
                                CurrentState = a.CurrentState,              // Put this in so we can get it out later to dispatch; it's not Visible
                                CurrentStateDesc = EnumActions.GetEnumDescription(a.CurrentState), // Convert enum form to English for display
                                ReturnNote = a.ReturnNote,
                                Owner = EnumActions.GetEnumDescription(StateActions.UserRoleToProcessRequest(a.CurrentState)), // Fetch "English" version of owner
                                Description = a.Description,
                                Archived = a.Archived
                            };
                            if (a.Archived)                                  // If true row is Archived
                                row.CurrentStateDesc = row.CurrentStateDesc + " (Archived)"; // Append indication that it's archifed

                            rows.Add(row);                                  // Add the filled-in row to the list of rows
                        }
                    }
                }

                gvStaffApp.DataSource = rows;                             // Give it to the GridView control
                gvStaffApp.PageIndex = pageIndex;                         // Position to the requested page
                gvStaffApp.DataBind();                                    // And get it in gear

                // Not enough to merit this                AllRqstRowCount.Text = rqsts.Count().ToString();     // Show total number of rows for amusement 

                NavigationActions.EnableGridViewNavButtons(gvStaffApp);   // Enable appropriate nav buttons based on page count
                gvStaffApp.SelectedIndex = -1;                            // No selected row any more
                litSelectedAppRow.Text = "";                                // No selected row remembered (for double click)
            }
            return;
        }

        // Fetch all of the Deposits for CS Staff, subject to further search constraints. Display in a GridView

        void LoadAllDeps(int pageIndex = 0)
        {
            if (!pnlDep.Visible)                                            // If false the panel is not visible
                return;                                                     // Don't waste time on filling it

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // There are so many permutations of this query that we have to build it up piece by piece.

                var pred = PredicateBuilder.True<Dep>();                    // Initialize predicate to select from Dep table
                pred = pred.And(r => !r.Inactive);                          // Only active Requests

                // Process "Active" and "Archived" flags. They're not independent, so we have to grind through their combinations

                if (ckRActive.Checked && ckRArchived.Checked)           // If true, take them all
                    ;                                                   // Don't ignore anything
                else if (ckRActive.Checked && !ckRArchived.Checked)     // If true, only Active checked
                    pred = pred.And(r => !r.Archived);                  // Ignore Archived requests
                else if (!ckRActive.Checked && ckRArchived.Checked)     // If true, only Archived checked
                    pred = pred.And(r => r.Archived);                   // Ignore Active requests
                else                                                    // Both boxes are unchecked
                    pred = pred.And(r => r.Archived && !r.Archived);    // Nonsensical. Returns nothing

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

                // If a specific project is selected, only select Deps from that project

                if (ddlProjectName.SelectedIndex != 0)                      // If != Project is selected. Fetch rqsts only for that Projects
                {
                    int projectID = Convert.ToInt32(ddlProjectName.SelectedValue);  // Convert ID of selected Project
                    pred = pred.And(r => r.ProjectID == projectID);         // Only active Apps from current project
                }

                // Now look for Requests in the right state. For auditor, it's only complete requests.

                if (litSavedUserRole.Text == UserRole.Auditor.ToString())   // If == user is an Auditor
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

                List<rowStaffDep> rows = new List<rowStaffDep>();           // Create the empty list
                foreach (var d in deps)
                {
                    bool useRow = false;                                    // Assume we're not interested in the row

                    // Process the Deposit Type checkboxes. If the "All Types" box is checked, take every row. Otherwise, only take rows
                    // whose DepositType matches a checked checkbox.

                    switch (d.DepType)                                      // By DepositType, look for relevant checkbox
                    {
                        case DepType.Cash:
                            {
                                if (ckDCash.Checked)                        // If true relevant checkbox is checked
                                    useRow = true;                          // Take the row
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
                                LogError.LogInternalError("StaffDashboard", $"Invalid DepositType in Deposit RqstID '{d.DepID.ToString()}'"); // Log fatal error
                                break;
                            }
                    }
                    if (useRow)                                             // If true row passes the Request Type screen
                    {
                        if (litSavedUserRole.Text != UserRole.Auditor.ToString()) // If true User is not an Auditor. Auditors don't check Next Reviewer
                        {

                            // Apply the Next Reviewer screen using the Current State of the row. Current State determines the identity of the next
                            // reviewer. It's Next Reviewer that's screen via checkboxes.

                            useRow = false;                                 // Assume this next screen will prove negative for the row

                            switch (d.CurrentState)                         // By Current State, look for relevant checkbox
                            {
                                case DepState.ApprovedReadyToDeposit:
                                case DepState.DepositComplete:
                                    {
                                        if (ckRCompleted.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case DepState.AwaitingFinanceDirector:
                                case DepState.RevisingByFinanceDirector:
                                    {
                                        if (ckRFinanceDirector.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case DepState.AwaitingProjectDirector:
                                case DepState.UnsubmittedByInternalCoordinator:
                                case DepState.RevisingByProjectDirector:
                                case DepState.RevisedByFinanceDirector:
                                    {
                                        if (ckRUnsubmitted.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case DepState.AwaitingCommunityDirector:
                                    {
                                        if (ckRCommunityDirector.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case DepState.Returned:
                                    {
                                        if (ckRReturned.Checked)
                                            useRow = true;
                                        break;
                                    }
                                default:
                                    {
                                        LogError.LogInternalError("StaffDashboard", $"Invalid CurrentState in Deposit RqstID '{d.DepID.ToString()}'"); // Log fatal error
                                        break;
                                    }
                            }
                        }
                        if (useRow)                                         // If true passed the second screen
                        {
                            rowStaffDep row = new rowStaffDep()
                            {                                               // Empty row all ready to fill
                                RequestID = d.DepID.ToString(),                 // Convert ID from int to string for easier retrieval later
                                ProjectID = d.ProjectID.ToString(),         // Same treatment for project ID
                                ProjectName = d.Project.Name,               // Fetch project name
                                CurrentTime = d.CurrentTime,                // When request was last updated
                                DepTypeDesc = EnumActions.GetEnumDescription((Enum)d.DepType), // Convert enum form to English for display
                                Amount = d.Amount,
                                CurrentState = d.CurrentState,              // Put this in so we can get it out later to dispatch; it's not Visible
                                CurrentStateDesc = EnumActions.GetEnumDescription(d.CurrentState), // Convert enum form to English for display
                                ReturnNote = d.ReturnNote,
                                Owner = EnumActions.GetEnumDescription(StateActions.UserRoleToProcessRequest(d.CurrentState)), // Fetch "English" version of owner
                                Description = d.Description,
                                Archived = d.Archived
                            };
                            if (d.Archived)                                  // If true row is Archived
                                row.CurrentStateDesc = row.CurrentStateDesc + " (Archived)"; // Append indication that it's archifed

                            rows.Add(row);                                  // Add the filled-in row to the list of rows
                        }
                    }
                }

                gvStaffDep.DataSource = rows;                             // Give it to the GridView control
                gvStaffDep.PageIndex = pageIndex;                         // Position to the requested page
                gvStaffDep.DataBind();                                    // And get it in gear

                // Not enough to merit this                AllRqstRowCount.Text = rqsts.Count().ToString();     // Show total number of rows for amusement 

                NavigationActions.EnableGridViewNavButtons(gvStaffDep);   // Enable appropriate nav buttons based on page count
                gvStaffDep.SelectedIndex = -1;                            // No selected row any more
                litSelectedDepRow.Text = "";                                // No selected row remembered (for double click)
            }
            return;
        }

        // Fetch all of the Expenses for CS Staff, subject to further search constraints. Display in a GridView

        void LoadAllExps(int pageIndex = 0)
        {
            if (!pnlExp.Visible)                                            // If false the panel is not visible
                return;                                                     // Don't waste time on filling it

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                // There are so many permutations of this query that we have to build it up piece by piece.

                var pred = PredicateBuilder.True<Exp>();                    // Initialize predicate to select from Exp table
                pred = pred.And(r => !r.Inactive);                          // Only active Requests

                // Process "Active" and "Archived" flags. They're not independent, so we have to grind through their combinations

                if (ckRActive.Checked && ckRArchived.Checked)           // If true, take them all
                    ;                                                   // Don't ignore anything
                else if (ckRActive.Checked && !ckRArchived.Checked)     // If true, only Active checked
                    pred = pred.And(r => !r.Archived);                  // Ignore Archived requests
                else if (!ckRActive.Checked && ckRArchived.Checked)     // If true, only Archived checked
                    pred = pred.And(r => r.Archived);                   // Ignore Active requests
                else                                                    // Both boxes are unchecked
                    pred = pred.And(r => r.Archived && !r.Archived);    // Nonsensical. Returns nothing

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

                // If a specific Project is selected, only select requests from that Project

                if (ddlProjectName.SelectedIndex > 0)                       // If > Project is selected. Fetch rqsts only for that Project
                {
                    int id = Convert.ToInt32(ddlProjectName.SelectedValue); // Convert ID of selected Project
                    pred = pred.And(r => r.ProjectID == id);                // Only requests from selected project
                }

                // Now look for Requests in the right state. For staff, it everything submitted for review. For auditor, it's only complete requests.

                if (litSavedUserRole.Text == UserRole.Auditor.ToString())   // If == user is an Auditor
                {
                    pred = pred.And(r => (r.CurrentState == ExpState.Paid)); // Auditors only see Complete Requests
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

                List<rowStaffExp> rows = new List<rowStaffExp>();   // Create the empty list
                foreach (var r in exps)
                {
                    bool useRow = false;                                    // Assume we're not interested in the row

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
                        case ExpType.PEXCard:
                            {
                                if (ckEPEXCard.Checked)                // If true relevant checkbox is checked
                                    useRow = true;                      // Take the row
                                break;
                            }
                        case ExpType.Payroll:
                            {
                                if (ckEPayroll.Checked)                // If true relevant checkbox is checked
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
                                LogError.LogInternalError("StaffDashboard", $"Invalid RqstType in Expense RqstID '{r.ExpID.ToString()}'"); // Log fatal error
                                break;
                            }
                    }
                    if (useRow)                                             // If true row passes the Expense Type screen
                    {
                        if (litSavedUserRole.Text != UserRole.Auditor.ToString()) // If true User is not an Auditor. Auditors don't check Next Reviewer
                        {

                            // Apply the Next Reviewer screen using the Current State of the row. Current State determines the identity of the next
                            // reviewer. It's Next Reviewer that's screen via checkboxes.

                            useRow = false;                                 // Assume this next screen will prove negative for the row

                            switch (r.CurrentState)                         // By Reviewer Type, look for relevant checkbox
                            {
                                case ExpState.AwaitingCommunityDirector:
                                case ExpState.RevisingByCommunityDirector:
                                    {
                                        if (ckRCommunityDirector.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case ExpState.Approved:
                                case ExpState.AwaitingFinanceDirector:
                                case ExpState.RevisingByFinanceDirector:
                                case ExpState.PaymentSent:
                                    {
                                        if (ckRFinanceDirector.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case ExpState.AwaitingInternalCoordinator:
                                case ExpState.RevisingByInternalCoordinator:
                                    {
                                        if (ckRInternalCoordinator.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case ExpState.AwaitingPresident:
                                case ExpState.RevisingByPresident:
                                    {
                                        if (ckRPresident.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case ExpState.AwaitingProjectDirector:
                                case ExpState.RevisedByCommunityDirector:
                                case ExpState.RevisedByFinanceDirector:
                                case ExpState.RevisedByInternalCoordinator:
                                case ExpState.RevisedByPresident:
                                case ExpState.UnsubmittedByInternalCoordinator:
                                case ExpState.UnsubmittedByProjectDirector:
                                case ExpState.UnsubmittedByProjectStaff:
                                    {
                                        if (ckRUnsubmitted.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case ExpState.Paid:
                                    {
                                        if (ckRCompleted.Checked)
                                            useRow = true;
                                        break;
                                    }
                                case ExpState.ReturnedToInternalCoordinator:
                                case ExpState.ReturnedToProjectDirector:
                                case ExpState.ReturnedToProjectStaff:
                                    {
                                        if (ckRReturned.Checked)
                                            useRow = true;
                                        break;
                                    }
                                default:
                                    {
                                        LogError.LogInternalError("StaffDashboard", $"Invalid CurrentState in Expense RqstID '{r.ExpID.ToString()}'"); // Log fatal error
                                        break;
                                    }
                            }
                        }
                        if (useRow)                                         // If true passed the second screen
                        {
                            rowStaffExp row = new rowStaffExp()
                            {          // Empty row all ready to fill
                                RowID = r.ExpID.ToString(),                 // Convert ID from int to string for easier retrieval later
                                ProjectID = r.ProjectID.ToString(),         // Same treatment for project ID
                                ProjectName = r.Project.Name,               // Fetch project name
                                CurrentTime = r.CurrentTime,                // When request was last updated
                                ExpTypeDesc = EnumActions.GetEnumDescription(r.ExpType), // Convert enum form to English for display
                                Amount = r.Amount,
                                CurrentState = r.CurrentState,              // Put this in so we can get it out later to dispatch; it's not Visible
                                CurrentStateDesc = EnumActions.GetEnumDescription(r.CurrentState), // Convert enum form to English for display
                                Description = r.Description,
                                ReturnNote = r.ReturnNote,
                                Owner = EnumActions.GetEnumDescription(StateActions.UserRoleToProcessRequest(r.CurrentState)), // Fetch "English" version of owner
                                Archived = r.Archived,
                                Rush = r.Rush                               // Whether the Request is a "Rush"
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
//                            row.Summary = r.Summary;

                            if (r.Archived)                                     // If true row is Archived
                                row.CurrentStateDesc = row.CurrentStateDesc + " (Archived)"; // Append indication that it's archifed

                            rows.Add(row);                                  // Add the filled-in row to the list of rows
                        }
                    }
                }
                gvStaffExp.DataSource = rows;                             // Give it to the GridView control
                gvStaffExp.PageIndex = pageIndex;                         // Position to the requested page
                gvStaffExp.DataBind();                                    // And get it in gear

                // Not enough to merit this                AllRqstRowCount.Text = rqsts.Count().ToString();     // Show total number of rows for amusement 

                NavigationActions.EnableGridViewNavButtons(gvStaffExp);   // Enable appropriate nav buttons based on page count
                gvStaffExp.SelectedIndex = -1;                            // No selected row any more
                litSelectedExpRow.Text = "";                                // No selected row remembered (for double click)
            }
            return;
        }

        // We no longer have a selected Request. Adjust buttons

        //void ResetAppContext()
        //{
        //    btnAppReview.Enabled = false;
        //}

        //void ResetDepContext()
        //{
        //    btnDepReview.Enabled = false;
        //}

        //void ResetExpContext()
        //{
        //    btnExpReview.Enabled = false;
        //}

        // Fill a Drop Down List with available Entity Names

        void FillEntityDDL(int? entityID)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                string fran = SupportingActions.GetFranchiseKey();  // Fetch current franchise key
                var query = from ent in context.Entitys
                            where !ent.Inactive                    // All active projects
                                && (ent.FranchiseKey == fran)      // For this franchise
                            orderby ent.Name
                            select new { ent.EntityID, ent.Name };

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
                    " -- Error: No Entities defined in Portal --", "-- All Entities --", alwaysDisplayLeader: true); // Put the cherry on top
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
                string fran = SupportingActions.GetFranchiseKey();  // Fetch current franchise key
                var query = from pers in context.Persons
                            where !pers.Inactive                    // All active projects
                                && (pers.FranchiseKey == fran)      // For this franchise
                            orderby pers.Name
                            select new { pers.PersonID, pers.Name };

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
                    " -- Error: No Persons defined in Portal --", "-- All Persons --", alwaysDisplayLeader: true); // Put the cherry on top
            }
            return;
        }

        // Fill a Drop Down List with available Project Names

        void FillProjectDDL(int? projectID)
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

                DdlActions.LoadDdl(ddlProjectName, projectID, rows,
                    " -- Error: No Projects defined in Portal --", "-- All Projects --", alwaysDisplayLeader: true); // Put the cherry on top
            }
            return;
        }

        // Save the current settings of the checkboxes in a (big) cookie

        void SaveCheckboxes()
        {
            HttpCookie staffCheckboxesCookie = new HttpCookie(PortalConstants.CStaffCheckboxes);

            staffCheckboxesCookie[PortalConstants.CStaffCkSearchVisible] = pnlSearch.Visible.ToString();

            staffCheckboxesCookie[PortalConstants.CStaffCkRCompleted] = ckRCompleted.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkRFinanceDirector] = ckRFinanceDirector.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkRInternalCoordinator] = ckRInternalCoordinator.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkRReturned] = ckRReturned.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkRCommunityDirector] = ckRCommunityDirector.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkRPresident] = ckRPresident.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkRUnsubmitted] = ckRUnsubmitted.Checked.ToString();

            staffCheckboxesCookie[PortalConstants.CStaffFromDate] = txtBeginningDate.Text;
            staffCheckboxesCookie[PortalConstants.CStaffToDate] = txtEndingDate.Text;

            int? selection = DdlActions.UnloadDdl(ddlEntityName);
            if (selection != null)                                      // If != something is selected
                staffCheckboxesCookie[PortalConstants.CStaffDdlEntityID] = selection.ToString();
            selection = DdlActions.UnloadDdl(ddlGLCode);
            if (selection != null)                                      // If != something is selected
                staffCheckboxesCookie[PortalConstants.CStaffDdlGLCodeID] = selection.ToString();
            selection = DdlActions.UnloadDdl(ddlPersonName);
            if (selection != null)                                      // If != something is selected
                staffCheckboxesCookie[PortalConstants.CStaffDdlPersonID] = selection.ToString();
            selection = DdlActions.UnloadDdl(ddlProjectName);
            if (selection != null)                                      // If != something is selected
                staffCheckboxesCookie[PortalConstants.CStaffDdlProjectID] = selection.ToString();

            staffCheckboxesCookie[PortalConstants.CStaffCkRActive] = ckRActive.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkRArchived] = ckRArchived.Checked.ToString();

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
            staffCheckboxesCookie[PortalConstants.CStaffCkEPEXCard] = ckEPEXCard.Checked.ToString();
            staffCheckboxesCookie[PortalConstants.CStaffCkEPayroll] = ckEPayroll.Checked.ToString();
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

                // Search panel

                if (staffCheckboxesCookie[PortalConstants.CStaffCkSearchVisible] == "True")
                    ExpandSearchPanel();
                else
                    CollapseSearchPanel();

                // Role check boxes

                if (staffCheckboxesCookie[PortalConstants.CStaffCkRCompleted] == "True") ckRCompleted.Checked = true;
                else ckRCompleted.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkRFinanceDirector] == "True") ckRFinanceDirector.Checked = true;
                else ckRFinanceDirector.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkRInternalCoordinator] == "True") ckRInternalCoordinator.Checked = true;
                else ckRInternalCoordinator.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkRReturned] == "True") ckRReturned.Checked = true;
                else ckRReturned.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkRCommunityDirector] == "True") ckRCommunityDirector.Checked = true;
                else ckRCommunityDirector.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkRPresident] == "True") ckRPresident.Checked = true;
                else ckRPresident.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkRUnsubmitted] == "True") ckRUnsubmitted.Checked = true;
                else ckRUnsubmitted.Checked = false;

                // Date Range

                txtBeginningDate.Text = staffCheckboxesCookie[PortalConstants.CStaffFromDate];
                txtEndingDate.Text = staffCheckboxesCookie[PortalConstants.CStaffToDate];

                // Dropdown Lists

                string entityID = staffCheckboxesCookie[PortalConstants.CStaffDdlEntityID];
                if (entityID == null)                                       // If = cookie doesn't contain an Entity ID; no selection
                    FillEntityDDL(null);                                    // Fill the list, no selection
                else
                    FillEntityDDL(Convert.ToInt32(entityID));               // Fill the list, highlight selection

                string GLCodeID = staffCheckboxesCookie[PortalConstants.CStaffDdlGLCodeID];
                if (GLCodeID == null)                                       // If = cookie doesn't contain an GLCode ID; no selection
                    FillGLCodeDDL(null);                                    // Fill the list, no selection
                else
                    FillGLCodeDDL(Convert.ToInt32(GLCodeID));               // Fill the list, highlight selection

                string personID = staffCheckboxesCookie[PortalConstants.CStaffDdlPersonID];
                if (personID == null)                                       // If = cookie doesn't contain an Person ID; no selection
                    FillPersonDDL(null);                                    // Fill the list, no selection
                else
                    FillPersonDDL(Convert.ToInt32(personID));               // Fill the list, highlight selection

                string projectID = staffCheckboxesCookie[PortalConstants.CStaffDdlProjectID];
                if (projectID == null)                                      // If = cookie doesn't contain an Project ID; no selection
                    FillProjectDDL(null);                                   // Fill the list, no selection
                else
                    FillProjectDDL(Convert.ToInt32(projectID));             // Fill the list, highlight selection

                // Archive check boxes

                string active = staffCheckboxesCookie[PortalConstants.CStaffCkRActive];
                if (active != null)                             // If != value is present
                {
                    if (active == "True")                       // If == checkbox was checked
                        ckRActive.Checked = true;
                    else
                        ckRActive.Checked = false;
                }

                string archive = staffCheckboxesCookie[PortalConstants.CStaffCkRArchived];
                if (archive != null)                            // If != value is present
                {
                    if (archive == "True")                      // If == checkbox was checked
                        ckRArchived.Checked = true;
                    else
                        ckRArchived.Checked = false;
                }

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

                if (staffCheckboxesCookie[PortalConstants.CStaffCkEPEXCard] == "True") ckEPEXCard.Checked = true;
                else ckEPEXCard.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkEPayroll] == "True") ckEPayroll.Checked = true;
                else ckEPayroll.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkEPurchaseOrder] == "True") ckEPurchaseOrder.Checked = true;
                else ckEPurchaseOrder.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkEReimbursement] == "True") ckEReimbursement.Checked = true;
                else ckEReimbursement.Checked = false;

                if (staffCheckboxesCookie[PortalConstants.CStaffCkEVendorInvoice] == "True") ckEVendorInvoice.Checked = true;
                else ckEVendorInvoice.Checked = false;
            }
            else                                                        // Cookie doesn't exist
            {
                FillEntityDDL(null);                                    // Fill the list, no selection
                FillGLCodeDDL(null);
                FillPersonDDL(null);
                FillProjectDDL(null);
            }
            return;
        }

        // Save the current settings of the page idexes of the three gridviews in a cookie

        void SaveStaffPageIndexes()
        {
            HttpCookie staffPageIndexCookie = new HttpCookie(PortalConstants.CStaffPageIndexes);
            staffPageIndexCookie[PortalConstants.CStaffPIApp] = gvStaffApp.PageIndex.ToString(); // Copy App gridview value
            staffPageIndexCookie[PortalConstants.CStaffPIDep] = gvStaffDep.PageIndex.ToString(); // Copy Dep gridview value
            staffPageIndexCookie[PortalConstants.CStaffPIExp] = gvStaffExp.PageIndex.ToString(); // Copy Exp gridview value

            Response.Cookies.Add(staffPageIndexCookie);                 // Store a new cookie
        }

        // Read that cookie, if present, and find current setting of the value. If absent, supply default value of zero.

        int RestorePageIndex(string cookieName)
        {
            HttpCookie staffPageIndexCookie = Request.Cookies[PortalConstants.CStaffPageIndexes]; // Ask for the Staff Checkbox cookie
            if (staffPageIndexCookie != null)                           // If != the cookie is present
            {
                string cookieValue = staffPageIndexCookie[cookieName];  // Fetch cookie value, if any
                if (!string.IsNullOrEmpty(cookieValue))                 // If false, value is present
                {
                    try
                    {
                        return Convert.ToInt32(cookieValue);            // Convert the cookie value from string to integer
                    }
                    catch
                    {
                        ;                                               // Conversion failed. Return default
                    }
                }
            }
            return 0;                                                   // Return default value
        }
    }
}
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
                if (userID == null || userID == "")                     // If == UserID not specified. Check cookie.
                {
                    HttpCookie userInfoCookie = Request.Cookies[PortalConstants.CUserInfo]; // Find the User Info cookie
                    userID = userInfoCookie[PortalConstants.CUserID];   // Fetch UserID from cookie
                    if (userID == "")                                   // If == that's blank, too. Now we have an error
                        LogError.LogQueryStringError("ProjectDashboard", "Unable to find UserID in Query String or UserID Cookie. User is not logged in"); // Fatal error
                }
                litSavedUserID.Text = userID;                           // Save in a "faster" place for later reference

                string projID = Request.QueryString[PortalConstants.QSProjectID]; // Look on the query string for Project ID
                HttpCookie projectInfoCookie = Request.Cookies[PortalConstants.CProjectInfo]; // Find the Project Info cookie
                if (projID == null || projID == "")                     // If == Project ID not specified as Query String. Check cookie
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

                if (litSavedProjectRole.Text == ProjectRole.Coordinator.ToString()) // If == User is a Coordinator. Powerful
                    btnDepNew.Enabled = true;                           // Allow them to create a new Deposit

                RestoreCheckboxes();                                    // Restore more recent checkbox settings from a cookie
                LoadAllApps();
                LoadAllDeps();
                LoadAllExps();                                          // Load the grid view for display
            }
            return;
        }

        // The user changed one of the Filter selections check boxes. Recreate the grid views with new Filter criteria.

        protected void ckRFilters_Changed(object sender, EventArgs e)
        {
            AllAppView.PageIndex = 0;                                   // Go back to the first page
            LoadAllApps();                                              // Reload the GridView using new criteria
            ResetAppContext();                                          // No selected row, no live buttons

            AllDepView.PageIndex = 0;                                   // Go back to first page
            LoadAllDeps();                                              // Reload the GridView using new criteria
            ResetDepContext();                                          // No selected row, no live buttons

            AllExpView.PageIndex = 0;                                   // Go back to first page
            LoadAllExps();                                              // Reload the GridView using new criteria
            ResetExpContext();                                          // No selected row, no live buttons

            SaveCheckboxes();                                           // Save current checkbox settings in a cookie
            return;
        }

        // Start of APPROVAL section

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

        // The user changed selection in the Since drop down list. Recreate the grid view with new Since criteria.

        protected void ddlASince_SelectedIndexChanged(object sender, EventArgs e)
        {
            ckRFilters_Changed(sender, e);                           // Exactly the same action as Filter checkbox change
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button. Also, bold the Status cell if this user can operate on the row.

        protected void AllAppView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)            // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Approval";         // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.AllAppView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged

                // See if the Request awaits action by the User. Find the right cell by its ID. The cell contains the text of the AppState enum value.
                // 

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                AppState state = EnumActions.ConvertTextToAppState(label.Text); // Carefully convert back into enumeration type

                bool bling = false;                                         // Assume row will not be bolded
                if (litSavedProjectRole.Text == ProjectRole.Coordinator.ToString()) // If == User is a Coordinator
                {
                    if ((state == AppState.UnsubmittedByCoordinator) || (state == AppState.AwaitingCoordinator)) // If == Coordinator is working on Request
                        bling = true;                                       // Bold the bling
                }
                else if (litSavedProjectRole.Text == ProjectRole.ProjectDirector.ToString()) // If != User is not the Project Director
                {
                    if ((state == AppState.UnsubmittedByProjectDirector) || (state == AppState.AwaitingProjectDirector) || (state == AppState.Returned))
                        bling = true;                                          // If true Request is in interesting state for PD
                }
                else if (litSavedProjectRole.Text == ProjectRole.ProjectStaff.ToString()) // If != User is not the Project Staff
                {
                    if (state == AppState.UnsubmittedByProjectStaff)
                        bling = true;                                          // If true Request is in interesting state for PS
                }
                if (bling)                                                      // If true User can operate on Request.
                    e.Row.Cells[ProjectAppViewRow.CurrentStateDescRow].Font.Bold = true; // If we get here, User can act on the row. Bold Status cell.
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected. This code assumes that
        // Cells[4] contains the enum value (not enum desription) of CurrentState. It also assumes that only the New and View buttons are
        // enabled all the time.

        protected void AllAppView_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label label = (Label)AllAppView.SelectedRow.Cells[4].FindControl("lblCurrentState"); // Find the label control that contains Current State
            AppState state = EnumActions.ConvertTextToAppState(label.Text); // Convert back into enumeration type
            btnAppCopy.Enabled = true;                                  // If any Request is selected, the user can always copy it
            btnAppDelete.Enabled = false;                               // Assume User cannot Delete Request
            btnAppEdit.Enabled = false;                                 // Assume User cannot Edit Request
            btnAppReview.Enabled = false;                               // Assume User cannot Review Request
            btnAppView.Enabled = true;                                  // If any Request is selected, the user can always view it

            switch (state)
            {
                case AppState.UnsubmittedByCoordinator:
                    {
                        if (litSavedProjectRole.Text == ProjectRole.Coordinator.ToString()) // If == the user is a coordinator
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
                case AppState.AwaitingCoordinator:
                    {
                        if (litSavedProjectRole.Text == ProjectRole.Coordinator.ToString()) // If == the user is a coordinator
                        {
                            btnAppReview.Enabled = true;
                        }
                        break;
                    }
                case AppState.AwaitingTrustDirector:
                case AppState.AwaitingFinanceDirector:
                case AppState.AwaitingTrustExecutive:
                case AppState.Approved:
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

        // Flip a page of the grid view control

        protected void AllAppView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                AllAppView.PageIndex = e.NewPageIndex;                  // Propagate the desired page index
                LoadAllApps();                                          // Reload the grid view control
                ResetAppContext();                                      // No selected row, no live buttons after a page flip
            }
            return;

        }

        // Copy button pressed. Simple dispatch to EditApproval.

        protected void btnAppCopy_Click(object sender, EventArgs e)
        {
            string appID = FetchSelectedRowLabel(AllAppView);               // Extract the text of the control, which is AppID
            Response.Redirect(PortalConstants.URLEditApproval + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + appID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandCopy); // Start with existing request
        }

        // Delete button pressed. We do this here.

        protected void btnAppDelete_Click(object sender, EventArgs e)
        {
            int appID = QueryStringActions.ConvertID(FetchSelectedRowLabel(AllAppView)).Int; // Get ready for action with an int version of the Exp ID

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
                ckRFilters_Changed(sender, e);                   // Update the grid view and buttons for display
                litSuccessMessage.Text = "Approval deleted";            // Report success to our user
            }
        }

        // Edit button clicked. Fetch the ID of the selected row and dispatch to EditApproval.

        protected void btnAppEdit_Click(object sender, EventArgs e)
        {
            string appID = FetchSelectedRowLabel(AllAppView);               // Extract the text of the control, which is AppID
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
            string appID = FetchSelectedRowLabel(AllAppView);               // Extract the text of the control, which is DepID

            // Unconditionally send Request to ReviewRequest. It is possible that the user does not have the authority to review the Request in
            // its current state. But we'll let ReviewRequest display all the detail for the Dep and then deny editing.

            Response.Redirect(PortalConstants.URLReviewApproval + "?" + PortalConstants.QSRequestID + "=" + appID + "&" // Start with an existing request
                                              + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview + "&" // Review it
                                              + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done
        }

        protected void btnAppView_Click(object sender, EventArgs e)
        {
            string appID = FetchSelectedRowLabel(AllAppView);               // Extract the text of the control, which is AppID
            Response.Redirect(PortalConstants.URLEditApproval + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + appID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandView); // Start with existing request
        }


        // Fetch all of the Requests for this project, subject to further search constraints. Display in a GridView.
        // Find current project ID in listSavedProjectID

        void LoadAllApps()
        {
            int projectID = Convert.ToInt32(litSavedProjectID.Text);    // Fetch ID of current project as an int
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var pred = PredicateBuilder.True<App>();               // Initialize predicate to select from App table
                pred = pred.And(r => r.ProjectID == projectID && !r.Inactive); // Only active Apps from current project

                // Process check boxes for Deposit State. Combining checks is a little tricky because some of the checks overlap.

                int filters = 0;
                if (ckRAwaitingProjectStaff.Checked) filters = filters | 1; // Implies Approved and Returned
                if (ckRAwaitingCWStaff.Checked) filters = filters | 2;
                if (ckRApproved.Checked) filters = filters | 4;
                if (ckRReturned.Checked) filters = filters | 8;

                switch (filters)
                {
                    case 0:
                        {
                            pred = pred.And(r => r.CurrentState == AppState.ReservedForFutureUse); // Doesn't ever match
                            break;
                        }
                    case 1:                                             // Awaiting Project Staff
                    case 5:                                             // Awaiting Project Staff and Approved
                    case 9:                                             // Awaiting Project Staff and Returned
                    case 13:                                            // Awaiting Project Staff, Approved and Returned
                        {
                            pred = pred.And(r => r.CurrentState == AppState.UnsubmittedByCoordinator
                                              || r.CurrentState == AppState.UnsubmittedByProjectDirector
                                              || r.CurrentState == AppState.UnsubmittedByProjectStaff
                                              || r.CurrentState == AppState.AwaitingProjectDirector
                                              || r.CurrentState == AppState.Approved
                                              || r.CurrentState == AppState.Returned);
                            break;
                        }
                    case 2:                                             // Awaiting CW Staff
                        {
                            pred = pred.And(r => r.CurrentState == AppState.AwaitingTrustDirector
                                              || r.CurrentState == AppState.AwaitingFinanceDirector
                                              || r.CurrentState == AppState.AwaitingTrustExecutive
                                              || r.CurrentState == AppState.AwaitingCoordinator);
                            break;
                        }
                    case 4:                                             // Just Approved
                        {
                            pred = pred.And(r => r.CurrentState == AppState.Approved);
                            break;
                        }
                    case 6:                                             // Awaiting CW Staff and Approved
                        {
                            pred = pred.And(r => r.CurrentState == AppState.AwaitingTrustDirector
                                              || r.CurrentState == AppState.AwaitingFinanceDirector
                                              || r.CurrentState == AppState.AwaitingTrustExecutive
                                              || r.CurrentState == AppState.AwaitingCoordinator
                                              || r.CurrentState == AppState.Approved);
                            break;
                        }
                    case 8:                                             // Just Returned
                        {
                            pred = pred.And(r => r.CurrentState == AppState.Returned);
                            break;
                        }
                    case 10:                                            // Awaiting CW Staff and Returned
                        {
                            pred = pred.And(r => r.CurrentState == AppState.AwaitingTrustDirector
                                              || r.CurrentState == AppState.AwaitingFinanceDirector
                                              || r.CurrentState == AppState.AwaitingTrustExecutive
                                              || r.CurrentState == AppState.AwaitingCoordinator
                                              || r.CurrentState == AppState.Returned);
                            break;
                        }
                    case 12:                                            // Approved and Returned
                        {
                            pred = pred.And(r => r.CurrentState == AppState.Approved
                                              || r.CurrentState == AppState.Returned);
                            break;
                        }
                    case 14:                                            // Awaiting CW Staff, Approved and Returned
                        {
                            pred = pred.And(r => r.CurrentState == AppState.AwaitingTrustDirector
                                              || r.CurrentState == AppState.AwaitingFinanceDirector
                                              || r.CurrentState == AppState.AwaitingTrustExecutive
                                              || r.CurrentState == AppState.AwaitingCoordinator
                                              || r.CurrentState == AppState.Approved
                                              || r.CurrentState == AppState.Returned);
                            break;
                        }
                    case 3:                                             // Awaiting Project Staff, Awaiting CW Staff
                    case 7:                                             // Awaiting Project Staff, Awaiting CW Staff and Approved
                    case 11:                                            // Awaiting Project Staff, Awaiting CW Staff and Returned
                    case 15:                                            // Awaiting Project Staff, Awaiting CW Staff, Approved and Returned 
                    default:                                            // Don't need a predicate, just get all the rows
                        break;
                }

                // Process Since options

                string since = ddlRSince.SelectedValue;                 // Figure out which one was selected
                if (since != "ProjectCreation")                         // If != a specific Since option was selected. Need predicate.
                {
                    DateTime sinceDate = new DateTime();
                    switch (since)                                      // Break out by the selected item from the drop down list
                    {
                        case "BalanceDate":
                            {
                                sinceDate = Convert.ToDateTime(litBalance.Text); // Fetch the Balance Date (from earlier) and convert to binary
                                break;
                            }
                        case "Last30":
                            {
                                sinceDate = DateTime.Today.AddDays(-30); // Compute date 30 days ago
                                break;
                            }
                        case "Last90":
                            {
                                sinceDate = DateTime.Today.AddDays(-90); // Compute date 90 days ago
                                break;
                            }
                        case "Last12":
                            {
                                sinceDate = DateTime.Today.AddMonths(-12); // Compute date 12 months ago
                                break;
                            }
                        default:
                            LogError.LogInternalError("ProjectDashboard", string.Format("Invalid value '{0}' from Since DDL",
                                since)); // Fatal error
                            break;
                    }
                    pred = pred.And(r => r.CurrentTime >= sinceDate);   // Add date selector to the Where predicate
                }
                List<App> apps = context.Apps.AsExpandable().Where(pred).OrderByDescending(o => o.CurrentTime).ToList();
                // Do the query using the constructed predicate, sort the result, and create a list of App rows

                // From this list of Apps, build a list of rows for the AllAppView GridView

                List<ProjectAppViewRow> rows = new List<ProjectAppViewRow>(); // Create an empty list for the GridView control
                foreach (var r in apps)                                 // Fill the list row-by-row
                {
                    ProjectAppViewRow row = new ProjectAppViewRow()     // Empty row all ready to fill
                    {
                        RowID = r.AppID.ToString(),                     // Convert ID from int to string for easier retrieval later
                        CurrentTime = r.CurrentTime,                    // When request was last updated
                        AppTypeDesc = EnumActions.GetEnumDescription(r.AppType), // Convert enum version to English version for display
                        Description = r.Description,                    // Free text description of deposit
                        CurrentState = r.CurrentState,                  // Load enum version for use when row is selected. But not visible
                        CurrentStateDesc = EnumActions.GetEnumDescription(r.CurrentState) // Convert enum version to English version for display

                    };
                    rows.Add(row);                                      // Add the filled-in row to the list of rows
                }
                AllAppView.DataSource = rows;                           // Give it to the GridView control
                AllAppView.DataBind();                                  // And get it in gear

                NavigationActions.EnableGridViewNavButtons(AllAppView); // Enable appropriate nav buttons based on page count
            }
            return;
        }

        // Start of DEPOSIT section


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

        // The user changed selection in the Since drop down list. Recreate the grid view with new Since criteria.

        protected void ddlDSince_SelectedIndexChanged(object sender, EventArgs e)
        {
            ckRFilters_Changed(sender, e);                           // Exactly the same action as Filter checkbox change
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }


        // Flip a page of the grid view control

        protected void AllDepView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                AllDepView.PageIndex = e.NewPageIndex;                  // Propagate the desired page index
                LoadAllDeps();                                          // Reload the grid view control
                ResetDepContext();                                      // No selected row, no live buttons after a page flip
            }
            return;
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button. Also, bold the Status cell if this user can operate on the row.

        protected void AllDepView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)            // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                e.Row.ToolTip = "Click to select this Deposit";         // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.AllDepView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged

                // See if the Request awaits action by the User. Find the right cell by its ID. The cell contains the text of the DepState enum value.
                // For Deposits, the only such situation is for a Project Director and a Deposit Request in "Awaiting Project Director" or "Returned" state.

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                DepState state = EnumActions.ConvertTextToDepState(label.Text); // Carefully convert back into enumeration type

                bool bling = false;                                         // Assume row will not be bolded
                if (litSavedProjectRole.Text == ProjectRole.Coordinator.ToString()) // If == User is a Coordinator
                {
                    if ((state == DepState.UnsubmittedByCoordinator) || (state == DepState.Returned)) // If == Coordinator is working on Request
                        bling = true;                                       // Bold the row
                }
                else if (litSavedProjectRole.Text == ProjectRole.ProjectDirector.ToString()) // If != User is not the Project Director
                {
                    if ((state == DepState.AwaitingProjectDirector))        // If true Request is in interesting state for Project Director
                        bling = true;                                       // Bold the row
                }                                                      
                if (bling)                                                  // If true User can operate on Request.
                    e.Row.Cells[ProjectDepViewRow.CurrentStateDescRow].Font.Bold = true; // If we get here, User can act on the row. Bold Status cell.
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected. This code assumes that only the View buttons are
        // enabled all the time. Every other button is nuanced based on role. We don't mess with the New button. It works independently of
        // which Request is selected.

        protected void AllDepView_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label label = (Label)AllDepView.SelectedRow.FindControl("lblCurrentState"); // Find the label control that contains Current State
            DepState state = EnumActions.ConvertTextToDepState(label.Text); // Carefully convert back into enumeration type

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
                btnDepCopy.Enabled = true;                              // No match what state, we can copy the request
                if (state == DepState.AwaitingProjectDirector)          // If == the Request is waiting our action
                    btnDepReview.Enabled = true;                        // A Project Director can review this Request
                return;                                                 // Otherwise, Project Director is powerless
            }

            if (litSavedProjectRole.Text == ProjectRole.Coordinator.ToString()) // If == User is a Project Coordinator. Powerful
            {
                btnDepCopy.Enabled = true;                              // No match what state, we can copy the request
                if (state == DepState.UnsubmittedByCoordinator)         // If == the Request is under construction by us
                {
                    btnDepDelete.Enabled = true;                        // Turn on other buttons
                    btnDepEdit.Enabled = true;                          // that make sense in this state
                }
            }
            return;
        }

        protected void btnDepCopy_Click(object sender, EventArgs e)
        {
            string depID = FetchSelectedRowLabel(AllDepView);               // Extract the text of the control, which is ExpID
            Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + depID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandCopy); // Start with existing request
        }

        // Delete button pushed. Clean up all the parts of the Deposit Request including supporting documents and history.

        protected void btnDepDelete_Click(object sender, EventArgs e)
        {
            int depID = QueryStringActions.ConvertID(FetchSelectedRowLabel(AllDepView)).Int; // Extract the text of the control, which is DepID
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
                ckRFilters_Changed(sender, e);                          // Update the grid view and buttons for display
                litSuccessMessage.Text = "Deposit deleted";             // Report success to our user
            }
            return;
        }

        // Edit button clicked. Fetch the DepID of the selected row and dispatch to EditdEPOSIT.

        protected void btnDepEdit_Click(object sender, EventArgs e)
        {
            string depID = FetchSelectedRowLabel(AllDepView);               // Extract the text of the control, which is DepID
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
            string depID = FetchSelectedRowLabel(AllDepView);               // Extract the text of the control, which is DepID

            // Unconditionally send Dep to ReviewRequest. It is possible that the user does not have the authority to review the Dep in
            // its current state. But we'll let ReviewRequest display all the detail for the Dep and then deny editing.

            Response.Redirect(PortalConstants.URLReviewDeposit + "?" + PortalConstants.QSRequestID + "=" + depID + "&" // Start with an existing request
                                              + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview + "&" // Review it
                                              + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done
        }

        // View button clicked.
        protected void btnDepView_Click(object sender, EventArgs e)
        {
            string depID = FetchSelectedRowLabel(AllDepView);               // Extract the text of the control, which is DepID
            Response.Redirect(PortalConstants.URLEditDeposit + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + depID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandView); // Start with existing request
        }

        void LoadAllDeps()
        {
            int projectID = Convert.ToInt32(litSavedProjectID.Text);    // Fetch ID of current project as an int
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var pred = PredicateBuilder.True<Dep>();               // Initialize predicate to select from Dep table
                pred = pred.And(r => r.ProjectID == projectID && !r.Inactive); // Only active Deps from current project

                // Process check boxes for Deposit State. Combining checks is a little tricky because some of the checks overlap.

                int filters = 0;
                if (ckRAwaitingProjectStaff.Checked) filters = filters | 1;
                if (ckRAwaitingCWStaff.Checked) filters = filters | 2;
                if (ckRApproved.Checked) filters = filters | 4;
                if (ckRReturned.Checked) filters = filters | 8;

                switch (filters)
                {
                    case 0:
                        {
                            pred = pred.And(r => r.CurrentState == DepState.ReservedForFutureUse); // Doesn't ever match
                            break;
                        }
                    case 1:                                         // Awaiting Project Staff (includes DepositComplete and Returned)
                    case 5:                                         // Awaiting Project Staff and DepositComplete
                    case 9:                                         // Awaiting Project Staff and Returned
                    case 13:                                        // Awaiting Project Staff, DepositComplete and Returned
                        {
                            pred = pred.And(r => r.CurrentState == DepState.UnsubmittedByCoordinator
                                              || r.CurrentState == DepState.AwaitingProjectDirector
                                              || r.CurrentState == DepState.DepositComplete
                                              || r.CurrentState == DepState.Returned);
                            break;
                        }
                    case 2:                                         // Awaiting CW Staff
                        {
                            pred = pred.And(r => r.CurrentState == DepState.AwaitingTrustDirector
                                              || r.CurrentState == DepState.AwaitingFinanceDirector
                                              || r.CurrentState == DepState.ApprovedReadyToDeposit);
                            break;
                        }
                    case 4:                                         // DepositComplete
                        {
                            pred = pred.And(r => r.CurrentState == DepState.DepositComplete);
                            break;
                        }
                    case 6:                                         // Awaiting CW Staff and DepositComplete
                        {
                            pred = pred.And(r => r.CurrentState == DepState.AwaitingTrustDirector
                                              || r.CurrentState == DepState.AwaitingFinanceDirector
                                              || r.CurrentState == DepState.ApprovedReadyToDeposit
                                              || r.CurrentState == DepState.DepositComplete);
                            break;
                        }
                    case 8:                                         // Returned
                        {
                            pred = pred.And(r => r.CurrentState == DepState.Returned);
                            break;
                        }
                    case 10:                                        // Awaiting CW Staff and Returned
                        {
                            pred = pred.And(r => r.CurrentState == DepState.AwaitingTrustDirector
                                              || r.CurrentState == DepState.AwaitingFinanceDirector
                                              || r.CurrentState == DepState.ApprovedReadyToDeposit
                                              || r.CurrentState == DepState.Returned);
                            break;
                        }
                    case 12:                                        // DepositComplete and Returned
                        {
                            pred = pred.And(r => r.CurrentState == DepState.DepositComplete
                                              || r.CurrentState == DepState.Returned);
                            break;
                        }
                    case 14:                                        // Awaiting CW Staff, DepositComplete and Returned
                        {
                            pred = pred.And(r => r.CurrentState == DepState.AwaitingTrustDirector
                                              || r.CurrentState == DepState.AwaitingFinanceDirector
                                              || r.CurrentState == DepState.ApprovedReadyToDeposit
                                              || r.CurrentState == DepState.DepositComplete
                                              || r.CurrentState == DepState.Returned);
                            break;
                        }
                    case 3:                                             // Awaiting Project Staff, Awaiting CW Staff
                    case 7:                                             // Awaiting Project Staff, Awaiting CW Staff, Deposit Complete
                    case 11:                                            // Awaiting Project Staff, Awaiting CW Staff, Returned
                    case 15:                                            // Awaiting Project Staff, Awaiting CW Staff, Deposit Complete, Returned
                    default:                                            // Don't need a predicate to get all the rows
                        break;
                }

                // Process Since options

                string since = ddlRSince.SelectedValue;                 // Figure out which one was selected
                if (since != "ProjectCreation")                         // If != a specific Since option was selected. Need predicate.
                {
                    DateTime sinceDate = new DateTime();
                    switch (since)                                      // Break out by the selected item from the drop down list
                    {
                        case "BalanceDate":
                            {
                                sinceDate = Convert.ToDateTime(litBalance.Text); // Fetch the Balance Date (from earlier) and convert to binary
                                break;
                            }
                        case "Last30":
                            {
                                sinceDate = DateTime.Today.AddDays(-30); // Compute date 30 days ago
                                break;
                            }
                        case "Last90":
                            {
                                sinceDate = DateTime.Today.AddDays(-90); // Compute date 90 days ago
                                break;
                            }
                        case "Last12":
                            {
                                sinceDate = DateTime.Today.AddMonths(-12); // Compute date 12 months ago
                                break;
                            }
                        default:
                            LogError.LogInternalError("ProjectDashboard", string.Format("Invalid value '{0}' from Since DDL",
                                since)); // Fatal error
                            break;
                    }
                    pred = pred.And(r => r.CurrentTime >= sinceDate);   // Add date selector to the Where predicate
                }
                List<Dep> deps = context.Deps.AsExpandable().Where(pred).OrderByDescending(o => o.CurrentTime).ToList();
                    // Do the query using the constructed predicate, sort the result, and create a list of Dep rows

                // From this list of Deps, build a list of rows for the AllExpView GridView

                List<ProjectDepViewRow> rows = new List<ProjectDepViewRow>(); // Create an empty list for the GridView control
                foreach (var r in deps)                                 // Fill the list row-by-row
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
                        CurrentStateDesc = EnumActions.GetEnumDescription(r.CurrentState) // Convert enum version to English version for display
                        
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
                    rows.Add(row);                                      // Add the filled-in row to the list of rows
                }
                AllDepView.DataSource = rows;                           // Give it to the GridView control
                AllDepView.DataBind();                                  // And get it in gear

                NavigationActions.EnableGridViewNavButtons(AllDepView); // Enable appropriate nav buttons based on page count
            }
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
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }

        void ExpandExpPanel()
        {
            pnlExp.Visible = true;                                      // The panel body becomes visible
            btnExpCollapse.Visible = true;                              // The collapse button becomes visible
            btnExpExpand.Visible = false;                               // The expand button becomes invisible
            return;
        }

        // The user changed selection in the Since drop down list. Recreate the grid view with new Since criteria.

        protected void ddlESince_SelectedIndexChanged(object sender, EventArgs e)
        {
            ckRFilters_Changed(sender, e);                        // Exactly the same action as Filter checkbox change
            SaveCheckboxes();                                               // Save current checkbox settings in a cookie
            return;
        }

        // Flip a page of the grid view control

        protected void AllExpView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                AllExpView.PageIndex = e.NewPageIndex;                  // Propagate the desired page index
                LoadAllExps();                                          // Reload the grid view control
                ResetExpContext();                                      // No selected row, no live buttons after a page flip
            }
            return;
        }

        // Invoked for each row as it gets its content data bound. Make the row sensitive to mouseover and click
        // thereby letting us select the row without a Select button

        protected void AllExpView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)            // If == this is indeed a row of our GridView control
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';"; // When pointer is over a row, change the pointer
                //                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';"; // No need to highlight anything this way
                e.Row.ToolTip = "Click to select this Expense";         // Establish tool tip during flyover
                e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.AllExpView, "Select$" + e.Row.RowIndex);
                // Mark the row "Selected" on a click. That will fire SelectedIndexChanged
                // See if the Request awaits action by the User. Find the right cell by its ID. The cell contains the text of the DepState enum value.
                // For Deposits, the only such situation is for a Project Director and a Deposit Request in "Awaiting Project Director" or "Returned" state.

                Label label = (Label)e.Row.FindControl("lblCurrentState");  // Find the label control that contains Current State in this row
                ExpState state = EnumActions.ConvertTextToExpState(label.Text); // Carefully convert back into enumeration type

                bool bling = false;                                         // Assume row will not be bolded
                if (litSavedProjectRole.Text == ProjectRole.Coordinator.ToString()) // If == User is a Coordinator
                {
                    if (state == ExpState.UnsubmittedByCoordinator)         // If == Coordinator is working on Request
                        bling = true;                                       // Bold the bling
                }
                else if (litSavedProjectRole.Text == ProjectRole.ProjectDirector.ToString()) // If != User is not the Project Director
                {
                    if ((state == ExpState.UnsubmittedByProjectDirector) || (state == ExpState.AwaitingProjectDirector) || (state == ExpState.Returned))
                        bling = true;                                          // If true Request is in interesting state for PD
                }
                else if (litSavedProjectRole.Text == ProjectRole.ProjectStaff.ToString()) // If != User is not the Project Staff
                {
                    if (state == ExpState.UnsubmittedByProjectStaff)
                        bling = true;                                          // If true Request is in interesting state for PS
                }
                if (bling)                                                      // If true User can operate on Request.
                    e.Row.Cells[ProjectExpViewRow.CurrentStateDescRow].Font.Bold = true; // If we get here, User can act on the row. Bold Status cell.
            }
            return;
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected. This code assumes that
        // Cells[4] contains the enum value (not enum desription) of CurrentState. It also assumes that only the New and View buttons are
        // enabled all the time.

        protected void AllExpView_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label label = (Label)AllExpView.SelectedRow.Cells[4].FindControl("lblCurrentState"); // Find the label control that contains Current State
            ExpState state = EnumActions.ConvertTextToExpState(label.Text); // Convert back into enumeration type
            btnExpCopy.Enabled = true;                                  // If any Request is selected, the user can always copy it
            btnExpDelete.Enabled = false;                               // Assume User cannot Delete Request
            btnExpEdit.Enabled = false;                                 // Assume User cannot Edit Request
            btnExpReview.Enabled = false;                               // Assume User cannot Review Request
            btnExpView.Enabled = true;                                  // If any Request is selected, the user can always view it

            switch (state)
            {
                case ExpState.UnsubmittedByCoordinator:
                    {
                        if (litSavedProjectRole.Text == ProjectRole.Coordinator.ToString()) // If == the user is a coordinator
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
                case ExpState.AwaitingTrustDirector:
                case ExpState.AwaitingFinanceDirector:
                case ExpState.AwaitingTrustExecutive:
                case ExpState.Approved:
                case ExpState.PaymentSent:
                case ExpState.Paid:
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

        // Copy button clicked. Conclude what we've been working on, then hit the road.

        protected void btnExpCopy_Click(object sender, EventArgs e)
        {
            string expID = FetchSelectedRowLabel(AllExpView);               // Extract the text of the control, which is ExpID
            Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + expID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandCopy); // Start with existing request
        }

        // Delete button clicked. Wipe out the Request and it's associated rows and files.

        protected void btnExpDelete_Click(object sender, EventArgs e)
        {
            int expID = QueryStringActions.ConvertID(FetchSelectedRowLabel(AllExpView)).Int; // Get ready for action with an int version of the Exp ID

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

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
                        "Error deleting Exp, SupportingDoc and ExpHistory rows or deleting Supporting Document"); // Fatal error
                }
                ckRFilters_Changed(sender, e);                          // Update the grid view and buttons for display
                litSuccessMessage.Text = "Expense deleted";             // Report success to our user
            }
       }

        // Edit button clicked. Fetch the ExpID of the selected row and dispatch to EditExpense.

        protected void btnExpEdit_Click(object sender, EventArgs e)
        {
            string expID = FetchSelectedRowLabel(AllExpView);               // Extract the text of the control, which is ExpID
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
            string expID = FetchSelectedRowLabel(AllExpView);               // Extract the text of the control, which is ExpID

            // Unconditionally send Exp to ReviewRequest. It is possible that the user does not have the authority to review the exp in
            // its current state. But we'll let ReviewRequest display all the detail for the Exp and then deny editing.

            Response.Redirect(PortalConstants.URLReviewExpense + "?" + PortalConstants.QSRequestID + "=" + expID + "&" // Start with an existing request
                                              + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview + "&" // Review it
                                              + PortalConstants.QSReturn + "=" + PortalConstants.URLProjectDashboard); // Return to this page when done

        }

        // View button clicked. 

        protected void btnExpView_Click(object sender, EventArgs e)
        {
            string expID = FetchSelectedRowLabel(AllExpView);               // Extract the text of the control, which is ExpID
            Response.Redirect(PortalConstants.URLEditExpense + "?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSRequestID + "=" + expID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandView); // Start with existing request
        }

        // Fetch all of the Requests for this project, subject to further search constraints. Display in a GridView.
        // Find current project ID in listSavedProjectID

        void LoadAllExps()
        {
            int projectID = Convert.ToInt32(litSavedProjectID.Text);    // Fetch ID of current project as an int
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var pred = PredicateBuilder.True<Exp>();               // Initialize predicate to select from Exp table
                pred = pred.And(r => r.ProjectID == projectID && !r.Inactive); // Only active Exps from current project

                // Process check boxes for Expense State. Combining checks is a little tricky because some of the checks overlap.

                int filters = 0;
                if (ckRAwaitingProjectStaff.Checked) filters = filters | 1; // Awaiting Project Staff
                if (ckRAwaitingCWStaff.Checked) filters = filters | 2;      // Awaiting CW Staff
                if (ckRApproved.Checked) filters = filters | 4;             // Paid
                if (ckRReturned.Checked) filters = filters | 8;             // Returned

                switch (filters)
                {
                    case 0:
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.ReservedForFutureUse); // Doesn't ever match
                            break;
                        }
                    case 1:                                             // Awaiting Project Staff (includes Paid and Returned)
                    case 5:                                             // Awaiting Project Staff and Paid
                    case 9:                                             // Awaiting Project Staff and Returned
                    case 13:                                            // Awaiting Project Staff, Paid and Returned
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.UnsubmittedByCoordinator
                                              || r.CurrentState == ExpState.UnsubmittedByProjectDirector
                                              || r.CurrentState == ExpState.UnsubmittedByProjectStaff
                                              || r.CurrentState == ExpState.AwaitingProjectDirector
                                              || r.CurrentState == ExpState.Paid
                                              || r.CurrentState == ExpState.Returned);
                            break;
                        }
                    case 2:                                             // Awaiting CW Staff
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.AwaitingTrustDirector
                                              || r.CurrentState == ExpState.AwaitingFinanceDirector
                                              || r.CurrentState == ExpState.AwaitingTrustExecutive
                                              || r.CurrentState == ExpState.Approved
                                              || r.CurrentState == ExpState.PaymentSent);
                            break;
                        }
                    case 4:                                             // Paid
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.Paid);
                            break;
                        }
                    case 6:                                             // Awaiting CW Staff and Paid
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.AwaitingTrustDirector
                                              || r.CurrentState == ExpState.AwaitingFinanceDirector
                                              || r.CurrentState == ExpState.AwaitingTrustExecutive
                                              || r.CurrentState == ExpState.Approved
                                              || r.CurrentState == ExpState.PaymentSent
                                              || r.CurrentState == ExpState.Paid);
                            break;
                        }
                    case 8:                                             // Returned
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.Returned);
                            break;
                        }
                    case 10:                                            // Awaiting CW Staff and Returned
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.AwaitingTrustDirector
                                              || r.CurrentState == ExpState.AwaitingFinanceDirector
                                              || r.CurrentState == ExpState.AwaitingTrustExecutive
                                              || r.CurrentState == ExpState.Approved
                                              || r.CurrentState == ExpState.PaymentSent
                                              || r.CurrentState == ExpState.Returned);
                            break;
                        }
                    case 12:                                            // Paid and Returned
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.Paid
                                              || r.CurrentState == ExpState.Returned);
                            break;
                        }
                    case 14:                                            // Awaiting CW Staff, Paid and Returned
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.AwaitingTrustDirector
                                              || r.CurrentState == ExpState.AwaitingFinanceDirector
                                              || r.CurrentState == ExpState.AwaitingTrustExecutive
                                              || r.CurrentState == ExpState.Approved
                                              || r.CurrentState == ExpState.PaymentSent
                                              || r.CurrentState == ExpState.Paid
                                              || r.CurrentState == ExpState.Returned);
                            break;
                        }
                    case 3:                                             // Awaiting Project Staff and Awaiting CW Staff
                    case 7:                                             // Awaiting Project Staff, Awaiting CW Staff and Paid
                    case 11:                                            // Awaiting Project Staff, Awaiting CW Staff and Returned
                    case 15:                                            // Awaiting Project Staff, Awaiting CW Staff, Paid and Returned
                    default:                                            // Don't need a predicate to get all the rows
                        break;
                }

                // Process Since options

                string since = ddlRSince.SelectedValue;                 // Figure out which one was selected
                if (since != "ProjectCreation")                         // If != a specific Since option was selected. Need predicate.
                {
                    DateTime sinceDate = new DateTime();
                    switch (since)                                      // Break out by the selected item from the drop down list
                    {
                        case "BalanceDate":
                            {
                                sinceDate = Convert.ToDateTime(litBalance.Text); // Fetch the Balance Date (from earlier) and convert to binary
                                break;
                            }
                        case "Last30":
                            {
                                sinceDate = DateTime.Today.AddDays(-30); // Compute date 30 days ago
                                break;
                            }
                        case "Last90":
                            {
                                sinceDate = DateTime.Today.AddDays(-90); // Compute date 90 days ago
                                break;
                            }
                        case "Last12":
                            {
                                sinceDate = DateTime.Today.AddMonths(-12); // Compute date 12 months ago
                                break;
                            }
                        default:
                            LogError.LogInternalError("ProjectDashboard", string.Format("Invalid value '{0}' from Since DDL", 
                                since)); // Fatal error
                            break;
                    }
                    pred = pred.And(r => r.CurrentTime >= sinceDate);   // Add date selector to the Where predicate
                }
                List<Exp> exps = context.Exps.AsExpandable().Where(pred).OrderByDescending(o => o.CurrentTime).ToList(); 
                // Do the query using the constructed predicate, sort the result, and create a list of Exp rows

                // From this list of Exps, build a list of rows for the AllExpView GridView

                List<ProjectExpViewRow> rows = new List<ProjectExpViewRow>(); // Create an empty list for the GridView control
                foreach (var r in exps)                                // Fill the list row-by-row
                {
                    ProjectExpViewRow row = new ProjectExpViewRow()   // Empty row all ready to fill
                    { 
                        RowID = r.ExpID.ToString(),                    // Convert ID from int to string for easier retrieval later
                        CurrentTime = r.CurrentTime,                    // When request was last updated
                        ExpTypeDesc = EnumActions.GetEnumDescription(r.ExpType), // Convert enum version to English version for display
                        Amount = ExtensionActions.LoadDecimalIntoTxt(r.Amount), // Carefully load decimal amount into text field
                        CurrentState = r.CurrentState,                  // Load enum version for use when row is selected
                        CurrentStateDesc = EnumActions.GetEnumDescription(r.CurrentState) // Convert enum version to English version for display
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
                    rows.Add(row);                                      // Add the filled-in row to the list of rows
                }
                AllExpView.DataSource = rows;                           // Give it to the GridView control
                AllExpView.DataBind();                                  // And get it in gear

                NavigationActions.EnableGridViewNavButtons(AllExpView); // Enable appropriate nav buttons based on page count
            }
        }

        // We no longer have a selected Request. Return to the first page of the GridView and adjust buttons

        void ResetAppContext()
        {
            AllAppView.SelectedIndex = -1;                              // No selected row any more
            btnAppCopy.Enabled = false;
            btnAppDelete.Enabled = false;
            btnAppEdit.Enabled = false;                                 // Therefore, the buttons don't work
                                                                        // Leave New button in its former state
            btnAppReview.Enabled = false;
            btnAppView.Enabled = false;
        }

        void ResetDepContext()
        {
            AllDepView.SelectedIndex = -1;                              // No selected row any more
            btnDepCopy.Enabled = false;
            btnDepDelete.Enabled = false;
            btnDepEdit.Enabled = false;                                 // Therefore, the buttons don't work
                                                                        // Leave New button in its former state
            btnDepReview.Enabled = false;
            btnDepView.Enabled = false;
        }

        void ResetExpContext()
        {
            AllExpView.SelectedIndex = -1;                              // No selected row any more
            btnExpCopy.Enabled = false;
            btnExpDelete.Enabled = false;
            btnExpEdit.Enabled = false;                                 // Therefore, the buttons don't work
            btnExpReview.Enabled = false;
            btnExpView.Enabled = false;
        }

        // A little common fragment that pulls the expID from the selected row of the GridView control and makes sure it's not blank.

        string FetchSelectedRowLabel(GridView allview)
        {
            Label label = (Label)allview.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains DepID or ExpID
            string ID = label.Text;                                     // Extract the text of the control, which is ExpID
            if (ID == "")
                LogError.LogInternalError("ProjectDashboard", string.Format(
                    "Unable to find Deposit/Expense ID '{0}' from selected GridView row in database", ID)); // Fatal error
            return ID;
        }

        // Save the current settings of the checkboxes in a (big) cookie

        void SaveCheckboxes()
        {
            HttpCookie projectCheckboxesCookie = new HttpCookie(PortalConstants.CProjectCheckboxes);

            projectCheckboxesCookie[PortalConstants.CProjectCkRAwaitingProjectStaff] = ckRAwaitingProjectStaff.Checked.ToString();
            projectCheckboxesCookie[PortalConstants.CProjectCkRAwaitingCWStaff] = ckRAwaitingCWStaff.Checked.ToString();
            projectCheckboxesCookie[PortalConstants.CProjectCkRApproved] = ckRApproved.Checked.ToString();
            projectCheckboxesCookie[PortalConstants.CProjectCkRReturned] = ckRReturned.Checked.ToString();


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
                ckRAwaitingProjectStaff.Checked = false;                        // Assume box should be unchecked
                if (projectCheckboxesCookie[PortalConstants.CProjectCkRAwaitingProjectStaff] == "True") ckRAwaitingProjectStaff.Checked = true;

                ckRAwaitingCWStaff.Checked = false;                             // Assume box should be unchecked
                if (projectCheckboxesCookie[PortalConstants.CProjectCkRAwaitingCWStaff] == "True") ckRAwaitingCWStaff.Checked = true;

                ckRApproved.Checked = false;                                // Assume box should be unchecked
                if (projectCheckboxesCookie[PortalConstants.CProjectCkRApproved] == "True") ckRApproved.Checked = true;

                ckRReturned.Checked = false;                                    // Assume box should be unchecked
                if (projectCheckboxesCookie[PortalConstants.CProjectCkRReturned] == "True") ckRReturned.Checked = true;

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
        }
    }
}
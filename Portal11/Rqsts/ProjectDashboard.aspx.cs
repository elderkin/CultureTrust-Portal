using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal11.Models;
using Portal11.Logic;
using System.IO;
using System.Data;
using Portal11.ErrorLog;

namespace Portal11.Rqsts
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
                    decimal total = StateActions.LoadGrantsList(projIDint, lstRestrictedGrants); // Load list of grants, return total current funds
                    litRestrictedGrants.Text = total.ToString("C");     // Convert total to currency string and display
                }
                //TODO
                LoadAllExps();                                         // Load the grid view for display
            }
        }

        // Collapse/Expand the panels of Expenses and Deposits

        protected void btnExpCollapse_Click(object sender, EventArgs e)
        {
            CollapseExpPanel();
        }

        void CollapseExpPanel()
        {
            pnlExp.Visible = false;                                     // The panel body becomes invisible
            btnExpCollapse.Visible = false;                             // The collapse button becomes invisible
            btnExpExpand.Visible = true;                                // The expand button becomes visible
        }

        protected void btnExpExpand_Click(object sender, EventArgs e)
        {
            ExpandExpPanel();
            LoadAllExps();                                             // Refresh the contents of the gridview
        }

        void ExpandExpPanel()
        {
            pnlExp.Visible = true;                                      // The panel body becomes visible
            btnExpCollapse.Visible = true;                              // The collapse button becomes visible
            btnExpExpand.Visible = false;                               // The expand button becomes invisible
        }

        //protected void btnDepositsCollapse_Click(object sender, EventArgs e)
        //{
        //    CollapseDepositsPanel();                                    // Do the heavy lifting
        //}

        //void CollapseDepositsPanel()
        //{
        //    pnlDeposits.Visible = false;
        //    btnDepositsCollapse.Visible = false;
        //    btnDepositsExpand.Visible = true;
        //}

        //protected void btnDepositsExpand_Click(object sender, EventArgs e)
        //{
        //    ExpandDepositsPanel();
        //    LoadAllDeposits();
        //}

        //void ExpandDepositsPanel()
        //{
        //    pnlDeposits.Visible = true;
        //    btnDepositsCollapse.Visible = true;
        //    btnDepositsExpand.Visible = false;
        //}

        // The user changed selection in the Since drop down list. Recreate the grid view with new Since criteria.

        protected void ddlESince_SelectedIndexChanged(object sender, EventArgs e)
        {
            ckEFilters_CheckedChanged(sender, e);                        // Exactly the same action as Filter checkbox change
        }

        // The user clicked or unclicked one of the check boxes. Recreate the grid view with new Filter criteria.

        protected void ckEFilters_CheckedChanged(object sender, EventArgs e)
        {
            LoadAllExps();                                             // Reload the GridView using new criteria
            AllExpView.PageIndex = 0;                                   // Go back to the first page
            AllExpView.SelectedIndex = -1;                              // No selected row any more
            btnExpEdit.Enabled = false;                                 // Therefore, the buttons don't work
            btnExpCopy.Enabled = false;
            btnExpDelete.Enabled = false;
            btnExpReview.Enabled = false;
            btnExpView.Enabled = false;
        }

        // Flip a page of the grid view control

        protected void AllExpView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex >= 0)                                    // If >= a value that we can handle
            {
                AllExpView.PageIndex = e.NewPageIndex;                  // Propagate the desired page index
                LoadAllExps();                                         // Reload the grid view control
                AllExpView.SelectedIndex = -1;                          // No selected row after a page flip
            }
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
            }
        }

        // The user has actually clicked on a row. Enable the buttons that only make sense when a row is selected. This code assumes that
        // Cells[4] contains the enum value (not enum desription) of CurrentState. It also assumes that only the New and View buttons are
        // enabled all the time.

        protected void AllExpView_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label label = (Label)AllExpView.SelectedRow.Cells[4].FindControl("lblCurrentState"); // Find the label control that contains Current State
            ExpState state = (ExpState)Enum.Parse(typeof(ExpState), label.Text, true); // Convert back into enumeration type

            switch (state)
            {
                case ExpState.Unsubmitted:
                    {
                        btnExpDelete.Enabled = true;
                        btnExpEdit.Enabled = true;
                        if (litSavedProjectRole.Text == UserRole.ProjectDirector.ToString()) // If == user is a Project Director, can Review
                            btnExpReview.Enabled = true;
                        else
                            btnExpReview.Enabled = false;
                        break;
                    }
                case ExpState.AwaitingProjectDirector:
                    {
                        btnExpDelete.Enabled = false;
                        btnExpEdit.Enabled = false;
                        if (litSavedProjectRole.Text == UserRole.ProjectDirector.ToString()) // If == user is a Project Director, can Review
                            btnExpReview.Enabled = true;
                        else
                            btnExpReview.Enabled = false;
                        break;
                    }
                case ExpState.AwaitingTrustDirector:
                case ExpState.AwaitingFinanceDirector:
                case ExpState.AwaitingTrustExecutive:
                case ExpState.Approved:
                case ExpState.CheckMailed:
                case ExpState.Paid:
                    {
                        btnExpDelete.Enabled = false;
                        btnExpEdit.Enabled = false;
                        btnExpReview.Enabled = false;
                        break;
                    }
                case ExpState.Returned:
                    {
                        btnExpDelete.Enabled = false;
                        if (litSavedProjectRole.Text == UserRole.ProjectDirector.ToString()) // If == user is a Project Director, can Edit
                            btnExpEdit.Enabled = true;
                        else
                            btnExpEdit.Enabled = false;
                        btnExpReview.Enabled = false;
                        break;
                    }
                default:
                    LogError.LogInternalError("ProjectDashboard", string.Format("Invalid ExpState '{0}' from database",
                        state)); // Fatal error
                    break;
            }
            btnExpCopy.Enabled = true;                                  // If any Request is selected, the user can always copy it
            btnExpView.Enabled = true;                                  // If any Request is selected, the user can always view it
            //if (state == ExpState.Unsubmitted)                          // If == Request is editable by user
            //{
            //    btnExpEdit.Enabled = true;
            //    btnExpCopy.Enabled = true;
            //    btnExpDelete.Enabled = true;
            //}
            //else if (state == ExpState.AwaitingProjectDirector)         // If == Request is viewable and submittable by user
            //{
            //    btnExpEdit.Enabled = false;
            //    btnExpCopy.Enabled = false;
            //    btnExpDelete.Enabled = false;
            //}
            //else if (state == ExpState.Returned)                       // If == Viewable but not editable
            //{
            //    btnExpEdit.Enabled = false;
            //    btnExpCopy.Enabled = true;
            //    btnExpDelete.Enabled = true;
            //}
            //else                                                        // All other Requests are not editable
            //{
            //    btnExpEdit.Enabled = false;
            //    btnExpCopy.Enabled = false;
            //    btnExpDelete.Enabled = false;
            //}
        }

        // Copy button clicked. Conclude what we've been working on, then hit the road.

        protected void btnExpCopy_Click(object sender, EventArgs e)
        {
            Label label = (Label)AllExpView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains ExpID
            string expID = label.Text;                                  // Extract the text of the control, which is ExpID
            if (expID == "")
                LogError.LogInternalError("ProjectDashboard", string.Format(
                    "Unable to find Expense ID '{0}' from selected GridView row in database", expID)); // Fatal error

            Response.Redirect("EditExpense?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSExpID + "=" + expID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandCopy); // Start with existing request
        }

        // Delete button clicked. Wipe out the Exp and it's associated rows and files.

        protected void btnExpDelete_Click(object sender, EventArgs e)
        {
            Label label = (Label)AllExpView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains ExpID
            string expID = label.Text;                                  // Extract the text of the control, which is ExpID
            if (expID == "")
                LogError.LogInternalError("ProjectDashboard", string.Format(
                    "Unable to find Expense ID '{0}' from selected GridView row in database", expID)); // Fatal error

            int expIDint = Convert.ToInt32(expID);                    // Get ready for action with an int version of the Exp ID

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  1) Blow off the Supporting Docs associated with the Exp. This means deleting the files and SupportingDoc rows.

                    string serverRoot = Server.MapPath(PortalConstants.SupportingDir); // Path to Supporting Docs directory on server
                    var query = from sd in context.SupportingDocs
                                where sd.OwnerID == expIDint
                                select sd;                              // Find all SupportingDoc rows for this ExpID
 
                    foreach (SupportingDoc sd in query)                 // Process each Supporting Doc in turn
                    {
                        File.Delete(serverRoot + sd.SupportingDocID.ToString() + Path.GetExtension(sd.FileName)); // Delete the file from the /Supporting directory
                        context.SupportingDocs.Remove(sd);              // and the row from the database
                    }

                    //  2) Delete the EDHistory rows associated with the Exp

                    context.EDHistorys.RemoveRange(context.EDHistorys.Where(x => x.ExpID == expIDint)); // Delete all of them

                    //  3) Kill off the Exp itself. Do this last so that if something earlier breaks, we can recover - by deleting again.

                    Exp exp = new Exp { ExpID = expIDint };             // Instantiate an Exp object with the selected row's ID
                    context.Exps.Attach(exp);                           // Find that record, but don't fetch it
                    context.Exps.Remove(exp);                           // Delete that row
                    context.SaveChanges();                              // Commit the deletions
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ProjectDashboard", 
                        "Error deleting Exp, SupportingDoc and EDHistory rows or deleting Supporting Document"); // Fatal error
                }
                LoadAllExps();                                          // Update the grid view for display
                litSuccessMessage.Text = "Expense deleted";             // Report success to our user
            }
       }

        // Edit button clicked. Fetch the ExpID of the selected row and dispatch to EditExpense.

        protected void btnExpEdit_Click(object sender, EventArgs e)
        {
            Label label = (Label)AllExpView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains ExpID
            string expID = label.Text;                                  // Extract the text of the control, which is ExpID
            if (expID == "")
                LogError.LogInternalError("ProjectDashboard", string.Format(
                    "Unable to find Expense ID '{0}' from selected GridView row in database", expID)); // Fatal error

            Response.Redirect("EditExpense?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSExpID + "=" + expID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit); // Start with an existing request
        }

        // New Request button has been clicked. Dispatch to Edit Detail

        protected void btnExpNew_Click(object sender, EventArgs e)
        {
            // Propagage the UserID and ProjectID that we were called with. No ExpID means a new Request.

            Response.Redirect("EditExpense?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandNew); // Start with an empty request
        }

        // Review Request button clicked. This is a "Staff" function that only the Project Director can access. Dispatch to the ReviewExpense
        // page.

        protected void btnExpReview_Click(object sender, EventArgs e)
        {
            string expID = FetchSelectedRowLabel();                         // Extract the text of the control, which is ExpID

            // Unconditionally send Exp to ReviewRequest. It is possible that the user does not have the authority to review the exp in
            // its current state. But we'll let ReviewRequest display all the detail for the Exp and then deny editing.

            Response.Redirect("ReviewExpense?" + PortalConstants.QSExpID + "=" + expID + "&" // Start with an existing request
                                              + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandReview + "&" // Review it
                                              + PortalConstants.QSReturn + "=ProjectDashboard"); // Return to this page when done

        }

        // View button clicked. 

        protected void btnExpView_Click(object sender, EventArgs e)
        {
            Label label = (Label)AllExpView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains ExpID
            string expID = label.Text;                                  // Extract the text of the control, which is ExpID
            if (expID == "")
                LogError.LogInternalError("ProjectDashboard", string.Format(
                    "Unable to find Expense ID '{0}' from selected GridView row in database", expID)); // Fatal error

            Response.Redirect("EditExpense?" + PortalConstants.QSUserID + "=" + litSavedUserID.Text + "&"
                                            + PortalConstants.QSProjectID + "=" + litSavedProjectID.Text + "&"
                                            + PortalConstants.QSExpID + "=" + expID + "&"
                                            + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandView); // Start with existing request
        }

        // A little common fragment that pulls the expID from the selected row of the GridView control and makes sure it's not blank.

        string FetchSelectedRowLabel()
        {
            Label label = (Label)AllExpView.SelectedRow.Cells[0].FindControl("lblRowID"); // Find the label control that contains ExpID
            string expID = label.Text;                                  // Extract the text of the control, which is ExpID
            if (expID == "")
                LogError.LogInternalError("ProjectDashboard", string.Format(
                    "Unable to find Expense ID '{0}' from selected GridView row in database", expID)); // Fatal error
            return expID;
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
                if (ckEAwaitingProjectStaff.Checked) filters = filters | 1;
                if (ckEAwaitingCWStaff.Checked) filters = filters | 2;
                if (ckEPaid.Checked) filters = filters | 4;
                if (ckEReturned.Checked) filters = filters | 8;

                switch (filters)
                {
                    case 0:
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.ReservedForFutureUse); // Doesn't ever match
                            break;
                        }
                    case 1:
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.Unsubmitted
                                              || r.CurrentState == ExpState.AwaitingProjectDirector);
                            break;
                        }
                    case 2:
                    case 10:
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.AwaitingTrustDirector
                                              || r.CurrentState == ExpState.AwaitingFinanceDirector
                                              || r.CurrentState == ExpState.AwaitingTrustExecutive
                                              || r.CurrentState == ExpState.Approved
                                              || r.CurrentState == ExpState.CheckMailed);
                            break;
                        }
                    case 3:
                    case 11:
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.Unsubmitted
                                              || r.CurrentState == ExpState.AwaitingProjectDirector
                                              || r.CurrentState == ExpState.AwaitingTrustDirector
                                              || r.CurrentState == ExpState.AwaitingFinanceDirector
                                              || r.CurrentState == ExpState.AwaitingTrustExecutive
                                              || r.CurrentState == ExpState.Approved
                                              || r.CurrentState == ExpState.CheckMailed);
                            break;
                        }
                    case 4:
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.Paid);
                            break;
                        }
                    case 5:
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.Unsubmitted
                                              || r.CurrentState == ExpState.AwaitingProjectDirector
                                              || r.CurrentState == ExpState.Paid);
                            break;
                        }
                    case 6:
                    case 14:
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.AwaitingTrustDirector
                                              || r.CurrentState == ExpState.AwaitingFinanceDirector
                                              || r.CurrentState == ExpState.AwaitingTrustExecutive
                                              || r.CurrentState == ExpState.Approved
                                              || r.CurrentState == ExpState.CheckMailed
                                              || r.CurrentState == ExpState.Returned
                                              || r.CurrentState == ExpState.Paid);
                            break;
                        }
                    case 8:
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.Returned);
                            break;
                        }
                    case 9:
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.Unsubmitted
                                              || r.CurrentState == ExpState.AwaitingProjectDirector
                                              || r.CurrentState == ExpState.Returned);
                            break;
                        }
                    case 12:
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.Paid
                                              || r.CurrentState == ExpState.Returned);
                            break;
                        }
                    case 13:
                        {
                            pred = pred.And(r => r.CurrentState == ExpState.Unsubmitted
                                              || r.CurrentState == ExpState.AwaitingProjectDirector
                                              || r.CurrentState == ExpState.Paid
                                              || r.CurrentState == ExpState.Returned);
                            break;
                        }
                    default:    // Includes case 7 and case 15 - all checked. Don't need a predicate to get all the rows
                        break;
                }

                // Process Since options

                string since = ddlESince.SelectedValue;                 // Figure out which one was selected
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
                        ExpTypeDesc = StateActions.GetEnumDescription(r.ExpType), // Convert enum version to English version for display
                        Amount = r.Amount,
                        CurrentState = r.CurrentState,                  // Load enum version for use when row is selected
                        CurrentStateDesc = StateActions.GetEnumDescription(r.CurrentState) // Convert enum version to English version for display
                    };

                    // Fill "Target" with Vendor Name or Employee Name or Recipient. Only one will be present, depending on ExpType.

                    if (r.Vendor != null)                               // If != a Vendor is present in the Request
                    {
                        if (r.Vendor.Name != null)
                            row.Target = r.Vendor.Name;
                    }
                    else if (r.Employee != null)                        // If != an Employee is present in the Request
                    {
                        if (r.Employee.Name != null)
                            row.Target = r.Employee.Name;
                    }
                    else if (r.Recipient != null)                       // If != a Recipient is present in the Request
                        row.Target = r.Recipient;
                    row.Summary = r.Summary;
                    rows.Add(row);                                      // Add the filled-in row to the list of rows
                }
                AllExpView.DataSource = rows;                           // Give it to the GridView control
                AllExpView.DataBind();                                  // And get it in gear

                NavigationActions.EnableGridViewNavButtons(AllExpView); // Enable appropriate nav buttons based on page count
            }
        }
    }
}
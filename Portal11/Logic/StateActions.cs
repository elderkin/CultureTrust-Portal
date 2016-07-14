﻿using System;
using System.Linq;
using System.Web;
using Portal11.Models;
using System.Web.UI.WebControls;
using System.Data;
using Portal11.ErrorLog;

namespace Portal11.Logic
{

    // OK, it's called "StateActions," but we sneak in a few other things, all related to enumerated data types.
    //  1) Routines that deal with the RequestState enum data type and the ExpHistory table
    //  2) Routines that deal with ProjectRole
    //  3) General support for enum data types (moved to EnumActions)
    //  4) General support for Drop Down Lists
    //  5) General support for other lists

    public class StateActions : System.Web.UI.Page
    {
        //  Start of: 1) Routines that deal with the RequestState enum data type and the ExpHistory table

        // Determine which Unsubmitted state corresponds to User's current Project Role

        public static AppState FindUnsubmittedAppState(string role)
        {
            if (role == ProjectRole.Coordinator.ToString())         // If == User is a Coordinator
            {
                return AppState.UnsubmittedByCoordinator;           // Report actual enum value of AppState
            }
            else if (role == ProjectRole.ProjectDirector.ToString()) // If == User is a Project Director
            {
                return AppState.UnsubmittedByProjectDirector;       // Return actual enum value of AppState
            }
            else if (role == ProjectRole.ProjectStaff.ToString())   // If == User is a Project Staff
            {
                return AppState.UnsubmittedByProjectStaff;
            }
            else
            {
                LogError.LogInternalError("FindUnsubmittedState", $"Invalid ProjectRole value '{role}' encountered"); // Fatal error
                return 0;
            }
        }

        public static DepState FindUnsubmittedDepState(string role)
        {
            if (role == ProjectRole.Coordinator.ToString())         // If == User is a Coordinator
            {
                return DepState.UnsubmittedByCoordinator;           // Report actual enum value of DepState
            }
            else
            {
                LogError.LogInternalError("FindUnsubmittedState", $"Invalid ProjectRole value '{role}' encountered"); // Fatal error
                return 0;
            }
        }
        public static ExpState FindUnsubmittedExpState(string role)
        {
            if (role == ProjectRole.Coordinator.ToString())         // If == User is a Coordinator
            {
                return ExpState.UnsubmittedByCoordinator;           // Report actual enum value of ExpState
            }
            else if (role == ProjectRole.ProjectDirector.ToString()) // If == User is a Project Director
            {
                return ExpState.UnsubmittedByProjectDirector;       // Return actual enum value of ExpState
            }
            else if (role == ProjectRole.ProjectStaff.ToString())   // If == User is a Project Staff
            {
                return ExpState.UnsubmittedByProjectStaff;
            }
            else
            {
                LogError.LogInternalError("FindUnsubmittedState", $"Invalid ProjectRole value '{role}' encountered"); // Fatal error
                return 0;
            }
        }

        // Given a current state, determine which role can approve the Deposit

        public static UserRole UserRoleToApproveRequest(AppState currentState, AppReviewType reviewType)
        {
            if (reviewType == AppReviewType.Express)                // A simpler review process
            {
                switch (currentState)
                {
                    case AppState.UnsubmittedByCoordinator:
                    case AppState.UnsubmittedByProjectDirector:
                    case AppState.UnsubmittedByProjectStaff:
                    case AppState.AwaitingProjectDirector:
                    case AppState.Approved:
                    case AppState.Returned:
                        return UserRole.Project;
                    case AppState.AwaitingTrustDirector:
                        return UserRole.TrustDirector;
                    case AppState.AwaitingCoordinator:
                        return UserRole.Coordinator;
                    default:
                        {
                            LogError.LogInternalError("StateActions", $"Unable to process AppState value '{currentState.ToString()}' from database"); // Fatal error
                            return UserRole.Undefined;
                        }
                }
            }
            else
            {
                switch (currentState)
                {
                    case AppState.UnsubmittedByCoordinator:
                    case AppState.UnsubmittedByProjectDirector:
                    case AppState.UnsubmittedByProjectStaff:
                    case AppState.AwaitingProjectDirector:
                    case AppState.Approved:
                    case AppState.Returned:
                        return UserRole.Project;
                    case AppState.AwaitingTrustDirector:
                        return UserRole.TrustDirector;
                    case AppState.AwaitingFinanceDirector:
                        return UserRole.FinanceDirector;
                    case AppState.AwaitingTrustExecutive:
                        return UserRole.TrustExecutive;
                    case AppState.AwaitingCoordinator:
                        return UserRole.Coordinator;
                    default:
                        {
                            LogError.LogInternalError("StateActions", $"Unable to process AppState value '{currentState.ToString()}' from database"); // Fatal error
                            return UserRole.Undefined;
                        }
                }
            }
        }

        public static UserRole UserRoleToApproveRequest(DepState currentState)
        {
            switch (currentState)
            {
                case DepState.UnsubmittedByCoordinator:
                case DepState.AwaitingProjectDirector:
                case DepState.DepositComplete:
                case DepState.Returned:
                    return UserRole.Project;
                case DepState.AwaitingTrustDirector:
                    return UserRole.TrustDirector;
                case DepState.AwaitingFinanceDirector:
                case DepState.ApprovedReadyToDeposit:
                    return UserRole.FinanceDirector;
                default:
                    {
                        LogError.LogInternalError("StateActions", $"Unable to process DepositState value '{currentState.ToString()}' from database"); // Fatal error
                        return UserRole.Undefined;
                    }
            }
        }

        // Given a role and a current state, determine whether the user can approve the Deposit
        //TODO: What if User has multiple roles?

        internal static bool UserRoleCanApproveRequest(UserRole role, DepState depositState)
        {
            if (role == UserRoleToApproveRequest(depositState))             // If == the specified role can approve the Rqst
                return true;                                                // Report that
            return false;                                                   // Otherwise the specified role cannot approve the Rqst
        }

        // Given a current state, determine which role can approve the Exp

        public static UserRole UserRoleToApproveRequest(ExpState currentState)
        {
            switch (currentState)
            {
                case ExpState.UnsubmittedByCoordinator:
                case ExpState.UnsubmittedByProjectStaff:
                case ExpState.UnsubmittedByProjectDirector:
                case ExpState.AwaitingProjectDirector:
                case ExpState.Paid:
                case ExpState.Returned:
                    return UserRole.Project;
                case ExpState.AwaitingTrustDirector:
                    return UserRole.TrustDirector;
                case ExpState.AwaitingFinanceDirector:
                case ExpState.Approved:
                case ExpState.PaymentSent:
                    return UserRole.FinanceDirector;
                case ExpState.AwaitingTrustExecutive:
                    return UserRole.TrustExecutive;
                default:
                    {
                        LogError.LogInternalError("StateActions", $"Unable to process ExpState value '{currentState.ToString()}' from database"); // Fatal error
                        return UserRole.Undefined;
                    }
            }
        }

        // Given a current state, determine whether the user can approve the Dep/Exp, based on UserRole and ProjectRole from cookies

        internal static bool UserCanApproveRequest(AppState appState, AppReviewType reviewType)
        {
            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Fetch user info cookie for current user
            UserRole userRole = EnumActions.ConvertTextToUserRole(userInfoCookie[PortalConstants.CUserRole]); //Fetch user role, convert to enum
            if (userRole == UserRoleToApproveRequest(appState, reviewType)) // If == the specified role can approve the Dep
            {
                if (userRole == UserRole.Project)                   // If == this is a Project user. Need another check
                {
                    HttpCookie projInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Fetch project info cookie for current project
                    ProjectRole projRole = EnumActions.ConvertTextToProjectRole(projInfoCookie[PortalConstants.CProjectRole]); // Fetch role
                    if (projRole == ProjectRole.ProjectDirector)    // If == this is a Project Director - the only role that can approve anything
                        return true;                                // Report that User can apporve Exp
                }
                return true;
            }
            return false;                                           // Otherwise the specified role cannot approve the Exp
        }

        internal static bool UserCanApproveRequest(DepState depState)
        {
            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Fetch user info cookie for current user
            UserRole userRole = EnumActions.ConvertTextToUserRole(userInfoCookie[PortalConstants.CUserRole]); //Fetch user role, convert to enum
            if (userRole == UserRoleToApproveRequest(depState))     // If == the specified role can approve the Dep
            {
                if (userRole == UserRole.Project)                   // If == this is a Project user. Need another check
                {
                    HttpCookie projInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Fetch project info cookie for current project
                    ProjectRole projRole = EnumActions.ConvertTextToProjectRole(projInfoCookie[PortalConstants.CProjectRole]); // Fetch role
                    if (projRole == ProjectRole.ProjectDirector)    // If == this is a Project Director - the only role that can approve anything
                        return true;                                // Report that User can apporve Exp
                }
                return true;
            }
            return false;                                           // Otherwise the specified role cannot approve the Exp
        }

        internal static bool UserCanApproveRequest(ExpState expState)
        {
            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Fetch user info cookie for current user
            UserRole userRole = EnumActions.ConvertTextToUserRole(userInfoCookie[PortalConstants.CUserRole]); //Fetch user role, convert to enum
            if (userRole == UserRoleToApproveRequest(expState))         // If == the specified role can approve the Exp
            {
                if (userRole == UserRole.Project)                   // If == this is a Project user. Need another check
                {
                    HttpCookie projInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Fetch project info cookie for current project
                    ProjectRole projRole = EnumActions.ConvertTextToProjectRole(projInfoCookie[PortalConstants.CProjectRole]); // Fetch role
                    if (projRole == ProjectRole.ProjectDirector)    // If == this is a Project Director - the only role that can approve anything
                        return true;                                // Report that User can apporve Exp
                }
                return true;
            }
            return false;                                           // Otherwise the specified role cannot approve the Exp
        }

        // Given a role and a current state, determine whether the user can debite the Exp from the current grant

        internal static bool UserRoleCanDebitExp(UserRole role, ExpState expState)
        {
            if (role == UserRole.FinanceDirector)                   // If == the specified role can debit
            {
                if (expState == ExpState.Approved)                  // If == Request is in the right state for debiting
                    return true;                                    // Green means go
            }
            return false;                                           // Otherwise the specified role cannot debit the Exp
        }

        // Copy the most recent state, which is about to be overwritten, into new History row. This preserves the previous state of the Request

        public static void CopyPreviousState(App app, AppHistory hist, string verb)
        {
            hist.AppID = app.AppID;                                 // Connect history row to original request row
            hist.PriorAppState = app.CurrentState;
            hist.HistoryTime = app.CurrentTime;
            hist.HistoryUserID = app.CurrentUserID;
            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Ask for the User Info cookie
            hist.HistoryNote = verb + " by '" + userInfoCookie[PortalConstants.CUserFullName] + "'"; // Explain why this revision appeared
            return;
        }

        public static void CopyPreviousState(Dep dep, DepHistory hist, string verb)
        {
            hist.DepID = dep.DepID;                                 // Connect history row to original request row
            hist.PriorDepState = dep.CurrentState;
            hist.HistoryTime = dep.CurrentTime;
            hist.HistoryUserID = dep.CurrentUserID;
            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Ask for the User Info cookie
            hist.HistoryNote = verb + " by '" + userInfoCookie[PortalConstants.CUserFullName] + "'"; // Explain why this revision appeared
            return;
        }

        public static void CopyPreviousState(Exp exp, ExpHistory hist, string verb)
        {
            hist.ExpID = exp.ExpID;                                 // Connect history row to original request row
            hist.PriorExpState = exp.CurrentState;
            hist.HistoryTime = exp.CurrentTime;
            hist.HistoryUserID = exp.CurrentUserID;
            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Ask for the User Info cookie
            hist.HistoryNote = verb + " by '" + userInfoCookie[PortalConstants.CUserFullName] + "'"; // Explain why this revision appeared
            return;
        }

        // Given a current state, determine what the next state should be. Mostly this is simple, but there is one kink, of course.

        public static AppState FindNextState(AppState currentState, AppReviewType reviewType)
        {
            if (reviewType == AppReviewType.Express)                // A simpler review process
            {

                // Project Member -> Project Director -> Trust Director -> Coordinator - "Approved"

                switch (currentState)                                   // Break out by current state
                {
                    case AppState.UnsubmittedByCoordinator:
                    case AppState.UnsubmittedByProjectStaff:
                        return AppState.AwaitingProjectDirector;        // From Coordinator or Staff, go to Project Director
                    case AppState.UnsubmittedByProjectDirector:
                    case AppState.AwaitingProjectDirector:
                        return AppState.AwaitingTrustDirector;          // From Project Director, go to Trust Director
                    case AppState.AwaitingTrustDirector:
                        return AppState.AwaitingCoordinator;
                    case AppState.AwaitingCoordinator:
                        return AppState.Approved;

                    // None of these states should ever arrive here. Report an internal logic error.

                    case AppState.Approved:
                    case AppState.AwaitingFinanceDirector:
                    case AppState.AwaitingTrustExecutive:
                    case AppState.Returned:
                    case AppState.Error:
                    case AppState.ReservedForFutureUse:
                    default:
                        {
                            LogError.LogInternalError("StateActions", $"Unable to decode DepState value '{currentState.ToString()}' from database"); // Fatal error
                            return AppState.Error;
                        }
                }
            }
            else if (reviewType == AppReviewType.Full)                  // Execute a full review
            {

                // Project Member -> Project Director -> Trust Director -> Finance Director -> Trust Executive - "Approved"

                switch (currentState)                                   // Break out by current state
                {
                    case AppState.UnsubmittedByCoordinator:
                    case AppState.UnsubmittedByProjectStaff:
                        return AppState.AwaitingProjectDirector;
                    case AppState.UnsubmittedByProjectDirector:
                    case AppState.AwaitingProjectDirector:
                        return AppState.AwaitingTrustDirector;
                    case AppState.AwaitingTrustDirector:
                        return AppState.AwaitingFinanceDirector;
                    case AppState.AwaitingFinanceDirector:
                        return AppState.AwaitingTrustExecutive;
                    case AppState.AwaitingTrustExecutive:
                        return AppState.Approved;

                    // None of these states should ever arrive here. Report an internal logic error.

                    case AppState.Approved:
                    case AppState.AwaitingCoordinator:
                    case AppState.Error:
                    case AppState.Returned:
                    case AppState.ReservedForFutureUse:
                    default:
                        {
                            LogError.LogInternalError("StateActions", $"Unable to decode AppState value '{currentState.ToString()}' from database"); // Fatal error
                            return AppState.Error;
                        }
                }

            }
            else
            {
                LogError.LogInternalError("StateActions", $"Unable to decode AppReviewType value '{reviewType.ToString()}' from database"); // Fatal error
                return AppState.Error;
            }
        }

        public static DepState FindNextState(DepState currentState)
        {
            switch (currentState)                                   // Break out by current state
            {
                case DepState.UnsubmittedByCoordinator:
                    return DepState.AwaitingProjectDirector;        // From Coordinator, go to Project Director
                case DepState.AwaitingProjectDirector:
                    return DepState.AwaitingTrustDirector;          // From Project Director, go to Trust Director
                case DepState.AwaitingTrustDirector:
                    return DepState.AwaitingFinanceDirector;
                case DepState.AwaitingFinanceDirector:
                //    return DepState.Approved;
                //case DepState.Approved:
                    return DepState.ApprovedReadyToDeposit;
                case DepState.ApprovedReadyToDeposit:
                    return DepState.DepositComplete;

                // None of these states should ever arrive here. Report an internal logic error.

                case DepState.DepositComplete:
                case DepState.Returned:
                case DepState.Error:
                case DepState.ReservedForFutureUse:
                default:
                    {
                        LogError.LogInternalError("StateActions", $"Unable to decode DepState value '{currentState.ToString()}' from database"); // Fatal error
                        return DepState.Error;
                    }
            }
        }

        public static ExpState FindNextState(ExpState currentState)
        {
            switch (currentState)                                   // Break out by current state
            {
                case ExpState.UnsubmittedByCoordinator:
                case ExpState.UnsubmittedByProjectStaff:
                    return ExpState.AwaitingProjectDirector;
                case ExpState.UnsubmittedByProjectDirector:
                case ExpState.AwaitingProjectDirector:
                    return ExpState.AwaitingTrustDirector;
                case ExpState.AwaitingTrustDirector:
                    return ExpState.AwaitingFinanceDirector;
                case ExpState.AwaitingFinanceDirector:
                    return ExpState.AwaitingTrustExecutive;
                case ExpState.AwaitingTrustExecutive:
                    return ExpState.Approved;
                case ExpState.Approved:
                    return ExpState.PaymentSent;
                case ExpState.PaymentSent:
                    return ExpState.Paid;

                // None of these states should ever arrive here. Report an internal logic error.

                case ExpState.Paid:
                case ExpState.Returned:
                case ExpState.ReservedForFutureUse:
                default:
                    {
                        LogError.LogInternalError("StateActions", $"Unable to decode ExpState value '{currentState.ToString()}' from database"); // Fatal error
                        return ExpState.Error;
                    }
            }
        }

        // Write current state variables for a row. If the caller supplies an DepHistory row, record the new stae there as well.

        public static void SetNewAppState(App app, AppState newState, string userID, AppHistory hist = null)
        {
            app.CurrentState = newState;                                // Store the new state in the Dep row
            app.CurrentTime = System.DateTime.Now;                      // Timestamp this update
            app.CurrentUserID = userID;                                 // Remember who made this change
            if (hist != null)                                           // If != there is an DepHistory row available for update
                hist.NewAppState = newState;                            // Also record the new state
            return;
        }


        public static void SetNewDepState(Dep dep, DepState newState, string userID, DepHistory hist = null)
        {
            dep.CurrentState = newState;                                // Store the new state in the Dep row
            dep.CurrentTime = System.DateTime.Now;                      // Timestamp this update
            dep.CurrentUserID = userID;                                 // Remember who made this change
            if (hist != null)                                           // If != there is an DepHistory row available for update
                hist.NewDepState = newState;                            // Also record the new state
            return;
        }

        // Write current state variables for a Exp row. If the caller supplies an ExpHistory row, record the new stae there as well.

        public static void SetNewExpState(Exp exp, ExpState newState, string userID, ExpHistory hist = null)
        {
            exp.CurrentState = newState;                               // Store the new state in the Exp row
            exp.CurrentTime = System.DateTime.Now;                     // Timestamp this update
            exp.CurrentUserID = userID;                                // Remember who made this change
            if (hist != null)                                           // If != there is an ExpHistory row available for update
                hist.NewExpState = newState;                            // Also record the new state
            return;
        }

        //  Start of: 2) Routines that deal with ProjectRole

        // For a Project and a User, change the role of the User on the Project. Lots of permutations here

        public static void ChangeProjectRole(int projID, string userID, ProjectRole newRole)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  1A) Delete the UserProject row that describes this User's the current role in the project, if any.
                    //  Got that? Whatever the User's current role on the Project - Director, Staff or nothing - blow it away

                    context.UserProjects.RemoveRange(context.UserProjects.Where(
                        up => (up.ProjectID == projID) && (up.UserID == userID))); // Whoosh!

                    //  1B) If the User's new role is Project Director, someone else may be Project Director now. Delete their UserProject row

                    if (newRole == ProjectRole.ProjectDirector)                 // If == User's new role is Project Director
                    {
                        context.UserProjects.RemoveRange(context.UserProjects.Where(
                            up => (up.ProjectID == projID) && (up.ProjectRole == ProjectRole.ProjectDirector))); // Kapow!
                    }

                    //  2) Add a new UserProject row that describes the desired relationship

                    if (newRole == ProjectRole.ProjectDirector || newRole == ProjectRole.ProjectStaff)
                    // If == new role requires a UserProject row for this User
                    {
                        UserProject toAdd = new UserProject                 // Instantiate and fill a new row
                        {
                            ProjectID = projID,
                            UserID = userID,
                            ProjectRole = newRole
                        };
                        context.UserProjects.Add(toAdd);                    // Store it
                    }
                    context.SaveChanges();                                  // Commit the changes
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "StateActions", "Error writing or deleting UserProject row"); // Fatal error
                }
            }
        }

        // Start of: 4) General support for Drop Down Lists

        // Complete the load of a DDL. Connect the DataTable as the DDL's DataSource. Label the TextField and ValueField. Bind the data. Then:
        // If it's a "new" DropDownList, for a new record, then insert a 0th row that forces user to make a selection.
        // If it's a "used" DropDownList, find the freshly filled item that matches our ID value and select it. That prepositions the DDL.
        // Offer the user an option to request an "add" of a new row. This just signals the request to CW Staff.
        // There's a corner case in which the DDL is empty - no rows. This is a user error - the underlying master file is unpopulated.
        //
        // The caller is assumed to have created columns named "Name" and "ID" in the DataTable.

        public static void LoadDdl(DropDownList ddl, int? id, DataTable tbl, string missing, string leader, bool needed=false, string trailer=null)
        {
            ddl.DataSource = tbl;                                           // Pass the DataTable table of rows to drop down list
            ddl.DataTextField = PortalConstants.DdlName;                    // Display this column in the list
            ddl.DataValueField = PortalConstants.DdlID;                     // Return this value for selected row
            ddl.DataBind();                                                 // And get it in gear

            // Special case 1: There's nothing in the list. This indicates an Administrator error - no entities have been assigned to the project.
            // Place a caller-supplied message in the first (and only) line of the DDL and select it.

            if (ddl.Items.Count == 0)                                       // If == the ddl is empty. This is a corner case.
            {
                ddl.Items.Insert(0, new ListItem(missing, String.Empty));   // Insert a caller-supplied message to tell the user there's a problem
                ddl.SelectedIndex = 0;                                      // and select it. But don't fail. Rely on user to fix this problem.
                return;
            }
//            ddl.ClearSelection();                                           // Get rid of any existing selection

            // Special case 2: The user has the option to ask the Administrator to create a new item.
            // Place a caller-supplied "trailer" in the last line.

            if (trailer != null)                                             // If != caller wants the "add" option
            {
                ddl.Items.Add(new ListItem(trailer, PortalConstants.DdlNeededSignal)); // Add a last row with our trailer value
            }

            // Special case 3: The list is populated, but nothing is selected and a "Needed" request hasn't been made.
            // Place a caller-supplied "leader" in the first line and select it.

            if ((id == null) && !needed)                                    // If true new Request, new ddl, create a 0th entry to force selection
            {
                ddl.Items.Insert(0, new ListItem(leader, String.Empty));    // Place an empty item at top of list to force selection of something
                ddl.SelectedIndex = 0;                                      // and select it.
                return;
            }

            // The list is now complete. If a special case has discerned which row to select, we don't get here.
            // But if we're here, either selected the "Needed" row or the previously selected row.

            if (needed)                                                     // Previous selection was "Needed." Restore that selection
            {
                ddl.Items.FindByValue(PortalConstants.DdlNeededSignal).Selected = true; // Mark the "Needed" item
            }
            else                                                            // Existing Request. Find its Item and select it
            {
                try                                                         // It's possible that our Request's selection is no longer in list
                {
                    ddl.ClearSelection();                                   // Get rid of any existing selection
                    ddl.Items.FindByValue(id.ToString()).Selected = true;   // Mark the selection
                }
                catch (NullReferenceException) { }                          // No harm, just don't select any item in the ddl
            }
            return;
        }

        // Unload a DDL into a database field. If nothing was selected, insert a null rather than an ID value. If it's selected, carefully convert it
        // Two flavors to handed a "needed" option. Needed is set true when th3e marker of the "Needed" DDL item is detected.

        public static int? UnloadDdl (DropDownList ddl)
        {
            string sv = ddl.SelectedValue;                                  // Was anything selected?
            if (sv == "")                                                   // If == nothing was selected
            {
                return null;                                                // So selected ID is null
            }
            else
            { 
                try
                {
                    return Convert.ToInt32(sv);                             // Convert and store the selected ID
                }
                catch (System.FormatException)
                {
                    LogError.LogInternalError("StateActions", "Unable to unload Selected Value from DDL"); // Fatal error
                }
            }
            return null;
        }
        public static void UnloadDdl(DropDownList ddl, out int? id, out bool needed)
        {
            string sv = ddl.SelectedValue;                                  // Was anything selected?
            if (sv == "")                                                   // If == nothing was selected
            {
                id = null;                                                  // So selected ID is null
                needed = false;                                             // And needed flag is not set
            }
            else if (sv == PortalConstants.DdlNeededSignal)                 // If == "Needed" option was selected
            {
                id = null;                                                  // So selected ID is null
                needed = true;                                              // And needed flag is set
            }
            else
            {
                try
                {
                    id = Convert.ToInt32(sv);                               // Convert and store the selected ID
                    needed = false;                                         // Needed flag is not set
                }
                catch (System.FormatException)
                {
                    LogError.LogInternalError("StateActions", "Unable to unload Selected Value from DDL"); // Fatal error
                    id = null;
                    needed = false;
                }
            }
            return;
        }

        // Start of 5)

        // Load a list with all the grants available to this project into a List Box

        public static decimal LoadGrantsList(int projID, ListBox lst)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var query = from pg in context.ProjectGrants
                            where pg.ProjectID == projID
                            orderby pg.Grant.Name
                            select new { pg.GrantID, pg.Grant.Name, pg.Grant.CurrentFunds };
                // Find all grants that are assigned to this project

                DataTable grants = new DataTable();
                grants.Columns.Add("GrantID");
                grants.Columns.Add("Name");

                decimal total = 0;                                  // Initialize total funds in grants
                foreach (var row in query)
                {
                    DataRow dr = grants.NewRow();                   // Build a row from the next query output
                    dr["GrantID"] = row.GrantID;
                    dr["Name"] = row.Name + " (" + row.CurrentFunds.ToString("C") + ")"; // Insert name (and balance) together
                    grants.Rows.Add(dr);                            // Add the new row to the data table
                    total = total + row.CurrentFunds;               // Accumulate total grants
                }

                if (grants.Rows.Count == 0)                         // If == the lst is empty because there is nothing in the database.
                {
                    DataRow dr = grants.NewRow();                   // Build a row for the filler
                    dr["GrantID"] = "";
                    dr["Name"] = " -- No Grants assigned to Project --";
                    grants.Rows.Add(dr);                            // Add the filler row
                }

                lst.DataSource = grants;                            // Pass the full table of users to drop down list
                lst.DataTextField = "Name";                         // Display this column in the list
                lst.DataValueField = "GrantID";                     // Return this value for selected row. Right now, the user can't select a row
                lst.DataBind();                                     // And get it in gear

                return total;                                       // Make the total available to caller
            }
        }

    }
}
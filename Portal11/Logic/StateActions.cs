﻿using System;
using System.Linq;
using System.Web;
using Portal11.Models;
using System.Web.UI.WebControls;
using System.Data;
using Portal11.ErrorLog;

namespace Portal11.Logic
{

    // Routines that deal with the RequestState enum data type and the ExpHistory table

    public class StateActions : System.Web.UI.Page
    {
        //  Start of: 1) Routines that deal with the RequestState enum data type and the ExpHistory table

        // Determine which Unsubmitted state corresponds to User's current Project Role

        public static AppState FindUnsubmittedAppState(string role)
        {
            if (role == ProjectRole.InternalCoordinator.ToString())         // If == User is a Coordinator
            {
                return AppState.UnsubmittedByInternalCoordinator;           // Report actual enum value of AppState
            }
            else if (role == ProjectRole.ProjectDirector.ToString())        // If == User is a Project Director
            {
                return AppState.UnsubmittedByProjectDirector;               // Return actual enum value of AppState
            }
            else if (role == ProjectRole.ProjectStaff.ToString())           // If == User is a Project Staff
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
            if (role == ProjectRole.InternalCoordinator.ToString())         // If == User is a Coordinator
            {
                return DepState.UnsubmittedByInternalCoordinator;           // Report actual enum value of DepState
            }
            else
            {
                LogError.LogInternalError("FindUnsubmittedState", $"Invalid ProjectRole value '{role}' encountered"); // Fatal error
                return 0;
            }
        }
        public static ExpState FindUnsubmittedExpState(string role)
        {
            switch (EnumActions.ConvertTextToProjectRole(role))
            {
                case ProjectRole.InternalCoordinator:
                    return ExpState.UnsubmittedByInternalCoordinator;
                case ProjectRole.ProjectDirector:
                    return ExpState.UnsubmittedByProjectDirector;
                case ProjectRole.ProjectStaff:
                    return ExpState.UnsubmittedByProjectStaff;
                default:
                    LogError.LogInternalError("FindUnsubmittedState", $"Invalid ProjectRole value '{role}' encountered"); // Fatal error
                return 0;
            }
        }

        // Given a current state, determine which project role can process the Request.

        public static ProjectRole ProjectRoleToProcessRequest(AppState currentState)
        {
            switch (currentState)
            {
                case AppState.UnsubmittedByInternalCoordinator:
                case AppState.AwaitingInternalCoordinator:
                    return ProjectRole.InternalCoordinator;
                case AppState.UnsubmittedByProjectDirector:
                case AppState.AwaitingProjectDirector:
                case AppState.Returned:
                    return ProjectRole.ProjectDirector;
                case AppState.UnsubmittedByProjectStaff:
                    return ProjectRole.ProjectStaff;
                case AppState.Approved:
                case AppState.AwaitingCommunityDirector:
                case AppState.AwaitingFinanceDirector:
                case AppState.AwaitingPresident:
                    return ProjectRole.NoRole;
                default:
                    {
                        LogError.LogInternalError("StateActions.ProjectRoleToProcessRequest", $"Unable to process AppState value '{currentState.ToString()}'"); // Fatal error
                        return ProjectRole.NoRole;
                    }
            }
        }

        public static ProjectRole ProjectRoleToProcessRequest(DepState currentState)
        {
            switch (currentState)
            {
                case DepState.UnsubmittedByInternalCoordinator:
                case DepState.Returned:
                    return ProjectRole.InternalCoordinator;
                case DepState.AwaitingProjectDirector:
                case DepState.RevisingByProjectDirector:
                case DepState.RevisedByFinanceDirector:
                    return ProjectRole.ProjectDirector;
                case DepState.RevisingByFinanceDirector:
                    return ProjectRole.RevisingFinanceDirector;
                case DepState.DepositComplete:
                case DepState.AwaitingCommunityDirector:
                case DepState.AwaitingFinanceDirector:
                case DepState.ApprovedReadyToDeposit:                       // Now obsolete
                    return ProjectRole.NoRole;
                default:
                    {
                        LogError.LogInternalError("StateActions.ProjectRoleToProcessRequest", $"Unable to process DepositState value '{currentState.ToString()}'"); // Fatal error
                        return ProjectRole.NoRole;
                    }
            }
        }

        public static ProjectRole ProjectRoleToProcessRequest(ExpState currentState)
        {
            switch (currentState)
            {
                case ExpState.AwaitingInternalCoordinator:
                case ExpState.ReturnedToInternalCoordinator:
                case ExpState.RevisingByInternalCoordinator:
                case ExpState.UnsubmittedByInternalCoordinator:
                    return ProjectRole.InternalCoordinator;
                case ExpState.ReturnedToProjectStaff:
                case ExpState.UnsubmittedByProjectStaff:
                    return ProjectRole.ProjectStaff;
                case ExpState.AwaitingProjectDirector:
                case ExpState.ReturnedToProjectDirector:
                case ExpState.RevisingByProjectDirector:
                case ExpState.UnsubmittedByProjectDirector:
                case ExpState.RevisedByCommunityDirector:
                case ExpState.RevisedByFinanceDirector:
                case ExpState.RevisedByInternalCoordinator:
                case ExpState.RevisedByPresident:
                    return ProjectRole.ProjectDirector;
                case ExpState.Approved:
                case ExpState.AwaitingCommunityDirector:
                case ExpState.AwaitingFinanceDirector:
                case ExpState.AwaitingPresident:
                case ExpState.Paid:
                case ExpState.PaymentSent:
                    return ProjectRole.NoRole;
                default:
                    {
                        LogError.LogInternalError("StateActions.UserRoleToApproveRequest", $"Unable to process ExpState value '{currentState.ToString()}'"); // Fatal error
                        return ProjectRole.NoRole;
                    }
            }
        }

        // Given a current state, determine whether the Return Note field can be edited

        public static bool StateToEditReturnNote(DepState currentState)
        {
            switch (currentState)
            {
                case DepState.UnsubmittedByInternalCoordinator:
                case DepState.DepositComplete:
                case DepState.ApprovedReadyToDeposit:                       // Now obsolete
                case DepState.Returned:
                    return false;                                           // Return Note is ReadOnly
                case DepState.AwaitingProjectDirector:
                case DepState.RevisedByFinanceDirector:
                case DepState.RevisingByProjectDirector:
                case DepState.RevisingByFinanceDirector:
                case DepState.AwaitingCommunityDirector:
                case DepState.AwaitingFinanceDirector:
                    return true;
                default:
                    {
                        LogError.LogInternalError("StateActions.StateToEditReturnNote", $"Unable to process DepositState value '{currentState.ToString()}'"); // Fatal error
                        return false;
                    }
            }
        }
        
        public static bool StateToEditReturnNote(ExpState currentState)
        {
            switch (currentState)
            {
                case ExpState.Approved:

                case ExpState.AwaitingCommunityDirector:
                case ExpState.AwaitingFinanceDirector:
                case ExpState.AwaitingInternalCoordinator:
                case ExpState.AwaitingPresident:
                case ExpState.AwaitingProjectDirector:

                case ExpState.RevisedByCommunityDirector:
                case ExpState.RevisedByFinanceDirector:
                case ExpState.RevisedByInternalCoordinator:
                case ExpState.RevisedByPresident:

                case ExpState.RevisingByCommunityDirector:
                case ExpState.RevisingByFinanceDirector:
                case ExpState.RevisingByInternalCoordinator:
                case ExpState.RevisingByPresident:
                case ExpState.RevisingByProjectDirector:
                    return true;
                default:
                    return false;                                           // Return Note is ReadOnly for everybody else
            }
        }

        // Given a current state, determine whether the request has been returned

        public static bool RequestIsReturned(DepState currentState)
        {
            switch (currentState)
            {
                case DepState.Returned:
                    return true;
                default:
                    return false;
            }
        }

        public static bool RequestIsReturned(ExpState currentState)
        {
            switch (currentState)
            {
                case ExpState.ReturnedToProjectDirector:
                case ExpState.ReturnedToInternalCoordinator:
                case ExpState.ReturnedToProjectStaff:
                    return true;
                default:
                    return false;
            }
        }

        // Given a current state, determine whether the request can be revised, rather than reviewed.

        public static bool RequestIsRevising(AppState currentState)
        {
            switch (currentState)
            {
                case AppState.UnsubmittedByInternalCoordinator:
                case AppState.AwaitingInternalCoordinator:
                case AppState.UnsubmittedByProjectDirector:
                case AppState.AwaitingProjectDirector:
                case AppState.Returned:
                case AppState.UnsubmittedByProjectStaff:
                case AppState.Approved:
                case AppState.AwaitingCommunityDirector:
                case AppState.AwaitingFinanceDirector:
                case AppState.AwaitingPresident:
                    return false;
                default:
                    {
                        LogError.LogInternalError("StateActions.StateToReviseRequest", $"Unable to process AppState value '{currentState.ToString()}'"); // Fatal error
                        return false;
                    }
            }
        }

        public static bool RequestIsRevising(DepState currentState)
        {
            switch (currentState)
            {
                case DepState.RevisingByProjectDirector:
                case DepState.RevisingByFinanceDirector:
                    return true;
                case DepState.UnsubmittedByInternalCoordinator:
                case DepState.Returned:
                case DepState.AwaitingProjectDirector:
                case DepState.RevisedByFinanceDirector:
                case DepState.DepositComplete:
                case DepState.AwaitingCommunityDirector:
                case DepState.AwaitingFinanceDirector:
                case DepState.ApprovedReadyToDeposit:                       // Now obsolete
                    return false;
                default:
                    {
                        LogError.LogInternalError("StateActions.StateToReviseRequest", $"Unable to process DepositState value '{currentState.ToString()}'"); // Fatal error
                        return false;
                    }
            }
        }

        public static bool RequestIsRevising(ExpState currentState)
        {
            switch (currentState)
            {
                case ExpState.RevisingByCommunityDirector:
                case ExpState.RevisingByFinanceDirector:
                case ExpState.RevisingByInternalCoordinator:
                case ExpState.RevisingByPresident:
                case ExpState.RevisingByProjectDirector:
                    return true;
                default:
                    return false;
            }
        }


        public static bool RequestIsUnsubmitted(ExpState currentState)
        {
            switch (currentState)
            {
                case ExpState.UnsubmittedByInternalCoordinator:
                case ExpState.UnsubmittedByProjectStaff:
                case ExpState.UnsubmittedByProjectDirector:
                    return true;
                default:
                    return false;
            }
        }

        // Given a current state, determine which user role can process the request. Note that we ignore whether the
        // request is Express or Full. We just look at the current state and say the role that can deal with it.
        // For Express requests, the request will never get into some of these states. But no harm done.

        public static UserRole UserRoleToProcessRequest(AppState currentState)
        {
            switch (currentState)
            {
                case AppState.UnsubmittedByInternalCoordinator:
                case AppState.UnsubmittedByProjectDirector:
                case AppState.UnsubmittedByProjectStaff:
                case AppState.AwaitingProjectDirector:
                case AppState.Approved:
                case AppState.Returned:
                    return UserRole.Project;
                case AppState.AwaitingCommunityDirector:
                    return UserRole.CommunityDirector;
                case AppState.AwaitingFinanceDirector:
                    return UserRole.FinanceDirector;
                case AppState.AwaitingPresident:
                    return UserRole.President;
                case AppState.AwaitingInternalCoordinator:
                    return UserRole.InternalCoordinator;
                default:
                    {
                        LogError.LogInternalError("StateActions.UserRoleToApproveRequest", $"Unable to process AppState value '{currentState.ToString()}'"); // Fatal error
                        return UserRole.Undefined;
                    }
            }
        }

        public static UserRole UserRoleToProcessRequest(DepState currentState)
        {
            switch (currentState)
            {
                case DepState.UnsubmittedByInternalCoordinator:
                case DepState.AwaitingProjectDirector:
                case DepState.DepositComplete:
                case DepState.RevisingByProjectDirector:
                case DepState.RevisedByFinanceDirector:
                    return UserRole.Project;
                case DepState.Returned:
                    return UserRole.InternalCoordinator;                    // Note: Deposits are returned to IC, not PD
                case DepState.AwaitingCommunityDirector:
                    return UserRole.CommunityDirector;
                case DepState.AwaitingFinanceDirector:
                case DepState.RevisingByFinanceDirector:
                case DepState.ApprovedReadyToDeposit:                       // Now obsolete
                    return UserRole.FinanceDirector;
                default:
                    {
                        LogError.LogInternalError("StateActions.UserRoleToApproveRequest", $"Unable to process DepositState value '{currentState.ToString()}'"); // Fatal error
                        return UserRole.Undefined;
                    }
            }
        }

        public static UserRole UserRoleToProcessRequest(ExpState currentState)
        {
            switch (currentState)
            {
                case ExpState.UnsubmittedByInternalCoordinator:
                case ExpState.UnsubmittedByProjectStaff:
                case ExpState.UnsubmittedByProjectDirector:
                case ExpState.AwaitingProjectDirector:
                case ExpState.Paid:
                case ExpState.ReturnedToProjectDirector:
                case ExpState.ReturnedToProjectStaff:
                case ExpState.RevisedByCommunityDirector:
                case ExpState.RevisedByFinanceDirector:
                case ExpState.RevisedByInternalCoordinator:
                case ExpState.RevisedByPresident:
                    return UserRole.Project;
                case ExpState.AwaitingInternalCoordinator:
                case ExpState.RevisingByInternalCoordinator:
                case ExpState.ReturnedToInternalCoordinator:
                    return UserRole.InternalCoordinator;
                case ExpState.AwaitingCommunityDirector:
                case ExpState.RevisingByCommunityDirector:
                    return UserRole.CommunityDirector;
                case ExpState.AwaitingFinanceDirector:
                case ExpState.RevisingByFinanceDirector:
                case ExpState.Approved:
                case ExpState.PaymentSent:
                    return UserRole.FinanceDirector;
                case ExpState.AwaitingPresident:
                case ExpState.RevisingByPresident:
                    return UserRole.President;
                default:
                    {
                        LogError.LogInternalError("StateActions.UserRoleToApproveRequest", $"Unable to process ExpState value '{currentState.ToString()}'"); // Fatal error
                        return UserRole.Undefined;
                    }
            }
        }

        // Given a current state, determine whether the user can approve the Dep/Exp, based on UserRole and ProjectRole from cookies

        internal static bool UserCanApproveRequest(AppState appState)
        {
            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Fetch user info cookie for current user
            UserRole userRole = EnumActions.ConvertTextToUserRole(userInfoCookie[PortalConstants.CUserRole]); //Fetch user role, convert to enum
            if (userRole == UserRoleToProcessRequest(appState))     // If == the specified role can approve the Dep
            {
                if (userRole == UserRole.Project)                   // If == this is a Project user. Need another check
                {
                    HttpCookie projInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Fetch project info cookie for current project
// ** What if this is missing? **
                    ProjectRole projRole = EnumActions.ConvertTextToProjectRole(projInfoCookie[PortalConstants.CProjectRole]); // Fetch role
                    if (projRole == ProjectRole.ProjectDirector)    // If == this is a Project Director - the only role that can approve anything
                        return true;                                // Report that User can apporve App
                    else
                        return false;                               // No other project roles can approve App
                }
                return true;
            }
            return false;                                           // Otherwise the specified role cannot approve the Exp
        }

        internal static bool UserCanApproveRequest(DepState depState)
        {
            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Fetch user info cookie for current user
            UserRole userRole = EnumActions.ConvertTextToUserRole(userInfoCookie[PortalConstants.CUserRole]); //Fetch user role, convert to enum
            if (userRole == UserRoleToProcessRequest(depState))     // If == the specified role can approve the Dep
            {
                if (userRole == UserRole.Project)                   // If == this is a Project user. Need another check
                {
                    HttpCookie projInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Fetch project info cookie for current project
                    ProjectRole projRole = EnumActions.ConvertTextToProjectRole(projInfoCookie[PortalConstants.CProjectRole]); // Fetch role
                    if (projRole == ProjectRole.ProjectDirector)    // If == this is a Project Director - the only role that can approve anything
                        return true;                                // Report that User can apporve Dep
                    else
                        return false;                               // No other project roles can approve Dep
                }
                return true;
            }
            return false;                                           // Otherwise the specified role cannot approve the Exp
        }

        internal static bool UserCanApproveRequest(ExpState expState)
        {
            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Fetch user info cookie for current user
            UserRole userRole = EnumActions.ConvertTextToUserRole(userInfoCookie[PortalConstants.CUserRole]); //Fetch user role, convert to enum
            if (userRole == UserRoleToProcessRequest(expState))     // If == the specified role can approve the Exp
            {
                if (userRole == UserRole.Project)                   // If == this is a Project user. Need another check
                {
                    HttpCookie projInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Fetch project info cookie for current project
                    ProjectRole projRole = EnumActions.ConvertTextToProjectRole(projInfoCookie[PortalConstants.CProjectRole]); // Fetch role
                    if (projRole == ProjectRole.ProjectDirector)    // If == this is a Project Director - the only role that can approve anything
                        return true;                                // Report that User can apporve Exp
                    else
                        return false;                               // No other project roles can approve Exp
                }
                return true;                                        // It's staff and they can approve
            }
            return false;                                           // Otherwise the specified role cannot approve the Exp
        }

        // Given a current state, determine whether the user can revise the Dep/Exp. This is based only on state

        internal static bool UserCanReviseRequest(DepState state)
        {
            switch (state)
            {
                case DepState.RevisedByFinanceDirector:
                    return false;                                   // In this state, request has been revised once. That's all they get.
                default:
                    return true;                                    // Everybody else can revise request
            }
        }

        internal static bool UserCanReviseRequest(ExpState state)
        {
            switch (state)
            {
                case ExpState.Approved:
                case ExpState.AwaitingCommunityDirector:
                case ExpState.AwaitingFinanceDirector:
                case ExpState.AwaitingInternalCoordinator:
                case ExpState.AwaitingPresident:
                case ExpState.AwaitingProjectDirector:
                    return true;                                    // Request is waiting for their review. They can revise.
                default:
                    return false;
            }
        }

        // Copy the most recent state, which is about to be overwritten, into new History row. This preserves the previous state of the Request

        public static void CopyPreviousState(App app, AppHistory hist, string verb)
        {
            hist.AppID = app.AppID;                                 // Connect history row to original request row
            hist.PriorAppState = app.CurrentState;
            hist.HistoryTime = app.CurrentTime;
            hist.HistoryUserID = app.CurrentUserID;
            hist.ReturnNote = app.ReturnNote;
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
            hist.ReturnNote = dep.ReturnNote;
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
            hist.ReturnNote = exp.ReturnNote;
            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Ask for the User Info cookie
            hist.HistoryNote = verb + " by '" + userInfoCookie[PortalConstants.CUserFullName] + "'"; // Explain why this revision appeared
            return;
        }

        // Given a current state, determine what the next state should be. Mostly this is simple, but there are kinks, of course.

        public static AppState FindNextState(AppState currentState, AppReviewType reviewType)
        {
            if (reviewType == AppReviewType.Express)                // A simpler review process
            {

                // Project Member -> Project Director -> Community Director -> Coordinator - "Approved"

                switch (currentState)                                   // Break out by current state
                {
                    case AppState.UnsubmittedByInternalCoordinator:
                    case AppState.UnsubmittedByProjectStaff:
                        return AppState.AwaitingProjectDirector;        // From Coordinator or Staff, go to Project Director
                    case AppState.UnsubmittedByProjectDirector:
                    case AppState.AwaitingProjectDirector:
                        return AppState.AwaitingCommunityDirector;          // From Project Director, go to Community Director
                    case AppState.AwaitingCommunityDirector:
                        return AppState.AwaitingInternalCoordinator;
                    case AppState.AwaitingInternalCoordinator:
                        return AppState.Approved;

                    // None of these states should ever arrive here. Report an internal logic error.

                    case AppState.Approved:
                    case AppState.AwaitingFinanceDirector:
                    case AppState.AwaitingPresident:
                    case AppState.Returned:
                    case AppState.Error:
                    case AppState.ReservedForFutureUse:
                    default:
                        {
                            LogError.LogInternalError("StateActions.FindNextState", 
                                $"Inappropriate AppState value '{currentState.ToString()}' and Review Type '{reviewType.ToString()}"); // Fatal error
                            return AppState.Error;
                        }
                }
            }
            else if (reviewType == AppReviewType.Full)                  // Execute a full review
            {

                // Project Member -> Project Director -> Community Director -> Finance Director -> President - "Approved"

                switch (currentState)                                   // Break out by current state
                {
                    case AppState.UnsubmittedByInternalCoordinator:
                    case AppState.UnsubmittedByProjectStaff:
                        return AppState.AwaitingProjectDirector;
                    case AppState.UnsubmittedByProjectDirector:
                    case AppState.AwaitingProjectDirector:
                        return AppState.AwaitingCommunityDirector;
                    case AppState.AwaitingCommunityDirector:
                        return AppState.AwaitingFinanceDirector;
                    case AppState.AwaitingFinanceDirector:
                        return AppState.AwaitingPresident;
                    case AppState.AwaitingPresident:
                        return AppState.Approved;

                    // None of these states should ever arrive here. Report an internal logic error.

                    case AppState.Approved:
                    case AppState.AwaitingInternalCoordinator:
                    case AppState.Error:
                    case AppState.Returned:
                    case AppState.ReservedForFutureUse:
                    default:
                        {
                            LogError.LogInternalError("StateActions.FindNextState", 
                                $"Inappropriate AppState value '{currentState.ToString()}' and Review Type '{reviewType.ToString()}"); // Fatal error
                            return AppState.Error;
                        }
                }
            }
            else if (reviewType == AppReviewType.ICOnly)                // Execute an Internal Coordinator Only review
            {

                // Project Member -> Project Director -> Internal Coordinator - "Approved"

                switch (currentState)                                   // Break out by current state
                {
                    case AppState.UnsubmittedByInternalCoordinator:
                    case AppState.UnsubmittedByProjectStaff:
                        return AppState.AwaitingProjectDirector;
                    case AppState.UnsubmittedByProjectDirector:
                    case AppState.AwaitingProjectDirector:
                        return AppState.AwaitingInternalCoordinator;
                    case AppState.AwaitingInternalCoordinator:
                        return AppState.Approved;

                    // None of these states should ever arrive here. Report an internal logic error.

                    case AppState.AwaitingCommunityDirector:
                    case AppState.AwaitingFinanceDirector:
                    case AppState.AwaitingPresident:
                    case AppState.Approved:
                    case AppState.Error:
                    case AppState.Returned:
                    case AppState.ReservedForFutureUse:
                    default:
                        {
                            LogError.LogInternalError("StateActions.FindNextState", 
                                $"Inappropriate AppState value '{currentState.ToString()}' and Review Type '{reviewType.ToString()}"); // Fatal error
                            return AppState.Error;
                        }
                }
            }
            else
            {
                LogError.LogInternalError("StateActions.FindNextState", $"Inappropriate ReviewType value '{reviewType.ToString()}'"); // Fatal error
                return AppState.Error;
            }
        }

        public static DepState FindNextState(DepState currentState, ReviewAction action)
        {
            switch (action)
            {

                // Approve means advance to the next review or be done

                case ReviewAction.Approve:
                    switch (currentState)                                   // Break out by current state
                    {
                        case DepState.UnsubmittedByInternalCoordinator:
                            return DepState.AwaitingProjectDirector;        // From Coordinator, go to Project Director
                        case DepState.AwaitingProjectDirector:
                        case DepState.RevisedByFinanceDirector:
                            return DepState.AwaitingFinanceDirector;        // From PD, go to Finance Director
                        case DepState.AwaitingFinanceDirector:
                            return DepState.DepositComplete;
                        default:                                            // No other states should arrive here. Report error
                            break;
                    }
                    break;

                // Return means go back to originator of the request

                case ReviewAction.Return:
                    switch (currentState)                                   // Break out by current state
                    {
                        case DepState.AwaitingProjectDirector:
                        case DepState.AwaitingCommunityDirector:
                        case DepState.AwaitingFinanceDirector:
                        case DepState.RevisedByFinanceDirector:
                            return DepState.Returned;
                        default:                                            // No other states should arrive here. Report error
                            break;
                    }
                    break;

                // Revise means current reviewer can edit the request

                case ReviewAction.Revise:
                    switch (currentState)                                   // Break out by current state
                    {
                        case DepState.Returned:
                            return DepState.UnsubmittedByInternalCoordinator;
                        case DepState.AwaitingProjectDirector:
                            return DepState.RevisingByProjectDirector;
                        case DepState.AwaitingFinanceDirector:
                            return DepState.RevisingByFinanceDirector;
                        default:                                            // No other states should arrive here. Report error
                            break;
                    }
                    break;

                // Submit means conclude the editing and commence review

                case ReviewAction.Submit:
                    switch (currentState)                                   // Break out by current state
                    {
                        case DepState.UnsubmittedByInternalCoordinator:
                        case DepState.Returned:
                            return DepState.AwaitingProjectDirector;
                        case DepState.RevisingByProjectDirector:
                            return DepState.Returned;
                        case DepState.RevisingByFinanceDirector:
                            return DepState.RevisedByFinanceDirector;
                        default:                                            // No other states should arrive here. Report error
                            break;
                    }
                    break;
                default:
                    break;
            }
                LogError.LogInternalError("StateActions.FindNextState", $"Inappropriate DepState value '{currentState.ToString()}' for ReviewAction value '{action.ToString()}'"); // Fatal error
                return DepState.Error;
        }

        public static ExpState FindNextState(ExpState currentState, ReviewAction action, ProjectRole role=ProjectRole.ProjectDirector)
        {
            switch (action)
            {

                // Approve means advance to the next review or be done

                case ReviewAction.Approve:
                    switch (currentState)                                   // Break out by current state
                    {
                        case ExpState.AwaitingProjectDirector:
                            return ExpState.AwaitingInternalCoordinator;
                        case ExpState.AwaitingInternalCoordinator:
                            return ExpState.AwaitingFinanceDirector;
                        case ExpState.AwaitingFinanceDirector:
                            return ExpState.AwaitingCommunityDirector;
                        case ExpState.AwaitingCommunityDirector:
                            return ExpState.AwaitingPresident;
                        case ExpState.AwaitingPresident:
                            return ExpState.Approved;
                        case ExpState.Approved:
                            return ExpState.Paid;
                        case ExpState.RevisedByInternalCoordinator:
                        case ExpState.RevisedByFinanceDirector:
                        case ExpState.RevisedByCommunityDirector:
                        case ExpState.RevisedByPresident:
                            return ExpState.AwaitingInternalCoordinator;
                        default:                                            // No other states should arrive here. Report error
                            break;
                    }
                    break;

                // Return means go back to originator of the request

                case ReviewAction.Return:
                    switch (currentState)                                   // Break out by current state
                    {
                        case ExpState.AwaitingProjectDirector:
                        case ExpState.AwaitingInternalCoordinator:
                        case ExpState.AwaitingFinanceDirector:
                        case ExpState.AwaitingCommunityDirector:
                        case ExpState.AwaitingPresident:
                        case ExpState.RevisedByInternalCoordinator:
                        case ExpState.RevisedByFinanceDirector:
                        case ExpState.RevisedByCommunityDirector:
                        case ExpState.RevisedByPresident:
                            return SendToOriginator(role);                  // Back to right "returned" state
                        default:                                            // No other states should arrive here. Report error
                            break;
                    }
                    break;

                // Revise means current reviewer can edit the request

                case ReviewAction.Revise:
                    switch (currentState)                                   // Break out by current state
                    {
                        case ExpState.ReturnedToInternalCoordinator:
                            return ExpState.UnsubmittedByInternalCoordinator;
                        case ExpState.ReturnedToProjectDirector:
                            return ExpState.UnsubmittedByProjectDirector;
                        case ExpState.ReturnedToProjectStaff:
                            return ExpState.UnsubmittedByProjectStaff;
                        case ExpState.AwaitingProjectDirector:
                            return ExpState.RevisingByProjectDirector;
                        case ExpState.AwaitingInternalCoordinator:
                            return ExpState.RevisingByInternalCoordinator;
                        case ExpState.AwaitingFinanceDirector:
                            return ExpState.RevisingByFinanceDirector;
                        case ExpState.AwaitingCommunityDirector:
                            return ExpState.RevisingByCommunityDirector;
                        case ExpState.AwaitingPresident:
                            return ExpState.RevisingByPresident;
                        case ExpState.Approved:
                            return ExpState.RevisingByFinanceDirector;
                        default:                                            // No other states should arrive here. Report error
                            break;
                    }
                    break;

                // Submit means conclude the editing and commence review

                case ReviewAction.Submit:
                    switch (currentState)                                   // Break out by current state
                    {
                        case ExpState.UnsubmittedByInternalCoordinator:
                        case ExpState.UnsubmittedByProjectStaff:
                            return ExpState.AwaitingProjectDirector;
                        case ExpState.UnsubmittedByProjectDirector:
                        case ExpState.AwaitingProjectDirector:
                            return ExpState.AwaitingInternalCoordinator;
                        case ExpState.RevisingByProjectDirector:
                            return SendToOriginator(role);                  // Back to right "returned" state
                        case ExpState.RevisingByInternalCoordinator:
                            return ExpState.RevisedByInternalCoordinator;
                        case ExpState.RevisingByFinanceDirector:
                            return ExpState.RevisedByFinanceDirector;
                        case ExpState.RevisingByCommunityDirector:
                            return ExpState.RevisedByCommunityDirector;
                        case ExpState.RevisingByPresident:
                            return ExpState.RevisedByPresident;
                        default:                                            // No other states should arrive here. Report error
                            break;
                    }
                    break;
                default:
                    break;
            }
            LogError.LogInternalError("StateActions.FindNextState", $"Inappropriate ExpState value '{currentState.ToString()}' for ReviewAction value '{action.ToString()}'"); // Fatal error
            return ExpState.Error;
        }
        static ExpState SendToOriginator(ProjectRole role)
        {
            switch (role)
                {
                case ProjectRole.InternalCoordinator:
                    return ExpState.ReturnedToInternalCoordinator;
                case ProjectRole.ProjectDirector:
                    return ExpState.ReturnedToProjectDirector;
                case ProjectRole.ProjectStaff:
                    return ExpState.ReturnedToProjectStaff;
                default:                                                // For compatibility with earlier releases, deal with an undefined project role
                    return ExpState.ReturnedToProjectDirector;
            }
        }

        // See if the FD is the next reviewer. This little thing is here to keep all state-related logic in this class.

        public static bool NextReviewIsSecondFD (ExpState state)
        {
            if (state == ExpState.Approved)                             // If == FD is next reviewer
                return true;                                            // Let the caller know
            return false;
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
    }
}
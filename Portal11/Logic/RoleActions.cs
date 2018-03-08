using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal11.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Portal11.ErrorLog;

namespace Portal11.Logic
{
    public class RoleActions
    {
        internal void AddRoles()
        {
            // Access the application context and create result variables

            using (Models.ApplicationDbContext context = new ApplicationDbContext())
            {
                IdentityResult IdRoleResult;

                // Create a RoleStore object using the DbContext we just accessed
                // The RoleStore is only allowed to contain IdentityRole objects

                var roleStore = new RoleStore<IdentityRole>(context);

                // Create a RoleManager object. It is only allowed to contain IdentityRole objects
                // When creating a RoleManager object, you pass in the new RoleStore object as a parameter

                var roleMgr = new RoleManager<IdentityRole>(roleStore);

                // Create each CultureTrust Portal role if they don't already exist.
                // There is an assumption here that if the first role is missing, they're all missing.
                // And if the first role is present, they're all present.
                // This is to accelerate application startup.

                try
                {

                    // During startup, this is the first touch of the database. So an error at this point is often structural,
                    // such as missing database or locked database.

                    if (!roleMgr.RoleExists(UserRole.Administrator.ToString())) // If not role does not currently exist
                    {
                        IdRoleResult = roleMgr.Create(new IdentityRole { Name = UserRole.Administrator.ToString() }); // Add the role (just once)
                        IdRoleResult = roleMgr.Create(new IdentityRole { Name = UserRole.InternalCoordinator.ToString() }); // Add the role (just once)
                        IdRoleResult = roleMgr.Create(new IdentityRole { Name = UserRole.FinanceDirector.ToString() }); // Add the role (just once)
                        IdRoleResult = roleMgr.Create(new IdentityRole { Name = UserRole.CommunityDirector.ToString() }); // Add the role (just once)
                        IdRoleResult = roleMgr.Create(new IdentityRole { Name = UserRole.President.ToString() }); // Add the role (just once)
                    }
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AddRoles", "Error creating new roles in database"); // Fatal error
                }
            }
        }

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
                    LogError.LogDatabaseError(ex, "RoleActions.ChangeProjectRole", "Error writing or deleting UserProject row"); // Fatal error
                }
            }
        }

        // For a user who is reviewing a request, use their UserRole to find a ProjectRole to use for revising the request.

        public static ProjectRole GetRevisingRole(UserRole userRole)
        {
            switch (userRole)
            {
                case UserRole.CommunityDirector:
                    return ProjectRole.RevisingCommunityDirector;
                case UserRole.FinanceDirector:
                    return ProjectRole.RevisingFinanceDirector;
                case UserRole.InternalCoordinator:
                    return ProjectRole.InternalCoordinator;
                case UserRole.President:
                    return ProjectRole.RevisingPresident;
                case UserRole.Project:
                    return ProjectRole.ProjectDirector;
                case UserRole.Administrator:
                case UserRole.Auditor:
                case UserRole.Undefined:
                default:
                    {
                        LogError.LogInternalError("RoleActions.GetRevisingRole", $"Unexpected UserRole {userRole}. No ProjectRole available"); // Fatal error
                        return ProjectRole.NoRole;
                    }
            }
        }

        // Determine whether a ProjectRole is a staff member, with all the rights and privileges incumbent on that office

        public static bool ProjectRoleIsStaff(ProjectRole role)
        {
            switch (role)
            {
                case ProjectRole.InternalCoordinator:
                case ProjectRole.RevisingCommunityDirector:
                case ProjectRole.RevisingFinanceDirector:
                case ProjectRole.RevisingPresident:
                    return true;
                case ProjectRole.ProjectDirector:
                case ProjectRole.ProjectStaff:
                    return false;
                default:
                    {
                        LogError.LogInternalError("RoleActions.ProjectRoleIsStaff", $"Unexpected ProjectRole {role}."); // Fatal error
                        return false;
                    }
            }
        }

        // In the midst of the Review process, update ReviseUserRole if appropriate. 
        // Complicated algorithm shared by Review Deposit, Review Document, and Review Expense.
        // Parameters:
        //  reviewUserRole - the UserRole of the current user right now.
        //  reviseUserRole - the stored UserRole from the request. This is the UserRole that initiated the revision cycle. We stay in
        //      this cycle until we get back to that UserRole.

        public static UserRole UpdateReviseUserRole(ReviewAction action, UserRole currentUserRole, UserRole reviseUserRole)
        {

            // If this is a Revise, just jam the currentUserRole into the Request and save it. That starts the revision cycle.
            // If this is a Return, don't change anything
            // If this is an Approve:
            //  If Request's ReviseUserRole is None, don't do anything - we're not in a revising cycle.
            //  Else If Request's ReviseUserRole is the same as the caller, reset Request's ReviseUserRole to None. Revising cycle is complet.
            // This lets Dashboards turn this row "Teal" from the time of a Revision to the time of a "re-Approval."

            switch (action)
            {
                case ReviewAction.Revise:
                    {
                        return currentUserRole;                         // Note that we are starting a "revise" cycle.
                    }
                case ReviewAction.Return:
                    {
                        break;                                          // No change
                    }
                case ReviewAction.Approve:
                    {
                        if (reviseUserRole != UserRole.None)            // If != we are in a "revise" cycle.
                        {
                            if (reviseUserRole == currentUserRole)      // If == we are approving to end a "revise" cycle
                                return UserRole.None;                   // Note that the "revise" cycle is complete
                        }
                        break;                                          // Otherwise no change
                    }
            }
            return reviseUserRole;                                      // Return original value - no change
        }

        // Determine whether a UserRole is a staff member, with all the rights and privileges incumbent on that office

        public static bool UserRoleIsStaff(UserRole role)
        {
            switch (role)
            {
                case UserRole.CommunityDirector:
                case UserRole.FinanceDirector:
                case UserRole.InternalCoordinator:
                case UserRole.President:
                    return true;
                default:
                    return false;
            }
        }
    }
}
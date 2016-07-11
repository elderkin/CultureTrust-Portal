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
                        IdRoleResult = roleMgr.Create(new IdentityRole { Name = UserRole.Coordinator.ToString() }); // Add the role (just once)
                        IdRoleResult = roleMgr.Create(new IdentityRole { Name = UserRole.FinanceDirector.ToString() }); // Add the role (just once)
                        IdRoleResult = roleMgr.Create(new IdentityRole { Name = UserRole.TrustDirector.ToString() }); // Add the role (just once)
                        IdRoleResult = roleMgr.Create(new IdentityRole { Name = UserRole.TrustExecutive.ToString() }); // Add the role (just once)
                    }
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AddRoles", "Error creating new roles in database"); // Fatal error
                }
            }
        }
    }
}
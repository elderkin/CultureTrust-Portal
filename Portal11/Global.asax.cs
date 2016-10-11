using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Data.Entity;
using Portal11.Models;
using Portal11.Logic;
using System.Web.Management;
using Portal11.ErrorLog;

namespace Portal11
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {

            // Log a note in Elmah error log saying that we have started

            LogError.LogApplicationStart();

            // Code that runs on application startup

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Create an initial set of tables, if new database

            // This has been quite a struggle! For starters, set the Initializer to CreateDatabaseIfNotExists, which will see if the tables
            // already exist and if not, will create them and invoke the Seed method to fill the usual suspects.

            //            Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>()); // The default - create the tables if they're not there already

            // A debugging option: Always toss the existing database and re-create it

            Database.SetInitializer(new PortalDatabaseInitializer()); // One-shot to fill empty database with tables

            // The "production" option. Use the Initializer to execute migrations if the model has changed.

//            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Portal11.Migrations.Configuration>());

            // Create custom user roles if they don't already exist

            RoleActions roleActions = new RoleActions();
            roleActions.AddRoles();
        }

        // Applicaztion-level error handler. We come here after a try-catch block and a page-level error handler have failed to resolve the error.
        // Try to determine the type of error and react accordingly.

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();                                 // Fetch the most recent exception

            if (ex != null)                                                 // If != there is an exception her to look at
            {
                HttpException rootProblem = ex.GetBaseException() as HttpException; // See if the root problem is an HTTP exception
                if (rootProblem != null)                                           // If != the exception was indeed an HTTP exception
                {

                    // Handle an attempt to upload a file that is too big for us

                    if (rootProblem.WebEventCode == WebEventCodes.RuntimeErrorPostTooLarge)
                    {
                        Server.ClearError();
                        Server.Transfer("UploadError.aspx");
                    }
                }
            }
//            LogError.LogInternalException(ex);                                      // Report and record Fatal error, no matter what it is

            // Now fall through to invoke the FatalError.aspx page and report to user

        }
 
    }
}
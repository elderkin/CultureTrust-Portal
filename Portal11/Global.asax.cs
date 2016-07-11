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
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Create an initial set of tables
//  fails          Database.SetInitializer(new PortalDatabaseInitializer());
//  works, but doesn't DropCreate          Database.SetInitializer<ApplicationDbContext>(null);
//  fails            Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
            Database.SetInitializer<ApplicationDbContext>(new PortalDatabaseInitializer());

            // Create custom user roles if they don't already exist
            RoleActions roleActions = new RoleActions();
            roleActions.AddRoles();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            var httpException = ex as HttpException ?? ex.InnerException as HttpException;
            if (httpException != null)
            {
                if (httpException.WebEventCode == WebEventCodes.RuntimeErrorPostTooLarge)
                {
                    Server.ClearError();
                    Server.Transfer("UploadError.aspx");
                }
            }
            if (ex is HttpUnhandledException)                               // If is this is an exception that we can report
                LogError.LogInternalException(ex);                          // Fatal error

        }
    }
}
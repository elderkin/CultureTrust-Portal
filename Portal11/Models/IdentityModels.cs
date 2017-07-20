using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Portal11.Models
{
    // You can add User data for the user by adding more properties to your User class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        // Add fields to the model

        [Required]
        public string FranchiseKey { get; set; }
        public bool Inactive { get; set; }
        public const int InactiveColumn = 3;                            // Column when displayed in GridView

        [StringLength(50)]
        public string FullName { get; set; }                            // The built-in UserName field seems conflated with Email, so provide alternative

        [StringLength(250), DataType(DataType.MultilineText)]
        public string Address { get; set; }

        // PhoneNumber field is already present in model

        public bool Administrator { get; set; }                         // The User is an Administrator. Available to any UserRole
        public UserRole UserRole { get; set; }
        public int GridViewRows { get; set; }                           // Number of rows in each GridView
        public const int GridViewRowsDefault = 10;                      // Default value
        public const int GridViewRowsMinimum = 5;                       // Can't go smaller than this
        public const int GridViewRowsMaximum = 100;                     // Can't go bigger than this
        public int LoginCount { get; set; }                             // Number of successful logins
        public DateTime LastLogin { get; set; }                         // Time of last successful login
        public bool NoEmailOnApprove { get; set; }                      // Don't Send Project Director email when request is approved (not implemented)
        public bool NoEmailOnReturn { get; set; }                       // Don't Send Project Director email when request is returned (not implemented)
        public bool NoEmailOnRefer { get; set; }                        // Don't send user email when request is referred for approval
        public bool NoEmailOnRushSubmit { get; set; }                   // Don't send user email when rush request is submitted

        // End of new fields
        //// Define names of user roles
        //public const string AdministratorRole = "Administrator",
        //                    FinanceDirectorRole = "FinanceDirector",
        //                    TrustDirectorRole = "TrustDirector",
        //                    TrustExecutiveRole = "TrustExecutive";

        public ClaimsIdentity GenerateUserIdentity(ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in 
            // CookieAuthenticationOptions.AuthenticationType
            var userIdentity = manager.CreateIdentity(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            return Task.FromResult(GenerateUserIdentity(manager));
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("PortalConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<App> Apps { get; set; }
        public DbSet<AppHistory> AppHistorys { get; set; }
        public DbSet<Dep> Deps { get; set; }
        public DbSet<DepHistory> DepHistorys { get; set; }
        public DbSet<Entity> Entitys { get; set; }
        public DbSet<Exp> Exps { get; set; }
        public DbSet<ExpHistory> ExpHistorys { get; set; }
        public DbSet<GLCodeSplit> GLCodeSplits { get; set; }
        public DbSet<Franchise> Franchises { get; set; }
        public DbSet<GLCode> GLCodes { get; set; }
        public DbSet<Grant> Grants { get; set; }
        public DbSet<GrantMaker> GrantMakers { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<PortalParameter> PortalParameters { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectClass> ProjectClasses { get; set; }
        public DbSet<ProjectClassMaster> ProjectClassMasters { get; set; }
        public DbSet<ProjectEntity> ProjectEntitys { get; set; }
        public DbSet<ProjectGrant> ProjectGrants { get; set; }
        public DbSet<ProjectPerson> ProjectPersons { get; set; }
        public DbSet<RequestHistory> RequestHistorys { get; set; }
        public DbSet<SupportingDoc> SupportingDocs { get; set; }
        public DbSet<SupportingDocTemp> SupportingDocTemps { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
//        public DbSet<ApplicationUser> ApplicationUsers { get; }
    }
}

#region Helpers
namespace Portal11
{
    public static class IdentityHelper
    {
        // Used for XSRF when linking external logins
        public const string XsrfKey = "XsrfId";

        public const string ProviderNameKey = "providerName";
        public static string GetProviderNameFromRequest(HttpRequest request)
        {
            return request.QueryString[ProviderNameKey];
        }

        public const string CodeKey = "code";
        public static string GetCodeFromRequest(HttpRequest request)
        {
            return request.QueryString[CodeKey];
        }

        public const string UserIdKey = "userId";
        public static string GetUserIdFromRequest(HttpRequest request)
        {
            return HttpUtility.UrlDecode(request.QueryString[UserIdKey]);
        }

        public static string GetResetPasswordRedirectUrl(string code, HttpRequest request)
        {
            var absoluteUri = "/Account/ResetPassword?" + CodeKey + "=" + HttpUtility.UrlEncode(code);
            return new Uri(request.Url, absoluteUri).AbsoluteUri.ToString();
        }

        public static string GetUserConfirmationRedirectUrl(string code, string userId, HttpRequest request)
        {
            var absoluteUri = "/Account/Confirm?" + CodeKey + "=" + HttpUtility.UrlEncode(code) + "&" + UserIdKey + "=" + HttpUtility.UrlEncode(userId);
            return new Uri(request.Url, absoluteUri).AbsoluteUri.ToString();
        }

        private static bool IsLocalUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))) || (url.Length > 1 && url[0] == '~' && url[1] == '/'));
        }

        public static void RedirectToReturnUrl(string returnUrl, HttpResponse response)
        {
            if (!String.IsNullOrEmpty(returnUrl) && IsLocalUrl(returnUrl))
            {
                response.Redirect(returnUrl);
            }
            else
            {
                response.Redirect("~/");
            }
        }
    }
}
#endregion

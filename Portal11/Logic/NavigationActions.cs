using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using System.Linq;
using Portal11.Models;


namespace Portal11.Logic
{
    public class NavigationActions : System.Web.UI.Page


    {
        public class userInfo
        {
            string Id { get; set; }
            string FullName { get; set; }
        };

        // Given a GridView control that has been loaded with data (via DataSource and DataBind), enable and disable the
        // navigation buttons - First, Prev, Next and Last - based on which page we are displaying

        internal static void EnableGridViewNavButtons(GridView gv)
        {

            // Enable only those navigation buttons that make sense at this point. First we have to find them.

            if (gv.PageCount > 1)                                   // If > there are multiple pages. We need nav buttons
            {
                GridViewRow pagerRow = gv.BottomPagerRow;
                Button first = (Button)pagerRow.FindControl("ButtonFirst");
                Button prev = (Button)pagerRow.FindControl("ButtonPrev");
                Button next = (Button)pagerRow.FindControl("ButtonNext");
                Button last = (Button)pagerRow.FindControl("ButtonLast");

                if (gv.PageIndex <= 0)                              // If <= we are at first page
                {
                    first.Enabled = false; prev.Enabled = false;    // First and Prev buttons are disabled
                }
                else
                {
                    first.Enabled = true; prev.Enabled = true;      // First and Prev buttons are enabled
                }

                if (((gv.PageCount - gv.PageIndex) - 1) <= 0)       // If <= we are at last page
                {
                    next.Enabled = false; last.Enabled = false;     // Next and Last buttons are disabled
                }
                else
                {
                    next.Enabled = true; last.Enabled = true;       // Next and Previous buttons are enabled
                }
            }
            return;
        }

        // Fill a particular GridView - gvEDHistory - with rows from the EDHistory table. Unusually, this GridView is used identically
        // by four pages, so filling it is factored here.

        public static void LoadAllAppHistorys(int appID, GridView gvEDHistory)
        {
            gvEDHistory.PageSize = CookieActions.FindGridViewRows();  // Set number of rows per page in grid
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var query = from edh in context.AppHistorys
                            where edh.AppID == appID
                            orderby edh.HistoryTime descending
                            select edh;

                List<AppHistory> edhs = query.ToList();                  // Fetch the available histories
                List<rowEDHistory> rows = new List<rowEDHistory>(); // Create the empty list

                foreach (var r in edhs)                                 // Cycle through the rows returned by the query
                {
                    rowEDHistory row = new rowEDHistory()       // Convert a row of the EDHistory table to a row of the GridView
                    {
                        Date = r.HistoryTime,
                        FormerStatus = EnumActions.GetEnumDescription(r.PriorAppState),
                        EstablishedBy = r.HistoryUser.FullName,
                        UpdatedStatus = EnumActions.GetEnumDescription(r.NewAppState),
                        ReasonForChange = r.HistoryNote,
                        ReturnNote = r.ReturnNote
                    };
                    rows.Add(row);                                      // Add the filled-in row to the list of rows
                }

                gvEDHistory.DataSource = rows;                        // Give it to the GridView cnorol
                gvEDHistory.DataBind();                               // And display it

                NavigationActions.EnableGridViewNavButtons(gvEDHistory); // Enable appropriate nav buttons based on page count
            }
            return;
        }

        public static void LoadAllDepHistorys(int depID, GridView gvEDHistory)
        {
            gvEDHistory.PageSize = CookieActions.FindGridViewRows();  // Set number of rows per page in grid
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var query = from edh in context.DepHistorys
                            where edh.DepID == depID
                            orderby edh.HistoryTime descending
                            select edh;

                List<DepHistory> edhs = query.ToList();                  // Fetch the available histories
                List<rowEDHistory> rows = new List<rowEDHistory>(); // Create the empty list

                foreach (var r in edhs)                                 // Cycle through the rows returned by the query
                {
                    rowEDHistory row = new rowEDHistory()       // Convert a row of the EDHistory table to a row of the GridView
                    {
                        Date = r.HistoryTime,
                        FormerStatus = EnumActions.GetEnumDescription(r.PriorDepState),
                        EstablishedBy = r.HistoryUser.FullName,
                        UpdatedStatus = EnumActions.GetEnumDescription(r.NewDepState),
                        ReasonForChange = r.HistoryNote,
                        ReturnNote = r.ReturnNote
                    };
                    rows.Add(row);                                      // Add the filled-in row to the list of rows
                }

                gvEDHistory.DataSource = rows;                        // Give it to the GridView cnorol
                gvEDHistory.DataBind();                               // And display it

                NavigationActions.EnableGridViewNavButtons(gvEDHistory); // Enable appropriate nav buttons based on page count
            }
            return;
        }

        public static void LoadAllExpHistorys(int expID, GridView gvEDHistory)
        {
            gvEDHistory.PageSize = CookieActions.FindGridViewRows();  // Set number of rows per page in grid
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var query = from edh in context.ExpHistorys
                            where edh.ExpID == expID
                            orderby edh.HistoryTime descending
                            select edh;

                List<ExpHistory> edhs = query.ToList();                  // Fetch the available histories
                List<rowEDHistory> rows = new List<rowEDHistory>(); // Create the empty list

                foreach (var r in edhs)                                 // Cycle through the rows returned by the query
                {
                    rowEDHistory row = new rowEDHistory()       // Convert a row of the EDHistory table to a row of the GridView
                    {
                        Date = r.HistoryTime,
                        FormerStatus = EnumActions.GetEnumDescription(r.PriorExpState),
                        EstablishedBy = r.HistoryUser.FullName,
                        UpdatedStatus = EnumActions.GetEnumDescription(r.NewExpState),
                        ReasonForChange = r.HistoryNote,
                        ReturnNote = r.ReturnNote
                    };
                    rows.Add(row);                                      // Add the filled-in row to the list of rows
                }

                gvEDHistory.DataSource = rows;                        // Give it to the GridView cnorol
                gvEDHistory.DataBind();                               // And display it

                NavigationActions.EnableGridViewNavButtons(gvEDHistory); // Enable appropriate nav buttons based on page count
            }
            return;
        }

        // Figure out where to go after User finishes with the current page. Usually we just know. But some pages, like ChangeUserPassword, can be invoked from
        // almost anywhere. For that case, we go where the User's context implies they came from. And we include a possible query string from the caller

        public static void NextPage(string queryString)
        {
            HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Fetch user info cookie, if any
            if (userInfoCookie != null)                             // If != there is a cookie there for us to work on
            {
                if (userInfoCookie[PortalConstants.CUserTypeStaff] != null) // If != the User is in a Staff role
                    HttpContext.Current.Response.Redirect(PortalConstants.URLStaffDashboard + queryString); // Go to the staff dashboard
                if (userInfoCookie[PortalConstants.CUserTypeProject] != null) // If != the User is on a Project, check for project cookie
                {
                    HttpCookie projectInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CProjectInfo]; // Fetch project info cookie, if any
                    if (projectInfoCookie != null)                  // If != the User is on a Project
                    {
                        HttpContext.Current.Response.Redirect(PortalConstants.URLProjectDashboard + queryString); // Go to the Project Dashboard
                    }
                }
                if (userInfoCookie[PortalConstants.CUserTypeAdmin] != null) // If != the User is an Admin
                    HttpContext.Current.Response.Redirect(PortalConstants.URLAdminMain + queryString); // Head for the main admin page
            }
            HttpContext.Current.Response.Redirect(PortalConstants.URLDefault + queryString); // We fell through all the good choices. Punt
        }

        // Two query strings - Severity and Status - fill or clear the caller-supplied Literal controls.
        // This lets Page 1 post its final status on Page 2.

        public static void ProcessSeverityStatus (Literal litSuccess, Literal litDanger)
        {
            litSuccess.Text = null; litDanger.Text = null;          // Assume neither will be used

            string severity = HttpContext.Current.Request.QueryString[PortalConstants.QSSeverity]; // Fetch severity, if any
            if (string.IsNullOrEmpty(severity))                     // If null or blank, no severity therefore no message to display
                return;

            switch (severity)                                       // Choose which literal to fill based on message severity
            {
                case PortalConstants.QSSuccess:
                    litSuccess.Text = HttpContext.Current.Request.QueryString[PortalConstants.QSStatus];
                    break;
                case PortalConstants.QSDanger:
                    litDanger.Text = HttpContext.Current.Request.QueryString[PortalConstants.QSStatus];
                    break;
                default:                                            // Anything else is an error, but just ignore it
                    break;                              
            }
            return;
        }
    }
}
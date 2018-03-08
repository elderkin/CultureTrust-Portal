using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal11.Logic;
using Portal11.Models;

namespace Portal11
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);

                // This page can be reached in several ways:
                //  1) As the default page for the web site. No logged in user. Go ask the user to log in.
                //  2) As corner case of the Login (and Switch Project) process. If the User has no projects or cancels out of
                //      the Select Project process, come here to let them contemplate their navel - but nothing else.

                HttpCookie userInfoCookie = Request.Cookies[PortalConstants.CUserInfo]; // Ask for the User Info cookie, if any
                if ((HttpContext.Current.User.Identity.Name != "") && (userInfoCookie != null)) // If true user is logged in and has cookie
                {
                    btnViewLink.NavigateUrl = PortalConstants.URLViewDoc        // Fill the link to the news page
                                + "?" + PortalConstants.QSDirectory + "=" + PortalConstants.NewsFileDir
                                + "&" + PortalConstants.QSServerFile + "=" + PortalConstants.NewsFileName
                                + "&" + PortalConstants.QSMIME + "=" + PortalConstants.NewsFileMIME
                                + "&" + PortalConstants.QSUserFile + "=" + PortalConstants.NewsOutputFile;
                    return;                                             // Case 2) Execute the Default page
                }

                // Case 1) Ask the user to log in

                Response.Redirect("~/Account/Login.aspx");

            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal11.Models;
using Portal11.ErrorLog;

namespace Portal11.Rqsts
{
    public partial class PreSelectProject : System.Web.UI.Page
    {

        // The only thing this "page" does is pass control on to the Select Project. It looks up the UserID from
        // cookies and hits the road.

        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfoCookie = Request.Cookies[PortalConstants.CUserInfo]; // Ask for the User Info cookie
            if (userInfoCookie == null)                             // If == the cookies are missing. That's an error
            {
                LogError.LogInternalError("PreSelectProject", "Missing UserInfo cookie"); // Fatal error
            }
            string userID = userInfoCookie[PortalConstants.CUserID]; // Fetch UserID from cookie

            // UserID to identify the user.
            // Command of "UserLogin" to specify our path through this function.

            Response.Redirect("~/Select/SelectProjectforUser?" + PortalConstants.QSUserID + "=" + userID + "&" +
                                                         PortalConstants.QSCommand + "=" + PortalConstants.QSCommandUserLogin);
        }
    }
}
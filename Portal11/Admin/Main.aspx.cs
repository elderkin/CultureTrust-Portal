using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal11.Models;
using Portal11.Logic;

namespace Portal11.Admin
{
    public partial class Main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If this is the very first invokation, no one has logged in yet. Remind the user to do that

                HttpCookie userInfoCookie = Request.Cookies[PortalConstants.CUserInfo];
                if (userInfoCookie == null)                                     // If null, no one logged in yet. Very first execution
                    litDangerMessage.Text = "This is the first execution of the Portal. Please create an Administrator User, restart your brower, and login.";
                else
                    NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage); // Otherwise carry out normal page startup
            }
        }
    }
}
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11
{
    public partial class Error404 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string pageName = Request.QueryString[PortalConstants.QSAspxErrorPath]; // Fetch name of page that could not be found
                if (pageName != null)                                       // If != page name is present
                    litPageName.Text = pageName;                            // Place the page name where user can see it
            }
        }
    }
}
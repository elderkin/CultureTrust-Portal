using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.ErrorLog
{
    public partial class DatabaseError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string errorText = Request.QueryString[PortalConstants.QSErrorText]; // Fetch text of the error
                if (errorText != null)                                      // If != error text is present
                    litErrorText.Text = errorText;                          // Place error text where user can see it
                string pageName = Request.QueryString[PortalConstants.QSPageName]; // Fetch name of page that encountered it
                if (pageName != null)                                       // If != page name is present
                    litPageName.Text = pageName;                            // Place the page name where user can see it
            }
        }
    }
}
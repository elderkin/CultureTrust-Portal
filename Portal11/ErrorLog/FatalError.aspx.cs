using Portal11.Models;
using System;
using System.Web.UI;

namespace Portal11.ErrorLog
{
    public partial class FatalError : System.Web.UI.Page
    {

        // Two cases:
        // 1) Page invoked by LogInternalError when our code detected an error. It logged the error, then invoked us.
        //      Query Strings:
        //          ErrorText = the text of the error that was encountered
        //          PageName = the name of the page that encountered the error
        //          MethodName = the name of the routine that the page called as error encountered

        // 2) Page invoked by runtime (as controlled by Web.config). Our Application_Error routine logged the error, then allowed the runtime to invoke us.
        //      Query Strings:
        //          aspxerrorpath = the name of the page that encountered the error

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string errorText = Request.QueryString[PortalConstants.QSErrorText]; // Fetch text of the error
                if (errorText != null)                                      // If != error text is present, case 1)
                {
                    litErrorText.Text = errorText;                          // Place error text where user can see it
                    string pageName;
                    try
                    {
                        pageName = Request.QueryString[PortalConstants.QSPageName]; // Fetch name of page that encountered it
                    }
                    catch (Exception)
                    {
                        pageName = "Unable to fetch page name from Request.QueryString.";
                    }
                    if (pageName != null)                                   // If != page name is present
                        litPageName.Text = pageName;                        // Place the page name where user can see it

                    string methodName;
                    try
                    {
                        methodName = Request.QueryString[PortalConstants.QSMethod]; // Fetch name of method that encountered it
                    }
                    catch (Exception)
                    {
                        methodName = "Unable to fetch method name from Request.QueryString.";
                    }
                    if (methodName != null)                                 // If != method name is present
                        litMethodName.Text = methodName;                    // Place the page name where user can see it
                }
                else                                                        // case 2)
                {
                    litErrorText.Text = "An Application Error has taken place."; // Text of message is fixed
                    string errorPath;
                    try
                    {
                        errorPath = Request.QueryString[PortalConstants.QSAspxErrorPath]; // Get ASP's opinion of the offending page
                    }
                    catch (Exception)
                    {
                        errorPath = "Unable to fetch ASPErrorPath from Request.QueryString.";
                    }
                    if (errorPath != null)                                  // A path is present
                        litPageName.Text = errorPath;                       // Display it as the page name
                }
                string path;
                try
                {
                    path = Request.UrlReferrer.PathAndQuery;                // Fetch the path and query parameters that got us here
                }
                catch (Exception)
                {
                    path = "Unable to fetch path from Request.UrlReferer.PathAndQuery.";
                }
                if (path != null)                                           // If != a path is present
                    litFromPathAndQuery.Text = path;                        // Display it for user

                DateTime now = System.DateTime.Now;                         // Fetch current time
                litCurrentTime.Text = now.ToString();                       // Display it for user
            }
        }
    }
}
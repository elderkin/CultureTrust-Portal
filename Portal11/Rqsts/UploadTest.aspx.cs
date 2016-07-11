using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Rqsts
{
    public partial class UploadTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //                btnUpload.Attributes.Add("onclick", "document.getElementById('" + btnUpload.ClientID + "').click();");
               // btnChoose.Attributes.Add("onclick", "document.getElementById('" + fupTest.ClientID + "').click();");
            }
            else
            {
                if (fupTest.HasFile)
                {
                    litDangerMessage.Text = "Got here, produced an error message";
                    return;
                }
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {

            // This code can enconuer an error if the user attempts to upload a file that is larger than our web.config says we can handle.
            // TODO: Research ways to catch this error

            try
            {

                if (fupTest.PostedFile == null ||
                    string.IsNullOrEmpty(fupTest.PostedFile.FileName) ||
                    fupTest.PostedFile.InputStream == null)
                    return;
                if (!fupTest.HasFile)
                    return;
                string name = fupTest.PostedFile.FileName;
                int length = fupTest.PostedFile.ContentLength;
                string type = fupTest.PostedFile.ContentType;
                string ext = Path.GetExtension(name);

                string fullname = Server.MapPath("~/Attachments/") + "file 1" + ext;
                fupTest.SaveAs(fullname);

                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + name + "\"");
                Response.AddHeader("Content-Length", length.ToString());
                Response.Flush();
                Response.TransmitFile(fullname);
                Response.End();

                return;
            }
            //    catch (System.Web.HttpApplication.EndRequest ex)
            //{
            //    return;
            //}
            catch (Exception ex)
            {
                string msg = ex.Message;
                Exception inner = ex.InnerException;
                if (ex.Message == "Thread was being aborted." && ex.InnerException == null) // If == Response.End has thrown an unhandled event
                    return;                                                     // This is its normal exit. We can continue.
                
                // For whatever reason, the upload didn't work. Tell the user that and continue
            }

        }

        protected void btnChoose_Click(object sender, EventArgs e)
        {
            return;
        }

         ////Global.asax
        //private void Application_Error(object sender, EventArgs e)
        //{
        //    var ex = Server.GetLastError();
        //    var httpException = ex as HttpException ?? ex.InnerException as HttpException;
        //    if (httpException == null) return;

        //    if (httpException.WebEventCode == WebEventCodes.RuntimeErrorPostTooLarge)
        //    {
        //        //handle the error
        //        Response.Write("Too big a file, dude"); //for example
        //    }
        //}
    }
}
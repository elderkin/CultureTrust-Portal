using Portal11.ErrorLog;
using Portal11.Logic;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Proj
{
    public partial class EditProjectClass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Create a new Project Class, Edit an existing Project Class. Communication from SelectProjectClass menu is through Query Strings:
            //      Command - "New" or "Edit" (Required)
            //      ProjectClassID - the database ID of the ProjectClass that owns this Request. If absent, invoke SelectProjectClass to find the Project Class.

            if (!Page.IsPostBack)
            {
                int ProjectClassID = Convert.ToInt32(Request.QueryString[PortalConstants.QSProjectClassID]); // Fetch the ProjectClassID, if present
                string cmd = Request.QueryString[PortalConstants.QSCommand];    // Fetch the command

                // Stash these parameters into invisible literals on the current page.

                litSavedProjectClassID.Text = ProjectClassID.ToString();
                litSavedCommand.Text = cmd;

                // Dispatch by Command

                switch (cmd)
                {
                    case PortalConstants.QSCommandNew:                  // Process a "New" command. Create new, empty ProjectClass for editing
                        {
                            // We don't have to "fill" any of the controls on the page, just use them in their initial state.
                            this.Title = "New Department";           // Show that we are creating a new ProjectClass
                            //litSuccessMessage.Text = "New Department is ready to edit";
                            break;
                        }
                    case PortalConstants.QSCommandEdit:                 // Process an "Edit" command. Read existing request, save it in same row
                        {

                            // If ProjectClassID is blank, we don't know which ProjectClass to Edit. 
                            // Invoke SelectProjectClass page to figure that out and come back here.

                            if (ProjectClassID == 0)                    // If == Query String was absent. Go find which ProjectClass, then come back to this page
                            {
                                Response.Redirect(PortalConstants.URLSelectProjectClass + "?" + PortalConstants.QSCommand + "=" + PortalConstants.QSCommandEdit);
                            }

                            // Fetch the row from the database. Fill in the panels using data rom the existing request. Lotta work!
                            litSuccessMessage.Text = "Selected Department is ready to edit";
                            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                            {
                                ProjectClass toEdit = context.ProjectClasses.Find(ProjectClassID); // Fetch ProjectClass row by its key
                                if (toEdit == null)
                                {
                                    LogError.LogInternalError("EditProjectClass", string.Format("Unable to find ProjectClassID '{0}' in database",
                                        ProjectClassID.ToString())); // Fatal error
                                }
                                LoadPanels(toEdit);                     // Fill in the visible panels from the request
                            }
                            break;
                        }
                    default:
                        LogError.LogQueryStringError("EditProjectClass", "Missing Query String 'Command'"); // Log the fatal error
                        return;                                         // Don't return any Data to caller
                }
            }
        }

        // Cancel button has been clicked. Just return to the main Admin page.

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLProjectDashboard);     // Back to the barn. Nothing to save.
        }

        // Save button clicked. "Save" means that we unload all the controls for the Project Class into a database row. 
        // If the Project Class is new, we just add a new row. If the Project Class already exists, we update it and add a history record to show the edit.
        // We can tell a Project Class is new if the Query String doesn't contain a mention of a ProjectClassID AND the literal litSavedProject ClassID is empty.
        // On the first such Save, we stash the ProjectClassID of the new Project Class into this literal so that a second Save during the same session will find
        // it and update it, rather than creating a second row.

        protected void Save_Click(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    int ProjectClassID = 0;                             // Assume no saved ID is available
                    if (litSavedProjectClassID.Text != "") ProjectClassID = Convert.ToInt32(litSavedProjectClassID.Text); // If != saved ID is available, use it  
                    if (ProjectClassID == 0)                            // If == no doubt about it, Save makes a new row
                    {
                        ProjectClass toSave = new ProjectClass();       // Get a place to hold everything
                        toSave.CreatedTime = System.DateTime.Now;       // Stamp time when ProjectClass was first created as "now"
                        UnloadPanels(toSave);                           // Move from Panels to record

                        // Find ProjectID of current project and jam it into the Project Class row

                        HttpCookie projectInfoCookie = Request.Cookies[PortalConstants.CProjectInfo]; // Find the Project Info cookie
                        if (projectInfoCookie == null)                  // If == cookie is missing
                            LogError.LogInternalError("EditProjectClass", "Unable to find Project Info Cookie"); // Fatal error
                        string projID = projectInfoCookie[PortalConstants.CProjectID]; // Fetch ProjectID from cookie
                        toSave.ProjectID = Convert.ToInt32(projID);     // Produce int version of this key

                        context.ProjectClasses.Add(toSave);             // Save new row
                        context.SaveChanges();                          // Commit the Add
                    }
                    else
                    {
                        ProjectClass toUpdate = context.ProjectClasses.Find(ProjectClassID);          // Fetch the ProjectClass that we want to update
                        UnloadPanels(toUpdate);                         // Move from Panels to record, modifying it in the process
                        context.SaveChanges();                          // Commit the Add or Modify
                    }
                }
                catch (Exception ex)
                {
                    if (ExceptionActions.IsDuplicateKeyException(ex))       // If true this is a Duplicate Key exception
                    {
                        litDangerMessage.Text = $"Another Department with the name '{txtName.Text}' already exists on this Project. Departmentes must be unique.";
                        return;                                             // Report the error to user and try again
                    }
                    LogError.LogDatabaseError(ex, "EditProjectClass", "Error writing ProjectClass row"); // Fatal error
                }
            }

            // Usually we go back to the Main Admin page. But in this case, Project Classes are usually done in bunches. So let's go
            // to the Select Project Class page and see if the user wants to edit another one.

            Response.Redirect(PortalConstants.URLSelectProjectClass + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                                                + PortalConstants.QSStatus + "=Department saved");
        }

        // Based on the selected Request Type, enable and disable the appropriate panels on the display.

        // Move values from the ProjectClass record into the Page.

        void LoadPanels(ProjectClass record)
        {
            // ProjectClass Name
            txtName.Text = record.Name;
            // Inactive flag
            chkInact.Checked = record.Inactive;
            // Default flag
            chkDefault.Checked = record.Default;
            // Description
            txtDescription.Text = record.Description;
        }

        // Move the "as edited" values from the Page into a ProjectClass record.

        void UnloadPanels(ProjectClass record)
        {
            // ProjectClass Name
            record.Name = txtName.Text;
            // Inactive Flag
            record.Inactive = chkInact.Checked;
            // Default flag
            record.Default = chkDefault.Checked;
            // Description
            record.Description = txtDescription.Text;
        }
    }
}
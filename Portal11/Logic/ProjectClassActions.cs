using Portal11.ErrorLog;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal11.Logic
{
    public class ProjectClassActions
    {

        // For a new Project, add a ProjectClass row for each row in the ProjectClassMaster table.

        public static void AddMasterProjectClasses(int projectID)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    string fran = SupportingActions.GetFranchiseKey();  // Fetch current franchise key
                    var query = from pcm in context.ProjectClassMasters
                                where pcm.FranchiseKey == fran && !pcm.Inactive
                                select pcm;                              // Find all active PCM rows for this franchise

                    List<ProjectClassMaster> master = query.ToList();   // Pull the master rows into a list
                    foreach (ProjectClassMaster pcm in master)          // Cycle through all of "our" rows in the master
                    {
                        ProjectClass pc = new ProjectClass()            // Instantiate and fill a new Project class
                        {
                            Inactive = pcm.Inactive,
                            ProjectID = projectID,                      // Supplied by caller to associate Project Class row with this Project
                            Name = pcm.Name,
                            Description = pcm.Description,
                            CreatedTime = System.DateTime.Now,
                            CreatedFromMaster  = true,
                            Default = pcm.Unrestricted
                        };
                        context.ProjectClasses.Add(pc);                 // Add the new row
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "AddMasterProjectClasses", "Error copying MPC rows to ProjectClass table"); // Fatal error
                }
            }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using Portal11.Models;

namespace Portal11.Admin
{
    public partial class AssociateUser : System.Web.UI.Page
    {
        List<ApplicationUser> AllUsers;
        Boolean UsersLoaded = false;                        // ** Doesn't work **
        List<Project> AllProjects;
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                if (!UsersLoaded)
                {
                    AllUsers = context.Users.ToList();     // SELECT all rows of the Users table and create a persistent list
                    UsersLoaded = true;
                }
            }
            return;                                    // We'll get these rows in a minute

        }

        // The return type can be changed to IEnumerable, however to support
        // paging and sorting, the following parameters must be added:
        //     int maximumRows
        //     int startRowIndex
        //     out int totalRowCount
        //     string sortByExpression
//        public IQueryable UserView_GetData()
        public IEnumerable<ApplicationUser> UserView_GetData()
        {
            return AllUsers;                                    // Let the control displa this list
        }

 
        protected void UserView_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;                                 // Cast the parameter into a GridView control
            UserNameLiteral.Text = AllUsers[gv.SelectedIndex].UserName;     // Fetch UserName of selected row

            //using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            //{
            //    AllUsers = context.Users.ToList();                          // Ugly - we just did this to load the GridView control
            //    string selectedUserID = AllUsers[row].Id;                   // Fetch ID of selected row
            //    UserNameLiteral.Text = AllUsers[row].UserName;              // Display UserName of selected row
            //}
            
        }

        // The return type can be changed to IEnumerable, however to support
        // paging and sorting, the following parameters must be added:
        //     int maximumRows
        //     int startRowIndex
        //     out int totalRowCount
        //     string sortByExpression
        public IEnumerable<Portal11.Models.Project> UserProjectView_GetData()
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                AllProjects = context.Projects.ToList();              // SELECT all rows of the Projects table and create a persistent list
            }
            return AllProjects;                                       // Let the control display this list
        }

        protected void UserProjectView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
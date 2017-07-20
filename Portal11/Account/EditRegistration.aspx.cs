using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Portal11.Models;
using Portal11.Logic;
using Portal11.ErrorLog;

namespace Portal11.Account
{
    public partial class EditRegistration : System.Web.UI.Page
    {

        // Come here from SelectUser, which has identified the User to edit
        //  UserID - the ID of the selected user
        //  FullName - the full name of the selected user.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // If the page before us has left a Query String with a status message, find it and display it

                NavigationActions.ProcessSeverityStatus(litSuccessMessage, litDangerMessage);
                litSuccessMessage.Text = "";                            // Since we display the User Name, don't also show it in Success message

                string userID = Request.QueryString[PortalConstants.QSUserID]; // Fetch the query string for User ID
                if (userID == "")                                           // No User ID. We are invoked incorrectly.
                {
                    LogError.LogQueryStringError("EditRegistration", "Invalid Query String UserID"); // Fatal error
                }

                // Fetch this user from the Identity Engine and display fields - other than Password

                using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
                {
                    var userMgr = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)); // Open path to User Manager
                    var existingUser = userMgr.FindById(userID);            // Look up user by the ID supplied by Select User
                    if (existingUser == null)                               // If null user not found
                    {
                        LogError.LogInternalError("EditRegistration", string.Format("Unable to find User by ID value '{0}'",
                            userID)); // Fatal error
                    }

                    // Load form with values returned from database

                    txtEmail.Text = existingUser.Email;
                    txtFullName.Text = existingUser.FullName;
                    txtAddress.Text = existingUser.Address;
                    txtPhoneNumber.Text = existingUser.PhoneNumber;
                    if (existingUser.GridViewRows != 0)                     // If value actually exists
                        txtGridViewRows.Text = existingUser.GridViewRows.ToString(); // Display current value

                    // Load current options into check boxes

                    foreach (ListItem item in cblOptions.Items)
                    {
                        if (item.Value == "Inactive")                       // If == load Inactive option from database
                            item.Selected = existingUser.Inactive;          // Load checkbox from database

                        else if (item.Value == UserRole.Administrator.ToString())  // If == load Administrator option from database
                            item.Selected = existingUser.Administrator;     // Load checkbox from database
                    }

                    // Load current Role (if any) into radio boxes. This code assumes that the Value names are identical to the UserRole enumerated values.

                    foreach (ListItem item in rblRole.Items)
                    {
                        if (item.Value != "")                               // If != there is no role associated with this item
                        {
                            if (existingUser.UserRole.ToString() == item.Value) // If == user currently has this role
                            {
                                item.Selected = true;                       // Set that radio button
                                break;                                      // There won't be any other roles, so no other radio buttons
                            }
                        }
                    }

                    // Load current email options into check boxes

                    foreach (ListItem item in cblEmailOptions.Items)
                    {
                        if (item.Value == "EmailOnApprove")                 // If == load option from database
                            item.Selected = !existingUser.NoEmailOnApprove;  // Load checkbox from database

                        else if (item.Value == "EmailOnReturn")             // If == load option from database
                            item.Selected = !existingUser.NoEmailOnReturn;  // Load checkbox from database

                        else if (item.Value == "EmailOnRefer")             // If == load option from database
                            item.Selected = !existingUser.NoEmailOnRefer;  // Load checkbox from database

                        else if (item.Value == "EmailOnRushSubmit")        // If == load option from database
                            item.Selected = !existingUser.NoEmailOnRushSubmit; // Load checkbox from database
                    }

                    txtLoginCount.Text = existingUser.LoginCount.ToString();
                    txtLastLogin.Text = existingUser.LastLogin.ToString();

                    litSavedUserID.Text = userID;                           // Save values for use as form processing finishes
                    litSavedEmail.Text = existingUser.Email;
                }
            }
        }

        // Back out without making any changes

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(PortalConstants.URLAdminMain);
        }

        // Lookup the user by id, then apply any fields that have changed

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                var userMgr = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)); // Open path to User Manager

                // Check for corner case in which Admin has asked to change the User's email address to an address that's already in use. Not allowed

                if (txtEmail.Text != litSavedEmail.Text)                    // If != the Admin changed the email text
                {
                    var emailUser = userMgr.FindByEmail(txtEmail.Text);     // Look for an existing user with the new email address
                    if (emailUser != null)                                  // If != such a user exists. The email is in use
                    {
                        litDangerMessage.Text = "Requested email is already in use";
                        return;
                    }
                }
                var existingUser = userMgr.FindById(litSavedUserID.Text); // Use the stashed User ID to find user
                if (existingUser == null)                               // If null user not found
                {
                    litDangerMessage.Text = "User account with specified ID was not found";
                    return;
                }

                // Load database with values returned from form

                try
                {
                    existingUser.Email = txtEmail.Text;
                    existingUser.FullName = txtFullName.Text;
                    existingUser.Address = txtAddress.Text;
                    existingUser.PhoneNumber = txtPhoneNumber.Text;
                    existingUser.UserName = txtEmail.Text;                  // Note username must be identical to email
                                                                            // This may get overwritten in the next section

                    // Process GridViewRows. A value of zero is legal; it means accept the default value

                    existingUser.GridViewRows = 0;                          // Supply default
                    if (txtGridViewRows.Text != "")                         // If != there is a value available
                        existingUser.GridViewRows = Convert.ToInt32(txtGridViewRows.Text); // Convert value user supplied

                    // Unload current options from check boxes

                    foreach (ListItem item in cblOptions.Items)
                    {
                        if (item.Value == "Inactive")                       // If == load Inactive option from database
                            existingUser.Inactive = item.Selected;          // Unload Inactive checkbox to database

                        else if (item.Value == UserRole.Administrator.ToString())  // If == load Administrator option from database
                            existingUser.Administrator = item.Selected;     // Unload Administrator checkbox to database
                    }

                    // Update database with Roles from check boxes

                    existingUser.UserRole = UserRole.Project;               // Assume they are a "vanilla" Project User, e.g., Project Director or Project Staff
                    foreach (ListItem item in rblRole.Items)
                    {
                        if (item.Value != "")                               // If != this item has a value (cooresponding to a role)
                        {
                            if (item.Selected)                              // Box is checked
                            {
                                existingUser.UserRole = EnumActions.ConvertTextToUserRole(item.Value); // Convert back into enumeration type
                                break;                                      // Radio buttons mean one and done
                            }
                        }
                    }
                    if ((existingUser.UserRole == UserRole.Project) && (existingUser.Administrator)) // If true Administrator checked, but no CS Staff role
                    {
                        litDangerMessage.Text = "A System Administrator must have a CultureTrust Staff Role.";
                        return;
                    }

                    // Unload current email options from check boxes

                    foreach (ListItem item in cblEmailOptions.Items)
                    {
                        if (item.Value == "EmailOnApprove")                 // If == load option from checkbox
                            existingUser.NoEmailOnApprove = !item.Selected; // Unload option checkbox to database

                        else if (item.Value == "EmailOnReturn")             // If == load option from checkbox
                            existingUser.NoEmailOnReturn = !item.Selected;  // Unload checkbox to database

                        else if (item.Value == "EmailOnRefer")              // If == load option from checkbox
                            existingUser.NoEmailOnRefer = !item.Selected;   // Unload checkbox to database

                        else if (item.Value == "EmailOnRushSubmit")         // If == load option from checkbox
                            existingUser.NoEmailOnRushSubmit = !item.Selected; // Unload checkbox to database
                    }

                    userMgr.Update(existingUser);                           // Write changes to database
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "EditRegistration", "Unable to update User rows in Identity database"); // Fatal error
                }
            }
            Response.Redirect(PortalConstants.URLAdminMain + "?" + PortalConstants.QSSeverity + "=" + PortalConstants.QSSuccess + "&"
                        + PortalConstants.QSStatus + "=" + "User registration successfully updated. Changes take effect at next login.");
        }
    }
}
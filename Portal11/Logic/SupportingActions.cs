using Portal11.ErrorLog;
using Portal11.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal11.Logic
{
    public class SupportingActions : System.Web.UI.Page
    {

        // Cleanup temporary Supporting Docs associated with our User ID.

        public static void CleanupTemp(string userID, Literal dan)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    if (userID != "")                               // If != there is a UserID that we can use
                    {
                        context.SupportingDocTemps.RemoveRange(context.SupportingDocTemps.Where(x => x.UserID == userID));
                        context.SaveChanges();                      // Commit that change

                        string serverRoot = HttpContext.Current.Server.MapPath(PortalConstants.SupportingDir); // Path to Supporting Docs directory on server

                        foreach (string file in Directory.GetFiles(HttpContext.Current.Server.MapPath(PortalConstants.SupportingDir),
                             userID + "*.*"))                       // Cycle through all of "our" files in this directory
                        {
                            File.Delete(file);                      // Delete next file
                        }
                    }
                }
                catch (Exception ex)
                {
                    dan.Text = "Unable to delete Supporting Documents: " + ex.Message; // Set an error message for user to see, but press on
                }
            }
        }

        // Provide the caller with the Franchise Key for this instance. Fetch the key from the UserInfoCookie of the current user

        public static string GetFranchiseKey()
        {
            if (HttpContext.Current != null)
            {
                HttpCookie userInfoCookie = HttpContext.Current.Request.Cookies[PortalConstants.CUserInfo]; // Find the User Info cookie
                if (userInfoCookie != null)                                     // There is a cookie here for us
                {
                    string franchiseKey = userInfoCookie[PortalConstants.CUserFranchiseKey];   // Fetch Franchise Key from cookie
                    if (franchiseKey != "")                                     // If != something there for us to use
                        return franchiseKey;                                    // Give the value to the caller
                }
            }
            return Franchise.LocalFranchiseKey;                             // If no other alternative, provide our default TODO: Do better
            //LogError.LogInternalError("GetFranchiseKey", "Unable to find Franchise Key in User Info Cookie. User is not logged in"); // Fatal error
            //return "";
        }

        //  2) Copy the Supporting Docs. We know that in the source Rqst, all the supporting docs are "permanent," i.e., we don't need
        //  to worry about "temporary" docs or revisions.

        public static void CopyDocs(ApplicationDbContext context, RequestType rqstType, int srcID, int destID)
        {
            string serverRoot = HttpContext.Current.Server.MapPath(PortalConstants.SupportingDir); // Path to Supporting Docs directory on server
            var query = from sd in context.SupportingDocs
                        where sd.OwnerID == srcID && sd.OwnerType == rqstType
                        select sd;                                  // Find all RqstSupporting rows for the source Rqst

            List<SupportingDoc> srcSDs = query.ToList();            // Stuff query results into list so only one active result set
            foreach (SupportingDoc srcSD in srcSDs)                 // Process each Supporting Doc in turn
            {
                SupportingDoc destSD = new SupportingDoc()
                {                                                   // Instantiate and fill a new SupportingDoc record for the destination Rqst
                    FileName = srcSD.FileName,
                    MIME = srcSD.MIME,
                    FileLength = srcSD.FileLength,
                    UploadedTime = srcSD.UploadedTime,
                    OwnerID = destID,                               // This belongs to the destination Rqst
                    OwnerType = rqstType
                };
                context.SupportingDocs.Add(destSD);                 // Add the new row
                context.SaveChanges();                              // Commit change and produce destination RqstSupportingID

                File.Copy(serverRoot + srcSD.SupportingDocID.ToString() + Path.GetExtension(srcSD.FileName),
                          serverRoot + destSD.SupportingDocID.ToString() + Path.GetExtension(destSD.FileName));
                    // Copy the source supporting doc file to create the destination supporting doc
            }
            return;
        }

        // Delete all the supporting documents and revisions (and SupportingDoc rows) for a Deposit, Document, or Expense.

        public static void DeleteDocs(RequestType rqstType, int ownerID)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    var query = from sd in context.SupportingDocs
                                where sd.OwnerID == ownerID && sd.OwnerType == rqstType
                                select sd;                              // Find all SupportingDoc rows for this ExpID

                    foreach (SupportingDoc sd in query)                 // Process each Supporting Doc in turn
                    {
                        DeleteSupportingDocFilesAndRevisionFiles(sd.SupportingDocID); // That name's pretty self-explanitory, eh?

                        context.SupportingDocs.Remove(sd);              // and the SD row from the database
                    }
                    context.SaveChanges();                              // Commit the database changes
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "DeleteDocs",
                        "Error deleting SupportingDoc rows or deleting Supporting Document"); // Fatal error
                }
            }
            return;
        }

        // See whether the selected Supporting Doc is a temporary file (or a permanent file).
        // This code assumes that a prior call has insured that a row is selected.

        public static bool IsDocTemp(ListBox lst)
        {
            string val = lst.SelectedValue;                         // Fetch the Value of the Selected row, if any. Contains ID with "T" flag
            if (val.Substring(0, PortalConstants.SupportingTempFlag.Length) == PortalConstants.SupportingTempFlag)
                // If == this entry has a corresponding row in Temp table.
                return true;                                        // Tell caller that the row contains a Temp document
            return false;
        }

        // Load the Supporting Listbox control from the database.
        //  1) Find all of the RqstSupporting rows associated with this Rqst. The Supporting Docs are in their final spot.
        //  2) Create a row in the Listbox control for this Supporting

        public static void LoadDocs(RequestType rqstType, int ownerID, ListBox lst, Literal dan)
        {
           using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  1) Find all of the RqstSupporting rows associated with this Rqst

                    var query = from r in context.SupportingDocs
                                where r.OwnerID == ownerID && r.OwnerType == rqstType // Specify the rows we want
                                select r;
                    foreach (SupportingDoc rs in query)             // Loop through all the rows that the query returned
                    {

                       //  2) Create a row in the Listbox control for this Supporting

                        lst.Items.Add(new ListItem(rs.FileName, rs.SupportingDocID.ToString())); // Value is the ID of this row
                    }
                }
                catch (Exception ex)
                {
                    dan.Text = "Unable to reload Supporting Document: " + ex.Message;
                    return;                                         // This failed, but press on no matter what
                }
            }
        }

        // Remove the selected Supporting Doc.
        //  1) Make sure that a row is selected. If not, its an error.
        //  2A) If the row is for a Temp, fetch the row of the SupportingDocTemp table, delete the file and delete the row.
        //  2B) If the row is for a permanent, do the same thing with the SupportingDoc table
        //  3) Remove the selected row from the Listbox.

        public static void RemoveDoc(ListBox lst, Literal suc, Literal dan)
        {
            suc.Text = ""; dan.Text = "";                           // Start with a clean slate of message displays

            //  1) Make sure that a row is selected. If not, its an error.

            string val = lst.SelectedValue;                         // Fetch the Value of the Selected row, if any. Contains ID with "T" flag
            ListItem item = lst.SelectedItem;                       // Fetch the Selected row, which contains filename
            if (val == "" || item == null)                          // If == no row selected, that's an error
            {
                dan.Text = "Please select a Supporting Document to Remove";
                return;
            }
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    string serverRoot = HttpContext.Current.Server.MapPath(PortalConstants.SupportingDir); // Path to Supporting Docs directory on server
                    if (val.Substring(0, PortalConstants.SupportingTempFlag.Length) == PortalConstants.SupportingTempFlag)
                        // If == this entry has a corresponding row in Temp table.
                    {

                        //  2A) If the row is for a Temp, fetch the row of the SupportingDocTemp table, delete the file and delete the row.
                        //  The Value contains the letter "T" plus the ID of the row in the SupportingDocTemp table

                        int sdtID = Convert.ToInt32(val.Substring(PortalConstants.SupportingTempFlag.Length,
                                                    val.Length - PortalConstants.SupportingTempFlag.Length)); // Remove the "T" and extract row ID
                        SupportingDocTemp sdt = context.SupportingDocTemps.Find(sdtID); // Use that ID to fetch that row
                        if (sdt == null)                            // If == couldn't find the row
                            LogError.LogInternalError("RemoveDoc", string.Format(
                                "SupportingDocTemporaryID '{0}' from selected GridView row not found in database",
                                sdtID.ToString()));                 // Log fatal error

                        File.Delete(serverRoot + sdt.FileName);     // Delete the file from the /Supporting directory
                        context.SupportingDocTemps.Remove(sdt);     // Delete the row from the database
                    }
                    else                                            // This entry has a corresponding row in the SupportingDoc table
                    {

                        //  2B) If the row is for a permanent, do the same thing with the SupportingDoc table
                        //  The Value just contains the ID of the row in the SupportingDoc table

                        int sdID = Convert.ToInt32(val);            // Convert SupportingDoc row ID
                        SupportingDoc sd = context.SupportingDocs.Find(sdID); // Use the ID to fetch that row
                        if (sd == null)                             // If == couldn't find the row
                            LogError.LogInternalError("RemoveDoc", string.Format(
                                "SupportingDocID '{0}' from selected GridView row not found in database",
                                sdID.ToString()));                  // Log fatal error
                        DeleteSupportingDocFilesAndRevisionFiles(sd.SupportingDocID); // Delete all the underlying files
                        context.SupportingDocs.Remove(sd);          // and the row from the database
                    }
                    context.SaveChanges();                          // Commit that change

                    //  3) Remove the selected row from the Listbox.

                    lst.Items.Remove(item);                         // Yes, do that
                    suc.Text = "Supporting Document successfully removed"; // Let the user know
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "RemoveDoc", "Error processing SupportingDoc row"); // Fatal error
                }
            }
        }

        // Revise the selected Supporting Doc. This works only for Document Requests.
        //  1) Make sure that a row is selected. If not, its an error.
        //  2) If the row is for a Temp, ignore this request. We don't revise Temp docs.
        //  3) Locate the Rqst row and SD row for the previous document
        //  4) Prepare a new History row
        //  5) Rename the previous document to contain the ID of the history row.
        //  6) Load the new document, but as a permanent file, not a temporary file
        //  7) Update the selected row from the Listbox.

        public static void ReviseDoc(FileUpload fup, ListBox lst, int docID, Literal suc, Literal dan)
        {
            suc.Text = ""; dan.Text = "";                           // Start with a clean slate of message displays

            //  1) Make sure that a row is selected. If not, its an error.

            string val = lst.SelectedValue;                         // Fetch the Value of the Selected row, if any. Contains ID with "T" flag
            ListItem item = lst.SelectedItem;                       // Fetch the Selected row, which contains filename
            if (val == "" || item == null)                          // If == no row selected, that's an error
            {
                dan.Text = "Please select a Supporting Document to Remove"; // Report the error
                return;
            }

            //  2) If the row is for a Temp, ignore this request. We don't revise Temp docs.

            if (val.Substring(0, PortalConstants.SupportingTempFlag.Length) == PortalConstants.SupportingTempFlag)
            // If == this entry has a corresponding row in Temp table. We don't revise temps.
            {
                dan.Text = "Please save the request before revising a Supporting Document"; // Report the error
                return;
            }

            // 2.5 Do some quick checks on the newly uploaded file before we change anything in the database

            string newName = fup.PostedFile.FileName;          // Fetch the name of the file to upload
            if (newName.Contains("#"))                         // If true, the file name contains a character we can't handle
            {
                dan.Text = "Name of selected file contains a pound sign character ('#') which the Portal cannot process. Please rename the file or choose another file.";
                return;
            }
            int newLength = fup.PostedFile.ContentLength;      // Fetch the length of the file - it might be too big for us
            if (newLength > PortalConstants.MaxSupportingFileSize) // If > file is too big to upload
            {
                dan.Text = "Selected file is too large to upload";
                return;
            }

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {
                    //  3) Locate the Rqst row and SD row for the previous document

                    Doc doc = context.Docs.Find(docID);             // Fetch Doc row by its key
                    if (doc == null)                                // If == couldn't find the row
                        LogError.LogInternalError("EditDocument, ReviseDoc", $"Unable to locate DocumentID '{docID.ToString()}' in database"); // Log fatal error

                    int sdID = Convert.ToInt32(val);                // Convert SupportingDoc row ID
                    SupportingDoc sd = context.SupportingDocs.Find(sdID); // Use the ID to fetch that row
                    if (sd == null)                                 // If == couldn't find the row
                        LogError.LogInternalError("ReviseDoc", 
                            $"SupportingDocID '{sdID.ToString()}' from selected GridView row not found in database"); // Log fatal error
                    if (sd.OwnerType != RequestType.Document)       // If != SD is connected to the wrong type of request
                        LogError.LogInternalError("ReviseDoc",
                            $"SupportingDocID '{sdID.ToString()}' is not associated with a Document Request"); // Log fatal error

                    //  4) Prepare a new History row. Store it early so we can have it's database ID.

                    DocHistory hist = new DocHistory();             // Get a place to build a new DepHistory row
                    StateActions.CopyPreviousState(doc, hist, "Supporting Document '" + sd.FileName + "' Revised to '" + newName + "'"); // Fill the DocHistory row from Doc request row
                    hist.SupportingDocID = sdID;                    // Connect the history to the supporting doc so we can track down this revision
                    hist.FileName = sd.FileName;                    // Save name of the old file
                    hist.MIME = sd.MIME;                            // Save type of the old file
                    hist.NewDocState = hist.PriorDocState;          // State of the request hasn't changed by a supporting document revision
                    hist.HistoryTime = System.DateTime.Now;         // Show that this revision is happening now, not when the owner Doc row was written

                    context.DocHistorys.Add(hist);                  // Add the new DocHistory row
                    context.SaveChanges();                          // Commit changes to get the ID of the new DocHistory row

                    //  5) Rename the previous document to contain the ID of the history row.
                    //    Old name: sdID.type
                    //    New name: sdID_histID.type

                    string serverRoot = HttpContext.Current.Server.MapPath(PortalConstants.SupportingDir); // Path to Supporting Docs directory on server
                    string oldExt = Path.GetExtension(sd.FileName);    // Parse out the file extension
                    File.Move(serverRoot + sd.SupportingDocID.ToString() + oldExt,
                              serverRoot + sd.SupportingDocID.ToString() + "_" + hist.DocHistoryID.ToString() + oldExt); // Change the name of the file

                    //  6) Load the new document, but as a permanent file, not a temporary file. Use the same filename (the sdID), but the
                    //  new file's type. Overwrite the SupportingDoc row to describe it.

                    string newExt = Path.GetExtension(newName);         // We'll propagate the extension
                    string uploadFullName = serverRoot + sd.SupportingDocID.ToString() + newExt; // And the full name including path
                    fup.SaveAs(uploadFullName);                     // Copy the Supporting file to server using our manufactured name

                    sd.FileName = newName;                              // Copy description of the new file into the SupportingDoc row
                    sd.MIME = fup.PostedFile.ContentType;               // Knowing MIME type helps with download
                    sd.FileLength = newLength;
                    sd.UploadedTime = System.DateTime.Now;
                    sd.OwnerID = docID;                                 // Connect this SupportingDoc to owner Document Request
                    sd.OwnerType = RequestType.Document;                // So far, we can only revise a Document Request's supporting doc

                    context.SaveChanges();                              // Update the sd row

                    //  7) Update the selected row from the Listbox to display the new file name.

                    lst.Items.FindByValue(val).Text = newName;          // Update the filename displayed in the list
                    suc.Text = "Supporting Document successfully revised"; // Let the user know
                }
                catch (Exception ex)
                {
                    LogError.LogDatabaseError(ex, "ReviseDoc", "Error processing SupportingDoc row"); // Fatal error
                }
            }
        }

        // Unload files from Listbox control. That is, move each supporting file from temporary to permanent status.
        //  1) Find each row in the ListBox of supporting files. A rowID preceded by "T" indicates an entry in the RqstSupportingTemp table
        //  2) Find the row of the RqstSupportingTemp table that corresponds to this file.
        //  3) Add a new row to the RqstSupporting table, copying RqstSupportingTemp info. This generates an ID
        //  4) Rename the supporting file in the /Supportings directory using the ID to produce a permanent filename
        //  5) Update the row of the Listbox to remove the "T" marker, since the file is now in its final resting place
        //  6) Remove the rows of the RqstSupportingTemp table.

        public static void UnloadDocs(ListBox lst, string userID, RequestType rqstType, int ownerID, Literal dan)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  1) Find each row in the ListBox of supporting files.

                    int index = 0;                                  // Track the row of the list
                    foreach (ListItem row in lst.Items)             // Process the rows of the list, one by one
                    {
                        if (row.Value.Substring(0, PortalConstants.SupportingTempFlag.Length) == PortalConstants.SupportingTempFlag)
                            // If == this entry has a corresponding row in Temp table.
                        {
                            string filename = row.Text;             // Pull original filename from list row
                            string ext = Path.GetExtension(filename); // Parse out the file extension
                            int sdtID = Convert.ToInt32(row.Value.Substring(PortalConstants.SupportingTempFlag.Length,
                                row.Value.Length - PortalConstants.SupportingTempFlag.Length)); // Pull rowID of RqstSupportingTemp row, less "T" flag

                            //  2) Find the row of the RqstSupportingTemp table that corresponds to this file.

                            SupportingDocTemp sdt = context.SupportingDocTemps.Find(sdtID); // Fetch the row that we want
                            if (sdt == null)                        // If == unable to find the rst row by its ID. An internal error
                                LogError.LogInternalError("UnloadDocs", string.Format(
                                    "RqstSupportingTempID '{0}' from selected GridView row not found in database",
                                    sdtID.ToString())); // Fatal error

                            //  3) Add a new row to the RqstSupporting table, copying RqstSupportingTemp info. This generates an ID

                            SupportingDoc sd = new SupportingDoc()  // Instantiate a new RqstSupporting row
                            {
                                FileName = filename,                // Load original name from Listbox
                                MIME = sdt.MIME,                    // Copy file type
                                FileLength = sdt.FileLength,        // Copy length of the file
                                UploadedTime = sdt.UploadedTime,    // Copy timestamp for posterity
                                OwnerID = ownerID,                  // Connect this Supporting to the Rqst
                                OwnerType = rqstType                // Remember whether Appproval, Deposit or Expense
                            };
                            context.SupportingDocs.Add(sd);         // Add the new row
                            context.SaveChanges();                  // Commit the database changes, generate ID

                            //  4) Rename the supporting file in the /Supportings directory using the ID to produce a permanent filename

                            string serverRoot = HttpContext.Current.Server.MapPath(PortalConstants.SupportingDir); // Path to Supporting Docs directory on server
                            File.Move(serverRoot + sdt.FileName, serverRoot + sd.SupportingDocID.ToString() + ext); // The new name is just the new row ID
                                // which is guaranteed do be unique

                            //  5) Update the row of the Listbox to remove the "T" marker, since the file is now in its final resting place

                            lst.Items[index].Value = sd.SupportingDocID.ToString(); // Update the row to get rid of the "T"
                        }
                        index++;                                    // Move to next row of ListBox
                    }

                    //  6) Remove the rows of the RqstSupportingTemp table. Blow out everything associated with our UserID.
                    //      Yes, we could just Remove each row as we process it above. But there are corner cases that leave an orphaned
                    //      row in the RST table, e.g., when the user shuts down the browser in mid-Save. So this is usually a bigger hammer
                    //      that we need.

                    context.SupportingDocTemps.RemoveRange(context.SupportingDocTemps.Where(x => x.UserID == userID));
                    context.SaveChanges();                          // Commit that change
                }
                catch (Exception ex)
                {
                    dan.Text = "Unable to upload Supporting Document: " + ex.Message;
                    return;                                         // This failed, but press on no matter what
                }
            }
        }

        // Upload CSV - completing a click of the FileUpload control. This is not a supporting document, but an Excel file
        // that the user wants us to read. But to read it, it must reside on the server. So our job is to get it there.
        // There are no database implications to this, just file movement.
        //
        // This isn't a supporting doc action, but it's so similar I thought it fit best here.

        public static bool UploadCSV(FileUpload fup, string serverFileName, Literal suc, Literal dan)
        {
            try
            {

                //  Pull the relevant information out of the FileUpload control.

                string name = fup.PostedFile.FileName;          // Fetch the name of the file to upload
                string ext = Path.GetExtension(name).ToUpper(); // We'll propagate the extension
                string type = fup.PostedFile.ContentType.ToUpper(); // Content type helps with download
//                if ((type != PortalConstants.CSVType) || (ext != PortalConstants.CSVExt)) // If != file is not a CSV
                if ((ext != PortalConstants.CSVExt))
                {
                    dan.Text = $"Selected file '{name}' is not a valid Comma-Separated Value (CSV) file";
                    return false;
                }
                string serverRoot = HttpContext.Current.Server.MapPath(PortalConstants.SupportingDir); // Path to Supporting Docs directory on server

                //  Save the file to the /Supporting directory using the ID to produce a filename

                string uploadFullName = serverRoot + serverFileName; // Formulate name of destination file
                fup.SaveAs(uploadFullName);                         // Copy the Supporting file to server using our manufactured name
                return true;
            }
            catch (Exception ex)
            {
                dan.Text = $"Unable to upload CSV file: '{ex.Message}'";
                return false;                                       // Tell caller this failed
            }
        }

        // Upload file - completing a click of the FileUpload control. Get ready for a wild ride!
        //  1) Pull the relevant information out of the FileUpload control.
        //  2) Add a new row to the SupportingDocTemp table. This generates an ID
        //  3) Save the file to the /Supportings directory using the ID to produce a temporary filename
        //  4) Update the SupportingDocTemp row to include the temporary filename
        //  5) Add a line to the Listbox control describing the new Supporting
        //  6) Tell the user what a wonderful job we did

        public static void UploadDoc(FileUpload fup, ListBox lst, Literal userID, Literal suc, Literal dan)
        {
            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {
                try
                {

                    //  1) Pull the relevant information out of the FileUpload control.

                    string name = fup.PostedFile.FileName;          // Fetch the name of the file to upload
                    if (name.Contains("#"))                         // If true, the file name contains a character we can't handle
                    {
                        dan.Text = "Name of selected file contains a pound sign character ('#') which the Portal cannot process. Please rename the file or choose another file.";
                        return;
                    }
                    int length = fup.PostedFile.ContentLength;      // Fetch the length of the file - it might be too big for us
                    if (length > PortalConstants.MaxSupportingFileSize) // If > file is too big to upload
                    {
                        dan.Text = "Selected file is too large to upload";
                        return;
                    }
                    string type = fup.PostedFile.ContentType;       // Content type helps with download
                    string ext = Path.GetExtension(name);           // We'll propagate the extension
                    string serverRoot = HttpContext.Current.Server.MapPath(PortalConstants.SupportingDir); // Path to Supporting Docs directory on server

                    //  2) Add a new row to the SupportingDocTemp table. This generates an ID.

                    SupportingDocTemp sdt = new SupportingDocTemp() // Prepare an empty row
                    {                                               // Don't know filename yet because it will include the ID of the new SDT row for uniqueness
                        MIME = type,
                        FileLength = length,
                        UploadedTime = System.DateTime.Now,         // Timestamp row for posterity
                        UserID = userID.Text                        // Use our UserID for uniqueness
                    };
                    context.SupportingDocTemps.Add(sdt);            // Add the new row, generate ID
                    context.SaveChanges();                          // Commit the new row

                    //  3) Save the file to the /Supportings directory using the ID to produce a filename

                    string uploadName = userID.Text + " " + sdt.SupportingDocTempID.ToString() + ext; // Formulate file name
                    string uploadFullName = serverRoot + uploadName; // And the full name including path
                    fup.SaveAs(uploadFullName);                     // Copy the Supporting file to server using our manufactured name

                    //  4) Update the SupportingDocTemp row to include the temporary filename

                    SupportingDocTemp toUpdate = context.SupportingDocTemps.Find(sdt.SupportingDocTempID); // Find the one that we just wrote
                    toUpdate.FileName = uploadName;                 // Insert the actual filename
                    context.SaveChanges();                          // Commit the new row including that change

                    //  5) Add a line to the Listbox control describing the new Supporting

                    lst.Items.Add(new ListItem(name, PortalConstants.SupportingTempFlag + sdt.SupportingDocTempID.ToString()));
                    // Add the filename (visible) 
                    // and "flagged-as-temp" row ID (invisible) in the Listbox

                    //  6) Tell the user what a wonderful job we did

                    suc.Text = "Successfully uploaded Supporting Document: " + name;
                    return;
                }
                catch (Exception ex)
                {
                    dan.Text = "Unable to upload Supporting Document: " + ex.Message;
                    LogError.LogSupportingDocumentError(ex, "UploadDoc", "Unable to upload Supporting Document"); // Log error
                    return;                                         // This failed, but press on no matter what
                }
            }
        }

        // View a Supporting Document, given the SupportingDoc row. The server does the real work here.
        //
        // Two cases here: 1A) We are viewing a temporary file, described by a SupportingDocTemp row. If so, the sdIDstring starts with the
        // letter "T". In this case, we get the name of the destination file from the callse. 1B) We are viewing a permanent file, 
        // described by a SupportingDoc row. If so, the sdIDstring is just a number and we can find the destination file name in the SD row.

        public static void ViewDoc(ListBox lst, Literal dan)
        {
            string sourceFileName = ""; int sourceFileLength = 0; string sourceMIME = ""; string destFileName = "";

            ViewDocPrep(lst, out sourceFileName, out sourceFileLength, out sourceMIME, out destFileName); // Hit the database to find these values

            try
            {
                string serverRoot = HttpContext.Current.Server.MapPath(PortalConstants.SupportingDir); // Path to Supporting Docs directory on server

                //  2) Command the server to download the file to the browser

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.ClearContent();                // Just download the file and let user figure it out
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + destFileName + "\"");
                // Name of file after download
                HttpContext.Current.Response.AddHeader("Content-Length", sourceFileLength.ToString());
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.TransmitFile(serverRoot + sourceFileName); // Name of source file on server /Supporting directory
                HttpContext.Current.Response.End();                         // Download that baby
                return;
            }
            catch (Exception ex)
            {
                Exception inner = ex.InnerException;
                if (ex.Message == "Thread was being aborted." && ex.InnerException == null) // If == Response.End has thrown an unhandled event
                    return;                                                 // This is its normal exit. We can continue.
                else
                {
                    LogError.LogSupportingDocumentError(ex, "ViewDoc", "Unable to download and view Supporting Document"); // Log error
                    dan.Text = "Unable to download Supporting Document: " + destFileName; // Display the offending document
                    return;                                                 // This failed, but press on no matter what
                }
            }
        }

        // A row in the supportinc document list has been selected. Formulate a (complicated) URL that can go into the "View" button
        // to open the doc in a new window.

        public static string ViewDocUrl(ListBox lst)
        {
            string sourceFileName = ""; int sourceFileLength = 0; string sourceMIME = ""; string destFileName = "";

            ViewDocPrep(lst, out sourceFileName, out sourceFileLength, out sourceMIME, out destFileName); // Hit the database to find these values

            return PortalConstants.URLViewDoc + "?" + PortalConstants.QSServerFile + "=" + sourceFileName + "&" +
                    PortalConstants.QSMIME + "=" + sourceMIME + "&" + PortalConstants.QSUserFile + "=" + destFileName;
        }

        // Delete all supporting documents and revisions given the SupportingDocID

        static void DeleteSupportingDocFilesAndRevisionFiles(int sdID)
        {
            string serverRoot = HttpContext.Current.Server.MapPath(PortalConstants.SupportingDir); // Path to Supporting Docs directory on server
            DirectoryInfo dir = new DirectoryInfo(serverRoot); // Connect to the directory in which supporting documents live  
            string sdfiles = sdID.ToString() + "*.*";           // Build up name of all SDs and revisions. Note: all file types deleted

            foreach (var file in dir.EnumerateFiles(sdfiles)) // Use wild card to find each document file in turn
            {
                file.Delete();                              // Delete next file
            }

        }
        // Given a ListItem in the Supporting Document list, fetch the name, length, and MIME type of the supporting document file.
        // In this context, "source file" means the file on the server's /Supporting directory where the user's original file was copied.

        static void ViewDocPrep(ListBox lst, out string sourceFileName, out int sourceFileLength, out string sourceMIME, out string destFileName)
        {
            ListItem item = lst.SelectedItem;                       // Fetch the Selected row
            if (item == null)                                       // If there is no selected row, we never should have gotten here
                LogError.LogInternalError("ViewDocPrep", "No supporting document row selected in list.");
            string sdIDstring = lst.SelectedValue;                  // Fetch the Value of the Selected row. This identifies the supporting doc

            using (Models.ApplicationDbContext context = new Models.ApplicationDbContext())
            {

                //  Figure out whether the Supporting Doc is temporary or permanent

                if (sdIDstring.Substring(0, PortalConstants.SupportingTempFlag.Length) == PortalConstants.SupportingTempFlag)
                // If == this entry has a corresponding row in Temp table.
                {

                    //  1A) If temporary, get details from the SupportingDocTemp row
                    //  The Value contains the letter "T" plus the ID of the row in the SupportingDocTemp table

                    int sdtID = Convert.ToInt32(sdIDstring.Substring(PortalConstants.SupportingTempFlag.Length,
                                                sdIDstring.Length - PortalConstants.SupportingTempFlag.Length));
                    // Remove the "T" and extract row ID
                    SupportingDocTemp sdt = context.SupportingDocTemps.Find(sdtID); // Use the ID to fetch that database row
                    if (sdt == null)                                        // If == couldn't find the row
                        LogError.LogInternalError("ViewDocPrep", $"RequestSupportingTempID '{sdtID}' from selected GridView row not found in database");
                    // Log fatal error

                    sourceFileName = sdt.FileName;                          // Source of the download - a temporary file
                    sourceFileLength = sdt.FileLength;                      // Report file length
                    sourceMIME = sdt.MIME;                                  // Report file type
                    destFileName = item.Text;                               // Pull the file name from the ListBox row, where it was displayed
                }
                else
                {

                    //  1B) If permanent, get details from the RqstSupporting row
                    //  The Value just contains the ID of the row in the RqstSupporting table

                    int sdID = Convert.ToInt32(sdIDstring);                 // Convert RqstSupporting row ID
                    SupportingDoc sd = context.SupportingDocs.Find(sdID);   // Use the ID to fetch that database row
                    if (sd == null)                                         // If == couldn't find the row
                        LogError.LogInternalError("ViewDocPrep", "RequestSupportingID from selected GridView row not found in database");
                    // Log fatal error

                    sourceFileName = sd.SupportingDocID.ToString() + Path.GetExtension(sd.FileName); // Formulate source of the download
                    sourceFileLength = sd.FileLength;                       // Report file length
                    sourceMIME = sd.MIME;                                   // Report file type
                    destFileName = sd.FileName;                             // Report user's original name for the file
                }
            }
            return;
        }
    }
}
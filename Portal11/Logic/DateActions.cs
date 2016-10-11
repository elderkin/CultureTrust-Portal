using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Portal11.Logic
{
    public class DateActions
    {

        // Take a date, convert it to a text string and store it in a text box.

        public static string LoadDateIntoTxt(DateTime date)
        {
            if (date > SqlDateTime.MinValue)                                // If > there is a useful date value
                return date.ToShortDateString();                            // I know, simple. But it's all done here in case it needs to change
            else
                return "";
        }

        // Take a date, convert it to a text string and store it in a text box. Then place the date in an associated calendar.

        public static void LoadDateIntoTxtCal(DateTime date, TextBox txt, Calendar cal)
        {
            if (date > SqlDateTime.MinValue)                                // If > there is a useful date value
            {
                txt.Text = LoadDateIntoTxt(date);
                cal.SelectedDate = date;
                cal.VisibleDate = date;
            }
            return;
        }

        // Take the Text from a TextBox, convert it carefully to a DateTime value, and insert it into a Calendar as the SelectedDate.

        public static void LoadTxtIntoCal(TextBox txt, Calendar cal)
        {
            if (txt.Text != "")                                             // If != there is a value in the text box
            {
                try
                {
                    cal.SelectedDate = Convert.ToDateTime(txt.Text);        // Attempt to convert string to date and place in calendar		
                }
                catch (FormatException)                                     // A date conversion error
                {
                    cal.SelectedDate = (DateTime)SqlDateTime.MinValue;      // Don't select a date in the calendar
                }
                cal.VisibleDate = cal.SelectedDate;                         // Open calendar to month containing selected date
            }
            return;
        }

        // Take the Text from a TextBox, convert it carefully to a DateTime value, and return it

        public static DateTime LoadTxtIntoDate(TextBox txt)
        {
            if (txt.Text != "")                                             // If != there is a value in the text box
            {
                try
                {
                    return Convert.ToDateTime(txt.Text);                    // Attempt to convert string to date and place in calendar		
                }
                catch (FormatException)                                     // A date conversion error
                {
                    ;                                                       // Survive the error
                }
            }
            return (DateTime)SqlDateTime.MinValue;                          // Return a placebo value
        }

        // Validate a typed-in date value from a text box
        public static void ValidateDateInput(TextBox txt, string source, Literal lit)
        {
            if (txt.Text != "")                                                 // If != there is a value in the text box
            {
                try
                {
                    DateTime temp;
                    temp = Convert.ToDateTime(txt.Text);                        // Attempt to convert string to date
                    txt.Text = temp.ToShortDateString();                        // If it worked, put it back in text box in pretty format
                    lit.Text = "";                                              // Erase prior error message
                }
                catch (FormatException)                                         // A date conversion error
                {
                    txt.Text = "";                                              // For now, just clear the bad date
                    lit.Text = "Invalid '" + source + "' value. Format is mm/dd/yyyy"; // Report the error
                }
            }
            return;
        }
    }
}
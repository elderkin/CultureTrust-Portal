using Portal11.Models;
using System;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Portal11.Logic
{
    public static class ExtensionActions
    {

        // Try to be a little cute. Among Quantity, Cost Per Item and Amount, if one is blank, try to fill it in for user.
        // Assumes that Quantity is an int, Cost is a decimal, and amount is a decimal.

        public static void FillQCA(TextBox quantity, TextBox cost, TextBox amount)
        {
            if (quantity.Text != "" && cost.Text != "" && amount.Text == "")    // Case 1: Quantity specified, Cost specified, Amount blank
            {
                int q = ExtensionActions.LoadTxtIntoInt(quantity);              // Convert quantity into int
                decimal c = ExtensionActions.LoadTxtIntoDecimal(cost);          // Convert cost per item into decimal
                decimal a = q * c;                                              // Calculate total amount
                amount.Text = a.ToString("C");                                  // Convert product to text and display
            }
        }

        // Enable/Disable selected list items in the Payment Methods Radio Button List

        public static void EnableRdoListItems(RadioButtonList rdo, bool check = true, bool creditcard = true, bool eft = true, bool invoice = true)
        {
            rdo.Items.FindByValue(PaymentMethod.Check.ToString()).Enabled = check;
            rdo.Items.FindByValue(PaymentMethod.CreditCard.ToString()).Enabled = creditcard;
            rdo.Items.FindByValue(PaymentMethod.EFT.ToString()).Enabled = eft;
            rdo.Items.FindByValue(PaymentMethod.Invoice.ToString()).Enabled = invoice;
            return;
        }

        // Take an enum and set the approprate button in a Radio Button List. One of the buttons must be set.

        public static void LoadEnumIntoRdo(Enum val, RadioButtonList rdo)
        {
            rdo.SelectedIndex = -1;                         // Blow out any previous selection
            string setting = val.ToString();                // Get the string version of the enum value
            rdo.Items.FindByValue(setting).Selected = true; // Set that button in the Radio Button List
            return;
        }

        // Take a Yes/No enum and set the appropriate button in a Radio Button List and back to enum. This is a special case since neither of
        // the radio buttons may be set

        public static void LoadYesNoIntoRdo(YesNo val, RadioButtonList rdo)
        {
            if (val == YesNo.None)                          // If == no radio button should be set
                rdo.SelectedIndex = -1;                     // Note no button set
            else
                LoadEnumIntoRdo(val, rdo);                  // Now we know a button will be set, so use the simpler case method
            return;
        }

        public static YesNo LoadRdoIntoYesNo(RadioButtonList rdo)
        {
            string selected = rdo.SelectedValue;            // Find which if any radio button is pressed
            if (selected != "")                             // If != one of the buttons is selected
                return EnumActions.ConvertTextToYesNo(selected); // Convert string to enum value
            return YesNo.None;
        }

        // Carefully convert a string from a TextBox containing a currency value into a decimal value.
        // Note the marker for a blank text field. This marker is recognized by LoadDecimalIntoTxt, above.

        public static decimal LoadTxtIntoDecimal(TextBox txt)
        {
            decimal bucks;
            if (txt.Text != "")                             // If != non-blank text field is present
            {
                bool convt = decimal.TryParse(txt.Text, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out bucks);
                // Use a localization-savvy method to convert string to decimal
                if (convt)                                  // If true conversion as successful
                    return bucks;                           // Return converted value as decimal
            }
            return -1;                                      // Else, return marker that field is unfilled
        }

        public static string LoadDecimalIntoTxt(decimal bucks)
        {
            if (bucks != -1)                                // If != there is a "real" value in the field
                return bucks.ToString("C");                 // Convert value to a currency string and return
            return "";                                      // Otherwise, return an empty string
        }

        // Carefully convert a string from a TextBox containing a number into an int destination

        public static int LoadTxtIntoInt(TextBox txt)
        {
            try
            {
                return Convert.ToInt32(txt.Text);           // Plunge into the conversion. Hey, it might work!
            }
            catch (Exception)
            {
                return 0;                                   // But if it doesn't, just return a value of zero
            }
        }

        // Trim a string to a specified maximum length

        public static string TrimString(this string name, int maxLength)
        {
            if (string.IsNullOrEmpty(name))                        // If true, the string is null or empty, just return it
                return name;
            return name.Length <= maxLength ? name : name.Substring(0, maxLength) + "..."; // If <= just return the string; else truncate and append ...
        }

        // Handy little tool to drill into an Exception to find the "root" problem

        public static Exception GetOriginalException(this Exception ex)
        {
            if (ex.InnerException == null) return ex;

            return ex.InnerException.GetOriginalException();
        }
    }
}
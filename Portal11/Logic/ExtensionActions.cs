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

        public static bool FillQCA(TextBox quantity, TextBox cost, TextBox amount)
        {
            if (!string.IsNullOrEmpty(quantity.Text) && !string.IsNullOrEmpty(cost.Text)) // If true Quantity specified, Cost specified, get to work
            {
                decimal c = ExtensionActions.LoadTxtIntoDecimal(cost);          // Convert cost per item into decimal
                int q = ExtensionActions.LoadTxtIntoInt(quantity);              // Convert quantity into int
                if (c == PortalConstants.BadDecimalValue)                       // If == conversion routine encountered an error
                {
                    amount.Text = "Invalid value";                              // The product is therefore invalid
                    cost.Focus();                                               // Go back to the first control
                    return false;
                }
                else if (q == 0)                                                // If == conversion routine encountered an error
                {
                    amount.Text = "Invalid value";                              // The product is therefore invalid
                    quantity.Focus();                                           // Go back and fix it
                    return false;
                }                
                else
                {
                    decimal a = q * c;                                          // Calculate total amount
                    amount.Text = a.ToString("C2");                             // Convert product to text and display
                    cost.Text = c.ToString("C2");                               // While we're here, pretty up the amount
                    return true;                                                // Calculations successful. Set focus on amount field, if you like
                }
            }
            if (string.IsNullOrEmpty(quantity.Text))                            // If true quantity is blank
                quantity.Focus();                                               // Shift input focus to that spot
            else if (string.IsNullOrEmpty(cost.Text))                           // If true cost is blank
                cost.Focus();                                                   // Shift input focus to that spot
            amount.Text = "";                                                   // Blank out any prior content in amount
            return false;                                                       // Tell caller not to focus on amount field
        }

        // Hide unselected list items in the Payment Methods Radio Button List

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
            rdo.SelectedValue = val.ToString();             // Set that button in the Radio Button List
            return;
        }

        // Take a Yes/No/None enum and set the appropriate button in a Radio Button List and back to enum. This is a special case since neither of
        // the radio buttons may be set

        public static void LoadPOVendorModeIntoRdo(POVendorMode val, RadioButtonList rdo)
        {
            if (val == POVendorMode.None)                  // If == no radio button should be set
                rdo.SelectedIndex = -1;                     // Note no button set
            else
                LoadEnumIntoRdo(val, rdo);                  // Now we know a button will be set, so use the simpler case method
            return;
        }

        public static POVendorMode LoadRdoIntoPOVendorMode(RadioButtonList rdo)
        {
            string selected = rdo.SelectedValue;            // Find which if any radio button is pressed
            if (selected != "")                             // If != one of the buttons is selected
                return EnumActions.ConvertTextToPOVendorMode(selected); // Convert string to enum value
            return POVendorMode.None;
        }


        // Take a Yes/No/None enum and set the appropriate button in a Radio Button List and back to enum. This is a special case since neither of
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
            return PortalConstants.BadDecimalValue;         // Else, return marker that field is unfilled
        }

        public static string LoadDecimalIntoTxt(decimal bucks)
        {
            if (bucks != PortalConstants.BadDecimalValue)   // If != there is a "real" value in the field
                return bucks.ToString("C2",CultureInfo.CurrentCulture.NumberFormat); // Convert value to a local-format currency string (with 2 digits of precision)
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

        // A decimal-based textbox has been updated. Reformat the contents as currency.

        public static void ReloadDecimalText(TextBox txt)
        {
            decimal amount = ExtensionActions.LoadTxtIntoDecimal(txt); // Pull revised amount from textbox and convert
            txt.Text = ExtensionActions.LoadDecimalIntoTxt(amount); // Pretty up amount and put it back into textbox
            return;
        }

        public static void ReloadDecimalText_NoNegative(TextBox txt)
        {
            decimal amount = ExtensionActions.LoadTxtIntoDecimal(txt); // Pull revised amount from textbox and convert
            if (amount < 0) amount = PortalConstants.BadDecimalValue; // If < amount is negative. That's not allowed in this case
            txt.Text = ExtensionActions.LoadDecimalIntoTxt(amount); // Pretty up amount and put it back into textbox
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

        // Adjust the labels on Document's Contract fields and panel visibility based on whether or not we have a multiple. Shared by
        // EditDocument and ReviewDocument, so it's here.

        public static void SetDocumentContractLabels(RadioButtonList rdoContractFunds, RadioButtonList rdoContractFundsMultiple,
                                                    Label lblContractFundsMultiple, Label lblContractFundsAmount,
                                                    Label lblContractFundsNumber, Label lblContractTotal,
                                                    Panel pnlContractFundsNumber)
        {
            bool multiples = rdoContractFundsMultiple.SelectedValue == PortalConstants.CYes; // Single or Multiple?

            if (rdoContractFunds.SelectedValue == PortalConstants.RDOContractReceivingFunds) // If true we are processing receipts
            {
                lblContractFundsMultiple.Text = "Multiple Receipts?";
                if (multiples)
                {
                    lblContractFundsAmount.Text = "Recurring Receipt Amount"; // Change label on amount
                    lblContractFundsNumber.Text = "Number of Receipts";
                    lblContractTotal.Text = "Total Received";
                }
                else
                    lblContractFundsAmount.Text = "Receipt Amount";
            }
            else if (rdoContractFunds.SelectedValue == PortalConstants.RDOContractPayingFunds) // If true we are processing payments
            {
                lblContractFundsMultiple.Text = "Multiple Payments?";
                if (multiples)
                {
                    lblContractFundsAmount.Text = "Recurring Payment Amount"; // Change label on amount
                    lblContractFundsNumber.Text = "Number of Payments";
                    lblContractTotal.Text = "Total Liability";
                }
                else
                    lblContractFundsAmount.Text = "Payment Amount";
            }
            pnlContractFundsNumber.Visible = multiples;     // Turn on/off the panel with NumberOfAmounts and Total fields
            return;
        }
    }
}
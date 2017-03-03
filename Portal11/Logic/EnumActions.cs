using Portal11.ErrorLog;
using Portal11.Models;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Portal11.Logic
{
    public class EnumActions
    {

        // General support for enum data types

        // Given a text version of the enum value, carefully convert it to an enum value.

        public static AppReviewType ConvertTextToAppReviewType(string text)
        {
            try
            {
                return (AppReviewType)Enum.Parse(typeof(AppReviewType), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToAppReviewType", $"Unable to parse text '{text}' to AppReviewType enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static AppState ConvertTextToAppState(string text)
        {
            try
            {
                return (AppState)Enum.Parse(typeof(AppState), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToAppState", $"Unable to parse text '{text}' to AppState enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static AppType ConvertTextToAppType(string text)
        {
            try
            {
                return (AppType)Enum.Parse(typeof(AppType), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToAppType", $"Unable to parse text '{text}' to AppType enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static bool ConvertTextToBool(string text)
        {
            if (text == "True")
                return true;
            return false;
        }
        public static DeliveryMode ConvertTextToDeliveryMode(string text)
        {
            try
            {
                return (DeliveryMode)Enum.Parse(typeof(DeliveryMode), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToDeliveryMode", $"Unable to parse text '{text}' to DeliveryMode enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static DepState ConvertTextToDepState(string text)
        {
            try
            {
                return (DepState)Enum.Parse(typeof(DepState), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToDepState", $"Unable to parse text '{text}' to DepState enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static DepType ConvertTextToDepType(string text)
        {
            try
            {
                return (DepType)Enum.Parse(typeof(DepType), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToDepType", $"Unable to parse text '{text}' to DepType enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static EntityRole ConvertTextToEntityRole(string text)
        {
            try
            {
                return (EntityRole)Enum.Parse(typeof(EntityRole), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToEntityRole", $"Unable to parse text '{text}' to EntityRole enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static ExpState ConvertTextToExpState(string text)
        {
            try
            {
                return (ExpState)Enum.Parse(typeof(ExpState), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToExpState", $"Unable to parse text '{text}' to ExpState enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static ExpType ConvertTextToExpType(string text)
        {
            try
            {
                return (ExpType)Enum.Parse(typeof(ExpType), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToExpType", $"Unable to parse text '{text}' to ExpType enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static PaymentMethod ConvertTextToPaymentMethod(string text)
        {
            try
            {
                return (PaymentMethod)Enum.Parse(typeof(PaymentMethod), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToPaymentMethod", $"Unable to parse text '{text}' to PaymentMethod enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static PersonRole ConvertTextToPersonRole(string text)
        {
            try
            {
                return (PersonRole)Enum.Parse(typeof(PersonRole), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToPersonRole", $"Unable to parse text '{text}' to PersonRole enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static PODeliveryMode ConvertTextToPODeliveryMode(string text)
        {
            try
            {
                return (PODeliveryMode)Enum.Parse(typeof(PODeliveryMode), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToPODeliveryMode", $"Unable to parse text '{text}' to PODeliveryMode enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static ProjectRole ConvertTextToProjectRole(string text)
        {
            try
            {
                return (ProjectRole)Enum.Parse(typeof(ProjectRole), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToProjectRole", $"Unable to parse text '{text}' to ProjectRole enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static UserRole ConvertTextToUserRole(string text)
        {
            try
            {
                return (UserRole)Enum.Parse(typeof(UserRole), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToUserRole", $"Unable to parse text '{text}' to UserRole enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        public static YesNo ConvertTextToYesNo(string text)
        {
            try
            {
                return (YesNo)Enum.Parse(typeof(YesNo), text, true); // Convert back into enumeration type
            }
            catch (Exception)
            {
                LogError.LogInternalError("EnumActions.ConvertTextToYesNo", $"Unable to parse text '{text}' to YesNo enum"); // Fatal error
                return 0;                                               // If conversion failed, substitute null value
            }
        }

        // Given an enum datatype, look for the custom attribute "Description" to find a friendlier version of the enumerated value.

        public static string GetEnumDescription(Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            LogError.LogInternalError("EnumAction.GetEnumDescription", $"Unable to lookup Enum value '{value.ToString()}'"); // Fatal error
            return "Internal Error";
        }
    }
}
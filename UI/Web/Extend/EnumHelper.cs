using System;
using System.ComponentModel;

namespace Web.Extend
{
    public static class EnumHelper
    {
        public static string DisplayString(this Enum value)
        {
            //Using reflection to get the field info
            var info = value.GetType().GetField(value.ToString());

            //Get the Description Attributes
            var attributes = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);

            //Only capture the description attribute if it is a concrete result (i.e. 1 entry)
            if (attributes.Length == 1)
            {
                return attributes[0].Description;
            }
             //Use the value for display if not concrete result
            return value.ToString();
            
        }

        public static object EnumValueOf<T>(this string descriptionOrValue)
        {
            //Get all possible values of this enum type
            Array tValues = Enum.GetValues(typeof(T));

            //Cycle through all values searching for a match (description or value)
            foreach (Enum val in tValues)
            {
                var displayString = DisplayString(val);
                if (displayString.Equals(descriptionOrValue) 
                    || val.ToString().Equals(descriptionOrValue))
                {
                    return val;
                }
            }

            throw new ArgumentException(string.Format("The string value is not of type {0}.", typeof(T).ToString()));
        }

    }
}
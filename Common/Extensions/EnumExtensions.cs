namespace Common.Extensions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    public static class EnumExtensions
    {
        /// <summary>
        /// Obtains display name from <see cref="DisplayAttribute"/>
        /// on <see cref="Enum"/>.
        /// </summary>
        /// <param name="enumVal"></param>
        /// <returns><see cref="DisplayAttribute.Name"/> or enum basic string representation.</returns>
        public static string GetDisplayName(this Enum enumVal)
        {
            var enumType = enumVal.GetType();
            var enumFields = enumType.GetMember(enumVal.ToString());

            var displayAttribute = enumFields.FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>();

            if (displayAttribute == null)
            {
                return enumVal.ToString();
            }

            return displayAttribute.Name;
        }
    }
}
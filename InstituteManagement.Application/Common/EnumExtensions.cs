using System.ComponentModel.DataAnnotations;

namespace InstituteManagement.Application.Common
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                             .FirstOrDefault() as DisplayAttribute;
            return attr?.Name ?? value.ToString();
        }
    }
}

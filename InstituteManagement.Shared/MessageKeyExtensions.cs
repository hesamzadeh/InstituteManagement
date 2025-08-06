using System;
using System.Globalization;
using System.Resources;

namespace InstituteManagement.Shared
{
    public static class MessageKeyExtensions
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager("InstituteManagement.Shared.Resources.Messages", typeof(MessageKeyExtensions).Assembly);

        public static string Get(this Enum key, string language)
        {
            CultureInfo culture;
            try
            {
                culture = new CultureInfo(string.IsNullOrWhiteSpace(language) ? "en-US" : language);
            }
            catch
            {
                culture = new CultureInfo("en-US"); // fallback if invalid
            }
            // e.g., Signup.BirthdayMustBeInPast
            var keyTypeName = key.GetType().FullName;
            var sectionName = keyTypeName?.Split('+').Reverse().Skip(1).FirstOrDefault(); // "Signup"
            var enumName = key.ToString(); // "BirthdayMustBeInPast"
            var resourceKey = sectionName != null ? $"{sectionName}.{enumName}" : enumName;

            return _resourceManager.GetString(resourceKey, culture) ?? resourceKey;
        }
    }
}

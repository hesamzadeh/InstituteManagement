using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using Microsoft.AspNetCore.Components;

namespace InstituteManagement.Shared
{
    public class UiLocalizer
    {
        private readonly string _rootNamespace;
        private string? _defaultBaseName; // relative path or full base name

        public UiLocalizer()
        {
            // e.g. typeof(SharedResources).Namespace == "InstituteManagement.Shared"
            _rootNamespace = $"{typeof(SharedResources).Namespace}.Resources";
        }

        /// <summary>
        /// Explicitly set the resource base name, e.g. "Components.Pages.Home"
        /// or "InstituteManagement.Shared.Resources.Components.Pages.Home".
        /// </summary>
        public void SetBaseName(string baseName)
        {
            if (string.IsNullOrWhiteSpace(baseName))
                throw new ArgumentNullException(nameof(baseName));

            _defaultBaseName = baseName.Trim();
        }

        /// <summary>
        /// Primary lookup method (ResourceManager-based).
        /// baseName may be relative ("Components.Pages.Home") or full ("InstituteManagement.Shared.Resources.Components.Pages.Home").
        /// culture may be null to use CultureInfo.CurrentUICulture.
        /// </summary>
        public string Get(string baseName, string key, string? culture = null)
        {
            if (string.IsNullOrWhiteSpace(baseName))
                throw new ArgumentNullException(nameof(baseName));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var resourceBaseName = NormalizeBaseName(baseName);

            var rm = new ResourceManager(resourceBaseName, typeof(SharedResources).Assembly);

            var ci = string.IsNullOrWhiteSpace(culture)
                ? CultureInfo.CurrentUICulture
                : new CultureInfo(culture);

            return rm.GetString(key, ci) ?? key;
        }

        /// <summary>
        /// Indexer style access. Uses SetBaseName value if provided, otherwise auto-detects from call stack.
        /// Example: Ui["AppTitle"]
        /// </summary>
        public string this[string key]
        {
            get
            {
                var baseNameToUse = _defaultBaseName ?? DetectBaseName();
                return Get(baseNameToUse, key);
            }
        }

        /// <summary>
        /// Normalize base name: if already full (contains root namespace), return as-is;
        /// otherwise prepend the root namespace folder.
        /// </summary>
        private string NormalizeBaseName(string baseName)
        {
            var trimmed = baseName.Trim().Trim('.');
            // if baseName already contains the root namespace, assume it's full
            if (trimmed.StartsWith(_rootNamespace, StringComparison.OrdinalIgnoreCase))
                return trimmed;

            return $"{_rootNamespace}.{trimmed}";
        }

        /// <summary>
        /// Detects the resource base name from the calling Razor component's namespace.
        /// Example result: "InstituteManagement.Shared.Resources.Components.Layout.NavMenu"
        /// </summary>
        private string DetectBaseName()
        {
            var stack = new StackTrace();
            var frame = stack.GetFrames()
                ?.Select(f => f.GetMethod()?.DeclaringType)
                .FirstOrDefault(t => t != null && t.IsSubclassOf(typeof(ComponentBase)));

            if (frame == null)
                throw new InvalidOperationException("Could not detect calling component for localization.");

            // Example frame.FullName: "InstituteManagement.Front.Components.Layout.NavMenu"
            var nsParts = frame.FullName!.Split('.');
            var componentsIndex = Array.IndexOf(nsParts, "Components");
            if (componentsIndex == -1 || componentsIndex == nsParts.Length - 1)
                throw new InvalidOperationException("Component is not inside a Components.* namespace.");

            var relativePath = string.Join(".", nsParts.Skip(componentsIndex));
            return $"{_rootNamespace}.{relativePath}";
        }
    }
}

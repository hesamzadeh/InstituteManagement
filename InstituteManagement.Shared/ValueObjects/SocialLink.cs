using System.ComponentModel.DataAnnotations;

namespace InstituteManagement.Shared.ValueObjects
{
    public class SocialLink
    {
        [MaxLength(50)]
        public string Platform { get; set; } = default!; // e.g., LinkedIn

        [MaxLength(300)]
        public string Url { get; set; } = default!;
    }
}

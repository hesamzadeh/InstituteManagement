using System.ComponentModel.DataAnnotations;

namespace InstituteManagement.Core.Common.ValueObjects
{
    public class Address
    {
        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(100)]
        public string? Province { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        [MaxLength(500)]
        public string FullAddress { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}

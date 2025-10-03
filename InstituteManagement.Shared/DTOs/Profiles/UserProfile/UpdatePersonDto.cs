using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstituteManagement.Shared.DTOs.Profiles.UserProfile
{
    public class UpdatePersonDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? Birthday { get; set; }
        public string? Country { get; set; }
        public string? Province { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? FullAddress { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}

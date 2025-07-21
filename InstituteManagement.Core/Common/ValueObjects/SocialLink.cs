using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstituteManagement.Core.Common.ValueObjects
{
    public class SocialLink
    {
        [MaxLength(50)]
        public string Platform { get; set; } = default!; // e.g., LinkedIn

        [MaxLength(300)]
        public string Url { get; set; } = default!;
    }
}

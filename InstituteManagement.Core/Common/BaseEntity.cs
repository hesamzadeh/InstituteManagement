using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstituteManagement.Core.Common
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime InsertedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModificationDate { get; set; } = DateTime.UtcNow;

        [Timestamp]
        public byte[]? Version { get; set; }
    }
}

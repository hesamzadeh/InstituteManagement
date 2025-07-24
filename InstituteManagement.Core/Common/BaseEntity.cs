using System.ComponentModel.DataAnnotations;

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

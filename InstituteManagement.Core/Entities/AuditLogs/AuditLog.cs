
namespace InstituteManagement.Core.Entities.AuditLogs
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }          // links to BaseEntity.Id
        public string PropertyName { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
        public string ChangedBy { get; set; } = string.Empty;    // user performing change
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }

}

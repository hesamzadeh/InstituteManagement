using InstituteManagement.Shared.Enums;

namespace InstituteManagement.Core.Entities.People
{
    public class PersonDocument : BaseEntity
    {
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = default!;

        /// <summary>
        /// Original filename uploaded by user.
        /// </summary>
        public string FileName { get; set; } = default!;

        /// <summary>
        /// Path to stored file (relative or absolute depending on your storage strategy).
        /// </summary>
        public string FilePath { get; set; } = default!;

        /// <summary>
        /// Type of document (e.g. "Passport", "NationalIdCard").
        /// </summary>
        public DocumentType FileType { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}

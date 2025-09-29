using InstituteManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstituteManagement.Infrastructure.Data.Configurations
{
    public class PersonDocumentConfiguration : IEntityTypeConfiguration<PersonDocument>
    {
        public void Configure(EntityTypeBuilder<PersonDocument> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.FileName)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(d => d.FilePath)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(d => d.FileType)
                   .HasConversion<string>()   // store enum as string (Passport, NationalIdCard, etc.)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(d => d.UploadedAt)
                   .HasDefaultValueSql("getutcdate()");
        }
    }
}

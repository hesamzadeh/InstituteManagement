using InstituteManagement.Application.Common.Interfaces;
using InstituteManagement.Core.Entities;
using InstituteManagement.Core.Entities.AuditLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace InstituteManagement.Infrastructure.Persistence.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        public AuditInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null)
                return base.SavingChangesAsync(eventData, result, cancellationToken);

            var currentUser = _currentUserService.UserId ?? "anonymous";
            var auditLogs = new List<AuditLog>();

            foreach (var entry in context.ChangeTracker.Entries())
            {
                // Only audit BaseEntity, AppUser, or owned types
                if (entry.Entity is BaseEntity || entry.Entity is AppUser || entry.Metadata.IsOwned())
                {
                    // Modified
                    if (entry.State == EntityState.Modified)
                    {
                        foreach (var prop in entry.Properties)
                        {
                            if (!prop.IsModified) continue;

                            var original = prop.OriginalValue?.ToString() ?? "";
                            var current = prop.CurrentValue?.ToString() ?? "";
                            if (original == current) continue;

                            // Skip noise
                            if (prop.Metadata.Name is nameof(BaseEntity.LastModificationDate) or nameof(BaseEntity.Version)
                                or "ConcurrencyStamp" or "SecurityStamp")
                                continue;

                            auditLogs.Add(new AuditLog
                            {
                                EntityName = GetEntityName(entry),
                                EntityId = GetEntityId(entry),
                                PropertyName = prop.Metadata.Name,
                                OldValue = original,
                                NewValue = current,
                                ChangedBy = currentUser,
                                ChangedAt = DateTime.UtcNow
                            });
                        }
                    }

                    // Added
                    if (entry.State == EntityState.Added)
                    {
                        auditLogs.Add(new AuditLog
                        {
                            EntityName = GetEntityName(entry),
                            EntityId = GetEntityId(entry),
                            PropertyName = "*",
                            OldValue = "",
                            NewValue = "Created",
                            ChangedBy = currentUser,
                            ChangedAt = DateTime.UtcNow
                        });
                    }

                    // Deleted
                    if (entry.State == EntityState.Deleted)
                    {
                        auditLogs.Add(new AuditLog
                        {
                            EntityName = GetEntityName(entry),
                            EntityId = GetEntityId(entry),
                            PropertyName = "*",
                            OldValue = "Exists",
                            NewValue = "Deleted",
                            ChangedBy = currentUser,
                            ChangedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            if (auditLogs.Any())
            {
                context.Set<AuditLog>().AddRange(auditLogs);
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static string GetEntityName(EntityEntry entry)
        {
            if (entry.Metadata.IsOwned())
            {
                var ownerNav = entry.Metadata.FindOwnership();
                if (ownerNav != null)
                {
                    return $"{ownerNav.PrincipalEntityType.ClrType.Name}.{entry.Entity.GetType().Name}";
                }
            }

            return entry.Entity.GetType().Name;
        }

        private static Guid GetEntityId(EntityEntry entry)
        {
            if (entry.Entity is BaseEntity be)
                return be.Id;

            if (entry.Entity is AppUser au)
                return au.Id;

            if (entry.Metadata.IsOwned())
            {
                var ownership = entry.Metadata.FindOwnership();
                var ownerEntry = entry.Context.ChangeTracker.Entries()
                    .FirstOrDefault(e => e.Metadata == ownership?.PrincipalEntityType);

                if (ownerEntry?.Entity is BaseEntity ownerBase)
                    return ownerBase.Id;
                if (ownerEntry?.Entity is AppUser ownerUser)
                    return ownerUser.Id;
            }

            return Guid.Empty;
        }

    }
}

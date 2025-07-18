using System.Collections.Generic;

namespace InstituteManagement.Application.Common.Interfaces;

public interface IAppDbContext
{
    //DbSet<Student> Students { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

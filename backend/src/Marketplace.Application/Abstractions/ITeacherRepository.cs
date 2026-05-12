using Marketplace.Domain.Entities;
using Marketplace.Domain.Enums;

namespace Marketplace.Application.Abstractions;

public interface ITeacherRepository
{
    Task<IReadOnlyList<Teacher>> GetTeachersAsync(TeacherLevel? level, CancellationToken cancellationToken);
    
    Task<Teacher> CreateTeacherAsync(Teacher teacher, CancellationToken cancellationToken);
    Task<Teacher?> GetByIdAsync(Guid teacherId, CancellationToken cancellationToken);
    Task<Teacher> UpdateTeacherAsync(Teacher teacher, CancellationToken cancellationToken);
    Task DeleteAsync(Guid teacherId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid teacherId, CancellationToken cancellationToken);
}

using Marketplace.Application.Abstractions;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Infrastructure.Persistence;

public sealed class PostgresTeacherRepository : ITeacherRepository
{
    private readonly MarketplaceDbContext _dbContext;

    public PostgresTeacherRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Teacher>> GetTeachersAsync(TeacherLevel? level, CancellationToken cancellationToken)
    {
        var query = _dbContext.Teachers.AsNoTracking();

        if (level.HasValue)
        {
            query = query.Where(t => t.Level == level.Value);
        }

        return await query
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<Teacher> CreateTeacherAsync(Teacher teacher, CancellationToken cancellationToken)
    {
        _dbContext.Teachers.Add(teacher);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return teacher;
    }

    public Task<Teacher?> GetByIdAsync(Guid teacherId, CancellationToken cancellationToken)
    {
        return _dbContext.Teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == teacherId, cancellationToken);
    }

    public async Task<Teacher> UpdateTeacherAsync(Teacher teacher, CancellationToken cancellationToken)
    {
        _dbContext.Teachers.Update(teacher);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return teacher;
    }

    public async Task DeleteAsync(Guid teacherId, CancellationToken cancellationToken)
    {
        var teacher = await _dbContext.Teachers
            .FirstOrDefaultAsync(t => t.Id == teacherId, cancellationToken);

        if (teacher is null)
        {
            return;
        }

        _dbContext.Teachers.Remove(teacher);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid teacherId, CancellationToken cancellationToken)
    {
        return _dbContext.Teachers
            .AsNoTracking()
            .AnyAsync(t => t.Id == teacherId, cancellationToken);
    }
}

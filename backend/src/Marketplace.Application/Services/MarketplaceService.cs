using Marketplace.Application.Abstractions;
using Marketplace.Application.DTOs;
using Marketplace.Application.Exceptions;
using Marketplace.Domain.Enums;
using Marketplace.Domain.Entities;

namespace Marketplace.Application.Services;

public sealed class MarketplaceService
{
    private const int MaxPageSize = 100;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IFavoritesRepository _favoritesRepository;
    private readonly ICurrencyConverter _currencyConverter;

    public MarketplaceService(
        ITeacherRepository teacherRepository,
        IFavoritesRepository favoritesRepository,
        ICurrencyConverter currencyConverter)
    {
        _teacherRepository = teacherRepository;
        _favoritesRepository = favoritesRepository;
        _currencyConverter = currencyConverter;
    }

    public async Task<PagedResult<TeacherCardDto>> GetTeachersAsync(
        string currency,
        string sessionId,
        TeacherLevel? level,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ValidationAppException("currency is required.");
        }

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ValidationAppException("sessionId is required.");
        }

        if (page <= 0)
        {
            throw new ValidationAppException("page must be greater than 0.");
        }

        if (pageSize <= 0 || pageSize > MaxPageSize)
        {
            throw new ValidationAppException($"pageSize must be between 1 and {MaxPageSize}.");
        }
        
        var normalizedCurrency = currency.Trim().ToUpperInvariant();
        var rate = await _currencyConverter.GetUsdToCurrencyRateAsync(normalizedCurrency, cancellationToken);
        var teachers = await _teacherRepository.GetTeachersAsync(level,cancellationToken);
        var favoriteIds = await _favoritesRepository.GetFavoriteTeacherIdsAsync(sessionId.Trim(), cancellationToken);

        var totalCount = teachers.Count;
        var pageItems = teachers
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TeacherCardDto(
                t.Id,
                t.Name,
                t.Level,
                t.BasePriceUsd,
                Math.Round(t.BasePriceUsd * rate, 2, MidpointRounding.AwayFromZero),
                normalizedCurrency,
                favoriteIds.Contains(t.Id)))
            .ToList();

        return new PagedResult<TeacherCardDto>(pageItems, page, pageSize, totalCount);
    }

    public async Task<TeacherDto> GetTeacherByIdAsync(Guid teacherId, CancellationToken cancellationToken)
    {
        if (teacherId == Guid.Empty)
        {
            throw new ValidationAppException("teacherId is required.");
        }

        var teacher = await _teacherRepository.GetByIdAsync(teacherId, cancellationToken);
        if (teacher is null)
        {
            throw new NotFoundAppException("Teacher not found.");
        }

        return new TeacherDto(teacher.Id, teacher.Name, teacher.Level, teacher.BasePriceUsd);
    }

    public async Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto dto, CancellationToken cancellationToken)
    {
        EnsureTeacherPayloadIsValid(dto.Name, dto.BasePriceUsd);

        var teacher = new Teacher
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Level = dto.Level,
            BasePriceUsd = dto.BasePriceUsd,
            CreatedAtUtc = DateTime.UtcNow
        };

        var created = await _teacherRepository.CreateTeacherAsync(teacher, cancellationToken);
        return new TeacherDto(created.Id, created.Name, created.Level, created.BasePriceUsd);
    }

    public async Task<TeacherDto> UpdateTeacherAsync(Guid teacherId, UpdateTeacherDto dto, CancellationToken cancellationToken)
    {
        if (teacherId == Guid.Empty)
        {
            throw new ValidationAppException("teacherId is required.");
        }

        EnsureTeacherPayloadIsValid(dto.Name, dto.BasePriceUsd);

        var existing = await _teacherRepository.GetByIdAsync(teacherId, cancellationToken);
        if (existing is null)
        {
            throw new NotFoundAppException("Teacher not found.");
        }

        var updated = new Teacher
        {
            Id = existing.Id,
            Name = dto.Name.Trim(),
            Level = dto.Level,
            BasePriceUsd = dto.BasePriceUsd
        };

        var saved = await _teacherRepository.UpdateTeacherAsync(updated, cancellationToken);
        return new TeacherDto(saved.Id, saved.Name, saved.Level, saved.BasePriceUsd);
    }

    public async Task DeleteTeacherAsync(Guid teacherId, CancellationToken cancellationToken)
    {
        if (teacherId == Guid.Empty)
        {
            throw new ValidationAppException("teacherId is required.");
        }

        var exists = await _teacherRepository.ExistsAsync(teacherId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundAppException("Teacher not found.");
        }

        await _teacherRepository.DeleteAsync(teacherId, cancellationToken);
    }

    public async Task<IReadOnlySet<Guid>> GetFavoritesAsync(string sessionId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ValidationAppException("sessionId is required.");
        }

        return await _favoritesRepository.GetFavoriteTeacherIdsAsync(sessionId.Trim(), cancellationToken);
    }

    public async Task AddFavoriteAsync(string sessionId, Guid teacherId, CancellationToken cancellationToken)
    {
        await EnsureFavoriteRequestIsValid(sessionId, teacherId, cancellationToken);
        await _favoritesRepository.AddFavoriteAsync(sessionId.Trim(), teacherId, cancellationToken);
    }

    public async Task RemoveFavoriteAsync(string sessionId, Guid teacherId, CancellationToken cancellationToken)
    {
        await EnsureFavoriteRequestIsValid(sessionId, teacherId, cancellationToken);
        await _favoritesRepository.RemoveFavoriteAsync(sessionId.Trim(), teacherId, cancellationToken);
    }

    private async Task EnsureFavoriteRequestIsValid(string sessionId, Guid teacherId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ValidationAppException("sessionId is required.");
        }

        if (teacherId == Guid.Empty)
        {
            throw new ValidationAppException("teacherId is required.");
        }

        var exists = await _teacherRepository.ExistsAsync(teacherId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundAppException("Teacher not found.");
        }
    }

    private static void EnsureTeacherPayloadIsValid(string name, decimal basePriceUsd)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationAppException("name is required.");
        }

        if (name.Trim().Length > 120)
        {
            throw new ValidationAppException("name must be 120 characters or fewer.");
        }

        if (basePriceUsd <= 0)
        {
            throw new ValidationAppException("basePriceUsd must be greater than 0.");
        }
    }
}

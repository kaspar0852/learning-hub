using Marketplace.Domain.Enums;

namespace Marketplace.Application.DTOs;

public sealed record TeacherCardDto(
    Guid Id,
    string Name,
    TeacherLevel Level,
    decimal BasePriceUsd,
    decimal LocalizedPrice,
    string Currency,
    bool IsFavorite);

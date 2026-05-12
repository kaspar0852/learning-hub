using Marketplace.Domain.Enums;

namespace Marketplace.Application.DTOs;

public sealed record TeacherDto(
    Guid Id,
    string Name,
    TeacherLevel Level,
    decimal BasePriceUsd);


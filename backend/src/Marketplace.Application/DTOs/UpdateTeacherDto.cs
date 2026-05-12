using Marketplace.Domain.Enums;

namespace Marketplace.Application.DTOs;

public sealed record UpdateTeacherDto(
    string Name,
    TeacherLevel Level,
    decimal BasePriceUsd);


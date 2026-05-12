using Marketplace.Domain.Enums;

namespace Marketplace.Application.DTOs;

public sealed record CreateTeacherDto( string Name,
    TeacherLevel Level,
    decimal BasePriceUsd);
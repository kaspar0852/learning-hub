using Marketplace.Domain.Enums;

namespace Marketplace.Domain.Entities;

public sealed class Teacher
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TeacherLevel Level { get; set; }
    public decimal BasePriceUsd { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

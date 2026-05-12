using Marketplace.Application.Services;
using Marketplace.Application.DTOs;
using Marketplace.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TeachersController : ControllerBase
{
    private readonly MarketplaceService _marketplaceService;

    public TeachersController(MarketplaceService marketplaceService)
    {
        _marketplaceService = marketplaceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTeachers(
        [FromQuery] string currency,
        [FromQuery] string sessionId,
        [FromQuery] TeacherLevel? level,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _marketplaceService.GetTeachersAsync(
            currency,
            sessionId,
            level,
            page,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{teacherId:guid}")]
    public async Task<IActionResult> GetTeacherById([FromRoute] Guid teacherId, CancellationToken cancellationToken = default)
    {
        var teacher = await _marketplaceService.GetTeacherByIdAsync(teacherId, cancellationToken);
        return Ok(teacher);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _marketplaceService.CreateTeacherAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetTeacherById), new { teacherId = created.Id }, created);
    }

    [HttpPut("{teacherId:guid}")]
    public async Task<IActionResult> UpdateTeacher(
        [FromRoute] Guid teacherId,
        [FromBody] UpdateTeacherDto dto,
        CancellationToken cancellationToken = default)
    {
        var updated = await _marketplaceService.UpdateTeacherAsync(teacherId, dto, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{teacherId:guid}")]
    public async Task<IActionResult> DeleteTeacher([FromRoute] Guid teacherId, CancellationToken cancellationToken = default)
    {
        await _marketplaceService.DeleteTeacherAsync(teacherId, cancellationToken);
        return NoContent();
    }
}

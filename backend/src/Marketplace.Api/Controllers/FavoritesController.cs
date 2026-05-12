using Marketplace.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers;

[ApiController]
[Route("api/sessions/{sessionId}/favorites")]
public sealed class FavoritesController : ControllerBase
{
    private readonly MarketplaceService _marketplaceService;

    public FavoritesController(MarketplaceService marketplaceService)
    {
        _marketplaceService = marketplaceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFavorites(
        [FromRoute] string sessionId,
        CancellationToken cancellationToken = default)
    {
        var result = await _marketplaceService.GetFavoritesAsync(sessionId, cancellationToken);
        return Ok(new { sessionId, teacherIds = result });
    }

    [HttpPost("{teacherId:guid}")]
    public async Task<IActionResult> AddFavorite(
        [FromRoute] string sessionId,
        [FromRoute] Guid teacherId,
        CancellationToken cancellationToken = default)
    {
        await _marketplaceService.AddFavoriteAsync(sessionId, teacherId, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{teacherId:guid}")]
    public async Task<IActionResult> RemoveFavorite(
        [FromRoute] string sessionId,
        [FromRoute] Guid teacherId,
        CancellationToken cancellationToken = default)
    {
        await _marketplaceService.RemoveFavoriteAsync(sessionId, teacherId, cancellationToken);
        return NoContent();
    }
}

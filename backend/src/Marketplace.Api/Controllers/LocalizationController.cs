using Marketplace.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class LocalizationController : ControllerBase
{
    private readonly LocalizationService _localizationService;

    public LocalizationController(LocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    [HttpGet("currency")]
    public async Task<IActionResult> GetCurrency(CancellationToken cancellationToken = default)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _localizationService.DetectCurrencyAsync(ipAddress, cancellationToken);
        return Ok(result);
    }
}

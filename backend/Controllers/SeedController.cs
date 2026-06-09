using Microsoft.AspNetCore.Mvc;
using MongoReviews.Api.Models.Dtos;
using MongoReviews.Api.Services;

namespace MongoReviews.Api.Controllers;

[ApiController]
[Route("api/seed")]
public class SeedController : ControllerBase
{
    private readonly SeedService _seedService;

    public SeedController(SeedService seedService)
    {
        _seedService = seedService;
    }

    [HttpPost]
    public async Task<IActionResult> Seed([FromQuery] bool reset = false)
    {
        if (!Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Contains("Development") is true)
            return Forbid();

        await _seedService.Seed(reset);
        var msg = reset ? "Datos reiniciados y creados" : "Datos de prueba insertados";
        return Ok(ApiResponse<object>.Ok(null!, msg));
    }
}

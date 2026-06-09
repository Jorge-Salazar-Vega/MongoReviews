using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoReviews.Api.Models.Dtos;
using MongoReviews.Api.Services;

namespace MongoReviews.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly ReviewService _reviewService;

    public UsuariosController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("perfil")]
    [Authorize]
    public IActionResult GetPerfil()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var email = User.FindFirstValue(ClaimTypes.Email)!;
        var nombre = User.Identity?.Name ?? "Usuario";

        return Ok(ApiResponse<object>.Ok(new
        {
            id = userId,
            email,
            nombre
        }));
    }

    [HttpGet("{id}/resenas")]
    public async Task<IActionResult> GetResenasByUser(string id)
    {
        var resenas = await _reviewService.GetByUser(id);
        return Ok(ApiResponse<List<ReviewResponse>>.Ok(resenas));
    }
}

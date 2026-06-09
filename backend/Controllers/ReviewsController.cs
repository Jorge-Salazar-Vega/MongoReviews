using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoReviews.Api.Models.Dtos;
using MongoReviews.Api.Services;

namespace MongoReviews.Api.Controllers;

[ApiController]
[Route("api/resenas")]
public class ReviewsController : ControllerBase
{
    private readonly ReviewService _reviewService;

    public ReviewsController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("serie/{idSerie}")]
    public async Task<IActionResult> GetBySerie(string idSerie, [FromQuery] int pagina = 1, [FromQuery] int limite = 10)
    {
        var result = await _reviewService.GetBySerie(idSerie, pagina, limite);
        return Ok(ApiResponse<PagedResult<ReviewResponse>>.Ok(result));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            var review = await _reviewService.Create(request, userId);
            return CreatedAtAction(nameof(Create), ApiResponse<ReviewResponse>.Ok(review, "Reseña creada exitosamente"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ReviewResponse>.Error(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ReviewResponse>.Error(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(string id, [FromBody] CreateReviewRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            var review = await _reviewService.Update(id, request, userId);
            return Ok(ApiResponse<ReviewResponse>.Ok(review, "Reseña actualizada exitosamente"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ReviewResponse>.Error(ex.Message));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            await _reviewService.Delete(id, userId);
            return Ok(ApiResponse<object>.Ok(null!, "Reseña eliminada exitosamente"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Error(ex.Message));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}

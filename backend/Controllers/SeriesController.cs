using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoReviews.Api.Models.Dtos;
using MongoReviews.Api.Services;

namespace MongoReviews.Api.Controllers;

[ApiController]
[Route("api/series")]
public class SeriesController : ControllerBase
{
    private readonly SeriesService _seriesService;

    public SeriesController(SeriesService seriesService)
    {
        _seriesService = seriesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pagina = 1,
        [FromQuery] int limite = 10,
        [FromQuery] string? genero = null,
        [FromQuery] string ordenarPor = "titulo",
        [FromQuery] string orden = "asc")
    {
        var result = await _seriesService.GetAll(pagina, limite, genero, ordenarPor, orden);
        return Ok(ApiResponse<PagedResult<SeriesResponse>>.Ok(result));
    }

    [HttpGet("buscar")]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] int pagina = 1,
        [FromQuery] int limite = 10)
    {
        var result = await _seriesService.Search(q, pagina, limite);
        return Ok(ApiResponse<PagedResult<SeriesResponse>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var series = await _seriesService.GetById(id);
        if (series is null)
            return NotFound(ApiResponse<SeriesResponse>.Error("Serie no encontrada"));

        return Ok(ApiResponse<SeriesResponse>.Ok(new SeriesResponse
        {
            Id = series.Id,
            Titulo = series.Titulo,
            Descripcion = series.Descripcion,
            Generos = series.Generos,
            AnioEstreno = series.AnioEstreno,
            Poster = series.Poster,
            Temporadas = series.Temporadas,
            RatingPromedio = series.RatingPromedio,
            TotalResenas = series.TotalResenas
        }));
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreateSeriesRequest request)
    {
        var series = await _seriesService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = series.Id },
            ApiResponse<SeriesResponse>.Ok(new SeriesResponse
            {
                Id = series.Id,
                Titulo = series.Titulo,
                Descripcion = series.Descripcion,
                Generos = series.Generos,
                AnioEstreno = series.AnioEstreno,
                Poster = series.Poster,
                Temporadas = series.Temporadas,
                RatingPromedio = series.RatingPromedio,
                TotalResenas = series.TotalResenas
            }, "Serie creada exitosamente"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateSeriesRequest request)
    {
        var updated = await _seriesService.Update(id, request);
        if (!updated)
            return NotFound(ApiResponse<SeriesResponse>.Error("Serie no encontrada"));

        return Ok(ApiResponse<SeriesResponse>.Ok(null!, "Serie actualizada exitosamente"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _seriesService.Delete(id);
        if (!deleted)
            return NotFound(ApiResponse<SeriesResponse>.Error("Serie no encontrada"));

        return Ok(ApiResponse<SeriesResponse>.Ok(null!, "Serie eliminada exitosamente"));
    }
}

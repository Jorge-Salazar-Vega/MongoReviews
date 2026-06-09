using System.ComponentModel.DataAnnotations;

namespace MongoReviews.Api.Models.Dtos;

public class CreateSeriesRequest
{
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200)]
    public string Titulo { get; set; } = null!;

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(2000)]
    public string Descripcion { get; set; } = null!;

    [Required(ErrorMessage = "Debe indicar al menos un género")]
    public List<string> Generos { get; set; } = [];

    [Range(1900, 2030)]
    public int AnioEstreno { get; set; }

    public string? Poster { get; set; }

    [Range(1, 100)]
    public int Temporadas { get; set; }
}

public class SeriesResponse
{
    public string Id { get; set; } = null!;
    public string Titulo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public List<string> Generos { get; set; } = [];
    public int AnioEstreno { get; set; }
    public string? Poster { get; set; }
    public int Temporadas { get; set; }
    public double RatingPromedio { get; set; }
    public int TotalResenas { get; set; }
}

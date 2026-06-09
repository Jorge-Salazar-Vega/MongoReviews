using System.ComponentModel.DataAnnotations;

namespace MongoReviews.Api.Models.Dtos;

public class CreateReviewRequest
{
    [Required(ErrorMessage = "El ID de la serie es obligatorio")]
    public string IdSerie { get; set; } = null!;

    [Required(ErrorMessage = "La puntuación es obligatoria")]
    [Range(1, 10, ErrorMessage = "La puntuación debe estar entre 1 y 10")]
    public int Puntuacion { get; set; }

    [StringLength(2000)]
    public string? Comentario { get; set; }
}

public class ReviewResponse
{
    public string Id { get; set; } = null!;
    public string IdUsuario { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public string? AvatarUsuario { get; set; }
    public string IdSerie { get; set; } = null!;
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
    public DateTime CreatedAt { get; set; }
}

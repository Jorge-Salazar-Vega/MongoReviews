using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string IdUsuario { get; set; } = null!;

    public string NombreUsuario { get; set; } = null!;
    public string? AvatarUsuario { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string IdSerie { get; set; } = null!;

    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
    public DateTime CreatedAt { get; set; }
}

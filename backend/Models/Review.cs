using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoReviews.Api.Models;

public class Review
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string IdUsuario { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string IdSerie { get; set; } = null!;

    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

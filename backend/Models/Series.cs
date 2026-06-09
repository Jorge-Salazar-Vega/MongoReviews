using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoReviews.Api.Models;

public class Series
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Titulo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public List<string> Generos { get; set; } = [];
    public int AnioEstreno { get; set; }
    public string? Poster { get; set; }
    public int Temporadas { get; set; }
    public double RatingPromedio { get; set; }
    public int TotalResenas { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

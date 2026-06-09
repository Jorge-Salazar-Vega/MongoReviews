using MongoDB.Bson;
using MongoDB.Driver;
using MongoReviews.Api.Models;
using MongoReviews.Api.Models.Dtos;

namespace MongoReviews.Api.Services;

public class ReviewService
{
    private readonly MongoDbContext _db;

    public ReviewService(MongoDbContext db)
    {
        _db = db;
    }

    public async Task<ReviewResponse> Create(CreateReviewRequest request, string userId)
    {
        var series = await _db.Series.Find(s => s.Id == request.IdSerie).FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException("La serie no existe");

        var existing = await _db.Reviews
            .Find(r => r.IdUsuario == userId && r.IdSerie == request.IdSerie)
            .FirstOrDefaultAsync();

        if (existing is not null)
            throw new InvalidOperationException("Ya has reseñado esta serie");

        var review = new Review
        {
            IdUsuario = userId,
            IdSerie = request.IdSerie,
            Puntuacion = request.Puntuacion,
            Comentario = request.Comentario
        };

        await _db.Reviews.InsertOneAsync(review);
        await RecalcularRating(request.IdSerie);

        return await GetReviewWithUser(review.Id);
    }

    public async Task<ReviewResponse> Update(string id, CreateReviewRequest request, string userId)
    {
        var review = await _db.Reviews.Find(r => r.Id == id).FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException("La reseña no existe");

        if (review.IdUsuario != userId)
            throw new UnauthorizedAccessException("No puedes modificar una reseña de otro usuario");

        await _db.Reviews.UpdateOneAsync(
            r => r.Id == id,
            Builders<Review>.Update
                .Set(r => r.Puntuacion, request.Puntuacion)
                .Set(r => r.Comentario, request.Comentario)
                .Set(r => r.UpdatedAt, DateTime.UtcNow)
        );

        await RecalcularRating(request.IdSerie);

        return await GetReviewWithUser(id);
    }

    public async Task<bool> Delete(string id, string userId)
    {
        var review = await _db.Reviews.Find(r => r.Id == id).FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException("La reseña no existe");

        if (review.IdUsuario != userId)
            throw new UnauthorizedAccessException("No puedes eliminar una reseña de otro usuario");

        var idSerie = review.IdSerie;
        var result = await _db.Reviews.DeleteOneAsync(r => r.Id == id);

        if (result.DeletedCount > 0)
            await RecalcularRating(idSerie);

        return result.DeletedCount > 0;
    }

    public async Task<PagedResult<ReviewResponse>> GetBySerie(string idSerie, int pagina, int limite)
    {
        var filter = Builders<Review>.Filter.Eq(r => r.IdSerie, idSerie);
        var total = (int)await _db.Reviews.CountDocumentsAsync(filter);
        var totalPaginas = (int)Math.Ceiling(total / (double)limite);

        var pipeline = new BsonDocument[]
        {
            new("$match", new BsonDocument("idSerie", new ObjectId(idSerie))),
            new("$sort", new BsonDocument("createdAt", -1)),
            new("$skip", (pagina - 1) * limite),
            new("$limit", limite),
            new("$lookup", new BsonDocument
            {
                { "from", "users" },
                { "localField", "idUsuario" },
                { "foreignField", "_id" },
                { "as", "usuario" }
            }),
            new("$unwind", "$usuario"),
            new("$project", new BsonDocument
            {
                { "_id", 1 },
                { "idUsuario", 1 },
                { "idSerie", 1 },
                { "puntuacion", 1 },
                { "comentario", 1 },
                { "createdAt", 1 },
                { "nombreUsuario", "$usuario.nombre" },
                { "avatarUsuario", "$usuario.avatar" }
            })
        };

        var reviews = await _db.Reviews
            .Aggregate<ReviewResponse>(pipeline)
            .ToListAsync();

        return new PagedResult<ReviewResponse>
        {
            Datos = reviews,
            Total = total,
            Pagina = pagina,
            TotalPaginas = totalPaginas
        };
    }

    public async Task<List<ReviewResponse>> GetByUser(string userId)
    {
        var pipeline = new BsonDocument[]
        {
            new("$match", new BsonDocument("idUsuario", new ObjectId(userId))),
            new("$sort", new BsonDocument("createdAt", -1)),
            new("$lookup", new BsonDocument
            {
                { "from", "users" },
                { "localField", "idUsuario" },
                { "foreignField", "_id" },
                { "as", "usuario" }
            }),
            new("$unwind", "$usuario"),
            new("$project", new BsonDocument
            {
                { "_id", 1 },
                { "idUsuario", 1 },
                { "idSerie", 1 },
                { "puntuacion", 1 },
                { "comentario", 1 },
                { "createdAt", 1 },
                { "nombreUsuario", "$usuario.nombre" },
                { "avatarUsuario", "$usuario.avatar" }
            })
        };

        return await _db.Reviews
            .Aggregate<ReviewResponse>(pipeline)
            .ToListAsync();
    }

    private async Task<ReviewResponse> GetReviewWithUser(string reviewId)
    {
        var pipeline = new BsonDocument[]
        {
            new("$match", new BsonDocument("_id", new ObjectId(reviewId))),
            new("$lookup", new BsonDocument
            {
                { "from", "users" },
                { "localField", "idUsuario" },
                { "foreignField", "_id" },
                { "as", "usuario" }
            }),
            new("$unwind", "$usuario"),
            new("$project", new BsonDocument
            {
                { "_id", 1 },
                { "idUsuario", 1 },
                { "idSerie", 1 },
                { "puntuacion", 1 },
                { "comentario", 1 },
                { "createdAt", 1 },
                { "nombreUsuario", "$usuario.nombre" },
                { "avatarUsuario", "$usuario.avatar" }
            })
        };

        return (await _db.Reviews
            .Aggregate<ReviewResponse>(pipeline)
            .FirstOrDefaultAsync())!;
    }

    private async Task RecalcularRating(string idSerie)
    {
        var pipeline = new BsonDocument[]
        {
            new("$match", new BsonDocument("idSerie", new ObjectId(idSerie))),
            new("$group", new BsonDocument
            {
                { "_id", null },
                { "promedio", new BsonDocument("$avg", "$puntuacion") },
                { "total", new BsonDocument("$sum", 1) }
            })
        };

        var result = await _db.Reviews
            .Aggregate<BsonDocument>(pipeline)
            .FirstOrDefaultAsync();

        var promedio = result is not null
            ? Math.Round(result["promedio"].AsDouble, 1)
            : 0.0;

        var total = result is not null
            ? result["total"].AsInt32
            : 0;

        await _db.Series.UpdateOneAsync(
            s => s.Id == idSerie,
            Builders<Series>.Update
                .Set(s => s.RatingPromedio, promedio)
                .Set(s => s.TotalResenas, total)
        );
    }
}

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

        return await ToResponse(review);
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
        review.Puntuacion = request.Puntuacion;
        review.Comentario = request.Comentario;

        return await ToResponse(review);
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

        var reviews = await _db.Reviews
            .Find(filter)
            .SortByDescending(r => r.CreatedAt)
            .Skip((pagina - 1) * limite)
            .Limit(limite)
            .ToListAsync();

        var response = new List<ReviewResponse>();
        foreach (var r in reviews)
            response.Add(await ToResponse(r));

        return new PagedResult<ReviewResponse>
        {
            Datos = response,
            Total = total,
            Pagina = pagina,
            TotalPaginas = totalPaginas
        };
    }

    public async Task<List<ReviewResponse>> GetByUser(string userId)
    {
        var reviews = await _db.Reviews
            .Find(r => r.IdUsuario == userId)
            .SortByDescending(r => r.CreatedAt)
            .ToListAsync();

        var response = new List<ReviewResponse>();
        foreach (var r in reviews)
            response.Add(await ToResponse(r));

        return response;
    }

    private async Task RecalcularRating(string idSerie)
    {
        var stats = await _db.Reviews
            .Find(r => r.IdSerie == idSerie)
            .ToListAsync();

        var promedio = stats.Count > 0
            ? Math.Round(stats.Average(r => r.Puntuacion), 1)
            : 0.0;

        await _db.Series.UpdateOneAsync(
            s => s.Id == idSerie,
            Builders<Series>.Update
                .Set(s => s.RatingPromedio, promedio)
                .Set(s => s.TotalResenas, stats.Count)
        );
    }

    private async Task<ReviewResponse> ToResponse(Review review)
    {
        var user = await _db.Users.Find(u => u.Id == review.IdUsuario).FirstOrDefaultAsync();

        return new ReviewResponse
        {
            Id = review.Id,
            IdUsuario = review.IdUsuario,
            NombreUsuario = user?.Nombre ?? "Desconocido",
            AvatarUsuario = user?.Avatar,
            IdSerie = review.IdSerie,
            Puntuacion = review.Puntuacion,
            Comentario = review.Comentario,
            CreatedAt = review.CreatedAt
        };
    }
}

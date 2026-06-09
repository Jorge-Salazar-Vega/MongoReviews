using MongoDB.Driver;
using MongoReviews.Api.Models;
using MongoReviews.Api.Models.Dtos;

namespace MongoReviews.Api.Services;

public class SeriesService
{
    private readonly MongoDbContext _db;

    public SeriesService(MongoDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<SeriesResponse>> GetAll(int pagina, int limite, string? genero, string ordenarPor, string orden)
    {
        var filter = Builders<Series>.Filter.Empty;
        if (!string.IsNullOrEmpty(genero))
            filter = Builders<Series>.Filter.AnyIn(s => s.Generos, new[] { genero });

        var sort = orden.ToLower() == "desc"
            ? Builders<Series>.Sort.Descending(GetSortField(ordenarPor))
            : Builders<Series>.Sort.Ascending(GetSortField(ordenarPor));

        var total = (int)await _db.Series.CountDocumentsAsync(filter);
        var totalPaginas = (int)Math.Ceiling(total / (double)limite);

        var items = await _db.Series
            .Find(filter)
            .Sort(sort)
            .Skip((pagina - 1) * limite)
            .Limit(limite)
            .Project(s => new SeriesResponse
            {
                Id = s.Id,
                Titulo = s.Titulo,
                Descripcion = s.Descripcion,
                Generos = s.Generos,
                AnioEstreno = s.AnioEstreno,
                Poster = s.Poster,
                Temporadas = s.Temporadas,
                RatingPromedio = s.RatingPromedio,
                TotalResenas = s.TotalResenas
            })
            .ToListAsync();

        return new PagedResult<SeriesResponse>
        {
            Datos = items,
            Total = total,
            Pagina = pagina,
            TotalPaginas = totalPaginas
        };
    }

    public async Task<Series?> GetById(string id)
    {
        return await _db.Series.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Series> Create(CreateSeriesRequest request)
    {
        var series = new Series
        {
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            Generos = request.Generos,
            AnioEstreno = request.AnioEstreno,
            Poster = request.Poster,
            Temporadas = request.Temporadas
        };

        await _db.Series.InsertOneAsync(series);
        return series;
    }

    public async Task<bool> Update(string id, CreateSeriesRequest request)
    {
        var result = await _db.Series.UpdateOneAsync(
            s => s.Id == id,
            Builders<Series>.Update
                .Set(s => s.Titulo, request.Titulo)
                .Set(s => s.Descripcion, request.Descripcion)
                .Set(s => s.Generos, request.Generos)
                .Set(s => s.AnioEstreno, request.AnioEstreno)
                .Set(s => s.Poster, request.Poster)
                .Set(s => s.Temporadas, request.Temporadas)
                .Set(s => s.UpdatedAt, DateTime.UtcNow)
        );
        return result.ModifiedCount > 0;
    }

    public async Task<bool> Delete(string id)
    {
        var result = await _db.Series.DeleteOneAsync(s => s.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<PagedResult<SeriesResponse>> Search(string query, int pagina, int limite)
    {
        var filter = Builders<Series>.Filter.Regex(s => s.Titulo, new MongoDB.Bson.BsonRegularExpression(query, "i"));
        var total = (int)await _db.Series.CountDocumentsAsync(filter);
        var totalPaginas = (int)Math.Ceiling(total / (double)limite);

        var items = await _db.Series
            .Find(filter)
            .Skip((pagina - 1) * limite)
            .Limit(limite)
            .Project(s => new SeriesResponse
            {
                Id = s.Id,
                Titulo = s.Titulo,
                Descripcion = s.Descripcion,
                Generos = s.Generos,
                AnioEstreno = s.AnioEstreno,
                Poster = s.Poster,
                Temporadas = s.Temporadas,
                RatingPromedio = s.RatingPromedio,
                TotalResenas = s.TotalResenas
            })
            .ToListAsync();

        return new PagedResult<SeriesResponse>
        {
            Datos = items,
            Total = total,
            Pagina = pagina,
            TotalPaginas = totalPaginas
        };
    }

    private static string GetSortField(string field) => field.ToLower() switch
    {
        "rating" => nameof(Series.RatingPromedio),
        "anio" => nameof(Series.AnioEstreno),
        _ => nameof(Series.Titulo)
    };
}

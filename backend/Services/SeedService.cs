using MongoDB.Driver;
using MongoReviews.Api.Models;

namespace MongoReviews.Api.Services;

public class SeedService
{
    private readonly MongoDbContext _db;

    public SeedService(MongoDbContext db)
    {
        _db = db;
    }

    public async Task Seed(bool reset = false)
    {
        if (reset)
        {
            await _db.Reviews.DeleteManyAsync(FilterDefinition<Review>.Empty);
            await _db.Series.DeleteManyAsync(FilterDefinition<Series>.Empty);
            await _db.Users.DeleteManyAsync(FilterDefinition<User>.Empty);
        }

        if (await _db.Users.CountDocumentsAsync(FilterDefinition<User>.Empty) > 0)
            return;

        var users = await SeedUsers();
        var series = await SeedSeries();
        await SeedReviews(users, series);
    }

    private async Task<List<User>> SeedUsers()
    {
        var users = new List<User>
        {
            new()
            {
                Nombre = "Admin",
                Email = "admin@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin1234", workFactor: 12),
                Rol = "admin",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Nombre = "María García",
                Email = "maria@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("usuario1234", workFactor: 12),
                Rol = "user",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Nombre = "Carlos López",
                Email = "carlos@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("usuario1234", workFactor: 12),
                Rol = "user",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await _db.Users.InsertManyAsync(users);
        return users;
    }

    private async Task<List<Series>> SeedSeries()
    {
        var series = new List<Series>
        {
            new()
            {
                Titulo = "Breaking Bad",
                Descripcion = "Un profesor de química con cáncer terminal se asocia con un exalumno para fabricar y vender metanfetamina, asegurando el futuro financiero de su familia.",
                Generos = ["Drama", "Suspenso", "Crimen"],
                AnioEstreno = 2008,
                Temporadas = 5,
                Poster = "https://m.media-amazon.com/images/M/MV5BYmQ4YWMxYjUtNjZmYi00MDQ1LWFjMjEtNjZiMWE5MjExNDJiXkEyXkFqcGc@._V1_SX300.jpg",
                RatingPromedio = 9.5,
                TotalResenas = 0
            },
            new()
            {
                Titulo = "Game of Thrones",
                Descripcion = "En un mundo fantástico, familias nobles luchan por el control del Trono de Hierro mientras una antigua amenaza despierta en el norte.",
                Generos = ["Drama", "Acción", "Fantasía"],
                AnioEstreno = 2011,
                Temporadas = 8,
                Poster = "https://m.media-amazon.com/images/M/MV5BYTRiNDQwYzAtMzVlZS00NTI5LWJjYjUtMzkwNTUzMWMxZTllXkEyXkFqcGc@._V1_SX300.jpg",
                RatingPromedio = 9.2,
                TotalResenas = 0
            },
            new()
            {
                Titulo = "Stranger Things",
                Descripcion = "Un grupo de niños en un pequeño pueblo descubren fenómenos sobrenaturales cuando un niño desaparece misteriosamente.",
                Generos = ["Ciencia Ficción", "Terror", "Drama"],
                AnioEstreno = 2016,
                Temporadas = 4,
                Poster = "https://m.media-amazon.com/images/M/MV5BMDZkYmVhNjMtNWU4MC00MDQxLWE3MjYtZGMzZWI1ZjhlOWJmXkEyXkFqcGc@._V1_SX300.jpg",
                RatingPromedio = 8.7,
                TotalResenas = 0
            },
            new()
            {
                Titulo = "The Mandalorian",
                Descripcion = "Un cazarrecompensas solitario en los confines de la galaxia protege a un ser misterioso mientras huye del Imperio.",
                Generos = ["Acción", "Ciencia Ficción", "Aventura"],
                AnioEstreno = 2019,
                Temporadas = 3,
                Poster = "https://m.media-amazon.com/images/M/MV5BZDhlMzY0ZGItZTcyNS00ZTAxLWIyMmYtZjQ4OGI1YjAzODIwXkEyXkFqcGc@._V1_SX300.jpg",
                RatingPromedio = 8.7,
                TotalResenas = 0
            },
            new()
            {
                Titulo = "The Office",
                Descripcion = "Un falso documental sigue la vida cotidiana de los empleados de una oficina de ventas de papel en Scranton, Pensilvania.",
                Generos = ["Comedia"],
                AnioEstreno = 2005,
                Temporadas = 9,
                Poster = "https://m.media-amazon.com/images/M/MV5BMDNkOTE4NDQtMTNmYi00MWE0LWE2ZTctY2NlNTQzMmY0YWRkXkEyXkFqcGc@._V1_SX300.jpg",
                RatingPromedio = 9.0,
                TotalResenas = 0
            },
            new()
            {
                Titulo = "Dark",
                Descripcion = "Una desaparición en un pequeño pueblo alemán desata una compleja historia que abarca cuatro generaciones y múltiples líneas temporales.",
                Generos = ["Drama", "Suspenso", "Ciencia Ficción"],
                AnioEstreno = 2017,
                Temporadas = 3,
                Poster = "https://m.media-amazon.com/images/M/MV5BOTk2NzYyMzctNzQ5Ni00NTE1LTkyNGEtYzE5MTRhMzhhNTFkXkEyXkFqcGc@._V1_SX300.jpg",
                RatingPromedio = 8.8,
                TotalResenas = 0
            },
            new()
            {
                Titulo = "Black Mirror",
                Descripcion = "Una serie antológica que explora un futuro cercano donde la tecnología tiene consecuencias inesperadas y a menudo perturbadoras.",
                Generos = ["Ciencia Ficción", "Suspenso", "Drama"],
                AnioEstreno = 2011,
                Temporadas = 6,
                Poster = "https://m.media-amazon.com/images/M/MV5BYjJiYTgwYzUtZmU4Yy00NjBiLWIyYjUtYmM2YTA1YjA3MjE5XkEyXkFqcGc@._V1_SX300.jpg",
                RatingPromedio = 8.8,
                TotalResenas = 0
            },
            new()
            {
                Titulo = "The Last of Us",
                Descripcion = "Veinte años después de que un hongo parasitario devastara la civilización, un superviviente escolta a una adolescente inmune a través de Estados Unidos.",
                Generos = ["Drama", "Acción", "Terror"],
                AnioEstreno = 2023,
                Temporadas = 1,
                Poster = "https://m.media-amazon.com/images/M/MV5BZWIyYzI1MjgtYjE1ZS00MDFhLWE3NTItNzFhNDFhMGJkMTQxXkEyXkFqcGc@._V1_SX300.jpg",
                RatingPromedio = 8.7,
                TotalResenas = 0
            },
            new()
            {
                Titulo = "Squid Game",
                Descripcion = "Cientos de personas endeudadas aceptan una misteriosa invitación a un juego de supervivencia donde el ganador se lleva un premio multimillonario.",
                Generos = ["Drama", "Suspenso", "Terror"],
                AnioEstreno = 2021,
                Temporadas = 1,
                Poster = "https://m.media-amazon.com/images/M/MV5BYTkwYjQ1ZTMtMjY4Ni00MDI4LThlOTMtMDc4OTMzNTc3Y2I4XkEyXkFqcGc@._V1_SX300.jpg",
                RatingPromedio = 7.8,
                TotalResenas = 0
            },
            new()
            {
                Titulo = "The Crown",
                Descripcion = "Sigue la vida y el reinado de la reina Isabel II desde su boda en 1947 hasta el siglo XXI, explorando los desafíos personales y políticos.",
                Generos = ["Drama", "Historia"],
                AnioEstreno = 2016,
                Temporadas = 6,
                Poster = "https://m.media-amazon.com/images/M/MV5BZmY0MzBlNjctN2QwOS00YjBmLWFmMzctY2MzMzI5MjU5M2I4XkEyXkFqcGc@._V1_SX300.jpg",
                RatingPromedio = 8.6,
                TotalResenas = 0
            }
        };

        await _db.Series.InsertManyAsync(series);
        return series;
    }

    private async Task SeedReviews(List<User> users, List<Series> series)
    {
        var reviews = new List<Review>
        {
            new()
            {
                IdUsuario = users[1].Id,
                IdSerie = series[0].Id,
                Puntuacion = 10,
                Comentario = "Una obra maestra absoluta. La evolución de Walter White es increíble de principio a fin.",
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new()
            {
                IdUsuario = users[2].Id,
                IdSerie = series[0].Id,
                Puntuacion = 9,
                Comentario = "Una de las mejores series de la historia. Solo el final de Game of Thrones le gana en controversia.",
                CreatedAt = DateTime.UtcNow.AddDays(-25)
            },
            new()
            {
                IdUsuario = users[0].Id,
                IdSerie = series[1].Id,
                Puntuacion = 8,
                Comentario = "Temporadas 1-4 son perfectas. Las últimas temporadas bajaron el nivel, pero sigue siendo épica.",
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new()
            {
                IdUsuario = users[1].Id,
                IdSerie = series[1].Id,
                Puntuacion = 10,
                Comentario = "A pesar de las críticas al final, nada supera la experiencia de verla por primera vez.",
                CreatedAt = DateTime.UtcNow.AddDays(-18)
            },
            new()
            {
                IdUsuario = users[2].Id,
                IdSerie = series[2].Id,
                Puntuacion = 9,
                Comentario = "Nostalgia ochentera perfecta. Los chicos son increíbles y la ambientación es impecable.",
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new()
            {
                IdUsuario = users[1].Id,
                IdSerie = series[3].Id,
                Puntuacion = 8,
                Comentario = "Baby Yoda se robó el show. Buena historia aunque algo lenta en la segunda temporada.",
                CreatedAt = DateTime.UtcNow.AddDays(-12)
            },
            new()
            {
                IdUsuario = users[2].Id,
                IdSerie = series[4].Id,
                Puntuacion = 9,
                Comentario = "Michael Scott es el mejor personaje de la TV. La temporada después de su salida decayó mucho.",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new()
            {
                IdUsuario = users[0].Id,
                IdSerie = series[5].Id,
                Puntuacion = 10,
                Comentario = "La mejor serie de ciencia ficción que he visto. Cada detalle está cuidadosamente planeado.",
                CreatedAt = DateTime.UtcNow.AddDays(-8)
            },
            new()
            {
                IdUsuario = users[1].Id,
                IdSerie = series[5].Id,
                Puntuacion = 8,
                Comentario = "Compleja pero fascinante. Necesitas verla con atención o te pierdes completamente.",
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            },
            new()
            {
                IdUsuario = users[2].Id,
                IdSerie = series[6].Id,
                Puntuacion = 9,
                Comentario = "\"San Junipero\" y \"USS Callister\" son de lo mejor que ha dado la televisión.",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                IdUsuario = users[0].Id,
                IdSerie = series[7].Id,
                Puntuacion = 9,
                Comentario = "Fiel al juego, con actuaciones espectaculares. Pedro Pascal es perfecto como Joel.",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new()
            {
                IdUsuario = users[1].Id,
                IdSerie = series[7].Id,
                Puntuacion = 10,
                Comentario = "La mejor adaptación de un videojuego a la TV. La historia te parte el corazón.",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new()
            {
                IdUsuario = users[2].Id,
                IdSerie = series[8].Id,
                Puntuacion = 7,
                Comentario = "Entretenida y adictiva, pero se nota mucho el estilo coreano. El final es predecible.",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                IdUsuario = users[0].Id,
                IdSerie = series[9].Id,
                Puntuacion = 8,
                Comentario = "Excelente producción y actuaciones. Algunas temporadas son más lentas que otras.",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                IdUsuario = users[1].Id,
                IdSerie = series[9].Id,
                Puntuacion = 9,
                Comentario = "Claire Foy como la reina Isabel es simplemente perfecta. Una mirada íntima a la monarquía.",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        await _db.Reviews.InsertManyAsync(reviews);

        foreach (var serie in series)
        {
            var serieReviews = reviews.Where(r => r.IdSerie == serie.Id).ToList();
            if (serieReviews.Count > 0)
            {
                var promedio = Math.Round(serieReviews.Average(r => r.Puntuacion), 1);
                await _db.Series.UpdateOneAsync(
                    s => s.Id == serie.Id,
                    Builders<Series>.Update
                        .Set(s => s.RatingPromedio, promedio)
                        .Set(s => s.TotalResenas, serieReviews.Count)
                );
            }
        }
    }
}

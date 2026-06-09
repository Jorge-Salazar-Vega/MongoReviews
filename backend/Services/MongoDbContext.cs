using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoReviews.Api.Models;

namespace MongoReviews.Api.Services;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<Series> Series => _database.GetCollection<Series>("series");
    public IMongoCollection<Review> Reviews => _database.GetCollection<Review>("reviews");
}

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoReviews.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JWT"));

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SeriesService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<SeedService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JWT");
        var secret = jwtSettings["Secret"]!;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

if (args.Length > 0 && args[0] == "seed")
{
    var reset = args.Contains("--reset");
    var sp = builder.Services.BuildServiceProvider();
    var seedService = sp.GetRequiredService<SeedService>();
    await seedService.Seed(reset);
    Console.WriteLine(reset ? "Datos reiniciados y creados." : "Datos de prueba insertados.");
    return;
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

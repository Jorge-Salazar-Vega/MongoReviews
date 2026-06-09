using System.ComponentModel.DataAnnotations;

namespace MongoReviews.Api.Models.Dtos;

public class UpdatePerfilRequest
{
    [StringLength(50, MinimumLength = 3)]
    public string? Nombre { get; set; }

    public string? Avatar { get; set; }
}

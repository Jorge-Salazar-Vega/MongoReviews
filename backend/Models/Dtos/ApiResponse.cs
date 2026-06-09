namespace MongoReviews.Api.Models.Dtos;

public class ApiResponse<T>
{
    public bool Exitoso { get; set; }
    public T? Datos { get; set; }
    public string? Mensaje { get; set; }
    public List<string>? Errores { get; set; }

    public static ApiResponse<T> Ok(T datos, string? mensaje = null) => new()
    {
        Exitoso = true,
        Datos = datos,
        Mensaje = mensaje
    };

    public static ApiResponse<T> Error(string mensaje, List<string>? errores = null) => new()
    {
        Exitoso = false,
        Mensaje = mensaje,
        Errores = errores
    };
}

public class PagedResult<T>
{
    public List<T> Datos { get; set; } = [];
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TotalPaginas { get; set; }
}

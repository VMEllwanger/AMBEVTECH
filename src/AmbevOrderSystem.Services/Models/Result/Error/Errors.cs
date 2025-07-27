using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace AmbevOrderSystem.Services.Models.Result.Error;

/// <summary>
/// Classe de erros genéricos.
/// </summary>
[ExcludeFromCodeCoverage]
public static class GenericErrors
{
    public static readonly ErrorModel InternalServerError = new("999", "Internal server error", HttpStatusCode.InternalServerError);
}

/// <summary>
/// Classe de erros de validação de entrada.
/// </summary>
[ExcludeFromCodeCoverage]
public static class InputValidationErrors
{
    public static ErrorModel GetCustomModelStateValidationError(string errorMessage)
    {
        return new ErrorModel("VLP059", errorMessage, HttpStatusCode.BadRequest);
    }
}

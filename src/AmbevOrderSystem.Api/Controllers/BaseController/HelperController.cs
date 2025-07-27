using AmbevOrderSystem.Api.Response.Error;
using AmbevOrderSystem.Services.Models.Result;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace AmbevOrderSystem.Api.Controllers.Base;

/// <summary>
/// Controller que expõe métodos para padronizar respostas de erro.
/// </summary> 
[ExcludeFromCodeCoverage]
public abstract class HelperController : ControllerBase
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult GenerateErrorResponse(Result failResult)
    {
        HttpErrorResponse errorResponse = new(failResult);

        return StatusCode((int)errorResponse.HttpStatusCode, errorResponse);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult GenerateErrorResponse(HttpErrorResponse errorResponse)
    {
        return StatusCode((int)errorResponse.HttpStatusCode, errorResponse);
    }
}

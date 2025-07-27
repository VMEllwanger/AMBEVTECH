
using AmbevOrderSystem.Api.Response.Error;
using AmbevOrderSystem.Services.Models.Result;
using AmbevOrderSystem.Services.Models.Result.Error;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AmbevOrderSystem.Api.ModelState
{
    public class CustomModelStateResponseFactory : IActionResult
    {
        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var invalidData = context.ModelState.FirstOrDefault(x => x.Key != "request");
                var errorMessage = invalidData.Value is not null ? $"O campo '{JsonNamingPolicy.CamelCase.ConvertName(invalidData.Key)}' é obrigatório." : "Payload inválido";

                Console.WriteLine($"Erro de validação: {errorMessage}");

                Result error = Result.Fail(InputValidationErrors.GetCustomModelStateValidationError(errorMessage));
                HttpErrorResponse badRequestResponse = new(error);

                var objectResult = new ObjectResult(badRequestResponse) { StatusCode = (int)System.Net.HttpStatusCode.BadRequest };

                await objectResult.ExecuteResultAsync(context);
            }
        }
    }
}

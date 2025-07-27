using AmbevOrderSystem.Services.Models.Result;
using AmbevOrderSystem.Services.Models.Result.Error;
using System.Net;
using System.Text.Json.Serialization;

namespace AmbevOrderSystem.Api.Response.Error
{
    public struct HttpErrorResponse
    {
        private const string _API_VERSION = "1.0.0";

        public HttpErrorResponse(Result result)
        {
            if (result.IsSuccessful)
            {
                throw new InvalidOperationException("A operação foi bem-sucedida, portanto não há erro para retornar.");
            }

            Version = _API_VERSION;


            if (result.Error is not null)
            {
                HttpStatusCode = result.Error.HttpStatusCode;
                Error = new HttpErrorResponseDetail(result.Error);
            }

            else if (result.ValidationErrors?.Any() == true)
            {
                HttpStatusCode = HttpStatusCode.BadRequest;
                Error = new HttpErrorResponseDetail("VALIDATION_ERROR", string.Join("; ", result.ValidationErrors));
            }

            else if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                HttpStatusCode = HttpStatusCode.BadRequest;
                Error = new HttpErrorResponseDetail("GENERIC_ERROR", result.ErrorMessage);
            }
            else
            {
                HttpStatusCode = HttpStatusCode.InternalServerError;
                Error = new HttpErrorResponseDetail("UNKNOWN_ERROR", "Erro desconhecido");
            }
        }

        [JsonPropertyName("status")]
        public HttpStatusCode HttpStatusCode { get; private set; }

        [JsonPropertyName("version")]
        public string Version { get; private set; }

        [JsonPropertyName("error")]
        public HttpErrorResponseDetail Error { get; private set; }
    }

    public struct HttpErrorResponseDetail
    {
        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; private set; }

        [JsonPropertyName("message")]
        public string Message { get; private set; }

        public HttpErrorResponseDetail(ErrorModel error)
        {
            ErrorCode = error.Code;
            Message = error.Message;
        }

        public HttpErrorResponseDetail(string errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }
    }
}
using System.Net;

namespace AmbevOrderSystem.Services.Models.Result.Error
{
    public sealed class ErrorModel
    {
        public ErrorModel(string code, string description, HttpStatusCode httpStatusCode)
        {
            Code = code;
            Message = description;
            HttpStatusCode = httpStatusCode;
        }

        public string Code { get; private set; }

        public string Message { get; private set; }

        public HttpStatusCode HttpStatusCode { get; private set; }
    }
}

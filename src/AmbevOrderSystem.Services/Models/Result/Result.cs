using AmbevOrderSystem.Services.Models.Result.Error;
using System.Net;

namespace AmbevOrderSystem.Services.Models.Result
{
    public sealed record Result<T> : Result
    {
        public T? Data { get; private set; }
        public T? Value { get; private set; }
        public List<string> ValidationErrors { get; private set; } = new();
        public string? ErrorMessage { get; private set; }

        private Result(T value) : base()
        {
            Value = value;
            Data = value;
        }

        private Result(ErrorModel errorModel, bool isPermanent) : base(errorModel, isPermanent)
        {
            ErrorMessage = errorModel.Message;
        }

        private Result(string errorMessage) : base(errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        private Result(List<string> validationErrors) : base(validationErrors)
        {
            ValidationErrors = validationErrors;
        }

        public static Result<T> Success(T value) => new Result<T>(value);
        public static Result<T> Failure(string errorMessage) => new Result<T>(errorMessage);
        public static Result<T> ValidationFailure(List<string> validationErrors) => new Result<T>(validationErrors);

        public static new Result<T> Fail(ErrorModel errorModel, bool isPermanent) => new Result<T>(errorModel, isPermanent);
        public static new Result<T> Fail(ErrorModel errorModel) => new Result<T>(errorModel, false);
        public static Result<T> Fail(string errorMessage) => new Result<T>(new ErrorModel("GENERIC_ERROR", errorMessage, HttpStatusCode.BadRequest), false);

        public static explicit operator Result<T>(ErrorModel errorModel) => Fail(errorModel);
    }


    public record Result
    {
        public List<string> ValidationErrors { get; protected set; } = new();
        public string? ErrorMessage { get; protected set; }

        protected Result()
        {
            IsSuccessful = true;
        }

        protected Result(ErrorModel errorModel, bool isPermanent)
        {
            IsSuccessful = false;
            Error = errorModel;
            IsPermanent = isPermanent;
            ErrorMessage = errorModel.Message;
        }

        protected Result(List<string> validationErrors)
        {
            IsSuccessful = false;
            ValidationErrors = validationErrors;
        }

        protected Result(string errorMessage)
        {
            IsSuccessful = false;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccessful { get; protected set; }
        public bool IsSuccess { get => IsSuccessful; }
        public bool IsFail { get => !IsSuccessful; }
        public bool IsPermanent { get; private set; }
        public ErrorModel? Error { get; private set; }

        public static Result Success() => new Result();
        public static Result Failure(string errorMessage) => new Result(errorMessage);
        public static Result ValidationFailure(List<string> validationErrors) => new Result(validationErrors);
        public static Result Fail(ErrorModel errorModel, bool isPermanent) => new Result(errorModel, isPermanent);
        public static Result Fail(ErrorModel errorModel) => new Result(errorModel, false);
        public static Result Fail(string errorMessage) => new Result(new ErrorModel("GENERIC_ERROR", errorMessage, HttpStatusCode.BadRequest), false);
    }
}

using System;
using System.Net;
using AuthoBson.Shared.Extensions;

namespace AuthoBson.Shared.Results
{
    public class ServiceResult
    {
        public bool IsSuccessful => StatusCode.IsSuccessful();
        public HttpStatusCode StatusCode { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();

        public static ServiceResult Ok()
            => new()
            {
                StatusCode = HttpStatusCode.OK
            };

        public static ServiceResult<TData> Ok<TData>(TData data) where TData : class 
            => new ()
            {
                StatusCode = HttpStatusCode.OK,
                Data = data
            };


        public static ServiceResult Accepted() 
            => new ()
            {
                StatusCode = HttpStatusCode.Accepted
            };

        public static ServiceResult<TData> Accepted<TData>(TData data) where TData : class 
            => new ()
            {
                StatusCode = HttpStatusCode.Accepted,
                Data = data
            };


        public static ServiceResult BadRequest(params string[] errors) 
            => new ()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Errors = errors
            };

        public static ServiceResult<TData> BadRequest<TData>(params string[] errors) where TData : class 
            => new ()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Errors = errors
            };


        public static ServiceResult Unauthorized() 
            => new ()
            {
                StatusCode = HttpStatusCode.Unauthorized,
            };

        public static ServiceResult<TData> Unauthorized<TData>() where TData : class 
            => new ()
            {
                StatusCode = HttpStatusCode.Unauthorized,
            };


        public static ServiceResult NotFound(params string[] errors) 
            => new ()
            {
                StatusCode = HttpStatusCode.NotFound,
                Errors = errors
            };

        public static ServiceResult<TData> NotFound<TData>(params string[] errors) where TData : class 
            => new ()
            {
                StatusCode = HttpStatusCode.NotFound,
                Errors = errors
            };


        public static ServiceResult Conflict(params string[] errors) 
            => new ()
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = errors
            };

        public static ServiceResult<TData> Conflict<TData>(params string[] errors) where TData : class 
            => new ()
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = errors
            };


        public static ServiceResult UnprocessableEntity(params string[] errors) 
            => new ()
            {
                StatusCode = HttpStatusCode.UnprocessableEntity,
                Errors = errors
            };

        public static ServiceResult<TData> UnprocessableEntity<TData>(params string[] errors) where TData : class 
            => new ()
            {
                StatusCode = HttpStatusCode.UnprocessableEntity,
                Errors = errors
            };

        public static ServiceResult NotImplemented(params string[] errors) 
            => new ()
            {
                StatusCode = HttpStatusCode.NotImplemented,
                Errors = errors
            };

        public static ServiceResult<TData> NotImplemented<TData>(params string[] errors) where TData : class 
            => new ()
            {
                StatusCode = HttpStatusCode.NotImplemented,
                Errors = errors
            };
        
        //
        // In case you will need to provide a custom API return code, add an entry to Extensions.HttpStatusCodeExtras
        // and cast make an object like above and fill the status code as:
        //
        //   StatusCode = (HttpStatusCode) HttpStatusCodeExtras.MyCustomStatus
        // 
    }

    public class ServiceResult<TData> : ServiceResult where TData : class
    {
        public TData Data { get; set; }

        public static ServiceResult<TData> FromDataless(ServiceResult serviceResult) 
            => new ()
            {
                StatusCode = serviceResult.StatusCode,
                Errors = serviceResult.Errors
            };
    }
}
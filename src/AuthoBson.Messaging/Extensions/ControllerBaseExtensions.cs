using Microsoft.AspNetCore.Mvc;
using AuthoBson.Messaging.Services.Shared;

namespace AuthoBson.Messaging.Extensions
{
    public static class ControllerBaseExtensions
    {   
        public static ActionResult FromServiceResult<TData>(
            this ControllerBase controllerBase, ServiceResult<TData> result
        ) where TData : class
        {
            if (result.IsSuccessful)
            {
                return controllerBase.StatusCode((int) result.StatusCode, result.Data);
            }

            return controllerBase.StatusCode((int) result.StatusCode, new ErrorResult
            {
                Errors = result.Errors
            });
        }
        
        public static ActionResult FromServiceResult (
            this ControllerBase controllerBase, ServiceResult result
        )
        {
            if (result.IsSuccessful)
            {
                return controllerBase.StatusCode((int) result.StatusCode);
            }

            return controllerBase.StatusCode((int) result.StatusCode, new ErrorResult
            {
                Errors = result.Errors
            });
        }
    }
}
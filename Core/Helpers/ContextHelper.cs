using Azure;
using Core.Enums;
using Core.Models;
using Core.ResourcesFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public static class ContextHelper
    {
        public static void GetError(this ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var exception = context.Exception;
                context.Exception = null;
                context.ExceptionDispatchInfo = null;

                _ = Logger.Instance.LogErrorAsync(context.ActionDescriptor.DisplayName + Constants.Arrow, exception);

                var response = new ResponseModel { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong };
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.HttpContext.Response.WriteAsJsonAsync(response).Wait();
            }
        }
    }
}

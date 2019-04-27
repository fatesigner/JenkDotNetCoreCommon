using JenkDotNetCoreCommon.Extensions;
using JenkDotNetCoreCommon.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace JenkDotNetCoreCommon.Middleware
{
    public class ErrorHandler
    {
        private readonly RequestDelegate next;

        public ErrorHandler(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // 错误状态 默认为500
            int errorCode = 500;
            HttpException httpEx = exception as HttpException;
            var errorResult = "";

            if (httpEx != null && httpEx.MessageData != null)
            {
                errorResult = JsonConvert.SerializeObject(httpEx.MessageData);
            }
            else
            {
                errorResult = JsonConvert.SerializeObject(new HttpErrorMessage()
                {
                    error = true,
                    message = exception.GetErrorMessage()
                });
            }

            if (httpEx != null)
            {
                errorCode = httpEx.StatusCode;
            }

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = errorCode;

            return context.Response.WriteAsync(errorResult);
        }
    }
}

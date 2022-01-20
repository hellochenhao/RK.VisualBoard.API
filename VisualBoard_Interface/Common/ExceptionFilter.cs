using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using System.Threading.Tasks;

namespace VisualBoard_Interface.Common
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            context.Result = new JsonResult(new
            {
                message = "系统发生异常",
                code = 500,
                result = context.Exception.Message,
            });
            Console.WriteLine(context.HttpContext.Request.Path);
            Console.WriteLine(context.HttpContext.Request.Body);
            Console.WriteLine(context.Exception.Message);
            //context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            return Task.CompletedTask;
        }
    }
}

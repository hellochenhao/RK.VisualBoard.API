using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VisualBoard_Interface.Common
{
    public class GlobalActionFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var noNeedLoginAttribute = controllerActionDescriptor.
                                   ControllerTypeInfo.
                                   GetCustomAttributes(true)
                                   .Where(a => a.GetType().Equals(typeof(NoLoginAttribute)))
                                   .ToList();
                noNeedLoginAttribute.AddRange(controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                                 .Where(a => a.GetType().Equals(typeof(NoLoginAttribute))).ToList());


                //如果标记了 NoLoginAttribute 则不验证其登录状态
                if (noNeedLoginAttribute.Any())
                {
                    var httpContext = context.HttpContext;
                    var request = httpContext.Request;
                    request.Body.Position = 0;
                    StreamReader sr = new StreamReader(request.Body);
                    string body = await sr.ReadToEndAsync();
                    var js = JsonConvert.DeserializeObject<JObject>(body);
                    //var time = (DateTime?)js["CreateTime2"];
                    return;
                }
            }

            // before Action
     
            context.Result = new JsonResult(new
            {
                message = "请重新登陆系统",
                code = 401,
                result = string.Empty,
            });
            return;
            //Action            
            var aaa=  await next();
            var reee = aaa.Result;
           
        } 

    }
}

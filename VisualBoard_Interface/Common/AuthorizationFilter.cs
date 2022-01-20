using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Rokin.Common.Redis;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VisualBoard_Interface.Common
{
    public class AuthorizationFilter : IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
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

                var nAuthorizeAttribute = controllerActionDescriptor.
                                  ControllerTypeInfo.
                                  GetCustomAttributes(true)
                                  .Where(a => a.GetType().Equals(typeof(AuthorizeAttribute)))
                                  .ToList();
                nAuthorizeAttribute.AddRange(controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                                .Where(a => a.GetType().Equals(typeof(AuthorizeAttribute))).ToList());
                //如果标记了 NoLoginAttribute 则不验证其登录状态
                if (noNeedLoginAttribute.Any()||!nAuthorizeAttribute.Any())
                {
                    return Task.CompletedTask;
                }
            }

            var token = context.HttpContext.Request.Headers["Authorization"].ToString();
            var user = context.HttpContext.Request.Headers["user"].ToString();
            var UserToken= RedisHelper.HGet("UserToken", user);
            if (token!=null&&token.Length>0&& token.Substring(7)== UserToken)
            {
                    return Task.CompletedTask;
      
            }

            context.Result = new JsonResult(new
            {
                message = "请重新登陆系统",
                code = 401,
                result=string.Empty,
            });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

    }

    public class NoLoginAttribute : Attribute { }
}

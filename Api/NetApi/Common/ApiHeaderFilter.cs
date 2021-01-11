using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetApi.Common
{
    public class ApiHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            var attrs = context.ApiDescription.ActionDescriptor.AttributeRouteInfo;

            //先判断是否是匿名访问,
            var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor != null)
            {
                var actionAttributes = descriptor.MethodInfo.GetCustomAttributes(inherit: true);

                bool isAnonymous = actionAttributes.Any(a => a is AllowAnonymousAttribute);
                bool isAuthorized = actionAttributes.Any(a => a is AuthorizeAttribute);

                //非匿名的方法,链接中添加accesstoken值
                if (!isAnonymous && isAuthorized)
                {
                    //operation.Parameters.Add(new OpenApiParameter()
                    //{
                    //    Name = "Authorization",
                    //    In = ParameterLocation.Header,//query header body path formData
                    //    Description = "Token验证",
                    //    Schema = new OpenApiSchema()
                    //    {
                    //        Type = "string",
                    //    },
                    //    Required = true //是否必选
                    //});
                }

                #region 自定义头部
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "userId",
                    In = ParameterLocation.Header,
                    Description = "用户主键",
                    Schema = new OpenApiSchema()
                    {
                        Type = "string",
                    },
                    Required = false
                });
                //operation.Parameters.Add(new OpenApiParameter()
                //{
                //    Name = "departmentId",
                //    In = ParameterLocation.Header,
                //    Description = "部门主键",
                //    Schema = new OpenApiSchema()
                //    {
                //        Type = "string",
                //    },
                //    Required = false
                //});
                #endregion
            }
        }
    }
}

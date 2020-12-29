using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NetApi.Business;
using NetApi.Common;
using NetApi.Models;
using NetApi.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetApi.Controllers
{
    /// <summary>
    /// User
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private Users _us;
        private readonly IJwtAuthManager _jwtAuthManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jwtAuthManager"></param>
        public UsersController(NetApiContext context, IJwtAuthManager jwtAuthManager)
        {
            _us = new Users(context);
            _jwtAuthManager = jwtAuthManager;
        }

        /// <summary>
        /// token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public JwtAuthResult Token([FromBody] userlogin user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "asd")
            };
            return _jwtAuthManager.GenerateTokens(user.Account, claims, DateTime.Now);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ApiResult GetUsers()
        {
            return _us.GetUsers();
        }

        /// <summary>
        /// test
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ApiResult Test()
        {
            return new ApiResult() { status = EnumStatus.OK, msg = "success" };
        }

    }
}

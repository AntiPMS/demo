using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NetApi.Business;
using NetApi.Common;
using NetApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        public UsersController(NetApiContext context)
        {
            _us = new Users(context);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResult GetUsers()
        {
            return _us.GetUsers();
        }
    }
}

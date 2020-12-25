using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NetApi.Business;
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
        private Users us;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_context"></param>
        /// <param name="_env"></param>
        public UsersController(NetApiContext _context, IWebHostEnvironment _env)
        {
            us = new Users(_context, _env);
        }

        /// <summary>
        /// demo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Demo()
        {
            return "Hello World!";
        }

        /// <summary>
        /// 获取当前环境
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetEnviroment()
        {
            return us.GetEnviroment();
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<users> GetUsers()
        {
            return us.GetUsers();
        }
    }
}

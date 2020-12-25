using Microsoft.AspNetCore.Hosting;
using NetApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetApi.Business
{

    public class Users
    {
        private readonly NetApiContext context;
        private readonly IWebHostEnvironment env;

        #region  构造函数,注入依赖.
        public Users(NetApiContext _context, IWebHostEnvironment _env)
        {
            context = _context;
            env = _env;
        }
        #endregion

        public string GetEnviroment()
        {
            return env.EnvironmentName;
        }

        public List<users> GetUsers()
        {
            return context.users.ToList();
        }
    }
}

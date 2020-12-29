using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NetApi.Common;
using NetApi.Models;
using NetApi.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetApi.Business
{

    public class Users
    {
        private readonly NetApiContext _context;

        #region  构造函数,注入依赖.
        public Users(NetApiContext context)
        {
            _context = context;
        }
        #endregion

        public ApiResult GetUsers()
        {
            ApiResult ast = new ApiResult() { status = EnumStatus.OK, totalCount = 0 };
            try
            {
                var data = _context.users.Select(m => new
                {
                    m.Id,
                    m.Account,
                    m.Name,
                    m.Remarks
                }).ToList();
                ast.content = data;
                ast.totalCount = data.Count;
            }
            catch (Exception e)
            {
                ast.status = EnumStatus.InternalServerError;
                ast.msg = e.ToString();
            }
            return ast;
        }
    }
}

﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetApi.Business;
using NetApi.Common;
using NetApi.Models;
using NetApi.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly NetApiContext _context;
        private readonly IWebsocketManager _wsManage;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jwtAuthManager"></param>
        /// <param name="webHostEnvironment"></param>
        /// <param name="configuration"></param>
        /// <param name="wsManage"></param>
        public UsersController(
            NetApiContext context
            , IJwtAuthManager jwtAuthManager
            , IWebHostEnvironment webHostEnvironment
            , IConfiguration configuration
            , IWebsocketManager wsManage)
        {
            _context = context;
            _us = new Users(context);
            _jwtAuthManager = jwtAuthManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _wsManage = wsManage;
        }

        /// <summary>
        /// 环境参数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ApiResult GetEnv()
        {
            return new ApiResult() { content = $@"env={_webHostEnvironment.EnvironmentName}, EnvTest:str={_configuration["EnvTest:str"]}" };
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userlogin">ViewModel:<see cref="RequestViewUsers"/></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ApiResult Login(RequestViewUsers userlogin)
        {
            ApiResult result = new ApiResult() { status = EnumStatus.OK, totalCount = 0 };
            try
            {
                userlogin.Pwd = MD5Helper.EnCode(userlogin.Pwd);//MD5加密
                var user = _context.users.FirstOrDefault(m => m.Account == userlogin.Account && m.Pwd == userlogin.Pwd);
                if (user != null)
                {
                    var claims = new[]
                    {
                        new Claim("Id",user.Id.ToString()),
                        new Claim("Account",user.Account),
                        new Claim("Name", user.Name),
                        new Claim(ClaimTypes.Role, "AdminRole")
                    };
                    var jwt = _jwtAuthManager.GenerateTokens(
                            new TokenUser() { Id = user.Id.ToString(), Account = user.Account, Name = user.Name }
                            , claims);
                    result.content = new { jwt.RefreshToken.User, jwt.AccessToken, RefreshToken = jwt.RefreshToken.TokenString };
                    result.msg = "登录成功!";
                }
                else
                {
                    result.status = EnumStatus.NotFound;
                    result.msg = "用户名或密码错误";
                }

            }
            catch (Exception e)
            {
                result.status = EnumStatus.InternalServerError;
                result.msg = e.ToString();
            }
            return result;
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="rtoken"><see cref="RequestViewRefreshToken.RefreshToken"/></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ApiResult RefreshToken(RequestViewRefreshToken rtoken)
        {
            ApiResult art = new ApiResult() { status = EnumStatus.OK, totalCount = 0 };
            try
            {
                if (string.IsNullOrWhiteSpace(rtoken.RefreshToken))
                {
                    art.status = EnumStatus.BadRequest;
                    art.msg = "RefreshToken不能为空";
                }
                else
                {
                    var uName = User.Identity.Name;
                    var uClaims = User.Claims.ToList();
                    var accessToken = HttpContext.GetTokenAsync("Bearer", "access_token").Result;
                    var jwt = _jwtAuthManager.Refresh(rtoken.RefreshToken, accessToken);
                    art.content = new { jwt.RefreshToken.User, jwt.AccessToken, RefreshToken = jwt.RefreshToken.TokenString };
                    art.msg = "刷新成功!";
                }
            }
            catch (Exception e)
            {
                art.status = EnumStatus.InternalServerError;
                art.msg = e.ToString();
            }
            return art;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns><see cref="users"/></returns>
        [HttpGet]
        [Authorize]
        public ApiResult GetUsers()
        {
            return _us.GetUsers();
        }

        /// <summary>
        /// 分页查询用户
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ApiResult GetUsersPage(int pageIndex, int pageSize)
        {
            return _us.GetUsersPage(pageIndex, pageSize);
        }

        /// <summary>
        /// 按名字查询用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ApiResult GetUsersByName(string name)
        {
            return _us.GetUsersByName(name);
        }

        /// <summary>
        /// 批量新增用户
        /// </summary>
        /// <param name="newUsers"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ApiResult AddUsers(List<RequestInsertUsers> newUsers)
        {
            return _us.AddUsers(newUsers);
        }

        /// <summary>
        /// 批量修改用户
        /// </summary>
        /// <param name="editUsers"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ApiResult UpdateUsers(List<RequestUpdateUsers> editUsers)
        {
            return _us.UpdateUsers(editUsers);
        }

        /// <summary>
        /// 批量删除用户
        /// </summary>
        /// <param name="usersId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ApiResult DelUsers(List<int> usersId)
        {
            return _us.DelUsers(usersId);
        }

        /// <summary>
        /// 获取当前所有在线socket
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<WebSocketClient> GetCurrentAll()
        {
            return _wsManage.GetCurrentAll();
        }

        /// <summary>
        /// 历史消息查询
        /// </summary>
        /// <param name="lastSendDate">最早消息的日期(精确到秒)</param>
        /// <param name="targetId">取咨询的主键</param>
        /// <param name="num">本次读取的消息数(≥1)</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public OutPut<List<UserMsg>> GetUserHisChatMsgBySendDateDecreasing(DateTime lastSendDate, string targetId, int num)
            => Users.GetUserHisChatMsgBySendDateDecreasing(_context, lastSendDate, targetId, num);

        /// <summary>
        /// 发送系统消息
        /// </summary>
        /// <param name="info">消息内容</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public OutPut SendMsg2User(MsgUserInfo info)
            => Users.SendMsg2User(_context, _wsManage, info);

        /// <summary>
        /// 系统消息查询
        /// </summary>
        /// <param name="userId">当前用户Id，即targetId</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public OutPut<List<userMsg>> GetSystemMsg(string userId)
            => Users.GetUserMsg(_context, userId, MsgType.System);

        /// <summary>
        /// 消息类型枚举
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public OutPut<Dictionary<string, int>> GetMsgTypeEnum()
            => Users.GetMsgTypeEnum(_wsManage);
    }
}

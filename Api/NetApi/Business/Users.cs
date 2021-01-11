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
    /// <summary>
    /// 用户功能 实现层
    /// </summary>
    public class Users
    {
        private readonly NetApiContext _context;

        #region  构造函数,注入依赖.
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        public Users(NetApiContext context)
        {
            _context = context;
        }
        #endregion

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public ApiResult GetUsers()
        {
            ApiResult result = new ApiResult() { status = EnumStatus.OK, totalCount = 0 };
            try
            {
                //_context --> pb 数据窗口库
                var data = _context.users
                                   //.GroupBy(m => m.Birthday, (gourpKey, n) => new
                                   //{
                                   //    Birthday = gourpKey,
                                   //    MaxId = n.Max(x => x.Id),
                                   //    Cnt = n.Count()
                                   //})
                                   .Select(m => new
                                   {
                                       //m.Birthday,
                                       //m.MaxId,
                                       //m.Cnt
                                       m.Id,
                                       m.Account,
                                       m.Name,
                                       m.Remarks
                                   });
                if (data.Any())
                {
                    result.content = data.ToList();
                    result.totalCount = data.Count();
                    result.msg = "查询成功!";
                }
                else
                {
                    result.msg = "暂无数据!";
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
        /// 分页查询用户
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ApiResult GetUsersPage(int pageIndex, int pageSize)
        {
            ApiResult result = new ApiResult() { status = EnumStatus.OK, totalCount = 0 };
            try
            {
                //规范入参值
                pageIndex = pageIndex <= 0 ? 1 : pageIndex;
                pageSize = pageSize <= 0 ? 20 : pageSize;

                //分页查询
                var data = _context.users
                          .OrderBy(m => m.Name)
                          .Skip((pageIndex - 1) * pageSize)
                          .Take(pageSize)
                          .Select(m => new
                          {
                              m.Id,
                              m.Account,
                              m.Name,
                              m.Remarks
                          });

                if (data.Any())
                {
                    result.content = data.ToList();
                    result.totalCount = _context.users.Count();
                    result.msg = "查询成功!";
                }
                else
                {
                    result.msg = "暂无数据!";
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
        /// 按名字查询用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ApiResult GetUsersByName(string name)
        {
            ApiResult result = new ApiResult() { status = EnumStatus.OK, totalCount = 0 };
            try
            {
                var data = _context.users
                           .Where(m => m.Name.Contains(name))
                           .Select(m => new
                           {
                               m.Id,
                               m.Account,
                               m.Name,
                               m.Remarks
                           });

                if (data.Any())
                {
                    result.content = data.ToList();
                    result.totalCount = _context.users.Where(m => m.Name.Contains(name)).Count();
                    result.msg = "查询成功!";
                }
                else
                {
                    result.msg = "暂无数据!";
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
        /// 新增用户
        /// </summary>
        /// <param name="newUsers">待新增用户数组</param>
        /// <returns></returns>
        public ApiResult AddUsers(List<RequestInsertUsers> newUsers)
        {
            ApiResult result = new ApiResult() { status = EnumStatus.OK, totalCount = 0 };
            using (var tran = _context.Database.BeginTransaction())
            {
                try
                {
                    List<users> insertDataList = new List<users>();

                    newUsers.ForEach(m =>
                    {
                        #region 赋值
                        insertDataList.Add(new users
                        {
                            Account = m.Account,
                            Pwd = MD5Helper.EnCode(m.Pwd),
                            Name = m.Name,
                            Sex = m.Sex
                        });
                        #endregion
                    });

                    _context.AddRange(insertDataList);
                    _context.SaveChanges();
                    //throw new Exception("报错了!");
                    tran.Commit();
                    result.msg = "插入成功!";
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    result.status = EnumStatus.InternalServerError;
                    result.msg = e.Message;
                }
            }
            return result;
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="editUsers">待修改用户视图数组</param>
        /// <returns></returns>
        public ApiResult UpdateUsers(List<RequestUpdateUsers> editUsers)
        {
            ApiResult result = new ApiResult() { status = EnumStatus.OK, totalCount = 0 };
            using (var tran = _context.Database.BeginTransaction())
            {
                try
                {
                    var rstList = new List<ResponseViewUsers>();
                    editUsers.ForEach(edit =>
                    {
                        var updUser = _context.users.Where(m => m.Id == edit.Id).FirstOrDefault();//查询出实体
                        if (updUser != null)
                        {
                            updUser.Account = edit.Account;
                            //updUser.Pwd = edit.Pwd;
                            updUser.Name = edit.Name;
                            updUser.Sex = edit.Sex;
                            var ent = _context.Update(updUser).Entity;
                            //返回结果
                            rstList.Add(new ResponseViewUsers
                            {
                                Id = ent.Id,
                                Account = ent.Account,
                                Name = ent.Name,
                                Sex = ent.Sex,
                                Birthday = ent.Birthday,
                                Remarks = ent.Remarks,
                                CompanyId = ent.CompanyId,
                                ProjectId = ent.ProjectId,
                                DeptId = ent.DeptId,
                                PositionId = ent.PositionId,
                            });
                        }
                    });
                    _context.SaveChanges();
                    tran.Commit();
                    result.msg = "修改成功!";
                    result.content = rstList;
                }
                catch (Exception e)
                {
                    result.content = null;
                    tran.Rollback();
                    result.status = EnumStatus.InternalServerError;
                    result.msg = e.Message;
                }
            }
            return result;
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="usersId">待删除用户Id数组</param>
        /// <returns></returns>
        public ApiResult DelUsers(List<int> usersId)
        {
            ApiResult result = new ApiResult() { status = EnumStatus.OK, totalCount = 0 };
            using (var tran = _context.Database.BeginTransaction())
            {
                try
                {
                    var delUsers = _context.users.Where(m => usersId.Contains(m.Id));
                    _context.users.RemoveRange(delUsers);
                    _context.SaveChanges();
                    tran.Commit();
                    result.msg = "删除成功!";
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    result.status = EnumStatus.InternalServerError;
                    result.msg = e.Message;
                }
            }
            return result;
        }

    }
}

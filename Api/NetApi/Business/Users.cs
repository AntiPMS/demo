using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NetApi.Common;
using NetApi.Models;
using NetApi.Models.View;
using Newtonsoft.Json;
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


        /// <summary>
        /// 消息置为已读
        /// </summary>
        /// <param name="db"></param>
        /// <param name="msgIdList">消息主键数组</param>
        /// <param name="socket">目前在线的socket</param>
        /// <returns></returns>
        public static OutPut SetMsgIsRead(NetApiContext db, List<string> msgIdList, IWebsocketManager socket)
        {

            OutPut op = new OutPut()
            {
                ResultStatus = EnumStatus.OK,
                TotalCount = 0
            };

            try
            {
                if (msgIdList == null || msgIdList.Count < 1)
                {
                    op.msg = "主键不能为空!";
                    op.ResultStatus = EnumStatus.NotFound;
                }
                else
                {
                    var msg = db.UserMsg.Where(m => msgIdList.Contains(m.Id) && m.IsRead == 0);
                    if (msg.Any())
                    {
                        msg.ToList().ForEach(m => m.IsRead = 1);
                        db.SaveChanges();
                        op.msg = "修改成功!";
                    }
                    else
                    {
                        op.ResultStatus = EnumStatus.NotFound;
                        op.msg = "记录已修改!";
                    }
                }
            }
            catch (Exception e)
            {
                op.ResultStatus = EnumStatus.InternalServerError;
                op.msg = e.Message;
            }

            return op;
        }

        /// <summary>
        /// 历史消息查询
        /// </summary>
        /// <param name="db"></param>
        /// <param name="lastSendDate">最早消息的日期(精确到秒)</param>
        /// <param name="targetId">取咨询的主键</param>
        /// <param name="num">本次读取的消息数</param>
        /// <returns></returns>
        public static OutPut<List<UserMsg>> GetUserHisChatMsgBySendDateDecreasing(NetApiContext db, DateTime lastSendDate, string targetId, int num)
        {
            OutPut<List<UserMsg>> op = new OutPut<List<UserMsg>>() { ResultStatus = EnumStatus.OK };
            try
            {
                num = num < 1 ? 20 : num;
                var hisMsg = db.UserMsg
                               .Where(m => m.TargetId == targetId && m.SendDate < lastSendDate)
                               .OrderByDescending(m => m.SendDate)
                               .Take(num)
                               .Select(m => new UserMsg
                               {
                                   Id = m.Id,
                                   SenderId = m.SenderId,
                                   SenderName = m.SenderName,
                                   TargetId = m.TargetId,
                                   MsgType = m.MsgType,
                                   Msg = m.Msg,
                                   SendDate = m.SendDate,
                                   IsRead = m.IsRead,
                               })
                               .OrderBy(m => m.SendDate);
                if (hisMsg.Any())
                {
                    op.msg = "查询成功!";
                    op.ResultData = hisMsg.ToList();
                    op.TotalCount = op.ResultData.Count();
                }
                else
                {
                    op.msg = "没有更多了...";
                    op.ResultStatus = EnumStatus.NotFound;
                }
            }
            catch (Exception e)
            {
                op.msg = e.Message;
                op.ResultStatus = EnumStatus.InternalServerError;
            }
            return op;
        }

        /// <summary>
        /// 发送系统消息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="socket"></param>
        /// <param name="info">消息内容</param>
        /// <returns></returns>
        public static OutPut SendMsg2User(NetApiContext db, IWebsocketManager socket, MsgUserInfo info)
        {
            OutPut op = new OutPut() { TotalCount = 0 };

            try
            {
                bool isOK;
                socket.SendMsg(new WebSocketClientModel
                {
                    TargetId = info.userId,
                    MsgType = MsgType.System,
                    Msg = JsonConvert.SerializeObject(info.msg),
                    SendDate = DateTime.Now,
                    SenderId = "-1",
                    SenderName = "System"
                }, out isOK);

                if (isOK)
                {
                    op.ResultStatus = EnumStatus.OK;
                    op.msg = "发送成功";
                }
                else
                {
                    op.ResultStatus = EnumStatus.InternalServerError;
                    op.msg = "发送失败";
                }
            }
            catch (Exception e)
            {
                op.ResultStatus = EnumStatus.InternalServerError;
                op.msg = e.Message;
            }
            return op;
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="targetId">接收人Id</param>
        /// <param name="msgType">消息类型</param>
        /// <returns></returns>
        public static OutPut<List<userMsg>> GetUserMsg(NetApiContext db, string targetId, MsgType msgType)
        {
            OutPut<List<userMsg>> op = new OutPut<List<userMsg>>() { ResultStatus = EnumStatus.OK, TotalCount = 0 };

            try
            {
                var _msgType = (sbyte)msgType;
                var data = db.UserMsg
                             .Where(m => m.TargetId == targetId && m.MsgType == _msgType)
                             .Select(m => new userMsg
                             {
                                 Id = m.Id,
                                 SenderId = m.SenderId,
                                 TargetId = m.TargetId,
                                 MsgType = m.MsgType,
                                 msg = m.Msg,
                                 SendDate = m.SendDate,
                                 IsRead = m.IsRead,
                             });
                if (data.Any())
                {
                    op.ResultData = data.ToList();
                    op.TotalCount = op.ResultData.Count;
                    op.msg = "查询成功!";
                }
                else
                {
                    op.ResultStatus = EnumStatus.NotFound;
                    op.msg = "暂无数据!";
                }
            }
            catch (Exception e)
            {
                op.ResultStatus = EnumStatus.InternalServerError;
                op.msg = e.Message;
            }
            return op;
        }

        /// <summary>
        /// 消息类型枚举
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static OutPut<Dictionary<string, int>> GetMsgTypeEnum(IWebsocketManager socket)
        {
            OutPut<Dictionary<string, int>> op = new OutPut<Dictionary<string, int>>() { TotalCount = 0 };

            try
            {
                op.ResultStatus = EnumStatus.OK;
                op.ResultData = socket.GetMsgTypeEnum();
                op.TotalCount = op.ResultData.Count;
                op.msg = "查询成功!";
            }
            catch (Exception e)
            {
                op.ResultStatus = EnumStatus.InternalServerError;
                op.msg = e.Message;
            }
            return op;
        }

    }
}

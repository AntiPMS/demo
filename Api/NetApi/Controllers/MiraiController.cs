using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetApi.Common;
using NetApi.Models.View;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MiraiController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IDatabase mirai;

        public MiraiController(
            IConfiguration _configuration
            , RedisHelper redis)
        {
            configuration = _configuration;
            mirai = redis.GetDatabase();
        }

        /// <summary>
        /// 查询全部wiki词条
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public OutPut<List<string>> GetMiraiWikiAll()
        {
            OutPut<List<string>> op = new OutPut<List<string>>() { ResultData = new List<string>() };
            //redis模糊查询， redis-cli:keys *{question}*
            var redisResult = mirai.ScriptEvaluate(LuaScript.Prepare(
                            //Redis的keys模糊查询：
                            " local res = redis.call('KEYS', @keypattern) return res "), new { @keypattern = "*" });
            if (!redisResult.IsNull)
            {
                foreach (var dic in (string[])redisResult)
                {
                    if (mirai.KeyType(dic).Equals(RedisType.String))
                    {
                        op.ResultData.Add($"{dic}:{mirai.StringGet(dic)}");
                    }
                    else if (mirai.KeyType(dic).Equals(RedisType.Hash))
                    {
                        RedisValue[] kv = mirai.HashValues(dic);
                        foreach (var item in kv)
                        {
                            var answer = JsonConvert.DeserializeObject<MsgModel>(item);
                            op.ResultData.Add($"{dic}:{answer.content}");
                        }
                    }

                }
            }
            return op;
        }
    }
}

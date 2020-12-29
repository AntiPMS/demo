﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetApi.Common
{
    /// <summary>
    /// 通用Api返回模型 
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 返回状态 https://httpstatuses.com/
        /// </summary>
        public EnumStatus status { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 返回内容 
        /// </summary>
        public object content { get; set; }

        /// <summary>
        /// 返回总数目
        /// </summary>
        public int totalCount { get; set; }
    }

    /// <summary>
    /// 状态枚举类
    /// </summary>
    public enum EnumStatus
    {
        OK = 200,
        Created = 201,
        Accepted = 202,
        NonAuthoritativeInformation = 203,
        NoContent = 204,
        //ResetContent = 205,
        //PartialContent = 206,
        //MultiStatus = 207,
        //AlreadyReported = 208,
        //IMUsed = 226,

        //MultipleChoices = 300,
        //MovedPermanently = 301,
        //Found = 302,
        //SeeOther = 303,
        //NotModified = 304,
        //UseProxy = 305,
        //TemporaryRedirect = 307,
        //PermanentRedirect = 308,

        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        //ProxyAuthenticationRequired = 407,
        //RequestTimeout = 408,
        //Conflict = 409,
        //Gone = 410,
        //LengthRequired = 411,
        //PreconditionFailed = 412,
        //PayloadTooLarge = 413,
        //RequestURItooLong = 414,
        //UnsupportedMediaType = 415,
        //RequestedRangeNotSatisfiable = 416,
        //ExpectationFailed = 417,
        //Imateapot = 418,
        //MisdirectedRequest = 421,
        //UnprocessableEntity = 422,
        //Locked = 423,
        //FailedDependency = 424,
        //UpgradeRequired = 426,
        //PreconditionRequired = 428,
        //TooManyRequests = 429,
        //RequestHeaderFieldsTooLarge = 431,
        //ConnectionClosedWithoutResponse = 444,
        //UnavailableForLegalReasons = 451,
        //ClientClosedRequest = 499,

        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        //HTTPVersionNotSupported = 505,
        //VariantAlsoNegotiates = 506,
        //InsufficientStorage = 507,
        //LoopDetected = 508,
        //NotExtended = 510,
        //NetworkAuthenticationRequired = 511,
        //NetworkConnectTimeoutError = 599,
    }
}

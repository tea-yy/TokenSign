using Server.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Server.Models
{
    public enum StatusCodeEnum
    {
        [Text("请求(或处理)成功")]
        Success = 200, //请求(或处理)成功

        [Text("内部请求出错")]
        Error = 500, //内部请求出错

        [Text("未授权标识")]
        Unauthorized = 401,//未授权标识

        [Text("请求参数不完整或不正确")]
        ParameterError = 400,//请求参数不完整或不正确

        [Text("请求TOKEN失效")]
        TokenInvalid = 403,//请求TOKEN失效

        [Text("HTTP请求类型不合法")]
        HttpMehtodError = 405,//HTTP请求类型不合法

        [Text("HTTP请求不合法,请求参数可能被篡改")]
        HttpRequestError = 406,//HTTP请求不合法

        [Text("该URL已经失效")]
        URLExpireError = 407,//HTTP请求不合法
    }
}
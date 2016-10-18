using Client.Common;
using Newtonsoft.Json;
using Server.Common;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Filters;

namespace Server.Filters
{
    public class ApiSecurityFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            ResultMsg resultMsg = null;
            var request = actionContext.Request;
            string method = request.Method.Method;
            string staffid = String.Empty, timestamp = string.Empty, nonce = string.Empty, signature = string.Empty;
            int id = 0;

            if (request.Headers.Contains("staffid"))
            {
                staffid = HttpUtility.UrlDecode(request.Headers.GetValues("staffid").FirstOrDefault());
            }
            if (request.Headers.Contains("timestamp"))
            {
                timestamp = HttpUtility.UrlDecode(request.Headers.GetValues("timestamp").FirstOrDefault());
            }
            if (request.Headers.Contains("nonce"))
            {
                nonce = HttpUtility.UrlDecode(request.Headers.GetValues("nonce").FirstOrDefault());
            }

            if (request.Headers.Contains("signature"))
            {
                signature = HttpUtility.UrlDecode(request.Headers.GetValues("signature").FirstOrDefault());
            }

            //GetToken方法不需要进行签名验证
            if (actionContext.ActionDescriptor.ActionName == "GetToken")
            {
                if (string.IsNullOrEmpty(staffid) || (!int.TryParse(staffid, out id) || string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(nonce)))
                {
                    resultMsg = new ResultMsg();
                    resultMsg.StatusCode = (int)StatusCodeEnum.ParameterError;
                    resultMsg.Info = StatusCodeEnum.ParameterError.GetEnumText();
                    resultMsg.Data = "";
                    actionContext.Response = HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));
                    base.OnActionExecuting(actionContext);
                    return;
                }
                else
                {
                    base.OnActionExecuting(actionContext);
                    return;
                }
            }


            //判断请求头是否包含以下参数
            if (string.IsNullOrEmpty(staffid) || (!int.TryParse(staffid, out id) || string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(nonce) || string.IsNullOrEmpty(signature)))
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.ParameterError;
                resultMsg.Info = StatusCodeEnum.ParameterError.GetEnumText();
                resultMsg.Data = "";
                actionContext.Response = HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));
                base.OnActionExecuting(actionContext);
                return;
            }

            //判断timespan是否有效
            double ts1 = 0;
            double ts2 = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
            bool timespanvalidate = double.TryParse(timestamp, out ts1);
            double ts = ts2 - ts1;
            bool falg = ts > int.Parse(WebSettingsConfig.UrlExpireTime) * 1000;
            if (falg || (!timespanvalidate))
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.URLExpireError;
                resultMsg.Info = StatusCodeEnum.URLExpireError.GetEnumText();
                resultMsg.Data = "";
                actionContext.Response = HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));
                base.OnActionExecuting(actionContext);
                return;
            }


            //判断token是否有效
            Token token = (Token)HttpRuntime.Cache.Get(id.ToString());
            string signtoken = string.Empty;
            if (HttpRuntime.Cache.Get(id.ToString()) == null)
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.TokenInvalid;
                resultMsg.Info = StatusCodeEnum.TokenInvalid.GetEnumText();
                resultMsg.Data = "";
                actionContext.Response = HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));
                base.OnActionExecuting(actionContext);
                return;
            }
            else
            {
                signtoken = token.SignToken.ToString();
            }

            //根据请求类型拼接参数
            NameValueCollection form = HttpContext.Current.Request.QueryString;
            string data = string.Empty;
            switch (method)
            {
                case "POST":
                    Stream stream = HttpContext.Current.Request.InputStream;
                    string responseJson = string.Empty;
                    StreamReader streamReader = new StreamReader(stream);
                    data = streamReader.ReadToEnd();
                    break;
                case "GET":
                    //第一步：取出所有get参数
                    IDictionary<string, string> parameters = new Dictionary<string, string>();
                    for (int f = 0; f < form.Count; f++)
                    {
                        string key = form.Keys[f];
                        parameters.Add(key, form[key]);
                    }

                    // 第二步：把字典按Key的字母顺序排序
                    IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
                    IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

                    // 第三步：把所有参数名和参数值串在一起
                    StringBuilder query = new StringBuilder();
                    while (dem.MoveNext())
                    {
                        string key = dem.Current.Key;
                        string value = dem.Current.Value;
                        if (!string.IsNullOrEmpty(key))
                        {
                            query.Append(key).Append(value);
                        }
                    }
                    data = query.ToString();
                    break;
                default:
                    resultMsg = new ResultMsg();
                    resultMsg.StatusCode = (int)StatusCodeEnum.HttpMehtodError;
                    resultMsg.Info = StatusCodeEnum.HttpMehtodError.GetEnumText();
                    resultMsg.Data = "";
                    actionContext.Response = HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));
                    base.OnActionExecuting(actionContext);
                    return;
            }
            
            bool result = SignExtension.Validate(timestamp, nonce, id, signtoken,data, signature);
            if (!result)
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.HttpRequestError;
                resultMsg.Info = StatusCodeEnum.HttpRequestError.GetEnumText();
                resultMsg.Data = "";
                actionContext.Response = HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));
                base.OnActionExecuting(actionContext);
                return;
            }
            else
            {
                base.OnActionExecuting(actionContext);
            }
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }

    }
}
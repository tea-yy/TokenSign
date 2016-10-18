using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Server.Common
{
    public class SignExtension
    {
        public static bool Validate(string timeStamp, string nonce,int staffId, string token,string data,string signature)
        {
            var hash = System.Security.Cryptography.MD5.Create();
            //拼接签名数据
            var signStr = timeStamp + nonce + staffId + token + data;
            //将字符串中字符按升序排序
            var sortStr = string.Concat(signStr.OrderBy(c => c));
            var bytes = Encoding.UTF8.GetBytes(sortStr);
            //使用MD5加密
            var md5Val = hash.ComputeHash(bytes);
            //把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            foreach (var c in md5Val)
            {
                result.Append(c.ToString("X2"));
            }

            return result.ToString().ToUpper() == signature;
        }
    }
}
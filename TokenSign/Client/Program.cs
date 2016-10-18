using Client.Common;
using Client.Entity;
using Newtonsoft.Json;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            int staffId=int.Parse(AppSettingsConfig.StaffId);
            var tokenResult = WebApiHelper.GetSignToken(staffId);
            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("id", "1");
            parames.Add("name", "wahaha");
            Tuple<string, string> parameters = WebApiHelper.GetQueryString(parames);
            var product1 = WebApiHelper.Get<ProductResultMsg>("http://localhost:14826/api/product/getproduct", parameters.Item1, parameters.Item2, staffId);
            Product product = new Product() { Id = 1, Name = "安慕希", Count = 10, Price = 58.8 };
            var product2 = WebApiHelper.Post<ProductResultMsg>("http://localhost:14826/api/product/addProduct",JsonConvert.SerializeObject(product), staffId);
            Console.Read();
        }
    }
}

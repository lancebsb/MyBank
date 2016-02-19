using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MyBank.Common
{
    public class Json
    {
        /// <summary>
        /// 业务操作返回结果信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string ResultMes(string code, string message)
        {
            StringBuilder fruit = new StringBuilder();
            fruit.Append("{\"Code\":\"" + code + "\",\"Message\":\"" + message + "\"}");
            return fruit.ToString();
        }
    }
}
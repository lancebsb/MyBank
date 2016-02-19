using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyBank.Model;
using System.Web.Caching;
namespace MyBank.Common
{
    public class Public
    {
        private MyBank.Model.DataBaseDataContext db = new DataBaseDataContext(SqlHelper.connString);
        public List<FundDic> GetFundDic(int typeID)
        {
            Cache dicCache = System.Web.HttpRuntime.Cache;
            const int cacheTime=5;
            string cacheKey="fund_"+typeID.ToString();//缓存名称
            List<FundDic> dicList = null;
            object obj = dicCache.Get(cacheKey);//得到缓存
            if (obj == null)
            {
                dicList = db.FundDic.Where(e => e.Type == typeID).ToList();
                dicCache.Insert(cacheKey, dicList, null, DateTime.Now.AddMinutes(cacheTime), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null
                    );
            }
            else
            {
                dicList = (List<FundDic>) obj;
            }
            
            return dicList;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MyBank.Model;
using System.Data.Linq.SqlClient;
using MyBank.Models;
using MyBank.Common; 

namespace MyBank.Controllers
{
    public class MyBankController : Controller
    {
        private MyBank.Model.DataBaseDataContext db = new DataBaseDataContext(SqlHelper.connString);
        private string info = string.Empty;
        private string code = string.Empty;
        //
        // GET: /MyBank/
        #region Get
        public ActionResult Index()
        {
            return View();
        }
        //添加页
        public ActionResult Add()
        {
            var dicList = GetFundDic(2);
            ViewData["FundDic"] = new SelectList(dicList,"ID","Item","");
            return View();
        }

        //编辑页
        public ActionResult Edit()
        {
            string statementID = Request.QueryString["statementID"] ?? "0";
            Statement model = db.Statement.Where(e => e.ID == int.Parse(statementID)).FirstOrDefault();
                if (model == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    var dicList = GetFundDic(int.Parse(model.Type.ToString()));
                    ViewData["FundDic"] = new SelectList(dicList, "ID", "Item", model.ItemID);
                    List<Object> typeList = new List<object>();
                    return View(model);
                }
        }
        //按类别统计
        public ActionResult Category()
        {
            return View();
        }
        //按月统计
        public ActionResult Month()
        {
            return View();
        }
        //按年统计
        public ActionResult Year()
        {
            return View();
        }
        #endregion

        #region Post
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStatement()
        {

            string result = string.Empty;
            int pageIndex = int.Parse(Request["page"]);// 当前页
            int pageSize = int.Parse(Request["rows"]);// 最大行数

            // 获取查询条件
            string typeID = Request["typeID"] ?? "0";//收入 支出
            string itemID = Request["itemID"] ?? "0";
            string userName = Request["userName"] ?? string.Empty;
            string beginTime = Request["beginTime"] ?? string.Empty;
            string endTime=Request["endTime"]??string.Empty;

            var dataList = from c in db.Statement
                           join dic in db.FundDic on c.ItemID equals dic.ID into temp
                           from tt in temp.DefaultIfEmpty()
                           join user in db.User on c.OperatorID equals user.ID into temp2
                           from t2 in temp2.DefaultIfEmpty()
                           orderby c.ID
                           select new
                           {
                               ID = c.ID,
                               Money = c.Money,
                               OccurTime = c.OccurTime,
                               CreateTime = c.CreateTime,
                               Type = c.Type,
                               Remark = c.Remark,
                               ItemID=tt.ID,
                               ItemName = tt.Item,
                               Operator = t2.NickName
                           };
            if (typeID!="0") //类型
            {
                dataList= dataList.Where(e => e.Type == int.Parse(typeID));
            }
            if (itemID!="0") //费用产生源
            {
                dataList = dataList.Where(e => e.ItemID == int.Parse(itemID));
            }
            if (!string.IsNullOrEmpty(userName)) //姓名
            {
                dataList = dataList.Where(e => e.Operator.Contains(userName));
            }
            if (beginTime != "" && endTime != "")
            {
                dataList = dataList.Where(e=>(e.OccurTime>=Convert.ToDateTime(beginTime)&&e.OccurTime<=Convert.ToDateTime(endTime)));
            }
            int total = dataList.Count();//总数
            var items = dataList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();//数据列表
            IList<Models.StatementInfo> statementInfo = new List<Models.StatementInfo>();
            foreach (var item in items)
            {
                Models.StatementInfo info = new Models.StatementInfo();
                info.ID = item.ID;
                info.ItemName = item.ItemName;
                info.Money = decimal.Parse(item.Money.ToString()).ToString("C");
                info.OccurTime = item.OccurTime;
                info.Operator = item.Operator;
                info.Remark = item.Remark;
                info.Type = item.Type;
                info.CreateTime = item.CreateTime;
                statementInfo.Add(info);
            }

            var income = dataList.Where(e=>e.Type==1).Sum(m=>m.Money);  //收入
            var expend = dataList.Where(e => e.Type == 2).Sum(m=>m.Money);  //支出
            string[] totalMoney={income.ToString(),expend.ToString()};
            Dictionary<int, decimal?> dicMoney = new Dictionary<int, decimal?>();
            dicMoney.Add(1, income == null ? 0 : income);
            dicMoney.Add(2,expend==null?0:expend);
             List<TotalMoney> moneyList = new List<TotalMoney>();
            
            foreach(var dic in dicMoney)
            {
                TotalMoney moneyModel = new TotalMoney();
                moneyModel.Money = dic.Value;
                moneyModel.Type=dic.Key;
                moneyList.Add(moneyModel);
            }

            // 绑定数据
            //object dataCount = dsUser.Tables[1].Rows[0][0];
            var jsonData = new
            {
                total = total,// 总行数
                rows = statementInfo,
                footer = moneyList
            };
            result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);

            return Content(result);
        }
        //保存数据
        [HttpPost]
        public ActionResult Add(FormCollection form)
        {
            
            Statement model = new Statement();
            try
            {
                model.ItemID =int.Parse(form.Get("ItemID"));
                model.Type = int.Parse(form.Get("Type"));
                model.Money = decimal.Parse(form.Get("Money"));
                model.OccurTime = Convert.ToDateTime(form.Get("OccurTime"));
                model.CreateTime = DateTime.Now;
                model.Remark = form.Get("Remark");
                model.OperatorID = 1; //操作人

                db.Statement.InsertOnSubmit(model);
                db.SubmitChanges();
                code = "ok";
                info = "保存成功！";
            }
            catch (Exception ex)
            { 
                code="Error";
                info="保存失败！";
            }
            return Content(Common.Json.ResultMes(code,info));
        }

        // 编辑 
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            string id = form.Get("ID");
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    Statement stateModel = db.Statement.Where(e => e.ID == int.Parse(id)).FirstOrDefault();
                    stateModel.ItemID = int.Parse(form.Get("ItemID"));
                    stateModel.Money = decimal.Parse(form.Get("Money"));
                    stateModel.Type =int.Parse(form.Get("Type"));
                    stateModel.OccurTime = Convert.ToDateTime(form.Get("OccurTime"));
                    stateModel.Remark = form.Get("Remark");
                    db.SubmitChanges();
                    code = "ok";
                    info = "保存成功！";
                }
                catch (Exception ex)
                {
                    code = "Error";
                    info = "修改失败！";
                }
                return Content(Common.Json.ResultMes(code, info));
            }
            else
            {
                code = "Error";
                info = "数据有问题！";
                return Content(Common.Json.ResultMes(code, info));
            }
        }
        /// <summary>
        /// 得到费用支出/收入源
        /// </summary>
        /// <returns></returns>
        public ActionResult GetItem()
        {
            string jsonData = string.Empty;
            string typeID = Request["typeID"] ?? string.Empty;
            if (!string.IsNullOrEmpty(typeID))
            {
                int id=int.Parse(typeID);
                var fundDic = GetFundDic(id);//得到字典数据
                jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(fundDic);
            }
            return Content(jsonData);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public ActionResult DelStatement()
        {
            string info = string.Empty;
            string code = string.Empty;
            string id = Request["statementID"] ?? string.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                Statement model = (from sm in db.Statement
                                   where sm.ID == int.Parse(id)
                                   select sm).FirstOrDefault();
                try
                {
                    db.Statement.DeleteOnSubmit(model);
                    db.SubmitChanges();
                    code = "ok";
                    info = "删除成功！";
                }
                catch (Exception ex)
                {
                    code = "Error";
                    info = "删除失败"+ex.ToString();
                }         
            }
            return Content(Common.Json.ResultMes(code, info));
        }
        /// <summary>
        /// 得到当月的数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCurrentMonthData()
        {
            DateTime now = DateTime.Now;
            var dataList = from dic in db.FundDic
                           join s in db.Statement.Where(a=> SqlMethods.DateDiffMonth(a.OccurTime, now) == 0) on dic.ID equals s.ItemID into temp 
                           from tt in temp.DefaultIfEmpty()
                           where  dic.Type == 2
                           group tt by dic.Item into g
                           select new
                           {
                               ItemName = g.Key,
                               Money = g.Sum(tt => tt.Money) ==null?0:g.Sum(tt => tt.Money)
                           };
            var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(dataList);
            return Content(jsonData);
                        
        }
        //每月支出
        public ActionResult GetExpenditureByMonth()
        {
            string year = Request["year"] ?? DateTime.Now.Year.ToString();
            //&& t.OccurTime.Value.Year.ToString() == year
            var dataList = (from t in db.Statement
                           where t.Type == 2 
                           group t by new { t.OccurTime.Value.Year,t.OccurTime.Value.Month} into g
                           select new Expenditure
                           {
                               Year=g.Key.Year,
                               Month=g.Key.Month,
                               Money=g.Sum(t=>t.Money)
                           }).ToList();
         
            //Dictionary<int, List<Expenditure>> dic = new Dictionary<int, List<Expenditure>>();
            IEnumerable<IGrouping<int, Expenditure>> groupList = dataList.GroupBy(e => e.Year);//按年份分组
            for(int i=0;i<groupList.Count();i++)
            {
                 //List<Expenditure> exList = groupList.ToList<int,Expenditure>();//分组后的集合
            }
            string strJson = string.Empty;
            foreach (IGrouping<int, Expenditure> info in groupList)
            {
                List<Expenditure> exList = info.ToList<Expenditure>();//分组后的集合
                
                for (int i = 1; i <= 12; i++)
                {
                    if (exList.Where(e => e.Month == i).FirstOrDefault() == null)
                    {
                        Expenditure model = new Expenditure();
                        model.Year = info.Key;
                        model.Money = 0;
                        model.Month = i;
                        exList.Add(model);
                    }
                }
                var newJson = new
                {
                    Year = info.Key,
                    Data = exList.OrderBy(e=>e.Month)
                };
                
                strJson += Newtonsoft.Json.JsonConvert.SerializeObject(newJson)+",";
                //dic.Add(info.Key, exList);
            }
            strJson = "["+strJson.TrimEnd(',')+"]";

            var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(groupList);
            return Content(strJson);
        }
        //每年支出
        public ActionResult GetExpenditureByYear()
        {
            var dataList = (from t in db.Statement
                            where t.Type == 2
                            group t by t.OccurTime.Value.Year into g
                            select new 
                            {
                                Year = g.Key,
                                Money = g.Sum(t => t.Money)
                            });
            var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(dataList);
            return Content(jsonData);
        }
        #endregion

        #region 内置
        // 获得字典数据
        public List<FundDic> GetFundDic(int typeID)
        {
            MyBank.Common.Public pubBll = new Common.Public();
            var dicList = pubBll.GetFundDic(typeID);
            return dicList;
        }

        public class TotalMoney
        {
            public decimal? Money { get; set; }
            public int Type { get; set; }
        }
        private class Expenditure
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal? Money { get; set; }
        }
        #endregion

        /// <summary>
        /// 获取数据图表的数据源-直接是个List<TMode>的集合队列
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDataChartSource()
        {
            int queryId = Request.Form["filterValue"] == null ? 0 : Convert.ToInt32(Request.Form["filterValue"]);
            int dimensionType = Request.Form["dimensionType"] == null ? 0 : Convert.ToInt32(Request.Form["dimensionType"]);
            MyBank.Common.PublicEnum.TimeTypeEnum type = (MyBank.Common.PublicEnum.TimeTypeEnum)Enum.Parse(typeof(MyBank.Common.PublicEnum.TimeTypeEnum), dimensionType.ToString());

            IList<ReportDataModel> reportDataLs = new List<ReportDataModel>() { 
                new ReportDataModel(){ ReportId=1, ReportName="David测试订单", ReportDate=new DateTime(2013,4,10), Impressions=10000, Clicks=5513, UV=45241},
                new ReportDataModel(){ ReportId=1, ReportName="David测试订单", ReportDate=new DateTime(2013,4,11), Impressions=23121, Clicks=5111, UV=53532},
                new ReportDataModel(){ ReportId=1, ReportName="David测试订单", ReportDate=new DateTime(2013,4,12), Impressions=76511, Clicks=4522, UV=34234},
                new ReportDataModel(){ ReportId=1, ReportName="David测试订单", ReportDate=new DateTime(2013,4,13), Impressions=96511, Clicks=7362, UV=41133},
                new ReportDataModel(){ ReportId=1, ReportName="David测试订单", ReportDate=new DateTime(2013,4,14), Impressions=42231, Clicks=4224, UV=42612},
                new ReportDataModel(){ ReportId=1, ReportName="David测试订单", ReportDate=new DateTime(2013,4,15), Impressions=34244, Clicks=6242, UV=51311},
                new ReportDataModel(){ ReportId=1, ReportName="David测试订单", ReportDate=new DateTime(2013,4,16), Impressions=86511, Clicks=3424, UV=84322},
                new ReportDataModel(){ ReportId=1, ReportName="David测试订单", ReportDate=new DateTime(2013,4,17), Impressions=31311, Clicks=6234, UV=77342},
                new ReportDataModel(){ ReportId=1, ReportName="David测试订单", ReportDate=new DateTime(2013,4,18), Impressions=23131, Clicks=7242, UV=61111},
                new ReportDataModel(){ ReportId=2, ReportName="David测试订单2", ReportDate=new DateTime(2013,4,19), Impressions=41311, Clicks=3244, UV=72421},
                new ReportDataModel(){ ReportId=2, ReportName="David测试订单2", ReportDate=new DateTime(2013,4,10), Impressions=10000, Clicks=5513, UV=45241},
                new ReportDataModel(){ ReportId=2, ReportName="David测试订单2", ReportDate=new DateTime(2013,4,11), Impressions=23121, Clicks=5111, UV=53532},
                new ReportDataModel(){ ReportId=2, ReportName="David测试订单2", ReportDate=new DateTime(2013,4,12), Impressions=34232, Clicks=4522, UV=34234},
                new ReportDataModel(){ ReportId=2, ReportName="David测试订单2", ReportDate=new DateTime(2013,4,13), Impressions=96511, Clicks=7362, UV=41133},
                new ReportDataModel(){ ReportId=2, ReportName="David测试订单2", ReportDate=new DateTime(2013,4,14), Impressions=96511, Clicks=4224, UV=42612},
                new ReportDataModel(){ ReportId=2, ReportName="David测试订单2", ReportDate=new DateTime(2013,4,15), Impressions=34244, Clicks=6242, UV=51311},
                new ReportDataModel(){ ReportId=2, ReportName="David测试订单2", ReportDate=new DateTime(2013,4,16), Impressions=96511, Clicks=3424, UV=84322},
                new ReportDataModel(){ ReportId=2, ReportName="David测试订单2", ReportDate=new DateTime(2013,4,17), Impressions=31311, Clicks=6234, UV=77342},
                new ReportDataModel(){ ReportId=2, ReportName="David测试订单2", ReportDate=new DateTime(2013,4,18), Impressions=96511, Clicks=7242, UV=61111},
                new ReportDataModel(){ ReportId=2, ReportName="David测试订单2", ReportDate=new DateTime(2013,4,19), Impressions=41311, Clicks=3244, UV=72421},
            };

            if (queryId > 0)
                reportDataLs = reportDataLs.Where(n => n.ReportId == queryId).ToList<ReportDataModel>();

            switch (type)
            {
                case MyBank.Common.PublicEnum.TimeTypeEnum.Week:
                    {
                        var result = from item in reportDataLs
                                     group item by item.ReportWeek into reportDataWeek
                                     select new
                                     {
                                         ReportWeekStr = reportDataWeek.Key,
                                         Impressions = reportDataWeek.Sum(n => n.Impressions),
                                         Clicks = reportDataWeek.Sum(n => n.Clicks),
                                         CTR = reportDataWeek.Sum(n => n.Clicks) == 0 ? 0 : Math.Round((double)reportDataWeek.Sum(n => n.Clicks) / reportDataWeek.Sum(n => n.Impressions), 6),
                                         UV = reportDataWeek.Max(n => n.UV)
                                     };
                        return Json(new { chartSource = result }, JsonRequestBehavior.AllowGet);
                    };
                case MyBank.Common.PublicEnum.TimeTypeEnum.Month:
                    {
                        var result = from item in reportDataLs
                                     group item by item.ReportYear into reportDataYear
                                     select new
                                     {
                                         ReportMonthStr = reportDataYear.Key,
                                         Impressions = reportDataYear.Sum(n => n.Impressions),
                                         Clicks = reportDataYear.Sum(n => n.Clicks),
                                         CTR = reportDataYear.Sum(n => n.Clicks) == 0 ? 0 : Math.Round((double)reportDataYear.Sum(n => n.Clicks) / reportDataYear.Sum(n => n.Impressions), 6),
                                         UV = reportDataYear.Max(n => n.UV)
                                     };
                        return Json(new { chartSource = result }, JsonRequestBehavior.AllowGet);
                    };
                default:
                    {
                        var result = reportDataLs;
                        return Json(new { chartSource = reportDataLs }, JsonRequestBehavior.AllowGet);
                    }
            }
        }

   
        public ActionResult test()
        {
            return View();
        }
    }

}
      

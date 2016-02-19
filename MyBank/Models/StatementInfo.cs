using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MyBank.Models
{
    [Serializable()]
    public class StatementInfo
    {
        public int ID { get; set; }
        public int? Type { get; set; }
        public int? ItemID { get; set; }
        public string Money { get; set; }
        public DateTime? OccurTime { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Remark { get; set; }
        public int? OperatorID { get; set; }
        public string Operator { get; set; }
        public string ItemName { get; set; }
    }
     [Serializable()]
    public class StatementData
    {
        public int total { get; set; }
        public IList<StatementInfo> rows { get; set; }
    }
}
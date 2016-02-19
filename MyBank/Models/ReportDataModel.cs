using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyBank.Models
{
    public class ReportDataModel
    {

            public int ReportId { get; set; }
            public string ReportName { get; set; }
            public DateTime ReportDate { get; set; }
            public int Impressions { get; set; }
            public int Clicks { get; set; }
            public int UV { get; set; }
            public string ReportWeek { get; set; }
            public string ReportYear { get; set; }

    }
}
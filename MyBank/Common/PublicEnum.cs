using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyBank.Common
{
    public class PublicEnum
    {
        public enum TimeTypeEnum
        {
            Day=1,
            Week=2,
            Month=3
        }
        public enum HighchartTypeEnum
        {
            混合型=1,
            饼图型=2,
            柱状图=3,
            多柱状图=4,
            多流线图=5,
            多横柱图=6,
            层叠图=7,
            区域图=8,
            温度计型=9
        }
    }
}
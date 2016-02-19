using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MyBank.Model
{
    public sealed class SqlHelper
    {
        public static readonly string connString = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
    }
}

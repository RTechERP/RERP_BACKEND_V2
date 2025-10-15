using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Common
{
    public static class Config
    {
        private static int _isPublish = 0;
        public static string Path()
        {
            string path = @"D:\\LeTheAnh\\RTC";
            return path;
        }
        public static string ConnectionString { get; set; } = @"Server=DESKTOP-GQKB5KK\SQLEXPRESS;database=RTC_Test;User Id = sa; Password=1;TrustServerCertificate=True";

        //public static string ConnectionString
        //{
        //    get
        //    {
        //        string connectionString = @"Server=DESKTOP-GQKB5KK\SQLEXPRESS;database=RTC_Test;User Id = sa; Password=1;TrustServerCertificate=True";
        //        if (_isPublish == 1) connectionString = @"";
        //        return connectionString;
        //    }
        //}
    }
}

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
            string path = @"C:\RTC\UPLOADFILE";
            return path;
        }

        public static string ConnectionString
        {
            get
            {
                string connectionString = @"Server=MSI;database=RTC;User Id = sa; Password=1;TrustServerCertificate=True";
                if (_isPublish == 1) connectionString = @"";
                return connectionString;
            }
        }


    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Common
{
    public static class Config
    {
        private static int _isPublish = 0;
        public static string ConnectionString
        {
            get
            {
                string connectionString = @"";
                if (_isPublish == 1) connectionString = @"";
                return connectionString;
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class EmployeeOverTimeByMonthParam
    {
        public int month { get; set; }
        public int year { get; set; }
        public int employeeId { get; set; }
        public int departmentId { get; set; }
        public string? keyWord { get; set; }
    }
}

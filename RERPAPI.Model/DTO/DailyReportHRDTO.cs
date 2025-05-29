using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using RERPAPI.Model.Entities;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class DailyReportHRDTO:DailyReportHR
    {
        public decimal PerformanceAVG { get; set; }
        public string WorkContent { get; set; }
        public string FilmName { get; set; }
        public string UnitName { get; set; }
        public string FullName { get; set; }
        public int ChucVuHDID { get; set; }
    }
}

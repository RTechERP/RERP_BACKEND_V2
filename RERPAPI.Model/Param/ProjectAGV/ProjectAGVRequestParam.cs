using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.ProjectAGV
{
    public class ProjectAGVRequestParam
    {
        public int Size { get; set; }
        public int Page { get; set; }

        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string? ProjectType { get; set; }
        public int? PmID { get; set; }
        public int? LeaderID { get; set; }
        public int? BussinessFieldID { get; set; }
        public string? ProjectStatus { get; set; }
        public int? CustomerID { get; set; }
        public int? SaleID { set; get; }
        public int? UserTechID { get; set; }
        public int? GlobalUserID { set; get; }
        public bool? IsAGV { set; get; }
    }
}

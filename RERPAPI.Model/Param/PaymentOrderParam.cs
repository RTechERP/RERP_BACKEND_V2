using RERPAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class PaymentOrderParam
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TypeOrder { get; set; }
        public int PaymentOrderTypeID { get; set; }
        public DateTime? DateStart { get; set; } = TextUtils.MinDate;
        public DateTime? DateEnd { get; set; } = TextUtils.MaxDate;
        public int DepartmentID { get; set; }
        public int EmployeeID { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public int IsIgnoreHR { get; set; } = -1;
        public int IsApproved { get; set; } = -1;
        public int IsSpecialOrder { get; set; }
        public int ApprovedTBPID { get; set; }
        public int Step { get; set; }
        public bool IsShowTable { get; set; } = true;
        public int Statuslog { get; set; }
        public int IsDelete { get; set; }
    }
}

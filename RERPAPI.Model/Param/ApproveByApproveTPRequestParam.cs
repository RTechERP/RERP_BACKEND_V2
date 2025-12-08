using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class ApproveByApproveTPRequestParam
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string? FilterText { get; set; }
        public int? IDApprovedTP { get; set; }
        public int? Status { get; set; }
        public int? DeleteFlag { get; set; }
        public int? EmployeeID { get; set; }
        public int? TType { get; set; }
        public int? StatusHR { get; set; }
        public int? StatusBGD { get; set; }
        public int? IsBGD { get; set; }
        public int? UserTeamID { get; set; }

    }
}

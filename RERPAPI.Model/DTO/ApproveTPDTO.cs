using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ApproveTPDTO
    {
        public class ApproveItemParam
        {
            public int? Id { get; set; }
            public string? TableName { get; set; } = "";
            public string? ValueUpdatedDate { get; set; } = "";
            public string? ValueDecilineApprove { get; set; } = "";
            public string? EvaluateResults { get; set; } = "";
            public string? FullName { get; set; } = "";
            public string? FieldName { get; set; } = "";

            public int? EmployeeID { get; set; }

            public bool? DeleteFlag { get; set; }
            public bool? IsApprovedTP { get; set; }
            public bool? IsApprovedHR { get; set; }
            public bool? IsApprovedBGD { get; set; }
            public int? IsCancelRegister { get; set; }
            public string? ReasonDeciline { get; set; }
            public int? TType { get; set; }           

            public bool? IsSeniorApproved { get; set; } 
        }

        public class ApproveRequestParam
        {
            public bool? IsApproved { get; set; }               
            public List<ApproveItemParam>? Items { get; set; } = new();
        }

        public class NotProcessedApprovalItem
        {
            public ApproveItemParam? Item { get; set; } = new();
            public string? Reason { get; set; } = "";
        }
    }
}

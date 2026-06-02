using RERPAPI.Model.Common;

namespace RERPAPI.Model.Param
{
    public class ApproveByApproveTPRequestParam
    {
        public DateTime? DateStart { get; set; } = TextUtils.MinDate;
        public DateTime? DateEnd { get; set; } = TextUtils.MaxDate;
        public string? FilterText { get; set; } = "";
        public int? IDApprovedTP { get; set; } = 0;
        public int? Status { get; set; } = 0;
        public int? DeleteFlag { get; set; } = 0;
        public int? EmployeeID { get; set; }
        public int? TType { get; set; } = 0;
        public int? StatusHR { get; set; } = -1;
        public int? StatusBGD { get; set; } = -1;
        public int? StatusSenior { get; set; } = -1;
        public int? IsBGD { get; set; }
        public int? UserTeamID { get; set; } = 0;
        public int? SeniorID { get; set; } = 0;
        public int? StatusDecilineSenior { get; set; } = -1;
    }
}
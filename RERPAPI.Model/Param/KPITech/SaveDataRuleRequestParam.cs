namespace RERPAPI.Model.Param.KPITech
{
    /// <summary>
    ///     employeeID: ID nhân viên
    ///     KPIEmployeePointID: ID của điểm KPI nhân viên
    ///     lstKPIEmployeePointDetail: Danh sách chi tiết điểm KPI của nhân viên
    ///     totalPercentRemaining: Phần trăm còn lại
    /// </summary>
    public class SaveDataRuleRequestParam
    {
        public int employeeID { get; set; }
        public int KPIEmployeePointID { get; set; }
        public decimal totalPercentRemaining { get; set; }
        public List<KPIEmployeePointDetailParam> lstKPIEmployeePointDetail { get; set; } = new List<KPIEmployeePointDetailParam>();
    }
}
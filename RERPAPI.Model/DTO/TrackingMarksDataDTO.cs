using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class TrackingMarksDataDTO : TrackingMark
    {
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string DocumentTypeName { get; set; }
        public string ApprovedName { get; set; }
        public string EmployeeSignName { get; set; }
        public string ApprovedText { get; set; }
        public string SealNameText { get; set; }
        public string TaxCompanyText { get; set; }
    }
}
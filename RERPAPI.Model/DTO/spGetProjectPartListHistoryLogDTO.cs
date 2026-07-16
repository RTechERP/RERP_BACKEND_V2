using System;

namespace RERPAPI.Model.DTO
{
    public class spGetProjectPartListHistoryLogDTO
    {
        public int ID { get; set; }
        public int? ProjectID { get; set; }
        public int? ProjectPartListVersionID { get; set; }
        public int? ProjectPartListID { get; set; }
        public string? ActionType { get; set; }
        public string? ContentLog { get; set; }
        public string? CreatedBy { get; set; }
        public int? CreatedByEmployeeID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public string? VersionCode { get; set; }
        public int? StatusVersion { get; set; }
        public string? ProjectTypeName { get; set; }
    }
}

namespace RERPAPI.Model.Param.KPITech
{
    public class RecalcKPIBatchRequestParam
    {
        public int KpiSessionID { get; set; }
        public int DepartmentID { get; set; } // 0 = All
        public int TeamID { get; set; }       // 0 = All
    }
}
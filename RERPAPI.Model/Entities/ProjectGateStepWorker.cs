using System;

namespace RERPAPI.Model.Entities
{
    public class ProjectGateStepWorker
    {
        public int ID { get; set; }
        public int ProjectGateStepLinkID { get; set; }
        public int EmployeeID { get; set; }
        public decimal? DayCount { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class EmployeeWorkingProcessDTO
    {
        public int ID { get; set; }
        public bool? IsApproved { get; set; }

        public int? Approveder { get; set; }

        public int? EmployeeID { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? JobPosition { get; set; }

        public string? Rank { get; set; }

        public string? Step { get; set; }

        public int? WorkUnit { get; set; }

        public int? Status { get; set; }

        public int? IndirectManagement { get; set; }

        public int? DirectManagement { get; set; }

        public string? DecisionNumber { get; set; }

        public DateTime? DecisionDay { get; set; }

        public decimal? Insurance { get; set; }

        public decimal? ProbationarySalary { get; set; }

        public decimal? BasicSalary { get; set; }

        public decimal? ShiftEat { get; set; }

        public decimal? Gasoline { get; set; }

        public decimal? Phone { get; set; }

        public decimal? House { get; set; }

        public decimal? Skin { get; set; }

        public decimal? Diligence { get; set; }

        public decimal? Other { get; set; }

        public string? Note { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? UpdatedBy { get; set; }
        public bool isDelete { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace RERPAPI.Model.DTO.HRM
{
    public class HRRecruitmentSummaryFilterDTO
    {
        public string? DateStart { get; set; }
        public string? DateEnd { get; set; }
        public int DepartmentID { get; set; } = 0;
        public int IsComplete { get; set; } = -1;
    }

    public class HRRecruitmentSummaryResponseDTO
    {
        public List<object> HiringRequests { get; set; } = new List<object>();
        public List<object> Candidates { get; set; } = new List<object>();
    }
}

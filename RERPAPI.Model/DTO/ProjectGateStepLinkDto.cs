using System;
using System.Collections.Generic;

namespace RERPAPI.Model.DTO
{
    public class SaveProjectGateStepRequestDto
    {
        public int ProjectID { get; set; }
        public List<ProjectGateStepLinkDto> Steps { get; set; }
    }

    public class ProjectGateStepLinkDto
    {
        public int ProjectGateStepID { get; set; }
        public int ProjectTypeID { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsRepeat { get; set; }
        public decimal? DayCount { get; set; }
        public int? PeopleCount { get; set; }
        public string? Content { get; set; }
        public List<ProjectGateStepWorkerDto> Workers { get; set; }
        public List<ProjectGateStepCheckListLinkDto> CheckLists { get; set; }
    }

    public class ProjectGateStepCheckListLinkDto
    {
        public int? ProjectGateStepCheckListID { get; set; }
        public string? PathFolder { get; set; }
        public bool? IsPass { get; set; }
    }

    public class ProjectGateStepWorkerDto
    {
        public int EmployeeID { get; set; }
        public decimal? DayCount { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmount { get; set; }
    }

    // ── Flat DTOs cho Dapper mapping từ stored procedure ──

    /// <summary>
    /// DTO phẳng map từ Query 1 của spGetProjectGateStepLinkByProject
    /// </summary>
    public class ProjectGateStepLinkResultDto
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public int ProjectGateStepID { get; set; }
        public int ProjectTypeID { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsRepeat { get; set; }
        public int? ProjectTaskID { get; set; }
    }

    /// <summary>
    /// DTO phẳng map từ Query 2 của spGetProjectGateStepLinkByProject
    /// </summary>
    public class ProjectGateStepWorkerResultDto
    {
        public int ID { get; set; }
        public int ProjectGateStepLinkID { get; set; }
        public int EmployeeID { get; set; }
        public decimal? DayCount { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmount { get; set; }
        public string EmployeeName { get; set; }
    }
}

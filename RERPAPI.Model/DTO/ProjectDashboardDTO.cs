using System;
using System.Collections.Generic;

namespace RERPAPI.Model.DTO
{
    public class ProjectDashboardRequestParam
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int DepartmentID { get; set; } = 0;
        public int EmployeeID { get; set; } = 0;
        public string? FilterText { get; set; } = "";
        public int GateType { get; set; } = 0;
    }

    public class ProjectDashboardItemResultDto
    {
        public int ID { get; set; }
        public string ProjectCode { get; set; } = "";
        public string ProjectName { get; set; } = "";
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string? EndUserName { get; set; }
        public int? UserID { get; set; }
        public string? FullNameSale { get; set; }
        public int? UserTechnicalID { get; set; }
        public string? FullNameTech { get; set; }
        public int? ProjectManager { get; set; }
        public string? FullNamePM { get; set; }
        public int ProjectStatus { get; set; }
        public string ProjectStatusName { get; set; } = "";
        public string ProjectStatusText { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpectedPlanDate { get; set; }
        public DateTime? RealityProjectEndDate { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = "";
        public int ProjectTypeID { get; set; }
        public string ProjectTypeName { get; set; } = "";
        public string CurrentGate { get; set; } = "G0";
        public int CurrentGateType { get; set; } = 0;
        public bool IsOverdue { get; set; }
        public bool IsGateCompleted { get; set; }
        public int Progress { get; set; }
    }

    public class GateDistributionResultDto
    {
        public string Gate { get; set; } = "";
        public int Count { get; set; }
        public string ProjectCodes { get; set; } = "";
    }

    public class ProjectTypeDistributionResultDto
    {
        public string Name { get; set; } = "";
        public int Count { get; set; }
        public string ProjectCodes { get; set; } = "";
    }

    public class DepartmentDistributionResultDto
    {
        public string DepartmentName { get; set; } = "";
        public int Total { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public string ProjectCodes { get; set; } = "";
    }
}

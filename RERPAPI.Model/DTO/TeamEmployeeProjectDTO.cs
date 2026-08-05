using System;
using System.Collections.Generic;

namespace RERPAPI.Model.DTO
{
    public class TeamEmployeeDto
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int DepartmentID { get; set; }
        public int UserTeamID { get; set; }
        public string UserTeamName { get; set; } = string.Empty;
    }

    public class TeamProjectParticipationDto
    {
        public int ProjectID { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public int ProjectStatus { get; set; }
        public string ProjectStatusText { get; set; } = string.Empty;
        public DateTime? PlanDateStart { get; set; }
        public DateTime? PlanDateEnd { get; set; }
        public DateTime? ActualDateStart { get; set; }
        public DateTime? ActualDateEnd { get; set; }
    }

    public class GetProjectsByEmployeesRequest
    {
        public List<int> EmployeeIds { get; set; } = new();
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}

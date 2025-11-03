using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectReportRequestDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ProjectType { get; set; }
    }

    public class ProjectReportFilterDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ProjectType { get; set; }
        public int DepartmentId { get; set; }
        public int EmployeeId { get; set; }
    }

    public class ProjectReportChartDTO
    {
        public string? Label { get; set; }
        public decimal Value { get; set; }
        public int Count { get; set; }
        public string? Color { get; set; }
    }

    public class ProjectReportSummaryDTO
    {
        public int TotalProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int InProgressProjects { get; set; }
        public int DelayedProjects { get; set; }
        public decimal TotalValue { get; set; }
        public decimal CompletedValue { get; set; }
        public decimal ProgressPercentage { get; set; }
    }
}
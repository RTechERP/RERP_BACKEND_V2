using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.HRM
{
    public class OrganizationalChartDTO
    {
        public List<OrganizationalChart>? organizationalCharts { get; set; }
        public List<OrganizationalChartDetail>? organizationalChartDetails { get; set; }
    }
}
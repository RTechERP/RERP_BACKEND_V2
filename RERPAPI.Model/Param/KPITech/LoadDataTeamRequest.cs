using RERPAPI.Model.Entities;

namespace RERPAPI.Model.Param.KPITech
{
    public class LoadDataTeamRequest
    {
        public int employeeID { get; set; }
        public int kpiSessionID { get; set; }
        public List<Employee> lstEmpChose { get; set; } = new List<Employee>();
    }
}
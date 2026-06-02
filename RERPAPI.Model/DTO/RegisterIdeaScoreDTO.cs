using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class RegisterIdeaScoreDTO : RegisterIdeaScore
    {
        public string? DepartmentName { get; set; }
        public string? TBPName { get; set; }
        public string? ScoreNew { get; set; }
        public int? EmployeeId { get; set; }
        public List<int>? LsDepartmentId { get; set; }
        public bool? tbpCheck { get; set; }
    }
}
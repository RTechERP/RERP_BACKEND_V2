using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class MakerTrainingDTO
    {
        public MakerTraining? MakerTraining { get; set; }
        public List<MakerTrainingDocument>? MakerTrainingDocument { get; set; }
        public List<MakerTrainingEmployeeLink>? MakerTrainingEmployeeLink { get; set; }
        public List<MakerTrainingDepartmentLink>? MakerTrainingDepartmentLink { get; set; }
        public List<MakerTrainingVideoLink>? MakerTrainingVideoLink { get; set; }
        public List<int>? DeletedFileIds { get; set; }
        public List<int>? DeletedEmployees { get; set; }
        public List<int>? DeletedDepartments { get; set; }
        public List<int>? DeletedVideoIds { get; set; }
    }
}
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class SaveProjectWorkerVersionDTO
    {
        public ProjectWorkerVersion? ProjectWorkerVersion { get; set; }
        public List<int> ProjectHistoryProblemIds { get; set; } = new List<int>();
    }
}
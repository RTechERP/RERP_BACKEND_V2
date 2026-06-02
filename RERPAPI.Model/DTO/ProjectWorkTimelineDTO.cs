namespace RERPAPI.Model.DTO
{
    public class ProjectWorkTimelineDTO
    {
        public int ProjectIDOld { get; set; }
        public int ProjectIDNew { get; set; }

        public List<int> reportIDs { get; set; }
    }
}
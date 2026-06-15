namespace RERPAPI.Model.DTO
{
    public class ProjectWorkReportDTO
    {
        public int ProjectIDOld { get; set; }
        public int ProjectIDNew { get; set; }

        public List<int> reportIDs { get; set; }
    }
}
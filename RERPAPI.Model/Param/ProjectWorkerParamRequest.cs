namespace RERPAPI.Model.Param
{
    public class ProjectWorkerParamRequest
    {
        public int projectID { get; set; }
        public int projectWorkerTypeID { get; set; }
        public int IsApprovedTBP { get; set; }
        public int IsDeleted { get; set; }
        public string? KeyWord { get; set; }
        public int versionID { get; set; }
    }
}
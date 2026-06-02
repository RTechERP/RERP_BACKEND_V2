namespace RERPAPI.Model.Param.Project
{
    public class ProjectDerpartmentSummaryParamRequest
    {
        public DateTime dateTimeS { get; set; }
        public DateTime dateTimeE { get; set; }
        public int departmentID { get; set; }
        public int userID { get; set; }
        public string? projectTypeID { get; set; }
        public string? keyword { get; set; }
        public int userTeamID { get; set; }
    }
}
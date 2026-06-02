using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ReportTypeRepo : GenericRepo<ReportType>
    {
        public ReportTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
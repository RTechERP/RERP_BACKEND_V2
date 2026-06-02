using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPIErrorEmployeeFileRepo : GenericRepo<KPIErrorEmployeeFile>
    {
        public KPIErrorEmployeeFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
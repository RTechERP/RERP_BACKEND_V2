using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPIErrorEmployeeRepo : GenericRepo<KPIErrorEmployee>
    {
        public KPIErrorEmployeeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
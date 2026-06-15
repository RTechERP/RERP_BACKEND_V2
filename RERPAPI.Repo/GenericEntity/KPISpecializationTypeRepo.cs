using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPISpecializationTypeRepo : GenericRepo<KPISpecializationType>
    {
        public KPISpecializationTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
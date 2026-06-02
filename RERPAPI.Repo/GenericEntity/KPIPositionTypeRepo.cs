using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPIPositionTypeRepo : GenericRepo<KPIPositionType>
    {
        public KPIPositionTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
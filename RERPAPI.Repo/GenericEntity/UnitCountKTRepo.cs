using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class UnitCountKTRepo : GenericRepo<UnitCountKT>
    {
        public UnitCountKTRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
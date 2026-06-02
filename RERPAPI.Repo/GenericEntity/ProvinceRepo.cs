using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProvinceRepo : GenericRepo<Province>
    {
        public ProvinceRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
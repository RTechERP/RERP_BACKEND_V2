using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class SaleUserTypeRepo : GenericRepo<SaleUserType>
    {
        public SaleUserTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
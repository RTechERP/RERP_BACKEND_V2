using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class OfficeSupplyRequestsDetailRepo : GenericRepo<OfficeSupplyRequestsDetail>
    {
        public OfficeSupplyRequestsDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
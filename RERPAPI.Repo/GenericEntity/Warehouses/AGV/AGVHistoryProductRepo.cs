using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Warehouses.AGV
{
    public class AGVHistoryProductRepo : GenericRepo<AGVHistoryProduct>
    {
        public AGVHistoryProductRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
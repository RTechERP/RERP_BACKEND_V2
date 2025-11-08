using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.Vehicle
{
    public class ProposeVehicleRepairDetailRepo : GenericRepo<ProposeVehicleRepairDetail>
    {
        //public bool Validate(ProposeVehicleRepairDetail item, out string message)
        //{

        //}
        public ProposeVehicleRepairDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

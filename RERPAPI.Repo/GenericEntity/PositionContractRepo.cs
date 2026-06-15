using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PositionContractRepo : GenericRepo<EmployeeChucVuHD>
    {
        public PositionContractRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
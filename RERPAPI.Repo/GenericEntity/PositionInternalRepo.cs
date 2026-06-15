using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PositionInternalRepo : GenericRepo<EmployeeChucVu>
    {
        public PositionInternalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
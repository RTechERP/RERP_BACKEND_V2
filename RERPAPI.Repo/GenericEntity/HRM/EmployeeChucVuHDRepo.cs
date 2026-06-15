using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeChucVuHDRepo : GenericRepo<EmployeeChucVuHD>
    {
        public EmployeeChucVuHDRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
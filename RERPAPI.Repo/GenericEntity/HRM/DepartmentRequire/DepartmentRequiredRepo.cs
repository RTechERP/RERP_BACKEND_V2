using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.DepartmentRequire
{
    public class DepartmentRequiredRepo : GenericRepo<DepartmentRequired>
    {
        public DepartmentRequiredRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
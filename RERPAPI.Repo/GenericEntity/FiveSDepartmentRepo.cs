using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class FiveSDepartmentRepo : GenericRepo<FiveSDepartment>
    {
        public FiveSDepartmentRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
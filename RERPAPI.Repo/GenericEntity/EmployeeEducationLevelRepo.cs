using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeEducationLevelRepo : GenericRepo<EmployeeEducationLevel>
    {
        public EmployeeEducationLevelRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
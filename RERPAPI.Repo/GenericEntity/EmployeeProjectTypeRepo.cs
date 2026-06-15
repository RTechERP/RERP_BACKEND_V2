using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeProjectTypeRepo : GenericRepo<EmployeeProjectType>
    {
        public EmployeeProjectTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeCurricularRepo : GenericRepo<EmployeeCurricular>
    {
        public EmployeeCurricularRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
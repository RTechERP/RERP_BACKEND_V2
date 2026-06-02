using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeApproveRepo : GenericRepo<EmployeeApprove>
    {
        public EmployeeApproveRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeApprovedRepo : GenericRepo<EmployeeApprove>
    {
        public EmployeeApprovedRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
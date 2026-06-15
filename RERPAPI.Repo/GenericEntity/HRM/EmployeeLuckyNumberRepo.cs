using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EmployeeLuckyNumberRepo : GenericRepo<EmployeeLuckyNumber>
    {
        public EmployeeLuckyNumberRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
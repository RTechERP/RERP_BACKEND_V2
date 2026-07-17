using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class SalaryIncreaseDetailRepo : GenericRepo<SalaryIncreaseDetail>
    {
        public SalaryIncreaseDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

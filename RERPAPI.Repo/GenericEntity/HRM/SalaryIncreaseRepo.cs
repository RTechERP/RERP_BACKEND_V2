using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class SalaryIncreaseRepo : GenericRepo<SalaryIncrease>
    {
        public SalaryIncreaseRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

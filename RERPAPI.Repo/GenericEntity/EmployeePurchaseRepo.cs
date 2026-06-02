using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeePurchaseRepo : GenericRepo<EmployeePurchase>
    {
        public EmployeePurchaseRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
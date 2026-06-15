using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeTeamSaleRepo : GenericRepo<EmployeeTeamSale>
    {
        public EmployeeTeamSaleRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
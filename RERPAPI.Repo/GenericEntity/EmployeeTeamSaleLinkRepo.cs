using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeTeamSaleLinkRepo : GenericRepo<EmployeeTeamSaleLink>
    {
        public EmployeeTeamSaleLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
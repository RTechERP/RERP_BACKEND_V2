using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class OrganizationalChartDetailRepo : GenericRepo<OrganizationalChartDetail>
    {
        public OrganizationalChartDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
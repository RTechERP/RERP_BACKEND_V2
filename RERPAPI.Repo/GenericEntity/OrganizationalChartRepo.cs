using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class OrganizationalChartRepo : GenericRepo<OrganizationalChart>
    {
        public OrganizationalChartRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
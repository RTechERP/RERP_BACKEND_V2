using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class vUserGroupLinksRepo : GenericRepo<vUserGroupLink>
    {
        public vUserGroupLinksRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
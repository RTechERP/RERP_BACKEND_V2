using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class GroupFileRepo : GenericRepo<GroupFile>
    {
        public GroupFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
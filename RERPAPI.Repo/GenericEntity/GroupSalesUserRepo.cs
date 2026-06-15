using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class GroupSalesUserRepo : GenericRepo<GroupSalesUser>
    {
        public GroupSalesUserRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
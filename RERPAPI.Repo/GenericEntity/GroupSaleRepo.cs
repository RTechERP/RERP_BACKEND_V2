using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class GroupSaleRepo : GenericRepo<GroupSale>
    {
        public GroupSaleRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
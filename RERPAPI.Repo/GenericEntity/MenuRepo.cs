using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class MenuRepo : GenericRepo<Menu>
    {
        public MenuRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
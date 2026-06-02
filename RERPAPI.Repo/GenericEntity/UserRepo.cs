using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class UserRepo : GenericRepo<User>
    {
        public UserRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
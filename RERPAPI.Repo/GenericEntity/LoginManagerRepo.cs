using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class LoginManagerRepo : GenericRepo<User>
    {
        public LoginManagerRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
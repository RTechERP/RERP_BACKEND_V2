using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class AppMobileVersionRepo : GenericRepo<AppMobileVersion>
    {
        public AppMobileVersionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class MainIndexRepo : GenericRepo<MainIndex>
    {
        public MainIndexRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
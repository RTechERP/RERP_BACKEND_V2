using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PONCCDetailRepo : GenericRepo<PONCCDetail>
    {
        public PONCCDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
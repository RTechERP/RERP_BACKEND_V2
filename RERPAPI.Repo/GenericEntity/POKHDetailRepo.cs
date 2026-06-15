using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class POKHDetailRepo : GenericRepo<POKHDetail>
    {
        public POKHDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
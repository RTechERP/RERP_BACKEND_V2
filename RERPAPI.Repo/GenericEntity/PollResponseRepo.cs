using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PollResponseRepo : GenericRepo<PollResponse>
    {
        public PollResponseRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
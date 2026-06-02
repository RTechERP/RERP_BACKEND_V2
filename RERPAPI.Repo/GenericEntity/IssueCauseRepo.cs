using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class IssueCauseRepo : GenericRepo<IssueCause>
    {
        public IssueCauseRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
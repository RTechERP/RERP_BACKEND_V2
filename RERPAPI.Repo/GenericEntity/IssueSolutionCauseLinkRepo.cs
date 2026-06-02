using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class IssueSolutionCauseLinkRepo : GenericRepo<IssueSolutionCauseLink>
    {
        public IssueSolutionCauseLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
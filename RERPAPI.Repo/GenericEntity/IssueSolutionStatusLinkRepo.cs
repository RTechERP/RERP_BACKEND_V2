using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class IssueSolutionStatusLinkRepo : GenericRepo<IssueSolutionStatusLink>
    {
        public IssueSolutionStatusLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
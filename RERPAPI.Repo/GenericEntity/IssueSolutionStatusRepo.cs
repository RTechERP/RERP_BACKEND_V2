using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class IssueSolutionStatusRepo : GenericRepo<IssueSolutionStatus>
    {
        public IssueSolutionStatusRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
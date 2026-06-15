using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class IssueLogSolutionRepo : GenericRepo<IssueLogSolution>
    {
        public IssueLogSolutionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
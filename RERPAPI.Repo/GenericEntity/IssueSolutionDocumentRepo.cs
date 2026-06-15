using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class IssueSolutionDocumentRepo : GenericRepo<IssueSolutionDocument>
    {
        public IssueSolutionDocumentRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
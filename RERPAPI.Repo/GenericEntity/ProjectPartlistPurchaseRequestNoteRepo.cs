using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartlistPurchaseRequestNoteRepo : GenericRepo<ProjectPartlistPurchaseRequestNote>
    {
        public ProjectPartlistPurchaseRequestNoteRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
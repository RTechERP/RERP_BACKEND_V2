using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartlistPriceRequestNoteRepo : GenericRepo<ProjectPartlistPriceRequestNote>
    {
        public ProjectPartlistPriceRequestNoteRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

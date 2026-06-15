using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class RegisterIdeaRepo : GenericRepo<RegisterIdea>
    {
        public RegisterIdeaRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class RegisterIdeaFileRepo : GenericRepo<RegisterIdeaFile>
    {
        public RegisterIdeaFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
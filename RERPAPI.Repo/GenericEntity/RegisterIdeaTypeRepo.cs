using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class RegisterIdeaTypeRepo : GenericRepo<RegisterIdeaType>
    {
        public RegisterIdeaTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
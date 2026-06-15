using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class RegisterIdeaDetailRepo : GenericRepo<RegisterIdeaDetail>
    {
        public RegisterIdeaDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
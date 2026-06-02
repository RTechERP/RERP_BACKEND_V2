using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class RegisterIdeaScoreRepo : GenericRepo<RegisterIdeaScore>
    {
        public RegisterIdeaScoreRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
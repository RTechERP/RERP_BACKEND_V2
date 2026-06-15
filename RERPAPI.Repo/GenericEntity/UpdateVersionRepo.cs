using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class UpdateVersionRepo : GenericRepo<UpdateVersion>
    {
        public UpdateVersionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
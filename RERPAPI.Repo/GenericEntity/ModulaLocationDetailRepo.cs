using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ModulaLocationDetailRepo : GenericRepo<ModulaLocationDetail>
    {
        public ModulaLocationDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
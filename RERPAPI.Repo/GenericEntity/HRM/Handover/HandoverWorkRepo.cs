using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.BBNV
{
    public class HandoverWorkRepo : GenericRepo<HandoverWork>
    {
        public HandoverWorkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class FirmBaseRepo : GenericRepo<FirmBase>
    {
        public FirmBaseRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
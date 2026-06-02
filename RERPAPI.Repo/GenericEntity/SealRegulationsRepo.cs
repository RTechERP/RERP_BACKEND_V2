using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class SealRegulationsRepo : GenericRepo<SealRegulation>
    {
        public SealRegulationsRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class FiveSErrorRepo : GenericRepo<FiveSError>
    {
        public FiveSErrorRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
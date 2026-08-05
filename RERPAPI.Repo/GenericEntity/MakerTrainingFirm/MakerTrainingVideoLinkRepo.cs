using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.MakerTrainingFirm
{
    public class MakerTrainingVideoLinkRepo : GenericRepo<MakerTrainingVideoLink>
    {
        public MakerTrainingVideoLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

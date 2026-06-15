using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.MakerTrainingFirm
{
    public class MakerTrainingRepo : GenericRepo<MakerTraining>
    {
        public MakerTrainingRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
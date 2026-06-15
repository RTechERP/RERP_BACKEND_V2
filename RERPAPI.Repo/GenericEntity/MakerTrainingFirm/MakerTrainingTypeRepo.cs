using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.MakerTrainingFirm
{
    public class MakerTrainingTypeRepo : GenericRepo<MakerTrainingType>
    {
        public MakerTrainingTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
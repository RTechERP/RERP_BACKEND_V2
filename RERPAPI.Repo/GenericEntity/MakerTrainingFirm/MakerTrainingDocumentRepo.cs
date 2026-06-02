using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.MakerTrainingFirm
{
    public class MakerTrainingDocumentRepo : GenericRepo<MakerTrainingDocument>
    {
        public MakerTrainingDocumentRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
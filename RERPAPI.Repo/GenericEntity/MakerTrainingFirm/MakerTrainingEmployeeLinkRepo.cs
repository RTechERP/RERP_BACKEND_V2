using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.MakerTrainingFirm
{
    public class MakerTrainingEmployeeLinkRepo : GenericRepo<MakerTrainingEmployeeLink>
    {
        public MakerTrainingEmployeeLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
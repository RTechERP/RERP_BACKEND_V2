using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TrainingRegistrationCategoryRepo : GenericRepo<TrainingRegistrationCategory>
    {
        public TrainingRegistrationCategoryRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
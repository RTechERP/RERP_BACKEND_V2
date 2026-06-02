using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TrainingRegistrationDetailRepo : GenericRepo<TrainingRegistrationDetail>
    {
        public TrainingRegistrationDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
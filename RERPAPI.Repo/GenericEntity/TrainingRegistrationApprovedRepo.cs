using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TrainingRegistrationApprovedRepo : GenericRepo<TrainingRegistrationApproved>
    {
        public TrainingRegistrationApprovedRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
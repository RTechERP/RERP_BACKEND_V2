using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TrainingRegistrationApprovedFlowRepo : GenericRepo<TrainingRegistrationApprovedFlow>
    {
        public TrainingRegistrationApprovedFlowRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
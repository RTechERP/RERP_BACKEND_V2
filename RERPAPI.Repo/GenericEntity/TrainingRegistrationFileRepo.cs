using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TrainingRegistrationFileRepo : GenericRepo<TrainingRegistrationFile>
    {
        public TrainingRegistrationFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
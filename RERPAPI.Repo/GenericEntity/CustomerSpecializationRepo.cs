using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CustomerSpecializationRepo : GenericRepo<CustomerSpecialization>
    {
        public CustomerSpecializationRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CustomerContactRepo : GenericRepo<CustomerContact>
    {
        public CustomerContactRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
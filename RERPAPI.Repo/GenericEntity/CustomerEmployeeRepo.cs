using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CustomerEmployeeRepo : GenericRepo<CustomerEmployee>
    {
        public CustomerEmployeeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
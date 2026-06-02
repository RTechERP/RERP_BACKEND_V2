using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CustomerRepo : GenericRepo<Customer>
    {
        public CustomerRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
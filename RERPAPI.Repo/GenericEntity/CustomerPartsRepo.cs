using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CustomerPartsRepo : GenericRepo<CustomerPart>
    {
        public CustomerPartsRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
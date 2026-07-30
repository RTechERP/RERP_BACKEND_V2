using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ExpectedPayableRepo : GenericRepo<ExpectedPayable>
    {
        public ExpectedPayableRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
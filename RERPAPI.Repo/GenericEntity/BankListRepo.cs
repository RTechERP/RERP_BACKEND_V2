using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BankListRepo : GenericRepo<BankList>
    {
        public BankListRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
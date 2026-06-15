using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class AccountingContractLogRepo : GenericRepo<AccountingContractLog>
    {
        public AccountingContractLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
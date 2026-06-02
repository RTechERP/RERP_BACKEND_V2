using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class AccountingContractFileRepo : GenericRepo<AccountingContractFile>
    {
        public AccountingContractFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
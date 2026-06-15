using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class AccountingContractTypeRepo : GenericRepo<AccountingContractType>
    {
        public AccountingContractTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
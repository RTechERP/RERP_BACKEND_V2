using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TaxCompanyRepo : GenericRepo<TaxCompany>
    {
        public TaxCompanyRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
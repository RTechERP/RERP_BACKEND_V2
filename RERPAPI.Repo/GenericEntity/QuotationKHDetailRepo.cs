using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class QuotationKHDetailRepo : GenericRepo<QuotationKHDetail>
    {
        public QuotationKHDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
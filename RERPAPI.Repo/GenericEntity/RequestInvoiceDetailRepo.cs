using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class RequestInvoiceDetailRepo : GenericRepo<RequestInvoiceDetail>
    {
        public RequestInvoiceDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
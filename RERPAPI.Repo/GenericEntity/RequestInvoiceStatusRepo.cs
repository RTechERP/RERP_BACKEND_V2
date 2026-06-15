using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class RequestInvoiceStatusRepo : GenericRepo<RequestInvoiceStatus>
    {
        public RequestInvoiceStatusRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
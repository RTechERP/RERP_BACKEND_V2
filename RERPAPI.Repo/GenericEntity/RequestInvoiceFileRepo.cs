using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class RequestInvoiceFileRepo : GenericRepo<RequestInvoiceFile>
    {
        public RequestInvoiceFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
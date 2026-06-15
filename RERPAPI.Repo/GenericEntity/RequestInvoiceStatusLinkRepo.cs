using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class RequestInvoiceStatusLinkRepo : GenericRepo<RequestInvoiceStatusLink>
    {
        public RequestInvoiceStatusLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
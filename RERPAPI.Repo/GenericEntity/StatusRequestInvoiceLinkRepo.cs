using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class StatusRequestInvoiceLinkRepo : GenericRepo<StatusRequestInvoiceLink>
    {
        public StatusRequestInvoiceLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
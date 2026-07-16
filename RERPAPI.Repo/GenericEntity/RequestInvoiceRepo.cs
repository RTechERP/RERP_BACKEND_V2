using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class RequestInvoiceRepo : GenericRepo<RequestInvoice>
    {
        public RequestInvoiceRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public string GetLatestCodeByDate(DateTime date)
        {
            var item = GetAll()
                .Where(x => x.CreatedDate.HasValue
                    && x.IsDeleted != true
                    && x.CreatedDate.Value.Date == date.Date)
                .OrderByDescending(x => x.ID)
                .FirstOrDefault();
            return item?.Code ?? "";
        }
    }
}
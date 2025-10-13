using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class RequestInvoiceRepo:GenericRepo<RequestInvoice>
    {
        public string GetLatestCodeByDate(DateTime date)
        {
            var item = GetAll()
                .Where(x => x.CreatedDate.HasValue
                    && x.CreatedDate.Value.Date == date.Date)
                .OrderByDescending(x => x.ID)
                .FirstOrDefault();
            return item?.Code ?? "";
        }

    }
}

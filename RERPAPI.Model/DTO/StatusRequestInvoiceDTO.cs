using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class StatusRequestInvoiceDTO
    {
        public List<RequestInvoiceStatusLink> StatusRequestInvoiceLinks { get; set; }
        public List<int> listIdsStatusDel { get; set; }
        public int requestInvoiceId { get; set; }
    }
}

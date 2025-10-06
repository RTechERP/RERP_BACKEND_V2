using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class QuotationDTO
    {
        public QuotationKH quotationKHs { get; set; } = new QuotationKH();
        public List<QuotationKHDetail> quotationKHDetails { get; set; } = new List<QuotationKHDetail>();
        public List<int> DeletedDetailIds { get; set; } = new List<int>();
    }
}

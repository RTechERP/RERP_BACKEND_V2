using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class MailItemPriceRequestDTO
    {
        public int EmployeeID { get; set; }
        public string QuoteEmployee { get; set; }
        public string ProjectCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Manufacturer { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public DateTime? DateRequest { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? DatePriceQuote { get; set; }
        public int CurrencyID { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalPriceExchange { get; set; }
    }
}

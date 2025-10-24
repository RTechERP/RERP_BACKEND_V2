using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class InsertCustomerDTO
    {
        public Customer? Customer { get; set; }
        public List<CustomerContact>? CustomerContacts { get; set; }
        public List<AddressStock>? AddressStocks { get; set; }
        public List<CustomerEmployee>? CustomerEmployees { get; set; }
        public int? BusinessFieldID { get; set; }
        public List<int>? isDeleted { get; set; }
    }
}

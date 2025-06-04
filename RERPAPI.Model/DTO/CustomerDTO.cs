using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class CustomerDTO
    {
        public int ID { get; set; }
        public string Province { get; set; }
        public string CodeProvinces { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerShortName { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public int CustomerType { get; set; }
        public string ProductDetails { get; set; }
        public string NoteDelivery { get; set; }
        public string NoteVoucher { get; set; }
        public string CheckVoucher { get; set; }
        public string HardCopyVoucher { get; set; }
        public int CustomerSpecializationID { get; set; }
        public DateTime? ClosingDateDebt { get; set; }
        public string Debt { get; set; }
        public string TaxCode { get; set; }
        public bool BigAccount { get; set; }
        public List<CustomerContactDTO> Contacts { get; set; }
        public List<CustomerAddressDTO> Addresses { get; set; }
        public List<CustomerEmployeeDTO> Sales { get; set; }
        public int BusinessFieldID { get; set; }
    }
}

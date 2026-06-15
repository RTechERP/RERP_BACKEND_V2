using RERPAPI.Model.Entities;

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

        ///TNB.update 27/10/25
        public List<int>? deletedIdsEmp { get; set; }

        public List<int>? deletedIdsAdrress { get; set; }
        public List<int>? deletedIdsContact { get; set; }
    }
}
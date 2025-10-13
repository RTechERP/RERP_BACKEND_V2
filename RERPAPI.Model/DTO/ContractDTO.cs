using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ContractDTO
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public int EmployeeLoaiHDLDID { get; set; }
        public string ContractNumber { get; set; }
        public string DateContract { get; set; }
        public string FullName { get; set; }
        public string DateOfBirth { get; set; }
        public string CCCD_CMND { get; set; }
        public string IssuedBy { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Sex { get; set; }
        public string Nationality { get; set; }
        public string DateRange { get; set; }
        public string ContractType { get; set; }
        public string ContractDuration { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string Salary { get; set; }
        public string NotificationDate { get; set; }
    }
}

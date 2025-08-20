using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Asset
{
    public class AssetAllocationExportDto
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public DateTime DateAllocation { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Possition { get; set; }
        public string Note { get; set; }

        public bool IsApproved { get; set; }
        public bool IsApproveAccountant { get; set; }
        public bool IsApprovedPersonalProperty { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? DateApproveAccountant { get; set; }
        public DateTime? DateApprovedPersonalProperty { get; set; }
        public DateTime? DateApprovedHR { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Asset
{
    public class AssetRecoveryExportDto
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public DateTime DateRecovery { get; set; }
        public string EmployeeReturnName { get; set; }
        public string DepartmentReturn { get; set; }
        public string PossitionReturn { get; set; }
        public string EmployeeRecoveryName { get; set; }
        public string DepartmentRecovery { get; set; }
        public string PossitionRecovery { get; set; }
        public string Note { get; set; }
        public bool IsApproved { get; set; }
        public bool IsApproveAccountant { get; set; }
        public bool IsApprovedPersonalProperty { get; set; }
        public DateTime? DateApprovedHR { get; set; }
        public DateTime? DateApproveAccountant { get; set; }
        public DateTime? DateApprovedPersonalProperty { get; set; }
    }

}

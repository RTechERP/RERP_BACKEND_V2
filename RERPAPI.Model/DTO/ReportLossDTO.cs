using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ReportLostFullDto
    {
        // === Phần dành cho TSLostReportAsset ===
   
        public int AssetManagementID { get; set; } // ID tài sản
        public DateTime DateLostReport { get; set; } // Ngày báo mất
        public string? Reason { get; set; } // Lý do mất

        // === Phần dành cho TSAllocationEvictionAsset ===
        public int? EmployeeID { get; set; }
        public int? DepartmentID { get; set; }
        public int? ChucVuID { get; set; }
        public DateTime DateAllocation { get; set; } // Ngày thu hồi tài sản
        public string? Note { get; set; } // Ghi chú lý do mất
        public string AssetStatus { get; set; } = "Mất "; // Trạng thái mới
        public int AssetStatusID { get; set; } = 4; // Mã trạng thái "Mất"
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
    }
}

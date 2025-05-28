using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ReportBrokenFullDto
    {
        // Dành cho bảng TSAssetManagement
        public int AssetID { get; set; }
        public string Note { get; set; }
        public string Status { get; set; } = "Hỏng";
        public int StatusID { get; set; } = 5;

        public DateTime DateReportBroken { get; set; }
        public string Reason { get; set; }
        public DateTime? CreatedDate { get; set; }
       
        public DateTime? UpdatedDate { get; set; }
        // Dành cho bảng TSAllocationEvictionAsset
        public int EmployeeID { get; set; }
        public int DepartmentID { get; set; }
        public int? ChucVuID { get; set; } // lấy từ bảng Employee
        public DateTime DateAllocation { get; set; }
    }
}


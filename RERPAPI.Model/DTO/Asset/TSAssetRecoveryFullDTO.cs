using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Asset
{
    public class TSAssetRecoveryFullDTO
    {
        // TSAssetRecovery (Master table)
        public int ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime DateRecovery { get; set; }
        public int EmployeeReturnID { get; set; }
        public int EmployeeRecoveryID { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }

        // Danh sách chi tiết thu hồi tài sản
        public List<TSAssetRecoveryDetailFullDTO> AssetDetails { get; set; } = new();
    }
    public class TSAssetRecoveryDetailFullDTO
    {
        // TSAssetRecoveryDetail
        public int ID { get; set; }
        public int STT { get; set; }
        public int AssetManagementID { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }

        // Dùng để cập nhật bảng TSAssetManagement (nếu cần)
        public int DepartmentID { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}

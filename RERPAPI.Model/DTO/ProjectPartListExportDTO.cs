using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectPartListExportDTO
    {
        public int ID { get; set; }

        // Các field từ TreeListNode (UI) - BẮT BUỘC
        public decimal RemainQuantity { get; set; }      // Số lượng còn lại có thể xuất
        public decimal QuantityReturn { get; set; }      // Số lượng trả (MỚI THÊM)
        public decimal QtyFull { get; set; }             // Số lượng đầy đủ
        public string ProductNewCode { get; set; }       // Mã nội bộ
        public string GroupMaterial { get; set; }        // Tên sản phẩm
        public string Unit { get; set; }                 // Đơn vị tính
        public string ProjectCode { get; set; }          // Mã dự án
        public string TT { get; set; }

        // Các field khác nếu cần cho ValidateKeep
        public int ProductID { get; set; }
        public int WarehouseID { get; set; }
        public int ProjectID { get; set; }
    }
}

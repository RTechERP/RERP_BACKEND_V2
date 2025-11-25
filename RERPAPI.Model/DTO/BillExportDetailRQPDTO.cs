using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class BillExportDetailRQPDTO
    {
        public int STT { get; set; }
        public int ChildID { get; set; }
        public int ParentID { get; set; }

        // Thông tin sản phẩm
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductNewCode { get; set; }
        public string ProductName { get; set; }
        public string ProductFullName { get; set; }
        public string Unit { get; set; }

        // Số lượng
        public decimal Qty { get; set; }              // Số lượng xuất (đã tính toán)
        public decimal TotalQty { get; set; }         // Tổng số lượng

        // Thông tin dự án
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCodeText { get; set; }
        public string ProjectCodeExport { get; set; }

        // Thông tin khác
        public int ProjectPartListID { get; set; }
        public string Note { get; set; }
        public string SerialNumber { get; set; }
    }
}

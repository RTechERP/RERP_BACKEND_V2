using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class BillImportTechnicalDTO
    {
        public int ID { get; set; }
        public string? BillCode { get; set; }
        public DateTime? CreatDate { get; set; }
        public string? Deliver { get; set; }
        public string? Receiver { get; set; }
        public bool? Status { get; set; }
        public string? Suplier { get; set; }
        public bool? BillType { get; set; }
        public string? WarehouseType { get; set; }
        public int? DeliverID { get; set; }
        public int? ReceiverID { get; set; }
        public int? SuplierID { get; set; }
        public int? GroupTypeID { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Image { get; set; }
        public int? WarehouseID { get; set; }
        public int? SupplierSaleID { get; set; }
        public int? BillTypeNew { get; set; }
        public int? IsBorrowSupplier { get; set; }
        public int? CustomerID { get; set; }
        public int? BillDocumentImportType { get; set; }
        public DateTime? DateRequestImport { get; set; }
        public int? RulePayID { get; set; }
        public bool? IsNormalize { get; set; }
        public int? ApproverID { get; set; }
        public bool? IsDeleted { get; set; }

        // Thêm các trường mở rộng từ JOIN và CASE
        public DateTime? DateStatus { get; set; }
        public string? NCC { get; set; }                   // Tên nhà cung cấp (có thể từ SupplierSale hoặc trực tiếp)
        public string? DepartmentName { get; set; }        // Tên phòng ban người giao
        public string? CustomerName { get; set; }          // Tên khách hàng
        public string? BillTypeNewText { get; set; }       // Text hiển thị loại phiếu
        public string? EmployeeApproveName { get; set; }   // Người duyệt
        public string? EmployeeReceiverName { get; set; }  // Người nhận
    }

}

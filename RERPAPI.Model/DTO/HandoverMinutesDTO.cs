using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class HandoverMinutesDTO
    {
        public int ID { get; set; } // ID biên bản (0 nếu tạo mới)
        public DateTime? DateMinutes { get; set; } // Ngày biên bản
        public int? CustomerID { get; set; } // ID khách hàng
        public string? CustomerAddress { get; set; } // Địa chỉ khách hàng
        public string? CustomerContact { get; set; } // Người liên hệ
        public string? CustomerPhone { get; set; } // Số điện thoại khách hàng
        public int? EmployeeID { get; set; } // ID nhân viên
        public string? Receiver { get; set; } // Người nhận
        public string? ReceiverPhone { get; set; } // Số điện thoại người nhận
        public int AdminWarehouseID { get; set; } // ID quản trị kho
        public bool? IsDeleted { get; set; }
        public List<HandoverMinutesDetailRequest>? Details { get; set; } // Danh sách chi tiết
        public List<int>? DeletedDetailIds { get; set; } // Danh sách ID chi tiết cần xóa
    }

    public class HandoverMinutesDetailRequest
    {
        public int ID { get; set; } // ID chi tiết (0 nếu tạo mới)
        public int STT { get; set; } // Số thứ tự
        public int POKHDetailID { get; set; } // ID chi tiết POKH
        public int Quantity { get; set; } // Số lượng
        public int? ProductStatus { get; set; } // Trạng thái sản phẩm
        public string? Guarantee { get; set; } // Bảo hành
        public int? DeliveryStatus { get; set; } // Trạng thái giao hàng
    }

}

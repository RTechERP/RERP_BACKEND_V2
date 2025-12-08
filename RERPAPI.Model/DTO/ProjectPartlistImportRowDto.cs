using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectPartlistImportRowDto
    {
        public string TT { get; set; }                 // STT dạng '1', '1.1', '1.1.1'
        public string GroupMaterial { get; set; }      // F2 - Tên vật tư
        public string ProductCode { get; set; }        // F3 - Mã thiết bị
        public string OrderCode { get; set; }          // F4 - Mã đặt hàng / OrderCode
        public string Manufacturer { get; set; }       // F5 - Hãng
        public string? SpecialCode { get;set; }          // F6- Mã đặc biệt 
        public string Model { get; set; }              // F7 - Model
        public decimal? QtyMin { get; set; }           // F8 - Số lượng / 1 máy
        public decimal? QtyFull { get; set; }          // F9 - Số lượng tổng
        public string Unit { get; set; }               // F10 - Đơn vị
        public decimal? Price { get; set; }            // F11 - Đơn giá
        public decimal? Amount { get; set; }           // F12 - Thành tiền
        public string LeadTime { get; set; }           // F13 - Tiến độ
        public string NCC { get; set; }                // F14 - Nhà cung cấp
        public DateTime? RequestDate { get; set; }     // F15 - Ngày yêu cầu đặt hàng
        public string LeadTimeRequest { get; set; }    // F16 - Tiến độ yêu cầu
        public decimal? QuantityReturn { get; set; }   // F17 - Số lượng đặt thực tế
        public string NCCFinal { get; set; }           // F18 - Nhà cung cấp mua
        public decimal? PriceOrder { get; set; }       // F19 - Giá đặt mua
        public DateTime? OrderDate { get; set; }       // F20 - Ngày đặt hàng thực tế
        public DateTime? ExpectedReturnDate { get; set; } // F21 - Dự kiến hàng về
        public int? Status { get; set; }               // F22 - Tình trạng
        public string Quality { get; set; }            // F23 - Chất lượng
        public string Note { get; set; }               // F24 - Ghi chú
        public string ReasonProblem { get; set; }      // F25 - Lý do phát sinh (nếu IsProblem)
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RERPAPI.Model.Entities.ESL
{
    public class ESLTestTableRegistrationDetail
    {
        public int ID { get; set; }
        public int RegistrationID { get; set; }
        public int No { get; set; }
        public int Type { get; set; } // 1=Đăng ký lần đầu, 2=Gia hạn, 3=Bàn giao
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public int OwnerID { get; set; }
        public int ApproverID { get; set; }
        
        public int Status { get; set; } // 0=Chờ duyệt, 1=Đã duyệt, 2=Từ chối
        public DateTime? ApproveDate { get; set; }
        public string ApproveNote { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}


using System;

namespace RERPAPI.Model.DTO.ESL
{
    public class ESLCheckConflictRequest
    {
        public int TestTableId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? ExcludeDetailId { get; set; }
    }

    public class ESLApproveRequest
    {
        public int DetailId { get; set; }
        public bool IsApproved { get; set; }
        public string Note { get; set; }
        public int ApproverId { get; set; }
    }

    public class ESLRegistrationRequest
    {
        // For No=1
        public int TestTableID { get; set; }
        public int OwnerID { get; set; }
        public int ApproverID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProjectCode { get; set; }
        public string RegistrationContent { get; set; }
    }

    public class ESLExtendHandoverRequest
    {
        public int RegistrationID { get; set; }
        public int Type { get; set; } // 2: Gia hạn, 3: Bàn giao
        public int OwnerID { get; set; } // OwnerID mới hoặc cũ
        public int ApproverID { get; set; }
    }

    public class ESLReturnRequest
    {
        public int RegistrationID { get; set; }
        public int ReturnBy { get; set; }
    }

    public class ESLRegistrationListDto
    {
        // Master
        public int RegistrationID { get; set; }
        public string RegistrationCode { get; set; }
        public int TestTableID { get; set; }
        public string TestTableName { get; set; }
        public string Barcode { get; set; }
        public int TableSide { get; set; }
        public string ProjectCode { get; set; }
        public string RegistrationContent { get; set; }
        
        // Detail (Latest or Pending)
        public int DetailID { get; set; }
        public int Type { get; set; } // 1, 2, 3
        public int No { get; set; }
        public int OwnerID { get; set; }
        public string OwnerName { get; set; }
        public int ApproverID { get; set; }
        public string ApproverName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public int Status { get; set; } // 0=Chờ duyệt, 1=Đã duyệt, 2=Từ chối
        
        public DateTime? CreatedDate { get; set; }
    }
}


using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ESLTestTableRegistration
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Mã của đơn đăng ký
    /// </summary>
    public string RegistrationCode { get; set; } = null!;

    /// <summary>
    /// ID của bàn test
    /// </summary>
    public int TestTableID { get; set; }

    /// <summary>
    /// Thời gian bắt đầu
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Mã dự án
    /// </summary>
    public string? ProjectCode { get; set; }

    /// <summary>
    /// Tên dự án
    /// </summary>
    public string? RegistrationContent { get; set; }

    /// <summary>
    /// Thời gian tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// ID dự án
    /// </summary>
    public int? ProjectID { get; set; }

    /// <summary>
    /// Trạng thái trả bàn
    /// </summary>
    public bool? IsReturned { get; set; }

    public virtual ICollection<ESLTestTableRegistrationDetail> ESLTestTableRegistrationDetails { get; set; } = new List<ESLTestTableRegistrationDetail>();

    public virtual ICollection<ESLTestTableRegistrationLog> ESLTestTableRegistrationLogs { get; set; } = new List<ESLTestTableRegistrationLog>();

    public virtual ESLTestTable TestTable { get; set; } = null!;
}

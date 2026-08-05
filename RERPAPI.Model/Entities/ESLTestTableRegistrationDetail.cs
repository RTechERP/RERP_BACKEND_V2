using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ESLTestTableRegistrationDetail
{
    /// <summary>
    /// ID tự tăng 
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID của đơn đăng ký master
    /// </summary>
    public int RegistrationID { get; set; }

    /// <summary>
    /// Số thứ tự của đơn đăng ký detail
    /// </summary>
    public int No { get; set; }

    /// <summary>
    ///  1=Đăng ký mới, 2=Gia hạn, 3=Bàn giao
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// Ngày bắt đầu 
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Thời gian trả
    /// </summary>
    public DateTime? ActualReturnDate { get; set; }

    /// <summary>
    /// Người sử dụng bàn test
    /// </summary>
    public int OwnerID { get; set; }

    /// <summary>
    /// Người duyệt
    /// </summary>
    public int ApproverID { get; set; }

    /// <summary>
    /// 0=Chờ duyệt, 1=Đã duyệt, 2=Từ chối
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Ngày duyệt
    /// </summary>
    public DateTime? ApproveDate { get; set; }

    /// <summary>
    /// Ghi chú duyệt
    /// </summary>
    public string? ApproveNote { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Trạng thái xóa
    /// </summary>
    public bool? IsDeleted { get; set; }
}

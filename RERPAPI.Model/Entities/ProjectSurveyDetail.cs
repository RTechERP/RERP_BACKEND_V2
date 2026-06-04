using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectSurveyDetail
{
    public int ID { get; set; }

    public int? ProjectSurveyID { get; set; }

    public int? ProjectTypeID { get; set; }

    /// <summary>
    /// Nhân viên kỹ thuật khảo sát
    /// </summary>
    public int? EmployeeID { get; set; }

    public DateTime? DateSurvey { get; set; }

    /// <summary>
    /// 1: Duyệt; 2: Hủy duyệt
    /// </summary>
    public int? Status { get; set; }

    public string? ReasonCancel { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? LeaderID { get; set; }

    public string? Result { get; set; }

    /// <summary>
    /// Buổi khảo sát (1: Buổi sáng; 2: Buổi chiều)
    /// </summary>
    public int? SurveySession { get; set; }
}

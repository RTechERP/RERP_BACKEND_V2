using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HRHiringCandidateInformationFormForeignLanguageSkill
{
    public int ID { get; set; }

    public int? HRHiringCandidateInformationFormID { get; set; }

    public string? ForeignLanguage { get; set; }

    /// <summary>
    /// 1:Tốt; 2:Khá; 3:Trung bình; 4:Yếu
    /// </summary>
    public int? Listening { get; set; }

    /// <summary>
    /// 1:Tốt; 2:Khá; 3:Trung bình; 4:Yếu
    /// </summary>
    public int? Speaking { get; set; }

    /// <summary>
    /// 1:Tốt; 2:Khá; 3:Trung bình; 4:Yếu
    /// </summary>
    public int? Reading { get; set; }

    /// <summary>
    /// 1:Tốt; 2:Khá; 3:Trung bình; 4:Yếu
    /// </summary>
    public int? Writing { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}

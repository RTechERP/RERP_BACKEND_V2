using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class FilmManagement
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    /// <summary>
    /// 1: Có yêu cầu bắt buộc nhật kết quả; 0: Ko bắt buộc
    /// </summary>
    public bool? RequestResult { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
    
}

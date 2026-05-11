using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class POKHLog
{
    public int ID { get; set; }

    /// <summary>
    /// ID pokh
    /// </summary>
    public int? POKHID { get; set; }

    /// <summary>
    /// Loại log(Created/Update/Delete)
    /// </summary>
    public string? TypeLog { get; set; }

    /// <summary>
    /// Nội dung log
    /// </summary>
    public string? ContentLog { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa
    /// </summary>
    public bool? IsDeleted { get; set; }
}

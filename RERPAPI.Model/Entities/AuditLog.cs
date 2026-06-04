using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AuditLog
{
    public long ID { get; set; }

    public string? TableName { get; set; }

    /// <summary>
    /// 1: Thêm, 2: Sửa, 3: Xóa
    /// </summary>
    public int? Action { get; set; }

    public string? UserName { get; set; }

    public DateTime? ActionDate { get; set; }

    public string? Note { get; set; }
}

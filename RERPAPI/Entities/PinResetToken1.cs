using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class PinResetToken1
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Token { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}

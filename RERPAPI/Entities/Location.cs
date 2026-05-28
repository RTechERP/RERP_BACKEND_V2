using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// bảng vị trí
/// </summary>
public partial class Location
{
    public int ID { get; set; }

    /// <summary>
    /// mã vị trí
    /// </summary>
    public string? LocationCode { get; set; }

    /// <summary>
    /// tên vị trí
    /// </summary>
    public string? LocationName { get; set; }

    public int? ProductGroupID { get; set; }

    public bool? IsDeleted { get; set; }
}

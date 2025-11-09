using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class HistoryError
{
    public long ID { get; set; }

    public long? ProductHistoryID { get; set; }

    public string? DescriptionError { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdateDate { get; set; }
}

using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class DocumentFile
{
    public int ID { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public int? DocumentID { get; set; }

    public string? FileNameOrigin { get; set; }
    public bool? IsDeleted { get; set; }
}

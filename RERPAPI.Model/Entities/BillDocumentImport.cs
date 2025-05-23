﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillDocumentImport
{
    public int ID { get; set; }

    public int? BillImportID { get; set; }

    public int? DocumentImportID { get; set; }

    public int? DocumentStatus { get; set; }

    public DateTime? LogDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public string? Note { get; set; }
}

using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectDocument
{
    public int ID { get; set; }
    public int ProjectID { get; set; }
    public string? Name { get; set; } 
    public byte Type { get; set; }                
    public string? FilePath { get; set; } 
    public string? Version { get; set; }
    public decimal? Size { get; set; }            
    public string? CreateBy { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }          


}

using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Newsletter
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public string? Title { get; set; }

    public string? NewsletterContent { get; set; }

    public int? Type { get; set; }

    public string? Image { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? OriginImgPath { get; set; }

    public string? ServerImgPath { get; set; }
}

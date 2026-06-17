using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TrainingRegistrationDetail
{
    public int ID { get; set; }

    public int? TrainingRegistrationID { get; set; }

    public int? TrainingRegistrationCategoryID { get; set; }

    public string? DescriptionDetail { get; set; }

    public string? Note { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

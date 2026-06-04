using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class MakerTraining
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? DepartmentID { get; set; }

    public int? MakerTrainingTypeID { get; set; }

    public int? FirmID { get; set; }

    public string? TrainerName { get; set; }

    public string? TrainingContent { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    public bool? IsTest { get; set; }

    public string? Location { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}

using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ESLTestTableRegistration
{
    public int ID { get; set; }

    public string RegistrationCode { get; set; } = null!;

    public int TestTableID { get; set; }

    public DateOnly StartDate { get; set; }

    public string? ProjectCode { get; set; }

    public string? RegistrationContent { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<ESLTestTableRegistrationDetail> ESLTestTableRegistrationDetails { get; set; } = new List<ESLTestTableRegistrationDetail>();

    public virtual ICollection<ESLTestTableRegistrationLog> ESLTestTableRegistrationLogs { get; set; } = new List<ESLTestTableRegistrationLog>();

    public virtual ESLTestTable TestTable { get; set; } = null!;
}

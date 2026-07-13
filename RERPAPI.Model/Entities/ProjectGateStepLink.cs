using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities
{
    public class ProjectGateStepLink
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public int ProjectGateStepID { get; set; }
        public int ProjectTypeID { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsRepeat { get; set; }
        public int? ProjectTaskID { get; set; }
        public bool? IsDeleted { get; set; }
    }
}

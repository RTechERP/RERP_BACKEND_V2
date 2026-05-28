using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RERPAPI.Model.Entities.ESL
{
    public class ESLTestTableRegistrationLog
    {
        public int ID { get; set; }
        public int RegistrationID { get; set; }
        public string Action { get; set; }
        public int ActionBy { get; set; }
        public DateTime? ActionDate { get; set; }
        public string Note { get; set; }
        public int? OldStatus { get; set; }
        public int? NewStatus { get; set; }
        public string APIResponse { get; set; }
    }
}

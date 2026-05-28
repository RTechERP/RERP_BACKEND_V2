using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RERPAPI.Model.Entities.ESL
{
    public class ESLTestTableRegistration
    {
        public int ID { get; set; }
        public string RegistrationCode { get; set; }
        public int TestTableID { get; set; }
        public DateTime StartDate { get; set; }
        public string ProjectCode { get; set; }
        public string RegistrationContent { get; set; }

        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}


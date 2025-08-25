using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class TrainingRegistrationApprovedDTO
    {
        public int ID { get; set; }
        public int TrainingRegistrationID { get; set; }
        public int EmployeeApprovedID { get; set; }
        public int EmployeeApprovedActualID { get; set; }
        public int StatusApproved { get; set; }
        public string Note { get; set; }
        public string UnapprovedReason { get; set; }
    }
}
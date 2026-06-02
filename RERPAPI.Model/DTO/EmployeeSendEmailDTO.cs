using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class EmployeeSendEmailDTO : EmployeeSendEmail
    {
        public DateTime? DeadlineFeedbackMail { get; set; }
    }
}
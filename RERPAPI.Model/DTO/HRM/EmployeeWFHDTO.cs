using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace RERPAPI.Model.DTO
{
    public class EmployeeWFHDTO
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int ApprovedID { get; set; }
        public string ApprovedName { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string Reason { get; set; }
        public DateTime DateWFH { get; set; }
        public int TimeWFH { get; set; }
        public string TimeWFHText { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int TotalDay { get; set; }
        public int ApprovedHR { get; set; }
        public bool IsApproved { get; set; }
        public bool IsApprovedHR { get; set; }
        public bool IsApprovedBGD { get; set; }
        public int ApprovedBGDID { get; set; }
        public DateTime? DateApprovedBGD { get; set; }
        public int DecilineApprove { get; set; }
        public string ReasonDeciline { get; set; }
        public string ReasonHREdit { get; set; }
        public bool IsProblem { get; set; }
        public string ContentWork { get; set; }
        public string EvaluateResults { get; set; }
        public bool IsDelete { get; set; }

        public int RowNumber { get; set; }
        public DateTime CreatDay { get; set; }

        public int StatusNumber { get; set; }
        public string StatusText { get; set; }

        public int StatusHRNumber { get; set; }
        public string StatusHRText { get; set; }

        public string FullNameBGD { get; set; }
        public string IsApprovedBGDText { get; set; }
    }
}

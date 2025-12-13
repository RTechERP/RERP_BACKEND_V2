using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class TrackingMarksDataDTO : TrackingMark
    {
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string DocumentTypeName { get; set; }
        public string ApprovedName { get; set; }
        public string EmployeeSignName { get; set; }
        public string ApprovedText { get; set; }
        public string SealNameText { get; set; }
        public string TaxCompanyText { get; set; }
    }
}

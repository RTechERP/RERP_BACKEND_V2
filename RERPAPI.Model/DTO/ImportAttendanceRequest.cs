using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ImportAttendanceRowDto
    {
        public int STT { get; set; }
        public string Code { get; set; } = "";
        public string AttendanceDate { get; set; } = ""; // yyyy-MM-dd
        public string DayWeek { get; set; } = "";
        public string CheckIn { get; set; } = "";        // "HH:mm" or ""
        public string CheckOut { get; set; } = "";       // "HH:mm" or ""
    }

    public class ImportAttendanceRequest
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int OfficeId { get; set; }              // DepartmentID (VD: 11 = HCM)
        public bool Overwrite { get; set; } = false;
        public List<ImportAttendanceRowDto> Rows { get; set; } = new();
    }

    public class ImportResult
    {
        public int Created { get; set; }
        public int Updated { get; set; }
        public int Skipped { get; set; }
        public List<ImportError> Errors { get; set; } = new();
    }

    public class ImportError
    {
        public string Row { get; set; } = "";
        public string Message { get; set; } = "";
    }
}

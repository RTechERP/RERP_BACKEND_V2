using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class ExportExcelDailyReportTechRequest
    {
        /// <summary>
        /// Từ ngày (optional, mặc định là 30 ngày trước)
        /// </summary>
        public DateTime? DateStart { get; set; }

        /// <summary>
        /// Đến ngày (optional, mặc định là ngày hiện tại)
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Team ID (optional, có thể là string với nhiều team phân cách bằng ";")
        /// </summary>
        public string TeamID { get; set; }

        /// <summary>
        /// Tên team (optional, dùng để tạo tên file)
        /// </summary>
        public string TeamName { get; set; }
    }
}

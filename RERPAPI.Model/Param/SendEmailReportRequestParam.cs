using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class SendEmailReportRequestParam
    {
        /// <summary>
        /// <summary>
        /// Nội dung email (có thể là text hoặc HTML)
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Ngày báo cáo (optional, mặc định là ngày hiện tại)
        /// </summary>
        public DateTime? DateReport { get; set; }
    }
}

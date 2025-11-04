using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.Duan.MeetingMinutes
{
    public class MeetingMinutesRequestParam
    {
        public string Keywords { get; set; }
        public int MeetingTypeID { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}

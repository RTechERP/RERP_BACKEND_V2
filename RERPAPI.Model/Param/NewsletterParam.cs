using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class NewsletterParam
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Keyword { get; set; } = "";
        public int TypeId { get; set; } = 0;
    }
}

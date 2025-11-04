using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectReportDto
    {
        public string ProjectType { get; set; }
        public int Count { get; set; }
        public decimal Value { get; set; }
    }

}
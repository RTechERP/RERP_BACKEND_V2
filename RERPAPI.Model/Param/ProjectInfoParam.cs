using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class ProjectInfoParam
    {
        public int ID { get; set; }

        public string? Name { get; set; }

        public string? Icon { get; set; }

        public string? Color { get; set; }

        public int? GroupProject { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? Priority { get; set; }

        public List<int>? projectInforEmployees { get; set; }
    }
}

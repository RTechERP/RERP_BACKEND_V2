using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.HRM
{
    public class OrganizationalChartDTO
    {
        public List<OrganizationalChart>? organizationalCharts { get; set; }
        public List<OrganizationalChartDetail>? organizationalChartDetails { get; set; }
    }
}

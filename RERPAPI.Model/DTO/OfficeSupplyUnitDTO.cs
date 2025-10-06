using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class OfficeSupplyUnitDTO: OfficeSupplyUnit
    {
        public int ID { get; set; }

        public string Name { get; set; }
    }
}

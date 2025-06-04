using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class CustomerSpecializationDTO {
        public int ID { get; set; }

        public int? STT { get; set; }

        public string? Code { get; set; }

        public string? Name { get; set; }
    }
    
}

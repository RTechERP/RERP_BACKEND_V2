using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.HRM
{
    public class VehicleBookingFileImageDTO : VehicleBookingFile
    {
        public DateTime? CreatdDate { get; set; }
        public DateTime? TimeNeedPresent { get; set; }
        public string? urlImage { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPhoneNumber { get; set; }
        public string? PackageName { get; set; }
        public string? SpecificDestinationAddress { get; set; }
        public string? Title { get; set; }

    }
}

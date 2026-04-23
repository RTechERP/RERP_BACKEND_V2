using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.MobileDTO
{
    public class UserMobileDTO: User
    {
        public string? FcmToken { get; set; }
        public string? DeviceID { get; set; }
    }
}

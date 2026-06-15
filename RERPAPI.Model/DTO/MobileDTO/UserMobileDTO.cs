using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.MobileDTO
{
    public class UserMobileDTO : User
    {
        public string? FcmToken { get; set; }
        public string? DeviceID { get; set; }
    }
}
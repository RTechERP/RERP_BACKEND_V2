using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.HRM
{
    public class BookingRoomDTO
    {
        public DateTime? AllDate { get; set; }
        public string DayOfWeek { get; set; }
        public int MeetingRoomID { get; set; }
        public DateTime? DateRegister { get; set; }
        public string EightAM { get; set; }
        public string EightHalfAM { get; set; }
        public string NineAM { get; set; }
        public string NineHalfAM { get; set; }
        public string TenAM { get; set; }
        public string TenHalfAM { get; set; }
        public string ElevenAM { get; set; }
        public string ElevenHalfAM { get; set; }
        public string TwelveAM { get; set; }
        public string TwelveHalfAM { get; set; }
        public string OnePM { get; set; }
        public string OneHalfPM { get; set; }
        public string TwoPM { get; set; }
        public string TwoHalfPM { get; set; }
        public string ThreePM { get; set; }
        public string ThreeHalfPM { get; set; }
        public string FourPM { get; set; }
        public string FourHalfPM { get; set; }
        public string FivePM { get; set; }
        public string FiveHalfPM { get; set; }
    }
}

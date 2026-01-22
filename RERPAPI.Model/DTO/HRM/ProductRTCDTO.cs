using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.HRM
{
    public class ProductRTCDTO : ProductRTC
    {
        public decimal InventoryReal { get; set; }
        //public string AddressBox { get; set; }
        public decimal CoordinatesX { get; set; }
        public decimal CoordinatesY { get; set; }
        public string LocationName { get; set; }
        public int STT { get; set; }
        public int HistortyID { get; set; }
        public DateTime? DateReturn { get; set; }
        public DateTime? DateReturnExpected { get; set; }
        public DateTime? DateBorrow { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public int StatusNew { get; set; }
        public string StatusText { get; set; }
        public int PeopleID { get; set; }
        public int LocationType { get; set; }
    }
}

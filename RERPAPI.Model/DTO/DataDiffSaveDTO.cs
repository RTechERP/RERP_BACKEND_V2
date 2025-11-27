using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class DataDiffSaveDTO
    {
        public int ID { get; set; }

        // Theo Partlist
        public string? Maker { get; set; }
        public string? Unit { get; set; }
        public string? ProductName { get; set; }

        // Theo kho (Stock)
        public string? MakerStock { get; set; }
        public string? UnitStock { get; set; }
        public string? ProductNameStock { get; set; }
    }
}

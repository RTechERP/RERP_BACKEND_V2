using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class PartlistDiffDTO
    {

        public int ProductSaleId { get; set; }
        public string ProductCode { get; set; } = string.Empty;

        public string GroupMaterialPartlist { get; set; } = string.Empty;
        public string GroupMaterialStock { get; set; } = string.Empty;

        public string ManufacturerPartlist { get; set; } = string.Empty;
        public string ManufacturerStock { get; set; } = string.Empty;

        public string UnitPartlist { get; set; } = string.Empty;
        public string UnitStock { get; set; } = string.Empty;

        public bool IsFix { get; set; }
        public string Choose { get; set; } = "Excel"; // "Excel" | "Stock"
    }
}

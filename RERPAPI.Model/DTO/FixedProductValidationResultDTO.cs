using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class FixedProductValidationResultDTO
    {
        public bool HasMismatch { get; set; }
        public List<string> MismatchedFields { get; set; } = new List<string>();
        public ProductSale FixedProductData { get; set; }
    }
}

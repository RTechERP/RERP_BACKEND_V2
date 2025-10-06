using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
   
    public class ProductsSaleDTO
    {
        public ProductSale ProductSale { get; set; }
        public Inventory? Inventory { get; set; }

     

    }
}

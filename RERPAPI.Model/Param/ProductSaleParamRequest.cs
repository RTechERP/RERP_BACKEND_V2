using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class ProductSaleParamRequest
    {
        public int id { get; set; }
        public string? find { get; set;}
        public bool checkedAll { get; set;}
    }
}

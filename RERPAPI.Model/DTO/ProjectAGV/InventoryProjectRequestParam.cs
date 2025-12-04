using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.ProjectAGV
{
    public class InventoryProjectRequestParam
    {
        public string? KeyWord { get; set; }      
        public int? ProjectID { get; set; }        
        public int? EmployeeID { get; set; }        
        public int? ProductSaleID { get; set; }              
    
    }
}

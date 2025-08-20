using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Asset
{
    public class AssetAllocationDetailExportDto
    {
        public int ID { get; set; }
        public int TSAssetAllocationID { get; set; }
        public int AssetManagementID { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }

        public string TSAssetName { get; set; }
        public string TSCodeNCC { get; set; }
        public string UnitName { get; set; }

        public string FullName { get; set; }           // Người sử dụng
        public string DepartmentName { get; set; }     // Phòng ban sử dụng
        public string PositionName { get; set; }       // Chức vụ người sử dụng
    }

}

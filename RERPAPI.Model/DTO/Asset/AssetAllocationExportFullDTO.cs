using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Asset
{
    public class AssetAllocationExportFullDto
    {
        public AssetAllocationExportDto Master { get; set; }
        public List<AssetAllocationDetailExportDto> Details { get; set; }
    }

}

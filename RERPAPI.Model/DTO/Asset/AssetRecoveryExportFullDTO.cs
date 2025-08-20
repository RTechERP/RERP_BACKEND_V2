using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Asset
{
    public class AssetRecoveryExportFullDto
    {
        public AssetRecoveryExportDto Master { get; set; }
        public List<AssetRecoveryDetailExportDto> Details { get; set; }
    }

}

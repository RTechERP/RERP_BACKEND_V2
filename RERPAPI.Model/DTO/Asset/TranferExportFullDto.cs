using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Asset
{
    public class TranferExportFullDto
    {
        public TranferAssetExportDto Master { get; set; }
        public List<TranferAssetDetailExportDto> Details { get; set; }
    }

}

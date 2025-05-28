using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    internal class TSSourceAssetDTO
    {
        public int ID { get; set; }

        public string? SourceCode { get; set; }

        public string? SourceName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? UpdatedBy { get; set; }
    }
}

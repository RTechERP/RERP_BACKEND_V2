using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Asset
{
    public class TranferAssetExportDto
    {
        public int ID { get; set; }
        public string CodeReport { get; set; }
        public DateTime TranferDate { get; set; }
        public string DeliverName { get; set; }
        public string PossitionDeliver { get; set; }
        public string DepartmentDeliver { get; set; }
        public string ReceiverName { get; set; }
        public string PossitionReceiver { get; set; }
        public string DepartmentReceiver { get; set; }
        public string Reason { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DateApprovedPersonalProperty { get; set; }
        public DateTime? DateApprovedHR { get; set; }
    }

}

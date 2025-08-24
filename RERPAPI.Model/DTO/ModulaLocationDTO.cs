using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ModulaLocationDTO : ModulaLocation
    {
        public List<dynamic> ModulaLocationDetails { get; set; } = new List<dynamic>();
        public List<ModulaLocationDetail> LocationDetails { get; set; } = new List<ModulaLocationDetail>();

        public class SerialNumberModulaLocation: HistoryProductRTC
        {
            public int ModulaLocationDetailID { get; set; }
            public int BillImportDetailID { get; set; }
            public int BillExportDetailID { get; set; }
            public string Name { get; set; } = "";
            public string SerialNumber { get; set; } = "";
            public int Quantity { get; set; }
            public int BillImportDetailTechnicalID { get; set; }
            public int BillExportDetailTechnicalID { get; set; }

            /// <summary>
            /// 1: Phiếu nhập
            /// 2: Phiếu xuất
            /// 3: Phiếu mượn
            /// </summary>
            public int BillType { get; set; }
            public int HistoryProductRTCID { get; set; }
        }


        public class CallModula
        {
            /// <summary>
            /// Mã tray
            /// </summary>
            public string Code { get; set; } = string.Empty;

            /// <summary>
            /// Thông tin sản phẩm,...
            /// </summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// Tọa độ X của vị trí
            /// </summary>
            public int AxisX { get; set; } = 0;


            /// <summary>
            /// Tọa độ Y của vị trí
            /// </summary>
            public int AxisY { get; set; } = 0;
        }
    }
}

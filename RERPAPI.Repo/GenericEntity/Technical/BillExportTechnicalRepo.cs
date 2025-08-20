using RERPAPI.Model.Entities;
using RERPAPI.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTCApi.Repo.GenericRepo
{
    public class BillExportTechnicalRepo : GenericRepo<BillExportTechnical>
    {
        public string GetBillCode(int billtype)
        {
            string billCode = "";
            DateTime billDate = DateTime.Now;

            string preCode = "PXKD";
       
            List<BillExportTechnical> billExports = GetAll().Where(x => (x.Code ?? "").Contains(billDate.ToString("yyMMdd"))).ToList();

            var listCode = billExports.Select(x => new
            {
                ID = x.ID,
                Code = x.Code,
                STT = string.IsNullOrWhiteSpace(x.Code) ? 0 : Convert.ToInt32(x.Code.Substring(x.Code.Length - 3)),
            }).ToList();

            string numberCodeText = "000";
            //string lastBillCode = string.IsNullOrWhiteSpace(billExport.Code) ? $"{preCode}{billDate.ToString("yyMMdd")}{numberCodeText}" : billExport.Code.Trim();
            //int numberCode = Convert.ToInt32(lastBillCode.Substring(lastBillCode.Length - 3));
            int numberCode = listCode.Count <= 0 ? 0 : listCode.Max(x => x.STT);
            numberCodeText = (++numberCode).ToString();
            while (numberCodeText.Length < 3)
            {
                numberCodeText = "0" + numberCodeText;
            }

            billCode = $"{preCode}{billDate.ToString("yyMMdd")}{numberCodeText}";

            return billCode;
        }
    }
}

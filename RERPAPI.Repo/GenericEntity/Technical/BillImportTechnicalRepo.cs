using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTCApi.Repo.GenericRepo
{
    public class BillImportTechnicalRepo : GenericRepo<BillImportTechnical>
    {
        public BillImportTechnicalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public string GetBillCode(int billtype)
        {
            string billCode = "";

            DateTime billDate = DateTime.Now;

            string preCode = "PNKD";
            if (billtype == 3) preCode = "PTD";

            //BillImportTechnical billImport = GetAll().Where(x => (x.CreatedDate ?? DateTime.MinValue).Year == billDate.Year &&
            //                                                     (x.CreatedDate ?? DateTime.MinValue).Month == billDate.Month &&
            //                                                     (x.CreatedDate ?? DateTime.MinValue).Day == billDate.Day)
            //                                .OrderByDescending(x => x.ID)
            //                                .FirstOrDefault() ?? new BillImportTechnical();
            //string code = preCode + billDate.ToString("yyMMdd");
            List<BillImportTechnical> billImports = GetAll().Where(x => (x.BillCode ?? "").Contains(billDate.ToString("yyMMdd"))).ToList();

            var listCode = billImports.Select(x => new
            {
                ID = x.ID,
                Code = x.BillCode,
                STT = string.IsNullOrWhiteSpace(x.BillCode) ? 0 : Convert.ToInt32(x.BillCode.Substring(x.BillCode.Length - 3)),
            }).ToList();

            string numberCodeText = "000";
            //string lastBillCode = string.IsNullOrWhiteSpace(billImport.BillCode) ? $"{preCode}{billDate.ToString("yyMMdd")}{numberCodeText}" : billImport.BillCode.Trim();
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

using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.AddNewBillExport
{
    public class BillExportRepo : GenericRepo<BillExport>
    {
        public BillExportRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public string GetBillCode(int billtype)
        {
            string billCode = "";
            DateTime billDate = DateTime.Now;
            //DateTime billDate = new DateTime(2024, 07, 18);

            string preCode = "PXK";
            if (billtype == 0 || billtype == 7) preCode = "PM";
            else if (billtype == 3) preCode = "PCT";
            else if (billtype == 4) preCode = "PMNB";
            else if (billtype == 5) preCode = "PXM";
            else preCode = "PXK";

            //BillExport billExport = GetAll().Where(x => (x.CreatedDate ?? DateTime.MinValue).Year == billDate.Year &&
            //                                                     (x.CreatedDate ?? DateTime.MinValue).Month == billDate.Month &&
            //                                                     (x.CreatedDate ?? DateTime.MinValue).Day == billDate.Day)
            //                                .OrderByDescending(x => x.ID)
            //                                .FirstOrDefault() ?? new BillExport();

            //var billExports = GetAll().Where(x => (x.CreatedDate ?? DateTime.MinValue).Year == billDate.Year &&
            //                                        (x.CreatedDate ?? DateTime.MinValue).Month == billDate.Month &&
            //                                        (x.CreatedDate ?? DateTime.MinValue).Day == billDate.Day);

            //string code = preCode + billDate.ToString("yyMMdd");
            List<BillExport> billExports = GetAll().Where(x => (x.Code ?? "").Contains(billDate.ToString("yyMMdd"))).ToList(); //Lee Min Khoi 16/07/2024

            var listCode = billExports.Select(x => new
            {
                ID = x.ID,
                Code = x.Code,
                STT = string.IsNullOrWhiteSpace(x.Code) ? 0 : Convert.ToInt32(x.Code.Substring(x.Code.Length - 3)),
            }).ToList();

            //var lastBillCodes = listCode.Count <= 0 ? 0 : listCode.Max(x => x.STT);

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

using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo;
using RERPAPI.Repo.GenericEntity;

namespace RTCApi.Repo.GenericRepo
{
    public class BillImportTechnicalRepo : GenericRepo<BillImportTechnical>
    {
        BillImportTechnicalLogRepo _BillImportTechnicalLog;
        CurrentUser _currentUser;
        public BillImportTechnicalRepo(CurrentUser currentUser, BillImportTechnicalLogRepo billImportTechnicalLogRepo) : base(currentUser)
        {
            _BillImportTechnicalLog = billImportTechnicalLogRepo;
            _currentUser = currentUser;
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
            List<BillImportTechnical> billImports = GetAll(x => x.IsDeleted == false).Where(x => (x.BillCode ?? "").Contains(billDate.ToString("yyMMdd"))).ToList();

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
        public async Task UpdateStatusAsync(BillImportTechnical bill, bool status)
        {
            // 1. Cập nhật thông tin phiếu
            bill.Status = status;
            bill.UpdatedBy = _currentUser.LoginName;
            bill.UpdatedDate = DateTime.Now;

            // Entity Framework sẽ tự track bill vì chúng ta đã Get nó ra trước đó, 
            // hoặc bạn có thể gọi Update rõ ràng:
            await UpdateAsync(bill);

            // 2. Ghi Log
            var log = new BillImportTechnicalLog
            {
                BillImportTechnicalID = bill.ID,
                StatusBill = status, // true hoặc false tùy tham số truyền vào
                DateStatus = DateTime.Now,
            };

            await _BillImportTechnicalLog.CreateAsync(log);
        }
    }
}

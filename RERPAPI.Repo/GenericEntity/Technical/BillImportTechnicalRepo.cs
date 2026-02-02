using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo;
using RERPAPI.Repo.GenericEntity;

namespace RTCApi.Repo.GenericRepo
{
    public class BillImportTechnicalRepo : GenericRepo<BillImportTechnical>
    {
        BillImportTechnicalLogRepo _BillImportTechnicalLog;
        ProductRTCRepo _productRTCRepo;
        CurrentUser _currentUser;
        public BillImportTechnicalRepo(CurrentUser currentUser, BillImportTechnicalLogRepo billImportTechnicalLogRepo, ProductRTCRepo productRTCRepo) : base(currentUser)
        {
            _BillImportTechnicalLog = billImportTechnicalLogRepo;
            _currentUser = currentUser;
            _productRTCRepo = productRTCRepo;
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
        public async Task UpdateHistoryProductRTC(BillimporttechnicalFullDTO dTO)
        {
            bool values = true;

            foreach (var item in dTO.billImportDetailTechnicals)
            {
                int ID = item.HistoryProductRTCID ?? 0;
                if (ID <= 0) continue;
                DateTime dateTime = DateTime.Now;
                SQLHelper<object>.ExcuteProcedure("spUpdateHistoryProductRTC", new string[] { "@HistoryProductRTCID", "@Values", "@DateReturn" }, new object[] { ID, values, dateTime.ToString("yyyy-MM-dd HH:mm:ss") });

                int ProductRTCQRCodeID = item.ProductRTCQRCodeID ?? 0;
                if (ProductRTCQRCodeID > 0)
                {
                    if (values)
                    {
                        //Status=1 Trong Kho
                        SQLHelper<object>.ExcuteProcedure("spUpdateStatusProductRTCQRCode", new string[] { "@ProductRTCQRCodeID", "@Status" }, new object[] { ProductRTCQRCodeID, 1 });
                    }
                    else
                    {
                        //Status=3 Đã xuất
                        SQLHelper<object>.ExcuteProcedure("spUpdateStatusProductRTCQRCode", new string[] { "@ProductRTCQRCodeID", "@Status" }, new object[] { ProductRTCQRCodeID, 3 });
                    }
                }

            }
            foreach (var item in dTO.billImportDetailTechnicals)
            {
                int ProductID = item.ProductID ?? 0;
                if (ProductID <= 0) continue;
                ProductRTC productRTCModel = _productRTCRepo.GetByID(ProductID);
                if (productRTCModel != null)
                {
                    productRTCModel.Note += " - " + item.Note;
                    await _productRTCRepo.UpdateAsync(productRTCModel);
                }
            }
        }

    }
}

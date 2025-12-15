using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillImportRepo : GenericRepo<BillImport>
    {
        DocumentImportRepo _documentImportRepo;
        CurrentUser _currentUser;
        public BillImportRepo(CurrentUser currentUser, DocumentImportRepo documentImportRepo) : base(currentUser)
        {
            _documentImportRepo = documentImportRepo;
            _currentUser = currentUser;
        }
        #region lấy mã phiếu nhập
        public string GetBillCode(int billtype)
        {
            string billCode = "";

            DateTime billDate = DateTime.Now;

            string preCode = "";
            if (billtype == 0 || billtype == 4) preCode = "PNK";
            else if (billtype == 1) preCode = "PT";
            else if (billtype == 3) preCode = "PNM";
            else preCode = "PTNB";

            //BillImport billImport = GetAll().Where(x => (x.CreatDate ?? DateTime.MinValue).Year == billDate.Year &&
            //                                                     (x.CreatDate ?? DateTime.MinValue).Month == billDate.Month &&
            //                                                     (x.CreatDate ?? DateTime.MinValue).Day == billDate.Day)
            //                                .OrderByDescending(x => x.ID)
            //                                .FirstOrDefault() ?? new BillImport();
            //string code = billDate.ToString("yyMMdd");
            List<BillImport> billImports = GetAll(x => (x.BillImportCode ?? "").Contains(billDate.ToString("yyMMdd")) && x.IsDeleted == false).ToList();

            var listCode = billImports.Select(x => new
            {
                ID = x.ID,
                Code = x.BillImportCode,
                STT = string.IsNullOrWhiteSpace(x.BillImportCode) ? 0 : Convert.ToInt32(x.BillImportCode.Substring(x.BillImportCode.Length - 3)),
            }).ToList();

            string numberCodeText = "000";
            //string lastBillCode = string.IsNullOrWhiteSpace(billImport.BillImportCode) ? $"{preCode}{billDate.ToString("yyMMdd")}{numberCodeText}" : billImport.BillImportCode.Trim();
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
        #endregion

        #region lưu dữ liệu phiếu nhập
        public async Task<int> SaveBillImport(BillImport billImport)
        {
            if (billImport == null) return 0;
            if (billImport.ID > 0)
            {
                billImport.UpdatedDate = DateTime.Now;
                await UpdateAsync(billImport);
            }
            else
            {
                billImport.CreatDate = DateTime.Now;
                billImport.IsDeleted = false;
                await CreateAsync(billImport);
                await _documentImportRepo.NewDocumentImport(billImport.ID);
            }
            return billImport.ID;
        }
        #endregion
        public bool ApproveDocument(List<BillImportApproveDocumentDTO> models, bool status, out string message)
        {
            message = "";
            if (models == null || models.Count <= 0)
            {
                message = "Vui lòng chọn phiếu muốn cập nhật trạng thái!";
                return false;
            }

            int total = models.Count;
            int updated = 0;
            int skipped = 0;

            foreach (var model in models)
            {
                if (model.ID <= 0)
                {
                    skipped++;
                    continue;
                }

                // Chỉ user nhận chứng từ hoặc admin mới được duyệt
                if (model.DoccumentReceiverID != _currentUser.ID && !_currentUser.IsAdmin)
                {
                    skipped++;
                    continue;
                }

                try
                {
                    model.StatusDocumentImport = status;
                    model.UpdatedBy = _currentUser.LoginName;
                    model.UpdatedDate = DateTime.Now;

                    Update(model);
                    updated++;
                }
                catch (Exception ex)
                {
                    skipped++;
                }
            }

            if (updated == 0)
            {
                message = "Không có phiếu nào được cập nhật!";
                return false;
            }

            message = $"Cập nhật thành công {updated}/{total} phiếu. Bỏ qua {skipped} phiếu không hợp lệ hoặc không có quyền!";
            return true;
        }

    }
}

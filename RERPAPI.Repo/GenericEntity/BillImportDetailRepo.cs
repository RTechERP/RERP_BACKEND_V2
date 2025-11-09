
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.Technical;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillImportDetailRepo : GenericRepo<BillImportDetail>
    {
        InvoiceLinkRepo _invoicelinkrepo;
        BillImportDetailSerialNumberRepo _billImportDetailSerialNumberRepo ;
        InventoryProjectRepo _inventoryProjectRepo;
        public BillImportDetailRepo(CurrentUser currentUser, InvoiceLinkRepo invoicelinkrepo, BillImportDetailSerialNumberRepo billImportDetailSerialNumberRepo, InventoryProjectRepo inventoryProjectRepo) : base(currentUser)
        {
            _invoicelinkrepo = invoicelinkrepo;
            _billImportDetailSerialNumberRepo = billImportDetailSerialNumberRepo;
            _inventoryProjectRepo = inventoryProjectRepo;
        }




        #region lưu dữ liệu chi tiết phiếu nhập
        public async Task SaveBillImportDetail(List<BillImportDetail> details, int billImportId)
        {
            if (details == null || details.Count == 0) return;

            var grouped = details
                .GroupBy(d => d.ProductID)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Qty ?? 0));

            foreach (var detail in details)
            {
                detail.BillImportID = billImportId;
                if (grouped.TryGetValue(detail.ProductID, out decimal total)) detail.TotalQty = total;

                // xử lý lưu chi tiết
                if (detail.ID > 0)
                {
                    detail.UpdatedDate = DateTime.Now;
                    await UpdateAsync(detail);
                }
                else
                {
                    detail.CreatedDate = DateTime.Now;
                    detail.IsDeleted = false;
                    await CreateAsync(detail);
                }
                // lưu serial number
                await SaveSerialNumberForDetail(detail);
                // xử lý tồn kho dự án
                await _inventoryProjectRepo.UpdateInventoryProject(detail);

                // xử lý liên kết hóa đơn
                await _invoicelinkrepo.InvoiceLinkForBillImport(detail);
                // Cập nhật trạng thái
                SQLHelper<dynamic>.ProcedureToList("spUpdateReturnedStatusForBillExportDetail",
                    new string[] { "@BillImportID", "@Approved" },
                    new object[] { detail.BillImportID ?? billImportId, 0 });
                //var listDetails = SQLHelper<BillImportDetail>.FindByAttribute("BillImportID", detail.BillImportID ?? billImportId);
                var listDetails = new List<BillImportDetail>();
                string poNCCDetailID = string.Join(",", listDetails.Select(x => x.PONCCDetailID));
                SQLHelper<dynamic>.ExcuteProcedure("spUpdateStatusPONCC",
                    new string[] { "@PONCCDetailID" },
                    new object[] { poNCCDetailID });
            }
        }
        #endregion

        #region xóa chi tiết phiếu nhập
        public async Task DeleteBillImportDetail(List<int> DeletedDetailIDs)
        {
            if (DeletedDetailIDs == null || DeletedDetailIDs.Count == 0) return;

            foreach (var id in DeletedDetailIDs)
            {
                BillImportDetail detail = GetByID(id);
                if (detail != null)
                {
                    detail.IsDeleted = true;
                    detail.UpdatedDate = DateTime.Now;
                    Update(detail);
                    var invoiceLink = _invoicelinkrepo.GetAll().Where(p => p.BillImportDetailID == id);
                    if (invoiceLink.Any())
                    {
                        await _invoicelinkrepo.DeleteByAttributeAsync("BillImportDetailID", id);
                    }
                }
            }
        }
        #endregion

        #region hàm lưu serial number
        public async Task SaveSerialNumberForDetail(BillImportDetail detail)
        {
            if (string.IsNullOrEmpty(detail.SerialNumber)) return;

            //  Parse danh sách ID
            var serialIds = detail.SerialNumber
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s.Trim()))
                .ToList();

            //  Lấy danh sách serial chưa gán chi tiết nào
            var serials = _billImportDetailSerialNumberRepo.GetAll(x => serialIds.Contains(x.ID) && x.IsDeleted != true && x.BillImportDetailID == null);

            //  Kiểm tra số lượng khớp Qty
            if (detail.Qty.HasValue && serials.Count != (int)detail.Qty)
                throw new Exception($"Số serial ({serials.Count}) không khớp Qty ({detail.Qty}) cho sản phẩm {detail.ProductID}");

            //  Nếu SerialNumber trong detail đang rỗng → gán chuỗi serial thực tế
            if (string.IsNullOrEmpty(detail.SerialNumber))
            {
                detail.SerialNumber = serials.Any()
                    ? string.Join(",", serials.Select(s => s.SerialNumber))
                    : null;

                await UpdateAsync(detail); // hoặc inject repo nếu có DI
            }

            // Cập nhật lại serial trong DB
            foreach (var serial in serials)
            {
                serial.BillImportDetailID = detail.ID;
                serial.UpdatedDate = DateTime.Now;
                await _billImportDetailSerialNumberRepo.UpdateAsync(serial);
            }
        }

        #endregion
    }
}

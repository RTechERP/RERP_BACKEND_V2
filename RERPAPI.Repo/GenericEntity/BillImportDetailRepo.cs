
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.Technical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillImportDetailRepo : GenericRepo<BillImportDetail>
    {
        InvoiceLinkRepo _invoicelinkrepo = new InvoiceLinkRepo();
        BillImportDetailSerialNumberRepo _billImportDetailSerialNumberRepo = new BillImportDetailSerialNumberRepo();
        InventoryProjectRepo _inventoryProjectRepo = new InventoryProjectRepo();

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
                await _billImportDetailSerialNumberRepo.SaveSerialNumberForDetail(detail);
                // xử lý tồn kho dự án
                await _inventoryProjectRepo.UpdateInventoryProject(detail);

                // xử lý liên kết hóa đơn
                await _invoicelinkrepo.InvoiceLinkForBillImport(detail);
                // Cập nhật trạng thái
                SQLHelper<dynamic>.ExcuteScalar("spUpdateReturnedStatusForBillExportDetail",
                    new string[] { "@BillImportID", "@Approved" },
                    new object[] { detail.BillImportID ?? billImportId, 0 });
                var listDetails = SQLHelper<BillImportDetail>.FindByAttribute("BillImportID", detail.BillImportID ?? billImportId);
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
    }
}

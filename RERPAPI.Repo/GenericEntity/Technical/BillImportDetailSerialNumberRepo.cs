using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class BillImportDetailSerialNumberRepo:GenericRepo<BillImportDetailSerialNumber>
    {
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
            var serials = GetAll(x => serialIds.Contains(x.ID) && x.IsDeleted != true && x.BillImportDetailID == null);

            //  Kiểm tra số lượng khớp Qty
            if (detail.Qty.HasValue && serials.Count != (int)detail.Qty)
                throw new Exception($"Số serial ({serials.Count}) không khớp Qty ({detail.Qty}) cho sản phẩm {detail.ProductID}");

            //  Nếu SerialNumber trong detail đang rỗng → gán chuỗi serial thực tế
            if (string.IsNullOrEmpty(detail.SerialNumber))
            {
                detail.SerialNumber = serials.Any()
                    ? string.Join(",", serials.Select(s => s.SerialNumber))
                    : null;

                await new BillImportDetailRepo().UpdateAsync(detail); // hoặc inject repo nếu có DI
            }

            // Cập nhật lại serial trong DB
            foreach (var serial in serials)
            {
                serial.BillImportDetailID = detail.ID;
                serial.UpdatedDate = DateTime.Now;
                await UpdateAsync(serial);
            }
        }

        #endregion
    }
}

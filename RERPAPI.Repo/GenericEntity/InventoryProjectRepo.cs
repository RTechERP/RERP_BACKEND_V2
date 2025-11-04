using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class InventoryProjectRepo:GenericRepo<InventoryProject>
    {
        #region xử lý tồn kho dự án
        public async Task UpdateInventoryProject(BillImportDetail detail)
        {
            if (detail.ProjectID <= 0) return;
            // Xử lý tồn kho dự án
            if (detail.ProjectID > 0)
            {
                var inv = GetAll().FirstOrDefault(p => p.ProjectID == detail.ProjectID &&
                                                       p.ProductSaleID == detail.ProductID &&
                                                       p.IsDeleted != true);
                if (inv != null)
                {
                    //inv.Quantity = (inv.Quantity ?? 0) + (detail.Qty ?? 0);
                    //inv.UpdatedDate = DateTime.Now;
                    //await UpdateAsync(inv);
                    inv.Quantity += detail.Qty;
                    await UpdateAsync(inv);
                }
                else
                {
                    await CreateAsync(new InventoryProject
                    {
                        ProjectID = detail.ProjectID,
                        ProductSaleID = detail.ProductID,
                        Quantity = detail.Qty,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    });
                }
            }
        }
        #endregion
    }
}

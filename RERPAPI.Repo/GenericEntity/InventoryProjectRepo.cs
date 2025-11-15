using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class InventoryProjectRepo : GenericRepo<InventoryProject>
    {
        ProjectPartlistPriceRequestRepo _projectPartlistPriceRequestRepo;
        BillImportRepo _billImportRepo;
        public InventoryProjectRepo(CurrentUser currentUser, ProjectPartlistPriceRequestRepo projectPartlistPriceRequestRepo, BillImportRepo billImportRepo) : base(currentUser)
        {
            _projectPartlistPriceRequestRepo = projectPartlistPriceRequestRepo;
            _billImportRepo = billImportRepo;
        }
        #region xử lý tồn kho dự án
        public async Task<int> UpdateInventoryProject(BillImportDetail detail)
        {
            if (detail.ProjectID <= 0) return 0;

            int? pokhDetailId = null;
            if (detail.ProjectPartListID.HasValue && detail.ProjectPartListID.Value > 0)
            {
                var ppl = _projectPartlistPriceRequestRepo.GetByID(detail.ProjectPartListID.Value);
                if (ppl != null)
                {
                    pokhDetailId = ppl.POKHDetailID;
                }
            }

            var billImport = _billImportRepo.GetByID(detail.BillImportID ?? 0);
            int warehouseId = billImport?.WarehouseID ?? 0;

            var root = GetAll().FirstOrDefault(p =>
                p.ProjectID == detail.ProjectID &&
                p.ProductSaleID == detail.ProductID &&
                p.ParentID == 0 &&
                p.IsDeleted != true &&
                ((pokhDetailId ?? 0) == 0
                    ? (p.POKHDetailID == null || p.POKHDetailID == 0)
                    : p.POKHDetailID == pokhDetailId));

            if (root == null)
            {
                root = new InventoryProject
                {
                    ProjectID = detail.ProjectID,
                    ProductSaleID = detail.ProductID,
                    WarehouseID = warehouseId,
                    Quantity = detail.Qty,
                    QuantityOrigin = detail.Qty,
                    POKHDetailID = pokhDetailId,
                    Note = detail.ProjectName,
                    IsDeleted = false,
                    ParentID = 0,
                    CreatedDate = DateTime.Now
                };
                await CreateAsync(root);
            }
            else
            {
                root.Quantity = (root.Quantity ?? 0) + (detail.Qty ?? 0);
                root.UpdatedDate = DateTime.Now;
                await UpdateAsync(root);
            }

            return root.ID;
        }
        #endregion
    }
}

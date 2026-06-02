using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class InventoryRepo : GenericRepo<Inventory>
    {
        public InventoryRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        #region kiểm tra tồn kho cho phiếu nhập

        public async Task CheckInventoryForImport(List<BillImportDetail> billImportDetail, BillImport billImport)
        {
            foreach (var detail in billImportDetail)
            {
                bool exists = GetAll().Any(x => x.WarehouseID == billImport.WarehouseID && x.ProductSaleID == detail.ProductID);// && x.ProductGroupID == billImport.KhoTypeID);
                if (!exists)
                {
                    Inventory inventory = new Inventory
                    {
                        WarehouseID = billImport.WarehouseID,
                        ProductSaleID = detail.ProductID,
                        //ProductGroupID = billImport.KhoTypeID,
                        TotalQuantityFirst = 0,
                        TotalQuantityLast = 0,
                        Import = 0,
                        Export = 0
                    };
                    await CreateAsync(inventory);
                }
            }
        }

        #endregion kiểm tra tồn kho cho phiếu nhập
    }
}
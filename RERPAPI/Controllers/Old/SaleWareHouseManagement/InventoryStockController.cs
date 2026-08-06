using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Runtime.CompilerServices;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryStockController : ControllerBase
    {
        private InventoryStockRepo _inventoryStockRepo;
        private ProductGroupRepo _productGroupRepo;
        private WarehouseRepo _warehouseRepo;
        private ProjectTypeRepo _projectTypeRepo;
        private ProductSaleRepo _productSaleRepo;
        private InventoryStockLogRepo _inventoryStockLogRepo;

        public InventoryStockController(
            ProductGroupRepo productGroupRepo,
            WarehouseRepo warehouseRepo,
            ProjectTypeRepo projectTypeRepo,
            InventoryStockRepo inventoryStockRepo,
            ProductSaleRepo productSaleRepo,
            InventoryStockLogRepo inventoryStockLogRepo
            )
        {
            _productGroupRepo = productGroupRepo;
            _warehouseRepo = warehouseRepo;
            _projectTypeRepo = projectTypeRepo;
            _inventoryStockRepo = inventoryStockRepo;
            _productSaleRepo = productSaleRepo;
            _inventoryStockLogRepo = inventoryStockLogRepo;
        }

        [HttpGet("project-type")]
        public async Task<IActionResult> GetProjectType()
        {
            try
            {
                var data = _projectTypeRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("warehouse")]
        public async Task<IActionResult> GetWarehouse()
        {
            try
            {
                var data = _warehouseRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("product-sale")]
        public async Task<IActionResult> GetProductSale(int warehouseId)
        {
            try
            {
                var param = new { WarehouseID = warehouseId };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetAllProductSaleByWareHouseID", param);
                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("product-group")]
        public async Task<IActionResult> GetProductGroup(string warehouseCode = "HN")
        {
            try
            {
                List<int> productGroupIDs = new List<int> { 73, 74, 75, 77 };
                List<ProductGroup> listProductGroup = new List<ProductGroup>();
                if (warehouseCode == "HN")
                {
                    listProductGroup = _productGroupRepo.GetAll();
                }
                else if (warehouseCode == "HCM")
                {
                    listProductGroup = _productGroupRepo.GetAll(x => !productGroupIDs.Contains(x.ID));
                }
                return Ok(ApiResponseFactory.Success(listProductGroup, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("validate-inventory")]
        public async Task<IActionResult> ValidateInventory(InventoryStock inventoryStock)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                List<InventoryStock> listInventoryStock = _inventoryStockRepo.GetAll(
                    x => x.ProductSaleID == inventoryStock.ProductSaleID &&
                    x.WarehouseID == inventoryStock.WarehouseID &&
                    x.ProjectTypeID == inventoryStock.ProjectTypeID &&
                    x.EmployeeIDRequest == inventoryStock.EmployeeStock &&
                    x.IsDeleted != true &&
                    x.ID != inventoryStock.ID
                    );

                if (listInventoryStock != null && listInventoryStock.Count > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, null));
                }

                if (inventoryStock.ID > 0 && inventoryStock.EmployeeIDRequest != currentUser.EmployeeID && !currentUser.IsAdmin)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể sửa thông tin của nhân viên khác!"));
                }

                return Ok(ApiResponseFactory.Success("", null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("data-inventory")]
        [RequiresPermission("N27,N35,N1,N33,N34")]
        public async Task<IActionResult> GetDataInventory(
            int id,
            int warehouseId,
            int productGroupId,
            string keyWords = "")
        {
            try
            {
                var warehouse = _warehouseRepo.GetByID(warehouseId);
                var param = new
                {
                    ID = id,
                    Find = keyWords,
                    WarehouseCode = warehouse.WarehouseCode,
                    IsStock = 1
                };

                var paramWarehouse = new
                {
                    WarehouseID = warehouseId,
                    ProductGroupID = id == 0 ? 99 : id
                };

                var dtMaster = await SqlDapper<object>.ProcedureToListAsync("spGetInventoryStock", param);
                var dtWarehouse = await SqlDapper<object>.ProcedureToListAsync("spGetProductGroupWarehouse", paramWarehouse);

                var result = new
                {
                    dtMaster = dtMaster,
                    dtWarehouse = dtWarehouse
                };

                return Ok(ApiResponseFactory.Success(result, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("inventory-stock-by-id")]
        [RequiresPermission("N27,N35,N1,N33,N34")]
        public async Task<IActionResult> GetInventoryStock(int id)
        {
            try
            {
                InventoryStock inventoryStock = _inventoryStockRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(inventoryStock, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-inventory")]
        [RequiresPermission("N27,N35,N1,N33,N34")]
        public async Task<IActionResult> DeleteInventory(List<int> ids)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (ids.Count() <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Chưa có dữ liệu để xóa"));

                foreach (int id in ids)
                {
                    if (id > 0)
                    {
                        InventoryStock inventory = _inventoryStockRepo.GetByID(id);
                        if (inventory != null)
                        {
                            inventory.IsDeleted = true;
                            await _inventoryStockRepo.UpdateAsync(inventory);

                            ProductSale productSale = _productSaleRepo.GetByID(inventory.ProductSaleID ?? 0);

                            string log = $"{currentUser.FullName} đã xó tồn kho sản phẩm [{productSale?.ProductCode}]";
                            await _inventoryStockLogRepo.AddLog(
                                inventory.ID,
                                log,
                                "Xóa"
                            );
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success("Xóa thành công", null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        [RequiresPermission("N27,N35,N1,N33,N34")]
        public async Task<IActionResult> SaveData(InventoryStock inventoryStock)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                inventoryStock.EmployeeIDRequest = currentUser.EmployeeID;
                if (inventoryStock.ID > 0)
                {
                    var oldModel = _inventoryStockRepo.GetByID(inventoryStock.ID);
                    string log = _inventoryStockLogRepo.GenerateLog(oldModel, inventoryStock);
                    if (!string.IsNullOrWhiteSpace(log))
                    {
                        await _inventoryStockLogRepo.AddLog(inventoryStock.ID,$"[Sửa] {currentUser.FullName} đã cập nhật:\n{log}", "Cập nhật");
                    }
                    
                    await _inventoryStockRepo.UpdateAsync(inventoryStock);
                   
                }
                else
                {
                    var checkInventory = _inventoryStockRepo.GetAll(x => x.ProductSaleID == inventoryStock.ProductSaleID && x.IsDeleted == false).FirstOrDefault();
                    ProductSale productSale = _productSaleRepo.GetByID(inventoryStock.ProductSaleID ?? 0);
                    if (checkInventory != null)
                    {
                        checkInventory.Quantity += inventoryStock.Quantity;
                        checkInventory.EmployeeIDRequest = currentUser.EmployeeID;

                        await _inventoryStockLogRepo.AddLog(checkInventory.ID, $"[Sửa] {currentUser.FullName} đã cập nhật:\n+ bổ sung tồn [{inventoryStock.Quantity}] vào sản phẩm [{productSale?.ProductCode}]", "Cập nhật");
                        await _inventoryStockRepo.UpdateAsync(checkInventory);
                    }
                    else
                    {
                        await _inventoryStockRepo.CreateAsync(inventoryStock);
                        string log = $"[Thêm mới] {currentUser.FullName} đã thêm mới tồn kho sản phẩm [{productSale?.ProductCode}] số lượng [{inventoryStock.Quantity}]";
                        await _inventoryStockLogRepo.AddLog(
                            inventoryStock.ID,
                            log,
                            "Thêm mới"
                        );
                    }
                        
                }

                return Ok(ApiResponseFactory.Success("Lưu thành công", null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("vaildate-inventory-stock")]
        public async Task<IActionResult> ValidateInventoryStock(List<InventoryStockDTO> inventoryStocks)
        {
            try
            {
                // Check sản phẩm xem có trong kho dự án hay không
                string productCodes = "";
                foreach (InventoryStockDTO item in inventoryStocks)
                {
                    var productsale = _productSaleRepo.GetAll(
                        x => x.ProductCode == item.ProductSaleCode &&
                        x.IsDeleted == false &&
                        x.ProductGroupID == 13
                        ).FirstOrDefault();

                    if (productsale == null)
                    {
                        productCodes += item.ProductSaleCode + "; ";
                    }
                }

                return Ok(ApiResponseFactory.Success(productCodes, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel(List<InventoryStockDTO> inventoryStocks)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                foreach (InventoryStockDTO item in inventoryStocks)
                {
                    var productsale = _productSaleRepo.GetAll(
                        x => x.ProductCode == item.ProductSaleCode &&
                        x.IsDeleted == false &&
                        x.ProductGroupID == 13
                    ).FirstOrDefault();

                    if (productsale != null)
                    {
                        InventoryStock inventory = _inventoryStockRepo.GetAll(
                            x => x.ProductSaleID == productsale.ID &&
                            x.IsDeleted == false
                        ).FirstOrDefault();

                        if (inventory != null)
                        {
                            var oldModel = new InventoryStock
                            {
                                ID = inventory.ID,
                                Quantity = inventory.Quantity,
                                ProductSaleID = inventory.ProductSaleID,
                                InventoryID = inventory.InventoryID,
                                WarehouseID = inventory.WarehouseID,
                                EmployeeIDRequest = inventory.EmployeeIDRequest,
                                EmployeeStock = inventory.EmployeeStock,
                                ProjectTypeID = inventory.ProjectTypeID,
                                Note = inventory.Note,
                                IsDeleted = inventory.IsDeleted
                            };

                            inventory.Quantity += item.Quantity;
                            inventory.Note = item.Note;
                            inventory.EmployeeIDRequest = currentUser.EmployeeID;

                            string log = _inventoryStockLogRepo.GenerateLog(oldModel, inventory);
                            if (!string.IsNullOrWhiteSpace(log))
                            {
                                await _inventoryStockLogRepo.AddLog(inventory.ID, $"[Nhập excel] {currentUser.FullName} đã cập nhật:\n{log}", "Cập nhật");
                            }
                            await _inventoryStockRepo.UpdateAsync(inventory);
                        }
                        else
                        {
                            inventory = new InventoryStock
                            {
                                ProductSaleID = productsale.ID,
                                Quantity = item.Quantity,
                                IsDeleted = false,
                                Note = item.Note,
                                ProjectTypeID = 0,
                                WarehouseID = 1,
                                EmployeeIDRequest = currentUser.EmployeeID
                            };

                            await _inventoryStockRepo.CreateAsync(inventory);

                            var productSale = _productSaleRepo.GetByID(inventory.ProductSaleID ?? 0);
                            string log = $"[Nhập excel] {currentUser.FullName} đã thêm mới tồn kho sản phẩm [{productSale?.ProductCode}] số lượng [{inventory.Quantity}]";
                            await _inventoryStockLogRepo.AddLog(
                                inventory.ID,
                                log,
                                "Thêm mới"
                            );
                        }

                    }
                }

                return Ok(ApiResponseFactory.Success("Lưu thành công", null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("inventory-stock-log")]
        public async Task<IActionResult> GetInventoryStockLog(int inventoryStockId)
        {
            try
            {
                var inventoryStockLog = _inventoryStockLogRepo.GetAll(x => x.InventoryStockID == inventoryStockId).OrderByDescending(x => x.CreatedDate);
                return Ok(ApiResponseFactory.Success(inventoryStockLog, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
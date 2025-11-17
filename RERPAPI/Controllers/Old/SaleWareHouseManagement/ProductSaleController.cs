using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSaleController : ControllerBase
    {
        private readonly ProductGroupRepo _productgroupRepo;
        private readonly ProductsSaleRepo _productsaleRepo;
        private readonly InventoryRepo _inventoryRepo;
        private readonly FirmRepo _firmRepo;

        public ProductSaleController(
            ProductGroupRepo productgroupRepo,
            ProductsSaleRepo productsaleRepo,
            InventoryRepo inventoryRepo,
            FirmRepo firmRepo)
        {
            _productgroupRepo = productgroupRepo;
            _productsaleRepo = productsaleRepo;
            _inventoryRepo = inventoryRepo;
            _firmRepo = firmRepo;
        }
        //api ngày 12/06/2025

        #region hàm lấy dữ liệu vật tư theo id, tên

        [HttpPost("")]
        public IActionResult GetProductSale([FromBody] ProductSaleParamRequest filter)
        {
            try
            {
                if (filter.checkedAll == true)
                {
                    filter.id = 0;
                }
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "usp_LoadProductsale", new string[] { "@id", "@Find", "@IsDeleted" },
                    new object[] { filter.id, filter.find ?? "", false }
                   );
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region hàm lấy dữ liệu productsale theo id
        [HttpGet("{id}")]
        public IActionResult getProductSaleByID(int id)
        {
            try
            {
                var rs = _productsaleRepo.GetByID(id);

                return Ok(ApiResponseFactory.Success(rs, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region hàm lấy vật tư theo id nhóm
        [HttpGet("get-product-sale-by-product-group")]
        public IActionResult getProductSaleByGroup(int productgroupID)
        {
            try
            {
                List<ProductSale> rs = _productsaleRepo.GetAll().Where(x => x.ProductGroupID == productgroupID).ToList();
                return Ok(ApiResponseFactory.Success(rs, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region hàm sinh mã nội bộ (productnewcode) 
        private string GenerateProductNewCode(int productGroupId)
        {
            // Bước 1: Lấy mã nhóm sản phẩm từ ID
            var productGroup = _productgroupRepo.GetByID(productGroupId);
            if (productGroup == null || string.IsNullOrWhiteSpace(productGroup.ProductGroupID))
                return string.Empty;

            string productGroupCode = productGroup.ProductGroupID.Trim();

            // Bước 2: Lấy danh sách sản phẩm thuộc nhóm này
            var listProducts = _productsaleRepo.GetAll()
                .Where(x => x.ProductGroupID == productGroupId &&
                            !string.IsNullOrWhiteSpace(x.ProductNewCode) &&
                            x.ProductNewCode.StartsWith(productGroupCode))
                .ToList();

            // Bước 3: Tính STT cao nhất đang dùng
            var listNewCodes = listProducts.Select(x => new
            {
                STT = int.TryParse(x.ProductNewCode.Substring(productGroupCode.Length), out int num) ? num : 0
            });

            int nextSTT = listNewCodes.Any() ? listNewCodes.Max(x => x.STT) + 1 : 1;
            string numberCodeText = nextSTT.ToString().PadLeft(9 - productGroupCode.Length, '0');

            return productGroupCode + numberCodeText;
        }
        #endregion
        //done+ update ngày 14/06 : xóa nhiều bản ghi 
        #region hàm thêm, sửa, xóa productSale
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveDataProductSale([FromBody] List<ProductsSaleDTO> dtos)
        {
            try
            {
                foreach (var dto in dtos)
                {
                    //TN.Binh update 19/10/25
                    if (!CheckProductCode(dto))
                    {
                        return BadRequest(new { status = 0, message = $"Mã sản phẩm [{dto.ProductSale.ProductCode}] đã tồn tại trong nhóm !" });
                    }
                    //end update 
                    if (dto.ProductSale.ID <= 0)
                    {
                        // Tạo mới
                        if (string.IsNullOrWhiteSpace(dto.ProductSale.ProductNewCode))
                        {
                            dto.ProductSale.ProductNewCode = GenerateProductNewCode((int)dto.ProductSale.ProductGroupID);
                        }
                        dto.ProductSale.Import = dto.ProductSale.Export = dto.ProductSale.NumberInStoreCuoiKy = dto.ProductSale.NumberInStoreDauky;
                        dto.ProductSale.SupplierName = "";
                        dto.ProductSale.ItemType = "";
                        //int newId = await _productsaleRepo.CreateAsynC(dto.ProductSale);
                        await _productsaleRepo.CreateAsync(dto.ProductSale);
                        int newId = dto.ProductSale.ID;
                        dto.Inventory.ProductSaleID = newId;
                        dto.Inventory.WarehouseID = 1;
                        dto.Inventory.Export = 0;
                        dto.Inventory.Import = 0;
                        dto.Inventory.TotalQuantityFirst = 0;
                        dto.Inventory.TotalQuantityLast = 0;
                        dto.Inventory.MinQuantity = 0;
                        dto.Inventory.IsStock = false;

                        await _inventoryRepo.CreateAsync(dto.Inventory);
                    }
                    else
                    {
                        // Cập nhật
                        _productsaleRepo.Update(dto.ProductSale);
                    }
                }

                return Ok(ApiResponseFactory.Success(dtos, "Xử lý dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        [HttpPost("save-data-excel")]
        public async Task<IActionResult> SaveDataProductSaleExcel([FromBody] List<ProductsSaleDTO> dtos)
        {
            try
            {
                int successCount = 0;
                int failCount = 0;
                List<string> duplicateCodes = new();
                List<string> errorMessages = new();

                foreach (var dto in dtos)
                {
                    try
                    {
                        if (!CheckProductCode(dto))
                        {
                            duplicateCodes.Add(dto.ProductSale.ProductCode ?? "N/A");
                            failCount++;
                            continue; // Bỏ qua bản ghi trùng mã
                        }

                        if (dto.ProductSale.ID <= 0)
                        {
                            // ✅ Tạo mới
                            if (string.IsNullOrWhiteSpace(dto.ProductSale.ProductNewCode))
                            {
                                dto.ProductSale.ProductNewCode = GenerateProductNewCode((int)dto.ProductSale.ProductGroupID);
                            }

                            dto.ProductSale.Import = dto.ProductSale.Export =
                                dto.ProductSale.NumberInStoreCuoiKy = dto.ProductSale.NumberInStoreDauky;

                            dto.ProductSale.SupplierName = "";
                            dto.ProductSale.ItemType = "";

                            await _productsaleRepo.CreateAsync(dto.ProductSale);
                            int newId = dto.ProductSale.ID;

                            dto.Inventory.ProductSaleID = newId;
                            dto.Inventory.WarehouseID = 1;
                            dto.Inventory.Export = 0;
                            dto.Inventory.Import = 0;
                            dto.Inventory.TotalQuantityFirst = 0;
                            dto.Inventory.TotalQuantityLast = 0;
                            dto.Inventory.MinQuantity = 0;
                            dto.Inventory.IsStock = false;

                            await _inventoryRepo.CreateAsync(dto.Inventory);

                            successCount++;
                        }
                        else
                        {
                            // ✅ Cập nhật
                            _productsaleRepo.Update(dto.ProductSale);
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        errorMessages.Add($"Lỗi khi xử lý mã [{dto.ProductSale?.ProductCode ?? "N/A"}]: {ex.Message}");
                    }
                }

                string message = $"Lưu thành công {successCount} bản ghi, thất bại {failCount} bản ghi.";
                if (duplicateCodes.Any())
                    message += $" Các mã trùng bị bỏ qua: {string.Join(", ", duplicateCodes)}.";

                return Ok(ApiResponseFactory.Success(new
                {
                    successCount,
                    failCount,
                    duplicateCodes,
                    errorMessages
                }, message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //TN.Binh update 19/10/25
        #region check trùng mã sản phẩm khi thêm, sửa vật tư 
        private bool CheckProductCode(ProductsSaleDTO dto)
        {
            bool check = true;
            var exists = _productsaleRepo.GetAll()
                .Where(x => x.ProductCode == dto.ProductSale.ProductCode
                            && x.ProductGroupID == dto.ProductSale.ProductGroupID
                            && x.ID != dto.ProductSale.ID && dto.ProductSale.IsDeleted == false).ToList();
            if (exists.Count > 0) check = false;
            return check;
        }

        //end update
        #endregion

        //check-productsale trong excel
        [HttpPost("check-codes")]
        public async Task<IActionResult> checkCodes([FromBody] List<ProductSaleCodeCheck> codes)
        {
            try
            {
                // Lấy danh sách các mã cần kiểm tra
                var productsaleCode = codes.Select(x => x.ProductCode).ToList();
                var productsaleName = codes.Select(x => x.ProductName).ToList();

                // Kiểm tra trong database
                var existingProducts = _productsaleRepo.GetAll()
                    .Where(x => productsaleCode.Contains(x.ProductCode) && productsaleName.Contains(x.ProductName))
                    .Select(x => new
                    {
                        x.ID, // Thêm ID vào đây
                        x.ProductCode,
                        x.ProductName

                    })
                    .ToList();
                return Ok(ApiResponseFactory.Success(new { existingProducts }, "kiểm tra code thành công!"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}




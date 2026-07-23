using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly UnitCountRepo _unitCountRepo;

        private readonly BillImportRepo _billImportRepo;
        private readonly BillExportRepo _billExportRepo;
        private readonly BillImportDetailRepo _billImportDetailRepo;
        private readonly BillExportDetailRepo _billExportDetailRepo;
        private readonly ProductSaleImportExportLogRepo _productSaleImportExportLogRepo;
        private readonly WarehouseRepo _warehouseRepo;
        private readonly ProductGroupRepo _productGroupRepo;
        private readonly CustomerRepo _customerRepo;
        private readonly SupplierSaleRepo _supplierRepo;
        private readonly POKHRepo _pokhRepo;
        private readonly POKHDetailRepo _pokhDetailRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly EmailHelper _emailHelper;


        public ProductSaleController(
            ProductGroupRepo productgroupRepo,
            ProductsSaleRepo productsaleRepo,
            InventoryRepo inventoryRepo,
            FirmRepo firmRepo,
            UnitCountRepo unitCountRepo,
            BillImportRepo billImportRepo,
            BillExportRepo billExportRepo,
            BillExportDetailRepo billExportDetailRepo,
            BillImportDetailRepo billimportDetailRepo,
            ProductSaleImportExportLogRepo productSaleImportExportLogRepo,
            WarehouseRepo warehouseRepo,
            ProductGroupRepo productGroupRepo,
            CustomerRepo customerRepo,
            SupplierSaleRepo supplierRepo,
            POKHRepo pokhRepo,
            POKHDetailRepo pokhDetailRepo,
            EmployeeRepo employeeRepo,
            EmailHelper emailHelper
            )
        {
            _productgroupRepo = productgroupRepo;
            _productsaleRepo = productsaleRepo;
            _inventoryRepo = inventoryRepo;
            _firmRepo = firmRepo;
            _unitCountRepo = unitCountRepo;
            _billImportRepo = billImportRepo;
            _billExportRepo = billExportRepo;
            _billImportDetailRepo = billimportDetailRepo;
            _billExportDetailRepo = billExportDetailRepo;
            _productSaleImportExportLogRepo = productSaleImportExportLogRepo;
            _warehouseRepo = warehouseRepo;
            _productGroupRepo = productGroupRepo;
            _customerRepo = customerRepo;
            _supplierRepo = supplierRepo;
            _pokhRepo = pokhRepo;
            _pokhDetailRepo = pokhDetailRepo;
            _employeeRepo = employeeRepo;
            _emailHelper = emailHelper;
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

        #endregion hàm lấy dữ liệu vật tư theo id, tên

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

        #endregion hàm lấy dữ liệu productsale theo id

        #region hàm lấy vật tư theo id nhóm

        [HttpGet("get-product-sale-by-product-group")]
        public IActionResult getProductSaleByGroup(int productgroupID)
        {
            try
            {
                List<ProductSale> rs = _productsaleRepo.GetAll(x => x.ProductGroupID == productgroupID && x.IsStandardized == true); // VTN update 22626
                return Ok(ApiResponseFactory.Success(rs, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion hàm lấy vật tư theo id nhóm

        [HttpGet("gen-code")]
        public IActionResult GenCode(int productgroupID)
        {
            try
            {
                string rs = GenerateProductNewCode(productgroupID);
                return Ok(ApiResponseFactory.Success(rs, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #region hàm sinh mã nội bộ (productnewcode)

        //private string GenerateProductNewCode(int productGroupId)
        //{
        //    // Bước 1: Lấy mã nhóm sản phẩm từ ID
        //    var productGroup = _productgroupRepo.GetByID(productGroupId);
        //    if (productGroup == null || string.IsNullOrWhiteSpace(productGroup.ProductGroupID))
        //        return string.Empty;

        //    string productGroupCode = productGroup.ProductGroupID.Trim();

        //    // Bước 2: Lấy danh sách sản phẩm thuộc nhóm này
        //    var listProducts = _productsaleRepo.GetAll(x => x.ProductGroupID == productGroupId &&
        //                    !string.IsNullOrWhiteSpace(x.ProductNewCode) &&
        //                    x.ProductNewCode.StartsWith(productGroupCode) && x.IsDeleted == false)
        //        .ToList();

        //    // Bước 3: Tính STT cao nhất đang dùng
        //    var listNewCodes = listProducts.Select(x => new
        //    {
        //        STT = int.TryParse(x.ProductNewCode.Substring(productGroupCode.Length), out int num) ? num : 0
        //    });

        //    int nextSTT = listNewCodes.Any() ? listNewCodes.Max(x => x.STT) + 1 : 1;
        //    string numberCodeText = nextSTT.ToString().PadLeft(9 - productGroupCode.Length, '0');

        //    return productGroupCode + numberCodeText;
        //}

        //private string GenerateProductNewCode(int productGroupId)
        //{
        //    // 1️⃣ Lấy nhóm hiện tại
        //    var currentGroup = _productgroupRepo.GetByID(productGroupId);
        //    if (currentGroup == null)
        //        return string.Empty;

        //    // 2️⃣ Xác định nhóm CHA
        //    var parentGroup = currentGroup.ParentID > 0
        //        ? _productgroupRepo.GetByID(currentGroup.ParentID.Value)
        //        : currentGroup;

        //    if (parentGroup == null || string.IsNullOrWhiteSpace(parentGroup.ProductGroupID))
        //        return string.Empty;

        //    string parentGroupCode = parentGroup.ProductGroupID.Trim();

        //    // 3️⃣ Lấy danh sách ID nhóm con (bao gồm cha)
        //    var groupIds = _productgroupRepo
        //        .GetAll(x => x.ID == parentGroup.ID || x.ParentID == parentGroup.ID)
        //        .Select(x => x.ID)
        //        .ToList();

        //    // 4️⃣ Lấy toàn bộ sản phẩm thuộc nhóm cha + con
        //    var listProducts = _productsaleRepo.GetAll(x =>
        //        x.ProductGroupID != null &&
        //        !string.IsNullOrWhiteSpace(x.ProductNewCode) &&
        //        x.ProductNewCode.StartsWith(parentGroupCode) &&
        //        x.IsDeleted == false
        //    ).ToList();

        //    // 2️⃣ Lọc tiếp trong memory
        //    //var listProducts = products
        //    //    .Where(x => groupIds.Contains(x.ProductGroupID!.Value))
        //    //    .ToList();

        //    // 5️⃣ Tính STT lớn nhất
        //    int maxSTT = listProducts
        //        .Select(x =>
        //        {
        //            var numberPart = x.ProductNewCode.Substring(parentGroupCode.Length);
        //            return int.TryParse(numberPart, out int num) ? num : 0;
        //        })
        //        .DefaultIfEmpty(0)
        //        .Max();

        //    int nextSTT = maxSTT + 1;

        //    // 6️ Format số (đủ 9 ký tự)
        //    string numberCodeText = nextSTT
        //        .ToString()
        //        .PadLeft(9 - parentGroupCode.Length, '0');

        //    return parentGroupCode + numberCodeText;
        //}

        private string GenerateProductNewCode(int productGroupId)
        {
            var currentGroup = _productgroupRepo.GetByID(productGroupId);
            if (currentGroup == null)
                return string.Empty;

            string groupCode = currentGroup.ProductGroupID.Trim();

            var groupIds = _productgroupRepo
                .GetAll(x => x.ProductGroupID == groupCode)
                .Select(x => x.ID)
                .ToList();

            var products = _productsaleRepo.GetAll(x =>
                    x.ProductGroupID != null &&
                    !string.IsNullOrWhiteSpace(x.ProductNewCode) &&
                    x.ProductNewCode.StartsWith(groupCode)
                ).ToList();

            var listProducts = products
                .Where(x => groupIds.Contains(x.ProductGroupID!.Value))
                .ToList();

            int maxSTT = listProducts
                .Select(x =>
                {
                    var numberPart = x.ProductNewCode.Substring(groupCode.Length);
                    return int.TryParse(numberPart, out int num) ? num : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            int nextSTT = maxSTT + 1;

            string numberCodeText = nextSTT
                .ToString()
                .PadLeft(9 - groupCode.Length, '0');

            return groupCode + numberCodeText;
        }

        #endregion hàm sinh mã nội bộ (productnewcode)

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
                        return BadRequest(ApiResponseFactory.Fail(null, $"Mã sản phẩm [{dto.ProductSale.ProductCode}] đã tồn tại trong nhóm !"));
                    }
                    //end update
                    if (dto.ProductSale.ID <= 0)
                    {
                        // Tạo mới
                        if (string.IsNullOrWhiteSpace(dto.ProductSale.ProductNewCode))
                        {
                            var productGroup = _productgroupRepo.GetByID((int)dto.ProductSale.ProductGroupID);
                            int productGroupID = productGroup.ParentID > 0 ? (int)productGroup.ParentID : (int)productGroup.ID;
                            dto.ProductSale.ProductNewCode = GenerateProductNewCode((int)dto.ProductSale.ProductGroupID);
                        }
                        dto.ProductSale.Import = dto.ProductSale.Export = dto.ProductSale.NumberInStoreCuoiKy = dto.ProductSale.NumberInStoreDauky;
                        dto.ProductSale.SupplierName = "";
                        dto.ProductSale.ItemType = "";
                        dto.ProductSale.IsStandardized = true;
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

                        int prdGroupID = dto.ProductSale.ProductGroupID ?? 0;
                        if (prdGroupID == 83 || prdGroupID == 84)
                        {
                            dto.Inventory.WarehouseID = 6;
                            dto.Inventory.ProductGroupID = dto.ProductSale.ProductGroupID;
                        }

                        await _inventoryRepo.CreateAsync(dto.Inventory);
                    }
                    else
                    {
                        // Cập nhật
                        var product = _productsaleRepo.GetSingleNoTracking(x => x.ID == dto.ProductSale.ID);
                        if (product.ProductGroupID != dto.ProductSale.ProductGroupID)
                        {
                            dto.ProductSale.ProductNewCode = GenerateProductNewCode(dto.ProductSale.ProductGroupID ?? 0);
                        }
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

        #endregion hàm thêm, sửa, xóa productSale

        [HttpPost("save-data-excel")]
        public async Task<IActionResult> SaveDataProductSaleExcel([FromBody] List<ProductSaleImportExcelDTO> dtos)
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
                        var groupName = (dto.ProductGroupName ?? "").Trim().ToLower();
                        var groupNo = (dto.ProductGroupNo ?? "").Trim().ToLower();

                        var typeNo = (dto.ProductGroupTypeNo ?? "").Trim().ToLower();
                        var typeName = (dto.ProductGroupTypeName ?? "").Trim().ToLower();

                        //if (groupNo == typeNo)
                        //{
                        //    return BadRequest(ApiResponseFactory.Fail(null, $"Loại vật tư [{typeNo}] trùng với nhóm [{groupNo}]!"));
                        //}

                        if (!string.IsNullOrWhiteSpace(groupName) || !string.IsNullOrWhiteSpace(groupNo))
                        {
                            var productGroup = _productgroupRepo
                                .GetAll(x =>
                                    x.IsVisible == true &&
                                    (x.ProductGroupName ?? "").ToLower() == groupName &&
                                    (x.ProductGroupID ?? "").ToLower() == groupNo
                                )
                                .FirstOrDefault()
                                ?? new ProductGroup
                                {
                                    ID = 0,
                                    ProductGroupName = dto.ProductGroupName,
                                    ProductGroupID = dto.ProductGroupNo
                                };

                            if (productGroup.ID <= 0)
                                await _productgroupRepo.CreateAsync(productGroup);

                            dto.ProductGroupID = productGroup.ID;

                            if (!string.IsNullOrWhiteSpace(typeNo) && !string.IsNullOrWhiteSpace(typeName))
                            {
                                var productGroupType = _productgroupRepo
                                .GetAll(x =>
                                    x.IsVisible != false &&
                                    (x.ProductGroupName ?? "").ToLower() == typeName &&
                                    (x.ProductGroupID ?? "").ToLower() == typeNo
                                )
                                .FirstOrDefault()
                                ?? new ProductGroup
                                {
                                    ID = 0,
                                    ProductGroupName = dto.ProductGroupTypeName,
                                    ProductGroupID = dto.ProductGroupTypeNo,
                                    ParentID = productGroup.ID
                                };

                                if (productGroupType.ID > 0 && productGroupType.ParentID != null && productGroupType.ParentID != productGroup.ID)
                                {
                                    var checkGroup = _productgroupRepo.GetByID(Convert.ToInt32(productGroupType.ParentID));
                                    return BadRequest(ApiResponseFactory.Fail(null, $"Mã loại vật tư [{typeNo}] đã tồn tại trong nhóm [{checkGroup.ProductGroupID}-{checkGroup.ProductGroupName}]! Vui lòng kiểm tra lại."));
                                }
                                if (productGroupType.ID <= 0)
                                {
                                    await _productgroupRepo.CreateAsync(productGroupType);
                                }
                                dto.ProductGroupID = productGroupType.ID;
                            }
                        }

                        if (_productsaleRepo.CheckCode(dto))
                        {
                            duplicateCodes.Add(dto.ProductCode ?? "N/A");
                            failCount++;
                            continue; // Bỏ qua bản ghi trùng mã
                        }

                        if (!string.IsNullOrWhiteSpace(dto.FirmName))
                        {
                            var firm = _firmRepo.GetAll(x => x.IsDelete == false && x.FirmName.Trim().ToLower() == dto.FirmName.Trim().ToLower()).FirstOrDefault() ?? new Firm()
                            {
                                ID = 0,
                                FirmName = dto.FirmName,
                                FirmCode = _firmRepo.GenerateCode(1),
                                IsDelete = false,
                                FirmType = 1
                            };
                            if (firm.ID <= 0) await _firmRepo.CreateAsync(firm);

                            dto.FirmID = firm.ID;
                            dto.Maker = firm.FirmName;
                        }

                        if (!string.IsNullOrWhiteSpace(dto.UnitName))
                        {
                            var unitCount = _unitCountRepo.GetAll(x => x.IsDeleted == false && x.UnitName.Trim().ToLower() == dto.UnitName.Trim().ToLower()).FirstOrDefault() ?? new UnitCount()
                            {
                                ID = 0,
                                UnitName = dto.UnitName,
                                UnitCode = dto.UnitName,
                                IsDeleted = false,
                            };
                            if (unitCount.ID <= 0) await _unitCountRepo.CreateAsync(unitCount);

                            dto.Unit = unitCount.UnitName;
                        }
                        if (dto.ID <= 0)
                        {
                            if (string.IsNullOrWhiteSpace(dto.ProductNewCode))
                            {
                                dto.ProductNewCode = GenerateProductNewCode((int)dto.ProductGroupID);
                            }

                            dto.Import = dto.Export =
                                dto.NumberInStoreCuoiKy = dto.NumberInStoreDauky;

                            dto.SupplierName = "";
                            dto.ItemType = "";
                            dto.IsStandardized = true;
                            await _productsaleRepo.CreateAsync(dto);

                            successCount++;
                        }
                        else
                        {
                            _productsaleRepo.Update(dto);
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        errorMessages.Add($"Lỗi khi xử lý mã [{dto?.ProductCode ?? "N/A"}]: {ex.Message}");
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
            var exists = _productsaleRepo.GetAll(x => x.ProductCode == dto.ProductSale.ProductCode
                            && x.ProductGroupID == dto.ProductSale.ProductGroupID
                            && x.ID != dto.ProductSale.ID && x.IsDeleted == false);
            if (exists.Count > 0) check = false;
            return check;
        }

        //end update

        #endregion check trùng mã sản phẩm khi thêm, sửa vật tư

        ////check-productsale trong excel
        //[HttpPost("check-codes")]
        //public async Task<IActionResult> checkCodes([FromBody] List<ProductSaleCodeCheck> codes)
        //{
        //    try
        //    {
        //        // Lấy danh sách các mã cần kiểm tra
        //        //var productsaleCode = codes.Select(x => x.ProductCode).ToList();
        //        //var productsaleName = codes.Select(x => x.ProductName).ToList();

        //        // Kiểm tra trong database
        //        //var existingProducts = _productsaleRepo.GetAll(x => productsaleCode.Contains(x.ProductCode) && productsaleName.Contains(x.ProductName) && x.IsDeleted==false)
        //        //    .Select(x => new
        //        //    {
        //        //        x.ID, // Thêm ID vào đây
        //        //        x.ProductCode,
        //        //        x.ProductName

        //        //    })
        //        //    .ToList();

        //        foreach (var item in codes)
        //        {
        //            var existingProducts = _productsaleRepo.GetAll(x => x.ProductCode.Trim().ToLower() == item.ProductCode.Trim().ToLower() && x.ProductName.Trim().ToLower() == item.ProductName.Trim().ToLower() && x.IsDeleted==false);
        //        }

        //        return Ok(ApiResponseFactory.Success(new { existingProducts }, "kiểm tra code thành công!"));

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        [HttpPost("standardize-product-group")]
        public async Task<IActionResult> visibleProductGroup([FromBody] List<ProductSale> data)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                foreach (ProductSale item in data)
                {
                    if (item.ID > 0)
                    {
                        var productSale = _productsaleRepo.GetByID(item.ID);
                        if (productSale != null)
                        {
                            productSale.IsStandardized = item.IsStandardized;
                            await _productsaleRepo.UpdateAsync(productSale);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Xử lý dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-product-sale-new")]
        public IActionResult getProductSaleNew()
        {
            try
            {
                var rs = _productsaleRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(rs, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("products-sale-import-export")]
        public async Task<IActionResult> ProductsSaleImportExport([FromBody] ProductsSaleImportExportDTO model)
        {
            try
            {
                var log = new StringBuilder();

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var now = DateTime.Now;
                var dateStr = model.Date?.ToString("yyMMdd");

                var wh = _warehouseRepo.GetByID(model.WarehouseId ?? 0);
                var pg = _productGroupRepo.GetByID(model.ProductGroupId ?? 0);
                var customer = _customerRepo.GetByID(model.CustomerId ?? 0);
                var supplier = _supplierRepo.GetByID(model.SupplierId ?? 0);

                log.AppendLine($"Người tạo   : {currentUser.LoginName}");
                log.AppendLine($"Thời gian   : {now:dd/MM/yyyy HH:mm:ss}");
                log.AppendLine($"Ngày chứng từ: {model.Date:dd/MM/yyyy}");
                log.AppendLine($"Ngày yêu cầu : {model.RequestDate:dd/MM/yyyy}");
                log.AppendLine($"Kho : {wh.WarehouseCode}");
                log.AppendLine($"Nhóm sản phẩm : {pg.ProductGroupName}");
                log.AppendLine($"Khách hàng  : {customer.CustomerName}");
                log.AppendLine($"Nhà cung cấp  : {supplier.NameNCC}");
                log.AppendLine($"Người giao hàng xuất  : {model.ReciverExportText}");
                log.AppendLine($"Người nhận hàng xuất  : {model.SenderExportText}");
                log.AppendLine($"Người giao hàng nhập  : {model.DeliverImportText}");
                log.AppendLine($"Người nhận hàng nhập  : {model.ReciverImportText}");
                log.AppendLine($"Ghi chú     : {model.Note}");
                log.AppendLine();


                // Lấy Số thứ tự tiếp theo cho PXK
                var lastExport = _billExportRepo
                    .GetAll(b => b.Code != null && b.Code.StartsWith("PXK" + dateStr))
                    .OrderByDescending(b => b.Code)
                    .Select(b => b.Code)
                    .FirstOrDefault();

                int expSeq = 1;
                if (lastExport != null && lastExport.Length >= 9)
                {
                    if (int.TryParse(lastExport.Substring(9), out int lastSeq)) expSeq = lastSeq + 1;
                }
                log.AppendLine($"Mã PXK: PXK{dateStr}{expSeq:D3}");
                // Lấy Số thứ tự tiếp theo cho PNK
                var lastImport = _billImportRepo
                    .GetAll(b => b.BillImportCode != null && b.BillImportCode.StartsWith("PNK" + dateStr))
                    .OrderByDescending(b => b.BillImportCode)
                    .Select(b => b.BillImportCode)
                    .FirstOrDefault();

                int impSeq = 1;
                if (lastImport != null && lastImport.Length >= 9)
                {
                    if (int.TryParse(lastImport.Substring(9), out int lastSeq)) impSeq = lastSeq + 1;
                }
                log.AppendLine($"Mã PNK: PNK{dateStr}{impSeq:D3}");
                log.AppendLine();

                var currentTime = DateTime.Now.TimeOfDay;
                // 1. Tạo Phiếu Xuất
                var billExport = new BillExport
                {
                    Code = $"PXK{dateStr}{expSeq:D3}",
                    CreatDate = model.Date,
                    RequestDate = model.RequestDate,
                    Status = 2,
                    IsApproved = false,
                    WarehouseID = model.WarehouseId,
                    KhoTypeID = model.ProductGroupId,
                    CustomerID = model.CustomerId,
                    SupplierID = model.SupplierId,
                    SenderID = model.SenderExportId,
                    UserID = model.ReciverExportId,
                    Description = model.Note,
                    CreatedBy = currentUser.LoginName,
                    CreatedDate = now,
                    IsDeleted = false,

                    DeliveryTime = DateTime.Now,
                    IsAfterHours = currentTime < TimeSpan.FromHours(8) || currentTime >= TimeSpan.FromHours(16)
                };

                await _billExportRepo.CreateAsync(billExport);

                // 2. Chi tiết xuất
                log.AppendLine("----- Chi tiết phiếu xuất -----");

                // 2. Chi tiết xuất
                int stt = 1;
                foreach (var row in model.DataDetails)
                {
                    var billExportDetail = new BillExportDetail
                    {
                        BillID = billExport.ID,
                        ProductID = row.ExportProductId,
                        Qty = row.Quantity,
                        TotalQty = row.ExportStockQty,
                        STT = stt++,
                        ProductFullName = row.ExportProductName,
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = now,
                        IsDeleted = false
                    };

                    await _billExportDetailRepo.CreateAsync(billExportDetail);

                    log.AppendLine($"[{billExportDetail.STT}]" +
                            $"Tên SP: {row.ExportProductName} | SL xuất: {row.Quantity} | ");

                    if (billExportDetail.ProductID > 0)
                    {
                        var productSale = _productsaleRepo.GetByID((int)billExportDetail.ProductID);
                        if (productSale != null)
                        {
                            productSale.IsStandardized = false;
                            await _productsaleRepo.UpdateAsync(productSale);

                            log.AppendLine($"-> Đã cập nhật trạng thái chuẩn hóa = false cho [{row.ExportProductName}]");
                        }
                    }
                }
                log.AppendLine();

                // 3. Tạo Phiếu Nhập
                var billImport = new BillImport
                {
                    BillImportCode = $"PNK{dateStr}{impSeq:D3}",
                    CreatDate = model.Date,
                    DateRequestImport = model.RequestDate,
                    Status = false,
                    BillTypeNew = 0,
                    WarehouseID = model.WarehouseId,
                    BillExportID = billExport.ID,
                    KhoTypeID = model.ProductGroupId,
                    SupplierID = model.SupplierId,
                    RulePayID = model.RulePayId,
                    DeliverID = model.DeliverImportId,
                    ReciverID = model.ReciverImportId,
                    CreatedBy = currentUser.LoginName,
                    CreatedDate = now,
                    IsDeleted = false,
                    Deliver = model.DeliverImportText,
                    Reciver = model.ReciverImportText,
                    KhoType = model.ProductGroupText

                };

                await _billImportRepo.CreateAsync(billImport);

                log.AppendLine("----- Chi tiết phiếu nhập -----");

                // 4. Chi tiết nhập
                stt = 1;
                foreach (var row in model.DataDetails)
                {
                    var billImportDetail = new BillImportDetail
                    {
                        BillImportID = billImport.ID,
                        ProductID = row.ImportProductId,
                        Qty = row.Quantity,
                        TotalQty = row.Quantity,
                        STT = stt++,
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = now,
                        IsDeleted = false
                    };

                    var prd = _productsaleRepo.GetByID((int)billImportDetail.ProductID);

                    await _billImportDetailRepo.CreateAsync(billImportDetail);

                    if (billImportDetail.ProductID > 0)
                    {
                        var productSale = _productsaleRepo.GetByID((int)billImportDetail.ProductID);
                        if (productSale != null)
                        {
                            productSale.IsStandardized = true;
                            await _productsaleRepo.UpdateAsync(productSale);

                            log.AppendLine($"-> Đã cập nhật trạng thái chuẩn hóa = true cho [{productSale.ProductCode}]");
                        }
                    }
                }

                var groupedForLog = model.DataDetails
                    .GroupBy(x => x.ImportProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalQty = g.Sum(x => x.Quantity)
                    })
                    .ToList();

                int logStt = 1;
                foreach (var group in groupedForLog)
                {
                    var prd = group.ProductId.HasValue
                        ? _productsaleRepo.GetByID((int)group.ProductId)
                        : null;

                    log.AppendLine($"[{logStt++}]" +
                                    $"Tên SP: {prd?.ProductName ?? "(không tìm thấy)"} | " +
                                    $"Tổng SL nhập: {group.TotalQty}");
                }
                log.AppendLine();

                await _productSaleImportExportLogRepo.WriteLog("Tạo mới", log.ToString(), currentUser.LoginName);

                return Ok(ApiResponseFactory.Success(null, $"Thành công! PXK: {billExport.Code}, PNK: {billImport.BillImportCode}"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("products-sale-approved-isfix")]
        public async Task<IActionResult> ProductSaleApprovedIsfix([FromBody] List<ProductSale> request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var ids = request.Select(x => x.ID).ToList();
                var requestDict = request.ToDictionary(x => x.ID);

                //Lấy sản phẩm
                var productSales = _productsaleRepo
                    .GetAll(x => ids.Contains(x.ID))
                    .ToList();

                //Lấy danh sách employee cần CC
                var employeeIds = productSales
                    .Where(x => x.EmployeeRequestApprovedID > 0)
                    .Select(x => x.EmployeeRequestApprovedID.Value)
                    .Distinct()
                    .ToList();

                var employeeDict = _employeeRepo
                    .GetAll(x => employeeIds.Contains(x.ID))
                    .ToDictionary(x => x.ID, x => x.EmailCongTy);

                //Lấy POKHDetail
                var pokhDetails = _pokhDetailRepo
                    .GetAll(x => ids.Contains((int)x.ProductID))
                    .ToList();

                //Group theo Product
                var productPokhGroup = pokhDetails
                    .GroupBy(x => x.ProductID)
                    .ToDictionary(x => x.Key, x => x.ToList());

                //Lấy POKH
                var pokhIds = pokhDetails
                    .Select(x => x.POKHID)
                    .Distinct()
                    .ToList();

                var pokhDict = _pokhRepo
                    .GetAll(x => pokhIds.Contains(x.ID))
                    .ToDictionary(x => x.ID, x => x.POCode);

                var emailCC = new HashSet<string>();

                var tableRows = new StringBuilder();

                int stt = 1;

                foreach (var item in productSales)
                {
                    item.IsApproved = requestDict[item.ID].IsApproved;
                    item.ApprovedID = currentUser.EmployeeID;

                    if (item.IsApproved != true)
                        continue;

                    //Email người yêu cầu
                    if (item.EmployeeRequestApprovedID > 0 &&
                        employeeDict.TryGetValue(item.EmployeeRequestApprovedID.Value, out var email) &&
                        !string.IsNullOrWhiteSpace(email))
                    {
                        emailCC.Add(email);
                    }

                    //PO liên quan
                    string poCodes = "";

                    if (productPokhGroup.TryGetValue(item.ID, out var details))
                    {
                        poCodes = string.Join(", ",
                            details
                                .Select(x => pokhDict.TryGetValue((int)x.POKHID, out var code) ? code : "")
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                .Distinct());   
                    }

                    tableRows.Append($@"
                            <tr>
                                <td style='border:1px solid #ddd;padding:8px;text-align:center;'>{stt++}</td>
                                <td style='border:1px solid #ddd;padding:8px;'>{poCodes}</td>
                                <td style='border:1px solid #ddd;padding:8px;'>{item.ProductNewCode}</td>
                                <td style='border:1px solid #ddd;padding:8px;'>{item.ProductCode}</td>
                                <td style='border:1px solid #ddd;padding:8px;'>{item.ProductName}</td>
                            </tr>");
                }

                //Update một lần
                await _productsaleRepo.UpdateRangeAsync_Binh(productSales);

                //Không có sản phẩm duyệt thì kết thúc
                if (tableRows.Length == 0)
                {
                    return Ok(ApiResponseFactory.Success(null, ""));
                }

                string permission = "N108";

                var data = await SqlDapper<object>.ProcedureToListAsync(
                    "spGetEmailByUserGroup",
                    new
                    {
                        UserGroupCode = permission
                    });

                var emailList = data as List<dynamic>;

                var emails = emailList
                    .Select(x => (string)x.EmailCongTy)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();

                if (!emails.Any())
                {
                    return Ok(ApiResponseFactory.Success(null, $"Không tìm thấy email nhóm quyền {permission}"));
                }

                string emailTo = emails.First();

                string emailCc = string.Join(",",
                    emails
                        .Skip(1)
                        .Concat(emailCC)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct());

                //string emailTo = "tester01@rtc.edu.vn";
                //string emailCc = "tech62@rtc.edu.vn";

                string department = string.IsNullOrWhiteSpace(currentUser.DepartmentName)
                    ? ""
                    : $" - phòng {currentUser.DepartmentName}";

                string subject = "THÔNG BÁO SẢN PHẨM ĐÃ ĐƯỢC DUYỆT";

                string body = $@"
                    <div style='font-family:Arial,sans-serif;line-height:1.6'>
                        <h2 style='color:#28a745'>THÔNG BÁO SẢN PHẨM ĐÃ ĐƯỢC DUYỆT</h2>

                        <p>Kính gửi Anh/Chị,</p>

                        <p>
                            Các sản phẩm thêm mới đã được duyệt và có thể ycbg/ycmh từ POKH. Vui lòng xem chi tiết thông tin sản phẩm bảng bên dưới
                        </p>

                        <table style='border-collapse:collapse;width:100%;margin-top:15px'>
                            <thead>
                                <tr style='background:#f2f2f2'>
                                    <th style='border:1px solid #ddd;padding:8px'>STT</th>
                                    <th style='border:1px solid #ddd;padding:8px'>POKH liên quan</th>
                                    <th style='border:1px solid #ddd;padding:8px'>Mã nội bộ</th>
                                    <th style='border:1px solid #ddd;padding:8px'>Mã sản phẩm</th>
                                    <th style='border:1px solid #ddd;padding:8px'>Tên sản phẩm</th>
                                </tr>
                            </thead>
                            <tbody>
                                {tableRows}
                            </tbody>
                        </table>

                        <br/>

                        <p>
                            Vui lòng đăng nhập hệ thống <strong>R-ERP</strong> để tiếp tục thực hiện YCMH/YCBG.
                            <a href='https://erp.rtc.edu.vn/rerpweb/pokh-hn?warehouseId=1'
                               target='_blank'>
                                Truy cập ngay!
                            </a>.
                        </p>
                    </div>";

                await _emailHelper.SendAsync(emailTo,subject,body,true,emailCc);

                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("activity-log")]
        public IActionResult getActivityLog()
        {
            try
            {
                var rs = _productSaleImportExportLogRepo.GetAll(x => x.IsDeleted != true).OrderByDescending(x => x.CreatedDate);
                return Ok(ApiResponseFactory.Success(rs, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}
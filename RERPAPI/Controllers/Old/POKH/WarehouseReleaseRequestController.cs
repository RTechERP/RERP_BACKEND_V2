using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.Old.POKH
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarehouseReleaseRequestController : ControllerBase
    {
        private readonly ProductGroupRepo _productGroupRepo;
        private readonly WarehouseRepo _warehouseRepo;

        public WarehouseReleaseRequestController(ProductGroupRepo productGroupRepo, WarehouseRepo warehouseRepo)
        {
            _productGroupRepo = productGroupRepo;
            _warehouseRepo = warehouseRepo;
        }
        [HttpGet("get-pokh-export-request")]
        public IActionResult GetPOKHExportRequest(int warehouseId, int customerId, int projectId, int productGroupId, string keyword = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetPOKHRequestExport",
                    new string[] { "@WarehouseID", "@CustomerID", "@ProjectID", "@ProductGroupID", "@Keyword" },
                    new object[] { warehouseId, customerId, projectId, productGroupId, keyword});
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-productgroup")]
        public IActionResult GetProductGroup()
        {
            try
            {
                var data = _productGroupRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-warehouse")]
        public IActionResult GetWarehouse()
        {
            try
            {
                var data = _warehouseRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("validate-keep")]
        public IActionResult ValidateKeep([FromBody] ValidateKeepDTO request)
        {
            try
            {
                // 1. Nếu đơn vị được allow → bỏ qua validation
                if (!string.IsNullOrEmpty(request.UnitName))
                {
                    var unitName = request.UnitName.Trim().ToLower();
                    var allowUnits = new List<string> { "m", "mét" }; // thay bằng unitNames allow

                    if (allowUnits.Contains(unitName))
                    {
                        return Ok(ApiResponseFactory.Success(new { IsValid = true }, ""));
                    }
                }

                // 2. getdata
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList(
                    "spGetInventoryProjectImportExport",
                    new string[] { "@WarehouseID", "@ProductID", "@ProjectID", "@POKHDetailID", "@BillExportDetailID" },
                    new object[] { request.WarehouseID, request.ProductID, request.ProjectID, request.POKHDetailID, request.BillExportDetailID }
                );

                // SP trả về: 0 = Giữ, 1 = Import, 2 = Export, 3 = Tồn CK
                var inventoryProjects = SQLHelper<dynamic>.GetListData(list, 0); // giữ
                var dtImport = SQLHelper<dynamic>.GetListData(list, 1);
                var dtExport = SQLHelper<dynamic>.GetListData(list, 2);
                var dtStock = SQLHelper<dynamic>.GetListData(list, 3);

                // 3. Convert dynamic → decimal an toàn
                decimal totalQuantityKeep = inventoryProjects.Count == 0 ? 0 :
                    Convert.ToDecimal(inventoryProjects[0].TotalQuantity);

                decimal totalQuantityLast = dtStock.Count == 0 ? 0 :
                    Convert.ToDecimal(dtStock[0].TotalQuantityLast);

                decimal totalImport = dtImport.Count == 0 ? 0 :
                    Convert.ToDecimal(dtImport[0].TotalImport);

                decimal totalExport = dtExport.Count == 0 ? 0 :
                    Convert.ToDecimal(dtExport[0].TotalExport);

                decimal quantityRemain = totalImport - totalExport;
                if (quantityRemain < 0) quantityRemain = 0;

                decimal totalStock = totalQuantityKeep + quantityRemain + totalQuantityLast;

                // 4. Validate tồn kho
                if (request.QuantityRequestExport > totalStock)
                {
                    return Ok(ApiResponseFactory.Success(new
                    {
                        IsValid = false,
                        ProductNewCode = request.ProductNewCode,
                        Message =
                            $"Số lượng sản phẩm [{request.ProductNewCode}] không đủ!\n" +
                            $"SL xuất: {request.QuantityRequestExport}\n" +
                            $"SL giữ: {totalQuantityKeep} | Tồn CK: {totalQuantityLast} | Tổng: {totalStock}"
                    }, ""));
                }

                // 5. Hợp lệ
                return Ok(ApiResponseFactory.Success(new
                {
                    IsValid = true,
                    ProductNewCode = "",
                    Message = "VALID"
                }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("validate-keep-new")]
        public IActionResult ValidateKeepNew([FromBody] ValidateKeepNewDTO request)
        {
            try
            {
                var unitNames = new string[] { "m", "mét" };

                // Sắp xếp items theo QuantityRequestExport tăng dần
                var sortedItems = request.Items
                    .OrderBy(x => x.QuantityRequestExport)
                    .ToList();

                //Tính tổng QuantityRequestExport cho mỗi ProductID 
                Dictionary<int, decimal> productRequestedTotals = new Dictionary<int, decimal>();
                foreach (var item in sortedItems)
                {
                    string unitName = (item.UnitName ?? "").Trim().ToLower();
                    if (unitNames.Contains(unitName)) continue; // bỏ qua unitNames m/mét

                    int productID = item.ProductID;
                    if (productID <= 0) continue;

                    if (productRequestedTotals.ContainsKey(productID))
                        productRequestedTotals[productID] += item.QuantityRequestExport;
                    else
                        productRequestedTotals[productID] = item.QuantityRequestExport;
                }

                // Validate từng item theo thứ tự đã sắp xếp 
                Dictionary<int, decimal> totalQuantityRequest = new Dictionary<int, decimal>();
                Dictionary<int, decimal> totalQuantityStock = new Dictionary<int, decimal>();
                Dictionary<int, decimal> productRemainingStock = new Dictionary<int, decimal>();

                List<int> validSelected = new List<int>();
                List<string> invalidCodes = new List<string>();

                foreach (var item in sortedItems)
                {
                    string unitName = (item.UnitName ?? "").Trim().ToLower();
                    int productID = item.ProductID;
                    decimal qtyReq = item.QuantityRequestExport;

                    // Xử lý m/mét auto hợpleej
                    if (unitNames.Contains(unitName))
                    {
                        validSelected.Add(item.POKHDetailID);
                        continue;
                    }

                    if (productID <= 0) continue;

                    // Cộng dồn totalQuantityRequest
                    if (totalQuantityRequest.ContainsKey(productID))
                        totalQuantityRequest[productID] += qtyReq;
                    else
                        totalQuantityRequest[productID] = qtyReq;

                    var list = SQLHelper<dynamic>.ProcedureToList(
                        "spGetInventoryProjectImportExport",
                        new[] { "@WarehouseID", "@ProductID", "@ProjectID", "@POKHDetailID", "@BillExportDetailID" },
                        new object[] { request.WarehouseID, productID, 0, item.POKHDetailID, 0 });

                    var inventoryProjects = SQLHelper<dynamic>.GetListData(list, 0);
                    var dtImport = SQLHelper<dynamic>.GetListData(list, 1);
                    var dtExport = SQLHelper<dynamic>.GetListData(list, 2);
                    var dtStock = SQLHelper<dynamic>.GetListData(list, 3);

                    decimal totalQuantityKeep = inventoryProjects.Count == 0 ? 0 : Convert.ToDecimal(inventoryProjects[0].TotalQuantity);
                    totalQuantityKeep = Math.Max(totalQuantityKeep, 0);

                    decimal totalQuantityLast = dtStock.Count == 0 ? 0 : Convert.ToDecimal(dtStock[0].TotalQuantityLast);
                    totalQuantityLast = Math.Max(totalQuantityLast, 0);

                    decimal totalImport = dtImport.Count == 0 ? 0 : Convert.ToDecimal(dtImport[0].TotalImport);
                    totalImport = Math.Max(totalImport, 0);

                    decimal totalExport = dtExport.Count == 0 ? 0 : Convert.ToDecimal(dtExport[0].TotalExport);
                    totalExport = Math.Max(totalExport, 0);

                    decimal quantityRemain = totalImport - totalExport;
                    decimal quantityStock = Math.Max(totalQuantityKeep + quantityRemain + totalQuantityLast, 0);

                    // Cộng dồn totalQuantityStock
                    if (totalQuantityStock.ContainsKey(productID))
                        totalQuantityStock[productID] += quantityStock;
                    else
                        totalQuantityStock[productID] = quantityStock;

                    // Kiểm tra diffKeys 
                    var diffKeys = totalQuantityRequest.Keys
                        .Intersect(totalQuantityStock.Keys)
                        .Where(k => totalQuantityRequest[k] > totalQuantityStock[k]);

                    if (diffKeys.Count() > 0)
                    {
                        if (qtyReq > quantityStock)
                        {
                            // Invalid
                            if (!string.IsNullOrWhiteSpace(item.ProductNewCode))
                                invalidCodes.Add(item.ProductNewCode);
                        }
                        else
                        {
                            // Valid
                            validSelected.Add(item.POKHDetailID);
                            if (!productRemainingStock.ContainsKey(productID))
                                productRemainingStock[productID] = quantityStock;
                            productRemainingStock[productID] -= qtyReq;
                        }
                    }
                    else
                    {
                        // Không có diffKeys tự động hợp lệ
                        validSelected.Add(item.POKHDetailID);
                    }
                }

                return Ok(ApiResponseFactory.Success(new
                {
                    ValidSelected = validSelected.Distinct().ToList(),
                    InvalidProductCodes = invalidCodes.Distinct().ToList()
                }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        public class ValidateKeepNewDTO
        {
            public int WarehouseID { get; set; }
            public List<ValidateItemDTO> Items { get; set; } = new List<ValidateItemDTO>();
        }

        public class ValidateItemDTO
        {
            public int ProductID { get; set; }
            public int POKHDetailID { get; set; }
            public string UnitName { get; set; }
            public decimal QuantityRequestExport { get; set; }
            public string ProductNewCode { get; set; }
        }
    }
}

using ClosedXML.Excel;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.Drawing;
using System.Drawing.Imaging;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;


namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillExportController : ControllerBase
    {
        ProductGroupRepo _productgroupRepo = new ProductGroupRepo();

        BillDocumentExportRepo _billdocumentexportRepo = new BillDocumentExportRepo();
        BillExportDetailRepo _billExportDetailRepo = new BillExportDetailRepo();
        BillExportRepo _billexportRepo = new BillExportRepo();
        InventoryRepo _inventoryRepo = new InventoryRepo();
        InventoryProjectExportRepo _inventoryprojectexportRepo = new InventoryProjectExportRepo();
        BillExportDetailSerialNumberRepo _billexportdetailserialnumberRepo = new BillExportDetailSerialNumberRepo();
        DocumentExportRepo _documentexportRepo = new DocumentExportRepo();
        BillExportLogRepo _billexportlogRepo = new BillExportLogRepo();
        ProjectRepo _projectRepo = new ProjectRepo();
        HistoryDeleteBillRepo _historyDeleteBillRepo = new HistoryDeleteBillRepo();
        BillExportDetailSerialNumberRepo _billExportDetailSerialNumberRepo = new BillExportDetailSerialNumberRepo();
        WarehouseRepo _warehouseRepo = new WarehouseRepo();
        BillExportDetailSerialNumberRepo billExportDetailSerialNumberRepo = new BillExportDetailSerialNumberRepo();
        [HttpGet("get-all-project")]
        public IActionResult getAllProject()
        {
            try
            {
                var result = _projectRepo.GetAll().OrderByDescending(x => x.ID);
                return Ok(new
                {
                    status = 1,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                });
            }
        }
        /*   [HttpGet("get-product")]
           public IActionResult getProductSale(int id)
           {
               try
               {
                   List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                          "spGetInventory", new string[] { "@ID", "@Find", "@WarehouseCode", "@IsStock", "@IsDeleted" },
                       new object[] { id, "", "HN", false, false }
                      );
                   return Ok(new
                   {
                       status = 1,
                       data = SQLHelper<object>.GetListData(result, 0)
                   });
               }
               catch (Exception ex)
               {
                   return BadRequest(new
                   {
                       status = 0,
                       message = ex.Message,
                       error = ex.ToString()
                   });
               }
           }*/
        //done
        [HttpPost("")]
        public IActionResult getBillExport([FromBody] BillExportParamRequest filter)
        {
            try
            {

                if (filter.checkedAll == true)
                {
                    filter.DateStart = new DateTime(1990, 01, 01);
                    filter.DateEnd = new DateTime(9999, 01, 01);
                }
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "spGetBillExport_New", new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@KhoType", "@FilterText", "@WarehouseCode" },
                    new object[] { filter.PageNumber, filter.PageSize, filter.DateStart, filter.DateEnd, filter.Status, filter.KhoType, filter.FilterText, filter.WarehouseCode }
                   );
                /* List<dynamic> billList = result[0]; // dữ liệu hóa đơn*/
                int totalPage = 0;

                if (result.Count > 1 && result[1].Count > 0)
                {
                    totalPage = (int)result[1][0].TotalPage;
                }

                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0),
                    totalPage = totalPage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("get-product")]
        public IActionResult getOptionProduct(string warehouseCode, int productGroupID)
        {
            try
            {
                /*  var warehouse = _warehouseRepo.GetByID(warehouseID);*/
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "spGetInventory", new string[] { "@ID", "@Find", "@WarehouseCode" },
                    new object[] { productGroupID, "", warehouseCode }
                   );
                /* List<dynamic> billList = result[0]; // dữ liệu hóa đơn*/
                int totalPage = 0;

                if (result.Count > 1 && result[1].Count > 0)
                {
                    totalPage = (int)result[1][0].TotalPage;
                }

                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0),
                    totalPage = totalPage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        //chi tiết phiếu xuất
        [HttpGet]
        public IActionResult getProductGroup(bool isAdmin, int departmentID)
        {
            try
            {
                List<ProductGroup> listPG;

                if (isAdmin)
                {
                    if (departmentID == 6)
                    {
                        listPG = _productgroupRepo.GetAll().Where(x => x.ProductGroupID == "C").ToList();
                    }
                    else
                    {
                        listPG = _productgroupRepo.GetAll().Where(x => x.ProductGroupID != "C").ToList();
                    }
                }
                else
                {
                    // Nếu không phải admin, có thể xử lý mặc định hoặc trả toàn bộ danh sách chẳng hạn
                    listPG = _productgroupRepo.GetAll().ToList();
                }

                return Ok(new
                {
                    status = 1,
                    data = listPG,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        private async Task<(bool IsValid, string ErrorMessage)> ValidateBillExport(BillExportDTO dto)
        {
            // Validate Code
            if (string.IsNullOrWhiteSpace(dto.billExport?.Code))
            {
                return (false, "Xin hãy điền số phiếu.");
            }

            // Check for duplicate code
            var existingBill = _billexportRepo.GetAll().Where(p => p.Code == dto.billExport.Code).FirstOrDefault();
            if (dto.billExport.ID > 0)
            {
                // For update: check if code exists for a different ID
                if (existingBill != null && existingBill.ID != dto.billExport.ID)
                {
                    return (false, "Số phiếu này đã tồn tại. Vui lòng chọn số phiếu khác!");
                }
            }
            else
            {
                // For create: check if code already exists
                if (existingBill != null)
                {
                    // Optionally generate new code here if loadBillNumber equivalent is needed
                    return (false, $"Phiếu đã tồn tại. Vui lòng chọn số phiếu khác hoặc tải lại số phiếu!");
                }
            }

            // Validate Customer or Supplier
            if ((dto.billExport.CustomerID <= 0 || dto.billExport.CustomerID == null) &&
                (dto.billExport.SupplierID <= 0 || dto.billExport.SupplierID == null))
            {
                return (false, "Xin hãy chọn Khách hàng hoặc Nhà cung cấp!");
            }

            // Validate Warehouse Type
            if (string.IsNullOrWhiteSpace(dto.billExport.WarehouseType))
            {
                return (false, "Xin hãy chọn kho quản lý.");
            }

            /*  // Validate Group
              if (string.IsNullOrWhiteSpace(dto.billExport.GroupID))
              {
                  return (false, "Xin hãy chọn nhóm.");
              }*/

            // Validate Sender
            if (dto.billExport.SenderID == null)
            {
                return (false, "Xin hãy chọn người giao.");
            }

            // Validate Status
            if (dto.billExport.Status < 0)
            {
                return (false, "Xin hãy chọn trạng thái.");
            }

            /*   // Validate Creation Date
               if (dto.billExport.Status != 6 && !dto.billExport.CreatedDate.HasValue)
               {
                   return (false, "Xin hãy chọn Ngày xuất!");
               }*/

            return (true, string.Empty);
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> saveDataBillExport([FromBody] BillExportDTO dto)
        {
            try
            {
                // validation
                var (isValid, errorMessage) = await ValidateBillExport(dto);
                if (!isValid)
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = errorMessage
                    });
                }
                // trường hợp xóa 
                if (dto.billExportDetail == null && dto.DeletedDetailIDs == null)
                {
                    // Cập nhật phiếu nhập

                    dto.billExport.UpdatedDate = DateTime.Now;
                    _billexportRepo.Update(dto.billExport);
                    return Ok(new
                    {
                        status = 1,
                        message = "Đã xóa thành công phiếu " + dto.billExport.Code

                    });
                }

                /*  var inventoryLookup = _inventoryRepo.GetAll()
                                            .ToDictionary(x => new { x.WarehouseID, x.ProductSaleID });*/
                var inventoryList = _inventoryRepo.GetAll().ToList();

                // ======= TÍNH TOTAL QTY CHUNG ======= //
                var groupedQuantities = dto.billExportDetail.GroupBy(d => d.ProductID).ToDictionary(g => g.Key, g => g.Sum(d => d.Qty));

                foreach (var detail in dto.billExportDetail)
                {
                    if (groupedQuantities.ContainsKey(detail.ProductID))
                    {
                        detail.TotalQty = groupedQuantities[detail.ProductID];
                    }
                }

                //start
                int billExportId;
                if (dto.billExport.ID <= 0)  // Thêm mới phiếu xuất
                {
                    dto.billExport.IsMerge = false;
                    dto.billExport.UnApprove = 0;
                    dto.billExport.IsDeleted = false;
                    await _billexportRepo.CreateAsync(dto.billExport);
                    billExportId = dto.billExport.ID;

                    //thêm chứng từ
                    var listID = _documentexportRepo.GetAll().Where(x => x.IsDeleted == false).ToList();
                    foreach (var item in listID)
                    {
                        BillDocumentExport billDocumentExport = new BillDocumentExport
                        {
                            BillExportID = billExportId,
                            DocumentExportID = item.ID,
                            Status = 0,
                            LogDate = DateTime.Now,
                            Note = ""
                        };
                        await _billdocumentexportRepo.CreateAsync(billDocumentExport);
                    }
                }
                else // Cập nhật phiếu xuất
                {
                    dto.billExport.UpdatedDate = DateTime.Now;
                    _billexportRepo.Update(dto.billExport);
                    billExportId = dto.billExport.ID;
                }
                //chi tiết phiếu xuất
                foreach (var detail in  dto.billExportDetail)
                {
                    detail.BillID = billExportId;

                    // Nếu chi tiết chưa có ID => thêm mới
                    if (detail.ID <= 0)
                    {
                        detail.IsDeleted = false;
                        await _billExportDetailRepo.CreateAsync(detail);
                        int billExportDetailID = detail.ID;
                    }
                    else
                    {
                        // Cập nhật
                        var existingDetail = _billExportDetailRepo.GetByID(detail.ID);
                        if (existingDetail != null)
                        {
                                _billExportDetailRepo.Update(detail);
                        }
                    }
                    //serial
                    // Update serial với BillExportDetailID
                    if (detail.SerialNumber != null && detail.SerialNumber.Any())
                    {
                        // Lấy serial từ SerialNumbers, kiểm tra CreatedDate gần nhất để tránh xung đột
                        var serialIds = detail.SerialNumber
                                .Split(',', StringSplitOptions.RemoveEmptyEntries) // tách theo dấu phẩy
                                .Select(x => int.Parse(x.Trim())) // chuyển sang int
                                .ToList();
                        var serials = _billExportDetailSerialNumberRepo.GetAll()
                            .Where(s => serialIds.Contains(s.ID) &&
                                        s.IsDeleted != true &&
                                        s.BillExportDetailID == null)            
                            .ToList();

                        if (detail.Qty.HasValue && serials.Count != (int)detail.Qty)
                        {
                            return BadRequest(new { 
                                status = 0,
                                message = $"Số serial ({serials.Count}) không khớp Qty ({detail.Qty})" 
                            });
                        }
                        // Update SerialNumber nếu chưa có
                        if (string.IsNullOrEmpty(detail.SerialNumber))
                        {
                            detail.SerialNumber = serials.Any() ? string.Join(",", serials.Select(s => s.SerialNumber)) : null;
                            _billExportDetailRepo.Update(detail);
                        }

                        // Cập nhật BillExportDetailID
                        foreach (var serial in serials)
                        {
                            //.BillExportDetailID = detail.ID;
                            serial.UpdatedDate = DateTime.Now;
                            _billExportDetailSerialNumberRepo.Update(serial);
                        }
                    }


                    if (dto.DeletedDetailIDs != null && dto.DeletedDetailIDs.Any())
                    {
                        foreach (var id in dto.DeletedDetailIDs)
                        {
                            var item = _billExportDetailRepo.GetByID(id);
                            if (item != null)
                            {
                                item.IsDeleted = true;
                                item.UpdatedDate = DateTime.Now;
                                _billExportDetailRepo.Update(item);

                            }
                        }
                        for (int j = 0; j < dto.DeletedDetailIDs.Count; j++)
                        {
                            var bxSn = _billExportDetailSerialNumberRepo.GetAll().Where(p => p.BillExportDetailID == dto.DeletedDetailIDs[j]).FirstOrDefault();
                            if (bxSn != null)
                            {
                                bxSn.IsDeleted = true;
                                bxSn.UpdatedDate = DateTime.Now;
                                _billExportDetailSerialNumberRepo.Update(bxSn);
                            }


                        }
                    }

                    // Kiểm tra tồn kho
                    bool exists = inventoryList.Any(x =>
                        x.WarehouseID == dto.billExport.WarehouseID &&
                        x.ProductSaleID == detail.ProductID);
                    if (!exists)
                    {
                        Inventory inventory = new Inventory
                        {
                            WarehouseID = dto.billExport.WarehouseID,
                            ProductSaleID = detail.ProductID,
                            TotalQuantityFirst = 0,
                            TotalQuantityLast = 0,
                            Import = 0,
                            Export = 0
                        };
                        await _inventoryRepo.CreateAsync(inventory);
                    }

                    // ====== XỬ LÝ LIÊN KẾT DỰ ÁN ======
                    var oldProjects = _inventoryprojectexportRepo.GetAll()
                        .Where(x => x.BillExportDetailID == detail.ID && x.InventoryProjectID == 0)
                        .ToList();

                    /*     var newProjectIds = detail.InventoryProjectIDs ?? new List<int>();*/

                    // 1. Xóa mềm các dự án không còn
                    foreach (var old in oldProjects)
                    {
                        old.IsDeleted = true;
                        await _inventoryprojectexportRepo.UpdateAsync(old);
                    }

                    // 2. Thêm mới nếu chưa có

                    bool existsInOld = oldProjects.Any(x => x.InventoryProjectID == 0 && x.IsDeleted == false);
                    if (!existsInOld)
                    {
                        InventoryProjectExport projectExport = new InventoryProjectExport
                        {
                            BillExportDetailID = detail.ID,
                            InventoryProjectID = 0,
                            UpdatedDate = DateTime.Now,
                            IsDeleted = false
                        };
                        await _inventoryprojectexportRepo.CreateAsync(projectExport);
                    }
                }

                return Ok(new
                {
                    status = 1,
                    message = "Xử lý thành công",
                    data = dto
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpGet("{id}")]
        public IActionResult getBillExportByID(int id)
        {
            try
            {
                BillExport result = _billexportRepo.GetByID(id);
                /*   var newCode = _billexportRepo.GetBillCode()*/
                return Ok(new
                {
                    status = 1,
                    data = result,
                    /*  newCode = newCode,*/
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpPost("delete-bill-export")]
        public async Task<IActionResult> deleteBillExport([FromBody] BillExport billExport)
        {
            try
            {
                if (billExport.IsApproved == true)
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Không thể xóa {" + billExport.Code + "} do đã nhận chứng từ!"
                    });
                }
                else
                {
                    billExport.IsDeleted = true;
                    await _billexportRepo.UpdateAsync(billExport);
                    HistoryDeleteBill historyDeleteBill = new HistoryDeleteBill
                    {
                        BillID = billExport.ID,
                        UserID = billExport.UserID,
                        DeleteDate = DateTime.Now,
                        TypeBill = billExport.Code,
                    };
                    await _historyDeleteBillRepo.CreateAsync(historyDeleteBill);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Đã xóa thành công mã phiếu {" + billExport.Code + "}!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                });

            }
        }

        [HttpPost("approved")]

        public async Task<IActionResult> Approved([FromBody] BillExport billExport, bool isapproved)
        {
            try
            {
                if (billExport.IsApproved == false && isapproved == false)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = $"{billExport.Code} chưa nhận chứng từ!",
                    });
                }
                billExport.IsApproved = isapproved;
                await _billexportRepo.UpdateAsync(billExport);

                SQLHelper<dynamic>.ExcuteProcedure("spCalculateExport_New", new string[] { "@ID", "@WarehouseID" }, new object[] { billExport.ID, billExport.WarehouseID });
                SQLHelper<dynamic>.ExcuteProcedure("spCalculatePOKH", new string[] { "@IDMaster" }, new object[] { billExport.ID });

                BillExportLog log = new BillExportLog()
                {
                    BillExportID = billExport.ID,
                    StatusBill = isapproved,
                    DateStatus = DateTime.Now,
                };
                await _billexportlogRepo.CreateAsync(log);
                return Ok(new
                {
                    status = 1,
                    message = $"{billExport.Code} {(isapproved ? "đã được nhận chứng từ thành công!" : "đã được hủy nhận chứng từ!")}"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        #region
        [HttpGet("get-bill-code")]
        public ActionResult<string> LoadBillNumber(int billTypeId, int? billId = null, int? currentStatus = null, string currentCode = null)
        {
            string billCode = string.Empty;
            string date = DateTime.Now.ToString("yyMMdd");

            if (billId.HasValue && billId.Value > 0 && !string.IsNullOrEmpty(currentCode) && currentStatus.HasValue)
            {
                string newPrefix = GetPrefix(billTypeId);
                string oldPrefix = GetPrefix(currentStatus.Value);
                billCode = currentCode.Replace(oldPrefix, newPrefix);
                return Ok(new { data = billCode });
            }

            string prefix = GetPrefix(billTypeId);
            int serial = 1;

            while (true)
            {
                string nextSerial = serial.ToString().PadLeft(3, '0');
                string candidateCode = prefix + date + nextSerial;

                // Kiểm tra trùng mã trong DB
                bool exists = _billexportRepo.GetAll().Any(x => x.Code == candidateCode && x.IsDeleted == false);
                if (!exists)
                {
                    billCode = candidateCode;
                    break;
                }

                serial++; // nếu trùng, tăng tiếp
            }

            return Ok(new { data = billCode });
        }

        // Xác định tiền tố mã phiếu dựa vào trạng thái
        private string GetPrefix(int billType)
        {
            return billType switch
            {
                0 => "PM",
                3 => "PCT",
                4 => "PMNB",
                5 => "PXM",
                _ => "PXK",
            };
        }

        // Truy vấn CSDL để lấy mã phiếu gần nhất trong ngày
        private string GetLastBillCodeToday()
        {
            // TODO: Bạn có thể thay đoạn này bằng truy vấn CSDL thật bằng Dapper/EF
            /*
            string sql = @"SELECT TOP 1 Code 
                           FROM BillExport 
                           WHERE DAY(CreatedDate) = @day 
                             AND MONTH(CreatedDate) = @month 
                             AND YEAR(CreatedDate) = @year 
                           ORDER BY ID DESC";
            */
            return ""; // giả lập chưa có mã nào trong ngày
        }

        #endregion
        //đã xuất kho 
        [HttpPost("shipped-out")]
        public async Task<IActionResult> ExportWareHouse([FromBody] BillExport billExport)
        {
            try
            {
                if (billExport.Status == 0 || billExport.Status == 1)
                {
                    billExport.Status = 2;
                    await _billexportRepo.UpdateAsync(billExport);
                }
                else
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = $"Vui lòng kiểm tra lại trạng thái phiếu xuất {billExport.Code} "

                    });
                }
                return Ok(new
                {
                    status = 1,
                    message = $"{billExport.Code} Đã cập nhật!"
                });
            }
            catch (Exception ex)

            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpGet("export-excel")]
        public IActionResult ExportExcel(int id, int type)
        {
            try
            {
                List<List<dynamic>> resultSets = SQLHelper<dynamic>.ProcedureToList(
                    "spGetExportExcel",
                    new string[] { "@ID" },
                    new object[] { id }
                );



                if (resultSets == null || resultSets.Count == 0 || resultSets[0].Count == 0)
                {
                    return BadRequest(new { status = 0, message = "Không có dữ liệu từ spGetExportExcel" });
                }

                var allData = resultSets[0];
                var masterData = allData.FirstOrDefault();


                if (masterData == null)
                {
                    return BadRequest(new { status = 0, message = "Không tìm thấy dữ liệu master" });
                }

                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "PhieuXuatSALE.xlsx");
                if (!System.IO.File.Exists(templatePath))
                {
                    return BadRequest(new { status = 0, message = "Không tìm thấy file mẫu Excel" });
                }

                using (var workbook = new XLWorkbook(templatePath))
                {
                    var sheet = workbook.Worksheet(1);

                    // Mapping dữ liệu Master
                    sheet.Cell(6, 1).Value = $"Số: {masterData.Code}";
                    sheet.Cell(9, 4).Value = string.IsNullOrWhiteSpace((string)masterData.FullName)
                        ? masterData.FullName
                        : $"{masterData.FullName}";

                    string customerName = masterData.CustomerName?.ToString()?.Trim() ?? "";
                    string supplierName = masterData.NameNCC?.ToString()?.Trim() ?? "";

                    sheet.Cell(10, 3).Value = "'- Khách hàng/Nhà cung cấp:";
                    sheet.Cell(10, 4).Value = !string.IsNullOrEmpty(customerName) ? customerName : supplierName;

                    sheet.Cell(11, 4).Value = masterData.Address?.ToString() ?? "";
                    sheet.Cell(12, 4).Value = masterData.AddressStock?.ToString() ?? "";

                    DateTime? creatDate = masterData.CreatDate != null ? Convert.ToDateTime(masterData.CreatDate) : (DateTime?)null;
                    if (creatDate.HasValue)
                    {
                        sheet.Cell(18, 9).Value = $"Ngày {creatDate.Value:dd} tháng {creatDate.Value:MM} năm {creatDate.Value:yyyy}";
                    }

                    sheet.Cell(25, 3).Value = masterData.FullNameSender?.ToString() ?? "";
                    sheet.Cell(25, 9).Value = masterData.FullName?.ToString() ?? "";

                    List<List<dynamic>> resultDetail = SQLHelper<dynamic>.ProcedureToList(
                  "spGetBillExportDetail",
                  new string[] { "@BillID" },
                  new object[] { id }
              );
                    var detailList = resultDetail[0]
                            .Cast<IDictionary<string, object>>()
                             .ToList();

                    // Mapping dữ liệu chi tiết
                    int startRow = 15;
                    int currentRow = startRow;
                    int stt = 1;
                    // Chỉ chèn thêm dòng nếu có nhiều hơn 1 mục chi tiết,
                    // vì file mẫu đã có sẵn 1 dòng để sử dụng.
                    if (detailList.Count > 1)
                    {
                        // Ví dụ: có 5 mục chi tiết -> chèn thêm 4 dòng.
                        sheet.Row(startRow).InsertRowsBelow(detailList.Count - 1);
                    }
                    if (detailList.Any())
                    {
                        foreach (var item in detailList)
                        {
                            int parentId = Convert.ToInt32(item["ParentID"] ?? 0);

                            if (type == 1 && parentId != 0) continue; // giống Winform: nếu type=1 thì bỏ qua dòng con

                            sheet.Cell(currentRow, 1).Value = stt++;
                            sheet.Cell(currentRow, 2).Value = item["ProductNewCode"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 3).Value = item["ProductCode"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 4).Value = item["ProductFullName"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 5).Value = item["ProductName"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 6).Value = item["Unit"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 7).Value = Convert.ToDecimal(item["Qty"] ?? 0);
                            sheet.Cell(currentRow, 8).Value = item["ProjectCodeText"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 9).Value = item["ProjectNameText"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 10).Value = item["ProductTypeText"]?.ToString() ?? "";

                            sheet.Cell(currentRow, 11).Value = (masterData.WarehouseID?.ToString() == "1")
                                ? item["ProductGroupName"]?.ToString() ?? ""
                                : item["WarehouseName"]?.ToString() ?? "";

                            string note = item["Note"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 12).Value = note.StartsWith("=") ? $"'{note}" : note;

                            currentRow++;
                        }
                    }
                    else // Nếu không có dòng chi tiết nào
                    {
                        // Xoá dòng mẫu đi
                        sheet.Row(startRow).Delete();
                    }

                    using (var stream = new MemoryStream())
                    {

                        // --- TẠO QR CODE bằng ZXing.Net ---
                        string qrCodeText = masterData?.Code?.ToString() ?? "Unknown";

                        var writer = new BarcodeWriterPixelData
                        {
                            Format = BarcodeFormat.QR_CODE,
                            Options = new EncodingOptions
                            {
                                Height = 250,
                                Width = 250,
                                Margin = 1
                            }
                        };

                        var pixelData = writer.Write(qrCodeText);

                        using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
                        {
                            var bitmapData = bitmap.LockBits(
                                new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                                ImageLockMode.WriteOnly,
                                PixelFormat.Format32bppRgb);

                            try
                            {
                                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                            }
                            finally
                            {
                                bitmap.UnlockBits(bitmapData);
                            }

                            // Lưu vào tệp tạm
                            string tempPath = Path.Combine(Path.GetTempPath(), $"qr_{Guid.NewGuid()}.png");
                            bitmap.Save(tempPath, ImageFormat.Png);

                            // Thêm ảnh vào Excel
                            sheet.AddPicture(tempPath)
       .MoveTo(sheet.Cell(1, 10), 20, 10)  // Đặt tại ô (1,10), lệch phải 20px, xuống 10px
       .WithSize(120, 120);

                            System.IO.File.Delete(tempPath);
                        }

                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"{masterData.Code}_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = "Lỗi khi xuất Excel",
                    error = ex.ToString()
                });
            }
        }

        [HttpPost("bill-export-synthetic")]
        public IActionResult getBillExportSynthetic([FromBody] BillExportSyntheticParamRequest filter)
        {
            try
            {

                if (filter.checkedAll == true)
                {
                    filter.DateStart = new DateTime(1990, 01, 01);
                    filter.DateEnd = new DateTime(9999, 01, 01);
                }
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                  "spGetBillExportSynthetic",
                 new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@KhoType", "@FilterText", "@WarehouseCode", "@IsDeleted" },
                    new object[] { filter.PageNumber, filter.PageSize, filter.DateStart, filter.DateEnd, filter.Status, filter.KhoType, filter.FilterText, filter.WarehouseCode, filter.IsDeleted }
                   );
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }

        }

        [HttpGet("scan")]
        public IActionResult ScanQRCode(string code, int warehouseId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return BadRequest(new { status = 0, message = "Mã QR không được để trống." });

                // 1. Tìm phiếu có mã code
                var bills = SQLHelper<BillExport>.FindByAttribute("Code", $"'{code}'"); // nhớ thêm nháy đơn
                var bill = bills.FirstOrDefault();

                if (bill == null)
                    return NotFound(new { status = 0, message = $"Không tìm thấy phiếu với mã {code}." });

                // 2. Kiểm tra nếu là phiếu không được quét
                string tableName = typeof(BillExport).Name.Replace("Model", "");
                int billTypeNew = tableName == "BillImport" ? 4 : (tableName == "BillImportTechnical" ? 5 : 0);

                if (billTypeNew != 0)
                {
                    var check = SQLHelper<BillExport>.FindByAttribute("Code", $"'{code}'")
                        .Where(x => x.Status == billTypeNew)
                        .FirstOrDefault();

                    if (check != null)
                        return BadRequest(new { status = 0, message = "Không thể quét phiếu loại Yêu cầu nhập kho." });
                }

                // 3. Gọi store procedure lấy chi tiết phiếu
                var result = SQLHelper<BillExport>.ProcedureToList(
                    "spGetBillExportScanQR",
                    new string[] { "@FilterText", "@WareHouseId" },
                    new object[] { code, warehouseId }
                );

                var detailList = SQLHelper<BillExport>.GetListData(result, 0); // lấy bảng đầu tiên

                if (detailList == null || detailList.Count == 0)
                {
                    return NotFound(new { status = 0, message = "Không có chi tiết phiếu trong kho." });
                }

                return Ok(new
                {
                    status = 1,
                    message = "Thành công",
                    data = detailList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 0, message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        // hàm lấy danh sách vật tư theo dự án 

        [HttpPost("get-product-project")]

        public IActionResult getproductProject(GetListProductByProjectPram filter)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetListProductImportExportByProjectID_New",
                    new string[] { "@projectId", "@projectCode", "@WarehouseCode" },
                    new object[] { filter.projectID, filter.projectCode, filter.WarehouseCode }
                    );
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    error = ex.Message
                });
            }
        }




    }

}
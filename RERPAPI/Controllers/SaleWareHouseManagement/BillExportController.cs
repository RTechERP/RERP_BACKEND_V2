using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using System.Collections.Generic;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillExportController : ControllerBase
    {
        ProductGroupRepo _productgroupRepo = new ProductGroupRepo();

        BillDocumentExportRepo _billdocumentexportRepo= new BillDocumentExportRepo();
        BillExportDetailRepo _billexportdetailRepo = new BillExportDetailRepo();
        BillExportRepo _billexportRepo = new BillExportRepo();
        InventoryRepo _inventoryRepo = new InventoryRepo();
        InventoryProjectExportRepo _inventoryprojectexportRepo = new InventoryProjectExportRepo(); 
        BillExportDetailSerialNumberRepo _billexportdetailserialnumberRepo = new BillExportDetailSerialNumberRepo();
        DocumentExportRepo _documentexportRepo = new DocumentExportRepo();
        BillExportLogRepo _billexportlogRepo = new BillExportLogRepo();

        //done
        [HttpPost("")]
        public IActionResult getBillExport([FromBody] BillExportParamRequest filter)
        {
            try
            {
              
                if (filter.checkedAll == true)
                {
                    filter.DateStart = new DateTime(1990, 01, 01);
                    filter.DateEnd = new DateTime(9999,01,01);
                }
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "spGetBillExport_New", new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@KhoType", "@FilterText", "@WarehouseCode" },
                    new object[] { filter.PageNumber, filter.PageSize, filter.DateStart, filter.DateEnd, filter.Status, filter.KhoType, filter.FilterText, filter.WarehouseCode }
                   );
                List<dynamic> billList = result[0]; // dữ liệu hóa đơn
                int totalPage = 0;

                if (result.Count > 1 && result[1].Count > 0)
                {
                    totalPage = (int)result[1][0].TotalPage;
                }

                return Ok(new
                {
                    status = 1,
                    data = billList,
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

        [HttpPost("save-data")]
        public async Task<IActionResult> saveDataBillExport([FromBody] BillExportDTO dto )
        {
            try
            {
                var inventoryList = _inventoryRepo.GetAll().ToList();// list invenptory để kiểm tra productId và warehouse sản phẩm trong phiếu xuất có tồn kho không

                if (dto.billExport.ID <= 0)
                {
                    // Tạo mới phiếu xuất
                    int newId = await _billexportRepo.CreateAsync(dto.billExport); // lấy ra id của phiếu xuất mới
                    List<DocumentExport> listID = _documentexportRepo.GetAll().Where(x=>x.IsDeleted ==false).ToList(); // lấy ra id của các chứng từ có isdelete =fale =0
                    
                    foreach(var item in listID)
                    {
                        //lấy newID = BillExportID và các id của chứng từ = documentExportID trong bảng  BillDocumentExport để gán các chứng từ cho phiếu xuất BilExport
                        BillDocumentExport billDocumentExport = new BillDocumentExport()
                        {
                            BillExportID = newId,
                            DocumentExportID = item.ID,
                            Status = 0,
                            LogDate = DateTime.Now,
                            Note = "",
                        };
                        await _billdocumentexportRepo.CreateAsync(billDocumentExport);
                    }
                    // thêm từng dòng sản phẩm (từng bản ghi BillExportDetail) cho phiếu xuất
                    foreach (var detail in dto.billExportDetail)
                    {
                        detail.BillID = newId;
                        await _billexportdetailRepo.CreateAsync(detail);

                        //check xem chi tiết sản phẩm có tồn kho hay không ( inventory không thực hiện lấy ở đây vì có hàm getall() để trong vòng for sẽ giảm hiệu năng
                        bool exists = inventoryList.Any(x => x.WarehouseID == dto.billExport.WarehouseID && x.ProductSaleID == detail.ProductID);

                        //nếu chưa có thì thêm mới trong tồn kho
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
                        if (detail.InventoryProjectIDs != null && detail.InventoryProjectIDs.Any())
                        {
                            foreach (var projectId in detail.InventoryProjectIDs)
                            {
                                InventoryProjectExport projectExport = new InventoryProjectExport
                                {
                                    BillExportDetailID = detail.ID,
                                    InventoryProjectID = projectId,
                                };
                                await _inventoryprojectexportRepo.CreateAsync(projectExport);
                            }
                        }
                    }
                }
                else
                {
                    // Cập nhật
                    _billexportRepo.UpdateFieldsByID(dto.billExport.ID, dto.billExport);
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

        [HttpPost("approved")]

        public async Task<IActionResult> Approved([FromBody] BillExport billExport,bool isapproved)
        {
            try {
                   string message = isapproved ? "nhận chứng từ" : "hủy nhận chứng từ"; 
                if(billExport.IsApproved ==false && isapproved == false)
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = $"{billExport.Code} chưa nhận chứng từ!",
                  
                    });
                }
                billExport.IsApproved = isapproved;
                await _billexportRepo.UpdateAsync(billExport);
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
            }catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        //đã xuất kho 
        [HttpPost("shipped-out")]
        public async Task<IActionResult> ExportWareHouse([FromBody] BillExport billExport)
        {
            try
            {
                if(billExport.Status ==0 || billExport.Status == 1)
                {
                    billExport.Status = 2;
                    await _billexportRepo.UpdateAsync(billExport);
                }
                else
                {
                    return BadRequest(new
                    {
                        status=0,
                        message= $"Vui lòng kiểm tra lại trạng thái phiếu xuất {billExport.Code} "

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
    }
}

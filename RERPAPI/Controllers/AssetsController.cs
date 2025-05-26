    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using RERPAPI.Model.Entities;
    using RERPAPI.Model.Common;
    using RERPAPI.Model.DTO;
    using RERPAPI.Repo.GenericEntity;
    using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
    using System;
    using static System.Runtime.InteropServices.JavaScript.JSType;
    using Microsoft.EntityFrameworkCore;
    using RERPAPI.Model.Context;
using Microsoft.AspNetCore.Http.HttpResults;

    namespace RERPAPI.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class AssetsController : ControllerBase
        {
       TSAssetRecoveryRepo tSAssetRecoveryRepo = new TSAssetRecoveryRepo();
        TSLostReportAssetRepo tslostreport = new TSLostReportAssetRepo();
             TSAllocationEvictionAssetRepo tSAllocationEvictionrepo = new TSAllocationEvictionAssetRepo();
           TSReportBrokenAssetRepo reportrepo = new TSReportBrokenAssetRepo();
            TSStatusAssetRepo tSStatusAssetRepo = new TSStatusAssetRepo();
            TTypeAssetsRepo typerepo = new TTypeAssetsRepo();
        
           TSAssetManagementRepo tasset = new TSAssetManagementRepo();
            TSSourceAssetsRepo tssourcerepo = new TSSourceAssetsRepo();
            TSAssetAllocationRepo tSAssetAllocationRepo = new TSAssetAllocationRepo();
       
            TSAssetAllocationDetailRepo tSAssetAllocationDetailRepo = new TSAssetAllocationDetailRepo();
            [HttpGet("getallassetsmanagement")]
            public IActionResult GetAllAsset()
            {
                try
                {
                    List<TSAssetManagement> tSAssetManagements = tasset.GetAll();
                    var maxSTT = tSAssetManagements
                              .Where(x => x.STT.HasValue)
                              .Max(x => x.STT);
                    return Ok(new
                    {
                  
                        status = 1,
                        data =  maxSTT 
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
            [HttpGet("getall")]
            public IActionResult GetAll()
        {
            try {
                    List<TSSourceAsset> tSSources = tssourcerepo.GetAll();
                    return Ok(new
                    {
                        status = 1,
                        data = tSSources
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
        [HttpPost("UpdateApprovalStatus")]
        public async Task<IActionResult> UpdateApprovalStatus([FromBody] ApproveActionDto dto)
        {
            try
            {
                if (dto.IDs == null || dto.IDs.Count == 0)
                    return BadRequest("Danh sách ID không hợp lệ.");
                int updatedCount = 0;
                string tb = "";
                foreach (var id in dto.IDs)
                {
                    var item = tSAssetAllocationRepo.GetByID(id);
                    if (item == null)
                        continue;
                    bool updated = false;
                    switch (dto.Action)
                    {
                        case "HR_APPROVE":
                            if(item.IsApprovedPersonalProperty==false||item.IsApprovedPersonalProperty==null)
                            {
                                return Ok(new
                                {
                                    status = 0,
                                    tb = $"Biên bản số {id} chưa được cá nhân duyệt"

                                });
                            }    
                            if (item.Status == 0 && item.IsApproveAccountant == true)
                            {
                                item.Status = 1;
                                item.DateApprovedHR = DateTime.Now;
                                updated = true;
                            }
                            break;
                        case "HR_CANCEL":
                            if (item.IsApproveAccountant == true)
                            {
                                return Ok(new
                                {
                                    status = 0,
                                    tb = $"Biên bản số {id} không thể hủy duyệt khi kế toán đã duyệt"

                                });
                            }
                            if (item.Status == 1 && item.IsApproveAccountant == false)
                            {
                                item.Status = 0;
                                item.DateApprovedHR = DateTime.Now;
                                  updated = true;
                            }
                            break;
                        case "ACCOUNTANT_APPROVE":
                            if (item.IsApprovedPersonalProperty == false||item.Status==0)
                            {
                                return Ok(new
                                {
                                    status = 0,
                                   tb = $"Biên bản số {id} chưa được cá nhân và nhân sự duyệt"

                                });
                            }
                            if ( item.IsApproveAccountant == false||item.IsApproveAccountant==null)
                            {
                                item.IsApproveAccountant = true;
                                item.DateApproveAccountant = DateTime.Now;
                                updated = true;
                            }
                            break;
                        case "ACCOUNTANT_CANCEL":
                            if ( item.IsApproveAccountant == true)
                            {
                                item.IsApproveAccountant = false;
                                item.DateApproveAccountant = DateTime.Now;
                                updated = true;
                            }
                            break;
                        default:
                            return BadRequest("Hành động không hợp lệ.");
                    }
                    if (updated)
                    {
                       tSAssetAllocationRepo.Update(item);
                        updatedCount++;
                    }
                    if (updatedCount == 0)
                    {
                        return Ok(new { status = 0, tb  });
                    }
                    await tSAssetAllocationRepo.UpdateAsync(item);
                }
             
                return Ok(new { status = 1, message = $"{updatedCount} bản ghi đã được cập nhật." });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("getallreportbroken")]
            public IActionResult GetAllRepoerBroken()
            {
                try
                {
                    List<TSReportBrokenAsset> reportbroken = reportrepo.GetAll();
                    return Ok(new
                    {
                        status = 1,
                        data = reportbroken
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
            [HttpGet("getstatus")]
            public IActionResult GetStatus()
            {
                try
                {
                    List<TSStatusAsset> tSStatusAssets = tSStatusAssetRepo.GetAll();
                    return Ok(new
                    {
                        status = 1,
                        data = tSStatusAssets
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
        
            [HttpGet("getassets")]
            public IActionResult GetList(string? FilterText, int PageNumber, int PageSize, DateTime? DateStart, DateTime? DateEnd, string? Status, string Department)
            {
                try
                {
                    DateTime minDate = new DateTime(2020, 12, 5);
                    DateTime maxDate = new DateTime(2025, 12, 19);
                    DateTime dateTimeS = DateStart ?? minDate;
                    DateTime dateTimeE = DateEnd ?? maxDate;
                    dateTimeS = new DateTime(dateTimeS.Year, dateTimeS.Month, dateTimeS.Day, 0, 0, 0);
                    dateTimeE = new DateTime(dateTimeE.Year, dateTimeE.Month, dateTimeE.Day, 23, 59, 59);
                    var assets = SQLHelper<dynamic>.ProcedureToList("spLoadTSAssetManagement",
                        new string[] { "@FilterText", "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@Department" },
                        new object[] { FilterText, PageNumber, PageSize, dateTimeS, dateTimeE, Status, Department });
                    List<TSAssetManagement> tSAssetManagements = tasset.GetAll();
                    return Ok(new
                    {
                        status = 1,
                        data = new
                        {
                            assets = SQLHelper<dynamic>.GetListData(assets, 0)
                        }
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new
                    {

                        status = 0,
                        message = ex.Message,
                        error = ex.ToString()
                    });
                }
            }
            [HttpGet("getallocation")]
            public IActionResult GetAllocation(string? ID)
            {
                try
                {
                    var assetsallocation = SQLHelper<dynamic>.ProcedureToList(
                "spLoadTSAllocationEvictionAsset",
                new string[] { "@ID" },        
                 new object[] { ID } );
                    return Ok(new
                    {
                        status = 1,
                        data = new
                        {
                            assetsallocation = SQLHelper<dynamic>.GetListData(assetsallocation, 0)
                        }
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new
                    {

                        status = 0,
                        message = ex.Message,
                        error = ex.ToString()
                    });
                }
            }
            [HttpGet("getassetsallocationdetail")]
            public IActionResult GetAllocationDetail(string? ID)
            {
                try
                {
                    var assetsallocationdetail = SQLHelper<dynamic>.ProcedureToList(
               "spGetTSAssetAllocationDetail",
               new string[] { "@TSAssetAllocationID" },    
               new object[] { ID }                        
           );

                    return Ok(new
                    {
                        status = 1,
                        data = new
                        {
                            assetsallocationdetail = SQLHelper<dynamic>.GetListData(assetsallocationdetail, 0)
                        }
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new
                    {

                        status = 0,
                        message = ex.Message,
                        error = ex.ToString()
                    });
                }
            }
            [HttpGet("gettype")]
            public IActionResult GetType()
            {
                try
                {
                    List<TSAsset> tSAssets = typerepo.GetAll();
                    return Ok(new
                    {
                        status = 1,
                        data = tSAssets
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
        [HttpGet("getTSAssetRecovery")]
        public async Task<ActionResult> GetTSAssetsRecovery(
      DateTime? Datestart = null, DateTime? dateEnd = null,
      int? employeereturnID = null, int? employeeRecoveryID = null,
      int? status = null, string? Filtertext = null,
      int pagesize = 100000, int pagenumber = 1)
        {
            try
            {
                var assetsrecovery = SQLHelper<dynamic>.ProcedureToList(
                    "spGetTSAssetRecovery",
                    new string[] {
                "@DateStart", "@DateEnd", "@EmployeeReturnID",
                "@EmployeeRecoveryID", "@Status", "@FilterText",
                "@PageSize", "@PageNumber"
                    },
                    new object[] {
                Datestart ?? DateTime.MinValue,
                dateEnd ?? DateTime.MaxValue,
                employeereturnID ?? 0,
                employeeRecoveryID ?? 0,
                status ?? -1,
                Filtertext ?? "",
                pagesize,
                pagenumber
                    });


                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsrecovery = SQLHelper<dynamic>.GetListData(assetsrecovery, 0)
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("getassetsrecoveryDetail")]
        public IActionResult GetAssetsRecoveryDetail(int? ID)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList(
             "spGetTSAssetRecoveryDetail",
             new string[] { "@TSAssetRecoveryID" },
             new object[] { ID }
         );
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsrecoverydetail = SQLHelper<dynamic>.GetListData(result, 0)
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {

                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("getTSAssestAllocation")]
        public async Task<IActionResult> GetTSAssetAllocation(
                             DateTime? dateStart = null,
                             DateTime? dateEnd = null,
                             int? employeeID = null,
                             string? status = null,
                             string? filterText = null,
                             int pageSize = 100000,
                             int pageNumber = 1)
        {
            try
            {
                var assetallocation = SQLHelper<dynamic>.ProcedureToList(
                    "spGetTSAssetAllocation",
                    new string[] { "@DateStart", "@DateEnd", "@EmployeeID", "@Status", "@FilterText", "@PageSize", "@PageNumber" },
                    new object[] {
                dateStart,
                dateEnd,
                employeeID,
                status,
                filterText ?? string.Empty,
                pageSize,
                pageNumber
                    });

                // Gọi hàm sinh mã
                var assetDate = dateStart ?? DateTime.Now; // Hoặc truyền ngày bạn muốn
              

                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetallocation = SQLHelper<dynamic>.GetListData(assetallocation, 0)
                       
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        
     
        [HttpPost("savedata")]
            public async Task<IActionResult> SaveAssets([FromBody] TSAssetManagement asset)
            {
                try
                {
                    if (asset.ID <= 0) await tasset.CreateAsync(asset);
                    else await tasset.UpdateAsync(asset);

                    return Ok(new
                    {
                        status = 1,
                        data = asset
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = ex.Message,
                        error = ex.ToString()
                    });
                }
            }
            [HttpPost("savedatareportbroken")]
            public async Task<IActionResult> SaveReportBroken([FromBody] ReportBrokenFullDto dto)
            {
                try
                {
                  
                    int? chucVuId = 30;
                    var assetmanagementData = tasset.GetByID(dto.AssetID) ?? new TSAssetManagement();
                    assetmanagementData.ID = dto.AssetID;
                    assetmanagementData.Note = dto.Note;
                    assetmanagementData.Status = dto.Status;
                    assetmanagementData.StatusID = dto.StatusID;
                    if (assetmanagementData.ID > 0)
                        await tasset.UpdateAsync(assetmanagementData);
                    else
                        await tasset.CreateAsync(assetmanagementData);

               
                    var reportbrokendata = new TSReportBrokenAsset

                    {
                        AssetManagementID = dto.AssetID,
                        DateReportBroken = dto.DateReportBroken,
                        Reason = dto.Reason,
                        CreatedDate = dto.DateReportBroken,
                        UpdatedDate = dto.DateReportBroken

                    };
                    await reportrepo.CreateAsync(reportbrokendata);

                    // 3. Thêm  
                    var allocationevictionasset = new TSAllocationEvictionAsset
                    {
                        AssetManagementID = dto.AssetID,
                        EmployeeID = dto.EmployeeID,
                        DepartmentID = dto.DepartmentID,
                        ChucVuID = chucVuId ?? 0,
                        DateAllocation = dto.DateReportBroken,
                        Note = dto.Reason,
                        Status = "Hỏng"
                    };
                    await tSAllocationEvictionrepo.CreateAsync(allocationevictionasset);

                    return Ok(new
                    {
                        status = 1,
                        data = new
                        {
                            assetmanagementData,
                            reportbrokendata,
                            allocationevictionasset
                        }
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new { status = 0, message = ex.Message });
                }
            }

            [HttpPost("savedatareportlost")]
            public async Task<IActionResult> SaveReportLost([FromBody] ReportLostFullDto dto)
            {
                try
                {
                    int? chucVuId = 30;
                    var assetmanagementData = tasset.GetByID(dto.AssetManagementID) ?? new TSAssetManagement();
                    assetmanagementData.ID = dto.AssetManagementID;
                    assetmanagementData.Note = dto.Note;
                    assetmanagementData.Status = dto.AssetStatus;
                    assetmanagementData.StatusID = dto.AssetStatusID;
                    assetmanagementData.UpdatedDate = dto.UpdatedDate;
                    assetmanagementData.UpdatedBy=dto.UpdatedBy;
                    if (assetmanagementData.ID > 0)
                        await tasset.UpdateAsync(assetmanagementData);
                    else
                        await tasset.CreateAsync(assetmanagementData);
                    var reportlostdata = new TSLostReportAsset
                    {
                             AssetManagementID  = dto.AssetManagementID,
                             DateLostReport =dto.DateLostReport,
                             Reason = dto.Reason,
                        CreatedDate = dto.CreatedDate,
                        CreatedBy = dto.CreatedBy,
                        UpdatedDate = dto.UpdatedDate,
                        UpdatedBy = dto.UpdatedBy

                    };
                    await tslostreport.CreateAsync(reportlostdata); 
                    var allocationevictionasset = new TSAllocationEvictionAsset
                    {
                        AssetManagementID = dto.AssetManagementID,
                        EmployeeID = dto.EmployeeID,
                        DepartmentID = dto.DepartmentID,
                        ChucVuID = chucVuId ?? 0,
                        DateAllocation = dto.DateAllocation,
                        Note = dto.Reason,
                        Status = "Hỏng",
                        CreatedDate= dto.CreatedDate,
                        CreatedBy=dto.CreatedBy,
                        UpdatedDate= dto.UpdatedDate,
                        UpdatedBy=dto.UpdatedBy
                    };
                    await tSAllocationEvictionrepo.CreateAsync(allocationevictionasset);
                    return Ok(new
                    {
                        status = 1,
                        data = new
                        {
                            assetmanagementData,
                            reportlostdata,
                            allocationevictionasset
                        }
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new { status = 0, message = ex.Message });
                }
            }   
        [HttpPost("SaveAllocation")]
        public async Task<IActionResult> SaveAllocationn([FromBody] TSAssetAllocationFullDTO dto)
        {
            try
            {
                //Thêm cấp phát
                var allocationModel = new TSAssetAllocation
                {
                    ID = dto.ID,
                    Code = dto.Code,
                    DateAllocation = dto.DateAllocation,
                    EmployeeID = dto.EmployeeID,
                    Note = dto.Note,
                    Status = 1
                };

                if (allocationModel.ID > 0)
                {
                    await tSAssetAllocationRepo.UpdateAsync(allocationModel);
                }
                else
                {
                    allocationModel.ID = await tSAssetAllocationRepo.CreateAsync(allocationModel);
                }

              //Thêm chi tiết
                foreach (var detail in dto.AssetDetails)
                {
                    var detailModel = new TSAssetAllocationDetail
                    {
                        ID = detail.ID,
                        STT = detail.STT,
                        AssetManagementID = detail.AssetManagementID,
                        TSAssetAllocationID = allocationModel.ID,
                        Quantity = detail.Quantity,
                        Note = detail.Note,
                        EmployeeID = detail.EmployeeID
                    };

                    if (detailModel.ID > 0)
                    {
                        await tSAssetAllocationDetailRepo.UpdateAsync(detailModel);
                    }
                    else
                    {
                        await tSAssetAllocationDetailRepo.CreateAsync(detailModel);
                    }

                   //Update AssetManagement tương ứng
                    var existingAsset = tasset.GetByID(detail.AssetManagementID);
              
                    if (existingAsset != null)
                    {
                        existingAsset.EmployeeID = detail.EmployeeID;
                        existingAsset.DepartmentID = detail.DepartmentID;
                        existingAsset.UpdatedDate = DateTime.Now;
                        existingAsset.UpdatedBy = detail.UpdatedBy;
                        existingAsset.StatusID = 2;
                        existingAsset.Status = "Đang sử dụng";

                      
                    }
                    await tasset.UpdateAsync(existingAsset);
                }

                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công",
                    data = allocationModel
                });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 0, message = ex.Message });
            }
        }

        [HttpGet("generaterecoverycode")]
        public async Task<IActionResult> GenerateRecoveryCode([FromQuery] DateTime? recoveryDate)
        {
            if (recoveryDate == null)
                return BadRequest("recoveryDate is required.");

            string newcode = tSAssetRecoveryRepo.GenCodeRecovery(recoveryDate);

            return Ok(new
            {
                status = 1,
                data = newcode
            });
        }

        [HttpGet("generate-allocation-code")]
        public async Task<IActionResult> GenerateAllocationCode([FromQuery] DateTime? allocationDate)
        {
            if (allocationDate == null)
                return BadRequest("allocationDate is required.");

            string newcode = tSAssetAllocationRepo.GenerateAllocationCode(allocationDate);

            return Ok(new
            {
                status = 1,
                data = newcode
            });
        }

        [HttpGet("generate-allocation-code-asset")]
        public async Task<IActionResult> GenerateAllocationCodeAsset([FromQuery] DateTime? assetdate)
        {
            if (assetdate == null)
                return BadRequest("allocationDate is required.");

            string newcode = tasset.GenerateAllocationCodeAsset(assetdate);

            return Ok(new
            {
                status = 1,
                data = newcode
            });
        }

        [HttpPost("deleteAssetAllocation")]
        public async Task<IActionResult> DeleteAssetAllocation([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest("Danh sách ID không hợp lệ.");

            foreach (var ID in ids)
            {
                var item =  tSAssetAllocationRepo.GetByID(ID);
                if (item != null)
                {
                    item.IsDeleted = true;      
                    tSAssetAllocationRepo.Update(item);
                }
                await tSAssetAllocationRepo.UpdateAsync(item);
            }
            return Ok(new { message = "Đã xóa thành công." });
        }
        [HttpPost("deleteAssetManagement")]
        public async Task<IActionResult> DeleteAssetManagement([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest("Danh sách ID không hợp lệ.");

            foreach (var ID in ids)
            {
                var item =  tasset.GetByID(ID);
                if (item != null)
                {
                    item.IsDeleted = true;
                    tasset.Update(item);
                }
                await tasset.UpdateAsync(item);
            }

           
            return Ok(new { message = "Đã xóa thành công." });
        }
        [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteAsset(int id)
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Asset ID phải lớn hơn 0."
                    });
                }
                try
                {
                    int affectedRows = await tasset.DeleteAsync(id);
                    if (affectedRows == 0)
                    {

                        return NotFound(new
                        {
                            status = 0,
                            message = $"Không tìm thấy asset với ID = {id}."
                        });
                    }
                    return NoContent();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        status = 0,
                        message = "Đã xảy ra lỗi khi xóa asset.",
                        error = ex.Message
                    });
                }
            }

        }
    }

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
      
        }
    }

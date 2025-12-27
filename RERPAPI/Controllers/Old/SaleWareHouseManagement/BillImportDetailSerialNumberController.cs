using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BillImportDetailSerialNumberController : ControllerBase
    {
        private readonly BillImportDetailSerialNumberRepo _billImportDetailSerialNumberRepo;
        private readonly BillExportDetailSerialNumberRepo _billExportDetailSerialNumberRepo;
        private readonly BillExportTechDetailSerialRepo _billExportTechDetailSerialRepo;
        private readonly BillImportTechDetailSerialRepo _billImportTechDetailSerialRepo;
        private readonly BillImportDetailSerialNumberModulaLocationRepo _billImportDetailSerialNumberModulaLocationRepo;
        private readonly BillExportDetailSerialNumberModulaLocationRepo _billExportDetailSerialNumberModulaLocationRepo;
        public BillImportDetailSerialNumberController(BillImportDetailSerialNumberRepo billImportDetailSerialNumberRepo, 
            BillExportDetailSerialNumberRepo billExportDetailSerialNumberRepo,
            BillExportTechDetailSerialRepo billExportTechDetailSerialRepo,
            BillImportTechDetailSerialRepo billImportTechDetailSerialRepo,
            BillImportDetailSerialNumberModulaLocationRepo billImportDetailSerialNumberModulaLocationRepo,
            BillExportDetailSerialNumberModulaLocationRepo billExportDetailSerialNumberModulaLocationRepo
            )
        {
            _billImportDetailSerialNumberRepo = billImportDetailSerialNumberRepo;
            _billExportDetailSerialNumberRepo = billExportDetailSerialNumberRepo;
            _billExportTechDetailSerialRepo = billExportTechDetailSerialRepo;
            _billImportTechDetailSerialRepo = billImportTechDetailSerialRepo;
            _billImportDetailSerialNumberModulaLocationRepo = billImportDetailSerialNumberModulaLocationRepo;
            _billExportDetailSerialNumberModulaLocationRepo = billExportDetailSerialNumberModulaLocationRepo;
        }

        [HttpGet("{id}")]
        public IActionResult getDataByID(int id)
        {
            try
            {
                var result = _billImportDetailSerialNumberRepo.GetByID(id);

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #region Không dùng
        //[HttpPost("save-data")]
        //[Authorize]
        //public async Task<IActionResult> saveData([FromBody] List<BillImportDetailSerialNumber> data)
        //{
        //    try
        //    {
        //        var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //        CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

        //        foreach (var item in data)
        //        {
        //            if (!string.IsNullOrWhiteSpace(item.SerialNumberRTC))
        //            {
        //                var serialRTC = _billImportDetailSerialNumberRepo.GetAll(x => x.SerialNumberRTC == item.SerialNumberRTC.Trim() &&
        //                                                                                x.BillImportDetailID == item.BillImportDetailID &&
        //                                                                                x.ID != item.ID);
        //                if (serialRTC.Count() > 0) return BadRequest(ApiResponseFactory.Fail(null, $"Số Serial Number RTC [{item.SerialNumberRTC}] đã tồn tại!", serialRTC));
        //            }

        //        }

        //        foreach (var item in data)
        //        {
        //            //if (string.IsNullOrWhiteSpace(item.SerialNumberRTC)) continue;
        //            if (item.ID > 0)
        //            {
        //                item.UpdatedBy = currentUser.LoginName;
        //                await _billImportDetailSerialNumberRepo.UpdateAsync(item);
        //            }
        //            else
        //            {
        //                item.CreatedBy = item.UpdatedBy = currentUser.LoginName;
        //                await _billImportDetailSerialNumberRepo.CreateAsync(item);
        //            }
        //        }
        //        return Ok(ApiResponseFactory.Success(data, "Lưu dữ liệu thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        #endregion



        [HttpGet("get-serialnumber")]
        [Authorize]
        public async Task<IActionResult> GetSerialNumber(int billImportDetailID)
        {
            try
            {
                var serialNumbers = _billImportDetailSerialNumberRepo.GetAll(x => x.BillImportDetailID == billImportDetailID);

                return Ok(ApiResponseFactory.Success(serialNumbers, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("location-modula")]
        [Authorize]
        public async Task<IActionResult> LoadLocationModula()
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList(
                    "spGetModulaLocation",
                    new string[]
                    {
                "@ModulaLocationID", "@Keyword"
                    },
                    new object[]
                    {
                0,""
                    }
                );

                var data = SQLHelper<dynamic>.GetListData(dt, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("data-serialnumber-tech")]
        [RequiresPermission("")]
        public IActionResult LoadData(int billId, int type, int warehouseId)
        {
            try
            {
                string procedureName = type == 1 ? "spGetBillImportTechDetailSerial" : "spGetBillExportTechDetailSerial";
                string paramName = type == 1 ? "@BillImportTechDetail" : "@BillExportTechDetailID";
                var dt = SQLHelper<dynamic>.ProcedureToList(
                    procedureName,
                    new string[]
                    {
                paramName, "@WarehouseID"
                    },
                    new object[]
                    {
                billId, warehouseId
                    }
                );

                var data = SQLHelper<dynamic>.GetListData(dt, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("data-serialnumber")]
        [RequiresPermission("")]
        public IActionResult LoadData(int billId, int type)
        {
            try
            {
                string procedureName = type == 1 ? "spGetBillImportDetailSerialNumber" : "spGetBillExportDetailSerialNumber";
                string paramName = type == 1 ? "@BillImportDetailID" : "@BillExportDetailID";
                var dt = SQLHelper<dynamic>.ProcedureToList(
                    procedureName,
                    new string[]
                    {
                paramName
                    },
                    new object[]
                    {
                billId
                    }
                );

                var data = SQLHelper<dynamic>.GetListData(dt, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("serialnumber-product")]
        [RequiresPermission("")]
        public IActionResult LoadDataProduct(int productID)
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList(
                    "spGetSerialNumberBillImportByProductID",
                    new string[]
                    {
                "@ProductID"
                    },
                    new object[]
                    {
                productID
                    }
                );

                var data = SQLHelper<dynamic>.GetListData(dt, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] BillDetailSerialNumberDTO dto)
        {
            try
            {
                if (dto.type == 1)
                {
                    if (dto.billImportDetailSerialNumbers == null || !dto.billImportDetailSerialNumbers.Any())
                    {
                        return BadRequest(new { status = 0, message = "Danh sách đơn rỗng" });
                    }

                    foreach (var item in dto.billImportDetailSerialNumbers)
                    {
                        if (item.ID <= 0)
                        {
                            await _billImportDetailSerialNumberRepo.CreateAsync(item);
                        }
                        else
                        {
                            await _billImportDetailSerialNumberRepo.UpdateAsync(item);
                        }
                    }
                }
                else
                {
                    if (dto.billExportDetailSerialNumbers == null || !dto.billExportDetailSerialNumbers.Any())
                    {
                        return BadRequest(new { status = 0, message = "Danh sách đơn rỗng" });
                    }

                    foreach (var item in dto.billExportDetailSerialNumbers)
                    {
                        if (item.ID <= 0)
                        {
                            await _billExportDetailSerialNumberRepo.CreateAsync(item);
                        }
                        else
                        {
                            await _billExportDetailSerialNumberRepo.UpdateAsync(item);
                        }
                    }
                }

                if (dto.lsDeleted != null && dto.lsDeleted.Count() > 0)
                {
                    if (dto.type == 1)
                    {
                        foreach (var id in dto.lsDeleted)
                        {
                            await _billImportDetailSerialNumberRepo.DeleteAsync(id);
                        }
                    }
                    else
                    {
                        foreach (var id in dto.lsDeleted)
                        {
                            await _billExportDetailSerialNumberRepo.DeleteAsync(id);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data-tech")]
        public async Task<IActionResult> SaveDataTech([FromBody] BillDetailSerialNumberDTO dto)
        {
            try
            {
                if (dto.type == 1)
                {
                    if (dto.billImportTechDetailSerials == null || !dto.billImportTechDetailSerials.Any())
                    {
                        return BadRequest(new { status = 0, message = "Danh sách nhập rỗng" });
                    }

                    foreach (var item in dto.billImportTechDetailSerials)
                    {
                        if (item.ID <= 0)
                        {
                            item.ID = 0;
                            await _billImportTechDetailSerialRepo.CreateAsync(item);
                        }
                        else
                        {
                            await _billImportTechDetailSerialRepo.UpdateAsync(item);
                        }

                        var locations = _billImportDetailSerialNumberModulaLocationRepo.
                            GetAll(x=> x.ModulaLocationDetailID ==  item.ModulaLocationDetailID
                            && x.BillImportTechDetailSerialID == item.ID).ToList();
                        if (locations.Count > 0) continue;

                        BillImportDetailSerialNumberModulaLocation location = new BillImportDetailSerialNumberModulaLocation();

                        location.ModulaLocationDetailID = item.ModulaLocationDetailID;
                        location.Quantity = 1;
                        location.BillImportTechDetailSerialID = item.ID;


                        await _billImportDetailSerialNumberModulaLocationRepo.CreateAsync(location);

                    }
                }
                else
                {
                    if (dto.billExportTechDetailSerials == null || !dto.billExportTechDetailSerials.Any())
                    {
                        return BadRequest(new { status = 0, message = "Danh sách xuất rỗng" });
                    }

                    foreach (var item in dto.billExportTechDetailSerials)
                    {
                        if (item.ID <= 0)
                        {
                            item.ID = 0;
                            await _billExportTechDetailSerialRepo.CreateAsync(item);
                        }
                        else
                        {
                            await _billExportTechDetailSerialRepo.UpdateAsync(item);
                        }

                        var locations = _billExportDetailSerialNumberModulaLocationRepo.
                            GetAll(x => x.ModulaLocationDetailID == item.ModulaLocationDetailID
                            && x.BillExportDetailSerialNumberID == item.ID).ToList();
                        if (locations.Count > 0) continue;

                        BillExportDetailSerialNumberModulaLocation location = new BillExportDetailSerialNumberModulaLocation();

                        location.ModulaLocationDetailID = item.ModulaLocationDetailID;
                        location.Quantity = 1;
                        location.BillExportDetailSerialNumberID = item.ID;


                        await _billExportDetailSerialNumberModulaLocationRepo.CreateAsync(location);
                    }

                    
                }

                if (dto.lsDeleted != null && dto.lsDeleted.Count() > 0)
                {
                    if (dto.type == 1)
                    {
                        foreach (var id in dto.lsDeleted)
                        {
                            await _billImportTechDetailSerialRepo.DeleteAsync(id);
                        }
                    }
                    else 
                    {
                        foreach (var id in dto.lsDeleted)
                        {
                            await _billExportTechDetailSerialRepo.DeleteAsync(id);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class ModulaLocationController : ControllerBase
    {
        private ModulaLocationRepo _locationRepo = new ModulaLocationRepo();

        //BillImportTechDetailSerialRepo _importDetailSerialNumberRepo = new BillImportTechDetailSerialRepo();
        //BillExportTechDetailSerialRepo _exportDetailSerialNumberRepo = new BillExportTechDetailSerialRepo();

        private BillImportDetailSerialNumberModulaLocationRepo _serialNumberImportModulaRepo = new BillImportDetailSerialNumberModulaLocationRepo();
        private BillExportDetailSerialNumberModulaLocationRepo _serialNumberExportModulaRepo = new BillExportDetailSerialNumberModulaLocationRepo();

        private HistoryProductRTCRepo _historyRepo = new HistoryProductRTCRepo();

        /// <summary>
        /// 1: Kho sale
        /// 2: Kho demo
        /// </summary>
        private const int WAREHOUSE_TYPE = 2;

        [HttpGet("getlocation")]
        public IActionResult GetLocation(string? keyword)
        {
            try
            {
                keyword = keyword ?? "";
                List<ModulaLocation> listLocations = _locationRepo.GetAll().Where(x => x.IsDeleted == false)
                                                                          .OrderBy(x => x.STT)
                                                                          .ToList();
                List<List<dynamic>> locationdetails = SQLHelper<object>.ProcedureToList("spGetModulaLocationDetail",
                                                                        new string[] { "@Keyword", "@WarehouseType" },
                                                                        new object[] { keyword.Trim(), WAREHOUSE_TYPE });
                var details = SQLHelper<object>.GetListData(locationdetails, 0);

                List<ModulaLocationDTO> locations = new List<ModulaLocationDTO>();
                foreach (var item in listLocations)
                {
                    ModulaLocationDTO location = new ModulaLocationDTO();
                    location.ID = item.ID;
                    location.STT = item.STT;
                    location.Code = item.Code;
                    location.Name = item.Name;
                    location.Width = item.Width;
                    location.Height = item.Height;
                    location.AxisX = item.AxisX;
                    location.AxisY = item.AxisY;
                    location.IsDeleted = item.IsDeleted;
                    location.ModulaLocationDetails = details.Where(x => x.ModulaLocationID == item.ID).ToList();

                    locations.Add(location);
                }

                return Ok(new
                {
                    status = 1,
                    data = new { locations }
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

        /// <summary>
        /// Get danh sách sản phẩm nhập - xuất
        /// </summary>
        /// <param name="billtype">1: Phiếu nhập; 2: Phiếu xuất; 3: Phiếu mượn trả</param>
        /// <param name="billcode">mã phiếu</param>
        /// <returns></returns>
        [HttpGet("getproducts")]
        public IActionResult GetProducts(int? billtype, string? billcode)
        {
            try
            {
                billtype = billtype ?? 0;
                billcode = billcode ?? "";
                List<List<dynamic>> data = SQLHelper<object>.ProcedureToList("spGetProductImportExport",
                                                                new string[] { "@BillType", "@BillCode", "@WarehouseType" },
                                                                new object[] { billtype, billcode, WAREHOUSE_TYPE });

                List<dynamic> importDetails = new List<dynamic>();
                List<dynamic> exportDetails = new List<dynamic>();
                List<dynamic> productRTCs = new List<dynamic>();

                int index = 0;

                if (billtype == 1) importDetails = SQLHelper<object>.GetListData(data, index);
                else if (billtype == 2) exportDetails = SQLHelper<object>.GetListData(data, index);
                else productRTCs = SQLHelper<object>.GetListData(data, index);

                return Ok(new
                {
                    status = 1,
                    data = new { importDetails, exportDetails, productRTCs }
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

        [HttpGet("getlocationdetail/{id}")]
        public IActionResult GetLocationByID(int id)
        {
            try
            {
                List<List<dynamic>> locations = SQLHelper<object>.ProcedureToList("spGetModulaLocationDetailByID",
                                                                new string[] { "@ID", "@WarehouseType" },
                                                                new object[] { id, WAREHOUSE_TYPE });

                var data = SQLHelper<object>.GetListData(locations, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("savedata")]
        public async Task<IActionResult> SaveDataAsync([FromBody] List<ModulaLocationDTO.SerialNumberModulaLocation> serialNumberModulaLocations)
        {
            try
            {
                APIResponse response = _locationRepo.CheckValidate(serialNumberModulaLocations);

                if (response.status == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, response.message));
                }

                for (int i = 0; i < serialNumberModulaLocations.Count; i++)
                {
                    var item = serialNumberModulaLocations[i];
                    if (item.BillType == 1) //Nếu là nhập kho
                    {
                        if (string.IsNullOrWhiteSpace(item.SerialNumber)) continue;
                        string message = await _serialNumberImportModulaRepo.SaveDataAsync(item, i);
                        if (!string.IsNullOrWhiteSpace(message))
                        {
                            return BadRequest(new
                            {
                                status = 0,
                                message
                            });
                        }
                    }
                    else if (item.BillType == 2) //Nếu là xuất kho
                    {
                        await _serialNumberExportModulaRepo.SaveDataAsync(item, i);
                    }
                    else if (item.BillType == 3) //Nếu là mượn
                    {
                        await _historyRepo.SaveDataAsync(item);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
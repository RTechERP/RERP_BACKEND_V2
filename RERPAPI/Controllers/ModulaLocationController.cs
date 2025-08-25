using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical;
using static RERPAPI.Model.DTO.ModulaLocationDTO;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulaLocationController : ControllerBase
    {
        ModulaLocationRepo locationRepo = new ModulaLocationRepo();
        ModulaLocationDetailRepo detailRepo = new ModulaLocationDetailRepo();

        BillImportDetailSerialNumberRepo importDetailSerialNumberRepo = new BillImportDetailSerialNumberRepo();
        BillExportDetailSerialNumberRepo exportDetailSerialNumberRepo = new BillExportDetailSerialNumberRepo();

        BillImportDetailSerialNumberModulaLocationRepo serialNumberImportModulaRepo = new BillImportDetailSerialNumberModulaLocationRepo();
        BillExportDetailSerialNumberModulaLocationRepo serialNumberExportModulaRepo = new BillExportDetailSerialNumberModulaLocationRepo();



        private readonly PersistentTcpClientService _tcpClient;

        string _statusModula = "11|1001|STATUS\r";
        string _callModula = "11|8328|CALL|@|1\r";
        string _returnModula = "11|1111|RETURN|1\r";
        string _lazerGoModula = "11|7777|LASER_GO|1|x|y\r";
        string _lazerOnModula = "11|3333|LASER_ON\r";
        string _lazerOffModula = "11|5555|LASER_OFF\r";
        string _displayClearModula = "11|6666|DISPLAY_CLEAR\r";
        string _displayShowModula = "11|2222|DISPLAY_SHOW|message|10|0\r";

        public ModulaLocationController(PersistentTcpClientService tcpClient)
        {
            _tcpClient = tcpClient;
        }

        [HttpGet("getlocation")]
        public IActionResult GetLocation(string? keyword)
        {
            try
            {
                keyword = keyword ?? "";
                List<ModulaLocation> listLocations = locationRepo.GetAll().Where(x => x.IsDeleted == false)
                                                                          .OrderBy(x => x.STT)
                                                                          .ToList();
                List<List<dynamic>> locationdetails = SQLHelper<object>.ProcedureToList("spGetModulaLocationDetail", new string[] { "@Keyword" }, new object[] { keyword.Trim() });
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
                return Ok(new
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
        /// <param name="billtype">1: Phiếu nhập; 2: Phiếu xuất</param>
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
                                                                new string[] { "@BillType", "@BillCode" },
                                                                new object[] { billtype, billcode });


                List<dynamic> importDetails = new List<dynamic>();
                List<dynamic> exportDetails = new List<dynamic>();

                if (billtype == 1) importDetails = SQLHelper<object>.GetListData(data, 0);
                else if (billtype == 2) exportDetails = SQLHelper<object>.GetListData(data, 0);

                return Ok(new
                {
                    status = 1,
                    data = new { importDetails, exportDetails }
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


        [HttpGet("get-location-detail")]
        public IActionResult GetLocationByID(int id)
        {
            try
            {

                List<List<dynamic>> locations = SQLHelper<object>.ProcedureToList("spGetModulaLocationDetailByID",
                                                                new string[] { "@ID" },
                                                                new object[] { id });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(locations, 0)
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
        public async Task<IActionResult> SaveData([FromBody] List<ModulaLocationDTO.SerialNumberModulaLocation> serialNumberModulaLocations)
        {
            try
            {
                for (int i = 0; i < serialNumberModulaLocations.Count; i++)
                {
                    var item = serialNumberModulaLocations[i];
                    if (string.IsNullOrWhiteSpace(item.SerialNumber)) continue;

                    if (item.BillImportDetailID > 0) //Nếu là nhập kho
                    {
                        //check trong request truyền lên
                        var serialNumberRequest = serialNumberModulaLocations.Where(x => x.SerialNumber == item.SerialNumber).ToList();
                        if (serialNumberRequest.Count() > 1)
                        {
                            return Ok(new
                            {
                                status = 0,
                                message = $"SerialNumber [{item.SerialNumber}] đã được nhập ở vị trí khác",
                            });
                        }

                        //check trong database
                        //         var serialNumbers = importDetailSerialNumberRepo.GetAll().Where(x => x.SerialNumber == item.SerialNumber).ToList();
                        //if (serialNumbers.Count() > 0)
                        //{
                        //    return Ok(new
                        //    {
                        //        status = 0,
                        //        message = $"SerialNumber [{item.SerialNumber}] đã được nhập ở vị trí khác",
                        //    });
                        //}
                    }
                }

                for (int i = 0; i < serialNumberModulaLocations.Count; i++)
                {
                    var item = serialNumberModulaLocations[i];
                    if (item.BillImportDetailID > 0) //Nếu là nhập kho
                    {
                        if (string.IsNullOrWhiteSpace(item.SerialNumber)) continue;

                        BillImportDetailSerialNumber serialNumber = importDetailSerialNumberRepo.GetAll().FirstOrDefault(x => x.SerialNumber == item.SerialNumber) ?? new BillImportDetailSerialNumber();

                        if (serialNumber.ID <= 0)
                        {
                            serialNumber.STT = i + 1;
                            serialNumber.SerialNumber = item.SerialNumber.Trim();
                            serialNumber.BillImportDetailID = item.BillImportDetailID;
                            serialNumber.SerialNumberRTC = "";
                            serialNumber.CreatedBy = serialNumber.UpdatedBy = item.CreatedBy;
                            serialNumber.CreatedDate = serialNumber.UpdatedDate = DateTime.Now;

                            importDetailSerialNumberRepo.Create(serialNumber);
                        }

                        BillImportDetailSerialNumberModulaLocation import = new BillImportDetailSerialNumberModulaLocation()
                        {
                            BillImportDetailSerialNumberID = serialNumber.ID,
                            ModulaLocationDetailID = item.ModulaLocationDetailID,
                            Quantity = item.Quantity,
                            IsDeleted = false,
                            CreatedBy = item.CreatedBy,
                            UpdatedBy = item.CreatedBy,

                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                        };

                        await serialNumberImportModulaRepo.CreateAsync(import);
                    }
                    else
                    {
                        BillExportDetailSerialNumber serialNumber = exportDetailSerialNumberRepo.GetAll().FirstOrDefault(x => x.SerialNumber == item.SerialNumber) ?? new BillExportDetailSerialNumber();
                        if (serialNumber.ID <= 0)
                        {
                            serialNumber.STT = i + 1;
                            serialNumber.SerialNumber = item.SerialNumber.Trim();
                            serialNumber.BillExportDetailID = item.BillExportDetailID;
                            serialNumber.SerialNumberRTC = "";
                            serialNumber.CreatedBy = serialNumber.UpdatedBy = item.CreatedBy;
                            serialNumber.CreatedDate = serialNumber.UpdatedDate = DateTime.Now;

                            exportDetailSerialNumberRepo.Create(serialNumber);
                        }

                        BillExportDetailSerialNumberModulaLocation export = new BillExportDetailSerialNumberModulaLocation()
                        {
                            BillExportDetailSerialNumberID = serialNumber.ID,
                            ModulaLocationDetailID = item.ModulaLocationDetailID,
                            Quantity = item.Quantity,
                            IsDeleted = false,
                            CreatedBy = item.CreatedBy,
                            UpdatedBy = item.CreatedBy,

                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                        };

                        await serialNumberExportModulaRepo.CreateAsync(export);
                    }
                }


                return Ok(new
                {
                    status = 1,
                    message = "Cập nhật thành công!",
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


        [HttpPost("savelocation")]
        public async Task<IActionResult> SaveLocation([FromBody] ModulaLocationDTO modulaLocation)
        {
            try
            {

                if (modulaLocation.ID <= 0)
                {
                    modulaLocation.CreatedDate = modulaLocation.UpdatedDate = DateTime.Now;
                    await locationRepo.CreateAsync(modulaLocation);
                }
                else
                {
                    modulaLocation.UpdatedDate = DateTime.Now;
                    await locationRepo.UpdateAsync(modulaLocation);
                }

                modulaLocation.LocationDetails.ForEach(x =>
                {
                    x.ModulaLocationID = modulaLocation.ID;
                    x.AxisX = 0;
                    x.AxisY = 1;
                });

                await detailRepo.CreateRangeAsync(modulaLocation.LocationDetails);

                return Ok(new
                {
                    status = 1,
                    message = "Cập nhật thành công!",
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



        [HttpPost("call-modula")]
        public async Task<IActionResult> CallModula([FromBody] ModulaLocationDTO.CallModula model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Code))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy Tray hoặc Vị trí!"));
                }

                await _tcpClient.SendStringAsync(_statusModula);
                string resultStatus = await _tcpClient.ReceiveStringAsync(4096);

                string call = _callModula.Replace("@", model.Code.Trim());

                await _tcpClient.SendStringAsync(call);

                string resultCall = await _tcpClient.ReceiveStringAsync(4096);

                if (string.IsNullOrEmpty(resultCall) || !resultCall.Contains('|'))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không nhận được phản hồi từ Modula!"));
                }
                if (resultCall.Split('|')[3].Trim() != "0")
                {
                    string errorMessage = (resultCall.Split('|')[3].Trim()) switch
                    {
                        "-1" => "Số khay không hợp lệ.",
                        "-2" => "Vị trí không hợp lệ.",
                        "-3" => "Vị trí đang bận.",
                        "-4" => "Khay đang bận.",
                        "-5" => "Vị trí bị vô hiệu hóa hoặc không có người dùng đăng nhập.",
                        "-6" => "Máy không ở chế độ tự động.",
                        _ => "Lỗi không xác định."
                    };
                    return BadRequest(ApiResponseFactory.Fail(null, errorMessage));
                }
                // Lazer
                string lazerGo = _lazerGoModula.Replace("x", model.AxisX.ToString()).Replace("y", model.AxisY.ToString());

                await _tcpClient.SendStringAsync(lazerGo);

                string resultLazerGo = await _tcpClient.ReceiveStringAsync(4096);

                await _tcpClient.SendStringAsync(_lazerOnModula);


                string resultLazerOn = await _tcpClient.ReceiveStringAsync(4096);


                string messageShow = _displayShowModula.Replace("message", model.Name);

                await _tcpClient.SendStringAsync(messageShow);

                string resultShow = await _tcpClient.ReceiveStringAsync(4096);

                return Ok(ApiResponseFactory.Success(null, $"Call thành công: {resultCall}| Result Lazer Go:{resultLazerGo} | Result Lazer On: {resultLazerOn} | Result show: {resultShow}"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }


        [HttpGet("return-modula")]
        public async Task<IActionResult> ReturnModula()
        {
            try
            {
                await _tcpClient.SendStringAsync(_lazerOffModula);
                string resultLazerOff = await _tcpClient.ReceiveStringAsync(4096);
                if (string.IsNullOrEmpty(resultLazerOff) || !resultLazerOff.Contains('|'))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không nhận được phản hồi từ Modula!"));
                }
                await _tcpClient.SendStringAsync(_returnModula);
                string resultCall = await _tcpClient.ReceiveStringAsync(4096);
                if (string.IsNullOrEmpty(resultCall) || !resultCall.Contains('|'))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không nhận được phản hồi từ Modula!"));
                }

                if (resultCall.Split('|')[3].Trim() != "0")
                {
                    string errorMessage = (resultCall.Split('|')[3].Trim()) switch
                    {
                        "-1" => "Vị trí trống (không có khay để trả).",
                        "-2" => "Vị trí không hợp lệ.",
                        "-3" => "Vị trí đang bận (đang xử lý thao tác khác).",
                        "-6" => "Máy không ở chế độ tự động.",
                        "-100" => "Lỗi chung (kiểm tra log WMS).",
                        _ => "Lỗi không xác định."
                    };
                    return BadRequest(ApiResponseFactory.Fail(null, errorMessage));
                }
                await _tcpClient.SendStringAsync(_displayClearModula);
                return Ok(ApiResponseFactory.Success(null, $"Return: {resultCall}"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

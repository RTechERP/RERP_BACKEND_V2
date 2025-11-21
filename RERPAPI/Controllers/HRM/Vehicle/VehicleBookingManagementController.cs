    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using RERPAPI.Attributes;
    using RERPAPI.Model.Common;
    using RERPAPI.Model.Context;
    using RERPAPI.Model.DTO;
    using RERPAPI.Model.DTO.HRM;
    using RERPAPI.Model.Entities;
    using RERPAPI.Model.Param.HRM.VehicleManagement;
    using RERPAPI.Repo.GenericEntity;
    using RERPAPI.Repo.GenericEntity.HRM.Vehicle;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    namespace RERPAPI.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class VehicleBookingManagementController : ControllerBase
        {
            VehicleBookingManagementRepo _vehicleBookingManagementRepo;
            VehicleBookingFileRepo _vehicleBookingFileRepo;


            public VehicleBookingManagementController(VehicleBookingManagementRepo vehicleBookingManagementRepo, VehicleBookingFileRepo vehicleBookingFileRepo)
            {
                _vehicleBookingManagementRepo = vehicleBookingManagementRepo;
                _vehicleBookingFileRepo = vehicleBookingFileRepo;
            }
        
                // POST: /api/vehiclebookingmanagement
                [HttpPost("get-vehicle-booking-management")]
            [RequiresPermission("N2,N34,N1,N68")]    
                public IActionResult GetVehicleBookingManagement([FromBody] VehicleBookingManagementRequestParam request)
                {
                    try
                    {

                        // 1. Chuẩn bị tham số
                        string procedureName = "spGetVehicleBookingManagement";
                        string[] paramNames = new string[] { "@StartDate", "@EndDate", "@Keyword", "Category", "@Status", "@IsCancel" };
                        object[] paramValues = new object[] { request.StartDate, request.EndDate,
                            request.Keyword.Trim(), request.Category,
                            request.Status, request.IsCancel,
                          };
                        // 2. Gọi procedure thông qua helper    
                        var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                        // 3. Xử lý kết quả
                        var result = SQLHelper<object>.GetListData(data, 0);
                        return Ok(new
                        {
                            data = result,
                            TotalPage = SQLHelper<object>.GetListData(data, 1),
                            Status = 1
                        });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message)); 
                    }
                }

            // Post: /api/vehiclebookingmanagement
            [HttpPost("save-data")]
            public async Task<IActionResult> SaveData([FromBody] VehicleBookingManagement vehicleBookingManagement)
            {
                try
                {
                    if(vehicleBookingManagement.ID>0)
                    {
                  
                        await _vehicleBookingManagementRepo.UpdateAsync(vehicleBookingManagement);
                    }
                    else
                    { 
                        await _vehicleBookingManagementRepo.CreateAsync(vehicleBookingManagement);
                    }
                    return Ok(ApiResponseFactory.Success(null, "Thêm dữ liệu thành công"));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
                }
            }

            // POST: /api/vehiclebookingmanagement
            [HttpPost("get-vehicle-schedule")]
            public IActionResult GetVehicleSchedule([FromBody] VehicleBookingManagementRequestParam getVehicleBookingManagementDTO)
            {
                try
                {

                    // 1. Chuẩn bị tham số
                    string procedureName = "spGetVehicleBookingManagementExcel";
                    string[] paramNames = new string[] { "@DateStart", "@DateEnd" };
                    object[] paramValues = new object[] { getVehicleBookingManagementDTO.StartDate, getVehicleBookingManagementDTO.EndDate };
                    // 2. Gọi procedure thông qua helper
                    var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                    // 3. Xử lý kết quả
                    var result = SQLHelper<object>.GetListData(data, 0);
                    return Ok(new
                    {
                        data = result,
                        Status = 1
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
                }
            }


            [HttpPost("get-list-image")]
            public IActionResult GetListImage([FromBody] List<VehicleBookingFileImageDTO> vehicleBookingFileImageDTO)
            {
                try
                {
                 
                    for (int i = vehicleBookingFileImageDTO.Count - 1; i >= 0; i--)
                    {
                        var item = vehicleBookingFileImageDTO[i];

                        var fileImage = _vehicleBookingFileRepo.GetAll(x => x.VehicleBookingID == item.ID).ToList();

                        if (fileImage.Any())
                        {
                 

                            var first = fileImage.First();

                            item.CreatdDate = first.CreatedDate;
                            item.FileName = first.FileName;
                            var createDate = first.CreatedDate.Value.ToString("dd.MM.yyyy");
                            string url = $"http://14.232.152.154:8083/api/datxe/DANGKYDATXENGAY{createDate}/{first.FileName}";
                            item.urlImage = url;
                            var timeRecive = item.TimeNeedPresent.Value.ToString("dd/MM/yyyyy HH:mm");
                            string title = $"Người nhận:[{item.ReceiverName}] - Thời gian nhận:[{timeRecive}]";
                            item.Title = title;


                            //public string? ReceiverPhoneNumber { get; set; }
                            //public string? PackageName { get; set; }
                            //public string? SpecificDestinationAddress { get; set; }
                            //public int ID { get; set; }
                            //public string? ReceiverName { get; set; }
                            //public DateTime? TimeNeedPresent { get; set; }
                            //public DateTime? CreatedDate { get; set; }
                            //public string? FileName { get; set; }
                            //public string? urlImage { get; set; }

                            // ✅ Với những record còn lại thì thêm mới
                            foreach (var fi in fileImage.Skip(1))
                            {
                                var createDateNew = fi.CreatedDate.Value.ToString("dd.MM.yyyy");
                                string urlMew = $"http://14.232.152.154:8083/api/datxe/DANGKYDATXENGAY{createDateNew}/{fi.FileName}";
                                vehicleBookingFileImageDTO.Add(new VehicleBookingFileImageDTO
                                {
                                    ID = item.ID,                 
                                    ReceiverName = item.ReceiverName,
                                    CreatdDate = fi.CreatedDate,
                                    FileName = fi.FileName,
                                    urlImage = urlMew,
                                    Title = title,
                                    ReceiverPhoneNumber = item.ReceiverPhoneNumber,
                                    PackageName = item.PackageName,
                                    SpecificDestinationAddress = item.SpecificDestinationAddress
                                });
                            }
                        }
                        else
                        {

                            vehicleBookingFileImageDTO.RemoveAt(i);
                        }
                    }


                    return Ok(new
                    {
                        data = vehicleBookingFileImageDTO,
                        Status = 1
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
                }
            }

            [HttpGet("get-vehicle")]
            public IActionResult GetVehicles()
            {
                try
                {
             
                    string procedureName = "spGetVehicleManagement";
                    string[] paramNames = new string[] { };
                    object[] paramValues = new object[] { };
                  
                    var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);

                    var result = SQLHelper<object>.GetListData(data, 0);


                    return Ok(new
                    {
                        data = result,
                        Status = 1
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
                }
            }
        }
    }

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.VehicleManagement;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;
using ZXing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleBookingManagementController : ControllerBase
    {
        VehicleBookingManagementRepo _vehicleBookingManagementRepo;
        VehicleBookingFileRepo _vehicleBookingFileRepo;
        List<PathStaticFile> _vehicleBookingFilePaths;
        private IConfiguration _configuration;

        private readonly EmployeeRepo _employeeRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly EmployeeApproveRepo _employeeApproveRepo;
        private readonly ProvinceRepo _provinceRepo;
        private readonly NotifyRepo _notifyRepo;
        private readonly CurrentUser _currentUser;

        public VehicleBookingManagementController(
            VehicleBookingManagementRepo vehicleBookingManagementRepo,
            VehicleBookingFileRepo vehicleBookingFileRepo,
            IConfiguration configuration,
            EmployeeRepo employeeRepo,
            ConfigSystemRepo configSystemRepo,
            ProjectRepo projectRepo,
            EmployeeApproveRepo employeeApproveRepo,
            ProvinceRepo provinceRepo,
            NotifyRepo notifyRepo,
            CurrentUser currentUser
            )
        {
            _vehicleBookingManagementRepo = vehicleBookingManagementRepo;
            _vehicleBookingFileRepo = vehicleBookingFileRepo;
            _configuration = configuration;
            _vehicleBookingFilePaths = configuration.GetSection("PathStaticFiles").Get<List<PathStaticFile>>() ?? new List<PathStaticFile>();
            _employeeRepo = employeeRepo;
            _configSystemRepo = configSystemRepo;
            _projectRepo = projectRepo;
            _employeeApproveRepo = employeeApproveRepo;
            _provinceRepo = provinceRepo;
            _notifyRepo = notifyRepo;
            _currentUser = currentUser;
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
                string[] paramNames = new string[] { "@StartDate", "@EndDate", "@Keyword", "@Category", "@EmployeeID", "@DriverEmployeeID", "@Status", "@IsCancel" };
                object[] paramValues = new object[] { request.StartDate, request.EndDate, request.Keyword.Trim(), request.Category, request.EmployeeId, request.DriverEmployeeId, request.Status, request.IsCancel,};
                // 2. Gọi procedure thông qua helper    
                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                // 3. Xử lý kết quả
                var result = SQLHelper<object>.GetListData(data, 0);
                //return Ok(new
                //{
                //    data = result,
                //    TotalPage = SQLHelper<object>.GetListData(data, 1),
                //    Status = 1
                //});
                return Ok(ApiResponseFactory.Success(new
                {
                    data = result,
                    TotalPage = SQLHelper<object>.GetListData(data, 1),
                    Status = 1
                }, ""));
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
                if(vehicleBookingManagement.DepartureDateActual==null)
                {
                    var booking = _vehicleBookingManagementRepo.GetByID(vehicleBookingManagement.ID);
                    vehicleBookingManagement.DepartureDateActual = booking.DepartureDate;
                    
                }    
                if (vehicleBookingManagement.ID > 0)
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
                //return Ok(new
                //{
                //    data = result,
                //    Status = 1
                //});
                return Ok(ApiResponseFactory.Success(new
                {
                    data = result,
                    Status = 1
                }, ""));
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
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
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
                        var templateFolder = _configuration.GetValue<string>("PathTemplate");
                        //PathStaticFile pathStatic = _vehicleBookingFilePaths.Where(x => x.PathName.ToUpper().Trim() == "Common".ToUpper()).FirstOrDefault();
                        //if (pathStatic==null) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy đường dẫn trên server!"));
                        //string pathFull = pathStatic.PathFull;
                        //string url = pathFull+$"/11. HCNS/DatXe/DANGKYDATXENGAY{createDate}/{first.FileName}";
                        //string url = $"api/share/Common/HCNS/datxe/DANGKYDATXENGAY{createDate}/{first.FileName}";

                        // relative path bên trong \\192.168.1.190\Common\...
                        var relativePath = $"11. HCNS/DatXe/DANGKYDATXENGAY{createDate}/{first.FileName}";

                        // Encode để xử lý khoảng trắng, dấu, ký tự đặc biệt (&, %, ...)
                        var encodedPath = Uri.EscapeUriString(relativePath);
                        string url = $"{baseUrl}/api/api/share/common/{encodedPath}";
                        //item.urlImage = url;
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
                            string urlMew = $"http://192.168.1.190/api/datxe/DANGKYDATXENGAY{createDateNew}/{fi.FileName}";
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


                //return Ok(new
                //{
                //    data = vehicleBookingFileImageDTO,
                //    Status = 1
                //});
                return Ok(ApiResponseFactory.Success(new
                {
                    data = vehicleBookingFileImageDTO,
                    Status = 1
                }, ""));
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


                //return Ok(new
                //{
                //    data = result,
                //    Status = 1
                //});
                return Ok(ApiResponseFactory.Success(new
                {
                    data = result,
                    Status = 1
                }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employees")]
        public IActionResult LoadEmployees()
        {
            try
            {
                var list = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                 new string[] { "@Keyword", "@Status" },
                                 new object[] { "", 0 });
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employee-by-id")]
        public IActionResult LoadEmployeeByID(int employeeId)
        {
            try
            {
                var list = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                 new string[] { "@Keyword", "@Status" },
                                 new object[] { "", -1 });
                var employee = list.FirstOrDefault(e => e.ID == employeeId);
                return Ok(ApiResponseFactory.Success(employee, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-projects")]
        public IActionResult LoadProjects()
        {
            try
            {
                var projects = _projectRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(projects, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-files")]
        public IActionResult LoadFiles(int vehicleBookingId)
        {
            try
            {
                var file = _vehicleBookingFileRepo.GetAll(x => x.VehicleBookingID == vehicleBookingId);
                return Ok(ApiResponseFactory.Success(file, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("vehicle-booking-cancel")]
        public IActionResult VehicleBookingCancel([FromBody] int vehicleBookingId)
        {
            try
            {
                VehicleBookingManagement vehicleBooking = _vehicleBookingManagementRepo.GetByID(vehicleBookingId);
                if (vehicleBooking == null) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu cần hủy"));
                vehicleBooking.Status = 3;
                vehicleBooking.IsCancel = true;
                _vehicleBookingManagementRepo.Update(vehicleBooking);
                return Ok(ApiResponseFactory.Success(null, "Hủy thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-province-arrives")]
        public IActionResult LoadProvinceArrives(int employeeId)
        {
            try
            {
                var provinces = _provinceRepo.GetAll();
                return Ok(ApiResponseFactory.Success(provinces, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-approved-list")]
        public IActionResult GetApprovedList()
        {
            var approved = _employeeApproveRepo.GetAll(x => x.Type == 1);
            return Ok(ApiResponseFactory.Success(approved, ""));
        }

        [HttpGet("get-province-departure")]
        public IActionResult LoadProvinceDeparture(int employeeId)
        {
            try
            {
                List<int> provinceDepartureIDs = new List<int>() { 1, 2, 3, 4 };

                var provinceDepartures = _provinceRepo.GetAll().Select(x => new
                {
                    x.ID,
                    ProvinceName = provinceDepartureIDs.Contains(x.ID) ? $"VP {x.ProvinceName}" : x.ProvinceName,
                }).ToList();
                return Ok(ApiResponseFactory.Success(provinceDepartures, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] VehicleBookingManagement vehicleBooking)
        {
            try
            {
                DateTime dateRegister = DateTime.Now;

                // Kiểm tra phát sinh
                bool isProblem = _vehicleBookingManagementRepo.IsProblem(vehicleBooking);

                var cateText = vehicleBooking.Category == 1 ? "đến" : "giao đến";
                //var cateText = (vehicleBooking.Category == 1 || vehicleBooking.Category == 4 || vehicleBooking.Category == 5)
                //                ? "đến"
                //                : (vehicleBooking.Category == 6 || vehicleBooking.Category == 7)
                //                    ? "đến lấy"
                //                    : "giao đến";


                //  Validate thời gian cần đến 
                if (!vehicleBooking.TimeNeedPresent.HasValue)
                    return BadRequest(ApiResponseFactory.Fail(null, $"Vui lòng nhập Thời gian cần {cateText}!"));

                DateTime timeNeed = vehicleBooking.TimeNeedPresent.Value;
                if (timeNeed <= dateRegister)
                    return BadRequest(ApiResponseFactory.Fail(null, "Thời gian cần đến phải lớn hơn thời gian hiện tại!"));

                if (vehicleBooking.TimeReturn.HasValue)
                {
                    if (vehicleBooking.TimeReturn.Value <= timeNeed)
                        return BadRequest(ApiResponseFactory.Fail(null, "Thời gian cần về phải lớn hơn thời gian cần đến!"));
                }

                // Validate thời gian xuất phát
                if (vehicleBooking.Category != 2 && vehicleBooking.Category != 6 && vehicleBooking.Category != 7 && vehicleBooking.Category != 8)
                {
                    if (!vehicleBooking.DepartureDate.HasValue)
                        return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Thời gian xuất phát!"));

                    if (vehicleBooking.DepartureDate.Value <= dateRegister)
                        return BadRequest(ApiResponseFactory.Fail(null, "Thời gian xuất phát phải lớn hơn thời gian hiện tại!"));

                    if (vehicleBooking.DepartureDate.Value >= timeNeed)
                        return BadRequest(ApiResponseFactory.Fail(null, "Thời gian xuất phát phải nhỏ hơn thời gian cần đến!"));
                }

                // Validate địa điểm 
                if (string.IsNullOrWhiteSpace(vehicleBooking.CompanyNameArrives))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Vui lòng nhập Công ty {cateText}!"));

                if (string.IsNullOrWhiteSpace(vehicleBooking.Province))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Vui lòng nhập Tỉnh {cateText}!"));

                if (string.IsNullOrWhiteSpace(vehicleBooking.SpecificDestinationAddress))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Vui lòng nhập Địa chỉ cụ thể {cateText}!"));

                //  Validate phát sinh 
                if (isProblem && vehicleBooking.Category != 5)
                {
                    if (vehicleBooking.ApprovedTBP == 0)
                        return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn Người duyệt!"));

                    if (string.IsNullOrWhiteSpace(vehicleBooking.ProblemArises))
                        return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Vấn đề phát sinh!"));
                }

                // Validate người đi / người nhận 
                if (vehicleBooking.Category == 1 || vehicleBooking.Category == 4 || vehicleBooking.Category == 5)
                {
                    if (string.IsNullOrWhiteSpace(vehicleBooking.PassengerName))
                        return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập tên người đi!"));

                    if (string.IsNullOrWhiteSpace(vehicleBooking.PassengerPhoneNumber))
                        return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập SDT người đi!"));
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(vehicleBooking.ReceiverName))
                        return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập tên người nhận hàng!"));

                    if (string.IsNullOrWhiteSpace(vehicleBooking.PackageName))
                        return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập tên kiện hàng!"));

                    if (vehicleBooking.PackageQuantity <= 0)
                        return BadRequest(ApiResponseFactory.Fail(null, "Số lượng kiện hàng phải lớn hơn 0!"));
                }

                if (vehicleBooking.ProjectID == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn Dự án!"));


                // Gán dữ liệu
                vehicleBooking.Status = vehicleBooking.Category == 4 ? 4 : 1;
                vehicleBooking.IsCancel = false;
                vehicleBooking.IsSend = false;
                vehicleBooking.IsNotifiled = false;

                //  Lưu
                if (vehicleBooking.ID > 0)
                {
                    _vehicleBookingManagementRepo.Update(vehicleBooking);
                }
                else
                {
                    _vehicleBookingManagementRepo.Create(vehicleBooking);

                    //Auto tạo đăng ký về
                    if (vehicleBooking.Category == 1 && vehicleBooking.TimeReturn.HasValue)
                    {
                        var exist = _vehicleBookingManagementRepo
                            .GetAll(x => x.ParentID == vehicleBooking.ID)
                            .Any();

                        if (!exist)
                        {
                            var returnBooking = new VehicleBookingManagement
                            {
                                // COPY NGUYÊN TỪ PHIẾU ĐI
                                VehicleType = vehicleBooking.VehicleType,
                                VehicleManagementID = vehicleBooking.VehicleManagementID,
                                EmployeeID = vehicleBooking.EmployeeID,
                                BookerVehicles = vehicleBooking.BookerVehicles,
                                PhoneNumber = vehicleBooking.PhoneNumber,

                                PassengerEmployeeID = vehicleBooking.PassengerEmployeeID,
                                PassengerCode = vehicleBooking.PassengerCode,
                                PassengerName = vehicleBooking.PassengerName,
                                PassengerDepartment = vehicleBooking.PassengerDepartment,
                                PassengerPhoneNumber = vehicleBooking.PassengerPhoneNumber,

                                Note = vehicleBooking.Note,
                                ProjectID = vehicleBooking.ProjectID,

                                //FIELD KHÁC CHO PHIẾU VỀ 
                                ParentID = vehicleBooking.ID,
                                Category = 5,
                                Status = 1,
                                ApprovedTBP = vehicleBooking.ApprovedTBP,
                                IsApprovedTBP = vehicleBooking.IsApprovedTBP,
                                ProblemArises = vehicleBooking.ProblemArises,
                                DeliverName = vehicleBooking.DeliverName,
                                DeliverPhoneNumber = vehicleBooking.DeliverPhoneNumber,
                                ReceiverName = vehicleBooking.ReceiverName,
                                ReceiverPhoneNumber = vehicleBooking.ReceiverPhoneNumber,

                                PackageWeight = vehicleBooking.PackageWeight,
                                PackageSize = vehicleBooking.PackageSize,
                                PackageQuantity = vehicleBooking.PackageQuantity,
                                PackageName = vehicleBooking.PackageName,
                                
                                

                                CompanyNameArrives = vehicleBooking.DepartureAddress,
                                DepartureAddress = vehicleBooking.CompanyNameArrives,
                                DepartureDate = vehicleBooking.TimeReturn,
                                TimeNeedPresent = vehicleBooking.TimeNeedPresent,

                                Province = vehicleBooking.Province,
                                SpecificDestinationAddress = vehicleBooking.SpecificDestinationAddress,

                                //  FLAG 
                                IsCancel = false,
                                IsSend = false,
                                IsNotifiled = false,
                                IsProblemArises = vehicleBooking.IsProblemArises,

                            };

                            _vehicleBookingManagementRepo.Create(returnBooking);
                        }
                    }

                    if (vehicleBooking.IsProblemArises == true)
                    {
                        string categoryText = vehicleBooking.Category switch
                        {
                            1 => "Đăng ký người đi ",
                            2 => "Đăng ký giao hàng thương mại",
                            3 => "Xếp xe về",
                            4 => "Chủ động phương tiện",
                            5 => "Đăng ký người về",
                            6 => "Đăng ký lấy hàng thương mại",
                            7 => "Đăng ký lấy hàng Demo/triển Lãm",
                            8 => "Đăng ký giao hàng Demo/triển lãm",
                            _ => ""
                        };

                        string content =
                            $"Mục đích: {categoryText}\n" +
                            $"Lý do: {vehicleBooking.ProblemArises}\n" +
                            $"Thời gian xuất phát: {vehicleBooking.DepartureDate:dd/MM/yyyy HH:mm}";

                        _notifyRepo.AddNotify("ĐĂNG KÝ XE", content, vehicleBooking.ApprovedTBP ?? 0);
                    }
                }

                return Ok(ApiResponseFactory.Success(vehicleBooking, "Tạo đăng ký xe thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpPost("upload-file")]
        //public async Task<IActionResult> UploadFile(
        //    int vehicleBookingId,
        //    [FromForm] List<IFormFile> files)
        //{
        //    try
        //    {
        //        var booking = _vehicleBookingManagementRepo.GetByID(vehicleBookingId);
        //        if (booking == null)
        //            return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại đơn đăng ký xe"));

        //        string pathServer = @"\\192.168.1.190\Common\11. HCNS\DatXe";
        //        string folder = $"DANGKYDATXENGAY{booking.CreatedDate:dd.MM.yyyy}";
        //        string pathUpload = Path.Combine(pathServer, folder);

        //        if (!Directory.Exists(pathUpload))
        //            Directory.CreateDirectory(pathUpload);

        //        foreach (var file in files)
        //        {
        //            var filePath = Path.Combine(pathUpload, file.FileName);
        //            using var stream = new FileStream(filePath, FileMode.Create);
        //            await file.CopyToAsync(stream);

        //            await _vehicleBookingFileRepo.CreateAsync(new VehicleBookingFile
        //            {
        //                VehicleBookingID = booking.ID,
        //                FileName = file.FileName,
        //                ServerPath = pathUpload,
        //            });
        //        }

        //        return Ok(ApiResponseFactory.Success(null, "Upload file thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        #region Upload file Vehicle Booking
        [HttpPost("upload-file")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadVehicleBookingFile(int vehicleBookingId)
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var key = form["key"].ToString();
                var files = form.Files;

                if (string.IsNullOrWhiteSpace(key))
                    return BadRequest(ApiResponseFactory.Fail(null, "Key không được để trống"));

                if (files == null || files.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách file không được để trống"));

                var booking = _vehicleBookingManagementRepo.GetByID(vehicleBookingId);
                if (booking == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại đơn đăng ký xe"));

                // Lấy đường dẫn upload từ cấu hình
                var uploadPath = _configSystemRepo.GetUploadPathByKey(key);
                if (string.IsNullOrWhiteSpace(uploadPath))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));

                // Xử lý subPath
                var subPathRaw = form["subPath"].ToString()?.Trim() ?? "";
                string targetFolder = uploadPath;

                if (!string.IsNullOrWhiteSpace(subPathRaw))
                {
                    var separator = Path.DirectorySeparatorChar;
                    var segments = subPathRaw
                        .Replace('/', separator)
                        .Replace('\\', separator)
                        .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                        .Select(seg =>
                        {
                            var invalidChars = Path.GetInvalidFileNameChars();
                            var cleaned = new string(seg.Where(c => !invalidChars.Contains(c)).ToArray());
                            cleaned = cleaned.Replace("..", "").Trim();
                            return cleaned;
                        })
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();

                    if (segments.Length > 0)
                        targetFolder = Path.Combine(uploadPath, Path.Combine(segments));
                }
                else
                {
                    targetFolder = Path.Combine(
                        uploadPath,
                        $"DANGKYDATXENGAY{booking.CreatedDate:dd.MM.yyyy}"
                    );
                }

                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                var processedFiles = new List<VehicleBookingFile>();

                foreach (var file in files)
                {
                    if (file.Length <= 0) continue;

                    var fileExtension = Path.GetExtension(file.FileName);
                    var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var uniqueFileName = $"{originalFileName}{fileExtension}";
                    var fullPath = Path.Combine(targetFolder, uniqueFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var bookingFile = new VehicleBookingFile
                    {
                        VehicleBookingID = booking.ID,
                        FileName = uniqueFileName,
                        ServerPath = targetFolder,
                    };

                    await _vehicleBookingFileRepo.CreateAsync(bookingFile);
                    processedFiles.Add(bookingFile);
                }

                return Ok(ApiResponseFactory.Success(
                    processedFiles,
                    $"{processedFiles.Count} tệp đã được tải lên thành công"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi upload file: {ex.Message}"));
            }
        }
        #endregion


        [HttpPost("remove-file")]
        public async Task<IActionResult> RemoveFile([FromBody] List<int> fileIds)
        {
            try
            {
                foreach (var id in fileIds)
                {
                    var file = _vehicleBookingFileRepo.GetByID(id);
                    if (file == null) continue;

                    string fullPath = Path.Combine(file.ServerPath, file.FileName);
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);

                    await _vehicleBookingFileRepo.DeleteAsync(id);
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa file thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("send-email")]
        public IActionResult SendEmail([FromBody] VehicleBookingManagementDTO booking)
        {
            try
            {
                int[] categories = { 1, 4, 5 };
                var employee = _employeeRepo.GetByID(booking.ApprovedTBP ?? 0) ?? new Employee();

                // gửi TBP
                if (employee.ID > 0)
                {
                    _vehicleBookingManagementRepo.SendEmail(
                        booking, booking.ApprovedTBP ?? 0, "ĐĂNG KÝ XE");
                }

                booking.ApprovedTBP = 0;
                // gửi người liên quan
                if (categories.Contains(booking.Category ?? 0))
                {
                    var passengers = booking.EmployeeAttaches
                        .Select(x => x.PassengerEmployeeID)
                        .Where(x => x.HasValue)
                        .Select(x => x.Value)
                        .Distinct();

                    foreach (var id in passengers)
                    {
                        if(_currentUser.EmployeeID == id)
                            continue;
                        _vehicleBookingManagementRepo.SendEmail(booking, id, "ĐĂNG KÝ XE");
                    }
                }
                else
                {
                    var receivers = booking.EmployeeAttaches
                        .Select(x => x.ReceiverEmployeeID)
                        .Where(x => x.HasValue)
                        .Select(x => x.Value)
                        .Distinct();

                    foreach (var id in receivers)
                        _vehicleBookingManagementRepo.SendEmail(booking, id, "ĐĂNG KÝ XE");
                }

                return Ok(ApiResponseFactory.Success(null, "Gửi email thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}

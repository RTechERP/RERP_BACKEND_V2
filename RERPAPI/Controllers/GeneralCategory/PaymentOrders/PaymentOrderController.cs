using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders;
using System.Numerics;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.GeneralCategory.PaymentOrders
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentOrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private CurrentUser _currentUser;

        private readonly ConfigSystemRepo _configSystemRepo;
        private readonly PaymentOrderRepo _paymentRepo;
        private readonly PaymentOrderDetailRepo _detailRepo;
        private readonly PaymentOrderLogRepo _logRepo;
        private readonly PaymentOrderFileRepo _fileRepo;
        private readonly PaymentOrderFileBankSlipRepo _fileBankSlipRepo;

        private readonly PaymentOrderTypeRepo _orderTypeRepo;
        private readonly EmployeeApprovedRepo _approvedRepo;
        private readonly SupplierSaleRepo _supplierSaleRepo;
        private readonly PONCCRepo _poNccRepo;
        private readonly RegisterContractRepo _registerContractRepo;
        private readonly ProjectRepo _projectRepo;


        public PaymentOrderController(IConfiguration configuration, CurrentUser currentUser,
            PaymentOrderRepo paymentRepo,
            PaymentOrderDetailRepo detailRepo,
            PaymentOrderLogRepo logRepo,
            ConfigSystemRepo configSystemRepo,
            PaymentOrderFileRepo fileRepo,
            PaymentOrderFileBankSlipRepo fileBankSlipRepo,

            PaymentOrderTypeRepo orderTypeRepo,
            EmployeeApprovedRepo approvedRepo,
            SupplierSaleRepo supplierSaleRepo,
            PONCCRepo poNcc,
            RegisterContractRepo registerContractRepo,
            ProjectRepo projectRepo
            )
        {
            _configuration = configuration;
            _currentUser = currentUser;
            _paymentRepo = paymentRepo;
            _detailRepo = detailRepo;
            _logRepo = logRepo;
            _configSystemRepo = configSystemRepo;
            _fileRepo = fileRepo;
            _fileBankSlipRepo = fileBankSlipRepo;

            _orderTypeRepo = orderTypeRepo;
            _approvedRepo = approvedRepo;
            _supplierSaleRepo = supplierSaleRepo;
            _poNccRepo = poNcc;
            _registerContractRepo = registerContractRepo;
            _projectRepo = projectRepo;
        }


        [HttpPost("")]
        public IActionResult GetAll([FromBody] PaymentOrderParam p)
        {
            try
            {
                p.DateStart = p.DateStart.Value.Date;
                p.DateEnd = p.DateEnd.Value.Date.AddDays(+1).AddSeconds(-1);

                var data = SQLHelper<object>.ProcedureToList("spGetPaymentOrder",
                            new string[] { "@PageNumber", "@PageSize", "@TypeOrder", "@PaymentOrderTypeID", "@DateStart", "@DateEnd", "@DepartmentID", "@EmployeeID", "@Keyword", "@IsIgnoreHR", "@IsApproved", "@IsSpecialOrder", "@ApprovedTBPID", "@Step", "@IsShowTable", "@Statuslog", "@IsDelete" },
                            new object[] { p.PageNumber, p.PageSize, p.TypeOrder, p.PaymentOrderTypeID, p.DateStart, p.DateEnd, p.DepartmentID, p.EmployeeID, p.Keyword, p.IsIgnoreHR, p.IsApproved, p.IsSpecialOrder, p.ApprovedTBPID, p.Step, p.IsShowTable, p.Statuslog, p.IsDelete });


                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0)));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetByD(int id)
        {
            try
            {
                //Get chi tiết đề nghị
                var dataDetail = SQLHelper<object>.ProcedureToList("spGetPaymentOrderByID", new string[] { "@ID" }, new object[] { id });
                var details = SQLHelper<object>.GetListData(dataDetail, 1);

                //Get file đính kèm
                var files = SQLHelper<object>.GetListData(dataDetail, 3);

                //Get file bank slip
                var fileBankSlips = _fileBankSlipRepo.GetAll(x => x.PaymentOrderID == id && x.IsDeleted != true);

                return Ok(ApiResponseFactory.Success(new
                {
                    details,
                    files,
                    fileBankSlips
                }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] PaymentOrderDTO payment)
        {
            try
            {
                _currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey") ?? "");

                //var validate = _paymentRepo.Validate(payment);
                //if (validate.status == 0)
                //{
                //    return BadRequest(validate);
                //}

                payment.EmployeeID = _currentUser.EmployeeID;
                payment.IsUrgent = payment.DatePayment.HasValue;
                if (payment.ID <= 0)
                {
                    payment.Code = _paymentRepo.GetCode(payment);
                    await _paymentRepo.CreateAsync(payment);
                }
                else await _paymentRepo.UpdateAsync(payment);

                //Update chi tiết thanh toán
                await _detailRepo.Create(payment);

                //Update quy trình duyệt
                await _logRepo.Create(payment);

                return Ok(ApiResponseFactory.Success(payment, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile([FromForm] PaymentOrderDTO payment)
        {
            try
            {
                _currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey") ?? "");

                var paymentOrderFiles = JsonConvert.DeserializeObject<List<PaymentOrderFile>>(payment.PaymentOrderFiles);
                foreach (var file in paymentOrderFiles)
                {
                    if (file.ID > 0) await _fileRepo.UpdateAsync(file);
                }


                //Sửa lý đường dẫn lưu file

                //var form = await Request.ReadFormAsync();
                //var key = form["key"].ToString();
                var files = Request.Form.Files;
                //var files = form.Files;

                string key = "PathPaymentOrder";

                // Kiểm tra input
                //if (string.IsNullOrWhiteSpace(key))
                //{
                //    return BadRequest(ApiResponseFactory.Fail(null, "Key không được để trống!"));
                //}

                //if (files == null || files.Count == 0)
                //{
                //    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách file không được để trống!"));
                //}

                // Lấy đường dẫn từ ConfigSystem
                var uploadPath = _configSystemRepo.GetUploadPathByKey(key);
                if (string.IsNullOrWhiteSpace(uploadPath))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));
                }

                // Đọc subPath từ form (nếu có) và ghép vào uploadPath
                //var subPathRaw = form["subPath"].ToString()?.Trim() ?? "";
                string targetFolder = uploadPath;
                //if (!string.IsNullOrWhiteSpace(subPathRaw))
                //{
                //    // Chuẩn hóa dấu phân cách và loại bỏ ký tự không hợp lệ trong từng segment
                //    var separator = Path.DirectorySeparatorChar;
                //    var segments = subPathRaw
                //        .Replace('/', separator)
                //        .Replace('\\', separator)
                //        .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                //        .Select(seg =>
                //        {
                //            var invalidChars = Path.GetInvalidFileNameChars();
                //            var cleaned = new string(seg.Where(c => !invalidChars.Contains(c)).ToArray());
                //            // Ngăn chặn đường dẫn leo lên thư mục cha
                //            cleaned = cleaned.Replace("..", "").Trim();
                //            return cleaned;
                //        })
                //        .Where(s => !string.IsNullOrWhiteSpace(s))
                //        .ToArray();

                //    if (segments.Length > 0)
                //    {
                //        targetFolder = Path.Combine(uploadPath, Path.Combine(segments));
                //    }
                //}

                foreach (var file in files)
                {
                    var result = await FileHelper.UploadFile(file, targetFolder);

                    if (result.status == 1)
                    {
                        var orderFile = new PaymentOrderFile();
                        orderFile.PaymentOrderID = (paymentOrderFiles.FirstOrDefault() ?? new PaymentOrderFile()).PaymentOrderID;
                        orderFile.FileName = TextUtils.ToString(result.data);
                        orderFile.OriginPath = "";
                        orderFile.ServerPath = targetFolder;

                        await _fileRepo.CreateAsync(orderFile);
                    }
                }

                return Ok(ApiResponseFactory.Success(payment.PaymentOrderFiles, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("get-data-combo")]
        public IActionResult GetDataCombo()
        {
            try
            {
                _currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey") ?? "");

                var paymentOrderTypes = _orderTypeRepo.GetAll(x => x.IsDelete != true && x.IsSpecialOrder != true);
                var approvedTBPs = _approvedRepo.GetAll(x => x.Type == 3 && x.IsDeleted != true);

                DateTime updateDateSupplier = new DateTime(2024, 04, 04);
                var supplierSales = _supplierSaleRepo.GetAll(x => x.UpdatedDate.Value.Date >= updateDateSupplier && x.IsDeleted != true)
                                                    .Select(x => new
                                                    {
                                                        ID = x.ID,
                                                        NameNCC = string.IsNullOrEmpty(x.MaSoThue ?? "".Trim()) ? x.NameNCC : $"{x.MaSoThue} - {x.NameNCC}",
                                                    }).OrderByDescending(x => x.ID).ToList();
                var poNCCs = _poNccRepo.GetAll(x => x.IsDeleted != true);
                var registerContracts = _registerContractRepo.GetAll(x => x.EmployeeID == _currentUser.EmployeeID && x.IsDeleted != true);
                var projects = _projectRepo.GetAll(x => x.IsDeleted != true);

                var data = new
                {
                    paymentOrderTypes,
                    approvedTBPs,
                    supplierSales,
                    poNCCs,
                    registerContracts,
                    projects
                };

                return Ok(ApiResponseFactory.Success(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("appoved-tbp")]
        public async Task<IActionResult> ApprovedTBP([FromBody] List<PaymentOrderDTO> payment)
        {
            try
            {
                var reponse = await _logRepo.Appoved(payment);
                if (reponse == 1)
                {
                    return Ok(ApiResponseFactory.Success(null, "Cập nhật thành công!"));
                }
                else
                {
                    return Ok(ApiResponseFactory.Fail(null, "Cập nhật thất bại!"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("appoved-hr")]
        public IActionResult ApprovedHR([FromBody] List<PaymentOrderDTO> payment)
        {
            try
            {
                return Ok(ApiResponseFactory.Success(_logRepo.Appoved(payment), "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("appoved-kttt")]
        public IActionResult ApprovedKTTT([FromBody] List<PaymentOrderDTO> payment)
        {
            try
            {
                return Ok(ApiResponseFactory.Success(_logRepo.Appoved(payment), "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("appoved-ktt")]
        public IActionResult ApprovedKTT([FromBody] List<PaymentOrderDTO> payment)
        {
            try
            {
                return Ok(ApiResponseFactory.Success(_logRepo.Appoved(payment), "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("appoved-bgd")]
        public IActionResult ApprovedBGD([FromBody] List<PaymentOrderDTO> payment)
        {
            try
            {
                return Ok(ApiResponseFactory.Success(_logRepo.Appoved(payment), "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

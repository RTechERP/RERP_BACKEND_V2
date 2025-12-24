using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders;

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
        private readonly PaymentOrderPORepo _paymentOrderPORepo;

        private readonly PaymentOrderTypeRepo _orderTypeRepo;
        private readonly EmployeeApprovedRepo _approvedRepo;
        private readonly SupplierSaleRepo _supplierSaleRepo;
        private readonly PONCCRepo _poNccRepo;
        private readonly RegisterContractRepo _registerContractRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly PONCCDetailRepo _poNccDetailRepo;
        private readonly ProductSaleRepo _productSaleRepo;
        private readonly ProductRTCRepo _productRTCRepo;
        private readonly UnitCountKTRepo _unitCountKTRepo;
        private readonly CurrencyRepo _currencyRepo;

        private readonly CustomerRepo _customerRepo;
        private readonly POKHRepo _poKHRepo;
        private readonly POKHDetailRepo _pOKHDetailRepo;
        private readonly EmployeeTeamSaleRepo _employeeTeamSaleRepo;

        public PaymentOrderController(IConfiguration configuration, CurrentUser currentUser,
            PaymentOrderRepo paymentRepo,
            PaymentOrderDetailRepo detailRepo,
            PaymentOrderLogRepo logRepo,
            ConfigSystemRepo configSystemRepo,
            PaymentOrderFileRepo fileRepo,
            PaymentOrderFileBankSlipRepo fileBankSlipRepo,
            PaymentOrderPORepo paymentOrderPORepo,

            PaymentOrderTypeRepo orderTypeRepo,
            EmployeeApprovedRepo approvedRepo,
            SupplierSaleRepo supplierSaleRepo,
            PONCCRepo poNcc,
            RegisterContractRepo registerContractRepo,
            ProjectRepo projectRepo,
            PONCCDetailRepo poNccDetailRepo,
            ProductSaleRepo productSaleRepo,
            ProductRTCRepo productRTCRepo,
            UnitCountKTRepo unitCountKTRepo,
            CurrencyRepo currencyRepo,

            CustomerRepo customerRepo,
            POKHRepo poKHRepo,
            POKHDetailRepo pOKHDetailRepo,
            EmployeeTeamSaleRepo employeeTeamSaleRepo
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
            _paymentOrderPORepo = paymentOrderPORepo;

            _orderTypeRepo = orderTypeRepo;
            _approvedRepo = approvedRepo;
            _supplierSaleRepo = supplierSaleRepo;
            _poNccRepo = poNcc;
            _registerContractRepo = registerContractRepo;
            _projectRepo = projectRepo;
            _poNccDetailRepo = poNccDetailRepo;
            _productSaleRepo = productSaleRepo;
            _productRTCRepo = productRTCRepo;
            _unitCountKTRepo = unitCountKTRepo;
            _currencyRepo = currencyRepo;

            _customerRepo = customerRepo;
            _poKHRepo = poKHRepo;
            _pOKHDetailRepo = pOKHDetailRepo;
            _employeeTeamSaleRepo = employeeTeamSaleRepo;
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

                var paymentOrder = SQLHelper<object>.GetListData(dataDetail, 0);
                var details = SQLHelper<object>.GetListData(dataDetail, 1);
                var signs = SQLHelper<object>.GetListData(dataDetail, 2);

                //Get file đính kèm
                var files = SQLHelper<object>.GetListData(dataDetail, 3);

                //Get file bank slip
                var fileBankSlips = _fileBankSlipRepo.GetAll(x => x.PaymentOrderID == id && x.IsDeleted != true);

                return Ok(ApiResponseFactory.Success(new
                {
                    paymentOrder,
                    details,
                    signs,
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
                //_currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey") ?? "");

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                _currentUser = ObjectMapper.GetCurrentUser(claims);

                var validate = _paymentRepo.Validate(payment);
                if (validate.status == 0)
                {
                    return BadRequest(validate);
                }

                payment.EmployeeID = _currentUser.EmployeeID;
                payment.IsUrgent = payment.DeadlinePayment.HasValue;
                if (payment.IsSpecialOrder == true) payment.TypeOrder = 0;
                if (payment.ID <= 0)
                {
                    payment.Code = _paymentRepo.GetCode(payment);
                    await _paymentRepo.CreateAsync(payment);
                }
                else await _paymentRepo.UpdateAsync(payment);

                //Update link pokh
                await _paymentOrderPORepo.Create(payment);

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
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                //_currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey") ?? "");

                var form = await Request.ReadFormAsync();

                var paymentOrderFiles = JsonConvert.DeserializeObject<List<PaymentOrderFile>>(form["PaymentOrderFile"]);
                foreach (var file in paymentOrderFiles)
                {
                    if (file.ID > 0) await _fileRepo.UpdateAsync(file);
                }


                var paymentOrderID = TextUtils.ToInt32(form["PaymentOrderID"]);
                var files = Request.Form.Files;

                // Lấy đường dẫn từ ConfigSystem
                var pathServer = _configSystemRepo.GetUploadPathByKey("PathPaymentOrder");
                if (string.IsNullOrWhiteSpace(pathServer))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: PathPaymentOrder"));
                }

                var order = _paymentRepo.GetByID(paymentOrderID);

                string pathPattern = $@"NĂM {order.DateOrder.Value.Year}\ĐỀ NGHỊ THANH TOÁN\THÁNG {order.DateOrder.Value.ToString("MM.yyyy")}\{order.DateOrder.Value.ToString("dd.MM.yyyy")}\{order.Code}";
                string pathUpload = Path.Combine(pathServer, pathPattern);

                foreach (var file in files)
                {
                    var result = await FileHelper.UploadFile(file, pathUpload);

                    if (result.status == 1)
                    {
                        var orderFile = new PaymentOrderFile();
                        orderFile.PaymentOrderID = order.ID;
                        //orderFile.FileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now:yyyyMMdd_HHmmss}{Path.GetExtension(file.FileName)}";
                        orderFile.FileName = TextUtils.ToString(result.data);
                        orderFile.OriginPath = "";
                        orderFile.ServerPath = pathUpload;

                        await _fileRepo.CreateAsync(orderFile);
                    }
                }

                //Process.Start(pathUpload);

                return Ok(ApiResponseFactory.Success(null, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("upload-file-bankslip")]
        public async Task<IActionResult> UploadFileBankSlip()
        {
            try
            {
                //_currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey") ?? "");

                var form = await Request.ReadFormAsync();
                var paymentOrderID = TextUtils.ToInt32(form["PaymentOrderID"]);
                var files = Request.Form.Files;

                // Lấy đường dẫn từ ConfigSystem
                var pathServer = _configSystemRepo.GetUploadPathByKey("PathPaymentOrder");
                if (string.IsNullOrWhiteSpace(pathServer))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: PathPaymentOrder"));
                }

                var order = _paymentRepo.GetByID(paymentOrderID);

                string pathPattern = $@"NĂM {order.DateOrder.Value.Year}\ĐỀ NGHỊ THANH TOÁN\THÁNG {order.DateOrder.Value.ToString("MM.yyyy")}\{order.DateOrder.Value.ToString("dd.MM.yyyy")}\{order.Code}";
                string pathUpload = Path.Combine(pathServer, pathPattern);

                foreach (var file in files)
                {
                    var result = await FileHelper.UploadFile(file, pathUpload);

                    if (result.status == 1)
                    {
                        var orderFile = new PaymentOrderFileBankSlip();
                        orderFile.PaymentOrderID = order.ID;
                        orderFile.FileName = TextUtils.ToString(result.data);
                        orderFile.OriginPath = "";
                        orderFile.ServerPath = pathUpload;

                        await _fileBankSlipRepo.CreateAsync(orderFile);
                    }
                }

                //Process.Start(pathUpload);

                return Ok(ApiResponseFactory.Success(null, "Cập nhật thành công!"));
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


                var customers = _customerRepo.GetAll(x => x.IsDeleted != true).OrderByDescending(x => x.ID).ToList();
                var pokhs = _poKHRepo.GetAll(x => x.IsDeleted != true).OrderByDescending(x => x.ID).ToList();
                var pokhDetails = _pOKHDetailRepo.GetAll(x => x.IsDeleted != true).OrderByDescending(x => x.ID).ToList();
                var userTeamNames = _employeeTeamSaleRepo.GetAll(x => x.IsDeleted == 0).OrderByDescending(x => x.ID).ToList();

                var data = new
                {
                    paymentOrderTypes,
                    approvedTBPs,
                    supplierSales,
                    poNCCs,
                    registerContracts,
                    projects,

                    customers,
                    pokhs,
                    pokhDetails,
                    userTeamNames
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
        public async Task<IActionResult> ApprovedHR([FromBody] List<PaymentOrderDTO> payment)
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

        [HttpPost("appoved-kttt")]
        public async Task<IActionResult> ApprovedKTTT([FromBody] List<PaymentOrderDTO> payment)
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

        [HttpPost("appoved-ktt")]
        public async Task<IActionResult> ApprovedKTT([FromBody] List<PaymentOrderDTO> payment)
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

        [HttpPost("appoved-bgd")]
        public async Task<IActionResult> ApprovedBGD([FromBody] List<PaymentOrderDTO> payment)
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
        [HttpGet("get-data-from-poncc/{ponccID}")]
        public IActionResult GetDataFromPONCC(int ponccID)
        {
            try
            {
                var poNCC = _poNccRepo.GetByID(ponccID);

                if (poNCC == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy PO NCC"));

                var supplierSale = _supplierSaleRepo.GetByID(poNCC.SupplierSaleID ?? 0);
                var currency = _currencyRepo.GetByID(poNCC.CurrencyID ?? 0);
                var currencyCode = currency?.Code ?? "VND";
                // Lấy details
                var details = _poNccDetailRepo.GetAll(x => x.PONCCID == ponccID).ToList();

                // Lấy tất cả ProductSale và ProductRTC (không dùng Contains)
                var allProductSales = _productSaleRepo.GetAll();
                var allProductRTCs = _productRTCRepo.GetAll();
                var allUnitCounts = _unitCountKTRepo.GetAll();

                // Tạo Dictionary để lookup
                var productSaleDict = allProductSales.ToDictionary(x => x.ID);
                var productRTCDict = allProductRTCs.ToDictionary(x => x.ID);
                var unitCountDict = allUnitCounts.ToDictionary(x => x.ID);

                // Map data
                var poNCCDetails = details.Select(detail =>
                {
                    string productName = "";
                    string unit = "";

                    if (detail.ProductSaleID > 0 && productSaleDict.TryGetValue(detail.ProductSaleID ?? 0, out var productSale))
                    {
                        productName = productSale.ProductName ?? "";
                        unit = productSale.Unit ?? "";
                    }
                    else if (detail.ProductRTCID > 0 && productRTCDict.TryGetValue(detail.ProductRTCID ?? 0, out var productRTC))
                    {
                        productName = productRTC.ProductName ?? "";
                        if (productRTC.UnitCountID > 0 && unitCountDict.TryGetValue(productRTC.UnitCountID ?? 0, out var unitCount))
                        {
                            unit = unitCount.UnitCountName ?? "";
                        }
                    }

                    return new
                    {
                        STT = detail.STT,
                        QtyRequest = detail.QtyRequest,
                        UnitPrice = detail.UnitPrice,
                        TotalPrice = detail.TotalPrice,
                        Note = detail.Note,
                        ProductSaleID = detail.ProductSaleID,
                        ProductRTCID = detail.ProductRTCID,
                        ProductName = productName,
                        Unit = unit
                    };
                }).ToList();

                return Ok(ApiResponseFactory.Success(new
                {
                    poNCC = new
                    {
                        poNCC.ID,
                        poNCC.POCode,
                        poNCC.BillCode,
                        poNCC.SupplierSaleID,
                        poNCC.AccountNumberSupplier,
                        poNCC.BankSupplier,
                        poNCC.TotalMoneyPO,
                        poNCC.CurrencyID,
                        poNCC.CurrencyRate,
                        Unit = currencyCode
                    },
                    supplierSale = supplierSale == null ? null : new
                    {
                        supplierSale.ID,
                        supplierSale.NameNCC,
                        supplierSale.SoTK,
                        supplierSale.NganHang
                    },
                    poNCCDetails
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

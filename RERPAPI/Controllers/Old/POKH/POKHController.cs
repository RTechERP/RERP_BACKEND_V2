using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Data;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.Old.POKH
{
    [Route("api/[controller]")]
    [ApiController]
    public class POKHController : ControllerBase
    {
        private readonly string _uploadPath;
        private readonly POKHRepo _pokhRepo;
        private readonly POKHDetailRepo _pokhDetailRepo;
        private readonly POKHDetailMoneyRepo _pokhDetailMoneyRepo;
        private readonly POKHFilesRepo _pokhFilesRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly CurrencyRepo _currencyRepo;
        private readonly ProductGroupRepo _productGroupRepo;
        private readonly ConfigSystemRepo _configSystemRepo;

        public POKHController(
            IWebHostEnvironment environment,
            POKHRepo pokhRepo,
            POKHDetailRepo pokhDetailRepo,
            POKHDetailMoneyRepo pokhDetailMoneyRepo,
            POKHFilesRepo pokhFilesRepo,
            ProjectRepo projectRepo,
            CurrencyRepo currencyRepo,
            ProductGroupRepo productGroupRepo,
            ConfigSystemRepo configSystemRepo)
        {
            _pokhRepo = pokhRepo;
            _pokhDetailRepo = pokhDetailRepo;
            _pokhDetailMoneyRepo = pokhDetailMoneyRepo;
            _pokhFilesRepo = pokhFilesRepo;
            _projectRepo = projectRepo;
            _currencyRepo = currencyRepo;
            _productGroupRepo = productGroupRepo;
            _configSystemRepo = configSystemRepo;

            //_uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "POKH");
            //if (!Directory.Exists(_uploadPath))
            //{
            //    Directory.CreateDirectory(_uploadPath);
            //}
        }
        #region Các hàm get dữ liệu
        [HttpGet("get-product")]
        public IActionResult loadProduct()
        {
            try
            {
                var listGroup = _productGroupRepo.GetAll().Select(x => x.ID).ToList();
                var idGroup = string.Join(",", listGroup);
                List<List<dynamic>> listProduct = SQLHelper<dynamic>.ProcedureToList("spGetProductSale", new string[] { "@IDgroup" }, new object[] { idGroup });
                var data = SQLHelper<dynamic>.GetListData(listProduct, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-pokh-kpi-detail")]
        public IActionResult loadPOKHKpiDetail(int id)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetPOKH_KPI", new string[] { "@IDMaster" }, new object[] { id });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        [HttpGet("get-detail-user")]
        public IActionResult loadDetailUser(int id, int idDetail)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetPOKHDetail_New", new string[] { "@ID", "@IDDetail" }, new object[] { id, idDetail });
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-pokh")]
        public IActionResult GetPOKH(string? filterText, int page, int size, int customerId, int userId, int POType, int status, int group, DateTime startDate, DateTime endDate, int warehouseId, int employeeTeamSaleId)
        {
            try
            {

                filterText = filterText ?? string.Empty;
                List<List<dynamic>> POKHs = SQLHelper<dynamic>.ProcedureToList("spGetPOKH",
                    new string[] { "@FilterText", "@PageNumber", "@PageSize", "@CustomerID", "@UserID", "@POType", "@Status", "@Group", "@StartDate", "@EndDate", "@WarehouseID", "@EmployeeTeamSaleID" },
                    new object[] { filterText, page, size, customerId, userId, POType, status, group, startDate, endDate, warehouseId, employeeTeamSaleId });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<dynamic>.GetListData(POKHs, 0),
                    totalPages = SQLHelper<dynamic>.GetListData(POKHs, 1),
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-employee-manager")]
        public IActionResult LoadEmployeeManager(int group, int userId, int teamId)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetEmployeeManager",
                    new string[] { "@group", "@UserID", "@TeamID" },
                    new object[] { group, userId, teamId });
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-pokh-product")]
        public IActionResult getPOKHProduct(int id, int idDetail)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetPOKHDetail_New", new string[] { "@ID", "@IDDetail" }, new object[] { id, idDetail });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-pokh-files")]
        public IActionResult getPOKHFiles(int id)
        {
            try
            {
                List<POKHFile> file = _pokhFilesRepo.GetAll().Where(x => x.IsDeleted != true && x.POKHID == id).ToList();
                return Ok(ApiResponseFactory.Success(file, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-project")]
        public IActionResult LoadProject()
        {
            try
            {
                var list = _projectRepo.GetAll().Select(x => new {x.ID, x.ProjectCode, x.UserID, x.ContactID, x.CustomerID, x.ProjectName, x.PO});
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-typePO")]
        public IActionResult LoadTypePO()
        {

            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetMainIndex", new string[] { "@Type" }, new object[] { 1 });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
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

        [HttpGet("get-currency")]
        public IActionResult LoadCurrency()
        {
            try
            {
                List<Currency> listCurrency = _currencyRepo.GetAll().ToList();
                return Ok(ApiResponseFactory.Success(listCurrency, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                RERPAPI.Model.Entities.POKH pokh = _pokhRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(pokh, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region Hàm lưu dữ liệu POKH, POKHDetail, POKHDetailMoney
        /// <summary>
        /// Xử lý tạo mới hoặc cập nhật đơn đặt hàng POKH và các chi tiết liên quan.
        /// </summary>
        /// Trả về đối tượng JSON chứa trạng thái thực thi (thành công hoặc thất bại), thông điệp và ID của đơn POKH.
        /// </returns>
        /// <remarks>
        /// - Nếu ID của POKH <= 0: thực hiện tạo mới đơn POKH.
        /// - Nếu ID > 0: thực hiện cập nhật thông tin đơn POKH hiện có.
        /// - Duyệt qua danh sách POKHDetails để tạo mới hoặc cập nhật từng dòng chi tiết đơn.
        ///   + Ánh xạ lại ParentID cho các chi tiết mới.
        /// - Duyệt qua danh sách POKHDetailsMoney để xử lý tạo/cập nhật thông tin.
        /// </remarks>
        [HttpPost("handle")]
        public async Task<IActionResult> Handle([FromBody] POKHDTO dto)
        {
            try
            {
                // Nếu request chỉ gửi flag (duyệt/hủy duyệt hoặc xóa po) mà không có detail thì bỏ qua ValidatePOKH
                bool isFlagOnlyAction = dto?.POKH != null
                    && dto.POKH.ID > 0
                    && (dto.POKHDetails == null || dto.POKHDetails.Count == 0)
                    && (dto.POKHDetailsMoney == null || dto.POKHDetailsMoney.Count == 0)
                    && (dto.POKH.IsApproved.HasValue || dto.POKH.IsDeleted.HasValue);

                if (isFlagOnlyAction)
                {
                    var existingPO = _pokhRepo.GetByID(dto.POKH.ID);
                    if (existingPO == null)
                        return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy POKH để cập nhật"));

                    if (dto.POKH.IsApproved.HasValue)
                        existingPO.IsApproved = dto.POKH.IsApproved;

                    if (dto.POKH.IsDeleted.HasValue)
                        existingPO.IsDeleted = dto.POKH.IsDeleted;

                    existingPO.UpdatedDate = DateTime.Now;
                    await _pokhRepo.UpdateAsync(existingPO);

                    var msgs = new List<string>();
                    if (dto.POKH.IsApproved.HasValue)
                        msgs.Add(dto.POKH.IsApproved.Value ? "Duyệt POKH thành công" : "Hủy duyệt POKH thành công");
                    if (dto.POKH.IsDeleted.HasValue)
                        msgs.Add("Xóa POKH thành công");

                    return Ok(ApiResponseFactory.Success(new { id = existingPO.ID }, string.Join(" - ", msgs)));
                }
                var errors = ValidatePOKH(dto);
                if (errors.Any())
                {
                    return Ok(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ", new { Errors = errors }));
                }

                if (dto.POKH.ID <= 0)
                {
                    await _pokhRepo.CreateAsync(dto.POKH);
                }
                else
                {
                    await _pokhRepo.UpdateAsync(dto.POKH);

                }
                var parentIdMapping = new Dictionary<int, int>();
                if (dto.POKHDetails.Count > 0)
                {
                    foreach (var item in dto.POKHDetails)
                    {
                        int idOld = item.ID;
                        int parentId = 0;

                        if (item.IsDeleted == true && idOld > 0)
                        {
                            var existing = _pokhDetailRepo.GetByID(idOld);
                            if (existing != null)
                            {
                                existing.IsDeleted = true;
                                await _pokhDetailRepo.UpdateAsync(existing);
                            }
                            continue;
                        }

                        if (item.ParentID.HasValue && parentIdMapping.ContainsKey(item.ParentID.Value))
                        {
                            parentId = parentIdMapping[item.ParentID.Value];
                        }

                        POKHDetail model = idOld > 0 ? _pokhDetailRepo.GetByID(idOld) : new POKHDetail();
                        // gán id và gán parent id
                        model.POKHID = dto.POKH.ID;
                        model.ParentID = parentId;
                        //
                        model.ProductID = item.ProductID;
                        model.STT = item.STT;
                        model.KHID = item.KHID;
                        model.GuestCode = item.GuestCode;
                        model.Qty = item.Qty;
                        model.FilmSize = item.FilmSize;
                        model.UnitPrice = item.UnitPrice;
                        model.IntoMoney = item.IntoMoney;
                        model.VAT = item.VAT;
                        model.Spec = item.Spec;
                        model.NetUnitPrice = item.NetUnitPrice;
                        model.TotalPriceIncludeVAT = item.TotalPriceIncludeVAT;
                        model.UserReceiver = item.UserReceiver;
                        model.DeliveryRequestedDate = item.DeliveryRequestedDate;
                        model.EstimatedPay = item.EstimatedPay;
                        model.BillDate = item.BillDate;
                        model.BillNumber = item.BillNumber;
                        model.Debt = item.Debt;
                        model.PayDate = item.PayDate;
                        model.GroupPO = item.GroupPO;
                        model.ActualDeliveryDate = item.ActualDeliveryDate;
                        model.RecivedMoneyDate = item.RecivedMoneyDate;
                        model.Note = item.Note;
                        model.IsDeleted = item.IsDeleted;


                        if (idOld > 0)
                        {
                            await _pokhDetailRepo.UpdateAsync(model);
                        }
                        else
                        {
                            await _pokhDetailRepo.CreateAsync(model);
                        }
                        parentIdMapping.Add(item.ID, model.ID);
                    }
                }
                if (dto.POKHDetailsMoney != null && dto.POKHDetailsMoney.Count > 0)
                {
                    foreach (var item in dto.POKHDetailsMoney)
                    {
                        int idOld = item.ID;
                        POKHDetailMoney detailMoney = idOld > 0 ? _pokhDetailMoneyRepo.GetByID(idOld) : new POKHDetailMoney();
                        detailMoney.POKHID = dto.POKH.ID;
                        detailMoney.POKHDetailID = 0;
                        detailMoney.PercentUser = item.PercentUser / 100;
                        detailMoney.UserID = item.UserID;
                        detailMoney.MoneyUser = item.MoneyUser;
                        detailMoney.RowHandle = item.RowHandle;
                        detailMoney.STT = item.STT;
                        detailMoney.ReceiveMoney = item.ReceiveMoney;
                        detailMoney.Month = dto.POKH.Month;
                        detailMoney.Year = dto.POKH.Year;
                        detailMoney.CreatedDate = DateTime.Now;
                        detailMoney.IsDeleted = item.IsDeleted;

                        if (idOld > 0)
                        {
                            await _pokhDetailMoneyRepo.UpdateAsync(detailMoney);
                        }
                        else
                        {
                            await _pokhDetailMoneyRepo.CreateAsync(detailMoney);
                        }
                    }
                }

                //return Ok(new
                //{
                //    status = 1,
                //    message = "Success",
                //    data = new { id = dto.POKH.ID }
                //});
                return Ok(ApiResponseFactory.Success(new {id = dto.POKH.ID}, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        private List<string> ValidatePOKH(POKHDTO dto)
        {
            List<string> errors = new List<string>();

            // =======================
            // Validate POKH Main
            // =======================
            var p = dto.POKH;

            if (p == null)
            {
                errors.Add("POKH không được để trống");
                return errors;
            }
             
            if (p.POCode.Length > 100)
                errors.Add("POCode không được vượt quá 200 ký tự");

            if (p.UserName?.Length > 200)
                errors.Add("UserName không được vượt quá 200 ký tự");

            if (p.BillCode?.Length > 200)
                errors.Add("BillCode không được vượt quá 200 ký tự");

            if (p.PONumber?.Length > 200)
                errors.Add("PONumber không được vượt quá 200 ký tự");

            if (p.Note?.Length > 500)
                errors.Add("Ghi chú không được vượt quá 500 ký tự");


            // =======================
            // Validate POKH Details
            // =======================
            //if (dto.POKHDetails != null)
            //{
            //    int row = 1;
            //    foreach (var d in dto.POKHDetails)
            //    {
            //        if (d.IsDeleted == true)
            //        {
            //            row++;
            //            continue;
            //        }

            //        if (!d.ProductID.HasValue || d.ProductID <= 0)
            //            errors.Add($"Dòng {row}: ProductID là bắt buộc");

            //        if (string.IsNullOrWhiteSpace(d.GuestCode) == false && d.GuestCode.Length > 100)
            //            errors.Add($"Dòng {row}: GuestCode vượt quá 100 ký tự");

            //        if (string.IsNullOrWhiteSpace(d.FilmSize) == false && d.FilmSize.Length > 200)
            //            errors.Add($"Dòng {row}: FilmSize vượt quá 200 ký tự");

            //        if (d.Qty.HasValue && d.Qty < 0)
            //            errors.Add($"Dòng {row}: Qty không được âm");

            //        if (d.UnitPrice.HasValue && d.UnitPrice < 0)
            //            errors.Add($"Dòng {row}: UnitPrice không được âm");

            //        if (d.VAT.HasValue && (d.VAT < 0 || d.VAT > 100))
            //            errors.Add($"Dòng {row}: VAT phải từ 0–100%");

            //        if (!string.IsNullOrWhiteSpace(d.BillNumber) && d.BillNumber.Length > 100)
            //            errors.Add($"Dòng {row}: BillNumber vượt quá 100 ký tự");

            //        if (!string.IsNullOrWhiteSpace(d.Note) && d.Note.Length > 500)
            //            errors.Add($"Dòng {row}: Note vượt quá 500 ký tự");

            //        row++;
            //    }
            //}


            // =======================
            // Validate POKH Detail Money
            // =======================
            //if (dto.POKHDetailsMoney != null)
            //{
            //    int rowM = 1;
            //    foreach (var m in dto.POKHDetailsMoney)
            //    {
            //        if (m.IsDeleted == true)
            //        {
            //            rowM++;
            //            continue;
            //        }

            //        if (!m.UserID.HasValue || m.UserID <= 0)
            //            errors.Add($"Tiền Row {rowM}: UserID là bắt buộc");

            //        if (m.PercentUser.HasValue && (m.PercentUser < 0 || m.PercentUser > 100))
            //            errors.Add($"Tiền Row {rowM}: PercentUser phải từ 0–100");

            //        if (m.ReceiveMoney.HasValue && m.ReceiveMoney < 0)
            //            errors.Add($"Tiền Row {rowM}: ReceiveMoney không được âm");

            //        if (m.STT.HasValue && m.STT < 0)
            //            errors.Add($"Tiền Row {rowM}: STT không hợp lệ");

            //        if (m.RowHandle.HasValue && m.RowHandle < 0)
            //            errors.Add($"Tiền Row {rowM}: RowHandle không hợp lệ");

            //        rowM++;
            //    }
            //}

            return errors;
        }

        #endregion
        //Tạo POCode
        [HttpGet("generate-POcode")]
        public IActionResult GeneratePOCode(string customer, bool isCopy, int warehouseID, int pokhID)
        {
            try
            {
                if (pokhID > 0 && !isCopy && warehouseID != 2)
                    return BadRequest("Không cần tạo PO Code mới.");

                string maPO = DateTime.Now.ToString("ddMMyyy");
                var latestCode = _pokhRepo.FindLastestCode(customer);

                string[] arr = latestCode?.Split('.') ?? new string[0];
                uint nextNumber;

                if (arr.Length < 2 || !uint.TryParse(arr[1], out nextNumber))
                {
                    nextNumber = 1;
                }
                else
                {
                    nextNumber++;
                }

                string newPOCode = $"{customer}_{maPO}.{nextNumber}";
                return Ok(ApiResponseFactory.Success(newPOCode, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #region Hàm xử lí File và lưu bảng POKHFile
        [HttpPost("upload")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(int poKHID)
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var key = form["key"].ToString();
                var files = form.Files;

                // Kiểm tra input
                if (string.IsNullOrWhiteSpace(key))
                    return BadRequest(ApiResponseFactory.Fail(null, "Key không được để trống!"));

                if (files == null || files.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách file không được để trống!"));

                var po = _pokhRepo.GetByID(poKHID);
                if (po == null)
                    throw new Exception("POKH not found");

                var uploadPath = _configSystemRepo.GetUploadPathByKey(key);
                if (string.IsNullOrWhiteSpace(uploadPath))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));

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
                    targetFolder = Path.Combine(uploadPath, $"NB{po.ID}");
                }

                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                var processedFile = new List<POKHFile>();

                foreach (var file in files)
                {
                    if (file.Length <= 0) continue;

                    // Tạo tên file unique để tránh trùng lặp
                    var fileExtension = Path.GetExtension(file.FileName);
                    var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var uniqueFileName = $"{originalFileName}{fileExtension}";
                    var fullPath = Path.Combine(targetFolder, uniqueFileName);

                    // Lưu file trực tiếp vào targetFolder (không tạo file tạm khác)
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var filePO = new POKHFile
                    {
                        POKHID = po.ID,
                        FileName = uniqueFileName,
                        OriginPath = targetFolder,
                        ServerPath = targetFolder,
                        IsDeleted = false,
                        CreatedBy = User.Identity?.Name ?? "System",
                        CreatedDate = DateTime.Now,
                        UpdatedBy = User.Identity?.Name ?? "System",
                        UpdatedDate = DateTime.Now
                    };

                    await _pokhFilesRepo.CreateAsync(filePO);
                    processedFile.Add(filePO);
                }

                return Ok(ApiResponseFactory.Success(processedFile, $"{processedFile.Count} tệp đã được tải lên thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi upload file: {ex.Message}"));
            }
        }

        //#region Hàm xử lí File và lưu bảng POKHFile
        //[HttpPost("upload")]
        //public async Task<IActionResult> Upload(int poKHID, [FromForm] List<IFormFile> files)
        //{
        //    try
        //    {
        //        // Lấy thông tin POKH
        //        var po = _pokhRepo.GetByID(poKHID);
        //        if (po == null)
        //        {
        //            throw new Exception("POKH not found");
        //        }

        //        // Tạo thư mục local cho file
        //        string pathPattern = $"NB{po.ID}";
        //        string pathUpload = Path.Combine(_uploadPath, pathPattern);

        //        // Tạo thư mục nếu chưa tồn tại
        //        if (!Directory.Exists(pathUpload))
        //        {
        //            Directory.CreateDirectory(pathUpload);
        //        }

        //        var processedFile = new List<POKHFile>();

        //        // Lưu từng file vào thư mục local
        //        foreach (var file in files)
        //        {
        //            if (file.Length > 0)
        //            {
        //                string filePath = Path.Combine(pathUpload, file.FileName);

        //                using (var stream = new FileStream(filePath, FileMode.Create))
        //                {
        //                    await file.CopyToAsync(stream);
        //                }

        //                var filePO = new POKHFile
        //                {
        //                    POKHID = po.ID,
        //                    FileName = file.FileName,
        //                    OriginPath = pathUpload,
        //                    ServerPath = pathUpload,
        //                    IsDeleted = false,
        //                    CreatedBy = User.Identity?.Name ?? "System",
        //                    CreatedDate = DateTime.Now,
        //                    UpdatedBy = User.Identity?.Name ?? "System",
        //                    UpdatedDate = DateTime.Now
        //                };

        //                await _pokhFilesRepo.CreateAsync(filePO);
        //                processedFile.Add(filePO);
        //            }
        //        }

        //        //return Ok(new
        //        //{
        //        //    status = 1,
        //        //    message = $"{processedFile.Count} tệp đã được tải lên thành công",
        //        //    data = processedFile
        //        //});
        //        return Ok(ApiResponseFactory.Success(processedFile, $"{processedFile.Count} tệp đã được tải lên thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        [HttpPost("delete-file")]
        public IActionResult DeleteFile([FromBody] List<int> fileIds)
        {
            if (fileIds == null || !fileIds.Any())
            //return BadRequest(new { success = false, message = "Danh sách file ID không được trống" });
            {
                throw new Exception("Danh sách file ID không được trống");
            }    

            try
            {
                var results = new List<object>();

                foreach (var fileId in fileIds)
                {
                    var file = _pokhFilesRepo.GetByID(fileId);

                    // Cập nhật database
                    file.IsDeleted = true;
                    //file.UpdatedBy = User.Identity?.Name ?? "System";
                    //file.UpdatedDate = DateTime.UtcNow;
                    _pokhFilesRepo.Update(file);

                    // Xóa file vật lý
                    var physicalPath = Path.Combine(file.ServerPath, file.FileName);
                    if (System.IO.File.Exists(physicalPath))
                        System.IO.File.Delete(physicalPath);

                    results.Add(new { fileId, success = true, message = "Xóa thành công" });
                }

                return Ok(ApiResponseFactory.Success(results, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        [HttpPost("copy-dto")]
        public async Task<IActionResult> CopyFromDTO([FromBody] POKHDTO dto)
        {
            try
            {
                // 1. Tạo mới POKH (bỏ ID cũ)
                var newPOKH = new RERPAPI.Model.Entities.POKH
                {
                    Status = dto.POKH.Status,
                    POCode = dto.POKH.POCode,
                    UserID = dto.POKH.UserID,
                    ReceivedDatePO = dto.POKH.ReceivedDatePO,
                    TotalMoneyPO = dto.POKH.TotalMoneyPO,
                    TotalMoneyKoVAT = dto.POKH.TotalMoneyKoVAT,
                    Note = dto.POKH.Note,
                    IsApproved = dto.POKH.IsApproved,
                    CustomerID = dto.POKH.CustomerID,
                    PartID = dto.POKH.PartID,
                    ProjectID = dto.POKH.ProjectID,
                    POType = dto.POKH.POType,
                    NewAccount = dto.POKH.NewAccount,
                    EndUser = dto.POKH.EndUser,
                    IsBill = dto.POKH.IsBill,
                    UserType = dto.POKH.UserType,
                    QuotationID = dto.POKH.QuotationID,
                    PONumber = dto.POKH.PONumber,
                    WarehouseID = dto.POKH.WarehouseID,
                    CurrencyID = dto.POKH.CurrencyID,
                    Year = dto.POKH.Year,
                    Month = dto.POKH.Month,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now
                };
                await _pokhRepo.CreateAsync(newPOKH);

                // 2. Mapping ID cũ (FE gửi lên) -> ID mới
                var idMapping = new Dictionary<int, int>();

                // 3. Tạo mới từng POKHDetail, lưu mapping
                foreach (var item in dto.POKHDetails)
                {
                    var newDetail = new POKHDetail
                    {
                        POKHID = newPOKH.ID,
                        // Copy các trường khác
                        ProductID = item.ProductID,
                        STT = item.STT,
                        KHID = item.KHID,
                        GuestCode = item.GuestCode,
                        Qty = item.Qty,
                        FilmSize = item.FilmSize,
                        UnitPrice = item.UnitPrice,
                        IntoMoney = item.IntoMoney,
                        VAT = item.VAT,
                        Spec = item.Spec,
                        NetUnitPrice = item.NetUnitPrice,
                        TotalPriceIncludeVAT = item.TotalPriceIncludeVAT,
                        UserReceiver = item.UserReceiver,
                        DeliveryRequestedDate = item.DeliveryRequestedDate,
                        EstimatedPay = item.EstimatedPay,
                        BillDate = item.BillDate,
                        BillNumber = item.BillNumber,
                        Debt = item.Debt,
                        PayDate = item.PayDate,
                        GroupPO = item.GroupPO,
                        ActualDeliveryDate = item.ActualDeliveryDate,
                        RecivedMoneyDate = item.RecivedMoneyDate,
                        Note = item.Note,
                        IsDeleted = false,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                    };
                    await _pokhDetailRepo.CreateAsync(newDetail);
                    idMapping[item.ID] = newDetail.ID; // item.ID là ID cũ FE gửi lên (có thể là số âm hoặc 0)
                }

                // 4. Cập nhật lại ParentID cho đúng
                foreach (var item in dto.POKHDetails)
                {
                    if (item.ParentID.HasValue && idMapping.ContainsKey(item.ID))
                    {
                        var newDetailId = idMapping[item.ID];
                        var newDetail = _pokhDetailRepo.GetByID(newDetailId);

                        if (idMapping.ContainsKey(item.ParentID.Value))
                        {
                            newDetail.ParentID = idMapping[item.ParentID.Value];
                            _pokhDetailRepo.Update(newDetail);
                        }
                    }
                }

                // 5. Tạo mới các bản ghi POKHDetailMoney
                if (dto.POKHDetailsMoney != null)
                {
                    foreach (var item in dto.POKHDetailsMoney)
                    {
                        var newMoney = new POKHDetailMoney
                        {
                            POKHID = newPOKH.ID,
                            // Copy các trường khác
                            POKHDetailID = 0,
                            PercentUser = item.PercentUser / 100,
                            UserID = item.UserID,
                            MoneyUser = item.MoneyUser,
                            RowHandle = item.RowHandle,
                            STT = item.STT,
                            ReceiveMoney = item.ReceiveMoney,
                            Month = dto.POKH.Month,
                            Year = dto.POKH.Year,
                            CreatedDate = DateTime.Now,
                            IsDeleted = false
                        };
                        await _pokhDetailMoneyRepo.CreateAsync(newMoney);
                    }
                }

                //return Ok(new
                //{
                //    status = 1,
                //    message = "Copy thành công",
                //    data = new { id = newPOKH.ID }
                //});
                return Ok(ApiResponseFactory.Success(new {id = newPOKH.ID}, "Copy thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

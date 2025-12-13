using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.DocumentManager;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TrackingMarksController : ControllerBase
    {
        private readonly SealRegulationsRepo _sealRegulationsRepo;
        private readonly TrackingMarksRepo _trackingMarksRepo;
        private readonly TrackingMarksSealRepo _trackingMarksSealRepo;
        private readonly TrackingMarksTaxCompanyRepo _trackingMarksTaxCompanyRepo;
        private readonly TrackingMarksFileRepo _trackingMarksFileRepo;

        private readonly CurrentUser _currentUser;
        private readonly TaxCompanyRepo _taxCompanyRepo;
        private readonly DocumentTypeRepo _documentTypeRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly DepartmentRepo _departmentRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        private readonly vUserGroupLinksRepo _vUserGroupLinksRepo;
        private readonly NotifyRepo _notifyRepo;
        public TrackingMarksController(CurrentUser currentUser,
            TaxCompanyRepo taxCompanyRepo,
            DocumentTypeRepo documentTypeRepo,
            EmployeeRepo employeeRepo,
            DepartmentRepo departmentRepo,
            ConfigSystemRepo configSystemRepo,
            vUserGroupLinksRepo vUserGroupLinksRepo,
            NotifyRepo notifyRepo,
            SealRegulationsRepo sealRegulationsRepo,
            TrackingMarksRepo trackingMarksRepo,
            TrackingMarksSealRepo trackingMarksSealRepo,
            TrackingMarksTaxCompanyRepo trackingMarksTaxCompanyRepo,
            TrackingMarksFileRepo trackingMarksFileRepo
            )
        {
            _currentUser = currentUser;
            _taxCompanyRepo = taxCompanyRepo;
            _documentTypeRepo = documentTypeRepo;
            _employeeRepo = employeeRepo;
            _departmentRepo = departmentRepo;
            _configSystemRepo = configSystemRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _notifyRepo = notifyRepo;
            _sealRegulationsRepo = sealRegulationsRepo;
            _trackingMarksRepo = trackingMarksRepo;
            _trackingMarksSealRepo = trackingMarksSealRepo;
            _trackingMarksTaxCompanyRepo = trackingMarksTaxCompanyRepo;
            _trackingMarksFileRepo = trackingMarksFileRepo;
        }

        [HttpGet("get-all")]
        public IActionResult GetAll(
            DateTime dateStart,
            DateTime dateEnd,
            string keyword = "",
            int employeeId = 0,
            int departmentId = 0,
            int status = -1)
        {
            try
            {
                var data = SQLHelper<TrackingMarksDataDTO>.ProcedureToListModel(
                    "spGetAllTrackingMarks",
                    new[] { "@DateStart", "@DateEnd", "@KeyWord", "@EmployeeID", "@DepartMentID", "@Status" },
                    new object[]
                    {
                        dateStart.Date,
                        dateEnd.Date.AddDays(1).AddSeconds(-1),
                        keyword ?? "",
                        employeeId,
                        departmentId,
                        status
                    });

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var tracking = _trackingMarksRepo.GetByID(id);
                if (tracking == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại dữ liệu"));

                var seals = _trackingMarksSealRepo.GetAll()
                    .Where(x => x.TrackingMartkID == id).ToList();

                var taxs = _trackingMarksTaxCompanyRepo.GetAll()
                    .Where(x => x.TrackingMartkID == id).ToList();

                var files = _trackingMarksFileRepo.GetAll()
                    .Where(x => x.TrackingMarksID == id).ToList();

                var employee = _employeeRepo.GetByID(tracking.EmployeeID ?? 0);
                var department = _departmentRepo.GetByID(employee.DepartmentID ?? 0);

                return Ok(ApiResponseFactory.Success(new
                {
                    tracking,
                    seals,
                    taxs,
                    files,
                    employee,
                    department
                }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save")]
        public IActionResult CreateOrUpdate([FromBody] TrackingMarksDTO model)
        {
            bool isCreate = false;
            try
            {
                var entity = model.ID > 0
                    ? _trackingMarksRepo.GetByID(model.ID)
                    : new TrackingMark();

                if (entity == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại dữ liệu"));

                entity.RegisterDate = model.RegisterDate;
                entity.EmployeeID = model.EmployeeID > 0 ? model.EmployeeID : _currentUser.EmployeeID;
                entity.DocumentTypeID = model.DocumentTypeID;
                entity.DocumentName = model.DocumentName;
                entity.DocumentQuantity = model.DocumentQuantity ?? 0;
                entity.DocumentTotalPage = model.DocumentTotalPage ?? 0;
                entity.DocumentTypeID = model.DocumentTypeID;
                entity.Deadline = model.Deadline;
                entity.EmployeeSignID = model.EmployeeSignID;
                entity.IsUrgent = model.Deadline.HasValue;

                if (entity.ID > 0)
                {
                    if (entity.Status > 0)
                        return BadRequest(ApiResponseFactory.Fail(null, "Phiếu đã duyệt, không thể sửa"));

                    if (entity.EmployeeID != _currentUser.EmployeeID)
                        return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể sửa phiếu của người khác"));

                    entity.UpdatedBy = _currentUser.LoginName;
                    entity.UpdatedDate = DateTime.Now;
                    _trackingMarksRepo.Update(entity);
                }
                else
                {
                    entity.Status = 0;
                    entity.CreatedBy = _currentUser.LoginName;
                    entity.CreatedDate = DateTime.Now;

                    _trackingMarksRepo.Create(entity);

                    isCreate = true;

                }

                _trackingMarksSealRepo.CreateListByTrackingMarkId(
                    model.ListSeal, entity.ID);

                _trackingMarksTaxCompanyRepo.CreateListByTrackingMarkId(
                    model.ListTaxCompany, entity.ID);

                if (isCreate)
                {
                    string deadline = entity.Deadline.HasValue
                        ? entity.Deadline.Value.ToString("dd/MM/yyyy")
                        : "";

                    string text = $"Văn bản: {entity.DocumentName}\nDeadline: {deadline}";
                    _notifyRepo.AddNotify("ĐĂNG KÝ ĐÓNG DẤU", text, 156);
                }

                return Ok(ApiResponseFactory.Success(new { id = entity.ID }, "Thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("delete-tracking-marks")]
        public IActionResult Delete(int id)
        {
            try
            {
                var tracking = _trackingMarksRepo.GetByID(id);

                if (tracking == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại dữ liệu"));

                if (tracking.Status > 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Phiếu đã duyệt, không thể xóa"));

                if (tracking.EmployeeID != _currentUser.EmployeeID)
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể xóa phiếu của người khác"));

                _trackingMarksRepo.Delete(id);
                _trackingMarksSealRepo.DeleteByTrackingMarkId(tracking.ID);
                _trackingMarksTaxCompanyRepo.DeleteByTrackingMarkId(tracking.ID);
                _trackingMarksFileRepo.DeleteByTrackingMarkId(tracking.ID);
                return Ok(ApiResponseFactory.Success(null, "Đã xóa"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("update-expect-date")]
        public IActionResult UpdateExpectDateComplete([FromBody] TrackingMark model)
        {
            try
            {
                var tracking = _trackingMarksRepo.GetByID(model.ID);
                if (tracking == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại dữ liệu"));

                tracking.ExpectDateComplete = model.ExpectDateComplete;
                _trackingMarksRepo.Update(tracking);

                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("approve")]
        public IActionResult Approve([FromBody] TrackingMarksApprovedDTO dto)
        {
            try
            {
                var roles = _vUserGroupLinksRepo.GetAll()
                    .Where(x => x.Code == "N75" && x.UserID == _currentUser.ID)
                    .ToList();

                if (!roles.Any() && !_currentUser.IsAdmin && dto.status == 1)
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền duyệt"));

                if (!roles.Any() && !_currentUser.IsAdmin && dto.status == 2)
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền hủy duyệt"));

                foreach (var id in dto.listID)
                {
                    var tracking = _trackingMarksRepo.GetByID(id);
                    if (tracking == null)
                        continue;

                    if (dto.status == 2 && string.IsNullOrEmpty(dto.reasonCancel))
                        return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập lý do hủy"));

                    tracking.Status = dto.status;
                    tracking.ReasonCancel = dto.reasonCancel;
                    tracking.ApprovedID = _currentUser.EmployeeID;
                    tracking.ApprovedDate = DateTime.Now;

                    _trackingMarksRepo.Update(tracking);
                }

                return Ok(ApiResponseFactory.Success(null, "Thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile(int id, [FromForm] List<IFormFile> files)
        {
            try
            {
                var tracking = _trackingMarksRepo.GetByID(id);
                if (tracking == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại dữ liệu"));

                if (tracking.EmployeeID != _currentUser.EmployeeID)
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền upload"));

                var config = _configSystemRepo.GetAll()
                    .FirstOrDefault(x => x.KeyName == "TrackingMarks");

                if (config == null || string.IsNullOrEmpty(config.KeyValue))
                    return BadRequest(ApiResponseFactory.Fail(null, "Chưa cấu hình đường dẫn"));

                string pathUpload = Path.Combine(
                    config.KeyValue.Trim(),
                    $"NĂM {tracking.RegisterDate.Value.Year}",
                    $"THÁNG {tracking.RegisterDate.Value:MM.yyyy}",
                    tracking.RegisterDate.Value.ToString("dd.MM.yyyy")
                );

                Directory.CreateDirectory(pathUpload);

                foreach (var file in files)
                {
                    string fullPath = Path.Combine(pathUpload, file.FileName);
                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    _trackingMarksFileRepo.Create(new TrackingMarksFile
                    {
                        TrackingMarksID = id,
                        FileName = file.FileName,
                        ServerPath = pathUpload,
                        CreatedBy = _currentUser.LoginName,
                        CreatedDate = DateTime.Now
                    });
                }

                return Ok(ApiResponseFactory.Success(null, "Upload file thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("download-file")]
        public IActionResult DownloadFile(int id, string fileName)
        {
            try
            {
                var tracking = _trackingMarksRepo.GetByID(id);
                if (tracking == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại dữ liệu"));

                var file = _trackingMarksFileRepo.GetAll()
                    .FirstOrDefault(x => x.TrackingMarksID == id && x.FileName == fileName);

                if (file == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy file"));

                string fullPath = Path.Combine(file.ServerPath, file.FileName);
                if (!System.IO.File.Exists(fullPath))
                    return BadRequest(ApiResponseFactory.Fail(null, "File không tồn tại"));

                var bytes = System.IO.File.ReadAllBytes(fullPath);
                return File(bytes, "application/octet-stream", file.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("seal-regulations")]
        public IActionResult GetSealRegulations()
        {
            try
            {
                var data = _sealRegulationsRepo.GetAll()
                    .ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("tax-companies")]
        public IActionResult GetTaxCompanies()
        {
            try
            {
                var data = _taxCompanyRepo.GetAll(x => x.IsDeleted != true)
                    .ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("document-types")]
        public IActionResult GetDocumentTypes()
        {
            try
            {
                var data = _documentTypeRepo.GetAll(x => x.IsDeleted != true)
                    .ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employees")]
        public IActionResult LoadUser()
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
        public class TrackingMarksDTO : TrackingMark
        {
            public List<TrackingMarksSeal> ListSeal { get; set; }
            public List<TrackingMarksTaxCompany> ListTaxCompany { get; set; }
        }

        public class TrackingMarksApprovedDTO
        {
            public List<int> listID { get; set; }
            public int status { get; set; }
            public string reasonCancel { get; set; }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;

namespace RERPAPI.Controllers.Old.KETOAN
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentImportExportController : ControllerBase
    {
        private readonly DocumentImportRepo _documentImportRepo;
        private readonly DocumentExportRepo _documentExportRepo;

        public DocumentImportExportController(
            DocumentImportRepo documentImportRepo,
            DocumentExportRepo documentExportRepo
            )
        {
            _documentImportRepo = documentImportRepo;
            _documentExportRepo = documentExportRepo;
        }

        [HttpGet("get-document-import")]
        public IActionResult GetDocumentImport()
        {
            try
            {
                var data = _documentImportRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-document-export")]
        public IActionResult GetDocumentExport()
        {
            try
            {
                var data = _documentExportRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-document")]
        public IActionResult Delete(int documentType, int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "ID không hợp lệ"));
                }

                if (documentType == 1)
                {
                    // Xóa chứng từ nhập
                    var model = _documentImportRepo.GetByID(id);
                    if (model == null || model.IsDeleted == true)
                    {
                        return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy chứng từ"));
                    }

                    model.IsDeleted = true;
                    _documentImportRepo.Update(model);
                }
                else if (documentType == 2)
                {
                    // Xóa chứng từ xuất
                    var model = _documentExportRepo.GetByID(id);
                    if (model == null || model.IsDeleted == true)
                    {
                        return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy chứng từ"));
                    }

                    model.IsDeleted = true;
                    _documentExportRepo.Update(model);
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Loại chứng từ không hợp lệ"));
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("save-document-import-export")]
        public IActionResult Save(int documentType, string code, string name, int? id = null)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(code))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Mã chứng từ"));
                }


                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên chứng từ"));
                }


                if (documentType == 1)
                {
                    var Exist = _documentImportRepo.GetAll(x => x.DocumentImportCode == code.Trim() && x.ID != id && x.IsDeleted != true);

                    if (Exist.Count > 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(
                            null,
                            $"Mã chứng từ [{code}] đã tồn tại. Vui lòng kiểm tra lại!"
                        ));
                    }

                    if (id.HasValue && id.Value > 0)
                    {
                        var model = _documentImportRepo.GetByID(id.Value);
                        if (model == null)
                            return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy chứng từ"));

                        model.DocumentImportCode = code;
                        model.DocumentImportName = name;
                        _documentImportRepo.Update(model);
                    }
                    else
                    {
                        var model = new DocumentImport
                        {
                            DocumentImportCode = code,
                            DocumentImportName = name
                        };
                        _documentImportRepo.Create(model);
                    }
                }

                else if (documentType == 2)
                {
                    var Exist = _documentExportRepo.GetAll(x => x.Code == code.Trim() && x.ID != id && x.IsDeleted != true);

                    if (Exist.Count > 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(
                            null,
                            $"Mã chứng từ [{code}] đã tồn tại. Vui lòng kiểm tra lại!"
                        ));
                    }

                    if (id.HasValue && id.Value > 0)
                    {
                        var model = _documentExportRepo.GetByID(id.Value);
                        if (model == null)
                            return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy chứng từ"));

                        model.Code = code;
                        model.Name = name;
                        _documentExportRepo.Update(model);
                    }
                    else
                    {
                        var model = new DocumentExport
                        {
                            Code = code,
                            Name = name
                        };
                        _documentExportRepo.Create(model);
                    }
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Loại chứng từ không hợp lệ"));
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}

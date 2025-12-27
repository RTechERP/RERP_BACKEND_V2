using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentImportPONCCController : ControllerBase
    {
        private readonly DocumentImportPONCCRepo _documentRepo;
        private readonly BillDocumentImportLogRepo _billDocumentImportLogRepo;
        public DocumentImportPONCCController(
            DocumentImportPONCCRepo documentRepo,
            BillDocumentImportLogRepo billDocumentImportLogRepo
        )
        {
            _documentRepo = documentRepo;
            _billDocumentImportLogRepo = billDocumentImportLogRepo;
        }

        [HttpGet("get-by-BdiID/{bdiID}")]
        public IActionResult getDataByDocumentInportID (int bdiID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                      "spGetAllDocumentImportPONCC", new string[] { "@BillImportID" },
                   new object[] { bdiID }
                  );
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        status = 0,
                        error = ex.Message
                    });
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<DocumentImportPONCC> dtos)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return BadRequest(new { status = 0, error = "Danh sách rỗng." });

                foreach (var dto in dtos)
                {
                    if (dto.ID <= 0) // Thêm mới
                    {
                        await _documentRepo.CreateAsync(dto);
                        int newID = dto.ID;
                        // Tạo log cho bản ghi mới (nếu cần)
                        BillDocumentImportLog log = new BillDocumentImportLog
                        {
                            BillDocumentImportID = newID,
                            DocumentStatus = dto.Status,
                            LogDate = dto.DateRecive,
                            Note = $"LÝ DO HUỶ: {dto.ReasonCancel}\nGHI CHÚ: {dto.Note}",
                            DocumentImportID = dto.DocumentImportID
                        };
                        await _billDocumentImportLogRepo.CreateAsync(log);
                    }
                    else // Cập nhật
                    {
                        await _documentRepo.UpdateAsync(dto);
                        BillDocumentImportLog log = new BillDocumentImportLog
                        {
                            BillDocumentImportID = dto.ID,
                            DocumentStatus = dto.Status,
                            LogDate = dto.DateRecive,
                            Note = $"LÝ DO HUỶ: {dto.ReasonCancel}\nGHI CHÚ: {dto.Note}",
                            DocumentImportID = dto.DocumentImportID
                        };
                        await _billDocumentImportLogRepo.CreateAsync(log);
                    }
                }

                return Ok(new
                {
                    status = 1,
                    message = "Đã lưu thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    error = $"Lỗi khi lưu dữ liệu: {ex.Message}"
                });
            }
        }

        [HttpPost("update-document")]
        public async Task<IActionResult> updateDocument(UpdateDocumentParam filter)
        {
            try
            {
                foreach (var item in filter.idsPONCC)
                {
                    if (item <= 0) continue;
                    //if (deliverID != Global.UserID && !isAdmin) continue; đợi phân quyền
                    var result = _documentRepo.GetAll()
                    .Where(p => p.DocumentImportID == filter.documentImportID && p.PONCCID == item)
                    .FirstOrDefault() ?? new DocumentImportPONCC();

                    result.PONCCID = item;
                    result.DocumentImportID = filter.documentImportID;
                    result.IsAdditional = result.ID > 0;
                    result.EmployeeAdditionalID = 214;
                    result.DateAdditional = DateTime.Now;
                    if (result.ID <= 0) // Nếu là bản ghi mới
                    {
                       await _documentRepo.CreateAsync(result);
                    }
                    else if (result.Status != 1) // Nếu bản ghi tồn tại và trạng thái cho phép cập nhật
                    {
                        await _documentRepo.UpdateAsync(result);
                    }
                }

                return Ok(new
                {
                    status = 1,
                    message = "Đã bổ sung chứng từ thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(
                        new
                        {
                            status = 0,
                            error = ex.Message
                        });
            }
        }
    }
}

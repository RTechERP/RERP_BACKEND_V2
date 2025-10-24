using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Model.DTO;
using ZXing;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillDocumentImportController : ControllerBase
    {
        BillDocumentImportRepo _billDocumentImportRepo = new BillDocumentImportRepo();
        BillImportRepo _billImportRepo = new BillImportRepo();
        BillDocumentImportLogRepo _billDocumentImportLogRepo = new BillDocumentImportLogRepo();
        [HttpGet("")]
        public IActionResult getData()
        {
            try
            {
                List<BillDocumentImport> result = _billDocumentImportRepo.GetAll();
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-by-billID/{billID}")]

        public IActionResult getDatabyBillID(int billID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                      "spGetBillDocumentImport", new string[] { "@BillImportID" },
                   new object[] { billID }
                  );
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu theo ID thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<BillDocumentImportDTO> dtos)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    throw new Exception("Danh sách rỗng!");

                /*   // Validation
                   foreach (var dto in dtos)
                   {
                       if (dto.DocumentStatus <= 0)
                       {
                           return BadRequest(new
                           {
                               status = 0,
                               error = $"Vui lòng nhập Trạng thái của chứng từ [{dto.DocumentImportID ?? "N/A"}]."
                           });
                       }
                       if (dto.DocumentStatus == 2 && string.IsNullOrEmpty(dto.ReasonCancel))
                       {
                           return BadRequest(new
                           {
                               status = 0,
                               error = $"Vui lòng nhập Lý do hủy của chứng từ [{dto.DocumentImportID ?? "N/A"}]."
                           });
                       }
                   }
   */
                bool isStatus2 = false;
                int billImportID = 0;

                foreach (var dto in dtos)
                {
                    var existing = _billDocumentImportRepo.GetByID(dto.BillDocuments.ID);
                    if (existing == null)
                        continue;

                    billImportID = existing.BillImportID ?? 0;

                    // Check if Status or ReasonCancel has changed
                    bool isChanged = existing.DocumentStatus != dto.BillDocuments.DocumentStatus || existing.Note != dto.lydo;

                    if (!isChanged)
                    {
                        if (dto.BillDocuments.DocumentStatus == 2)
                            isStatus2 = true;
                        continue;
                    }

                    // Update the existing record
                    existing.DocumentStatus = dto.BillDocuments.DocumentStatus;
                    existing.Note = dto.note?.Trim(); // Map ReasonCancel to Note
                    existing.LogDate = DateTime.Now;
                  

                    await _billDocumentImportRepo.UpdateAsync(existing);

                    if (existing.DocumentStatus == 2)
                        isStatus2 = true;

                    // Create log entry
                    var log = new BillDocumentImportLog
                    {
                        BillDocumentImportID = existing.ID,
                        DocumentStatus = existing.DocumentStatus,
                        LogDate = DateTime.Now,
                        Note = $"LÝ DO HUỶ: {dto.lydo ?? ""}\nGHI CHÚ: {dto.note ?? ""}".Trim(),
                        CreatedDate = DateTime.Now
                    };
                    await _billDocumentImportLogRepo.CreateAsync(log);
                }

                // Update parent BillImport
                if (billImportID > 0)
                {
                    var parent = _billImportRepo.GetByID(billImportID);
                    if (parent != null)
                    {
                        parent.BillDocumentImportType = isStatus2 ? 2 : 1;
                        await _billImportRepo.UpdateAsync(parent);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Xử lý dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using System.Diagnostics.Contracts;
using ZXing;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillDocumentExportController : ControllerBase
    {
        BillDocumentExportRepo _billDocumentExportRepo = new BillDocumentExportRepo();
        BillExportRepo _billExportRepo=new BillExportRepo();    
        BillDocumentExportLogRepo _billDocumentExportLogRepo = new BillDocumentExportLogRepo();
        [HttpGet("")]
        public IActionResult getData()
        {
            try
            {
                List<BillDocumentExport> result = _billDocumentExportRepo.GetAll();
                return Ok(ApiResponseFactory.Success(result, "Xử lý thành công!"));
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
                      "spGetBillDocumentExport", new string[] { "@BillExportID" },
                   new object[] { billID }
                  );
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<BillDocumentExport> dtos)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    throw new Exception("Danh sách rỗng!");

                bool isStatus2 = false;
                int billExportID = 0;

                foreach (var dto in dtos)
                {
                    var existing = _billDocumentExportRepo.GetByID(dto.ID);
                    if (existing == null) continue;

                    billExportID = existing.BillExportID ?? 0; // lưu lại để cập nhật sau

                    bool isChanged = existing.Status != dto.Status || existing.Note != dto.Note;

                    if (!isChanged)
                    {
                        if (dto.Status == 2)
                            isStatus2 = true;

                        continue;
                    }

                    // cập nhật bản chính
                    existing.Status = dto.Status;
                    existing.Note = dto.Note;
                    existing.LogDate = DateTime.Now;

                    await _billDocumentExportRepo.UpdateAsync(existing);

                    if (existing.Status == 2)
                        isStatus2 = true;

                    // ghi log
                    var log = new BillDocumentExportLog
                    {
                        BillDocumentExportID = existing.ID,
                        Status = existing.Status,
                        Note = existing.Note,
                        LogDate = existing.LogDate,
                        CreatedDate = DateTime.Now
                    };
                    await _billDocumentExportLogRepo.CreateAsync(log);
                }

                // cập nhật trạng thái cha
                if (billExportID > 0)
                {
                    var parent = _billExportRepo.GetByID(billExportID);
                    if (parent != null)
                    {
                        parent.BillDocumentExportType = isStatus2 ? 2 : 1;
                        await _billExportRepo.UpdateAsync(parent);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Xử lý thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}

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

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillDocumentExportController : ControllerBase
    {
        private readonly BillDocumentExportRepo _billDocumentExportRepo;
        private readonly BillExportRepo _billExportRepo;
        private readonly BillDocumentExportLogRepo _billDocumentExportLogRepo;

        public BillDocumentExportController(
            BillDocumentExportRepo billDocumentExportRepo,
            BillExportRepo billExportRepo,
            BillDocumentExportLogRepo billDocumentExportLogRepo)
        {
            _billDocumentExportRepo = billDocumentExportRepo;
            _billExportRepo = billExportRepo;
            _billDocumentExportLogRepo = billDocumentExportLogRepo;
        }
        [HttpGet("")]
        public IActionResult getData()
        {
            try
            {
                List<BillDocumentExport> result = _billDocumentExportRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, ex.Message });
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
        public async Task<IActionResult> SaveData([FromBody] List<BillDocumentExport> dtos)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return BadRequest(new { status = 0, error = "Danh sách rỗng." });

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

                return Ok(new { status = 1 });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    error = ex.Message
                });
            }
        }

    }
}

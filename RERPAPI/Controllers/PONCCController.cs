using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PONCCController : ControllerBase
    {
        PONCCRepo _ponccRepo = new PONCCRepo();
        PONCCDetailRepo _ponccDetailRepo = new PONCCDetailRepo();
        PONCCRulePayRepo _ponccRulePayRepo = new PONCCRulePayRepo();
        DocumentImportPONCCRepo _documentImportRepo = new DocumentImportPONCCRepo();
        PONCCDetailRequestBuyRepo _detailRequestBuyRepo = new PONCCDetailRequestBuyRepo();
        BillImportDetailRepo _billImportDetailRepo = new BillImportDetailRepo();
        PONCCDetailLogRepo _detailLogRepo = new PONCCDetailLogRepo();
        SupplierSaleRepo _supplierSaleRepo = new SupplierSaleRepo();

        [HttpGet("getall")]
        public IActionResult GetAll([FromQuery] PONCCParam param)
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetPONCC_Khanh", ["@FilterText", "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@SupplierID", "@Status", "@EmployeeID"], [param.Keywords, param.Page, param.Size, param.DateStart, param.DateEnd, param.SupplierSaleID, param.Status, param.EmployeeID]);
                var dtAll = SQLHelper<dynamic>.GetListData(dt, 0);
                var dtCommercial = dtAll.Select(x => x.POType == 0).ToList();
                var dtPOBorrow = dtAll.Select(x => x.POType == 1).ToList();
                int totalPage = dt[1][0].TotalPage;
                return Ok(ApiResponseFactory.Success(new
                {
                    TotalPage = totalPage,
                    dtCommercial,
                    dtPOBorrow
                }, "Tải dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi: " + ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] PONCCDTO dto)
        {
            try
            {
                string message = string.Empty;

                // Validate cơ bản
                if (!_ponccRepo.Validate(dto, out message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }

                // Lưu PO chính
                if (dto.ID <= 0)
                {
                    await _ponccRepo.CreateAsync(dto);
                }
                else
                {
                    await _ponccRepo.UpdateAsync(dto);
                }

                // Lưu chi tiết PO
                foreach (var detail in dto.lstPONCCDetail)
                {
                    detail.PONCCID = dto.ID;
                    if (detail.ID <= 0)
                    {
                        await _ponccDetailRepo.CreateAsync(detail);
                    }
                    else
                    {
                        await _ponccDetailRepo.UpdateAsync(detail);
                    }
                }

                // Lưu điều khoản thanh toán
                foreach (var rule in dto.lstPONCCRulePay)
                {
                    rule.PONCCID = dto.ID;
                    if (rule.ID <= 0)
                    {
                        await _ponccRulePayRepo.CreateAsync(rule);
                    }
                    else
                    {
                        await _ponccRulePayRepo.UpdateAsync(rule);
                    }
                }

                // Lưu chứng từ nhập khẩu
                foreach (var doc in dto.lstDocumentImportPONCC)
                {
                    doc.PONCCID = dto.ID;
                    if (doc.ID <= 0)
                    {
                        await _documentImportRepo.CreateAsync(doc);
                    }
                    else
                    {
                        await _documentImportRepo.UpdateAsync(doc);
                    }
                }

                // Lưu mapping Detail <-> RequestBuy
                foreach (var map in dto.lstPONCCDetailRequestBuy)
                {
                    if (map.ID <= 0)
                    {
                        await _detailRequestBuyRepo.CreateAsync(map);
                    }
                    else
                    {
                        await _detailRequestBuyRepo.UpdateAsync(map);
                    }
                }

                // Lưu Bill nhập kho
                foreach (var bill in dto.lstBillImportDetail)
                {
                    if (bill.ID <= 0)
                    {
                        await _billImportDetailRepo.CreateAsync(bill);
                    }
                    else
                    {
                        await _billImportDetailRepo.UpdateAsync(bill);
                    }
                }

                // Lưu Log thay đổi chi tiết
                foreach (var log in dto.lstPONCCDetailLog)
                {
                    if (log.ID <= 0)
                    {
                        await _detailLogRepo.CreateAsync(log);
                    }
                    else
                    {
                        await _detailLogRepo.UpdateAsync(log);
                    }
                }

                return Ok(ApiResponseFactory.Success(dto, "Lưu dữ liệu PO thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(
                    ex,
                    $"Lỗi: {ex.Message}"
                ));
            }
        }

        [HttpPost("approved")]
        public async Task<IActionResult> Approved([FromBody] List<PONCC> lstPONCC)
        {
            try
            {
                foreach (var poncc in lstPONCC)
                {
                    string message = string.Empty;

                    // Validate cơ bản
                    if (!_ponccRepo.ValidateApproved(poncc, out message))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, message));
                    }

                    PONCC po = _ponccRepo.GetByID(poncc.ID);
                    po.IsApproved = poncc.IsApproved;
                    po.UpdatedBy = poncc.UpdatedBy;
                    po.UpdatedDate = DateTime.Now;

                    await _ponccRepo.UpdateAsync(po);
                }

                return Ok(ApiResponseFactory.Success(
                    lstPONCC,
                    $"Đã {(lstPONCC[0].IsApproved == true ? "duyệt" : "hủy duyệt")} thành công!"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(
                    ex,
                    $"Lỗi: {ex.Message}"
                ));
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] List<PONCC> lstPONCC)
        {
            try
            {
                foreach (var poncc in lstPONCC)
                {
                    string message = string.Empty;

                    // Validate xóa
                    if (!_ponccRepo.ValidateDelete(poncc, out message))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, message));
                    }

                    PONCC po = _ponccRepo.GetByID(poncc.ID);
                    po.IsDeleted = true;
                    po.UpdatedBy = poncc.UpdatedBy;
                    po.UpdatedDate = DateTime.Now;

                    await _ponccRepo.UpdateAsync(po);
                }

                return Ok(ApiResponseFactory.Success(
                    lstPONCC,
                    "Xóa PO thành công!"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(
                    ex,
                    $"Lỗi: {ex.Message}"
                ));
            }
        }

        [HttpGet("getponccdetail")]
        public IActionResult GetPONCCDetail(int poID)
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetPONCCDetail_Khanh",
                    new string[] { "@PONCCID" },
                    new object[] { poID });

                return Ok(ApiResponseFactory.Success(
                    SQLHelper<object>.GetListData(dt, 0),
                    "Lấy chi tiết PO thành công!"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(
                    ex,
                    $"Lỗi: {ex.Message}"
                ));
            }
        }

        [HttpGet("getpocode")]
        public IActionResult GetPOCode(int supplierSaleID)
        {
            try
            {
                string poCode = _ponccRepo.GetPOCode(supplierSaleID);
                return Ok(ApiResponseFactory.Success(poCode, "Tạo mã PO thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(
                    ex,
                    $"Lỗi: {ex.Message}"
                ));
            }
        }

        [HttpPost("getbillcode")]
        public IActionResult GetBillCode(PONCC model)
        {
            try
            {
                string billCode = _ponccRepo.GetBillCode(model);
                return Ok(ApiResponseFactory.Success(billCode, "Tạo mã Bill thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(
                    ex,
                    $"Lỗi: {ex.Message}"
                ));
            }
        }

        [HttpGet("getdocumentimport")]
        public IActionResult GetDocumentImport(int ponccId)
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetAllDocumentImportByPONCCID",
                    new string[] { "@PONCCID", "@BillImportID" },
                    new object[] { ponccId, 0 });

                return Ok(ApiResponseFactory.Success(
                    SQLHelper<object>.GetListData(dt, 0),
                    "Lấy danh sách chứng từ nhập khẩu thành công!"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(
                    ex,
                    $"Lỗi: {ex.Message}"
                ));
            }
        }

    }
}

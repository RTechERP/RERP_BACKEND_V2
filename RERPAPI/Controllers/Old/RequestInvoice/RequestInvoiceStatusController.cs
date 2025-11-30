using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.RequestInvoice
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestInvoiceStatusController : ControllerBase
    {
        private readonly RequestInvoiceStatusRepo _requestInvoiceStatusRepo;
        private readonly RequestInvoiceFileRepo _requestInvoiceFileRepo;
        private readonly RequestInvoiceStatusLinkRepo _requestInvoiceStatusLinkRepo;
        public RequestInvoiceStatusController(
            RequestInvoiceStatusRepo requestInvoiceStatusRepo,
            RequestInvoiceStatusLinkRepo requestInvoiceStatusLinkRepo,
            RequestInvoiceFileRepo requestInvoiceFileRepo
            )
        {
            _requestInvoiceStatusRepo = requestInvoiceStatusRepo;
            _requestInvoiceFileRepo = requestInvoiceFileRepo;
            _requestInvoiceStatusLinkRepo = requestInvoiceStatusLinkRepo;
        }
        [HttpGet("get-status-invoice")]
        public IActionResult GetMainStatusInvoice(int requestInvoiceId)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetRequestInvoiceStatus", new string[] { "@RequestInvoiceID" }, new object[] { requestInvoiceId });
                var mainData = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(mainData, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-status")]
        public IActionResult GetStatus()
        {
            try
            {
                var data = _requestInvoiceStatusRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-status-file")]
        public IActionResult GetStatusFile(int requestInvoiceId)
        {
            try
            {
                var mainData = _requestInvoiceFileRepo.GetAll(x => x.RequestInvoiceID == requestInvoiceId && x.IsDeleted != true && x.FileType == 2);
                return Ok(ApiResponseFactory.Success(mainData, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-status-invoice")]
        public async Task<IActionResult> SaveStatusInvoice(StatusRequestInvoiceDTO dto)
        {
            try
            {
                //cần thêm validate
                //Thiếu upload

                foreach (var item in dto.StatusRequestInvoiceLinks)
                {
                    RequestInvoiceStatusLink model = await _requestInvoiceStatusLinkRepo.GetByIDAsync(item.ID) ?? new RequestInvoiceStatusLink();
                    model.RequestInvoiceID = dto.requestInvoiceId;
                    model.StatusID = item.StatusID;
                    model.IsApproved = item.IsApproved;
                    model.IsCurrent = item.IsCurrent;
                    model.AmendReason = item.AmendReason;
                    model.IsDeleted = false;
                    model.CreatedDate = DateTime.Now;
                    if (item.ID <= 0)
                    {
                        await _requestInvoiceStatusLinkRepo.CreateAsync(model);
                    }
                    else
                    {
                        await _requestInvoiceStatusLinkRepo.UpdateAsync(model);
                    }
                }
                if (dto.listIdsStatusDel.Count > 0)
                {
                    foreach (var id in dto.listIdsStatusDel)
                    {
                        var statusDel = await _requestInvoiceStatusLinkRepo.GetByIDAsync(id);
                        if (statusDel != null)
                        {
                            statusDel.IsDeleted = true;
                            statusDel.UpdatedDate = DateTime.Now;
                            await _requestInvoiceStatusLinkRepo.UpdateAsync(statusDel);
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu trạng thái thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-status")]
        public async Task<IActionResult> SaveStatusInvoice(RequestInvoiceStatus model)
        {
            try
            {
                if(model.ID > 0)
                {
                    await _requestInvoiceStatusRepo.UpdateAsync(model);
                }
                else
                {
                    await _requestInvoiceStatusRepo.CreateAsync(model);
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu trạng thái thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

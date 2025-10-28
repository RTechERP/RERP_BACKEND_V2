using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.IssueSolution
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueLogSolutionController : ControllerBase
    {
        private readonly IssueLogSolutionRepo _issueLogSolutionRepo = new IssueLogSolutionRepo();
        private readonly IssueSolutionCauseLinkRepo _issueSolutionCauseLinkRepo = new IssueSolutionCauseLinkRepo();
        private readonly IssueSolutionStatusLinkRepo _issueSolutionStatusLinkRepo = new IssueSolutionStatusLinkRepo();
        private readonly IssueSolutionDocumentRepo _issueSolutionDocumentRepo = new IssueSolutionDocumentRepo();
        private readonly CustomerRepo _customerRepo = new CustomerRepo();
        private readonly ProjectRepo _projectRepo = new ProjectRepo();
        private readonly IssueCauseRepo _issueCauseRepo = new IssueCauseRepo();
        private readonly IssueSolutionStatusRepo _issueSolutionStatusRepo = new IssueSolutionStatusRepo();
        List<List<dynamic>> listDocuments = SQLHelper<dynamic>.ProcedureToList("spGetDocumentIssue", new string[] { }, new object[] { });

        [HttpGet()]
        public IActionResult GetAllIssueSolution(string? keyword, int? issueSolutionType)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetAllIssueLogSolution", new string[] { "@Keyword" , "@IssueSolutionType" }, new object[] { keyword, issueSolutionType });
                //List<dynamic> listPart = list[0];
                return Ok(ApiResponseFactory.Success(SQLHelper<dynamic>.GetListData(list, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-detail")]
        public IActionResult GetIssueSolutionDetail(int id)
        {
            try 
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetIssueLogSolutionDetail", new string[] { "@IssueSolutionID" }, new object[] { id });
                var mainData = SQLHelper<dynamic>.GetListData(list, 0);
                var docData = SQLHelper<dynamic>.GetListData(list, 1);
                return Ok(ApiResponseFactory.Success(new { mainData, docData }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-documents")]
        public IActionResult GetAllDocuments()
        {
            try
            {
                return Ok(ApiResponseFactory.Success(SQLHelper<dynamic>.GetListData(listDocuments, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-statuses")]
        public IActionResult GetAllStatuses()
        {
            try
            {
                var statuses = _issueSolutionStatusRepo.GetAll().Where(x=>x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(statuses, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-causes")]
        public IActionResult GetAllCause()
        {
            try
            {
                var causes = _issueCauseRepo.GetAll().Where(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(causes, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-all-customers")]
        public IActionResult GetAllCustomers(int id)
        {
            try
            {
                var customers = _customerRepo.GetAll();
                //List<Customer> customers = _customerRepo.GetAll();
                return Ok(ApiResponseFactory.Success(customers, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-projects")]
        public IActionResult GetProject()
        {
            try
            {
                var data = _projectRepo.GetAll().ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save")]
        public async Task<IActionResult> Save(IssueSolutionDTO dto)
        {
            try
            {
                if (dto.issueSolutionLogs.ID <= 0)
                {
                    await _issueLogSolutionRepo.CreateAsync(dto.issueSolutionLogs);
                }
                else
                {
                    await _issueLogSolutionRepo.UpdateAsync(dto.issueSolutionLogs);
                }
                var issueSolutionId = dto.issueSolutionLogs.ID;
                if(dto.issueSolutionCauseLink != null)
                {
                    var oldCauses = _issueSolutionCauseLinkRepo.GetAll()
                        .Where(x => x.IssueSolutionID == issueSolutionId && (x.IsDeleted == null || x.IsDeleted == false)).ToList();

                    foreach (var item in oldCauses)
                    {
                        item.IsDeleted = true;
                        _issueSolutionCauseLinkRepo.Update(item);
                    }

                    dto.issueSolutionCauseLink.IssueSolutionID = dto.issueSolutionLogs.ID;
                    await _issueSolutionCauseLinkRepo.CreateAsync(dto.issueSolutionCauseLink);
                    
                }
                if(dto.issueSolutionStatusLink != null)
                {
                    var oldStatus = _issueSolutionStatusLinkRepo.GetAll()
                        .Where(x => x.IssueSolutionID == issueSolutionId && (x.IsDeleted == null || x.IsDeleted == false)).ToList();

                    foreach (var item in oldStatus)
                    {
                        item.IsDeleted = true;
                        _issueSolutionStatusLinkRepo.Update(item);
                    }
                    dto.issueSolutionStatusLink.IssueSolutionID = dto.issueSolutionLogs.ID;
                    await _issueSolutionStatusLinkRepo.CreateAsync(dto.issueSolutionStatusLink);
                    
                }

                if(dto.issueSolutionDocuments != null && dto.issueSolutionDocuments.Count > 0)
                {

                    var oldDocuments = _issueSolutionDocumentRepo.GetAll()
                        .Where(x => x.IssueSolutionID == issueSolutionId && (x.IsDeleted == null || x.IsDeleted == false)).ToList();

                    foreach (var item in oldDocuments)
                    {
                        item.IsDeleted = true;
                        _issueSolutionDocumentRepo.Update(item);
                    }

                    foreach (var document in dto.issueSolutionDocuments)
                    {
                            document.IssueSolutionID = dto.issueSolutionLogs.ID;
                            await _issueSolutionDocumentRepo.CreateAsync(document);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Success"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

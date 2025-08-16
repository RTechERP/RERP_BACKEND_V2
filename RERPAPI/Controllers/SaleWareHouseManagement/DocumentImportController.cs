using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using ZXing;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentImportController : ControllerBase
    {
        DocumentImportRepo _documentImportRepo = new DocumentImportRepo();
        [HttpGet("")]
        public IActionResult getDocumentImportByPO(int poNCCId,int billImportID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                      "spGetAllDocumentImportByPONCCID", new string[] { "@PONCCID", "@BillImportID" },
                new object[] { poNCCId, billImportID }
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
        [HttpGet("dropdownmenu")]
        public IActionResult getDataContextMenu()
        {
            try
            {
                List<DocumentImport> result = _documentImportRepo.GetAll().Where(p => !p.IsDeleted.GetValueOrDefault()).ToList();
                return Ok(new
                {
                    status = 1,
                    data = result,
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

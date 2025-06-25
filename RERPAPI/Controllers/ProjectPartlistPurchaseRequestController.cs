using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using System.Data;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectPartlistPurchaseRequestController : ControllerBase
    {
        #region Khai báo repository
        #endregion
        #region Lấy tất cả yêu cầu mua hàng
        [HttpPost("get-all")]
        public IActionResult GetAll([FromBody] ProjectPartlistPurchaseRequestParam filter)
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetProjectPartlistPurchaseRequest_New",
                    new string[] {
                "@DateStart", "@DateEnd", "@StatusRequest", "@ProjectID", "@Keyword",
                "@SupplierSaleID", "@IsApprovedTBP", "@IsApprovedBGD", "@IsCommercialProduct",
                "@POKHID", "@ProductRTCID", "@IsDeleted", "@IsTechBought", "@IsJobRequirement","@PageSize","@PageNumber"
                    },
                    new object[] {
                filter.DateStart, filter.DateEnd, filter.StatusRequest, filter.ProjectID, filter.Keyword,
                filter.SupplierSaleID, filter.IsApprovedTBP , filter.IsApprovedBGD, filter.IsCommercialProduct,
                filter.POKHID, filter.ProductRTCID, filter.IsDeleted, filter.IsTechBought, filter.IsJobRequirement, filter.Size, filter.Page
                    });

                var allData = SQLHelper<dynamic>.GetListData(dt, 0);

                var purchaseRequests = allData.Where(x => x.ProductRTCID == null || x.ProductRTCID <= 0).ToList();
                var dataRTC = allData.Where(x => x.ProductRTCID > 0 && x.TicketType == 0).ToList();
                var techBought = allData.Where(x => x.IsTechBought == true).ToList();
                string loginname = "NV0319";
                string password = "MQA=";
                var loginUser = SQLHelper<dynamic>.ProcedureToList("spLogin", new string[] { "@LoginName", "@Password" }, new object[] { loginname, password });
                var userInfo = SQLHelper<dynamic>.GetListData(loginUser, 0);
                int employeeID = 0;
                if (userInfo != null && userInfo.Count > 0)
                {
                    employeeID = Convert.ToInt32(userInfo[0].EmployeeID ?? 0);

                }
                var productRTCBorrow = allData
                    .Where(x => x.TicketType == 1 && (filter.IsApprovedTBP==0 || x.ApprovedTBP == employeeID))
                    .ToList();

                var productCommercial = allData.Where(p => p.IsCommercialProduct==true).ToList();

                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        purchaseRequests,
                        dataRTC,
                        techBought,
                        productRTCBorrow,
                        productCommercial

                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        #endregion
    }
}

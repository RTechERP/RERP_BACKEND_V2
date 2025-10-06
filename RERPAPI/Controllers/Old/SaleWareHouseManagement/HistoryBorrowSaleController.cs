﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryBorrowSaleController : ControllerBase
    {
        BillExportDetailRepo _billExportDetailRepo= new BillExportDetailRepo();
        WarehouseRepo _wareHouseRepo = new WarehouseRepo();

        [HttpPost("")]

        public IActionResult getReport(HistoryBorrowParam filter)
        {
            var rs = _wareHouseRepo.GetAll().Where(x => x.WarehouseCode == "HN").FirstOrDefault();
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetHistoryBorrowSale",
                    new string[] { "@PageNumber", "@PageSize", "@DateBegin", "@DateEnd", "@ProductGroupID", "@ReturnStatus", "@FilterText", "@WareHouseID", "@EmployeeID" },
                    new object[] { filter.PageNumber, filter.PageSize, filter.DateStart, filter.DateEnd, filter .ProductGroupID, filter.Status -1 , filter.FilterText, rs.ID, filter.EmployeeID }
                    );
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
                });
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

        //đã, hủy trả
        [HttpPost("approved-returned")]

        public async Task<IActionResult> ApprovedReturned([FromBody] List<int> billExportDetailIds, bool isApproved)
        {
            try
            {
                string message = isApproved ? "đã trả" : "hủy trả";
                foreach (var id in billExportDetailIds)
                {
                    if (id <= 0)
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = $"ID không hợp lệ: {id}"
                        });
                    }

                    var result = _billExportDetailRepo.GetByID(id);
                    if (result == null)
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = $"Không tìm thấy bản ghi với ID: {id}"
                        });
                    }

                    result.ReturnedStatus = isApproved;
                    result.UpdatedDate = DateTime.Now;
                    _billExportDetailRepo.Update(result);
                }

                return Ok(new
                {
                    status = 1,
                    message = $"{(isApproved ? "Đã trả!" : "Đã hủy trả")}"
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
    }
}

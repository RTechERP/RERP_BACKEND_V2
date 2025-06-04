using Azure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Security.Cryptography;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeSuppliesController : ControllerBase
    {

       
        OfficeSuplyRepo off = new OfficeSuplyRepo();
        OfficeSupplyUnitRepo osurepo = new OfficeSupplyUnitRepo();
       

        [HttpGet("getdataofficesupplies")]
        public IActionResult GetOfficeSupplies(string keyword = "")
        {
            try
            {
                List<OficeSuppliesDTO> result = SQLHelper<OficeSuppliesDTO>.ProcedureToList(
              "spGetOfficeSupply",
              new string[] { "@KeyWord" },
             new object[] { keyword ?? "" }  // đảm bảo không null
          );
                var data = result.Where(x => x.IsDeleted == false).ToList();
                var nextCode = off.GetNextCodeRTC();

                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        officeSupply=data,
                        nextCode=nextCode,
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
        [HttpGet("getbyidofficesupplies")]
        public IActionResult GetbyIDOfficeSupplies(int id)
        {
            try
            {
                OfficeSupply dst = off.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = dst
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


        [HttpPost("deleteofficesupply")]
        public async Task<IActionResult> DeleteVpp([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest(new { status = 0, message = "Lỗi", error = ToString() });
                foreach (var id in ids)
                {
                    var item = off.GetByID(id);
                    if (item != null)
                    {
                        item.IsDeleted = true; // Gán trường IsDeleted thành true
                        /* await off.UpdateAsync(item);*/
                        off.UpdateFieldsByID(id, item);/* // Cập nhật lại mục*/
                    }
                }
                return Ok(new
                {
                    status = 1,
                    message = "Đã xóa thành công."
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
        [HttpPost("CheckCodes")]
        public async Task<IActionResult> CheckCodes([FromBody] List<ProductCodeCheck> codes)
        {
            try
            {
                // Lấy danh sách các mã cần kiểm tra
                var codeRTCs = codes.Select(x => x.CodeRTC).ToList();
                var codeNCCs = codes.Select(x => x.CodeNCC).ToList();

                // Kiểm tra trong database
                var existingProducts = off.GetAll()
                    .Where(x => codeRTCs.Contains(x.CodeRTC) || codeNCCs.Contains(x.CodeNCC))
                    .Select(x => new {
                        ID = x.ID, // Thêm ID vào đây
                        CodeRTC = x.CodeRTC,
                        CodeNCC = x.CodeNCC,
                        NameRTC = x.NameRTC,
                        NameNCC = x.NameNCC
                    })
                    .ToList();

                return Ok(new
                {
                    data = new
                    {
                        existingProducts = existingProducts
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        //cap nhat and them
        [HttpPost("addandupdate")]
        public async Task<IActionResult> AddandUpdate([FromBody] OfficeSupply officesupply)
        {
            try
            {
                if (officesupply.ID <= 0)
                {
                    await off.CreateAsync(officesupply);                   
                }
                else
                {
                    off.UpdateFieldsByID(officesupply.ID, officesupply);
                }
                return Ok(new
                {
                    status = 1,
                    data = officesupply,
                    message = "Cập nhật thành công!"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }

        }

    }
}

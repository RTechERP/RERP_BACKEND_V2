using Azure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Security.Cryptography;

namespace RERPAPI.Controllers.Old.OfficeSuppliesManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeSupplyController : ControllerBase
    {


        OfficeSupplyRepo _officesupplyRepo = new OfficeSupplyRepo();
        OfficeSupplyUnitRepo osurepo = new OfficeSupplyUnitRepo();


        [HttpGet("")]
        public IActionResult getOfficeSupply(string keyword = "")
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
              "spGetOfficeSupply",
              new string[] { "@KeyWord" },
             new object[] { keyword ?? "" }  // đảm bảo không null
          );
              
                var nextCode = _officesupplyRepo.GetNextCodeRTC();
                List<dynamic> rs = result[0];
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        officeSupply = rs,
                        nextCode,
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
        /// <summary>
        /// Hàm tìm kiếm data OfficeSupply theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult getOfficeSupplyByID(int id)
        {
            try
            {
                OfficeSupply dst = _officesupplyRepo.GetByID(id);
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


        [HttpPost("delete-office-supply")]
        public async Task<IActionResult> deleteOfficeSupply([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest(new { status = 0, message = "Lỗi", error = ToString() });
                foreach (var id in ids)
                {
                    var item = _officesupplyRepo.GetByID(id);
                    if (item != null)
                    {
                        item.IsDeleted = true; // Gán trường IsDeleted thành true
                        /* await _officesupplyRepo.UpdateAsync(item);*/
                        _officesupplyRepo.Update(item);/* // Cập nhật lại mục*/
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
        [HttpPost("check-codes")]
        public async Task<IActionResult> checkCodes([FromBody] List<ProductCodeCheck> codes)
        {
            try
            {
                // Lấy danh sách các mã cần kiểm tra
                var codeRTCs = codes.Select(x => x.CodeRTC).ToList();
                var codeNCCs = codes.Select(x => x.CodeNCC).ToList();

                // Kiểm tra trong database
                var existingProducts = _officesupplyRepo.GetAll()
                    .Where(x => codeRTCs.Contains(x.CodeRTC) && codeNCCs.Contains(x.CodeNCC))
                    .Select(x => new
                    {
                        x.ID, // Thêm ID vào đây
                        x.CodeRTC,
                        x.CodeNCC,
                        x.NameRTC,
                        x.NameNCC
                    })
                    .ToList();

                return Ok(new
                {
                    data = new
                    {
                        existingProducts
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        //cap nhat and them
        [HttpPost("save-data")]
        public async Task<IActionResult> saveDataOfficeSupply([FromBody] OfficeSupply officesupply)
        {
            try
            {
                if (officesupply.ID <= 0)
                {
                    await _officesupplyRepo.CreateAsync(officesupply);
                }
                else
                {
                    _officesupplyRepo.Update(officesupply);
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

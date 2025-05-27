using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeSuppliesController : ControllerBase
    {

        private readonly RTCContext _context;
        RTCContext db = new RTCContext();
        OfficeSuplyRepo off = new OfficeSuplyRepo();

        OfficeSupplyUnitRepo osurepo = new OfficeSupplyUnitRepo();
        public OfficeSuppliesController(RTCContext context)
        {
            _context = context;
        }

        [HttpGet("getdataofficesupplies")]
        public IActionResult GetOfficeSupplies([FromQuery] string keyword = "")
        {
            List<OficeSuppliesDTO> result = SQLHelper<OficeSuppliesDTO>.ProcedureToList(
                "spGetOfficeSupply",
                new string[] { "@KeyWord" },
               new object[] { (object)(keyword ?? "") }  // đảm bảo không null
            );
            var data = result.Where(x => x.IsDeleted == false).ToList();
            return Ok(new
            {
                status = 0,
                data = data
            });
        }

        [HttpGet("getdataofficesupplyunit")]
        public IActionResult GetDataOfficeSupplyUnini()
        {
            List<OfficeSupplyUnit> result = SQLHelper<OfficeSupplyUnit>.FindAll();
            var data = result.Where(x => x.IsDeleted == false).ToList();
            return Ok(
                new { status = 0, data = data }
            );
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
            if (ids == null || ids.Count == 0)
                return BadRequest("Danh sách ID không hợp lệ.");

            foreach (var id in ids)
            {
                var item = await db.OfficeSupplies.FindAsync(id);
                if (item != null)
                {
                    item.IsDeleted = true; // Gán trường IsDeleted thành true
                    /* await off.UpdateAsync(item);*/
                    db.OfficeSupplies.Update(item);/* // Cập nhật lại mục*/
                }
            }

            await db.SaveChangesAsync();
            return Ok(new { message = "Đã xóa thành công." });
        }

        [HttpGet]
        [Route("next-codeRTC")]
        public async Task<IActionResult> GetNextCodeRTC()
        {
            try
            {
                var allCodes = await db.OfficeSupplies
                           .Where(x => x.CodeRTC.StartsWith("VPP"))
                           .Select(x => x.CodeRTC)
                           .ToListAsync();
                int maxNumber = 0;
                foreach (var code in allCodes)
                {
                    var numberPart = code.Substring(3);
                    if (int.TryParse(numberPart, out int num))
                    {
                        if (num > maxNumber)
                            maxNumber = num;
                    }
                }
                int nextNumber = maxNumber + 1;
                var nextCodeRTC = "VPP" + nextNumber;
                return Ok(nextCodeRTC);
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

        //danh sách tính
        [HttpPost("savedatofficesupplyunit")]
        public async Task<IActionResult> SaveDST([FromBody] OfficeSupplyUnit dst)
        {
            try
            {
                if (dst.ID <= 0)
                {
                    dst.IsDeleted = false;
                    await osurepo.CreateAsync(dst);
                }
                else await osurepo.UpdateAsync(dst);

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
        [HttpGet("getbyidofficesupplyunit")]
        public IActionResult GetByIDOfficeSupplyUnit(int id)
        {
            try
            {
                OfficeSupplyUnit dst = osurepo.GetByID(id);
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

        [HttpPost("deleteofficesupplyunit")]
        public async Task<IActionResult> DeleteOfficeSupplyUnit([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest("Danh sách ID không hợp lệ.");

            foreach (var id in ids)
            {
                var item = await db.OfficeSupplyUnits.FindAsync(id);
                if (item != null)
                {
                    item.IsDeleted = true; // Gán trường IsDeleted thành true
                    db.OfficeSupplyUnits.Update(item); // Cập nhật lại mục
                }
            }
            await db.SaveChangesAsync();
            return Ok(new { message = "Đã xóa thành công." });
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitCountController : ControllerBase
    {
        UnitCountRepo _unitcountRepo = new UnitCountRepo();


        [HttpGet("")]
        public IActionResult getUnitCount()
        {

            try
            {
                List<UnitCount> dataUnit = _unitcountRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = dataUnit,
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

        [HttpPost("save-data")]
        public async Task<IActionResult> saveUnitCount([FromBody] List<UnitCountDTO> dtos)
        {
            try
            {
                //TN.Binh update 19/10/25
                foreach (var dto in dtos)
                {
                    if (!CheckUnitCode(dto))
                    {
                        return Ok(new { status = 0, message = $"Mã đơn vị [{dto.UnitCode}] đã tồn tại!" });
                    }
                }
                foreach (var dto in dtos)
                {
                    if (dto.ID <= 0) await _unitcountRepo.CreateAsync(dto);
                    else await _unitcountRepo.UpdateAsync(dto);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Thêm đơn vị tính thành công!",

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

        //TN.Binh update 28/10/25
        #region check trùng mã sản phẩm khi thêm, sửa đơn vị
        private bool CheckUnitCode(UnitCountDTO dto)
        {
            bool check = true;
            var exists = _unitcountRepo.GetAll()
                .Where(x => x.UnitCode == dto.UnitCode
                            && x.ID != dto.ID).ToList();
            if (exists.Count > 0) check = false;
            return check;
        }
        #endregion
        //end update
    }
}

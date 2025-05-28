using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static RERPAPI.Model.DTO.ModulaLocationDTO;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulaLocationController : ControllerBase
    {
        ModulaLocationRepo locationRepo = new ModulaLocationRepo();
        ModulaLocationDetailRepo detailRepo = new ModulaLocationDetailRepo();

        BillImportDetailSerialNumberRepo importDetailSerialNumberRepo = new BillImportDetailSerialNumberRepo();
        BillExportDetailSerialNumberRepo exportDetailSerialNumberRepo = new BillExportDetailSerialNumberRepo();

        BillImportDetailSerialNumberModulaLocationRepo serialNumberImportModulaRepo = new BillImportDetailSerialNumberModulaLocationRepo();
        BillExportDetailSerialNumberModulaLocationRepo serialNumberExportModulaRepo = new BillExportDetailSerialNumberModulaLocationRepo();

        [HttpGet("getlocation")]
        public IActionResult GetLocation()
        {
            try
            {
                List<ModulaLocation> listLocations = locationRepo.GetAll().Where(x=>x.IsDeleted == false).ToList();
                List<List<dynamic>> locationdetails = SQLHelper<object>.ProcedureToDynamicLists("spGetModulaLocationDetail", new string[] { }, new object[] { });
                var details = SQLHelper<object>.GetListData(locationdetails, 0);

                List<ModulaLocationDTO> locations = new List<ModulaLocationDTO>();
                foreach (var item in listLocations)
                {
                    ModulaLocationDTO location = new ModulaLocationDTO();
                    location.ID = item.ID;
                    location.STT = item.STT;
                    location.Code = item.Code;
                    location.Name = item.Name;
                    location.Width = item.Width;
                    location.Height = item.Height;
                    location.AxisX = item.AxisX;
                    location.AxisY = item.AxisY;
                    location.IsDeleted = item.IsDeleted;
                    location.ModulaLocationDetails = details.Where(x => x.ModulaLocationID == item.ID).ToList();

                    locations.Add(location);
                }

                return Ok(new
                {
                    status = 1,
                    data = new { locations }
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
        /// <summary>
        /// Get danh sách sản phẩm nhập - xuất
        /// </summary>
        /// <param name="billtype">1: Phiếu nhập; 2: Phiếu xuất</param>
        /// <param name="billcode">mã phiếu</param>
        /// <returns></returns>
        [HttpGet("getproducts")]
        public IActionResult GetProducts(int billtype, string billcode)
        {
            try
            {

                List<List<dynamic>> data = SQLHelper<object>.ProcedureToDynamicLists("spGetProductImportExport",
                                                                new string[] { "@BillType", "@BillCode" },
                                                                new object[] { billtype, billcode });


                List<dynamic> importDetails = new List<dynamic>();
                List<dynamic> exportDetails = new List<dynamic>();

                if (billtype == 1) importDetails = SQLHelper<object>.GetListData(data, 0);
                else if (billtype == 2) exportDetails = SQLHelper<object>.GetListData(data, 0);

                return Ok(new
                {
                    status = 1,
                    data = new { importDetails, exportDetails }
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


        [HttpGet("getlocationdetail")]
        public IActionResult GetLocationByID(int id)
        {
            try
            {

                List<List<dynamic>> locations = SQLHelper<object>.ProcedureToDynamicLists("spGetModulaLocationDetailByID",
                                                                new string[] { "@ID" },
                                                                new object[] { id });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(locations, 0)
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

        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] List<ModulaLocationDTO.SerialNumberModulaLocation> serialNumberModulaLocations)
        {
            try
            {
                for (int i = 0; i < serialNumberModulaLocations.Count; i++)
                {
                    var item = serialNumberModulaLocations[i];
                    if (item.BillImportDetailID > 0) //Nếu là nhập kho
                    {
                        if (string.IsNullOrWhiteSpace(item.SerialNumber)) continue;

                        BillImportDetailSerialNumber serialNumber = importDetailSerialNumberRepo.GetAll().FirstOrDefault(x => x.SerialNumber == item.SerialNumber) ?? new BillImportDetailSerialNumber();

                        if (serialNumber.ID <= 0)
                        {
                            serialNumber.STT = i + 1;
                            serialNumber.SerialNumber = item.SerialNumber.Trim();
                            serialNumber.BillImportDetailID = item.BillImportDetailID;
                            serialNumber.SerialNumberRTC = "";
                            serialNumber.CreatedBy = serialNumber.UpdatedBy = item.CreatedBy;
                            serialNumber.CreatedDate = serialNumber.UpdatedDate = DateTime.Now;

                            importDetailSerialNumberRepo.Create(serialNumber);
                        }

                        BillImportDetailSerialNumberModulaLocation import = new BillImportDetailSerialNumberModulaLocation() 
                        {
                            BillImportDetailSerialNumberID = serialNumber.ID,
                            ModulaLocationDetailID = item.ModulaLocationDetailID,
                            Quantity = item.Quantity,
                            IsDeleted = false,
                            CreatedBy = item.CreatedBy,
                            UpdatedBy = item.CreatedBy,

                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                        };

                        await serialNumberImportModulaRepo.CreateAsync(import);
                    }
                    else
                    {
                        BillExportDetailSerialNumber serialNumber = exportDetailSerialNumberRepo.GetAll().FirstOrDefault(x => x.SerialNumber == item.SerialNumber) ?? new BillExportDetailSerialNumber();
                        if (serialNumber.ID <= 0)
                        {
                            serialNumber.STT = i + 1;
                            serialNumber.SerialNumber = item.SerialNumber.Trim();
                            serialNumber.BillExportDetailID = item.BillExportDetailID;
                            serialNumber.SerialNumberRTC = "";
                            serialNumber.CreatedBy = serialNumber.UpdatedBy = item.CreatedBy;
                            serialNumber.CreatedDate = serialNumber.UpdatedDate = DateTime.Now;

                            exportDetailSerialNumberRepo.Create(serialNumber);
                        }

                        BillExportDetailSerialNumberModulaLocation export = new BillExportDetailSerialNumberModulaLocation()
                        {
                            BillExportDetailSerialNumberID = serialNumber.ID,
                            ModulaLocationDetailID = item.ModulaLocationDetailID,
                            Quantity = item.Quantity,
                            IsDeleted = false,
                            CreatedBy = item.CreatedBy,
                            UpdatedBy = item.CreatedBy,

                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                        };

                        await serialNumberExportModulaRepo.CreateAsync(export);
                    }
                }


                return Ok(new
                {
                    status = 1,
                    message = "Cập nhật thành công!",
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


        [HttpPost("savelocation")]
        public async Task<IActionResult> SaveLocation([FromBody] ModulaLocationDTO modulaLocation)
        {
            try
            {

                if (modulaLocation.ID <= 0)
                {
                    modulaLocation.CreatedDate = modulaLocation.UpdatedDate = DateTime.Now;
                    await locationRepo.CreateAsync(modulaLocation);
                }
                else
                {
                    modulaLocation.UpdatedDate = DateTime.Now;
                    await locationRepo.UpdateAsync(modulaLocation);
                }

                modulaLocation.LocationDetails.ForEach(x =>
                {
                    x.ModulaLocationID = modulaLocation.ID;
                    x.AxisX = 0;
                    x.AxisY = 1;
                });

                await detailRepo.CreateRangeAsync(modulaLocation.LocationDetails);

                return Ok(new
                {
                    status = 1,
                    message = "Cập nhật thành công!",
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

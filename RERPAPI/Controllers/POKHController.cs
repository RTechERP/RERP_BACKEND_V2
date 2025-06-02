using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Data;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class POKHController : ControllerBase
    {

        private readonly string _uploadPath;
        POKHRepo _pokhRepo = new POKHRepo();
        POKHDetailRepo _pokhDetailRepo = new POKHDetailRepo();
        POKHDetailMoneyRepo _pokhDetailMoneyRepo = new POKHDetailMoneyRepo();
        POKHFilesRepo _pokhFilesRepo = new POKHFilesRepo();


        public POKHController(IWebHostEnvironment environment)
        {
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "POKH");
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        [HttpGet("GetPOKH")]
        public IActionResult GetPOKH(string? filterText, int pageNumber, int pageSize, int customerId, int userId, int POType, int status, int group, DateTime startDate, DateTime endDate, int warehouseId, int employeeTeamSaleId)
        {
            try
            {
                List<List<dynamic>> POKHs = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetPOKH",
                    new string[] { "@FilterText", "@PageNumber", "@PageSize", "@CustomerID", "@UserID", "@POType", "@Status", "@Group", "@StartDate", "@EndDate", "@WarehouseID", "@EmployeeTeamSaleID" },
                    new object[] { filterText, pageNumber, pageSize, customerId, userId, POType, status, group, startDate, endDate, warehouseId, employeeTeamSaleId });
                return Ok(new
                {
                    status = 1,
                    data = POKHs[0]
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
        [HttpGet("GetEmployeeManager")]
        public IActionResult LoadEmployeeManager(int group, int userId, int teamId)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetEmployeeManager",
                    new string[] { "@group", "@UserID", "@TeamID" },
                    new object[] { group, userId, teamId });
                return Ok(new
                {
                    status = 1,
                    data = list
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString(),
                });
            }
        }
        [HttpGet("LoadPOKHProduct")]
        public IActionResult getPOKHProduct(int id, int idDetail)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetPOKHDetail", new string[] { "@ID", "@IDDetail" }, new object[] { id, idDetail });
                List<dynamic> listPOKHProduct = list[0];
                return Ok(new
                {
                    status = 1,
                    data = listPOKHProduct
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
        [HttpGet("LoadPOKHFiles")]
        public IActionResult getPOKHFiles(int id)
        {
            try
            {
                List<POKHFile> file = SQLHelper<POKHFile>.FindByAttribute("POKHID", id);
                List<POKHFile> list = file.Where(x => x.IsDeleted == false).ToList();
                return Ok(new
                {
                    status = 1,
                    data = list,
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
        [HttpGet("loadProject")]
        public IActionResult LoadProject()
        {
            try
            {
                string sqlQuery = "SELECT ID,ProjectCode,UserID,ContactID,CustomerID,ProjectName,PO From Project";

                using (SqlConnection conn = new SqlConnection(Config.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spSearchAllForTrans", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@sqlCommand", sqlQuery));
                        cmd.CommandTimeout = 6000;

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var result = new List<ProjectDTO>();
                            while (reader.Read())
                            {
                                result.Add(new ProjectDTO
                                {
                                    ID = reader.GetInt32(0),
                                    ProjectCode = reader.GetString(1),
                                    UserID = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                                    ContactID = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                                    CustomerID = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                                    ProjectName = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    PO = reader.IsDBNull(6) ? null : reader.GetString(6)
                                });
                            }
                            return Ok(new
                            {
                                status = 1,
                                data = result
                            });
                        }

                    }
                }
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
        [HttpGet("GetTypePO")]
        public IActionResult LoadTypePO()
        {

            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetMainIndex", new string[] { "@Type" }, new object[] { 1 });
                List<dynamic> listTypePO = list[0];

                return Ok(new
                {
                    status = 1,
                    data = listTypePO
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

        [HttpGet("GetCurrency")]
        public IActionResult LoadCurrency()
        {
            try
            {
                List<Currency> listCurrency = SQLHelper<Currency>.FindAll();
                return Ok(new
                {
                    status = 1,
                    data = listCurrency
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

        [HttpGet("GetPOKHByID")]
        public IActionResult GetByID(int id)
        {
            try
            {
                POKH pokh = _pokhRepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = pokh
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
        [HttpPost("Handle")]
        public async Task<IActionResult> Handle([FromBody] POKHDTO dto)
        {
            try
            {
                if (dto.POKH.ID <= 0)
                {
                    await _pokhRepo.CreateAsync(dto.POKH);
                }
                else
                {
                    _pokhRepo.UpdateFieldsByID(dto.POKH.ID, dto.POKH);
                }
                var parentIdMapping = new Dictionary<int, int>();
                if (dto.pOKHDetails.Count > 0)
                {
                    foreach (var item in dto.pOKHDetails)
                    {
                        int idOld = item.ID;
                        int parentId = 0;

                        if (item.ParentID.HasValue && parentIdMapping.ContainsKey(item.ParentID.Value))
                        {
                            parentId = parentIdMapping[item.ParentID.Value];
                        }

                        POKHDetail model = idOld > 0 ? _pokhDetailRepo.GetByID(idOld) : new POKHDetail();
                        // gán id và gán parent id
                        model.POKHID = dto.POKH.ID;
                        model.ParentID = parentId;
                        //
                        model.ProductID = item.ProductID;
                        model.STT = item.STT;
                        model.KHID = item.KHID;
                        model.GuestCode = item.GuestCode;
                        model.Qty = item.Qty;
                        model.FilmSize = item.FilmSize;
                        model.UnitPrice = item.UnitPrice;
                        model.IntoMoney = item.IntoMoney;
                        model.VAT = item.VAT;
                        model.Spec = item.Spec;
                        model.NetUnitPrice = item.NetUnitPrice;
                        model.TotalPriceIncludeVAT = item.TotalPriceIncludeVAT;
                        model.UserReceiver = item.UserReceiver;
                        model.DeliveryRequestedDate = item.DeliveryRequestedDate;
                        model.EstimatedPay = item.EstimatedPay;
                        model.BillDate = item.BillDate;
                        model.BillNumber = item.BillNumber;
                        model.Debt = item.Debt;
                        model.PayDate = item.PayDate;
                        model.GroupPO = item.GroupPO;
                        model.ActualDeliveryDate = item.ActualDeliveryDate;
                        model.RecivedMoneyDate = item.RecivedMoneyDate;
                        model.Note = item.Note;
                        model.IsDeleted = item.IsDeleted;


                        if (idOld > 0)
                        {
                            _pokhDetailRepo.UpdateFieldsByID(idOld, model);
                        }
                        else
                        {
                            await _pokhDetailRepo.CreateAsync(model);
                        }
                        parentIdMapping.Add(item.ID, model.ID);
                    }
                }
                if (dto.pOKHDetailsMoney != null && dto.pOKHDetailsMoney.Count > 0)
                {
                    foreach (var item in dto.pOKHDetailsMoney)
                    {
                        int idOld = item.ID;
                        POKHDetailMoney detailMoney = idOld > 0 ? _pokhDetailMoneyRepo.GetByID(idOld) : new POKHDetailMoney();
                        detailMoney.POKHID = dto.POKH.ID;
                        detailMoney.POKHDetailID = 0;
                        detailMoney.PercentUser = item.PercentUser / 100;
                        detailMoney.UserID = item.UserID;
                        detailMoney.MoneyUser = item.MoneyUser;
                        detailMoney.RowHandle = item.RowHandle;
                        detailMoney.STT = item.STT;
                        detailMoney.ReceiveMoney = item.ReceiveMoney;
                        detailMoney.Month = dto.POKH.Month;
                        detailMoney.Year = dto.POKH.Year;
                        detailMoney.CreatedDate = DateTime.Now;
                        detailMoney.IsDeleted = item.IsDeleted;

                        if (idOld > 0)
                        {
                            _pokhDetailMoneyRepo.UpdateFieldsByID(idOld, detailMoney);
                        }
                        else
                        {
                            await _pokhDetailMoneyRepo.CreateAsync(detailMoney);
                        }
                    }
                }

                return Ok(new
                {
                    status = 1,
                    message = "Success",
                    data = new { id = dto.POKH.ID }
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
        [HttpDelete("DeletePOKH")]
        public async Task<IActionResult> DeletePOKH(int id)
        {
            try
            {
                var item = _pokhRepo.GetByID(id);
                if (item == null)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "POKH not found"
                    });
                }
                await _pokhRepo.DeleteAsync(id);
                return Ok(new
                {
                    status = 1,
                    message = "Deleted successfully"
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
        [HttpPost("DeleteRangePOKH")]
        public async Task<IActionResult> DeleteRangePOKH([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "No IDs provided"
                    });
                }
                var entities = _pokhRepo.GetAll().Where(x => ids.Contains(x.ID)).ToList();
                await _pokhRepo.DeleteRangeAsync(entities);
                return Ok(new
                {
                    status = 1,
                    message = "Deleted selected records successfully"
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
        [HttpGet("GeneratePOCode")]
        public IActionResult GeneratePOCode(string customer, bool isCopy, int warehouseID, int pokhID)
        {
            try
            {
                if (pokhID > 0 && !isCopy && warehouseID != 2)
                    return BadRequest("Không cần tạo PO Code mới.");

                string maPO = DateTime.Now.ToString("ddMMyyy");
                var latestCode = _pokhRepo.FindLastestCode(customer);

                string[] arr = latestCode?.Split('.') ?? new string[0];
                uint nextNumber;

                if (arr.Length < 2 || !uint.TryParse(arr[1], out nextNumber))
                {
                    nextNumber = 1;
                }
                else
                {
                    nextNumber++;
                }

                string newPOCode = $"{customer}_{maPO}.{nextNumber}";
                return Ok(new
                {
                    status = 1,
                    data = newPOCode
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
        [HttpGet("LoadProduct")]
        public IActionResult loadProduct()
        {
            try
            {
                var listGroup = SQLHelper<ProductGroup>.FindAll().Select(x => x.ID).ToList();
                var idGroup = string.Join(",", listGroup);
                List<List<dynamic>> listProduct = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetProductSale", new string[] { "@IDgroup" }, new object[] { idGroup });
                List<dynamic> list = listProduct[0];
                return Ok(new
                {
                    status = 1,
                    data = list,
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
        [HttpGet("LoadDetailUser")]
        public IActionResult loadDetailUser(int id, int idDetail)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetPOKHDetail_New", new string[] { "@ID", "@IDDetail" }, new object[] { id, idDetail });
                return Ok(new
                {
                    status = 1,
                    data = list,
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
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(int poKHID, [FromForm] List<IFormFile> files)
        {
            try
            {
                // Lấy thông tin POKH
                var po = _pokhRepo.GetByID(poKHID);
                if (po == null)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "POKH not found"
                    });
                }

                // Tạo thư mục local cho file
                string pathPattern = $"NB{po.ID}";
                string pathUpload = Path.Combine(_uploadPath, pathPattern);

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(pathUpload))
                {
                    Directory.CreateDirectory(pathUpload);
                }

                var processedFile = new List<POKHFile>();


                // Lưu từng file vào thư mục local
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string fileName = _pokhRepo.GenerateUniqueFileName(file.FileName);
                        string filePath = Path.Combine(pathUpload, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var filePO = new POKHFile
                        {
                            POKHID = po.ID,
                            FileName = fileName,
                            OriginPath = file.FileName,
                            ServerPath = pathUpload,
                            IsDeleted = false,
                            CreatedBy = User.Identity?.Name ?? "System",
                            CreatedDate = DateTime.Now,
                            UpdatedBy = User.Identity?.Name ?? "System",
                            UpdatedDate = DateTime.Now
                        };

                        await _pokhFilesRepo.CreateAsync(filePO);
                        processedFile.Add(filePO);
                    }
                }

                return Ok(new
                {
                    status = 1,
                    message = $"{processedFile.Count} tệp đã được tải lên thành công",
                    data = processedFile
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
        [HttpPost("DeleteFile")]
        public  IActionResult DeleteFile([FromBody] List<int> fileIds)
        {
            if (fileIds == null || !fileIds.Any())
                return BadRequest(new { success = false, message = "Danh sách file ID không được trống" });

            try
            {
                var results = new List<object>();

                foreach (var fileId in fileIds)
                {
                    var file = _pokhFilesRepo.GetByID(fileId);

                    // Cập nhật database
                    file.IsDeleted = true;
                    //file.UpdatedBy = User.Identity?.Name ?? "System";
                    //file.UpdatedDate = DateTime.UtcNow;
                    _pokhFilesRepo.UpdateFieldsByID(file.ID, file);

                    // Xóa file vật lý
                    var physicalPath = Path.Combine(file.ServerPath, file.FileName);
                    if (System.IO.File.Exists(physicalPath))
                        System.IO.File.Delete(physicalPath);

                    results.Add(new { fileId, success = true, message = "Xóa thành công" });
                }

                return Ok(new { success = true, results });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}

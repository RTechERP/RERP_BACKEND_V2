using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Data;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierSaleController : ControllerBase
    {
        private readonly SupplierSaleRepo _supplierSaleRepo;
        private readonly SupplierSaleContactRepo _supplierSaleContactRepo;
        private readonly EmployeeRepo _employeeRepo;

        public SupplierSaleController()
        {
            _supplierSaleRepo = new SupplierSaleRepo();
            _supplierSaleContactRepo = new SupplierSaleContactRepo();
            _employeeRepo = new EmployeeRepo();
        }

        /// <summary>
        /// Lấy danh sách nhà cung cấp với phân trang và tìm kiếm
        /// </summary>
        [HttpGet("get-data")]
        public IActionResult GetSupplierSaleData(string keywords = "", int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spFindSupplierNCC",
                    new string[] { "@Find", "@PageNumber", "@PageSize" },
                    new object[] { keywords, pageNumber, pageSize });

                var data = SQLHelper<dynamic>.GetListData(list, 0);
                var totalPage = list.Count > 1 ? SQLHelper<dynamic>.GetListData(list, 1).FirstOrDefault()?.TotalPage : 1;

                return Ok(ApiResponseFactory.Success(new { data = data, totalPage = totalPage }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lấy tất cả nhà cung cấp
        /// </summary>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var suppliers = _supplierSaleRepo.GetAll(x => x.IsDeleted != true)
                    .OrderBy(x => x.ID).ToList();
                return Ok(ApiResponseFactory.Success(suppliers, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lấy nhà cung cấp theo ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var supplier = _supplierSaleRepo.GetByID(id);
                if (supplier == null)
                {
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy nhà cung cấp"));
                }

                return Ok(ApiResponseFactory.Success(supplier, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lấy danh sách liên hệ của nhà cung cấp
        /// </summary>
        [HttpGet("{id}/contacts")]
        public IActionResult GetSupplierContacts(int id)
        {
            try
            {
                var contacts = _supplierSaleContactRepo.GetAll(x => x.SupplierID == id).ToList();
                return Ok(ApiResponseFactory.Success(contacts, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lấy danh sách nhân viên
        /// </summary>
        [HttpGet("get-employees")]
        public IActionResult GetEmployees()
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetEmployee",
                    new string[] { "@Status" },
                    new object[] { 0 });

                var employees = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(employees, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Tạo mới nhà cung cấp
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SupplierSale supplier)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(supplier.CodeNCC))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Mã NCC"));
                }

                if (string.IsNullOrEmpty(supplier.NameNCC))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên NCC"));
                }

                if (string.IsNullOrEmpty(supplier.AddressNCC))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Địa chỉ"));
                }

                // Check if CodeNCC already exists
                var existingSupplier = _supplierSaleRepo.GetAll(x => x.CodeNCC == supplier.CodeNCC && x.IsDeleted != true).FirstOrDefault();
                if (existingSupplier != null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Mã NCC [{supplier.CodeNCC}] đã tồn tại"));
                }

                supplier.CreatedDate = DateTime.Now;
                supplier.NgayUpdate = DateTime.Now;
                supplier.IsDeleted = false;

                var result = await _supplierSaleRepo.CreateAsync(supplier);
                return Ok(ApiResponseFactory.Success(result, "Tạo nhà cung cấp thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Cập nhật nhà cung cấp
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] SupplierSale supplier)
        {
            try
            {
                var existingSupplier = _supplierSaleRepo.GetByID(id);
                if (existingSupplier == null)
                {
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy nhà cung cấp"));
                }

                // Validate required fields
                if (string.IsNullOrEmpty(supplier.CodeNCC))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Mã NCC"));
                }

                if (string.IsNullOrEmpty(supplier.NameNCC))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên NCC"));
                }

                if (string.IsNullOrEmpty(supplier.AddressNCC))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Địa chỉ"));
                }

                // Check if CodeNCC already exists (exclude current record)
                var duplicateSupplier = _supplierSaleRepo.GetAll(x => x.CodeNCC == supplier.CodeNCC && x.ID != id && x.IsDeleted != true).FirstOrDefault();
                if (duplicateSupplier != null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Mã NCC [{supplier.CodeNCC}] đã tồn tại"));
                }

                // Update fields
                existingSupplier.CodeNCC = supplier.CodeNCC;
                existingSupplier.NameNCC = supplier.NameNCC;
                existingSupplier.TenTiengAnh = supplier.TenTiengAnh;
                existingSupplier.AddressNCC = supplier.AddressNCC;
                existingSupplier.PhoneNCC = supplier.PhoneNCC;
                existingSupplier.OrdererNCC = supplier.OrdererNCC;
                existingSupplier.Debt = supplier.Debt;
                existingSupplier.NVPhuTrach = supplier.NVPhuTrach;
                existingSupplier.LoaiHangHoa = supplier.LoaiHangHoa;
                existingSupplier.Brand = supplier.Brand;
                existingSupplier.MaNhom = supplier.MaNhom;
                existingSupplier.MaSoThue = supplier.MaSoThue;
                existingSupplier.Website = supplier.Website;
                existingSupplier.SoTK = supplier.SoTK;
                existingSupplier.NganHang = supplier.NganHang;
                existingSupplier.Note = supplier.Note;
                existingSupplier.Company = supplier.Company;
                existingSupplier.ShortNameSupplier = supplier.ShortNameSupplier;
                existingSupplier.EmployeeID = supplier.EmployeeID;
                existingSupplier.RulePayID = supplier.RulePayID;
                existingSupplier.IsDebt = supplier.IsDebt;
                existingSupplier.FedexAccount = supplier.FedexAccount;
                existingSupplier.OriginItem = supplier.OriginItem;
                existingSupplier.BankCharge = supplier.BankCharge;
                existingSupplier.AddressDelivery = supplier.AddressDelivery;
                existingSupplier.Description = supplier.Description;
                existingSupplier.RuleIncoterm = supplier.RuleIncoterm;
                existingSupplier.UpdatedDate = DateTime.Now;
                existingSupplier.NgayUpdate = DateTime.Now;

                _supplierSaleRepo.Update(existingSupplier);
                return Ok(ApiResponseFactory.Success(existingSupplier, "Cập nhật nhà cung cấp thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Xóa nhà cung cấp (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var supplier = _supplierSaleRepo.GetByID(id);
                if (supplier == null)
                {
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy nhà cung cấp"));
                }

                supplier.IsDeleted = true;
                supplier.UpdatedDate = DateTime.Now;
                _supplierSaleRepo.Update(supplier);

                return Ok(ApiResponseFactory.Success(null, "Xóa nhà cung cấp thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lưu thông tin liên hệ của nhà cung cấp
        /// </summary>
        [HttpPost("{id}/contacts")]
        public async Task<IActionResult> SaveContacts(int id, [FromBody] List<SupplierSaleContact> contacts)
        {
            try
            {
                var supplier = _supplierSaleRepo.GetByID(id);
                if (supplier == null)
                {
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy nhà cung cấp"));
                }

                foreach (var contact in contacts)
                {
                    contact.SupplierID = id;

                    if (contact.ID > 0)
                    {
                        // Update existing contact
                        var existingContact = _supplierSaleContactRepo.GetByID(contact.ID);
                        if (existingContact != null)
                        {
                            existingContact.SupplierName = contact.SupplierName;
                            existingContact.SupplierPhone = contact.SupplierPhone;
                            existingContact.SupplierEmail = contact.SupplierEmail;
                            existingContact.Describe = contact.Describe;
                            existingContact.UpdatedDate = DateTime.Now;
                            await _supplierSaleContactRepo.UpdateAsync(existingContact);
                        }
                    }
                    else
                    {
                        // Create new contact
                        contact.CreatedDate = DateTime.Now;
                        await _supplierSaleContactRepo.CreateAsync(contact);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thông tin liên hệ thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
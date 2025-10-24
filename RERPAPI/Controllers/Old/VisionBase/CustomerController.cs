using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using Newtonsoft.Json;
using System.IO;
using RERPAPI.Model.DTO;
using System.Data;

namespace RERPAPI.Controllers.Old.VisionBase
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        AddressStockRepo _addressStockRepo = new AddressStockRepo();
        CustomerContactRepo _customerContactRepo = new CustomerContactRepo();
        CustomerEmployeeRepo _customerEmployeeRepo = new CustomerEmployeeRepo();
        EmployeeRepo _employeeRepo = new EmployeeRepo();
        CustomerSpecializationRepo _customerSpecializationRepo = new CustomerSpecializationRepo();
        BusinessFieldRepo _businessFieldRepo = new BusinessFieldRepo();
        CustomerRepo _customerRepo = new CustomerRepo();
        BusinessFieldLinkRepo _businessFieldLinkRepo = new BusinessFieldLinkRepo();
        private static string json = System.IO.File.ReadAllText(@"jsonProvinces.txt");
        private static List<Provinces> provinces = JsonConvert.DeserializeObject<List<Provinces>>(json);
        public class Provinces
        {
            public int STT { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
        }

        [HttpGet("get-customer")]
        public IActionResult GetCustomer( int page, int size, int employeeId, int groupId, string? filterText = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetCustomer", 
                    new string[] { "@PageNumber", "@PageSize", "@FilterText", "@EmployeeID", "@GroupID" }, 
                    new object[] { page, size, filterText, employeeId, groupId });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                var data1 = SQLHelper<dynamic>.GetListData(list, 1);
                return Ok(ApiResponseFactory.Success(new {data, data1}, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-details")]
        public IActionResult GetContactAndAddress(int customerId)
        {
            try
            {
                var contact = _customerContactRepo.GetAll().Where(x => x.CustomerID == customerId).ToList();
                var address = _addressStockRepo.GetAll().Where(x => x.CustomerID == customerId).ToList();
                var customerEmployee = _customerEmployeeRepo.GetAll().Where(x => x.CustomerID == customerId).ToList();
                var employees = _employeeRepo.GetAll().ToList();
                var employee = (from ce in customerEmployee
                               join e in employees on ce.EmployeeID equals e.ID
                               select new
                               {
                                   e.FullName,
                                   ce.ID,
                                   ce.EmployeeID
                               }).ToList();
                return Ok(ApiResponseFactory.Success(new { contact, address, employee }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-customer-specialization")]
        public IActionResult GetCustomerSpecialization()
        {
            try
            {
                var data = _customerSpecializationRepo.GetAll().OrderBy(x=>x.STT).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-business-field")]
        public IActionResult GetBusinessField()
        {
            try
            {
                var data = _businessFieldRepo.GetAll().ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-provinces")]
        public IActionResult GetProvinces()
        {
            try
            {
                var data = provinces.OrderBy(x => x.STT).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-detail")]
        public IActionResult GetDetail(int id)
        {
            try
            {
                string provinceCode = "";
                string customerCode = "";
                Customer model = _customerRepo.GetByID(id);
                if (!string.IsNullOrEmpty(model.CustomerCode))
                {
                    if (model.CustomerCode.Trim().Length <= 4) customerCode = model.CustomerCode;
                    else customerCode = model.CustomerCode.Substring(4, model.CustomerCode.Length - 4);

                    if (model.CustomerCode.Trim().Length >= 3) provinceCode = model.CustomerCode.Substring(0, 3);
                }
                var business = _businessFieldLinkRepo.GetAll().FirstOrDefault(x => x.CustomerID == id);
                var addressStock = _addressStockRepo.GetAll().Where(x => x.CustomerID == id);
                var customerContact = _customerContactRepo.GetAll().Where(x => x.CustomerID == id);
                var customerEmployee = _customerEmployeeRepo.GetAll().Where(x => x.CustomerID == id).ToList();
                var employees = _employeeRepo.GetAll().ToList();
                var customerEmployeeWithName = (from ce in customerEmployee
                                                join e in employees on ce.EmployeeID equals e.ID
                                                select new
                                                {
                                                    ce.ID,
                                                    ce.EmployeeID,
                                                    e.FullName
                                                }).ToList();

                //if (warehouseID == 3)
                //{
                //    colCustomerPart.Caption = "Bộ phận";
                //    colContactPhone.Caption = "SĐT";
                //    colContactEmail.Caption = "Email";
                //}
                return Ok(ApiResponseFactory.Success(new { customerEmployeeWithName, model, provinceCode, customerCode, business, addressStock, customerContact}, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Save(InsertCustomerDTO dto )
        {
            try
            {
                //TN.Binh update xóa nhiều 24/10/25
                if(dto.isDeleted != null)
                {
                    foreach (var item in dto.isDeleted)
                    {
                        var cus = _customerRepo.GetAll(x => x.ID == item).FirstOrDefault();
                        cus.IsDeleted = true;
                        await _customerRepo.UpdateAsync(cus);
                    }
                }
                //end
                if(dto.Customer != null)
                {
                    Customer customer = dto.Customer.ID > 0 ? _customerRepo.GetByID(dto.Customer.ID) : new Customer();
                    customer.Province = dto.Customer.Province;
                    customer.CustomerCode = dto.Customer.CustomerCode;
                    customer.CustomerName = dto.Customer.CustomerName;
                    customer.CustomerShortName = dto.Customer.CustomerShortName;
                    customer.Address = dto.Customer.Address;
                    customer.CustomerType = dto.Customer.CustomerType;
                    customer.ProductDetails = dto.Customer.ProductDetails;
                    customer.NoteDelivery = dto.Customer.NoteDelivery;
                    customer.NoteVoucher = dto.Customer.NoteVoucher;
                    customer.CheckVoucher = dto.Customer.CheckVoucher;
                    customer.HardCopyVoucher = dto.Customer.HardCopyVoucher;
                    customer.CustomerSpecializationID = dto.Customer.CustomerSpecializationID;
                    customer.ClosingDateDebt = dto.Customer.ClosingDateDebt;
                    customer.Debt = dto.Customer.Debt;
                    customer.TaxCode = dto.Customer.TaxCode;
                    customer.IsDeleted = dto.Customer.IsDeleted;
                    customer.BigAccount = dto.Customer.BigAccount;
                    if (customer.ID > 0)
                    {
                        //customer.UpdatedBy =
                        customer.UpdatedDate = DateTime.Now;
                        await _customerRepo.UpdateAsync(customer);
                    }
                    else
                    {
                        //customer.CreatedBy = 
                        customer.CreatedDate = DateTime.Now;
                        await _customerRepo.CreateAsync(customer);
                    }
                    if (dto.CustomerContacts != null && dto.CustomerContacts.Count > 0)
                    {
                        foreach (var item in dto.CustomerContacts)
                        {
                            CustomerContact contact = item.ID > 0 ? _customerContactRepo.GetByID(item.ID) : new CustomerContact();
                            contact.CustomerID = customer.ID;
                            contact.ContactName = item.ContactName;
                            contact.ContactPhone = item.ContactPhone;
                            contact.ContactEmail = item.ContactEmail;
                            contact.CustomerPart = item.CustomerPart;
                            contact.CustomerPosition = item.CustomerPosition;
                            contact.CustomerTeam = item.CustomerTeam;
                            if (contact.ID > 0)
                            {
                                await _customerContactRepo.UpdateAsync(contact);
                            }
                            else
                            {
                                await _customerContactRepo.CreateAsync(contact);
                            }
                        }
                    }
                    if (dto.AddressStocks != null && dto.AddressStocks.Count > 0)
                    {
                        foreach (var item in dto.AddressStocks)
                        {
                            AddressStock address = item.ID > 0 ? _addressStockRepo.GetByID(item.ID) : new AddressStock();
                            address.CustomerID = customer.ID;
                            address.Address = item.Address;
                            if (address.ID > 0)
                            {
                                await _addressStockRepo.UpdateAsync(address);
                            }
                            else
                            {
                                await _addressStockRepo.CreateAsync(address);
                            }
                        }
                    }
                    if (dto.CustomerEmployees != null && dto.CustomerEmployees.Count > 0)
                    {
                        foreach (var item in dto.CustomerEmployees)
                        {
                            CustomerEmployee customerEmployee = item.ID > 0 ? _customerEmployeeRepo.GetByID(item.ID) : new CustomerEmployee();
                            customerEmployee.CustomerID = customer.ID;
                            customerEmployee.EmployeeID = item.EmployeeID;
                            if (customerEmployee.ID > 0)
                            {
                                await _customerEmployeeRepo.UpdateAsync(customerEmployee);
                            }
                            else
                            {
                                await _customerEmployeeRepo.CreateAsync(customerEmployee);
                            }
                        }
                    }
                    BusinessFieldLink business = _businessFieldLinkRepo.GetAll().FirstOrDefault(x => x.CustomerID == customer.ID);
                    if (business == null) business = new BusinessFieldLink();
                    business.CustomerID = customer.ID;
                    business.BusinessFieldID = dto.BusinessFieldID;
                    if (business.ID > 0)
                    {
                        await _businessFieldLinkRepo.UpdateAsync(business);
                    }
                    else
                    {
                        await _businessFieldLinkRepo.CreateAsync(business);
                    }
                }

                return Ok(ApiResponseFactory.Success("", "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

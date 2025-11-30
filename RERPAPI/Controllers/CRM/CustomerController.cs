using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.CRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        //CustomerRepo customerRepo = new CustomerRepo();
        //CustomerContactRepo customerContactRepo = new CustomerContactRepo();
        //AddressStockRepo addressStockRepo = new AddressStockRepo();
        //GroupSaleRepo groupSaleRepo = new GroupSaleRepo();
        //CustomerEmployeeRepo customerEmployeeRepo = new CustomerEmployeeRepo();
        //BusinessFieldLinkRepo businessFieldLinkRepo = new BusinessFieldLinkRepo();
        //EmployeeRepo employeeRepo = new EmployeeRepo();


        

        //[HttpGet("{id}")]
        //public IActionResult GetCustomerByID(int id)
        //{
        //    try
        //    {
        //        var customer = customerRepo.GetByID(id);
        //        //return Ok(new
        //        //{
        //        //    status = 1,
        //        //    data = customer
        //        //});

        //        return Ok(ApiResponseFactory.Success(customer, ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        //return BadRequest(new
        //        //{
        //        //    status = 0,
        //        //    message = ex.Message,
        //        //    error = ex.ToString()
        //        //});

        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}


        //[HttpGet()]
        //public IActionResult GetCustomer(int groupId, int employeeId, string? filterText, int pageNumber, int pageSize)
        //{
        //    try
        //    {
        //        filterText = string.IsNullOrWhiteSpace(filterText) ? " " : filterText;
        //        var customers = SQLHelper<object>.ProcedureToList("spGetCustomer",
        //            new string[] { "@PageNumber", "@PageSize", "@EmployeeID", "@GroupID", "@FilterText" },
        //            new object[] { pageNumber, pageSize, employeeId, groupId, filterText ?? "" });
        //        //return Ok(new
        //        //{
        //        //    status = 1,
        //        //    data = SQLHelper<object>.GetListData(customers, 0)
        //        //});

        //        return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(customers, 0), ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        //return BadRequest(new
        //        //{
        //        //    status = 0,
        //        //    message = ex.Message,
        //        //    error = ex.ToString()
        //        //});

        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}


        //[HttpGet("export-excel")]
        //public IActionResult GetCustomerToExcel(int groupId, int employeeId, string? filterText, int pageNumber, int pageSize)
        //{
        //    try
        //    {
        //        filterText = string.IsNullOrWhiteSpace(filterText) ? " " : filterText;
        //        var customers = SQLHelper<object>.ProcedureToList("spGetCustomer", new string[] { "@PageNumber", "@PageSize", "@EmployeeID", "@GroupID", "@FilterText" }, new object[] { pageNumber, pageSize, employeeId, groupId, filterText });
        //        //return Ok(new
        //        //{
        //        //    status = 1,
        //        //    data = SQLHelper<object>.GetListData(customers, 2)
        //        //});

        //        return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(customers, 2), ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        //return BadRequest(new
        //        //{
        //        //    status = 0,
        //        //    message = ex.Message,
        //        //    error = ex.ToString()
        //        //});

        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}


        //[HttpGet("{id}/customer-contact")]
        //public IActionResult GetCustomerContact(int id)
        //{
        //    try
        //    {

        //        var customerContacts = SQLHelper<object>.ProcedureToList("spGetCustomerContactByCustomerID", new string[] { "@CustomerID" }, new object[] { id });

        //        //return Ok(new
        //        //{
        //        //    status = 1,
        //        //    data = SQLHelper<object>.GetListData(customerContacts, 0)
        //        //});

        //        return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(customerContacts, 0), ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        //return BadRequest(new
        //        //{
        //        //    status = 0,
        //        //    message = ex.Message,
        //        //    error = ex.ToString()
        //        //});

        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        //[HttpGet("{id}/customer-employee")]
        //public IActionResult GetCustomerEmployeeByCustomerID(int id)
        //{

        //    try
        //    {
        //        string employeeName = "";
        //        List<CustomerEmployee> employeeSales = SQLHelper<CustomerEmployee>.FindByAttribute("CustomerID", id);
        //        //foreach (var employee in employeeSales)
        //        //{
        //        //    //employeeName = SQLHelper<Employee>.FindByID((long)employee.EmployeeID).FullName;
        //        //    employeeName = employeeRepo.GetByID((int)employee.EmployeeID);
        //        //}
        //        //return Ok(new
        //        //{
        //        //    status = 1,
        //        //    data = employeeSales.Select(e => new
        //        //    {
        //        //        e.ID,
        //        //        e.CustomerID,
        //        //        e.EmployeeID,
        //        //        EmployeeName = employeeRepo.GetByID((int)e.EmployeeID)?.FullName
        //        //    }).ToList()
        //        //});


        //        var data = employeeSales.Select(e => new
        //        {
        //            e.ID,
        //            e.CustomerID,
        //            e.EmployeeID,
        //            EmployeeName = employeeRepo.GetByID((int)e.EmployeeID)?.FullName
        //        }).ToList();
        //        return Ok(ApiResponseFactory.Success(data, ""));

        //    }
        //    catch (Exception ex)
        //    {
        //        //return BadRequest(new
        //        //{
        //        //    status = 0,
        //        //    message = ex.Message,
        //        //    error = ex.ToString()
        //        //});

        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}


        //[HttpPost]
        //public async Task<IActionResult> SaveCustomer([FromBody] CustomerDTO request)
        //{
        //    try
        //    {
        //        var customer = new Customer
        //        {
        //            ID = request.ID,
        //            Province = request.Province,
        //            CustomerCode = $"{request.CodeProvinces?.Trim()}-{request.CustomerCode?.Trim()}",
        //            CustomerShortName = request.CustomerShortName?.Trim().ToUpper(),
        //            CustomerName = request.CustomerName?.Trim(),
        //            Address = request.Address?.Trim(),
        //            CustomerType = request.CustomerType,
        //            ProductDetails = request.ProductDetails?.Trim(),
        //            NoteDelivery = request.NoteDelivery?.Trim(),
        //            NoteVoucher = request.NoteVoucher?.Trim(),
        //            CheckVoucher = request.CheckVoucher?.Trim(),
        //            HardCopyVoucher = request.HardCopyVoucher?.Trim(),
        //            CustomerSpecializationID = request.CustomerSpecializationID,
        //            ClosingDateDebt = request.ClosingDateDebt,
        //            Debt = request.Debt?.Trim(),
        //            TaxCode = request.TaxCode?.Trim(),
        //            IsDeleted = false,
        //            BigAccount = request.BigAccount
        //        };
        //        if (customer.ID <= 0)
        //        {
        //            await customerRepo.CreateAsync(customer);
        //        }
        //        else
        //        {
        //            await customerRepo.UpdateAsync(customer);
        //        }

        //        if (request.ID > 0)
        //        {
        //            var existingContacts = SQLHelper<CustomerContact>.FindByAttribute("CustomerID", customer.ID);
        //            var contactsToDelete = existingContacts.Where(c => !request.Contacts.Any(rc => rc.idConCus == c.ID)).ToList();
        //            foreach (var contact in contactsToDelete)
        //            {
        //                await customerContactRepo.DeleteAsync(contact.ID);
        //            }

        //            var existingAddresses = SQLHelper<AddressStock>.FindByAttribute("CustomerID", customer.ID);
        //            var addressesToDelete = existingAddresses.Where(a => !request.Addresses.Any(ra => ra.ID == a.ID)).ToList();
        //            foreach (var address in addressesToDelete)
        //            {
        //                await addressStockRepo.DeleteAsync(address.ID);
        //            }
        //            var existingSales = SQLHelper<CustomerEmployee>.FindByAttribute("CustomerID", customer.ID);
        //            var salesToDelete = existingSales.Where(s => !request.Sales.Any(rs => rs.ID == s.ID)).ToList();
        //            foreach (var sale in salesToDelete)
        //            {
        //                await customerEmployeeRepo.DeleteAsync(sale.ID);
        //            }
        //        }


        //        foreach (var contact in request.Contacts ?? new List<CustomerContactDTO>())
        //        {
        //            var customerContact = new CustomerContact
        //            {
        //                ID = contact.idConCus,
        //                CustomerID = customer.ID,
        //                ContactName = contact.ContactName?.Trim(),
        //                ContactPhone = contact.ContactPhone?.Trim(),
        //                ContactEmail = contact.ContactEmail?.Trim(),
        //                CustomerPart = contact.CustomerPart?.Trim(),
        //                CustomerPosition = contact.CustomerPosition?.Trim(),
        //                CustomerTeam = contact.CustomerTeam?.Trim()
        //            };
        //            if (customerContact.ID <= 0)
        //            {
        //                await customerContactRepo.CreateAsync(customerContact);
        //            }
        //            else
        //            {
        //                await customerContactRepo.UpdateAsync(customerContact);
        //            }
        //        }



        //        foreach (var address in request.Addresses ?? new List<CustomerAddressDTO>())
        //        {
        //            var customerAddress = new AddressStock
        //            {
        //                ID = address.ID,
        //                CustomerID = customer.ID,
        //                Address = address.Address?.Trim(),
        //            };

        //            if (customerAddress.ID <= 0)
        //            {
        //                await addressStockRepo.CreateAsync(customerAddress);
        //            }
        //            else
        //            {
        //                await addressStockRepo.UpdateAsync(customerAddress);
        //            }
        //        }

        //        foreach (var saleEmployee in request.Sales ?? new List<CustomerEmployeeDTO>())
        //        {
        //            var sale = new CustomerEmployee
        //            {
        //                ID = saleEmployee.ID,
        //                CustomerID = customer.ID,
        //                EmployeeID = saleEmployee.EmployeeID,
        //            };
        //            if (sale.ID <= 0)
        //            {
        //                await customerEmployeeRepo.CreateAsync(sale);
        //            }
        //            else
        //            {
        //                await customerEmployeeRepo.UpdateAsync(sale);
        //            }
        //        }

        //        var business = SQLHelper<BusinessFieldLink>.FindByAttribute("CustomerID", customer.ID).FirstOrDefault() ?? new BusinessFieldLink();

        //        business.CustomerID = customer.ID;
        //        business.BusinessFieldID = request.BusinessFieldID;

        //        if (business.ID <= 0)
        //        {
        //            await businessFieldLinkRepo.CreateAsync(business);
        //        }
        //        else
        //        {
        //            await businessFieldLinkRepo.UpdateAsync(business);
        //        }


        //        //return Ok(new
        //        //{
        //        //    status = 1,
        //        //    data = customer,
        //        //    message = "Customer saved successfully."
        //        //});

        //        return Ok(ApiResponseFactory.Success(customer, "Customer saved successfully."));
        //    }
        //    catch (Exception ex)
        //    {
        //        //return BadRequest(new
        //        //{
        //        //    status = 0,
        //        //    message = ex.Message,
        //        //    error = ex.ToString()
        //        //});

        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}


        //[HttpGet("{customerID}")]
        //public async Task<IActionResult> DeleteCustomer(int customerID)
        //{
        //    try
        //    {
        //        var customer = customerRepo.GetByID(customerID);
        //        customer.IsDeleted = true;
        //        await customerRepo.UpdateAsync(customer);
        //        //return Ok(new
        //        //{
        //        //    status = 1,
        //        //    message = "Customer deleted successfully."
        //        //});

        //        return Ok(ApiResponseFactory.Success(customer, "Customer deleted successfully."));
        //    }
        //    catch (Exception ex)
        //    {
        //        //return BadRequest(new
        //        //{
        //        //    status = 0,
        //        //    message = ex.Message,
        //        //    error = ex.ToString()
        //        //});

        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        //[HttpGet("{id}/address-stock")]
        //public IActionResult GetAddressStockByCustomerID(int id)
        //{
        //    try
        //    {
        //        var addressStock = SQLHelper<AddressStock>.FindByAttribute("CustomerID", id);
        //        //return Ok(new
        //        //{
        //        //    status = 1,
        //        //    data = addressStock
        //        //});

        //        return Ok(ApiResponseFactory.Success(addressStock, "Customer deleted successfully."));
        //    }
        //    catch (Exception ex)
        //    {
        //        //return BadRequest(new
        //        //{
        //        //    status = 0,
        //        //    message = ex.Message,
        //        //    error = ex.ToString()
        //        //});

        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        AddressStockRepo _addressStockRepo;
        CustomerContactRepo _customerContactRepo;
        CustomerEmployeeRepo _customerEmployeeRepo;
        EmployeeRepo _employeeRepo;
        CustomerSpecializationRepo _customerSpecializationRepo;
        BusinessFieldRepo _businessFieldRepo;
        CustomerRepo _customerRepo;
        BusinessFieldLinkRepo _businessFieldLinkRepo;
        public CustomerController(AddressStockRepo addressStockRepo, CustomerContactRepo customerContactRepo, CustomerEmployeeRepo customerEmployeeRepo, EmployeeRepo employeeRepo, CustomerSpecializationRepo customerSpecializationRepo, BusinessFieldRepo businessFieldRepo, CustomerRepo customerRepo, BusinessFieldLinkRepo businessFieldLinkRepo)
        {
            _addressStockRepo = addressStockRepo;
            _customerContactRepo = customerContactRepo;
            _customerEmployeeRepo = customerEmployeeRepo;
            _employeeRepo = employeeRepo;
            _customerSpecializationRepo = customerSpecializationRepo;
            _businessFieldRepo = businessFieldRepo;
            _customerRepo = customerRepo;
            _businessFieldLinkRepo = businessFieldLinkRepo;
        }
        private static string json = System.IO.File.ReadAllText(@"jsonProvinces.txt");
        private static List<Provinces> provinces = JsonConvert.DeserializeObject<List<Provinces>>(json);
        private static List<dynamic> dataExport = new();
        public class Provinces
        {
            public int STT { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
        }

        [HttpGet("get-customers")]
        public IActionResult GetAll()
        {
            try
            {
                List<Customer> customers = _customerRepo.GetAll();

                return Ok(ApiResponseFactory.Success(customers, ""));
            }
            catch (Exception ex)
            {

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-data-by-procedure")]
        [Authorize]
        [RequiresPermission("N1,N27,N53,N31,N69")]
        public IActionResult GetCustomer(int page, int size, int employeeId, int groupId, string? filterText = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetCustomer",
                    new string[] { "@PageNumber", "@PageSize", "@FilterText", "@EmployeeID", "@GroupID" },
                    new object[] { page, size, filterText, employeeId, groupId });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                var data1 = SQLHelper<dynamic>.GetListData(list, 1);
                var data2 = SQLHelper<dynamic>.GetListData(list, 2);
                dataExport = data2;
                return Ok(ApiResponseFactory.Success(new { data, data1, data2 }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-details")]
        [Authorize]
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
        [Authorize]
        public IActionResult GetCustomerSpecialization()
        {
            try
            {
                var data = _customerSpecializationRepo.GetAll().OrderBy(x => x.STT).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-business-field")]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
                return Ok(ApiResponseFactory.Success(new { customerEmployeeWithName, model, provinceCode, customerCode, business, addressStock, customerContact }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        [Authorize]
        public async Task<IActionResult> Save(InsertCustomerDTO dto)
        {
            try
            {
                var validate = dto.Customer;

                List<string> errors = new List<string>();

                if (validate.CustomerName?.Length > 200)
                    errors.Add("Tên khách hàng không được vượt quá 200 ký tự");
                if (string.IsNullOrWhiteSpace(validate.CustomerName))
                    errors.Add("Tên khách hàng là bắt buộc");

                if (validate.CustomerCode?.Length > 30)
                    errors.Add("Mã khách hàng không được vượt quá 30 ký tự");
                if (string.IsNullOrWhiteSpace(validate.CustomerCode))
                    errors.Add("Mã khách hàng là bắt buộc");

                if (validate.CustomerShortName?.Length > 200)
                    errors.Add("Tên viết tắt không được vượt quá 200 ký tự");

                if (validate.Address?.Length > 200)
                    errors.Add("Địa chỉ không được vượt quá 200 ký tự");

                if (validate.Phone?.Length > 100)
                    errors.Add("Số điện thoại (Phone) không được vượt quá 100 ký tự");

                if (validate.Email?.Length > 200)
                    errors.Add("Email không được vượt quá 200 ký tự");

                if (validate.Note?.Length > 255)
                    errors.Add("Ghi chú (Note) không được vượt quá 255 ký tự");

                if (validate.ContactName?.Length > 100)
                    errors.Add("Tên người liên hệ (ContactName) không được vượt quá 100 ký tự");

                if (validate.ContactPhone?.Length > 100)
                    errors.Add("Số điện thoại liên hệ (ContactPhone) không được vượt quá 100 ký tự");

                if (validate.ContactEmail?.Length > 100)
                    errors.Add("Email liên hệ (ContactEmail) không được vượt quá 100 ký tự");

                if (validate.ContactNote?.Length > 300)
                    errors.Add("Ghi chú liên hệ (ContactNote) không được vượt quá 300 ký tự");

                if (validate.NoteDelivery?.Length > 500)
                    errors.Add("Lưu ý giao không được vượt quá 500 ký tự");

                if (validate.NoteVoucher?.Length > 500)
                    errors.Add("Ghi chú hóa đơn (NoteVoucher) không được vượt quá 500 ký tự");

                if (validate.CheckVoucher?.Length > 500)
                    errors.Add("Đầu mối check chứng từ không được vượt quá 500 ký tự");

                if (validate.HardCopyVoucher?.Length > 500)
                    errors.Add("Hard copy voucher không được vượt quá 500 ký tự");

                if (validate.Debt?.Length > 500)
                    errors.Add("Công nợ không được vượt quá 500 ký tự");

                if (validate.AdressStock?.Length > 500)
                    errors.Add("Địa chỉ kho (AdressStock) không được vượt quá 500 ký tự");

                if (validate.TaxCode?.Length > 500)
                    errors.Add("Mã số thuế (TaxCode) không được vượt quá 500 ký tự");

                if (validate.CustomerDetails?.Length > 500)
                    errors.Add("Chi tiết khách hàng (CustomerDetails) không được vượt quá 500 ký tự");

                if (validate.ProductDetails?.Length > 500)
                    errors.Add("Chi tiết sản phẩm (ProductDetails) không được vượt quá 500 ký tự");

                if (validate.Province?.Length > 50)
                    errors.Add("Tỉnh/Thành phố (Province) không được vượt quá 50 ký tự");

                if (errors.Any())
                {
                    var errorMessage = "Dữ liệu không hợp lệ: " + string.Join("; ", errors);
                    return Ok(ApiResponseFactory.Fail(null, errorMessage, new { Errors = errors }));
                }


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
                return Ok(ApiResponseFactory.Success("", "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-multiple")]
        [Authorize]
        public async Task<IActionResult> DeleteMultiple(List<int> ids)
        {
            try
            {
                foreach (int id in ids)
                {
                    Customer customer = _customerRepo.GetByID(id);
                    customer.UpdatedDate = DateTime.Now;
                    customer.IsDeleted = true;
                    await _customerRepo.UpdateAsync(customer);

                }
                return Ok(ApiResponseFactory.Success("", "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("export-excel")]
        [Authorize]
        public IActionResult ExportDataToExcel()
        {
            var columnMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "CustomerName", "Tên khách hàng" },
                { "Address", "Địa chỉ" },
                { "Province", "Tỉnh" },
                { "TypeName", "Loại hình" },
                { "Name", "Ngành" },
                { "ContactName", "Tên liên hệ" },
                { "CustomerPart", "Chức vụ" },
                { "ContactPhone", "ĐT" },
                { "ContactEmail", "Email" },
                { "FullName", "Sales" },
                { "CustomerCode", "Mã khách hàng" },
                { "CustomerShortName", "Tên ký hiệu" },
            };

            try
            {
                if (dataExport == null || !dataExport.Any())
                    return BadRequest("Không có dữ liệu để xuất Excel");

                using (var workbook = new ClosedXML.Excel.XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Data2");

                    // Lấy cột từ phần tử đầu tiên
                    var firstRow = dataExport.First() as IDictionary<string, object>;
                    int colIndex = 1;

                    foreach (var key in firstRow.Keys)
                    {
                        string displayName = columnMap.ContainsKey(key) ? columnMap[key] : key;
                        worksheet.Cell(1, colIndex).Value = displayName;
                        worksheet.Cell(1, colIndex).Style.Font.Bold = true;
                        worksheet.Cell(1, colIndex).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(1, colIndex).Style.Alignment.Vertical = ClosedXML.Excel.XLAlignmentVerticalValues.Center;
                        worksheet.Cell(1, colIndex).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                        worksheet.Cell(1, colIndex).Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                        colIndex++;
                    }

                    // Ghi dữ liệu
                    int rowIndex = 2;
                    foreach (var row in dataExport)
                    {
                        var dict = row as IDictionary<string, object>;
                        colIndex = 1;
                        foreach (var value in dict.Values)
                        {
                            worksheet.Cell(rowIndex, colIndex).Value = value?.ToString();
                            colIndex++;
                        }
                        rowIndex++;
                    }

                    worksheet.Columns().AdjustToContents(); 
                    worksheet.Column(2).Width = 70; 
                    worksheet.Column(3).Width = 70;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.RangeUsed().Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    worksheet.RangeUsed().Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;
                        string fileName = $"DanhSachKhachHang_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                        return File(stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}

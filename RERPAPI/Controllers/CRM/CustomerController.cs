using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.CRM
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        CustomerRepo customerRepo = new CustomerRepo();
        CustomerContactRepo customerContactRepo = new CustomerContactRepo();
        AddressStockRepo addressStockRepo = new AddressStockRepo();
        GroupSaleRepo groupSaleRepo = new GroupSaleRepo();
        CustomerEmployeeRepo customerEmployeeRepo = new CustomerEmployeeRepo();
        BusinessFieldLinkRepo businessFieldLinkRepo = new BusinessFieldLinkRepo();
        EmployeeRepo employeeRepo = new EmployeeRepo();


        [HttpGet("get-customers")]
        public IActionResult GetAll()
        {
            try
            {
                List<Customer> customers = customerRepo.GetAll();
                //return Ok(new
                //{
                //    status = 1,
                //    data = customers
                //});

                return Ok(ApiResponseFactory.Success(customers, ""));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetCustomerByID(int id)
        {
            try
            {
                var customer = customerRepo.GetByID(id);
                //return Ok(new
                //{
                //    status = 1,
                //    data = customer
                //});

                return Ok(ApiResponseFactory.Success(customer, ""));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet()]
        public IActionResult GetCustomer(int groupId, int employeeId, string? filterText, int pageNumber, int pageSize)
        {
            try
            {
                filterText = string.IsNullOrWhiteSpace(filterText) ? " " : filterText;
                var customers = SQLHelper<object>.ProcedureToList("spGetCustomer",
                    new string[] { "@PageNumber", "@PageSize", "@EmployeeID", "@GroupID", "@FilterText" },
                    new object[] { pageNumber, pageSize, employeeId, groupId, filterText ?? "" });
                //return Ok(new
                //{
                //    status = 1,
                //    data = SQLHelper<object>.GetListData(customers, 0)
                //});

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(customers, 0), ""));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("export-excel")]
        public IActionResult GetCustomerToExcel(int groupId, int employeeId, string? filterText, int pageNumber, int pageSize)
        {
            try
            {
                filterText = string.IsNullOrWhiteSpace(filterText) ? " " : filterText;
                var customers = SQLHelper<object>.ProcedureToList("spGetCustomer", new string[] { "@PageNumber", "@PageSize", "@EmployeeID", "@GroupID", "@FilterText" }, new object[] { pageNumber, pageSize, employeeId, groupId, filterText });
                //return Ok(new
                //{
                //    status = 1,
                //    data = SQLHelper<object>.GetListData(customers, 2)
                //});

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(customers, 2), ""));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("{id}/customer-contact")]
        public IActionResult GetCustomerContact(int id)
        {
            try
            {

                var customerContacts = SQLHelper<object>.ProcedureToList("spGetCustomerContactByCustomerID", new string[] { "@CustomerID" }, new object[] { id });

                //return Ok(new
                //{
                //    status = 1,
                //    data = SQLHelper<object>.GetListData(customerContacts, 0)
                //});

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(customerContacts, 0), ""));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}/customer-employee")]
        public IActionResult GetCustomerEmployeeByCustomerID(int id)
        {

            try
            {
                string employeeName = "";
                List<CustomerEmployee> employeeSales = SQLHelper<CustomerEmployee>.FindByAttribute("CustomerID", id);
                //foreach (var employee in employeeSales)
                //{
                //    //employeeName = SQLHelper<Employee>.FindByID((long)employee.EmployeeID).FullName;
                //    employeeName = employeeRepo.GetByID((int)employee.EmployeeID);
                //}
                //return Ok(new
                //{
                //    status = 1,
                //    data = employeeSales.Select(e => new
                //    {
                //        e.ID,
                //        e.CustomerID,
                //        e.EmployeeID,
                //        EmployeeName = employeeRepo.GetByID((int)e.EmployeeID)?.FullName
                //    }).ToList()
                //});


                var data = employeeSales.Select(e => new
                {
                    e.ID,
                    e.CustomerID,
                    e.EmployeeID,
                    EmployeeName = employeeRepo.GetByID((int)e.EmployeeID)?.FullName
                }).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost]
        public async Task<IActionResult> SaveCustomer([FromBody] CustomerDTO request)
        {
            try
            {
                var customer = new Customer
                {
                    ID = request.ID,
                    Province = request.Province,
                    CustomerCode = $"{request.CodeProvinces?.Trim()}-{request.CustomerCode?.Trim()}",
                    CustomerShortName = request.CustomerShortName?.Trim().ToUpper(),
                    CustomerName = request.CustomerName?.Trim(),
                    Address = request.Address?.Trim(),
                    CustomerType = request.CustomerType,
                    ProductDetails = request.ProductDetails?.Trim(),
                    NoteDelivery = request.NoteDelivery?.Trim(),
                    NoteVoucher = request.NoteVoucher?.Trim(),
                    CheckVoucher = request.CheckVoucher?.Trim(),
                    HardCopyVoucher = request.HardCopyVoucher?.Trim(),
                    CustomerSpecializationID = request.CustomerSpecializationID,
                    ClosingDateDebt = request.ClosingDateDebt,
                    Debt = request.Debt?.Trim(),
                    TaxCode = request.TaxCode?.Trim(),
                    IsDeleted = false,
                    BigAccount = request.BigAccount
                };
                if (customer.ID <= 0)
                {
                    await customerRepo.CreateAsync(customer);
                }
                else
                {
                    await customerRepo.UpdateAsync(customer);
                }

                if (request.ID > 0)
                {
                    var existingContacts = SQLHelper<CustomerContact>.FindByAttribute("CustomerID", customer.ID);
                    var contactsToDelete = existingContacts.Where(c => !request.Contacts.Any(rc => rc.idConCus == c.ID)).ToList();
                    foreach (var contact in contactsToDelete)
                    {
                        await customerContactRepo.DeleteAsync(contact.ID);
                    }

                    var existingAddresses = SQLHelper<AddressStock>.FindByAttribute("CustomerID", customer.ID);
                    var addressesToDelete = existingAddresses.Where(a => !request.Addresses.Any(ra => ra.ID == a.ID)).ToList();
                    foreach (var address in addressesToDelete)
                    {
                        await addressStockRepo.DeleteAsync(address.ID);
                    }
                    var existingSales = SQLHelper<CustomerEmployee>.FindByAttribute("CustomerID", customer.ID);
                    var salesToDelete = existingSales.Where(s => !request.Sales.Any(rs => rs.ID == s.ID)).ToList();
                    foreach (var sale in salesToDelete)
                    {
                        await customerEmployeeRepo.DeleteAsync(sale.ID);
                    }
                }


                foreach (var contact in request.Contacts ?? new List<CustomerContactDTO>())
                {
                    var customerContact = new CustomerContact
                    {
                        ID = contact.idConCus,
                        CustomerID = customer.ID,
                        ContactName = contact.ContactName?.Trim(),
                        ContactPhone = contact.ContactPhone?.Trim(),
                        ContactEmail = contact.ContactEmail?.Trim(),
                        CustomerPart = contact.CustomerPart?.Trim(),
                        CustomerPosition = contact.CustomerPosition?.Trim(),
                        CustomerTeam = contact.CustomerTeam?.Trim()
                    };
                    if (customerContact.ID <= 0)
                    {
                        await customerContactRepo.CreateAsync(customerContact);
                    }
                    else
                    {
                        await customerContactRepo.UpdateAsync(customerContact);
                    }
                }



                foreach (var address in request.Addresses ?? new List<CustomerAddressDTO>())
                {
                    var customerAddress = new AddressStock
                    {
                        ID = address.ID,
                        CustomerID = customer.ID,
                        Address = address.Address?.Trim(),
                    };

                    if (customerAddress.ID <= 0)
                    {
                        await addressStockRepo.CreateAsync(customerAddress);
                    }
                    else
                    {
                        await addressStockRepo.UpdateAsync(customerAddress);
                    }
                }

                foreach (var saleEmployee in request.Sales ?? new List<CustomerEmployeeDTO>())
                {
                    var sale = new CustomerEmployee
                    {
                        ID = saleEmployee.ID,
                        CustomerID = customer.ID,
                        EmployeeID = saleEmployee.EmployeeID,
                    };
                    if (sale.ID <= 0)
                    {
                        await customerEmployeeRepo.CreateAsync(sale);
                    }
                    else
                    {
                        await customerEmployeeRepo.UpdateAsync(sale);
                    }
                }

                var business = SQLHelper<BusinessFieldLink>.FindByAttribute("CustomerID", customer.ID).FirstOrDefault() ?? new BusinessFieldLink();

                business.CustomerID = customer.ID;
                business.BusinessFieldID = request.BusinessFieldID;

                if (business.ID <= 0)
                {
                    await businessFieldLinkRepo.CreateAsync(business);
                }
                else
                {
                    await businessFieldLinkRepo.UpdateAsync(business);
                }


                //return Ok(new
                //{
                //    status = 1,
                //    data = customer,
                //    message = "Customer saved successfully."
                //});

                return Ok(ApiResponseFactory.Success(customer, "Customer saved successfully."));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("{customerID}")]
        public async Task<IActionResult> DeleteCustomer(int customerID)
        {
            try
            {
                var customer = customerRepo.GetByID(customerID);
                customer.IsDeleted = true;
                await customerRepo.UpdateAsync(customer);
                //return Ok(new
                //{
                //    status = 1,
                //    message = "Customer deleted successfully."
                //});

                return Ok(ApiResponseFactory.Success(customer, "Customer deleted successfully."));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}/address-stock")]
        public IActionResult GetAddressStockByCustomerID(int id)
        {
            try
            {
                var addressStock = SQLHelper<AddressStock>.FindByAttribute("CustomerID", id);
                //return Ok(new
                //{
                //    status = 1,
                //    data = addressStock
                //});

                return Ok(ApiResponseFactory.Success(addressStock, "Customer deleted successfully."));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}

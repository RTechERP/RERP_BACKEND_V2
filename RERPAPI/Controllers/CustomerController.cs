using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        CustomerRepo customerRepo = new CustomerRepo();
        CustomerContactRepo customerContactRepo = new CustomerContactRepo();
        AddressStockRepo addressStockRepo = new AddressStockRepo();
        GroupSaleRepo groupSaleRepo = new GroupSaleRepo();
        CustomerEmployeeRepo customerEmployeeRepo = new CustomerEmployeeRepo();
        BusinessFieldLinkRepo businessFieldLinkRepo = new BusinessFieldLinkRepo();
        EmployeeRepo employeeRepo = new EmployeeRepo();

        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            try
            {
                List<Customer> customers = customerRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = customers
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

        [HttpGet("getCustomerById")]
        public IActionResult GetCustomerByID(int customerID)
        {
            try
            {
                var customer = customerRepo.GetByID(customerID);
                return Ok(new
                {
                    status = 1,
                    data = customer
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


        [HttpGet("getCustomer")]
        public IActionResult GetCustomer(int groupId, int employeeId, string? filterText, int pageNumber, int pageSize)
        {
            try
            {
                filterText = string.IsNullOrWhiteSpace(filterText) ? " " : filterText;
                var customers = SQLHelper<object>.ProcedureToList("spGetCustomer" ,new string[] { "@PageNumber", "@PageSize", "@EmployeeID", "@GroupID", "@FilterText" }, new object[] {pageNumber, pageSize, employeeId, groupId, filterText });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(customers, 0)
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


        [HttpGet("getCustomerToExcel")]
        public IActionResult GetCustomerToExcel(int groupId, int employeeId, string? filterText, int pageNumber, int pageSize)
        {
            try
            {
                filterText = string.IsNullOrWhiteSpace(filterText) ? " " : filterText;
                var customers = SQLHelper<object>.ProcedureToList("spGetCustomer", new string[] { "@PageNumber", "@PageSize", "@EmployeeID", "@GroupID", "@FilterText" }, new object[] { pageNumber, pageSize, employeeId, groupId, filterText });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(customers, 2)
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


        [HttpGet("getCustomerContact")]
        public IActionResult GetCustomerContact(int customerId)
        {
            try
            {

                var customerContacts = SQLHelper<object>.ProcedureToList("spGetCustomerContactByCustomerID", new string[] { "@CustomerID" }, new object[] { customerId });


                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(customerContacts, 0)
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

        [HttpGet("getCustomerEmployeeByCustomerID")]
        public IActionResult GetCustomerEmployeeByCustomerID(int customerId)
        {

            try
            {
                var employeeName = "";
                List<CustomerEmployee> employeeSales = SQLHelper<CustomerEmployee>.FindByAttribute("CustomerID", customerId);
                //foreach (var employee in employeeSales)
                //{
                //    //employeeName = SQLHelper<Employee>.FindByID((long)employee.EmployeeID).FullName;
                //    employeeName = employeeRepo.GetByID((int)employee.EmployeeID);
                //}
                return Ok(new
                {
                    status = 1,
                    data = employeeSales.Select(e => new
                    {
                        e.ID,
                        e.CustomerID,
                        e.EmployeeID,
                        EmployeeName = employeeRepo.GetByID((int)e.EmployeeID)?.FullName
                    }).ToList()
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


        [HttpPost("saveCustomer")]
        public async Task<IActionResult> SaveCustomer([FromBody]CustomerDTO request)
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
                } else
                {
                    customerRepo.UpdateFieldsByID(customer.ID, customer);
                }
                
                if(request.ID > 0)
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


                foreach(var contact in request.Contacts ?? new List<CustomerContactDTO>())
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
                        customerContactRepo.UpdateFieldsByID(customerContact.ID, customerContact);
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
                        addressStockRepo.UpdateFieldsByID(customerAddress.ID, customerAddress);
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
                        customerEmployeeRepo.UpdateFieldsByID(sale.ID, sale);
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
                

                return Ok(new
                {
                    status = 1,
                    data = customer,
                    message = "Customer saved successfully."
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

        [HttpPut("updateCustomer/{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDTO request)
        {
            try
            {
                //var existingCustomer = SQLHelper<Customer>.FindByID(id);
                var existingCustomer = customerRepo.GetByID(id);
                if (existingCustomer == null)
                {
                    return NotFound(new
                    {
                        status = 0,
                        message = "Customer not found."
                    });
                }

                // Cập nhật thông tin khách hàng
                existingCustomer.Province = request.Province;
                existingCustomer.CustomerCode = $"{request.CodeProvinces?.Trim()}-{request.CustomerCode?.Trim()}";
                existingCustomer.CustomerShortName = request.CustomerShortName?.Trim().ToUpper();
                existingCustomer.CustomerName = request.CustomerName?.Trim();
                existingCustomer.Address = request.Address?.Trim();
                existingCustomer.CustomerType = request.CustomerType;
                existingCustomer.ProductDetails = request.ProductDetails?.Trim();
                existingCustomer.NoteDelivery = request.NoteDelivery?.Trim();
                existingCustomer.NoteVoucher = request.NoteVoucher?.Trim();
                existingCustomer.CheckVoucher = request.CheckVoucher?.Trim();
                existingCustomer.HardCopyVoucher = request.HardCopyVoucher?.Trim();
                existingCustomer.CustomerSpecializationID = request.CustomerSpecializationID;
                existingCustomer.ClosingDateDebt = request.ClosingDateDebt;
                existingCustomer.Debt = request.Debt?.Trim();
                existingCustomer.TaxCode = request.TaxCode?.Trim();
                existingCustomer.BigAccount = request.BigAccount;

                // Cập nhật khách hàng trong database
                await customerRepo.UpdateAsync(existingCustomer);

                // Xử lý contacts
                var existingContacts = SQLHelper<CustomerContact>.FindByAttribute("CustomerID", existingCustomer.ID);
                foreach (var contact in request.Contacts ?? new List<CustomerContactDTO>())
                {
                    var customerContact = existingContacts.FirstOrDefault(c => c.CustomerID == contact.ID) ?? new CustomerContact();
                    customerContact.CustomerID = existingCustomer.ID;
                    customerContact.ContactName = contact.ContactName?.Trim();
                    customerContact.ContactPhone = contact.ContactPhone?.Trim();
                    customerContact.ContactEmail = contact.ContactEmail?.Trim();
                    customerContact.CustomerPart = contact.CustomerPart?.Trim();
                    customerContact.CustomerPosition = contact.CustomerPosition?.Trim();
                    customerContact.CustomerTeam = contact.CustomerTeam?.Trim();

                    if (customerContact.ID <= 0)
                    {
                        await customerContactRepo.CreateAsync(customerContact);
                    }
                    else
                    {
                        await customerContactRepo.UpdateAsync(customerContact);
                    }
                }

                // Xử lý addresses
                var existingAddresses = SQLHelper<AddressStock>.FindByAttribute("CustomerID", existingCustomer.ID);
                foreach (var address in request.Addresses ?? new List<CustomerAddressDTO>())
                {
                    var customerAddress = existingAddresses.FirstOrDefault(a => a.ID == address.ID) ?? new AddressStock();
                    customerAddress.CustomerID = existingCustomer.ID;
                    customerAddress.Address = address.Address?.Trim();

                    if (customerAddress.ID <= 0)
                    {
                        await addressStockRepo.CreateAsync(customerAddress);
                    }
                    else
                    {
                        await addressStockRepo.UpdateAsync(customerAddress);
                    }
                }

                // Xử lý sale employees
                var existingSales = SQLHelper<CustomerEmployee>.FindByAttribute("CustomerID", existingCustomer.ID);
                foreach (var saleEmployee in request.Sales ?? new List<CustomerEmployeeDTO>())
                {
                    var sale = existingSales.FirstOrDefault(s => s.ID == saleEmployee.ID) ?? new CustomerEmployee();
                    sale.CustomerID = existingCustomer.ID;
                    sale.EmployeeID = saleEmployee.EmployeeID;

                    if (sale.ID <= 0)
                    {
                        await customerEmployeeRepo.CreateAsync(sale);
                    }
                    else
                    {
                        await customerEmployeeRepo.UpdateAsync(sale);
                    }
                }

                // Xử lý business field link
                var business = SQLHelper<BusinessFieldLink>.FindByAttribute("CustomerID", existingCustomer.ID).FirstOrDefault() ?? new BusinessFieldLink();
                business.CustomerID = existingCustomer.ID;
                business.BusinessFieldID = request.BusinessFieldID;

                if (business.ID <= 0)
                {
                    await businessFieldLinkRepo.CreateAsync(business);
                }
                else
                {
                    await businessFieldLinkRepo.UpdateAsync(business);
                }

                return Ok(new
                {
                    status = 1,
                    data = existingCustomer,
                    message = "Customer updated successfully."
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



        [HttpDelete("deleteCustomer")]
        public IActionResult DeleteCustomer(int customerID)
        {
            try
            {
               var customer = customerRepo.GetByID(customerID);
                customer.IsDeleted = true;
                customerRepo.Update(customer);
                return Ok(new
                {
                    status = 1,
                    message = "Customer deleted successfully."
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

        [HttpGet("getAddressStockByCustomerID")]
        public IActionResult GetAddressStockByCustomerID(int customerID)
        {
            try
            {
                var addressStock = SQLHelper<AddressStock>.FindByAttribute("CustomerID", customerID);
                return Ok(new
                {
                    status = 1,
                    data = addressStock
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
